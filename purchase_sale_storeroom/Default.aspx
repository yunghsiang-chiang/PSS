<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="purchase_sale_storeroom._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script> 
        var $buoop = { required: { e: -1, f: -1, o: -1, s: -1, c: -1 }, insecure: true, api: 2024.07 };
        function $buo_f() {
            var e = document.createElement("script");
            e.src = "https://browser-update.org/update.min.js";
            document.body.appendChild(e);
        };
        try { document.addEventListener("DOMContentLoaded", $buo_f, false) }
        catch (e) { window.attachEvent("onload", $buo_f) }
    </script>
    <main>
        <section class="row" aria-labelledby="aspnetTitle">
            <h1 id="aspnetTitle">愿同修庫房作業順利</h1>
            <p class="lead">
                庫房作業,系統起到 "輔助"功能<br />
                現場的布置/規劃, 目視管理 才是核心<br />
                共勉之
            </p>
            <p><a href="https://inf.news/zh-hant/career/91029482a638caa06441774b7cae9771.html#google_vignette" class="btn btn-primary btn-md">外部網站:淺談倉庫管理目視化 &raquo;</a></p>
        </section>

        <div class="row">
            <section class="col-md-3" aria-labelledby="gettingStartedTitle">
                <h2 id="gettingStartedTitle">查看庫存</h2>
                <p>
                    慈場倉庫物品數量<br />
                    數量資訊,<b style="color: forestgreen">更新頻率 優異</b>於 日用後台<br />
                </p>
                <p>
                    <a class="btn btn-primary btn-md" href="http://10.10.3.75/storeroom/item_realtime.aspx">Web Link &raquo;</a>
                </p>
            </section>
            <section class="col-md-3" aria-labelledby="librariesTitle">
                <h2 id="librariesTitle">庫房撿料</h2>
                <p>
                    庫房物品,出庫作業<br />
                    若有使用爬蟲<b style="color: forestgreen">自動化</b>列印 每日出貨單<br />
                    搭配自動化,此頁可以看到 出貨單需求資訊
                </p>
                <p>
                    <a class="btn btn-primary btn-mdt" href="http://10.10.3.75/storeroom/materials_out.aspx">Web Link &raquo;</a>
                </p>
            </section>
            <section class="col-md-3" aria-labelledby="hostingTitle">
                <h2 id="hostingTitle">調貨</h2>
                <p>
                    調貨前推薦先使用 查看庫存 確認 慈場現貨資訊<br />
                    再申請調貨作業
                </p>
                <p>
                    <a class="btn btn-primary btn-md" href="http://10.10.3.75/storeroom/transfer_inventory.aspx">Web Link &raquo;</a>
                </p>
            </section>
            <section class="col-md-3" aria-labelledby="newItemTitle">
                <h2 id="newItemTitle">新購入庫</h2>
                <p>
                    新庫入庫 系統會卡控登入者所屬 慈場<br />
                    避免慈場間資料錯亂<br />
                </p>
                <p>
                    <a class="btn btn-primary btn-md" href="http://10.10.3.75/purchase/entry_system.aspx">Web Link &raquo;</a>
                </p>
            </section>
        </div>
        <div class="row">
            <section class="col-md-3" aria-labelledby="top30Title">
                <h2 id="top30Title">歷史庫房作業30筆</h2>
                <p>
                    歷史庫房作業30筆
                </p>
                <p>
                    <a class="btn btn-primary btn-md" href="http://10.10.3.75/storeroom/inout_top30.aspx">Web Link</a>
                </p>
            </section>
            <section class="col-md-3" aria-labelledby="testTitle">
                <h2 id="testTitle">功能測試</h2>
                <p>
                    暫時功能測試區
                </p>
                <p>
                    <a class="btn btn-primary btn-md" href="http://10.10.3.75/temp_test.aspx">Temp_Test</a>
                </p>
            </section>
        </div>
    </main>

</asp:Content>
