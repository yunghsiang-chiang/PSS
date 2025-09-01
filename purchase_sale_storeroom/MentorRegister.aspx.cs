using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Services;
using System.Web.Script.Services;

namespace purchase_sale_storeroom
{
    public partial class MentorRegister : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private static string CS => ConfigurationManager.ConnectionStrings["HochiSystem"].ConnectionString;

        // DTOs
        public class MemberItemDto
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Period { get; set; }
            public string AreaName { get; set; }
            public string DisplayText { get; set; }
        }


        public class TreeNodeDto
        {
            public int Id { get; set; }
            public string Label { get; set; }
            public string Meta { get; set; }
            public List<TreeNodeDto> Children { get; set; } = new List<TreeNodeDto>();
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<MemberItemDto> SearchMembers(string q)
        {
            var list = new List<MemberItemDto>();
            if (string.IsNullOrWhiteSpace(q)) return list;
            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"SELECT TOP 50 m.HID, m.HUserName, m.HPeriod, a.HArea1
FROM HMember AS m
LEFT JOIN HArea AS a ON a.HID = m.HAreaID
WHERE m.HStatus = '1' AND m.HUserName LIKE @q
ORDER BY m.HUserName", conn))
            {
                cmd.Parameters.AddWithValue("@q", "%" + q + "%");
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var id = rd.GetInt32(0);
                        var name = rd.IsDBNull(1) ? "" : rd.GetString(1);
                        var period = rd.IsDBNull(2) ? "" : rd.GetString(2);
                        var area = rd.IsDBNull(3) ? null : rd.GetString(3);
                        var disp = $"{name}（{period}" + (string.IsNullOrEmpty(area) ? ")" : $"／{area})");
                        list.Add(new MemberItemDto { Id = id, UserName = name, Period = period, AreaName = area, DisplayText = disp });
                    }
                }
            }
            return list;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<object> GetTypes()
        {
            var list = new List<object>();
            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"SELECT HID, HMentorTypeName FROM HMentorType ORDER BY HSort", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new { hid = rd.GetInt32(0), hmentorTypeName = rd.IsDBNull(1) ? "" : rd.GetString(1) });
                    }
                }
            }
            return list;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<object> GetRoles()
        {
            var list = new List<object>();
            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"SELECT HID, HMentorRoleName FROM HMentorRole ORDER BY HSort", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new { hid = rd.GetInt32(0), hmentorRoleName = rd.IsDBNull(1) ? "" : rd.GetString(1) });
                    }
                }
            }
            return list;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object CreateRelationship(int hMemberID, int hMentorMemberID, int? hMentorTypeID, int? hMentorRoleID,
string hStartDate, string hEndDate, bool hPrimaryYN, string hRemark)
        {
            if (hMemberID == hMentorMemberID)
                throw new ApplicationException("學員與導師不可為同一人。");


            if (CreatesCycle(hMemberID, hMentorMemberID))
                throw new ApplicationException("此關係將形成循環，請選擇其他導師。");


            using (var conn = new SqlConnection(CS))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    // 重複檢查（期間重疊 or 未結束）
                    using (var chk = new SqlCommand(@"SELECT COUNT(1)
FROM HMMentorRelationship
WHERE HMemberID=@m AND HMentorMemberID=@mm
AND (HEndDate IS NULL OR @EndDate IS NULL OR @EndDate >= HStartDate)", conn, tran))
                    {
                        chk.Parameters.AddWithValue("@m", hMemberID);
                        chk.Parameters.AddWithValue("@mm", hMentorMemberID);
                        chk.Parameters.AddWithValue("@EndDate", string.IsNullOrWhiteSpace(hEndDate) ? (object)DBNull.Value : DateTime.Parse(hEndDate));
                        var exists = (int)chk.ExecuteScalar() > 0;
                        if (exists) throw new ApplicationException("已存在相同導師對此學員的有效護持紀錄。");
                    }


                    using (var cmd = new SqlCommand(@"INSERT INTO HMMentorRelationship
(HMemberID, HMentorMemberID, HMentorTypeID, HMentorRoleID,
HStartDate, HEndDate, HPrimaryYN, HRemark, HStatus, HCreate, HCreateDT)
VALUES (@HMemberID, @HMentorMemberID, @HMentorTypeID, @HMentorRoleID,
@HStartDate, @HEndDate, @HPrimaryYN, @HRemark, 1, @HCreate, CONVERT(varchar(19), GETDATE(), 120));
SELECT SCOPE_IDENTITY();", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@HMemberID", hMemberID);
                        cmd.Parameters.AddWithValue("@HMentorMemberID", hMentorMemberID);
                        cmd.Parameters.AddWithValue("@HMentorTypeID", (object)hMentorTypeID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HMentorRoleID", (object)hMentorRoleID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HStartDate", string.IsNullOrWhiteSpace(hStartDate) ? DateTime.Today : DateTime.Parse(hStartDate));
                        cmd.Parameters.AddWithValue("@HEndDate", string.IsNullOrWhiteSpace(hEndDate) ? (object)DBNull.Value : DateTime.Parse(hEndDate));
                        cmd.Parameters.AddWithValue("@HPrimaryYN", hPrimaryYN);
                        cmd.Parameters.AddWithValue("@HRemark", string.IsNullOrWhiteSpace(hRemark) ? (object)DBNull.Value : hRemark);
                        cmd.Parameters.AddWithValue("@HCreate", (object)(System.Web.HttpContext.Current?.User?.Identity?.Name ?? "mentor-ui"));


                        var id = Convert.ToInt32(Math.Round(Convert.ToDecimal(cmd.ExecuteScalar())));
                        tran.Commit();
                        return new { ok = true, id };
                    }
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static TreeNodeDto GetTree(int memberId)
        {
            return BuildMentorTree(memberId, 0, 6);
        }


        private static bool CreatesCycle(int memberId, int mentorId)
        {
            var visited = new HashSet<int>();
            var queue = new Queue<int>();
            queue.Enqueue(mentorId);
            using (var conn = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"SELECT HMentorMemberID FROM HMMentorRelationship
WHERE HMemberID=@mid AND (HEndDate IS NULL OR HEndDate >= CONVERT(date, GETDATE()))", conn))
            {
                conn.Open();
                while (queue.Count > 0)
                {
                    var cur = queue.Dequeue();
                    if (!visited.Add(cur)) continue;
                    if (cur == memberId) return true;


                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@mid", cur);
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            if (!rd.IsDBNull(0)) queue.Enqueue(rd.GetInt32(0));
                        }
                    }
                }
            }
            return false;
        }

        private static TreeNodeDto BuildMentorTree(int memberId, int depth, int maxDepth)
        {
            var node = new TreeNodeDto { Id = memberId, Label = memberId.ToString() };


            using (var conn = new SqlConnection(CS))
            using (var cmdSelf = new SqlCommand(@"SELECT TOP 1 m.HUserName, m.HPeriod, a.HArea1
FROM HMember m
LEFT JOIN HArea a ON a.HID = m.HAreaID
WHERE m.HID=@id", conn))
            using (var cmdUp = new SqlCommand(@"SELECT r.HMentorMemberID, ment.HUserName, ment.HPeriod, a.HArea1,
ISNULL(t.HMentorTypeName, '') AS TypeName,
ISNULL(r2.HMentorRoleName, '') AS RoleName,
r.HPrimaryYN
FROM HMMentorRelationship r
JOIN HMember ment ON ment.HID = r.HMentorMemberID
LEFT JOIN HArea a ON a.HID = ment.HAreaID
LEFT JOIN HMentorType t ON t.HID = r.HMentorTypeID
LEFT JOIN HMentorRole r2 ON r2.HID = r.HMentorRoleID
WHERE r.HMemberID=@id AND (r.HEndDate IS NULL OR r.HEndDate >= CONVERT(date, GETDATE()))
ORDER BY r.HPrimaryYN DESC", conn))
            {
                conn.Open();


                // self label
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


                // uppers
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
                        var meta = string.Join("／", new[] { typeName, roleName }.Where(s => !string.IsNullOrEmpty(s)));
                        child.Meta = meta;
                        node.Children.Add(child);
                    }
                }
            }
            return node;
        }

    }
}


