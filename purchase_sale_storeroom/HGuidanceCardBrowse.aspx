<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HGuidanceCardBrowse.aspx.cs" Inherits="purchase_sale_storeroom.HGuidanceCardBrowse" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Web" %>

<!DOCTYPE html>
<html lang="zh-TW">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>師指引圖卡瀏覽</title>
    <style>
        :root { color-scheme: light; --primary:#2563eb; --muted:#64748b; --border:#dbe3ef; --bg:#f7f5ef; --card:#fffaf1; }
        * { box-sizing:border-box; }
        body { margin:0; font-family:"Microsoft JhengHei", Arial, sans-serif; color:#1f2937; background:linear-gradient(180deg, #fff7e6 0%, #f6f8fb 100%); }
        .page { max-width:980px; margin:0 auto; padding:24px 16px 48px; }
        .topbar { display:flex; justify-content:space-between; align-items:center; gap:16px; margin-bottom:24px; }
        h1 { margin:0 0 8px; font-size:30px; }
        .subtitle { margin:0; color:var(--muted); line-height:1.7; }
        .lang-switch { display:flex; gap:8px; flex-wrap:wrap; justify-content:flex-end; }
        .lang-link { border:1px solid var(--border); background:#fff; color:#334155; border-radius:999px; padding:9px 15px; text-decoration:none; font-weight:700; }
        .lang-link.active { background:var(--primary); color:#fff; border-color:var(--primary); }
        .cards { display:grid; grid-template-columns:repeat(2, minmax(0, 1fr)); gap:18px; }
        .guidance-card { background:#fff; border:1px solid var(--border); border-radius:24px; padding:16px; box-shadow:0 14px 34px rgba(15,23,42,.08); overflow:hidden; }
        .card-date { color:#b45309; font-weight:800; letter-spacing:.04em; margin-bottom:6px; }
        .card-title { font-size:20px; font-weight:800; margin-bottom:14px; min-height:28px; }
        .image-wrap { width:100%; background:var(--card); border-radius:18px; border:1px solid #f3dfb7; overflow:hidden; display:flex; align-items:center; justify-content:center; min-height:240px; }
        .guidance-card-img { width:100%; height:auto; display:block; }
        .copy-btn { margin-top:14px; width:100%; border:0; border-radius:999px; padding:12px 16px; background:#16a34a; color:#fff; font-weight:800; cursor:pointer; font-size:15px; }
        .empty { background:#fff; border:1px solid var(--border); border-radius:22px; padding:28px; text-align:center; color:var(--muted); }
        @media (max-width:760px) { .topbar { display:block; } .lang-switch { justify-content:flex-start; margin-top:16px; } .cards { grid-template-columns:1fr; } }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page">
            <header class="topbar">
                <div>
                    <h1>師指引圖卡</h1>
                    <p class="subtitle">切換語系瀏覽已發布圖卡，並可一鍵複製 LINE 圖片分享碼。</p>
                </div>
                <nav class="lang-switch" aria-label="語系切換">
                    <asp:Repeater ID="Rpt_Languages" runat="server">
                        <ItemTemplate>
                            <a class='<%# GetLanguageCssClass(Eval("Code")) %>' href='<%# "HGuidanceCardBrowse.aspx?lang=" + Eval("Code") %>'><%# Eval("Name") %></a>
                        </ItemTemplate>
                    </asp:Repeater>
                </nav>
            </header>

            <asp:Panel ID="Pnl_Empty" runat="server" Visible="false" CssClass="empty">
                目前沒有可瀏覽的圖卡，請稍後再試。
            </asp:Panel>

            <div class="cards">
                <asp:Repeater ID="Rpt_Cards" runat="server">
                    <ItemTemplate>
                        <article class="guidance-card">
                            <div class="card-date"><%# Eval("HTopicDate", "{0:yyyy/MM/dd}") %></div>
                            <div class="card-title"><%# Eval("HTopicTitle") %></div>
                            <div class="image-wrap">
                                <img src='<%# ResolveImageUrl(Eval("HImagePath")) %>'
                                     alt='<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("HTopicTitle"))) %>'
                                     class="guidance-card-img"
                                     loading="lazy"
                                     onerror="this.style.display='none'; this.parentNode.innerHTML='圖片暫時無法顯示';" />
                            </div>
                            <button type="button" class="copy-btn" onclick='<%# "copyLineShareCode(" + ToJsString(Eval("HLineShareCode")) + ")" %>'>
                                點我複製LINE圖片分享碼
                            </button>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </form>
    <script>
        function copyLineShareCode(text) {
            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(text).then(function () {
                    alert("已複製：" + text);
                }).catch(function () {
                    fallbackCopyText(text);
                });
            } else {
                fallbackCopyText(text);
            }
        }

        function fallbackCopyText(text) {
            var textarea = document.createElement("textarea");
            textarea.value = text;
            textarea.style.position = "fixed";
            textarea.style.left = "-9999px";
            document.body.appendChild(textarea);
            textarea.focus();
            textarea.select();

            try {
                document.execCommand("copy");
                alert("已複製：" + text);
            } catch (err) {
                alert("複製失敗，請手動複製：" + text);
            }

            document.body.removeChild(textarea);
        }
    </script>
</body>
</html>
