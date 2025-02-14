<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Instructor_Training.aspx.cs" Inherits="purchase_sale_storeroom.Instructor_Training" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>講師培訓</title>
    
    <!-- 引入 Bootstrap & jQuery -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" />
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

    <style>
        body {
            background-color: #f8f9fa;
            font-family: "Arial", sans-serif;
        }
        .container {
            margin-top: 20px;
        }
        .section-title {
            font-size: 20px;
            font-weight: bold;
            margin-bottom: 10px;
            padding: 10px;
            background-color: #007bff;
            color: white;
            border-radius: 5px;
        }
        .grid-container {
            background: white;
            padding: 15px;
            border-radius: 5px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            margin-bottom: 20px;
        }
        .grid-container table {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- 備受傳資訊 -->
            <div class="section-title">備受傳資訊</div>
            <div class="grid-container">
                <asp:GridView ID="gv_teach" CssClass="table table-bordered table-striped" runat="server"></asp:GridView>
            </div>

            <!-- 課程參與情況 -->
            <div class="section-title">課程參與情況</div>
            <div class="grid-container">
                <asp:GridView ID="gv_RollCall" CssClass="table table-bordered table-striped" runat="server"></asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
