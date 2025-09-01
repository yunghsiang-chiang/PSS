﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;

namespace purchase_sale_storeroom
{
    public partial class MentorRegister : System.Web.UI.Page
    {
        private string CS => ConfigurationManager.ConnectionStrings["HochiSystem"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTypes();
                BindRoles();
                phNoData.Visible = true;
                txtStartDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        // ========= 查學員 =========
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var dt = QueryMembers(txtQ.Text.Trim());

            if (dt.Rows.Count == 0)
            {
                rptMembers.DataSource = null;
                rptMembers.DataBind();
                phNoData.Visible = true;

                ltTree.Text = "";
                upTree.Update();     // ★ 樹狀圖區塊也跟著清空
                upActive.Update();
                return;
            }

            rptMembers.DataSource = dt;
            rptMembers.DataBind();
            phNoData.Visible = false;

            bool auto = (chkAutoTree != null && chkAutoTree.Checked) || dt.Rows.Count == 1;
            if (auto)
            {
                int firstId = Convert.ToInt32(dt.Rows[0]["HID"]);
                hfSelectedMemberId.Value = firstId.ToString();
                ltTree.Text = BuildTreeHtml(BuildMentorTree(firstId, 0, 6));
                upTree.Update();     // ★ 重新繪製樹
                BindActiveMentors(firstId);  // 內部會 upActive.Update()
            }
            else
            {
                ltTree.Text = "";
                upTree.Update();     // ★ 沒自動載入也先清空
                upActive.Update();
            }
        }


        private DataTable QueryMembers(string q)
        {
            var dt = new DataTable();
            if (string.IsNullOrWhiteSpace(q)) return dt;

            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
SELECT TOP 50 m.HID,
       m.HUserName,
       m.HPeriod,
       a.HArea,
       (m.HUserName + N'（' + ISNULL(m.HPeriod,N'') + CASE WHEN a.HArea IS NULL THEN N')' ELSE N'／'+ a.HArea + N')' END) AS DisplayText
FROM HMember AS m
LEFT JOIN HArea  AS a ON a.HID = m.HAreaID
WHERE m.HStatus = '1' AND m.HUserName LIKE @q
ORDER BY m.HUserName;", conn))
            {
                cmd.Parameters.AddWithValue("@q", "%" + q + "%");
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }
            return dt;
        }

