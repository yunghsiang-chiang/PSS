using purchase_sale_storeroom.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.IO;


namespace purchase_sale_storeroom.sale
{
    public partial class Revenue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {

                clsDB db = new clsDB();
                string sql = "SELECT ExportID, FORMAT(ExportStartDate, 'yyyy-MM') + ' ~ ' + FORMAT(ExportEndDate, 'MM') AS PeriodText FROM HochiReports.dbo.OrderExportLog ORDER BY ExportStartDate DESC";
                DataTable dt = db.SQL_Select(sql, "HochiSystemConnectionString");
                ddlMonth.DataSource = dt;
                ddlMonth.DataTextField = "PeriodText";
                ddlMonth.DataValueField = "ExportID";
                ddlMonth.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int exportId = int.Parse(ddlMonth.SelectedValue);
            clsDB db = new clsDB();

            string sql = $"SELECT CheckoutPrice, Quantity, ProductCode, ProductName, ProductOption, DistributionPoint FROM HochiReports.dbo.HochiOrders WHERE ExportID = {exportId}";
            DataTable dt = db.SQL_Select(sql, "HochiSystemConnectionString");

            // 取得 ProfitSharing 對應表
            string profitMapSql = "SELECT DistributionPoint, ProfitSharingPoint FROM HochiReports.dbo.ProfitSharing";
            DataTable mapTable = db.SQL_Select(profitMapSql, "HochiSystemConnectionString");

            // 建立對照字典
            Dictionary<string, string> distributionToPoint = new Dictionary<string, string>();
            foreach (DataRow row in mapTable.Rows)
            {
                string from = row["DistributionPoint"]?.ToString() ?? "";
                string to = row["ProfitSharingPoint"]?.ToString() ?? "";
                if (!distributionToPoint.ContainsKey(from))
                    distributionToPoint[from] = string.IsNullOrEmpty(to) ? from : to;
            }

            // 統計商品金額 (根據 ProfitSharingPoint 分組)
            var physicalGoods = new Dictionary<string, decimal>();
            // 儲存每個分潤點對應的商品明細
            Dictionary<string, List<string>> detailMap = new Dictionary<string, List<string>>();
            // 新增 Dictionary 儲存每個分潤點的 Top3 原始資料
            Dictionary<string, Dictionary<string, (int Qty, decimal Total)>> top3Map = new Dictionary<string, Dictionary<string, (int Qty, decimal Total)>>();

            foreach (DataRow row in dt.Rows)
            {
                string productCode = row["ProductCode"]?.ToString() ?? "";
                string distribution = row["DistributionPoint"]?.ToString() ?? "";
                decimal price = row["CheckoutPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CheckoutPrice"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                decimal amount = price * qty;

                // 非書籍才計算
                bool isBook = false;
                string productName = row["ProductName"]?.ToString() ?? "";
                string productOption = row["ProductOption"]?.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(productCode) && productCode.Length >= 2 && !int.TryParse(productCode.Substring(0, 2), out _))
                    isBook = true;
                if (productCode == "-" && !string.IsNullOrWhiteSpace(productOption))
                    isBook = true;
                if (!string.IsNullOrWhiteSpace(productName) && productName.Contains("冊"))
                    isBook = true;

                if (!isBook)
                {
                    string profitPoint = distributionToPoint.ContainsKey(distribution) ? distributionToPoint[distribution] : distribution;
                    if (!physicalGoods.ContainsKey(profitPoint))
                        physicalGoods[profitPoint] = 0;
                    physicalGoods[profitPoint] += amount;

                    string detailRow = $"<tr><td>{HttpUtility.HtmlEncode(productName)}</td><td>{HttpUtility.HtmlEncode(productOption)}</td><td>{HttpUtility.HtmlEncode(productCode)}</td><td>{price}</td><td>{qty}</td></tr>";

                    if (!detailMap.ContainsKey(profitPoint))
                        detailMap[profitPoint] = new List<string>();
                    detailMap[profitPoint].Add(detailRow);

                    if (!top3Map.ContainsKey(profitPoint))
                        top3Map[profitPoint] = new Dictionary<string, (int, decimal)>();

                    if (!top3Map[profitPoint].ContainsKey(productName))
                        top3Map[profitPoint][productName] = (0, 0);

                    top3Map[profitPoint][productName] = (
                        top3Map[profitPoint][productName].Qty + qty,
                        top3Map[profitPoint][productName].Total + price * qty
                    );

                }


            }

            // 輸出實體商品分潤表格
            string js = "<script>$('#tblProductShare tbody').empty();";
            foreach (var item in physicalGoods.OrderByDescending(i => i.Value))
            {
                decimal share = Math.Round(item.Value * 0.05M, 0);
                js += $"$('#tblProductShare tbody').append('<tr><td><a href=\"#\" class=\"profit-point-link\" data-point=\"{item.Key}\">{item.Key}</a></td><td>{item.Value:N0}</td><td>{share:N0}</td></tr>');";

            }

