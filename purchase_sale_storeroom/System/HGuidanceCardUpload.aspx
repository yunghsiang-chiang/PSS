<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HGuidanceCardUpload.aspx.cs" Inherits="purchase_sale_storeroom.System_HGuidanceCardUpload" %>

<!DOCTYPE html>
<html lang="zh-TW">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>師指引圖卡上傳</title>
    <style>
        :root { color-scheme: light; --primary:#2563eb; --danger:#dc2626; --muted:#64748b; --border:#dbe3ef; --bg:#f6f8fb; }
        * { box-sizing: border-box; }
        body { margin:0; font-family:"Microsoft JhengHei", Arial, sans-serif; background:var(--bg); color:#172033; }
        .page { max-width:1120px; margin:0 auto; padding:32px 16px 48px; }
        .hero { display:flex; justify-content:space-between; gap:16px; align-items:flex-start; margin-bottom:24px; }
        h1 { margin:0 0 8px; font-size:32px; }
        .hint { margin:0; color:var(--muted); line-height:1.7; }
        .panel { background:#fff; border:1px solid var(--border); border-radius:22px; padding:24px; margin-bottom:22px; box-shadow:0 12px 32px rgba(15,23,42,.06); }
        .grid { display:grid; grid-template-columns:repeat(2, minmax(0, 1fr)); gap:18px; }
        .field { display:flex; flex-direction:column; gap:8px; }
        label, .field-title { font-weight:700; color:#22304a; }
        input[type=text], input[type=date], textarea { width:100%; border:1px solid var(--border); border-radius:12px; padding:11px 12px; font:inherit; }
        textarea { min-height:96px; resize:vertical; }
        .checkline { display:flex; align-items:center; gap:8px; padding-top:30px; }
        .upload-grid { display:grid; grid-template-columns:repeat(4, minmax(0, 1fr)); gap:14px; margin-top:18px; }
        .upload-card { border:1px dashed #b7c5dc; border-radius:16px; padding:16px; background:#fbfdff; min-height:146px; }
        .upload-card small { display:block; color:var(--muted); margin-top:8px; line-height:1.5; }
        .current { margin-top:10px; padding:8px 10px; border-radius:10px; background:#eef6ff; color:#1e40af; font-size:13px; line-height:1.5; }
        .actions { display:flex; gap:12px; align-items:center; margin-top:20px; flex-wrap:wrap; }
        .btn { border:0; border-radius:999px; padding:11px 22px; font-weight:700; cursor:pointer; text-decoration:none; display:inline-block; }
        .btn-primary { background:var(--primary); color:#fff; }
        .btn-secondary { background:#e2e8f0; color:#1e293b; }
        .message { border-radius:14px; padding:14px 16px; margin-bottom:18px; line-height:1.7; }
        .message-success { background:#ecfdf5; color:#047857; border:1px solid #a7f3d0; }
        .message-error { background:#fef2f2; color:#b91c1c; border:1px solid #fecaca; }
        .message-info { background:#eff6ff; color:#1d4ed8; border:1px solid #bfdbfe; }
        table { width:100%; border-collapse:collapse; overflow:hidden; border-radius:16px; }
        th, td { padding:12px 10px; border-bottom:1px solid var(--border); text-align:left; vertical-align:middle; }
        th { background:#f1f5f9; color:#334155; white-space:nowrap; }
        .status-ok { color:#047857; font-weight:700; }
        .status-no { color:#b91c1c; font-weight:700; }
        .table-wrap { overflow-x:auto; }
        @media (max-width: 860px) { .grid, .upload-grid { grid-template-columns:1fr; } .hero { display:block; } }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page">
            <header class="hero">
                <div>
                    <h1>師指引圖卡上傳</h1>
                    <p class="hint">依日期維護四語系圖片；新增時四張必填，編輯時可只重傳需要更新的語系。</p>
                </div>
                <asp:HyperLink ID="Lnk_Browse" runat="server" CssClass="btn btn-secondary" NavigateUrl="~/HGuidanceCardBrowse.aspx" Target="_blank">開啟前台瀏覽頁</asp:HyperLink>
            </header>

            <asp:Panel ID="Pnl_Message" runat="server" Visible="false" CssClass="message">
                <asp:Literal ID="Lit_Message" runat="server" />
            </asp:Panel>

            <section class="panel">
                <h2><asp:Literal ID="Lit_FormTitle" runat="server" Text="新增圖卡" /></h2>
                <asp:HiddenField ID="Hf_TopicID" runat="server" />
                <div class="grid">
                    <div class="field">
                        <asp:Label ID="Lbl_TopicDate" runat="server" AssociatedControlID="Txt_TopicDate" Text="日期（必填）" />
                        <asp:TextBox ID="Txt_TopicDate" runat="server" TextMode="Date" />
                    </div>
                    <div class="field">
                        <asp:Label ID="Lbl_TopicTitle" runat="server" AssociatedControlID="Txt_TopicTitle" Text="主題名稱" />
                        <asp:TextBox ID="Txt_TopicTitle" runat="server" MaxLength="200" />
                    </div>
                    <div class="field">
                        <asp:Label ID="Lbl_Description" runat="server" AssociatedControlID="Txt_Description" Text="備註" />
                        <asp:TextBox ID="Txt_Description" runat="server" TextMode="MultiLine" />
                    </div>
                    <div class="checkline">
                        <asp:CheckBox ID="Chk_IsPublished" runat="server" Text="是否發布到前台" />
                    </div>
                </div>

                <div class="upload-grid">
                    <div class="upload-card">
                        <div class="field-title">繁體圖片</div>
                        <asp:FileUpload ID="Fu_ZhTW" runat="server" />
                        <small>允許 jpg/jpeg/png/webp，單檔 5MB 以內。</small>
                        <asp:Literal ID="Lit_CurrentZhTW" runat="server" />
                    </div>
                    <div class="upload-card">
                        <div class="field-title">英文圖片</div>
                        <asp:FileUpload ID="Fu_En" runat="server" />
                        <small>允許 jpg/jpeg/png/webp，單檔 5MB 以內。</small>
                        <asp:Literal ID="Lit_CurrentEn" runat="server" />
                    </div>
                    <div class="upload-card">
                        <div class="field-title">印尼圖片</div>
                        <asp:FileUpload ID="Fu_Id" runat="server" />
                        <small>允許 jpg/jpeg/png/webp，單檔 5MB 以內。</small>
                        <asp:Literal ID="Lit_CurrentId" runat="server" />
                    </div>
                    <div class="upload-card">
                        <div class="field-title">簡體圖片</div>
                        <asp:FileUpload ID="Fu_ZhCN" runat="server" />
                        <small>允許 jpg/jpeg/png/webp，單檔 5MB 以內。</small>
                        <asp:Literal ID="Lit_CurrentZhCN" runat="server" />
                    </div>
                </div>

                <div class="actions">
                    <asp:Button ID="Btn_Save" runat="server" Text="儲存" CssClass="btn btn-primary" OnClick="Btn_Save_Click" />
                    <asp:HyperLink ID="Lnk_New" runat="server" CssClass="btn btn-secondary" NavigateUrl="~/System/HGuidanceCardUpload.aspx">新增另一筆</asp:HyperLink>
                </div>
            </section>

            <section class="panel">
                <h2>最近資料列表</h2>
                <div class="table-wrap">
                    <asp:GridView ID="Gv_Recent" runat="server" AutoGenerateColumns="False" GridLines="None" EmptyDataText="目前沒有圖卡資料。">
                        <Columns>
                            <asp:BoundField DataField="HTopicDate" HeaderText="日期" DataFormatString="{0:yyyy/MM/dd}" HtmlEncode="false" />
                            <asp:BoundField DataField="HTopicTitle" HeaderText="主題名稱" />
                            <asp:TemplateField HeaderText="繁體"><ItemTemplate><span class='<%# IsUploaded(Eval("HasZhTW")) ? "status-ok" : "status-no" %>'><%# IsUploaded(Eval("HasZhTW")) ? "已上傳" : "未上傳" %></span></ItemTemplate></asp:TemplateField>
                            <asp:TemplateField HeaderText="英文"><ItemTemplate><span class='<%# IsUploaded(Eval("HasEn")) ? "status-ok" : "status-no" %>'><%# IsUploaded(Eval("HasEn")) ? "已上傳" : "未上傳" %></span></ItemTemplate></asp:TemplateField>
                            <asp:TemplateField HeaderText="印尼"><ItemTemplate><span class='<%# IsUploaded(Eval("HasId")) ? "status-ok" : "status-no" %>'><%# IsUploaded(Eval("HasId")) ? "已上傳" : "未上傳" %></span></ItemTemplate></asp:TemplateField>
                            <asp:TemplateField HeaderText="簡體"><ItemTemplate><span class='<%# IsUploaded(Eval("HasZhCN")) ? "status-ok" : "status-no" %>'><%# IsUploaded(Eval("HasZhCN")) ? "已上傳" : "未上傳" %></span></ItemTemplate></asp:TemplateField>
                            <asp:TemplateField HeaderText="是否發布"><ItemTemplate><%# IsPublished(Eval("HIsPublished")) ? "是" : "否" %></ItemTemplate></asp:TemplateField>
                            <asp:BoundField DataField="LastUpdateTime" HeaderText="修改時間" DataFormatString="{0:yyyy/MM/dd HH:mm}" HtmlEncode="false" />
                            <asp:HyperLinkField HeaderText="編輯" Text="編輯" DataNavigateUrlFields="HID" DataNavigateUrlFormatString="~/System/HGuidanceCardUpload.aspx?id={0}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </section>
        </div>
    </form>
</body>
</html>
