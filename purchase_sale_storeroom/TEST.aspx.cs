using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace purchase_sale_storeroom
{
    public partial class TEST : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 從資料庫中獲取 exhibition 表的數據
                var colorDistributionData = GetExhibitionData();

                // 序列化數據為 JSON 格式，傳遞給前台
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(colorDistributionData);

                // 設定 JavaScript 變數，將數據傳遞到前端
                ClientScript.RegisterStartupScript(this.GetType(), "exhibitionDataScript",
                    $"var exhibitionData = {jsonData};", true);
            }
        }

        private List<ExhibitionData> GetExhibitionData()
        {
            try
            {
                List<ExhibitionData> data = new List<ExhibitionData>();

                // 設定 MySQL 連線字串
                string connectionString = "server=192.168.11.51;user id=hochi_root;password=hochi_Taoyuan;database=activity";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT colorGroup, " +
                                   "SUM(CASE WHEN color = '紅' THEN 1 ELSE 0 END) AS red, " +
                                   "SUM(CASE WHEN color = '橙' THEN 1 ELSE 0 END) AS orange, " +
                                   "SUM(CASE WHEN color = '黃' THEN 1 ELSE 0 END) AS yellow, " +
                                   "SUM(CASE WHEN color = '綠' THEN 1 ELSE 0 END) AS green, " +
                                   "SUM(CASE WHEN color = '藍' THEN 1 ELSE 0 END) AS blue, " +
                                   "SUM(CASE WHEN color = '靛' THEN 1 ELSE 0 END) AS indigo, " +
                                   "SUM(CASE WHEN color = '紫' THEN 1 ELSE 0 END) AS purple " +
                                   "FROM exhibition GROUP BY colorGroup";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new ExhibitionData
                            {
                                colorGroup = reader.GetString("colorGroup"),
                                red = reader.GetInt32("red"),
                                orange = reader.GetInt32("orange"),
                                yellow = reader.GetInt32("yellow"),
                                green = reader.GetInt32("green"),
                                blue = reader.GetInt32("blue"),
                                indigo = reader.GetInt32("indigo"),
                                purple = reader.GetInt32("purple")
                            });
                        }
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Response.Write("<script>console.log('"+ ex.Message.ToString()+ "');</script>");
                return null;
            }
            
        }

        // 定義一個用於存儲展覽數據的類
        public class ExhibitionData
        {
            public string colorGroup { get; set; }
            public int red { get; set; }
            public int orange { get; set; }
            public int yellow { get; set; }
            public int green { get; set; }
            public int blue { get; set; }
            public int indigo { get; set; }
            public int purple { get; set; }
        }
    }
}