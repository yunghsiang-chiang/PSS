﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Revenue.aspx.cs" Inherits="purchase_sale_storeroom.sale.Revenue" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control mb-3"></asp:DropDownList>
        <asp:Button ID="btnSearch" runat="server" Text="查詢" CssClass="btn btn-primary mb-3" OnClick="btnSearch_Click" />

        <div id="revenueResult" class="container">
            <!-- 實體商品 -->
            <h4>📦 實體商品分潤 (5%)</h4>
            <table class="table table-bordered" id="tblProductShare">
                <thead>
                    <tr>
                        <th>配送地點</th>
                        <th>總金額</th>
                        <th>5% 分潤</th>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>

            <!-- 書籍 -->
            <h4>📚 書籍銷售分潤 (80%)</h4>
            <table class="table table-bordered" id="tblBookShare">
                <thead>
                    <tr>
                        <th>書名</th>
                        <th>數量</th>
                        <th>單價</th>
                        <th>總金額</th>
                        <th>分潤金額 (80%)</th>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>
        </div>

    </form>
</body>
</html>
