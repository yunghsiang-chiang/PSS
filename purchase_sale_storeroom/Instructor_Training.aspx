<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Instructor_Training.aspx.cs" Inherits="purchase_sale_storeroom.Instructor_Training" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            備受傳資訊
        </div>
        <div>
            <asp:GridView ID="gv_teach" runat="server"></asp:GridView>
        </div>
        <div>
            課程參與情況
        </div>
        <div>
            <asp:GridView ID="gv_RollCall" runat="server"></asp:GridView>
        </div>
    </form>
</body>
</html>
