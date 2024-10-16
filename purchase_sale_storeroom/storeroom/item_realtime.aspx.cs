using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using purchase_sale_storeroom.App_Code;
using MySql.Data.MySqlClient;

namespace purchase_sale_storeroom.storeroom
{
    public partial class item_realtime : System.Web.UI.Page
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
,b as (SELECT class_id, class_name, c_table_name FROM purchase_sale_storeroom.c_class),
c as (SELECT item_id,gender FROM purchase_sale_storeroom.c_hochi_clothes
union
SELECT item_id,gender FROM purchase_sale_storeroom.c_hochi_pants where gender != 'X'
union
SELECT item_id,gender FROM purchase_sale_storeroom.c_hochi_shoes
union
SELECT item_id,gender FROM purchase_sale_storeroom.c_sleeve
union
SELECT item_id,gender FROM purchase_sale_storeroom.c_stand_collar_shirt
union
SELECT item_id,gender FROM purchase_sale_storeroom.c_t_shirt
union
SELECT item_id,gender FROM purchase_sale_storeroom.c_thermal_clothing)
select class_name,item_name,case a.gender when '0' then c.gender when '1' then c.gender else 'X' end 'gender',size,area,qty from a left join b on a.class_id = b.class_id left join c on a.item_id = c.item_id");
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
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["class"] = e.Row.Cells[0].Text; /* 把類別加上class屬性,用於互動 */
                //將size裡面 顏色的資訊 刪除
                if (e.Row.Cells[3].Text.Length > 3)
                {
                    e.Row.Cells[3].Text = e.Row.Cells[3].Text.Substring(0, e.Row.Cells[3].Text.Length - 3);
                }
                //隱藏 gneder = X size=-
                if (e.Row.Cells[2].Text == "X")
                {
                    e.Row.Cells[2].Text = "<p class=\"hidden\">X</p>";
                }
                if (e.Row.Cells[3].Text == "-")
                {
                    e.Row.Cells[3].Text = "<p class=\"hidden\">-</p>";
                }
            }
        }
    }
}