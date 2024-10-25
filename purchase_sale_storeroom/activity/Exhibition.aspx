<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exhibition.aspx.cs"
    Inherits="purchase_sale_storeroom.activity.Exhibition" %>

    <!DOCTYPE html>

    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>2024 台北國際蔬素食展傳光人打卡</title>
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" rel="stylesheet" />
        <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
        <script src="../Scripts/activity/Exhibition.js"></script>
    </head>

    <body>
        <div class="container my-5">
            <h1 class="mb-4">2024 台北國際蔬素食展<br />傳光人打卡</h1>

            <!-- 光系選擇 -->
            <div class="mb-3">
                <label class="form-label">光系:</label>
                <select class="form-select" id="colorGroup">
                    <option value="gold">金光系</option>
                    <option value="silver">銀光系</option>
                    <option value="pure">純光系</option>
                </select>
            </div>

            <!-- 光色選擇 -->
            <div class="mb-3">
                <label class="form-label">光色:</label>
                <select class="form-select" id="color">
                    <option value="red">紅</option>
                    <option value="orange">橙</option>
                    <option value="yellow">黃</option>
                    <option value="green">綠</option>
                    <option value="blue">藍</option>
                    <option value="indigo">靛</option>
                    <option value="purple">紫</option>
                </select>
            </div>

            <!-- 參與素食展 -->
            <div class="mb-3">
                <label class="form-label">是否參與素食展:</label>
                <div>
                    <input type="radio" name="IsAttendance" value="是" checked="checked" />
                    是
                    <input type="radio" name="IsAttendance" value="否" />
                    否
                </div>
            </div>

            <button class="btn btn-primary" id="submit-btn">提交</button>
        </div>
    </body>

    </html>