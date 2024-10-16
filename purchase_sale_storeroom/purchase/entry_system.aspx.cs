using purchase_sale_storeroom.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml.Linq;
using Mysqlx.Crud;

namespace purchase_sale_storeroom.purchase
{
    
    public partial class entry_system : System.Web.UI.Page
    {
        //宣告 資料庫物件 clsDB
        clsDB clsDB = new clsDB();
        protected void Page_Load(object sender, EventArgs e)
        {
            //初始化
            if (!IsPostBack)
            {
                //類別清單
                default_RadioButtonList_class();
                //預設呈現類別清單
                MultiView1.ActiveViewIndex = 0;
            }
        }
        /// <summary>
        /// 類別清單
        /// </summary>
        protected void default_RadioButtonList_class()
        {
            DataTable dataTable = new DataTable();
            dataTable = clsDB.MySQL_Select(@"SELECT class_name,c_table_name FROM purchase_sale_storeroom.c_class");
            if (dataTable.Rows.Count>0)
            {
                rbl_ClassList.DataSource = dataTable;
                rbl_ClassList.DataTextField= "class_name";
                rbl_ClassList.DataValueField = "c_table_name";
                rbl_ClassList.DataBind();
                rbl_ClassList.Items.Add(new ListItem("創立新類別", "create_new_table"));

                //變更項目顏色
                foreach (ListItem i in rbl_ClassList.Items)
                    if (i.Value == "create_new_table")
                        i.Attributes["style"] = "color:red;";
                    else if (i.Value == "c_book")
                        i.Attributes["style"] = "color:burlywood;";
                    else if (i.Value == "c_hochi_clothes" || i.Value== "c_hochi_shoes" || i.Value== "c_hochi_outdoor_shoes" || i.Value== "c_backpack" || i.Value== "c_hochi_socks" || i.Value== "c_hochi_pants" || i.Value== "c_hochi_tableware" || i.Value== "c_hochi_coat" || i.Value== "c_stand_collar_shirt")
                        i.Attributes["style"] = "color:Indigo;";
            }
        }
        /// <summary>
        /// 類別項目選取 觸發事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbl_ClassList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //切換view 與 對應內容
            if (rbl_ClassList.SelectedValue!= "create_new_table")
            {
                MultiView1.ActiveViewIndex = 1;
                //變更 項目清單
                gv_item_append_content(rbl_ClassList.SelectedValue);
            }
            else
            {   
                //引轉至創新項目頁面
                Response.Redirect("~/purchase/create_new_class.aspx", true);

            }
        }
        /// <summary>
        /// 變更 項目清單
        /// </summary>
        /// <param name="tablename"></param>
        protected void gv_item_append_content(string tablename)
        {
            DataTable dt = new DataTable();
            dt = clsDB.MySQL_Select(@"SELECT *,'' as '點擊入庫數量或手動輸入' FROM purchase_sale_storeroom." + tablename);
            if (dt.Rows.Count>0)
            {
                gv_item_append.DataSource = dt;
                gv_item_append.DataBind();
            }
        }
        /// <summary>
        /// row data bound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_item_append_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //隱藏item_id
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //不使用e.Row.Cells[0].Visible = false
                //因為後續入庫會回頭取得item_id 資訊
                //e.Row.Cells[0].Visible = false;
                e.Row.Cells[0].Attributes["class"] = "標題"; /* 把類別加上class屬性,用於互動 */
                e.Row.Cells[e.Row.Cells.Count - 1].Width = 250;
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Attributes["class"] = "隱藏"; /* 把類別加上class屬性,用於互動 */
                //e.Row.Cells[0].Visible = false;
                string show_counter_input = "<link rel=\"stylesheet\" href=\"https://use.fontawesome.com/releases/v5.1.1/css/all.css\" integrity=\"sha384-O8whS3fhG2OnA5Kas0Y9l3cfpmYjapjI0E4theH4iuMD+pLhbf6JI0jIMfYcK3yZ\" crossorigin=\"anonymous\">";
                show_counter_input += "<div class=\"container\">";
                show_counter_input += "<div class=\"col-sm-6\">";
                show_counter_input += "<div class=\"input-group sm-6\">";
                show_counter_input += "<input type=\"text\" id=\"qty_input" + e.Row.RowIndex.ToString() + "\" name=\"qty_input"+e.Row.RowIndex.ToString()+"\"  class=\"form-control form-control-sm\" value=\"0\" min=\"0\">";
                show_counter_input += "<div class=\"input-group-prepend\">";
                show_counter_input += "<button class=\"btn btn-dark btn-sm\" id=\"minus-btn" + e.Row.RowIndex.ToString() + "\"  onclick='return false;'><i class=\"fa fa-minus\"></i></button></div>";
                show_counter_input += "<div class=\"input-group-append\">";
                show_counter_input += "<button class=\"btn btn-dark btn-sm\" id=\"plus-btn" + e.Row.RowIndex.ToString() + "\"   onclick='return false;'><i class=\"fa fa-plus\"></i></button>";
                show_counter_input += "</div></div></div></div>";
                /* 加上對應的 JS */
                show_counter_input += "<script>";
                show_counter_input += "$(document).ready(function(){";
                show_counter_input += "$('#plus-btn" + e.Row.RowIndex.ToString() + "').click(function(){";
                show_counter_input += "$('#qty_input" + e.Row.RowIndex.ToString() + "').val(parseInt($('#qty_input" + e.Row.RowIndex.ToString() + "').val()) + 1 );";
                show_counter_input += "});";
                show_counter_input += "$('#minus-btn" + e.Row.RowIndex.ToString() + "').click(function(){";
                show_counter_input += "$('#qty_input" + e.Row.RowIndex.ToString() + "').val(parseInt($('#qty_input" + e.Row.RowIndex.ToString() + "').val()) - 1 );";
                show_counter_input += "if ($('#qty_input" + e.Row.RowIndex.ToString() + "').val() == -1) {";
                show_counter_input += "$('#qty_input" + e.Row.RowIndex.ToString() + "').val(0); } });  });";
                show_counter_input += "</script>";
                e.Row.Cells[e.Row.Cells.Count-1].Text = show_counter_input;
            }


        }
        /// <summary>
        /// 入庫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bt_submit_Click(object sender, EventArgs e)
        {
            if (gv_item_append.Rows.Count>0)
            {
                string mysql_command = "";
                string mysql_command2 = "insert into purchase_sale_storeroom.h_item_inout (item_id, item_name, lm_time, in_out, area, sub_inv, qty) values";
                List<string> sqlcommandList = new List<string>();
                string temp_alert_str = "入庫明細 \\n";
                for (int i=0;i< gv_item_append.Rows.Count;i++)
                {
                    string check_qty = Request.Form["qty_input"+i.ToString()].ToString();
                    Int64 outint = new Int64();
                    if (Int64.TryParse(check_qty, out outint))
                    {
                        if (Convert.ToInt64(check_qty) > 0)
                        {
                            mysql_command += "UPDATE purchase_sale_storeroom.r_item_qty SET qty = qty + "+ Convert.ToInt64(check_qty).ToString() + "  WHERE(item_id = '"+ gv_item_append.Rows[i].Cells[0].Text + "');";
                            sqlcommandList.Add( "('"+ gv_item_append.Rows[i].Cells[0].Text + "','"+ gv_item_append.Rows[i].Cells[1].Text + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','in','桃園','F11',"+Convert.ToInt64(check_qty).ToString()+")" ) ;
                            temp_alert_str += gv_item_append.Rows[i].Cells[0].Text + "-"+ gv_item_append.Rows[i].Cells[1].Text + ":" + Convert.ToInt64(check_qty).ToString() +"\\n";
                        }
                    }
                }
                //更新庫房數量
                clsDB.MySQL_Command(mysql_command);
                //資料庫的 h_item_inout 新增對應項目
                clsDB.MySQL_Command(mysql_command2 + String.Join(",",sqlcommandList));
                Response.Write("<script> alert('"+temp_alert_str+"');</script>");
                MultiView1.ActiveViewIndex = 0;
            }
        }
        /// <summary>
        /// 創新項目入庫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bt_create_item_Click(object sender, EventArgs e)
        {
            //引轉至創新項目頁面
            Response.Redirect("~/purchase/create_new_item.aspx?table_name=" + rbl_ClassList.SelectedValue, true);
        }
    }
}