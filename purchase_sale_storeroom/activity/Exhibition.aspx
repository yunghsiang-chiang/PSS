<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exhibition.aspx.cs" Inherits="purchase_sale_storeroom.activity.Exhibition" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>素食展服務調查</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="../Scripts/activity/Exhibition.js"></script>
</head>
<body>
    <div class="container my-5">
        <h1 class="mb-4">素食展問卷調查</h1>

        <!-- 光系選擇 -->
        <div class="mb-3">
            <label class="form-label">光系:</label>
            <select class="form-select" id="colorGroup">
                <option value="金光系">金光系</option>
                <option value="銀光系">銀光系</option>
                <option value="純光系">純光系</option>
            </select>
        </div>

        <!-- 光選擇 -->
        <div class="mb-3">
            <label class="form-label">光:</label>
            <select class="form-select" id="color">
                <option value="紅">紅</option>
                <option value="橙">橙</option>
                <option value="黃">黃</option>
                <option value="綠">綠</option>
                <option value="藍">藍</option>
                <option value="靛">靛</option>
                <option value="紫">紫</option>
            </select>
        </div>

        <!-- 參與素食展 -->
        <div class="mb-3">
            <label class="form-label">是否參與素食展:</label>
            <div>
                <input type="radio" name="IsAttendance" value="是" checked>
                是
                <input type="radio" name="IsAttendance" value="否">
                否
            </div>
        </div>

        <button class="btn btn-primary" id="submit-btn">提交</button>
    </div>
</body>
</html>
