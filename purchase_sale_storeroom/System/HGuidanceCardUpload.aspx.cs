using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace purchase_sale_storeroom
{
    public partial class System_HGuidanceCardUpload : Page
    {
        private const string ConnectionStringName = "HochiReports";
        private const int MaxFileSizeBytes = 5 * 1024 * 1024;
        private const string SystemActorName = "GuidanceCardUpload";
        private static readonly string[] RequiredTopicColumns = { "HID", "HTopicDate", "HTopicTitle", "HTopicCode", "HDescription", "HIsPublished", "HStatus", "HCreate", "HCreateDT", "HModify", "HModifyDT" };
        private static readonly string[] RequiredImageColumns = { "HID", "HTopicID", "HLangCode", "HLangName", "HImagePath", "HOriginalFileName", "HContentType", "HFileSizeBytes", "HImageWidth", "HImageHeight", "HStatus", "HCreate", "HCreateDT", "HModify", "HModifyDT" };
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly HashSet<string> AllowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/webp" };
        private static readonly LanguageInfo[] Languages =
        {
            new LanguageInfo("zh-TW", "繁體", "Fu_ZhTW", "Lit_CurrentZhTW"),
            new LanguageInfo("en", "英文", "Fu_En", "Lit_CurrentEn"),
            new LanguageInfo("id", "印尼", "Fu_Id", "Lit_CurrentId"),
            new LanguageInfo("zh-CN", "簡體", "Fu_ZhCN", "Lit_CurrentZhCN")
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!ValidateDatabaseSchema())
                {
                    DisableForm();
                    return;
                }

                BindRecentList();
                LoadEditMode();
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!ValidateDatabaseSchema())
            {
                DisableForm();
                return;
            }

            DateTime topicDate;
            if (!DateTime.TryParse(Txt_TopicDate.Text, out topicDate))
            {
                ShowMessage("請先選擇日期。", "error");
                return;
            }

            int editingTopicId;
            bool isEditing = int.TryParse(Hf_TopicID.Value, out editingTopicId) && editingTopicId > 0;
            if (isEditing)
            {
                topicDate = GetTopicDate(editingTopicId) ?? topicDate;
            }
            List<string> warnings = new List<string>();
            List<UploadFileInfo> uploadedFiles = new List<UploadFileInfo>();

            foreach (LanguageInfo language in Languages)
            {
                FileUpload upload = GetFileUpload(language.UploadControlId);
                if (upload != null && upload.HasFile)
                {
                    UploadFileInfo fileInfo;
                    string error = ValidateUpload(upload, language, topicDate, out fileInfo);
                    if (!String.IsNullOrEmpty(error))
                    {
                        ShowMessage(error, "error");
                        return;
                    }

                    uploadedFiles.Add(fileInfo);
                    if (!IsFourToFive(fileInfo.Width, fileInfo.Height))
                    {
                        warnings.Add("提醒：" + language.Name + "圖片比例不是 4:5，仍已儲存，建議後續更換。");
                    }
                }
                else if (!isEditing)
                {
                    ShowMessage("新增資料時，四個語系圖片都必填；請補上「" + language.Name + "圖片」。", "error");
                    return;
                }
            }

            if (isEditing && uploadedFiles.Count == 0 && String.IsNullOrWhiteSpace(Txt_TopicTitle.Text) && String.IsNullOrWhiteSpace(Txt_Description.Text))
            {
                // Still allow publish-only updates.
            }

            string folderVirtualPath = "uploads/GuidanceCards/" + topicDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "/";
            string folderPhysicalPath = Server.MapPath("~/" + folderVirtualPath);
            Directory.CreateDirectory(folderPhysicalPath);

            List<string> savedPhysicalPaths = new List<string>();
            SqlTransaction transaction = null;

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                try
                {
                    int topicId = UpsertTopic(connection, transaction, topicDate, isEditing ? editingTopicId : 0);

                    foreach (UploadFileInfo uploadedFile in uploadedFiles)
                    {
                        string extension = Path.GetExtension(uploadedFile.OriginalFileName).ToLowerInvariant();
                        string fileName = uploadedFile.Language.Code + extension;
                        string physicalPath = Path.Combine(folderPhysicalPath, fileName);
                        string relativePath = folderVirtualPath + fileName;

                        uploadedFile.Upload.SaveAs(physicalPath);
                        savedPhysicalPaths.Add(physicalPath);

                        UpsertImage(connection, transaction, topicId, uploadedFile, relativePath);
                    }

                    transaction.Commit();
                    ShowMessage("儲存成功。" + (warnings.Count > 0 ? "<br />" + HttpUtility.HtmlEncode(String.Join("\n", warnings)).Replace("\n", "<br />") : String.Empty), warnings.Count > 0 ? "info" : "success");
                    Hf_TopicID.Value = topicId.ToString(CultureInfo.InvariantCulture);
                    Txt_TopicDate.ReadOnly = true;
                    Lit_FormTitle.Text = "編輯圖卡";
                    LoadTopic(topicId);
                    BindRecentList();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    foreach (string path in savedPhysicalPaths)
                    {
                        TryDeleteFile(path);
                    }

                    ShowMessage("儲存失敗：" + HttpUtility.HtmlEncode(ex.Message), "error");
                }
            }
        }

        protected bool IsUploaded(object value)
        {
            return Convert.ToInt32(value == DBNull.Value ? 0 : value, CultureInfo.InvariantCulture) == 1;
        }

        protected bool IsPublished(object value)
        {
            if (value == DBNull.Value || value == null) return false;
            if (value is bool) return (bool)value;
            return Convert.ToInt32(value, CultureInfo.InvariantCulture) == 1;
        }

        private bool ValidateDatabaseSchema()
        {
            try
            {
                Dictionary<string, HashSet<string>> columns = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                using (SqlCommand command = new SqlCommand(@"
SELECT TABLE_NAME, COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('HGuidanceCardTopic', 'HGuidanceCardImage')
ORDER BY TABLE_NAME, ORDINAL_POSITION;", connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = Convert.ToString(reader["TABLE_NAME"]);
                            string columnName = Convert.ToString(reader["COLUMN_NAME"]);
                            if (!columns.ContainsKey(tableName))
                            {
                                columns[tableName] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            }
                            columns[tableName].Add(columnName);
                        }
                    }
                }

                List<string> missing = new List<string>();
                AddMissingColumns(columns, "HGuidanceCardTopic", RequiredTopicColumns, missing);
                AddMissingColumns(columns, "HGuidanceCardImage", RequiredImageColumns, missing);

                if (missing.Count > 0)
                {
                    ShowMessage("資料表欄位與預期不一致，已停止操作，請先確認：<br />" + HttpUtility.HtmlEncode(String.Join("\n", missing)).Replace("\n", "<br />"), "error");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowMessage("無法檢查資料表欄位，已停止操作：" + HttpUtility.HtmlEncode(ex.Message), "error");
                return false;
            }
        }

        private static void AddMissingColumns(Dictionary<string, HashSet<string>> columns, string tableName, IEnumerable<string> requiredColumns, List<string> missing)
        {
            if (!columns.ContainsKey(tableName))
            {
                missing.Add(tableName + " 資料表不存在或無法讀取。");
                return;
            }

            foreach (string requiredColumn in requiredColumns)
            {
                if (!columns[tableName].Contains(requiredColumn))
                {
                    missing.Add(tableName + "." + requiredColumn + " 欄位不存在。");
                }
            }
        }

        private void LoadEditMode()
        {
            int topicId;
            if (int.TryParse(Request.QueryString["id"], out topicId) && topicId > 0)
            {
                LoadTopic(topicId);
            }
        }


        private DateTime? GetTopicDate(int topicId)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand("SELECT TOP 1 HTopicDate FROM dbo.HGuidanceCardTopic WHERE HID = @HID AND HStatus = 1;", connection))
            {
                command.Parameters.Add("@HID", SqlDbType.Int).Value = topicId;
                connection.Open();
                object result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value) return null;
                return Convert.ToDateTime(result, CultureInfo.InvariantCulture);
            }
        }

        private void LoadTopic(int topicId)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand(@"
SELECT TOP 1 HID, HTopicDate, HTopicTitle, HDescription, HIsPublished
FROM dbo.HGuidanceCardTopic
WHERE HID = @HID AND HStatus = 1;", connection))
            {
                command.Parameters.Add("@HID", SqlDbType.Int).Value = topicId;
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ShowMessage("找不到指定的圖卡主題。", "error");
                        return;
                    }

                    Hf_TopicID.Value = topicId.ToString(CultureInfo.InvariantCulture);
                    Txt_TopicDate.Text = Convert.ToDateTime(reader["HTopicDate"], CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Txt_TopicDate.ReadOnly = true;
                    Txt_TopicTitle.Text = Convert.ToString(reader["HTopicTitle"]);
                    Txt_Description.Text = Convert.ToString(reader["HDescription"]);
                    Chk_IsPublished.Checked = IsPublished(reader["HIsPublished"]);
                    Lit_FormTitle.Text = "編輯圖卡";
                }
            }

            BindCurrentImages(topicId);
        }

        private void BindCurrentImages(int topicId)
        {
            Dictionary<string, string> imagePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand(@"
SELECT HLangCode, HImagePath
FROM dbo.HGuidanceCardImage
WHERE HTopicID = @HTopicID AND HStatus = 1;", connection))
            {
                command.Parameters.Add("@HTopicID", SqlDbType.Int).Value = topicId;
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        imagePaths[Convert.ToString(reader["HLangCode"])] = Convert.ToString(reader["HImagePath"]);
                    }
                }
            }

            foreach (LanguageInfo language in Languages)
            {
                Literal literal = GetLiteral(language.CurrentLiteralId);
                if (literal == null) continue;

                string path;
                if (imagePaths.TryGetValue(language.Code, out path) && !String.IsNullOrWhiteSpace(path))
                {
                    literal.Text = "<div class=\"current\">目前：<a href=\"" + ResolveUrl("~/" + path) + "\" target=\"_blank\">已上傳</a></div>";
                }
                else
                {
                    literal.Text = "<div class=\"current\">目前：尚未上傳</div>";
                }
            }
        }

        private void BindRecentList()
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand(@"
SELECT TOP 50
    t.HID,
    t.HTopicDate,
    t.HTopicTitle,
    t.HIsPublished,
    ISNULL(t.HModifyDT, t.HCreateDT) AS LastUpdateTime,
    MAX(CASE WHEN i.HLangCode = 'zh-TW' AND i.HStatus = 1 THEN 1 ELSE 0 END) AS HasZhTW,
    MAX(CASE WHEN i.HLangCode = 'en' AND i.HStatus = 1 THEN 1 ELSE 0 END) AS HasEn,
    MAX(CASE WHEN i.HLangCode = 'id' AND i.HStatus = 1 THEN 1 ELSE 0 END) AS HasId,
    MAX(CASE WHEN i.HLangCode = 'zh-CN' AND i.HStatus = 1 THEN 1 ELSE 0 END) AS HasZhCN
FROM dbo.HGuidanceCardTopic t
LEFT JOIN dbo.HGuidanceCardImage i ON t.HID = i.HTopicID
WHERE t.HStatus = 1
GROUP BY t.HID, t.HTopicDate, t.HTopicTitle, t.HIsPublished, t.HModifyDT, t.HCreateDT
ORDER BY t.HTopicDate DESC;", connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                Gv_Recent.DataSource = table;
                Gv_Recent.DataBind();
            }
        }

        private int UpsertTopic(SqlConnection connection, SqlTransaction transaction, DateTime topicDate, int editingTopicId)
        {
            int topicId = 0;

            if (editingTopicId > 0)
            {
                using (SqlCommand verify = new SqlCommand("SELECT TOP 1 HID FROM dbo.HGuidanceCardTopic WHERE HID = @HID AND HStatus = 1;", connection, transaction))
                {
                    verify.Parameters.Add("@HID", SqlDbType.Int).Value = editingTopicId;
                    object result = verify.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        topicId = Convert.ToInt32(result, CultureInfo.InvariantCulture);
                    }
                }
            }

            if (topicId == 0)
            {
                using (SqlCommand find = new SqlCommand("SELECT TOP 1 HID FROM dbo.HGuidanceCardTopic WHERE HTopicDate = @HTopicDate AND HStatus = 1;", connection, transaction))
                {
                    find.Parameters.Add("@HTopicDate", SqlDbType.Date).Value = topicDate.Date;
                    object result = find.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        topicId = Convert.ToInt32(result, CultureInfo.InvariantCulture);
                    }
                }
            }

            string currentUser = SystemActorName;
            if (topicId > 0)
            {
                using (SqlCommand update = new SqlCommand(@"
UPDATE dbo.HGuidanceCardTopic
SET HTopicTitle = @HTopicTitle,
    HDescription = @HDescription,
    HIsPublished = @HIsPublished,
    HModify = @HModify,
    HModifyDT = GETDATE()
WHERE HID = @HID AND HStatus = 1;", connection, transaction))
                {
                    AddTopicParameters(update, topicDate, currentUser);
                    update.Parameters.Add("@HID", SqlDbType.Int).Value = topicId;
                    update.ExecuteNonQuery();
                }

                return topicId;
            }

            using (SqlCommand insert = new SqlCommand(@"
INSERT INTO dbo.HGuidanceCardTopic
    (HTopicDate, HTopicTitle, HTopicCode, HDescription, HIsPublished, HStatus, HCreate, HCreateDT, HModify, HModifyDT)
OUTPUT INSERTED.HID
VALUES
    (@HTopicDate, @HTopicTitle, @HTopicCode, @HDescription, @HIsPublished, 1, @HModify, GETDATE(), @HModify, GETDATE());", connection, transaction))
            {
                AddTopicParameters(insert, topicDate, currentUser);
                return Convert.ToInt32(insert.ExecuteScalar(), CultureInfo.InvariantCulture);
            }
        }

        private void AddTopicParameters(SqlCommand command, DateTime topicDate, string currentUser)
        {
            command.Parameters.Add("@HTopicDate", SqlDbType.Date).Value = topicDate.Date;
            command.Parameters.Add("@HTopicTitle", SqlDbType.NVarChar, 200).Value = String.IsNullOrWhiteSpace(Txt_TopicTitle.Text) ? (object)DBNull.Value : Txt_TopicTitle.Text.Trim();
            command.Parameters.Add("@HTopicCode", SqlDbType.VarChar, 8).Value = topicDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            command.Parameters.Add("@HDescription", SqlDbType.NVarChar, -1).Value = String.IsNullOrWhiteSpace(Txt_Description.Text) ? (object)DBNull.Value : Txt_Description.Text.Trim();
            command.Parameters.Add("@HIsPublished", SqlDbType.Bit).Value = Chk_IsPublished.Checked;
            command.Parameters.Add("@HModify", SqlDbType.NVarChar, 100).Value = currentUser;
        }

        private void UpsertImage(SqlConnection connection, SqlTransaction transaction, int topicId, UploadFileInfo fileInfo, string relativePath)
        {
            int imageId = 0;
            using (SqlCommand find = new SqlCommand(@"
SELECT TOP 1 HID
FROM dbo.HGuidanceCardImage
WHERE HTopicID = @HTopicID AND HLangCode = @HLangCode AND HStatus = 1;", connection, transaction))
            {
                find.Parameters.Add("@HTopicID", SqlDbType.Int).Value = topicId;
                find.Parameters.Add("@HLangCode", SqlDbType.VarChar, 10).Value = fileInfo.Language.Code;
                object result = find.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    imageId = Convert.ToInt32(result, CultureInfo.InvariantCulture);
                }
            }

            string currentUser = SystemActorName;
            if (imageId > 0)
            {
                using (SqlCommand update = new SqlCommand(@"
UPDATE dbo.HGuidanceCardImage
SET HImagePath = @HImagePath,
    HOriginalFileName = @HOriginalFileName,
    HContentType = @HContentType,
    HFileSizeBytes = @HFileSizeBytes,
    HImageWidth = @HImageWidth,
    HImageHeight = @HImageHeight,
    HModify = @HModify,
    HModifyDT = GETDATE()
WHERE HID = @HID AND HStatus = 1;", connection, transaction))
                {
                    AddImageParameters(update, topicId, fileInfo, relativePath, currentUser);
                    update.Parameters.Add("@HID", SqlDbType.Int).Value = imageId;
                    update.ExecuteNonQuery();
                }
                return;
            }

            using (SqlCommand insert = new SqlCommand(@"
INSERT INTO dbo.HGuidanceCardImage
    (HTopicID, HLangCode, HLangName, HImagePath, HOriginalFileName, HContentType, HFileSizeBytes, HImageWidth, HImageHeight, HStatus, HCreate, HCreateDT, HModify, HModifyDT)
VALUES
    (@HTopicID, @HLangCode, @HLangName, @HImagePath, @HOriginalFileName, @HContentType, @HFileSizeBytes, @HImageWidth, @HImageHeight, 1, @HModify, GETDATE(), @HModify, GETDATE());", connection, transaction))
            {
                AddImageParameters(insert, topicId, fileInfo, relativePath, currentUser);
                insert.ExecuteNonQuery();
            }
        }

        private static void AddImageParameters(SqlCommand command, int topicId, UploadFileInfo fileInfo, string relativePath, string currentUser)
        {
            command.Parameters.Add("@HTopicID", SqlDbType.Int).Value = topicId;
            command.Parameters.Add("@HLangCode", SqlDbType.VarChar, 10).Value = fileInfo.Language.Code;
            command.Parameters.Add("@HLangName", SqlDbType.NVarChar, 20).Value = fileInfo.Language.Name;
            command.Parameters.Add("@HImagePath", SqlDbType.NVarChar, 500).Value = relativePath;
            command.Parameters.Add("@HOriginalFileName", SqlDbType.NVarChar, 255).Value = Path.GetFileName(fileInfo.OriginalFileName);
            command.Parameters.Add("@HContentType", SqlDbType.NVarChar, 100).Value = fileInfo.ContentType;
            command.Parameters.Add("@HFileSizeBytes", SqlDbType.BigInt).Value = fileInfo.FileSizeBytes;
            command.Parameters.Add("@HImageWidth", SqlDbType.Int).Value = fileInfo.Width;
            command.Parameters.Add("@HImageHeight", SqlDbType.Int).Value = fileInfo.Height;
            command.Parameters.Add("@HModify", SqlDbType.NVarChar, 100).Value = currentUser;
        }

        private string ValidateUpload(FileUpload upload, LanguageInfo language, DateTime topicDate, out UploadFileInfo fileInfo)
        {
            fileInfo = null;
            string extension = Path.GetExtension(upload.FileName);
            if (!AllowedExtensions.Contains(extension))
            {
                return language.Name + "圖片副檔名不允許，僅支援 .jpg、.jpeg、.png、.webp。";
            }

            if (upload.PostedFile.ContentLength <= 0 || upload.PostedFile.ContentLength > MaxFileSizeBytes)
            {
                return language.Name + "圖片大小不可超過 5MB。";
            }

            if (!AllowedContentTypes.Contains(upload.PostedFile.ContentType))
            {
                return language.Name + "圖片 ContentType 不允許，僅支援 image/jpeg、image/png、image/webp。";
            }

            try
            {
                using (Stream stream = upload.PostedFile.InputStream)
                {
                    if (stream.CanSeek) stream.Position = 0;
                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(stream))
                    {
                        fileInfo = new UploadFileInfo
                        {
                            Language = language,
                            Upload = upload,
                            OriginalFileName = upload.FileName,
                            ContentType = upload.PostedFile.ContentType,
                            FileSizeBytes = upload.PostedFile.ContentLength,
                            Width = image.Width,
                            Height = image.Height
                        };
                    }
                    if (stream.CanSeek) stream.Position = 0;
                }
            }
            catch
            {
                if (String.Equals(extension, ".webp", StringComparison.OrdinalIgnoreCase))
                {
                    fileInfo = new UploadFileInfo
                    {
                        Language = language,
                        Upload = upload,
                        OriginalFileName = upload.FileName,
                        ContentType = upload.PostedFile.ContentType,
                        FileSizeBytes = upload.PostedFile.ContentLength,
                        Width = 0,
                        Height = 0
                    };
                    return null;
                }

                return language.Name + "圖片無法讀取，請確認檔案格式是否正確。";
            }

            return null;
        }

        private static bool IsFourToFive(int width, int height)
        {
            if (width <= 0 || height <= 0) return false;
            return Math.Abs((width / (double)height) - 0.8d) < 0.01d;
        }

        private FileUpload GetFileUpload(string id)
        {
            switch (id)
            {
                case "Fu_ZhTW": return Fu_ZhTW;
                case "Fu_En": return Fu_En;
                case "Fu_Id": return Fu_Id;
                case "Fu_ZhCN": return Fu_ZhCN;
                default: return null;
            }
        }

        private Literal GetLiteral(string id)
        {
            switch (id)
            {
                case "Lit_CurrentZhTW": return Lit_CurrentZhTW;
                case "Lit_CurrentEn": return Lit_CurrentEn;
                case "Lit_CurrentId": return Lit_CurrentId;
                case "Lit_CurrentZhCN": return Lit_CurrentZhCN;
                default: return null;
            }
        }

        private void DisableForm()
        {
            Btn_Save.Enabled = false;
            Txt_TopicDate.Enabled = false;
            Txt_TopicTitle.Enabled = false;
            Txt_Description.Enabled = false;
            Chk_IsPublished.Enabled = false;
            foreach (LanguageInfo language in Languages)
            {
                FileUpload upload = GetFileUpload(language.UploadControlId);
                if (upload != null) upload.Enabled = false;
            }
        }

        private void ShowMessage(string message, string type)
        {
            Pnl_Message.Visible = true;
            Pnl_Message.CssClass = "message message-" + type;
            Lit_Message.Text = message;
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch
            {
                // Best-effort cleanup only.
            }
        }

        private static string GetConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ConfigurationErrorsException("找不到連線字串：" + ConnectionStringName);
            }
            return settings.ConnectionString;
        }

        private sealed class LanguageInfo
        {
            public LanguageInfo(string code, string name, string uploadControlId, string currentLiteralId)
            {
                Code = code;
                Name = name;
                UploadControlId = uploadControlId;
                CurrentLiteralId = currentLiteralId;
            }

            public string Code { get; private set; }
            public string Name { get; private set; }
            public string UploadControlId { get; private set; }
            public string CurrentLiteralId { get; private set; }
        }

        private sealed class UploadFileInfo
        {
            public LanguageInfo Language { get; set; }
            public FileUpload Upload { get; set; }
            public string OriginalFileName { get; set; }
            public string ContentType { get; set; }
            public long FileSizeBytes { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