        // ========= 查詢結果：新增 / 查看樹 =========
        protected void rptMembers_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Add")
            {
                var parts = (e.CommandArgument as string ?? "").Split('|');
                if (parts.Length >= 2)
                {
                    hfSelectedMemberId.Value = parts[0];
                    txtSelectedMemberDisplay.Text = parts[1];

                    // 清空上次導師選擇
                    txtMentorQ.Text = "";
                    lblMentorPicked.Text = "";
                    hfSelectedMentorId.Value = "";
                    upMentor.Update();

                    OpenModal();
                }
            }
            else if (e.CommandName == "Tree")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                hfSelectedMemberId.Value = id.ToString();
                ltTree.Text = BuildTreeHtml(BuildMentorTree(id, 0, 6));
                upTree.Update();         // ★ 畫面即時更新樹
                BindActiveMentors(id);   // 會更新 upActive
            }

        }

        // ========= Modal(新增)：導師搜尋 / 選擇 =========
        protected void btnMentorSearch_Click(object sender, EventArgs e)
        {
            var dt = QueryMembers(txtMentorQ.Text.Trim());

            if (int.TryParse(hfSelectedMemberId.Value, out int mid))
            {
                var rows = dt.Select($"HID <> {mid}");
                var dt2 = dt.Clone();
                foreach (var r in rows) dt2.ImportRow(r);
                dt = dt2;
            }

            rptMentors.DataSource = dt;
            rptMentors.DataBind();

            upMentor.Update();
            OpenModal();
        }

        protected void rptMentors_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Pick")
            {
                var parts = (e.CommandArgument as string ?? "").Split('|');
                if (parts.Length >= 2)
                {
                    hfSelectedMentorId.Value = parts[0];
                    lblMentorPicked.Text = "已選擇：" + parts[1];

                    // 保險：同步前端 DOM
                    var js = $"document.getElementById('{hfSelectedMentorId.ClientID}').value='{parts[0]}';";
                    ScriptManager.RegisterStartupScript(this, GetType(), "syncHfMentor", js, true);
                }
                upMentor.Update();
                OpenModal();
            }
        }

        // ========= 新增護持（後台） =========
        protected void btnSubmitRel_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfSelectedMemberId.Value, out int memberId))
            { ShowAlert("請先選擇學員"); OpenModal(); return; }
            if (!int.TryParse(hfSelectedMentorId.Value, out int mentorId))
            { ShowAlert("請先選擇導師"); OpenModal(); return; }
            if (memberId == mentorId)
            { ShowAlert("學員與導師不可為同一人。"); OpenModal(); return; }
            if (CreatesCycle(memberId, mentorId))
            { ShowAlert("此關係將形成循環，請選擇其他導師。"); OpenModal(); return; }

            var typeId = string.IsNullOrEmpty(ddlType.SelectedValue) ? (int?)null : int.Parse(ddlType.SelectedValue);
            var roleId = string.IsNullOrEmpty(ddlRole.SelectedValue) ? (int?)null : int.Parse(ddlRole.SelectedValue);
            DateTime start = string.IsNullOrWhiteSpace(txtStartDate.Text) ? DateTime.Today : DateTime.Parse(txtStartDate.Text);
            DateTime? end = string.IsNullOrWhiteSpace(txtEndDate.Text) ? (DateTime?)null : DateTime.Parse(txtEndDate.Text);
            bool primary = chkPrimary.Checked;
            string remark = string.IsNullOrWhiteSpace(txtRemark.Text) ? null : txtRemark.Text;

            using (var conn = new SqlConnection(CS))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    // 檢查是否已存在重疊有效區間
                    using (var chk = new SqlCommand(@"
SELECT COUNT(1)
FROM HMMentorRelationship
WHERE HMemberID=@m AND HMentorMemberID=@mm AND HStatus=1
  AND (HEndDate IS NULL OR @EndDate IS NULL OR @EndDate >= HStartDate);", conn, tran))
                    {
                        chk.Parameters.AddWithValue("@m", memberId);
                        chk.Parameters.AddWithValue("@mm", mentorId);
                        chk.Parameters.AddWithValue("@EndDate", (object)end ?? DBNull.Value);
                        int exists = (int)chk.ExecuteScalar();
                        if (exists > 0)
                        {
                            tran.Rollback();
                            ShowAlert("已存在相同導師對此學員的有效護持紀錄。");
                            OpenModal();
                            return;
                        }
                    }

                    using (var cmd = new SqlCommand(@"
INSERT INTO HMMentorRelationship
(HMemberID, HMentorMemberID, HMentorTypeID, HMentorRoleID, HStartDate, HEndDate, HPrimaryYN, HRemark, HStatus, HCreate, HCreateDT)
VALUES (@HMemberID, @HMentorMemberID, @HMentorTypeID, @HMentorRoleID, @HStartDate, @HEndDate, @HPrimaryYN, @HRemark, 1, @HCreate, CONVERT(varchar(19), GETDATE(), 120));", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@HMemberID", memberId);
                        cmd.Parameters.AddWithValue("@HMentorMemberID", mentorId);
                        cmd.Parameters.AddWithValue("@HMentorTypeID", (object)typeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HMentorRoleID", (object)roleId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HStartDate", start);
                        cmd.Parameters.AddWithValue("@HEndDate", (object)end ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HPrimaryYN", primary);
                        cmd.Parameters.AddWithValue("@HRemark", (object)remark ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HCreate", (object)(Context?.User?.Identity?.Name ?? "mentor-ui"));

                        cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                }
            }

            ShowAlert("新增成功");
            CloseModal();

            ltTree.Text = BuildTreeHtml(BuildMentorTree(memberId, 0, 6));
            BindActiveMentors(memberId);
        }

        // ========= 目前有效護持列表 =========
        private void BindActiveMentors(int memberId)
        {
            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
SELECT r.HID,
       ment.HUserName,
       ment.HPeriod,
       a.HArea,
       ISNULL(t.HMentorTypeName,'') AS TypeName,
       ISNULL(ro.HMentorRoleName,'') AS RoleName,
       r.HStartDate,
       r.HEndDate,
       r.HPrimaryYN
FROM HMMentorRelationship r
JOIN HMember ment ON ment.HID = r.HMentorMemberID
LEFT JOIN HArea a ON a.HID = ment.HAreaID
LEFT JOIN HMentorType t ON t.HID = r.HMentorTypeID
LEFT JOIN HMentorRole ro ON ro.HID = r.HMentorRoleID
WHERE r.HStatus=1 AND r.HMemberID=@mid
  AND (r.HEndDate IS NULL OR r.HEndDate > CONVERT(date, GETDATE()))
ORDER BY r.HPrimaryYN DESC, r.HStartDate DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@mid", memberId);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    var list = new List<dynamic>();
                    while (rd.Read())
                    {
                        int hid = rd.GetInt32(0);
                        string name = rd.IsDBNull(1) ? "" : rd.GetString(1);
                        string period = rd.IsDBNull(2) ? "" : rd.GetString(2);
                        string area = rd.IsDBNull(3) ? "" : rd.GetString(3);
                        string type = rd.IsDBNull(4) ? "" : rd.GetString(4);
                        string role = rd.IsDBNull(5) ? "" : rd.GetString(5);
                        DateTime start = rd.GetDateTime(6);
                        string endStr = rd.IsDBNull(7) ? "" : rd.GetDateTime(7).ToString("yyyy-MM-dd");
                        bool primary = !rd.IsDBNull(8) && rd.GetBoolean(8);

                        string displayText = $"{name}（{period}" + (string.IsNullOrEmpty(area) ? "）" : $"／{area}）");
                        string displayHtml = displayText + (primary ? " <span class='badge text-bg-info'>主要</span>" : "");

                        list.Add(new
                        {
                            HID = hid,
                            MentorDisplayText = displayText,  // 純文字（給 CommandArgument）
                            MentorDisplayHtml = displayHtml,  // HTML（給 Literal）
                            TypeRole = string.Join("／", new[] { type, role }.Where(s => !string.IsNullOrEmpty(s))),
                            DateRange = start.ToString("yyyy-MM-dd") + (string.IsNullOrEmpty(endStr) ? " ~ " : $" ~ {endStr}")
                        });
                    }
                    rptActive.DataSource = list;
                    rptActive.DataBind();
                }
            }
            upActive.Update();
        }

        // 列表上的「結束」
        protected void rptActive_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "End")
            {
                var parts = (e.CommandArgument as string ?? "").Split('|');
                if (parts.Length >= 1)
                {
                    hfEndRelId.Value = parts[0];
                    txtEndMentorDisplay.Text = parts.Length >= 2 ? parts[1] : "";
                    txtEndDate2.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    txtEndRemark.Text = "";
                }

                upEnd.Update();  // 先把 Modal 的新值送回瀏覽器
                OpenEndModal();  // 再開啟 Modal
            }
        }

        // ========= 結束護持（必填日期與說明，覆蓋 HRemark） =========
        protected void btnEndConfirm_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfEndRelId.Value, out int relId))
            { ShowAlert("找不到要結束的資料。"); return; }

            if (!DateTime.TryParse(txtEndDate2.Text, out DateTime endDate))
            { ShowAlert("請填寫正確的結束日期。"); OpenEndModal(); return; }

            var remark = (txtEndRemark.Text ?? "").Trim();
            if (string.IsNullOrEmpty(remark))
            { ShowAlert("請填寫結束說明（必填）。"); OpenEndModal(); return; }

            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
