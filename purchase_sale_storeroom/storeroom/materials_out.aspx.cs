using purchase_sale_storeroom.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace purchase_sale_storeroom.storeroom
{
    public partial class materials_out : System.Web.UI.Page
    {
        //將Sql連線class 宣告進來使用
        clsDB clsDB = new clsDB();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //快篩條件 顯示
                create_radio_input();
                //庫存資訊 顯示
                gv_realtime_databind();
                //揀料資訊
                temp_pack();
            }
        }

        /// <summary>
        /// 庫存資訊 顯示
        /// </summary>
        /// <param name="filter_conditions">篩選條件[可省略]</param>
        protected void gv_realtime_databind(string filter_conditions = "")
        {
            DataTable tempdt = new DataTable();
            tempdt = clsDB.MySQL_Select(@"with a as (SELECT item_id, item_name, area, sub_inv, qty, safety_stock,substr(item_id,1,2) 'class_id',substr(item_id,11,1) 'gender',substr(item_id,12,length(item_id)-8) 'size'  FROM purchase_sale_storeroom.r_item_qty)
                                        ,b as (SELECT class_id, class_name, c_table_name FROM purchase_sale_storeroom.c_class)
                                        select class_name '類型',item_id '類型ID',item_name '類型名稱',gender '分類',size '尺寸',qty '數量','' as '快速揀料' from a left join b on a.class_id = b.class_id
                                        where qty != 0");
            if (tempdt.Rows.Count > 0)
            {
                gv_item_realtime.DataSource = tempdt;
                gv_item_realtime.DataBind();
            }
        }
        /// <summary>
        /// 建立快篩選單
        /// </summary>
        protected void create_radio_input()
        {
            DataTable tempdt = new DataTable();
            tempdt = clsDB.MySQL_Select(@"with a as (SELECT item_id, item_name, area, sub_inv, qty, safety_stock,substr(item_id,1,2) 'class_id',substr(item_id,11,1) 'gender',substr(item_id,12,length(item_id)-8) 'size'  FROM purchase_sale_storeroom.r_item_qty)
,b as (SELECT class_id, class_name, c_table_name FROM purchase_sale_storeroom.c_class)
select distinct class_name from a left join b on a.class_id = b.class_id");
            if (tempdt.Rows.Count > 0)
            {
                //使用後台 動態 建立前台項目
                //前台 樣式
                //< div class="form-check form-check-inline">
                //<input class="form-check-input" type="radio" name="inlineRadioOptions" id="inlineRadio1" value="和氣褲"  onclick="handleClick(this);">
                //<label class="form-check-label" for="inlineRadio1">和氣褲</label>
                //</div>
                Label temp_label = new Label();
                temp_label.Text = "";
                for (int i = 0; i < tempdt.Rows.Count; i++)
                {
                    temp_label.Text += "<div class=\"form-check form-check-inline\">";
                    temp_label.Text += "<input class=\"form-check-input\" type=\"radio\" name=\"inlineRadioOptions\" id=\"inlineRadio" + (i + 1).ToString() + "\" value=\"" + tempdt.Rows[i]["class_name"].ToString() + "\"  onclick=\"handleClick(this);\">";
                    temp_label.Text += "<label class=\"form-check-label\" for=\"inlineRadio" + (i + 1).ToString() + "\">" + tempdt.Rows[i]["class_name"].ToString() + "</label>";
                    temp_label.Text += "</div>";
                }
                p_quick_filter.Controls.Add(temp_label);
            }
        }
        /// <summary>
        /// 資料呈現時事件,這裡追加 class屬性,讓後面的css可以根據class 進行隱藏/顯示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_item_realtime_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Attributes["class"] = "標題"; /* 把類別加上class屬性,用於互動 */
                e.Row.Cells[1].Visible= false;
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Visible = false;
                e.Row.Attributes["class"] = e.Row.Cells[0].Text; /* 把類別加上class屬性,用於互動 */
                /* bootstrap "Simple input spinner controls" */
                /* https://bootsnipp.com/snippets/orG5B */
                string show_counter_input = "<link rel=\"stylesheet\" href=\"https://use.fontawesome.com/releases/v5.1.1/css/all.css\" integrity=\"sha384-O8whS3fhG2OnA5Kas0Y9l3cfpmYjapjI0E4theH4iuMD+pLhbf6JI0jIMfYcK3yZ\" crossorigin=\"anonymous\">";
                show_counter_input += "<div class=\"container\">";
                show_counter_input += "<div class=\"col-sm-6\">";
                show_counter_input += "<div class=\"input-group sm-6\">";
                show_counter_input += "<input type=\"text\" id=\"qty_input"+e.Row.RowIndex.ToString()+"\"  name=\"qty_input" + e.Row.RowIndex.ToString() + "\" class=\"form-control form-control-sm\" value=\"0\" min=\"0\">";
                show_counter_input += "<div class=\"input-group-prepend\">";
                show_counter_input += "<button class=\"btn btn-dark btn-sm\" id=\"minus-btn" + e.Row.RowIndex.ToString() + "\"  onclick='return false;'><i class=\"fa fa-minus\"></i></button></div>";
                show_counter_input += "<div class=\"input-group-append\">";
                show_counter_input += "<button class=\"btn btn-dark btn-sm\" id=\"plus-btn" + e.Row.RowIndex.ToString() + "\"   onclick='return false;'><i class=\"fa fa-plus\"></i></button>";
                show_counter_input += "</div></div></div></div>";
                /* 加上對應的 JS */
                show_counter_input += "<script>";
                show_counter_input += "$(document).ready(function(){";
                show_counter_input += "$('#qty_input" + e.Row.RowIndex.ToString() + "').prop('disabled', false);";
                show_counter_input += "$('#plus-btn" + e.Row.RowIndex.ToString() + "').click(function(){";
                show_counter_input += "$('#qty_input" + e.Row.RowIndex.ToString() + "').val(parseInt($('#qty_input" + e.Row.RowIndex.ToString() + "').val()) + 1 );";
                show_counter_input += "});";
                show_counter_input += "$('#minus-btn" + e.Row.RowIndex.ToString() + "').click(function(){";
                show_counter_input += "$('#qty_input" + e.Row.RowIndex.ToString() + "').val(parseInt($('#qty_input" + e.Row.RowIndex.ToString() + "').val()) - 1 );";
                show_counter_input += "if ($('#qty_input" + e.Row.RowIndex.ToString() + "').val() == -1) {";
                show_counter_input += "$('#qty_input" + e.Row.RowIndex.ToString() + "').val(0); } });  });";
                show_counter_input += "</script>";
                e.Row.Cells[6].Text = show_counter_input;
                e.Row.Cells[6].Width = new Unit("100px");
            }
        }
        /// <summary>
        /// 揀料提交[暫時]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bt_temp_submit_Click(object sender, EventArgs e)
        {
            //快篩條件 顯示
            create_radio_input();

            string sql_command_delete = "delete from purchase_sale_storeroom.t_item_out";
            clsDB.MySQL_Command(sql_command_delete);

            string temp_pickupitem = "檢料內容如下:\\n";
            string sql_command_str = "insert into purchase_sale_storeroom.t_item_out(item_id, item_name, lm_time, area, sub_inv, qty) values ";
            int Isvalues = 0;
            for (int i=0;i<gv_item_realtime.Rows.Count;i++)
            {
                if (Request.Form["qty_input" + i.ToString()].ToString()!="0")
                {
                    Isvalues += 1;
                    temp_pickupitem += gv_item_realtime.Rows[i].Cells[0].Text+"\\t";
                    temp_pickupitem += gv_item_realtime.Rows[i].Cells[1].Text + "\\t";
                    temp_pickupitem += gv_item_realtime.Rows[i].Cells[2].Text + "\\t";
                    temp_pickupitem += gv_item_realtime.Rows[i].Cells[3].Text + "\\t";
                    temp_pickupitem += gv_item_realtime.Rows[i].Cells[4].Text + "\\t";
                    temp_pickupitem += Request.Form["qty_input" + i.ToString()].ToString() + "\\n";
                    sql_command_str += "('"+ gv_item_realtime.Rows[i].Cells[1].Text+"',";
                    sql_command_str += "'" + gv_item_realtime.Rows[i].Cells[2].Text + "',";
                    sql_command_str += "'"+DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"',";
                    sql_command_str += "'桃園',";
                    sql_command_str += "'F11',";
                    sql_command_str += Request.Form["qty_input" + i.ToString()].ToString() + "),";
                }
            }
            if (Isvalues != 0)
            {
                sql_command_str = sql_command_str.Substring(0, sql_command_str.Length-1);
            }
            Response.Write("<script>alert('"+temp_pickupitem+"');</script>");
            clsDB.MySQL_Command(sql_command_str);

            //揀料資訊
            temp_pack();
            
        }
        /// <summary>
        /// 揀料資訊
        /// </summary>
        protected void temp_pack()
        {
            string sql_select = "SELECT item_id '物品ID', item_name '物品名稱', DATE_FORMAT(lm_time, '%Y/%m/%d %H:%i:%s') as '揀料時間', area, sub_inv, qty FROM purchase_sale_storeroom.t_item_out";
            DataTable dt = new DataTable();
            dt = clsDB.MySQL_Select(sql_select);
            gv_temp_pick.Caption = "揀料數量vs出貨單明細";
            gv_temp_pick.DataSource = dt;
            gv_temp_pick.DataBind();
        }
    }
}