            // 建立商品統計 Dictionary
            var productTopMap = new Dictionary<string, (int TotalQty, decimal TotalAmount)>();

            foreach (DataRow row in dt.Rows)
            {
                string productName = row["ProductName"]?.ToString() ?? "";
                decimal price = row["CheckoutPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CheckoutPrice"]);
                int qty = Convert.ToInt32(row["Quantity"]);

                if (string.IsNullOrWhiteSpace(productName))
                    continue;

                if (!productTopMap.ContainsKey(productName))
                    productTopMap[productName] = (0, 0);

                productTopMap[productName] = (
                    productTopMap[productName].TotalQty + qty,
                    productTopMap[productName].TotalAmount + price * qty
                );
            }




            // 書籍彙總表：以書名為主 key
            var bookSummary = new Dictionary<string, (int Qty, decimal TotalAmount, decimal UnitPrice)>();

            foreach (DataRow row in dt.Rows)
            {
                string productCode = row["ProductCode"]?.ToString() ?? "";
                string productName = row["ProductName"]?.ToString() ?? "";
                string productOption = row["ProductOption"]?.ToString() ?? "";
                decimal price = row["CheckoutPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CheckoutPrice"]);
                int qty = Convert.ToInt32(row["Quantity"]);

                bool isBook = false;
                string bookKey = "";

                if (!string.IsNullOrWhiteSpace(productCode) && productCode.Length >= 2 && !int.TryParse(productCode.Substring(0, 2), out _))
                    isBook = true;

                if (productCode == "-" && !string.IsNullOrWhiteSpace(productOption))
                    isBook = true;

                if (!string.IsNullOrWhiteSpace(productName) && productName.Contains("冊"))
                    isBook = true;

