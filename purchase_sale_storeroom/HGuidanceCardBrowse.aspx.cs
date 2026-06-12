using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.UI;

namespace purchase_sale_storeroom
{
    public partial class HGuidanceCardBrowse : Page
    {
        private const string ConnectionStringName = "HochiSystem";
        private static readonly LanguageInfo[] Languages =
        {
            new LanguageInfo("zh-TW", "繁體"),
            new LanguageInfo("en", "英文"),
            new LanguageInfo("id", "印尼"),
            new LanguageInfo("zh-CN", "簡體")
        };

        protected string CurrentLanguageCode { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentLanguageCode = NormalizeLanguage(Request.QueryString["lang"]);

            if (!IsPostBack)
            {
                BindLanguages();
                BindCards();
            }
        }

        protected string GetLanguageCssClass(object code)
        {
            return String.Equals(Convert.ToString(code), CurrentLanguageCode, StringComparison.OrdinalIgnoreCase)
                ? "lang-link active"
                : "lang-link";
        }

        protected string ResolveImageUrl(object imagePath)
        {
            string path = Convert.ToString(imagePath);
            if (String.IsNullOrWhiteSpace(path)) return String.Empty;
            return ResolveUrl("~/" + path.TrimStart('~', '/'));
        }

        protected string ToJsString(object value)
        {
            string text = Convert.ToString(value) ?? String.Empty;
            return "'" + text
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", "")
                .Replace("\n", "") + "'";
        }

        private void BindLanguages()
        {
            Rpt_Languages.DataSource = Languages;
            Rpt_Languages.DataBind();
        }

        private void BindCards()
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                command.CommandText = ViewExists(connection) ? GetViewSql() : GetJoinSql();
                command.Parameters.Add("@HLangCode", SqlDbType.VarChar, 10).Value = CurrentLanguageCode;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            Rpt_Cards.DataSource = table;
            Rpt_Cards.DataBind();
            Pnl_Empty.Visible = table.Rows.Count == 0;
        }

        private static bool ViewExists(SqlConnection connection)
        {
            using (SqlCommand command = new SqlCommand(@"
SELECT COUNT(1)
FROM INFORMATION_SCHEMA.VIEWS
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'VGuidanceCardImageBrowse';", connection))
            {
                return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
            }
        }

        private static string GetViewSql()
        {
            return @"
SELECT
    HTopicID,
    HTopicDate,
    HTopicDateCode,
    HTopicTitle,
    HImageID,
    HLangCode,
    HLangName,
    HImagePath,
    HLineShareCode
FROM dbo.VGuidanceCardImageBrowse
WHERE HLangCode = @HLangCode
ORDER BY HTopicDate DESC;";
        }

        private static string GetJoinSql()
        {
            return @"
SELECT
    t.HID AS HTopicID,
    t.HTopicDate,
    CONVERT(CHAR(8), t.HTopicDate, 112) AS HTopicDateCode,
    t.HTopicTitle,
    i.HID AS HImageID,
    i.HLangCode,
    i.HLangName,
    i.HImagePath,
    N'{師指引圖:' + CONVERT(CHAR(8), t.HTopicDate, 112) + N'&' + i.HLangName + N'}' AS HLineShareCode
FROM dbo.HGuidanceCardTopic t
INNER JOIN dbo.HGuidanceCardImage i ON t.HID = i.HTopicID
WHERE
    t.HStatus = 1
    AND i.HStatus = 1
    AND t.HIsPublished = 1
    AND i.HLangCode = @HLangCode
ORDER BY t.HTopicDate DESC;";
        }

        private static string NormalizeLanguage(string languageCode)
        {
            foreach (LanguageInfo language in Languages)
            {
                if (String.Equals(language.Code, languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    return language.Code;
                }
            }

            return "zh-TW";
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

        public sealed class LanguageInfo
        {
            public LanguageInfo(string code, string name)
            {
                Code = code;
                Name = name;
            }

            public string Code { get; private set; }
            public string Name { get; private set; }
        }
    }
}
