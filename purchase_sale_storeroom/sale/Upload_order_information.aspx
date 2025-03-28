<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload_order_information.aspx.cs" Inherits="purchase_sale_storeroom.sale.Upload_order_information" %>

<!DOCTYPE html>
<html lang="zh-Hant">
<head runat="server">
    <title>上傳訂單資料</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</head>
<body>
    <form id="form1" runat="server" class="container mt-5">
        <h3 class="mb-4">上傳訂單報表</h3>

        <div class="row mb-3">
            <div class="col-md-4">
                <label for="ddlSource" class="form-label">來源系統</label>
                <select id="ddlSource" class="form-select" runat="server">
                    <option value="Shopline">Shopline</option>
                    <option value="Other">Other</option>
                </select>
            </div>

            <div class="col-md-4">
                <label for="startDate" class="form-label">起始日期</label>
                <input type="date" id="startDate" class="form-control" runat="server" />
            </div>

            <div class="col-md-4">
                <label for="endDate" class="form-label">結束日期</label>
                <input type="date" id="endDate" class="form-control" runat="server" />
            </div>
        </div>

        <div class="mb-3">
            <label for="fileUpload" class="form-label">選擇訂單報表檔案 <small class="text-danger">(僅接受 .xlsx 檔案)</small></label>
            <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" accept=".xlsx" />
        </div>

        <div class="mb-3">
            <asp:Button ID="btnUpload" runat="server" CssClass="btn btn-primary" Text="上傳並匯入" OnClick="btnUpload_Click" />
        </div>

        <div class="alert alert-info" id="uploadResult" runat="server" visible="false">
            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
        </div>
        <div class="alert alert-info" visible="false">&nbsp;</div>
        <hr />
        <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="ExportID" DataSourceID="SqlDataSource1" AllowPaging="True" AllowSorting="True">
            <Columns>
                <asp:BoundField DataField="ExportID" HeaderText="ExportID" ReadOnly="True" InsertVisible="False" SortExpression="ExportID"></asp:BoundField>
                <asp:BoundField DataField="SourceSystem" HeaderText="SourceSystem" SortExpression="SourceSystem"></asp:BoundField>
                <asp:BoundField DataField="ExportStartDate" HeaderText="ExportStartDate" SortExpression="ExportStartDate"></asp:BoundField>
                <asp:BoundField DataField="ExportEndDate" HeaderText="ExportEndDate" SortExpression="ExportEndDate"></asp:BoundField>
                <asp:BoundField DataField="ExportTime" HeaderText="ExportTime" SortExpression="ExportTime"></asp:BoundField>
                <asp:BoundField DataField="TotalOrders" HeaderText="TotalOrders" SortExpression="TotalOrders"></asp:BoundField>
                <asp:BoundField DataField="FileName" HeaderText="FileName" SortExpression="FileName"></asp:BoundField>
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnectionString %>" SelectCommand="SELECT [ExportID]
      ,[SourceSystem]
      ,[ExportStartDate]
      ,[ExportEndDate]
      ,[ExportTime]
      ,[TotalOrders]
      ,[FileName]
  FROM [HochiReports].[dbo].[OrderExportLog]
  order by ExportStartDate desc"></asp:SqlDataSource>
        </form>
    <script>
        // 你可以加入日期檢查或提示
    </script>
</body>
</html>