                if (isBook)
                {
                    bookKey = !string.IsNullOrWhiteSpace(productName) ? productName : productOption;
                    if (!bookSummary.ContainsKey(bookKey))
                        bookSummary[bookKey] = (0, 0, price);
                    bookSummary[bookKey] = (
                        bookSummary[bookKey].Qty + qty,
                        bookSummary[bookKey].TotalAmount + price * qty,
                        price
                    );
                }
            }

            // 書籍表格輸出
            js += "$('#tblBookShare tbody').empty();";
            decimal bookTotalAmount = 0;
            decimal bookShareTotal = 0;

            foreach (var book in bookSummary)
            {
                int qty = book.Value.Qty;
                decimal total = book.Value.TotalAmount;
                decimal unit = book.Value.UnitPrice;
                decimal share = Math.Round(total * 0.8M, 0);

                js += $"$('#tblBookShare tbody').append('<tr><td>{book.Key}</td><td>{qty}</td><td>{unit:N0}</td><td>{total:N0}</td><td>{share:N0}</td></tr>');";
                bookTotalAmount += total;
                bookShareTotal += share;
            }

            js += "$(document).off('click', '.profit-point-link');";

            js += "$(document).on('click', '.profit-point-link', function() {" +
                  "var point = $(this).data('point');" +
                  "var rows = detailMap[point] || '';" +
                  "$('#detailTableBody').html(rows);" +
                  "$('#top3Table tbody').html(top3Map[point] || '');" +
                  "$('#detailModal').modal('show');"+
                  "});";


            // 總計列

            js += $"$('#tblBookShare tbody').append('<tr style=\"font-weight:bold;\"><td>總計</td><td></td><td></td><td>{bookTotalAmount:N0}</td><td>{bookShareTotal:N0}</td></tr>');";
            

            js += "var detailMap = {};";
            foreach (var kvp in detailMap)
            {
                string key = kvp.Key.Replace("'", "\\'");
                string value = string.Join("", kvp.Value).Replace("'", "\\'");
                js += $"detailMap['{key}'] = '{value}';";
            }

            js += "$('#top3Table tbody').empty();";
            int rank = 1;
            foreach (var item in productTopMap.OrderByDescending(i => i.Value.TotalAmount).Take(3))
            {
                js += $"$('#top3Table tbody').append('<tr><td>{rank}</td><td>{HttpUtility.JavaScriptStringEncode(item.Key)}</td><td>{item.Value.TotalQty}</td><td>{item.Value.TotalAmount:N0}</td></tr>');";
                rank++;
            }

            js += "var top3Map = {};";

            foreach (var point in top3Map.Keys)
            {
                var list = top3Map[point]
                    .OrderByDescending(p => p.Value.Total)
                    .Take(3)
                    .Select((item, idx) =>
                        $"<tr><td>{idx + 1}</td><td>{HttpUtility.JavaScriptStringEncode(item.Key)}</td><td>{item.Value.Qty}</td><td>{item.Value.Total:N0}</td></tr>"
                    );

                string html = string.Join("", list).Replace("'", "\\'").Replace("\n", "").Replace("\r", "");
                js += $"top3Map['{point.Replace("'", "\\'")}'] = '{html}';";
            }


            js += "</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "updateTable", js);

        }

        // 匯出按鈕事件
        protected void btnExport_Click(object sender, EventArgs e)
        {
            int exportId = int.Parse(ddlMonth.SelectedValue);
            clsDB db = new clsDB();

            string sql = $"SELECT CheckoutPrice, Quantity, ProductCode, ProductName, ProductOption, DistributionPoint FROM HochiReports.dbo.HochiOrders WHERE ExportID = {exportId}";
            DataTable dt = db.SQL_Select(sql, "HochiSystemConnectionString");

            string profitMapSql = "SELECT DistributionPoint, ProfitSharingPoint FROM HochiReports.dbo.ProfitSharing";
            DataTable mapTable = db.SQL_Select(profitMapSql, "HochiSystemConnectionString");
            Dictionary<string, string> distributionToPoint = mapTable.AsEnumerable()
                .GroupBy(r => r.Field<string>("DistributionPoint"))
                .ToDictionary(g => g.Key, g => string.IsNullOrWhiteSpace(g.First()["ProfitSharingPoint"].ToString()) ? g.Key : g.First()["ProfitSharingPoint"].ToString());

            var productTable = new DataTable();
            productTable.Columns.Add("分潤據點");
            productTable.Columns.Add("總金額", typeof(decimal));
            productTable.Columns.Add("分潤金額 (5%)", typeof(decimal));

            var bookTable = new DataTable();
            bookTable.Columns.Add("書名");
            bookTable.Columns.Add("數量", typeof(int));
            bookTable.Columns.Add("單價", typeof(decimal));
            bookTable.Columns.Add("總金額", typeof(decimal));
            bookTable.Columns.Add("分潤金額 (80%)", typeof(decimal));

            var physicalGoods = new Dictionary<string, decimal>();
            var bookSummary = new Dictionary<string, (int Qty, decimal TotalAmount, decimal UnitPrice)>();

            foreach (DataRow row in dt.Rows)
            {
                string productCode = row["ProductCode"]?.ToString() ?? "";
                string distribution = row["DistributionPoint"]?.ToString() ?? "";
                string productName = row["ProductName"]?.ToString() ?? "";
                string productOption = row["ProductOption"]?.ToString() ?? "";
                decimal price = row["CheckoutPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CheckoutPrice"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                decimal amount = price * qty;

                bool isBook = false;
                if (!string.IsNullOrWhiteSpace(productCode) && productCode.Length >= 2 && !int.TryParse(productCode.Substring(0, 2), out _)) isBook = true;
                if (productCode == "-" && !string.IsNullOrWhiteSpace(productOption)) isBook = true;
                if (!string.IsNullOrWhiteSpace(productName) && productName.Contains("冊")) isBook = true;

                if (isBook)
                {
                    string bookKey = !string.IsNullOrWhiteSpace(productName) ? productName : productOption;
                    if (!bookSummary.ContainsKey(bookKey))
                        bookSummary[bookKey] = (0, 0, price);
                    bookSummary[bookKey] = (
                        bookSummary[bookKey].Qty + qty,
                        bookSummary[bookKey].TotalAmount + amount,
                        price
                    );
                }
                else
                {
                    string profitPoint = distributionToPoint.ContainsKey(distribution) ? distributionToPoint[distribution] : distribution;
                    if (!physicalGoods.ContainsKey(profitPoint))
                        physicalGoods[profitPoint] = 0;
                    physicalGoods[profitPoint] += amount;
                }
            }

            foreach (var item in physicalGoods)
            {
                decimal share = Math.Round(item.Value * 0.05M, 0);
                productTable.Rows.Add(item.Key, item.Value, share);
            }

            foreach (var book in bookSummary)
            {
                int qty = book.Value.Qty;
                decimal total = book.Value.TotalAmount;
                decimal unit = book.Value.UnitPrice;
                decimal share = Math.Round(total * 0.8M, 0);
                bookTable.Rows.Add(book.Key, qty, unit, total, share);
            }

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            using (ExcelPackage ep = new ExcelPackage())
            {
                var ws1 = ep.Workbook.Worksheets.Add("實體商品分潤");
                ws1.Cells[1, 1].LoadFromDataTable(productTable, true, TableStyles.Light9);

                var ws2 = ep.Workbook.Worksheets.Add("書籍銷售分潤");
                ws2.Cells[1, 1].LoadFromDataTable(bookTable, true, TableStyles.Light11);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string filename = $"分潤報表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                Response.AddHeader("content-disposition", "attachment;  filename=" + filename);
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }


    }
}