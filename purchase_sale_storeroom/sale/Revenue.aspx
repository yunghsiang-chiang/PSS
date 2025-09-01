<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Revenue.aspx.cs" Inherits="purchase_sale_storeroom.sale.Revenue" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/lottie-web/5.12.0/lottie.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">

        <div id="loadingOverlay" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: white; z-index: 9999; display: flex; flex-direction: column; justify-content: center; align-items: center;">
            <div id="lottieContainer" style="width: 200px; height: 200px;"></div>
            <div style="margin-top: 20px; font-size: 18px; color: #666; text-align: center;">光之旅程即將展開...</div>
        </div>


        <div class="container text-center my-4">
            <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control d-inline-block w-auto me-2"></asp:DropDownList>
            <asp:Button ID="btnSearch" runat="server" Text="查詢" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            <asp:Button ID="btnExport" runat="server" Text="匯出 Excel" CssClass="btn btn-success mb-3" OnClick="btnExport_Click" />

        </div>

        <div id="revenueResult" class="container">
            <!-- 實體商品 -->
            <h4>📦 實體商品分潤 (5%)</h4>
            <table class="table table-bordered" id="tblProductShare">
                <thead>
                    <tr>
                        <th>分潤點</th>
                        <th>總金額</th>
                        <th>5% 分潤</th>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>


            <!-- Modal 結構 -->
            <div class="modal fade" id="detailModal" tabindex="-1" aria-labelledby="detailModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="detailModalLabel">商品明細</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <h5 class="mt-4">Top 3 銷售商品（依商品名稱合併）</h5>
                            <table class="table" id="top3Table">
                                <thead>
                                    <tr>
                                        <th>排行</th>
                                        <th>商品名稱</th>
                                        <th>總數量</th>
                                        <th>總金額</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>

                            <h5>商品明細</h5>
                            <table class="table">
                                <thead>
                                    <tr>
                                        <th>商品名稱</th>
                                        <th>商品選項</th>
                                        <th>商品編碼</th>
                                        <th>單價</th>
                                        <th>數量</th>
                                    </tr>
                                </thead>
                                <tbody id="detailTableBody"></tbody>
                            </table>
                        </div>

                    </div>
                </div>
            </div>

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
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            // 載入 Lottie 翻書動畫
            lottie.loadAnimation({
                container: document.getElementById('lottieContainer'),
                renderer: 'svg',
                loop: true,
                autoplay: true,
                path: 'https://assets5.lottiefiles.com/packages/lf20_Ef3PBxZB4a.json' // 替換為你選的動畫 JSON 連結
            });

            // 模擬資料載入（實際使用時替換為你頁面載入完成的 callback）
            setTimeout(() => {
                document.getElementById("loadingOverlay").style.display = "none";
            }, 8000); // 等待 8 秒後結束動畫
        });

    </script>
</body>
</html>
