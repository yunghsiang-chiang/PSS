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
  <link href="/Content/mentor.css" rel="stylesheet" />
</head>
<body class="bg-light">
  <form id="form1" runat="server">
    <%-- ScriptManager 一定要在 runat=server 的表單內 --%>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

    <div class="container py-4">
      <h3 class="mb-3">護持者登記（Web Forms 版）</h3>

      <!-- 查詢 -->
      <div class="card mb-3 shadow-sm">
        <div class="card-body">
          <label class="form-label">以姓名查詢</label>
          <div class="input-group">
            <input id="q" class="form-control" placeholder="輸入姓名關鍵字…" />
            <button type="button" class="btn btn-primary" id="btnSearch">查詢</button>
          </div>
          <div class="form-text">顯示：姓名（期別／區屬）</div>
        </div>
      </div>

      <!-- 查詢結果 -->
      <div class="card mb-4 shadow-sm">
        <div class="card-header">查詢結果</div>
        <div class="card-body" id="resultList">
          <div class="text-muted">尚無資料，請先查詢。</div>
        </div>
      </div>

      <!-- 樹狀圖顯示 -->
      <div class="card shadow-sm">
        <div class="card-header">護持者連線（樹狀圖）</div>
        <div class="card-body">
          <div id="tree" class="mentor-tree"></div>
        </div>
      </div>
    </div>

    <!-- 新增護持關係 Modal -->
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
                <input id="selectedMemberDisplay" class="form-control" readonly="readonly" />
              </div>
              <div class="col-md-6">
                <label class="form-label">選擇護持者（導師）</label>
                <div class="input-group">
                  <input id="mentorQuery" class="form-control" placeholder="輸入姓名關鍵字…" />
                  <button type="button" class="btn btn-outline-secondary" id="btnSearchMentor">搜尋</button>
                </div>
                <div id="mentorResult" class="small text-muted mt-2">請先搜尋導師。</div>
              </div>
              <div class="col-md-3">
                <label class="form-label">導師類型</label>
                <select id="mentorType" class="form-select"></select>
              </div>
              <div class="col-md-3">
                <label class="form-label">導師角色</label>
                <select id="mentorRole" class="form-select"></select>
              </div>
              <div class="col-md-4">
                <label class="form-label">開始日期</label>
                <input id="startDate" type="date" class="form-control" />
              </div>
              <div class="col-md-4">
                <label class="form-label">結束日期（可空）</label>
                <input id="endDate" type="date" class="form-control" />
              </div>
              <div class="col-md-4 d-flex align-items-end">
                <div class="form-check">
                  <input class="form-check-input" type="checkbox" id="primaryYN" />
                  <label class="form-check-label" for="primaryYN">主要導師</label>
                </div>
              </div>
              <div class="col-12">
                <label class="form-label">備註</label>
                <textarea id="remark" class="form-control" rows="2" placeholder="（選填）"></textarea>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">取消</button>
            <button type="button" class="btn btn-primary" id="btnSubmitRel">新增</button>
          </div>
        </div>
      </div>
    </div>
  </form>

  <!-- JS：順序很重要 -->
  <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
  <script src="/Scripts/mentor.js"></script>
</body>
</html>
