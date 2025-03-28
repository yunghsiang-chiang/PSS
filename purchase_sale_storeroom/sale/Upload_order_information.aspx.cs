using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using ExcelDataReader;
using System.Configuration;


namespace purchase_sale_storeroom.sale
{
    public partial class Upload_order_information : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {

            if (!fileUpload.HasFile)
            {
                lblResult.Text = "請選擇檔案！";
                uploadResult.Visible = true;
                return;
            }

            string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();

            if (fileExtension != ".xlsx")
            {
                lblResult.Text = "❌ 只允許上傳 .xlsx 檔案，請重新選擇！";
                uploadResult.Visible = true;
                return;
            }


            // 儲存檔案
            string fileName = Path.GetFileName(fileUpload.FileName);
            string tempPath = Server.MapPath("~/TempFiles/");
            string filePath = Path.Combine(tempPath, fileName);
            fileUpload.SaveAs(filePath);

            int totalRows = 0;
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["HochiSystemConnectionString"].ConnectionString;


            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var config = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration { UseHeaderRow = true }
                    };

                    var dataSet = reader.AsDataSet(config);
                    var table = dataSet.Tables[0];

                    // ✅ 驗證欄位格式正確
                    string[] expectedColumns = new string[] {
    "訂單號碼", "訂單日期", "訂單狀態", "預購訂單", "付款方式", "付款日期", "付款狀態", "貨幣",
    "付款訂單號碼", "付款總金額", "訂單小計", "運費", "附加費", "優惠折扣", "折抵購物金", "訂單合計",
    "訂單備註", "送貨方式", "送貨狀態", "收件人", "收件人電話號碼", "地址 1", "地址 2", "城市",
    "地區/州/省份", "國家／地區", "郵政編號（如適用)", "管理員備註", "門市名稱", "統一編號", "發票地址",
    "商品貨號", "商品名稱", "選項", "商品原價", "商品結帳價", "結帳價類型", "數量", "商品類型", "地區/傳光點"
};

                    bool validFormat = expectedColumns.All(col => table.Columns.Contains(col));

                    if (!validFormat)
                    {
                        lblResult.Text = "❌ 上傳失敗：欄位格式不符，請使用正確模板！";
                        uploadResult.Visible = true;
                        return;
                    }


                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();

