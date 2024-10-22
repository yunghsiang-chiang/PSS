<%@ Page Title="" Language="C#" MasterPageFile="~/Attendance.Master" AutoEventWireup="true" CodeBehind="TEST.aspx.cs" Inherits="purchase_sale_storeroom.TEST" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <div class="container my-5">
        <h1 class="mb-4">光系問卷統計 - 堆疊值條圖</h1>
        <canvas id="exhibitionStackedBarChart"></canvas>
    </div>

    <script>
        // 等待文檔準備完畢
        document.addEventListener('DOMContentLoaded', function () {
            // 將後端傳遞的數據用於繪圖
            const colorGroups = exhibitionData.map(d => d.colorGroup);

            const redData = exhibitionData.map(d => d.red);
            const orangeData = exhibitionData.map(d => d.orange);
            const yellowData = exhibitionData.map(d => d.yellow);
            const greenData = exhibitionData.map(d => d.green);
            const blueData = exhibitionData.map(d => d.blue);
            const indigoData = exhibitionData.map(d => d.indigo);
            const purpleData = exhibitionData.map(d => d.purple);

            const ctx = document.getElementById('exhibitionStackedBarChart').getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: colorGroups, // 金光系、銀光系等
                    datasets: [
                        {
                            label: '紅',
                            data: redData,
                            backgroundColor: '#FF0000'
                        },
                        {
                            label: '橙',
                            data: orangeData,
                            backgroundColor: '#FFA500'
                        },
                        {
                            label: '黃',
                            data: yellowData,
                            backgroundColor: '#FFFF00'
                        },
                        {
                            label: '綠',
                            data: greenData,
                            backgroundColor: '#008000'
                        },
                        {
                            label: '藍',
                            data: blueData,
                            backgroundColor: '#0000FF'
                        },
                        {
                            label: '靛',
                            data: indigoData,
                            backgroundColor: '#4B0082'
                        },
                        {
                            label: '紫',
                            data: purpleData,
                            backgroundColor: '#800080'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        x: {
                            stacked: true, // 堆疊
                            title: {
                                display: true,
                                text: '光系'
                            }
                        },
                        y: {
                            stacked: true, // 堆疊
                            title: {
                                display: true,
                                text: '人數'
                            },
                            beginAtZero: true
                        }
                    },
                    plugins: {
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        });
    </script>
</asp:Content>
