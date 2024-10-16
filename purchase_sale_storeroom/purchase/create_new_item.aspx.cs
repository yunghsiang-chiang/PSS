using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using purchase_sale_storeroom.App_Code;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Drawing;
using Microsoft.AspNet.FriendlyUrls;


namespace purchase_sale_storeroom.purchase
{
    public partial class create_new_item : System.Web.UI.Page
    {
        //宣告 資料庫連接物件
        clsDB clsDB = new clsDB();
        //便利 新增紐取得 元件,並且少拜訪sql次數
        DataTable dt = new DataTable();

        /// <summary>
        /// 動態元件創建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Init(object sender, EventArgs e)
        {
            //注意! 動態生成追加事件 不能寫在(!IsPostBack)裡面! 會無效!
            //table name 資訊存在
            if (!String.IsNullOrWhiteSpace(Request.QueryString["table_name"]))
            {

                dt = clsDB.MySQL_Select(@"SHOW COLUMNS FROM purchase_sale_storeroom." + Request.QueryString["table_name"]);
                if (dt.Rows.Count > 0)
                {
                    //建立新項目填寫欄位
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Label top_class = new Label();
                        top_class.Text = @"<div class=""row"">
                                        <div class=""col-sm-6"">";
                        Label label = new Label();
                        label.Text = dt.Rows[i][0].ToString();
                        Label down_class = new Label();
                        down_class.Text = @"</div>
                                        </div>";
                        TextBox textBox = new TextBox();
                        textBox.Attributes["name"] = "tb_" + i.ToString();
                        textBox.ClientIDMode = ClientIDMode.Static;
                        textBox.ID = "tb_" + i.ToString();
                        if (i == 0)
                        {
                            DataTable dataTable = new DataTable();
                            dataTable = clsDB.MySQL_Select(@"SELECT distinct substr(item_id,1,2) 'class_id',substr(item_id,3,4) 'serior_number' FROM purchase_sale_storeroom." + Request.QueryString["table_name"]);
                            if (dataTable.Rows.Count > 0)
                            {
                                CultureInfo cul = CultureInfo.CurrentCulture;
                                int weekNum = cul.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                                string new_item_id = dataTable.Rows[0]["class_id"].ToString() + (dataTable.Rows.Count + 1).ToString("0000") + DateTime.Now.ToString("yyyyMMdd").Substring(2, 2) + weekNum.ToString("00");
                                textBox.Text = new_item_id;
                                textBox.ReadOnly = true;
                                textBox.BackColor = Color.Gray;
                            }
                        }
                        p_create.Controls.Add(top_class);
                        p_create.Controls.Add(label);
                        p_create.Controls.Add(down_class);
                        p_create.Controls.Add(top_class);
                        p_create.Controls.Add(textBox);
                        p_create.Controls.Add(down_class);
                    }
                    Label l_qty = new Label();
                    l_qty.Text = "入庫數量<br/>";
                    TextBox tb_qty = new TextBox();
                    tb_qty.TextMode = TextBoxMode.Number;
                    tb_qty.ClientIDMode =ClientIDMode.Static;
                    tb_qty.ID = "tb_qty";
                    tb_qty.Attributes["name"] = "tb_qty";
                    
                    Label l_safetyqty = new Label();
                    l_safetyqty.Text = "<br/>安全庫存警界數值<br/>";
                    TextBox tb_safetyqty = new TextBox();
                    tb_safetyqty.TextMode = TextBoxMode.Number;
                    tb_safetyqty.ClientIDMode = ClientIDMode.Static;
                    tb_safetyqty.ID = "tb_safetyqty";
                    tb_safetyqty.Attributes["name"] = "tb_safetyqty";
                    Label L_br = new Label();
                    L_br.Text = "<br/>";
                    Button btn_create = new Button();
                    btn_create.Text = "新增入庫";
                    btn_create.Click += new EventHandler(btn_Click);
                    p_create.Controls.Add(l_qty);
                    p_create.Controls.Add(tb_qty);
                    p_create.Controls.Add(l_safetyqty);
                    p_create.Controls.Add(tb_safetyqty);
                    p_create.Controls.Add(L_br);
                    p_create.Controls.Add(btn_create);
                }
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        /// <summary>
        /// 提供 動態生成Button 觸發事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count > 0)
            {
                List<string> columnsList = new List<string>();
                List<string> valuesList = new List<string>();
                string temp_alert_str = "新增資料至資料庫內容如下 \\n";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string check_qty = Request.Form["ctl00$MainContent$tb_"+(i).ToString()].ToString();
                    valuesList.Add(check_qty);
                    temp_alert_str += dt.Rows[i][0].ToString()+"欄位:" + check_qty + "\\n";
                    columnsList.Add(dt.Rows[i][0].ToString()); //將資料表的 columns資訊填入List

                    //若資料表只有 item_id、item_name 兩欄位,將 gender 與 size 預設資料碼 設定為 X-
                    if (i==1 && dt.Rows.Count==2)
                    {
                        valuesList[0] = valuesList[0] +"X-";
                    }
                }

                //假設一些 初始化與空值情況
                string tb_qty = "0";
                string safetyqty = "0";
                Int64 checknumber = new Int64();
                if (!String.IsNullOrEmpty(Request.Form["ctl00$MainContent$tb_qty"].ToString()) && Int64.TryParse(Request.Form["ctl00$MainContent$tb_qty"].ToString(),out checknumber))
                {
                    tb_qty = Request.Form["ctl00$MainContent$tb_qty"].ToString();
                }
                if (!String.IsNullOrEmpty(Request.Form["ctl00$MainContent$tb_safetyqty"].ToString()) && Int64.TryParse(Request.Form["ctl00$MainContent$tb_safetyqty"].ToString(), out checknumber))
                {
                    safetyqty = Request.Form["ctl00$MainContent$tb_safetyqty"].ToString();
                }
                
                temp_alert_str += "入庫數量:"+Request.Form["ctl00$MainContent$tb_qty"].ToString() + "\\n";
                temp_alert_str += "安全庫存設定"+Request.Form["ctl00$MainContent$tb_safetyqty"].ToString() + "\\n";
                

                //欄位定位數值 雛形先寫死,後續變更再想其他資訊輔助變得彈性寫法
                //資料庫的 c_資料表 新增對應項目
                clsDB.MySQL_Command(@"insert into purchase_sale_storeroom." + Request.QueryString["table_name"] + " (" + String.Join(",", columnsList) + ") values ('" + String.Join("','", valuesList) + "')");
                //資料庫的 r_item_qty 新增對應項目
                clsDB.MySQL_Command(@"insert into purchase_sale_storeroom.r_item_qty (item_id, item_name, area, sub_inv, qty, safety_stock, is_phaseout) values ('" + String.Join("','", valuesList) + "','桃園','F11'," + tb_qty + "," + safetyqty + ",0)");
                //資料庫的 h_item_inout 新增對應項目
                clsDB.MySQL_Command(@"insert into purchase_sale_storeroom.h_item_inout (item_id, item_name, lm_time, in_out, area, sub_inv, qty) values ('"+String.Join("','",valuesList)+"','"+DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"','in','桃園','F11',"+ tb_qty + ")");
                Response.Write("<script> alert('" + temp_alert_str + "');</script>");
                //引轉至 入庫頁面
                Response.Redirect("~/purchase/entry_system.aspx");
            }
        }


    }
}