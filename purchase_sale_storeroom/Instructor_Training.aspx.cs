using purchase_sale_storeroom.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Google.Apis.Util;

namespace purchase_sale_storeroom
{
    public partial class Instructor_Training : System.Web.UI.Page
    {
        clsDB clsDB = new clsDB();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                getData();
            }
        }

        protected void getData()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["HID"]))
            {
                string HID = Request.QueryString["HID"];
                // QueryString 不為空，執行相關邏輯
                DataTable dataTable = new DataTable();
                dataTable = clsDB.SQL_Select(@"with a as (
SELECT HCourseID,case HAStatus when 0 then '未報到' when 1 then '實體報到'　when 2 then '線上報到' when 4 then '請假' end '出勤狀態',HRCDate
  FROM [HochiSystem].[dbo].[HRollCall]
  where HMemberID = "+ HID + @" ),b as (
  SELECT HID,HCourseName
  FROM [HochiSystem].[dbo].[HCourse]
  WHERE HID in (SELECT HCourseID
  FROM [HochiSystem].[dbo].[HRollCall]
  where HMemberID = "+ HID + @"
) )
SELECT b.HCourseName,MIN(a.HRCDate) 'HRCDate',Sum(case when 出勤狀態='未報到'　then 1 else 0 end ) '未報到',Sum(case when 出勤狀態='實體報到'　then 1 else 0 end ) '實體報到',Sum(case when 出勤狀態='線上報到'　then 1 else 0 end ) '線上報到',Sum(case when 出勤狀態='請假'　then 1 else 0 end ) '請假' FROM a LEFT JOIN b ON a.HCourseID = b.HID
GROUP BY b.HCourseName
order by HRCDate desc", "HochiSystemConnectionString");

                if (dataTable.Rows.Count>0)
                {
                    gv_RollCall.DataSource = dataTable;
                    gv_RollCall.DataBind();
                }

                //取得授傳表
                DataTable dataTable1 = new DataTable();
                dataTable1 = clsDB.SQL_Select("SELECT HID,HDharmaName,0 '次數' FROM HochiSystem.dbo.HDharma", "HochiSystemConnectionString");
                dataTable1.Columns["次數"].ReadOnly = false;

                var dataTable1_linq = dataTable1.AsEnumerable();


                //取得個人被授傳紀錄
                DataTable dataTable2 = new DataTable();
                dataTable2 = clsDB.SQL_Select("SELECT HMemberID,HDharmaPass FROM HochiSystem.dbo.HCourseBooking WHERE   HDharmaPass <> '' and HMemberID=" + HID, "HochiSystemConnectionString");

                if (dataTable2.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable2.Rows)
                    {
                        string hdharmaPass = row["HDharmaPass"].ToString();

                        // 確保資料不為空
                        if (!string.IsNullOrEmpty(hdharmaPass))
                        {
                            // 去掉最後的逗號，然後分割字串
                            string[] dharmaPassArray = hdharmaPass.TrimEnd(',').Split(',');

                            foreach (string dharmaID in dharmaPassArray)
                            {
                                if (int.TryParse(dharmaID, out int hid)) // 確保 dharmaID 是有效數字
                                {
                                    // 使用 LINQ 找到對應 HID 並更新次數
                                    var matchingRows = dataTable1.AsEnumerable()
                                        .Where(r => r.Field<int>("HID") == hid);

                                    foreach (var matchingRow in matchingRows)
                                    {
                                        int currentCount = matchingRow.Field<int>("次數");
                                        matchingRow.SetField("次數", currentCount + 1);
                                    }
                                }
                            }
                        }
                    }

                    // 使用 LINQ 查詢次數不為 0 的資料
                    var filteredRows = dataTable1.AsEnumerable()
                        .Where(row => row.Field<int>("次數") > 0)
                        .CopyToDataTable(); // 確保 DataTable 正確綁定到 GridView

                    gv_teach.DataSource = filteredRows;
                    gv_teach.DataBind();
                }
                else
                {
                    DataTable tempdt = new DataTable();
                    tempdt.Columns.Add("訊息");
                    DataRow tempdatarow = tempdt.NewRow();
                    tempdatarow["訊息"] = "EDU資料庫沒有同修被授傳紀錄!";
                    tempdt.Rows.Add(tempdatarow);
                    gv_teach.DataSource = tempdt;
                    gv_teach.DataBind();
                }


            }
            else
            {
                Response.Write("沒有同修的ID無法順利呈現同修資訊!");
            }
        }
    }
}