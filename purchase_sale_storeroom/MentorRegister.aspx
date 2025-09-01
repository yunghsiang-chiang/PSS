<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="MentorRegister.aspx.cs"
    Inherits="purchase_sale_storeroom.MentorRegister"
    ClientIDMode="Static" %>

<!DOCTYPE html>
<html lang="zh-Hant" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>護持者登記（Web Forms）</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- 先載入 Bootstrap JS，確保後台注入腳本可用 -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
    <link href="/Content/mentor.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="container py-4">
            <h3 class="mb-3">護持者登記（Web Forms 版）</h3>

            <!-- 查詢學員 -->
            <div class="card mb-3 shadow-sm">
                <div class="card-body">
                    <label class="form-label">以姓名查詢</label>
                    <div class="input-group">
                        <asp:TextBox ID="txtQ" runat="server" CssClass="form-control" placeholder="輸入姓名關鍵字…" />
                        <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-primary" Text="查詢"
                            OnClick="btnSearch_Click" UseSubmitBehavior="false" />
                    </div>

                    <div class="form-check mt-2">
                        <asp:CheckBox ID="chkAutoTree" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label" for="chkAutoTree">搜尋後自動顯示護持者連線（樹狀圖）</label>
                    </div>

                    <div class="form-text">顯示：姓名（期別／區屬）</div>
                </div>
            </div>

            <!-- 查詢結果 -->
            <div class="card mb-4 shadow-sm">
                <div class="card-header">查詢結果</div>
                <div class="card-body">
                    <asp:Repeater ID="rptMembers" runat="server" OnItemCommand="rptMembers_ItemCommand">
                        <ItemTemplate>
                            <div class="d-flex align-items-center justify-content-between border rounded p-2 mb-2">
                                <div><%# Eval("DisplayText") %></div>
                                <div>
                                    <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-sm btn-outline-primary"
                                        CommandName="Add"
                                        CommandArgument='<%# Eval("HID") + "|" + Eval("DisplayText") %>'>＋ 新增護持關係</asp:LinkButton>
                                    <asp:LinkButton ID="btnTree" runat="server" CssClass="btn btn-sm btn-link"
                                        CommandName="Tree"
                                        CommandArgument='<%# Eval("HID") %>'>查看樹</asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:PlaceHolder ID="phNoData" runat="server" Visible="false">
                        <div class="text-muted">查無資料</div>
                    </asp:PlaceHolder>
                </div>
            </div>

            <!-- 樹狀圖：會在結束後更新 -->
            <asp:UpdatePanel ID="upTree" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card shadow-sm mb-4">
                        <div class="card-header">護持者連線（樹狀圖）</div>
                        <div class="card-body">
                            <asp:Literal ID="ltTree" runat="server" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnEndConfirm" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

            <!-- 目前護持導師（可結束） -->
            <asp:UpdatePanel ID="upActive" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card shadow-sm">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <span>目前護持導師（可結束）</span>
                            <small class="text-muted">列出所選學員當下有效的護持關係</small>
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="rptActive" runat="server" OnItemCommand="rptActive_ItemCommand">
                                <HeaderTemplate>
                                    <div class="row fw-semibold border-bottom pb-2 mb-2">
                                        <div class="col-4">導師</div>
                                        <div class="col-3">類型／角色</div>
                                        <div class="col-3">起訖日期</div>
                                        <div class="col-2 text-end">動作</div>
                                    </div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="row align-items-center py-2 border-bottom">
                                        <!-- 用 Literal 呈現含徽章的 HTML -->
                                        <div class="col-4">
                                            <asp:Literal ID="ltMentor" runat="server" Mode="PassThrough"
                                                Text='<%# Eval("MentorDisplayHtml") %>' />
                                        </div>
                                        <div class="col-3"><%# Eval("TypeRole") %></div>
                                        <div class="col-3"><%# Eval("DateRange") %></div>
                                        <div class="col-2 text-end">
                                            <!-- CommandArgument 傳「純文字」版本 -->
                                            <asp:LinkButton ID="btnEnd" runat="server" CssClass="btn btn-sm btn-outline-danger"
                                                CommandName="End"
                                                CommandArgument='<%# Eval("HID") + "|" + Eval("MentorDisplayText") %>'>結束</asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:PlaceHolder ID="phNoActive" runat="server" Visible="<%# rptActive.Items.Count == 0 %>">
                                        <div class="text-muted">（沒有有效護持關係）</div>
                                    </asp:PlaceHolder>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnEndConfirm" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

        </div>

        <!-- 隱藏欄位（學員） -->
        <asp:HiddenField ID="hfSelectedMemberId" runat="server" />

        <!-- Modal：新增護持關係 -->
        <div class="modal fade" id="addRelModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">新增護持關係</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="row g-3">
                            <div class="col-12">
                                <label class="form-label">被護持者（學員）</label>
                                <asp:TextBox ID="txtSelectedMemberDisplay" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>

                            <!-- 在 Modal 內用 UpdatePanel 做後台查詢導師；把 HiddenField 放進來 -->
                            <asp:UpdatePanel ID="upMentor" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:HiddenField ID="hfSelectedMentorId" runat="server" />

                                    <div class="col-md-6">
                                        <label class="form-label">選擇護持者（導師）</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="txtMentorQ" runat="server" CssClass="form-control" placeholder="輸入姓名關鍵字…" />
                                            <asp:Button ID="btnMentorSearch" runat="server" CssClass="btn btn-outline-secondary"
                                                Text="搜尋" OnClick="btnMentorSearch_Click" UseSubmitBehavior="false" />
                                        </div>
                                        <div class="mt-2">
                                            <asp:Repeater ID="rptMentors" runat="server" OnItemCommand="rptMentors_ItemCommand">
                                                <ItemTemplate>
                                                    <div class="border rounded p-2 mb-2 d-flex justify-content-between align-items-center">
                                                        <div><%# Eval("DisplayText") %></div>
                                                        <asp:LinkButton ID="btnPick" runat="server" CssClass="btn btn-sm btn-outline-success"
                                                            CommandName="Pick"
                                                            CommandArgument='<%# Eval("HID") + "|" + Eval("DisplayText") %>'>選擇</asp:LinkButton>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <asp:Label ID="lblMentorPicked" runat="server" CssClass="text-success"></asp:Label>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <div class="col-md-3">
                                <label class="form-label">導師類型</label>
                                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">導師角色</label>
                                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">開始日期</label>
                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">結束日期（可空）</label>
                                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                            <div class="col-md-4 d-flex align-items-end">
                                <div class="form-check">
                                    <asp:CheckBox ID="chkPrimary" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label" for="chkPrimary">主要導師</label>
                                </div>
                            </div>
                            <div class="col-12">
                                <label class="form-label">備註</label>
                                <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnSubmitRel" runat="server" CssClass="btn btn-primary"
                            Text="新增" OnClick="btnSubmitRel_Click" UseSubmitBehavior="false" />
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">取消</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal：結束護持關係（放在自己的 UpdatePanel 內） -->
        <asp:UpdatePanel ID="upEnd" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <div class="modal fade" id="endRelModal" tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog modal-md modal-dialog-centered">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">結束護持關係</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfEndRelId" runat="server" />
                                <div class="mb-2">
                                    <label class="form-label">導師</label>
                                    <asp:TextBox ID="txtEndMentorDisplay" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="mb-2">
                                    <label class="form-label">結束日期（必填）</label>
                                    <asp:TextBox ID="txtEndDate2" runat="server" CssClass="form-control" TextMode="Date" />
                                </div>
                                <div class="mb-2">
                                    <label class="form-label">結束說明（必填，將覆蓋 HRemark）</label>
                                    <asp:TextBox ID="txtEndRemark" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                                </div>
                                <div class="alert alert-warning small mb-0">
                                    儲存後會將此筆護持關係設為結束（填入 HEndDate）並覆蓋備註（HRemark）。
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnEndConfirm" runat="server" CssClass="btn btn-danger"
                                    Text="確認結束" OnClick="btnEndConfirm_Click" UseSubmitBehavior="false" />
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">取消</button>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>