                        // 插入 ExportLog
                        SqlCommand logCmd = new SqlCommand(@"
                        INSERT INTO HochiReports.dbo.OrderExportLog (SourceSystem, ExportStartDate, ExportEndDate, ExportTime, TotalOrders, FileName)
                        OUTPUT INSERTED.ExportID
                        VALUES (@SourceSystem, @StartDate, @EndDate, GETDATE(), @TotalOrders, @FileName)", conn);

                        logCmd.Parameters.AddWithValue("@SourceSystem", ddlSource.Value);
                        logCmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(startDate.Value));
                        logCmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(endDate.Value));
                        logCmd.Parameters.AddWithValue("@TotalOrders", table.Rows.Count);
                        logCmd.Parameters.AddWithValue("@FileName", fileName);

                        int exportId = (int)logCmd.ExecuteScalar();

                        foreach (DataRow row in table.Rows)
                        {
                            SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO HochiReports.dbo.HochiOrders (
                                OrderID, SourceOrderID, OrderDate, OrderStatus, IsPreOrder, PaymentMethod, PaymentDate,
                                PaymentStatus, Currency, PaymentOrderID, PaymentTotal, Subtotal, ShippingFee, AdditionalFee,
                                Discount, CreditUsed, OrderTotal, OrderNote, ShippingMethod, ShippingStatus, RecipientName,
                                RecipientPhone, Address1, Address2, City, StateOrRegion, Country, PostalCode, AdminNote,
                                StoreName, TaxID, InvoiceAddress, ProductCode, ProductName, ProductOption, ProductPrice,
                                CheckoutPrice, CheckoutPriceType, Quantity, ProductType, DistributionPoint, ExportID
                            ) VALUES (
                                @OrderID, @SourceOrderID, @OrderDate, @OrderStatus, @IsPreOrder, @PaymentMethod, @PaymentDate,
                                @PaymentStatus, @Currency, @PaymentOrderID, @PaymentTotal, @Subtotal, @ShippingFee, @AdditionalFee,
                                @Discount, @CreditUsed, @OrderTotal, @OrderNote, @ShippingMethod, @ShippingStatus, @RecipientName,
                                @RecipientPhone, @Address1, @Address2, @City, @StateOrRegion, @Country, @PostalCode, @AdminNote,
                                @StoreName, @TaxID, @InvoiceAddress, @ProductCode, @ProductName, @ProductOption, @ProductPrice,
                                @CheckoutPrice, @CheckoutPriceType, @Quantity, @ProductType, @DistributionPoint, @ExportID
                            )", conn);

                            cmd.Parameters.AddWithValue("@OrderID", row["訂單號碼"]);
                            cmd.Parameters.AddWithValue("@SourceOrderID", DBNull.Value); // 預設為空
                            cmd.Parameters.AddWithValue("@OrderDate", row["訂單日期"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDateTime(row["訂單日期"]));
                            cmd.Parameters.AddWithValue("@OrderStatus", row["訂單狀態"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@IsPreOrder", row["預購訂單"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PaymentMethod", row["付款方式"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PaymentDate", row["付款日期"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDateTime(row["付款日期"]));
                            cmd.Parameters.AddWithValue("@PaymentStatus", row["付款狀態"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Currency", row["貨幣"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PaymentOrderID", row["付款訂單號碼"] ?? (object)DBNull.Value);

                            cmd.Parameters.AddWithValue("@PaymentTotal", row["付款總金額"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["付款總金額"]));
                            cmd.Parameters.AddWithValue("@Subtotal", row["訂單小計"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["訂單小計"]));
                            cmd.Parameters.AddWithValue("@ShippingFee", row["運費"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["運費"]));
                            cmd.Parameters.AddWithValue("@AdditionalFee", row["附加費"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["附加費"]));
                            cmd.Parameters.AddWithValue("@Discount", row["優惠折扣"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["優惠折扣"]));
                            cmd.Parameters.AddWithValue("@CreditUsed", row["折抵購物金"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["折抵購物金"]));
                            cmd.Parameters.AddWithValue("@OrderTotal", row["訂單合計"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["訂單合計"]));

                            cmd.Parameters.AddWithValue("@OrderNote", row["訂單備註"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShippingMethod", row["送貨方式"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShippingStatus", row["送貨狀態"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@RecipientName", row["收件人"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@RecipientPhone", row["收件人電話號碼"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Address1", row["地址 1"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Address2", row["地址 2"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@City", row["城市"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@StateOrRegion", row["地區/州/省份"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Country", row["國家／地區"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PostalCode", row["郵政編號（如適用)"] ?? (object)DBNull.Value);

                            cmd.Parameters.AddWithValue("@AdminNote", row["管理員備註"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@StoreName", row["門市名稱"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TaxID", row["統一編號"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@InvoiceAddress", row["發票地址"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ProductCode",string.IsNullOrWhiteSpace(row["商品貨號"]?.ToString()) ? "-" : row["商品貨號"].ToString());

                            cmd.Parameters.AddWithValue("@ProductName", row["商品名稱"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ProductOption", row["選項"] ?? (object)DBNull.Value);

                            cmd.Parameters.AddWithValue("@ProductPrice", row["商品原價"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["商品原價"]));
                            cmd.Parameters.AddWithValue("@CheckoutPrice", row["商品結帳價"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(row["商品結帳價"]));
                            cmd.Parameters.AddWithValue("@CheckoutPriceType", row["結帳價類型"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Quantity", row["數量"] == DBNull.Value ? (object)DBNull.Value : Convert.ToInt32(row["數量"]));
                            cmd.Parameters.AddWithValue("@ProductType", row["商品類型"] ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DistributionPoint", row["地區/傳光點"] ?? (object)DBNull.Value);

                            cmd.Parameters.AddWithValue("@ExportID", exportId);


                            cmd.ExecuteNonQuery();
                            totalRows++;
                        }
                    }
                }

                lblResult.Text = $"✅ 匯入成功，共 {totalRows} 筆訂單資料已寫入資料庫。";
                uploadResult.Visible = true;
                gvOrders.DataBind(); // ⬅ 這行會讓 GridView 自動顯示更新結果
            }
            catch (Exception ex)
            {
                lblResult.Text = "❌ 匯入失敗：" + ex.Message;
                uploadResult.Visible = true;
            }
        }
    }
}