UPDATE HMMentorRelationship
   SET HEndDate=@endDate,
       HRemark=@remark,
       HModify=@upd,
       HModifyDT=CONVERT(varchar(19), GETDATE(), 120)
 WHERE HID=@id;", conn))
            {
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.Parameters.AddWithValue("@remark", remark);
                cmd.Parameters.AddWithValue("@upd", (object)(Context?.User?.Identity?.Name ?? "mentor-ui"));
                cmd.Parameters.AddWithValue("@id", relId);

                conn.Open();
                int n = cmd.ExecuteNonQuery();
                if (n <= 0) { ShowAlert("更新失敗，請重試。"); OpenEndModal(); return; }
            }

            // ★ 用單一腳本：顯示提示 → 關閉 Modal → 清理 backdrop / body 狀態
            var js = @"
Sys.Application.add_load(function () {
  try {
    alert('已結束護持關係。');
    var el = document.getElementById('endRelModal');
    if (el) {
      var m = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
      m.hide();
    }
    setTimeout(function(){
      document.querySelectorAll('.modal-backdrop').forEach(function(b){ b.remove(); });
      document.body.classList.remove('modal-open');
      document.body.style.removeProperty('padding-right');
    }, 10);
  } catch(ex) {}
});";
            // 更新成功後，直接關閉 Modal，並清理 backdrop
            ScriptManager.RegisterStartupScript(this, GetType(), "endOkHide",
                @"
    (function(){
      alert('已結束護持關係。');
      var el = document.getElementById('endRelModal');
      if (el) {
        try {
          var m = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
          m.hide();
        } catch(e) {}
      }
      // 保險：把殘留 backdrop / body 狀態清掉
      setTimeout(function(){
        try{
          document.querySelectorAll('.modal-backdrop').forEach(function(b){ b.remove(); });
          document.body.classList.remove('modal-open');
          document.body.style.removeProperty('padding-right');
        }catch(ex){}
      }, 10);
    })();", true);

            // 伺服端重繪樹與清單（這裡會觸發 upTree / upActive 的更新）
            if (int.TryParse(hfSelectedMemberId.Value, out int memberId))
            {
                ltTree.Text = BuildTreeHtml(BuildMentorTree(memberId, 0, 6));
                upTree.Update();
                BindActiveMentors(memberId); // 方法內部已有 upActive.Update()
            }
        }



        // ========= 共用：下拉 / Modal / 樹 / 檢查 =========
        private void BindTypes()
        {
            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"SELECT HID, HMentorTypeName FROM HMentorType ORDER BY HSort", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    ddlType.Items.Clear();
                    ddlType.Items.Add(new System.Web.UI.WebControls.ListItem("（無）", ""));
                    while (rd.Read())
                        ddlType.Items.Add(new System.Web.UI.WebControls.ListItem(
                            rd.IsDBNull(1) ? "" : rd.GetString(1),
                            rd.GetInt32(0).ToString()));
                }
            }
        }

        private void BindRoles()
        {
            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"SELECT HID, HMentorRoleName FROM HMentorRole ORDER BY HSort", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    ddlRole.Items.Clear();
                    ddlRole.Items.Add(new System.Web.UI.WebControls.ListItem("（無）", ""));
                    while (rd.Read())
                        ddlRole.Items.Add(new System.Web.UI.WebControls.ListItem(
                            rd.IsDBNull(1) ? "" : rd.GetString(1),
                            rd.GetInt32(0).ToString()));
                }
            }
        }

        private void ShowAlert(string msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(),
                $"alert({System.Web.HttpUtility.JavaScriptStringEncode(msg, true)});", true);
        }

        private void OpenModal()
        {
            var js = @"
Sys.Application.add_load(function () {
  var el = document.getElementById('addRelModal');
  if (!el) return;
  var m = bootstrap.Modal.getOrCreateInstance(el);
  m.show();
});";
            ScriptManager.RegisterStartupScript(this, GetType(), "openModal", js, true);
        }
        private void CloseModal()
        {
            var js = @"
Sys.Application.add_load(function () {
  var el = document.getElementById('addRelModal');
  if (!el) return;
  var m = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
  m.hide();
});";
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", js, true);
        }

        private void OpenEndModal()
        {
            var js = @"
Sys.Application.add_load(function () {
  var el = document.getElementById('endRelModal');
  if (!el) return;
  var m = bootstrap.Modal.getOrCreateInstance(el);
  m.show();
});";
            ScriptManager.RegisterStartupScript(this, GetType(), "openEndModal", js, true);
        }
        private void CloseEndModal()
        {
            var js = @"
Sys.Application.add_load(function () {
  var el = document.getElementById('endRelModal');
  if (!el) return;
  var m = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
  m.hide();
});";
            ScriptManager.RegisterStartupScript(this, GetType(), "closeEndModal", js, true);
        }

        // ===== 樹狀圖 & 循環檢查 =====
        public class TreeNodeDto
        {
            public int Id { get; set; }
            public string Label { get; set; }
            public string Meta { get; set; }
            public List<TreeNodeDto> Children { get; set; } = new List<TreeNodeDto>();
        }

        private bool CreatesCycle(int memberId, int mentorId)
        {
            var visited = new HashSet<int>();
            var q = new Queue<int>(); q.Enqueue(mentorId);
            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
SELECT HMentorMemberID FROM HMMentorRelationship
WHERE HMemberID=@mid AND (HEndDate IS NULL OR HEndDate > CONVERT(date, GETDATE()));", conn))
            {
                conn.Open();
                while (q.Count > 0)
                {
                    var cur = q.Dequeue();
                    if (!visited.Add(cur)) continue;
                    if (cur == memberId) return true;

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@mid", cur);
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read())
                            if (!rd.IsDBNull(0)) q.Enqueue(rd.GetInt32(0));
                }
            }
            return false;
        }

        private TreeNodeDto BuildMentorTree(int memberId, int depth, int maxDepth)
        {
            var node = new TreeNodeDto { Id = memberId, Label = memberId.ToString() };

            using (var conn = new SqlConnection(CS))
            using (var cmdSelf = new SqlCommand(@"
SELECT TOP 1 m.HUserName, m.HPeriod, a.HArea
FROM HMember m
LEFT JOIN HArea a ON a.HID = m.HAreaID
WHERE m.HID=@id;", conn))
            using (var cmdUp = new SqlCommand(@"
SELECT r.HMentorMemberID, ment.HUserName, ment.HPeriod, a.HArea,
ISNULL(t.HMentorTypeName, '') AS TypeName,
ISNULL(r2.HMentorRoleName, '') AS RoleName,
r.HPrimaryYN
FROM HMMentorRelationship r
JOIN HMember ment ON ment.HID = r.HMentorMemberID
LEFT JOIN HArea a ON a.HID = ment.HAreaID
LEFT JOIN HMentorType t ON t.HID = r.HMentorTypeID
LEFT JOIN HMentorRole r2 ON r2.HID = r.HMentorRoleID
WHERE r.HMemberID=@id AND (r.HEndDate IS NULL OR r.HEndDate > CONVERT(date, GETDATE()))
ORDER BY r.HPrimaryYN DESC;", conn))
            {
                conn.Open();

                cmdSelf.Parameters.AddWithValue("@id", memberId);
                using (var rd = cmdSelf.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        var name = rd.IsDBNull(0) ? "" : rd.GetString(0);
                        var period = rd.IsDBNull(1) ? "" : rd.GetString(1);
                        var area = rd.IsDBNull(2) ? null : rd.GetString(2);
                        node.Label = $"{name}（{period}" + (string.IsNullOrEmpty(area) ? ")" : $"／{area})");
                    }
                }

                if (depth >= maxDepth) return node;

                cmdUp.Parameters.AddWithValue("@id", memberId);
                using (var rd = cmdUp.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var upId = rd.GetInt32(0);
                        var upName = rd.IsDBNull(1) ? "" : rd.GetString(1);
                        var upPeriod = rd.IsDBNull(2) ? "" : rd.GetString(2);
                        var upArea = rd.IsDBNull(3) ? null : rd.GetString(3);
                        var typeName = rd.IsDBNull(4) ? "" : rd.GetString(4);
                        var roleName = rd.IsDBNull(5) ? "" : rd.GetString(5);

                        var child = BuildMentorTree(upId, depth + 1, maxDepth);
                        child.Label = $"{upName}（{upPeriod}" + (string.IsNullOrEmpty(upArea) ? ")" : $"／{upArea})");
                        child.Meta = string.Join("／", new[] { typeName, roleName }.Where(s => !string.IsNullOrEmpty(s)));
                        node.Children.Add(child);
                    }
                }
            }
            return node;
        }

        private string BuildTreeHtml(TreeNodeDto node)
        {
            if (node == null) return "<div class='text-muted'>尚無樹狀資料</div>";
            var sb = new System.Text.StringBuilder();
            sb.Append("<ul class='tree-root'>");
            RenderNode(node, sb);
            sb.Append("</ul>");
            return sb.ToString();
        }

        private void RenderNode(TreeNodeDto n, System.Text.StringBuilder sb)
        {
            string meta = string.IsNullOrEmpty(n.Meta) ? "" : $"<span class='badge text-bg-light ms-2'>{System.Web.HttpUtility.HtmlEncode(n.Meta)}</span>";
            sb.Append("<li><span class='node'>");
            sb.Append(System.Web.HttpUtility.HtmlEncode(n.Label));
            sb.Append(meta);
            sb.Append("</span>");
            if (n.Children != null && n.Children.Count > 0)
            {
                sb.Append("<ul>");
                foreach (var c in n.Children) RenderNode(c, sb);
                sb.Append("</ul>");
            }
            sb.Append("</li>");
        }
    }
}
