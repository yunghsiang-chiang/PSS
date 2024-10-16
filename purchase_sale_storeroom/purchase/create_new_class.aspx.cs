using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using purchase_sale_storeroom.App_Code;
using System.Security.Cryptography;
using System.Drawing;



namespace purchase_sale_storeroom.purchase
{
    public partial class create_new_class : System.Web.UI.Page
    {
        //宣告 資料庫連線 物件
        clsDB clsDB = new clsDB();
        ////宣告 資料表用於公用 減少進出資料庫
        //DataTable dt = new DataTable();
        protected void Page_Init(object sender, EventArgs e)
        {
            //注意! 動態生成追加事件 不能寫在(!IsPostBack)裡面! 會無效!
            //dt = clsDB.MySQL_Select(@"SELECT class_id,class_name,c_table_name FROM purchase_sale_storeroom.c_class");
            //if (dt.Rows.Count>0)
            //{
            //    Label l_container = new Label();
            //    l_container.Text = "<div class=\"container\">";
            //    Label label_newClassID = new Label();
            //    label_newClassID.Text = "<h2>類別ID不可修改,系統自動填寫</h2><h3>新類別ID</h3>";
            //    Label down_class = new Label();
            //    down_class.Text = "</div>";
            //    Label top_class = new Label();
            //    top_class.Text = "<div class=\"col-sm-12\">";
            //    TextBox tb_classID = new TextBox();
            //    tb_classID.Attributes["name"] = "tb_clsssID";
            //    tb_classID.ClientIDMode = ClientIDMode.Static;
            //    tb_classID.ID = "tb_class_ID";
            //    tb_classID.Text = dt.Rows.Count.ToString("00");
            //    tb_classID.ReadOnly = true;
            //    tb_classID.BackColor = Color.Gray;
            //    Label l_newClassName = new Label();
            //    l_newClassName.Text = "<h3>新類別名稱(限定使用中文)</h3>";
            //    TextBox tb_className = new TextBox();
            //    tb_className.Attributes["name"]= "tb_clsssName";
            //    tb_className.ClientIDMode = ClientIDMode.Static;
            //    tb_className.ID = "tb_clsssName";
            //    Label l_newCDatatable = new Label();
            //    l_newCDatatable.Text = "<h3>資料庫-資料表名稱(限定使用英文)</h3>";
            //    TextBox tb_CDatatable = new TextBox();
            //    tb_CDatatable.Attributes["name"] = "tb_CDatatable";
            //    tb_CDatatable.ClientIDMode = ClientIDMode.Static;
            //    tb_CDatatable.ID = "tb_CDatatable";

            //    p_create.Controls.Add(l_container);
            //    p_create.Controls.Add(label_newClassID);
            //    p_create.Controls.Add(top_class);
            //    p_create.Controls.Add(tb_classID);
            //    p_create.Controls.Add(down_class);

            //    p_create.Controls.Add(l_newClassName);
            //    p_create.Controls.Add(top_class);
            //    p_create.Controls.Add(tb_className);
            //    p_create.Controls.Add(down_class);

            //    p_create.Controls.Add(l_newCDatatable);
            //    p_create.Controls.Add(top_class);
            //    p_create.Controls.Add(tb_CDatatable);
            //    p_create.Controls.Add(down_class);

            //    p_create.Controls.Add(down_class);
            //}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //新類別ID 不讓使用者修改
                DataTable dt = new DataTable();
                dt = clsDB.MySQL_Select("SELECT class_id,class_name,c_table_name FROM purchase_sale_storeroom.c_class");
                tb_classID.Text = dt.Rows.Count.ToString();
                tb_classID.ReadOnly= true;
                tb_classID.BackColor=Color.Gray;

            }
        }
        /// <summary>
        /// 創立新類別 按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bt_submit_Click(object sender, EventArgs e)
        {
            string Is_check_str = @"`size` VARCHAR(5) NOT NULL COMMENT '尺寸',
  `gender` VARCHAR(5) NOT NULL COMMENT '性別',";
            string mysql_str1 = @"CREATE TABLE `purchase_sale_storeroom`.`c_"+tb_cDatatable.Text+@"` (
  `item_id` VARCHAR(25) NOT NULL COMMENT '物品ID',
  `item_name` VARCHAR(45) NOT NULL COMMENT '物品名稱',";
            string mysql_str2 = @"  PRIMARY KEY (`item_id`))
COMMENT = '"+tb_className.Text+@"';";

            string mysql_str;
            if (cb_gender_size.Checked)
            {
                mysql_str = mysql_str1 + Is_check_str + mysql_str2;
            }
            else
            {
                mysql_str = mysql_str1 + mysql_str2;
            }

            // 資料表 c_class 新增資料
            clsDB.MySQL_Command("INSERT INTO purchase_sale_storeroom.c_class(class_id, class_name, c_table_name) VALUES ('"+tb_classID.Text+"', '"+tb_className.Text+"', 'c_"+tb_cDatatable.Text +"')");
            // 創立資料表 c_"使用者填寫英文"
            clsDB.MySQL_Command(mysql_str);
            //Response.Write("INSERT INTO purchase_sale_storeroom.c_class(class_id, class_name, c_table_name) VALUES ('\"+tb_classID+\"', '\"+tb_className+\"', 'c_\"+tb_cDatatable+\"')");
            //Response.Write("<br/>");
            //Response.Write(mysql_str);
            Response.Write("<script>alert('觸發按鈕');</script>");
        }
    }
}