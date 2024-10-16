using purchase_sale_storeroom.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace purchase_sale_storeroom.storeroom
{
    public partial class inout_top30 : System.Web.UI.Page
    {
        //宣告 資料庫 物件
        clsDB clsDB = new clsDB();
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            dataTable = clsDB.MySQL_Select(@"SELECT item_id 'ID', item_name '名稱', lm_time '時間', in_out '進出', area '區域', qty '數量', required_id '出貨單號', required_status '出貨單狀態'  FROM purchase_sale_storeroom.h_item_inout
order by lm_time desc
LIMIT 30");
            if (dataTable.Rows.Count>0)
            {
                gv_top30.Caption = "最新30筆 庫房進入資料";
                gv_top30.DataSource = dataTable;
                gv_top30.DataBind();
            }
        }
    }
}