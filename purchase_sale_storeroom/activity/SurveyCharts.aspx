<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SurveyCharts.aspx.cs" Inherits="purchase_sale_storeroom.activity.SurveyCharts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>問卷數據視覺化</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="../Scripts/activity/SurveyCharts.js"></script>
</head>
<body>
    <div class="container my-5">
        <h1 class="mb-4 text-center">問卷數據視覺化</h1>

        <div class="row">
            <!-- 社群數據圓餅圖 -->
            <div class="col-12 col-md-6">
                <h3 class="text-center">社群數據</h3>
                <canvas id="socialPieChart"></canvas>
            </div>

            <!-- 年齡範圍直條圖 -->
            <div class="col-12 col-md-6">
                <h3 class="text-center">年齡範圍分布</h3>
                <canvas id="ageBarChart"></canvas>
            </div>
        </div>

        <div class="row mt-5">
            <!-- message_board_color1 和 message_board_color2 分組直條圖 -->
            <div class="col-12">
                <h3 class="text-center">顏色選擇分布</h3>
                <canvas id="colorDistributionBarChart"></canvas>
            </div>
        </div>

        <div class="row mt-5">
            <!-- 填寫問卷的小時分布圖 -->
            <div class="col-12">
                <h3 class="text-center">填寫問卷時間分布</h3>
                <canvas id="submissionTimeLineChart"></canvas>
            </div>
        </div>
    </div>
</body>
</html>
