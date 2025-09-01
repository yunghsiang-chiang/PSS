<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HMember_Edit.aspx.cs" Inherits="purchase_sale_storeroom.HMember_Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>日期格式驗證範例</title>

    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" rel="stylesheet" />

    <style>
        .validator-error {
            color: red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container mt-4">
        <div class="form-group">
            <label for="TB_HBirth">生日 (格式 yyyy/MM/dd)：</label>
            <asp:TextBox ID="TB_HBirth" CssClass="form-control" runat="server" />
            <asp:RegularExpressionValidator
                ID="revBirthFormat"
                runat="server"
                ControlToValidate="TB_HBirth"
                ValidationExpression="^\d{4}/(0[1-9]|1[0-2])/(0[1-9]|[12]\d|3[01])$"
                ErrorMessage="請輸入正確格式：yyyy/MM/dd"
                Display="Dynamic"
                CssClass="validator-error" />
        </div>

        <asp:Button ID="btnSubmit" runat="server" Text="送出" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
        <asp:Label ID="lblResult" runat="server" CssClass="mt-2 d-block" />
    </form>

    <!-- JS Dependencies -->
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/locales/bootstrap-datepicker.zh-TW.min.js"></script>

    <script>
        $(function () {
            $('#<%= TB_HBirth.ClientID %>').datepicker({
                format: "yyyy/mm/dd",
                language: "zh-TW",
                autoclose: true,
                todayHighlight: true
            });
        });
    </script>
</body>
</html>
