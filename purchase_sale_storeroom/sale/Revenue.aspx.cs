using purchase_sale_storeroom.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                }
            }

            // 輸出實體商品分潤表格
            string js = "<script>$('#tblProductShare tbody').empty();";
            foreach (var item in physicalGoods.OrderByDescending(i => i.Value))
            {
                decimal share = Math.Round(item.Value * 0.05M, 0);
                js += $"$('#tblProductShare tbody').append('<tr><td>{item.Key}</td><td>{item.Value:N0}</td><td>{share:N0}</td></tr>');";
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

            // 總計列
            js += $"$('#tblBookShare tbody').append('<tr style=\"font-weight:bold;\"><td>總計</td><td></td><td></td><td>{bookTotalAmount:N0}</td><td>{bookShareTotal:N0}</td></tr>');";
            js += "</script>";

            ClientScript.RegisterStartupScript(this.GetType(), "updateTable", js);

        }

    }
}