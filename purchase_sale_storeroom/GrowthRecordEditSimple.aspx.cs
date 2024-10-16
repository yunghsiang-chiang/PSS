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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace purchase_sale_storeroom
{
    public partial class GrowthRecordEditSimple : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Cookie Check
            if (Request.Cookies["UserInfo"] != null)
            {
                if (!IsPostBack)
                {
                    GetItemList();
                    if (Request["id"] != null)
                    {
                        hfID.Value = Request["id"].ToString().Substring(0, 10);
                        lFunc.Text = "修改成長紀錄";
                        GetData(Request["id"].ToString());
                    }
                    else
                    {
                        lFunc.Text = "新增成長紀錄";
                        txtRecordDate.Text = DateTime.Now.AddHours(0).ToString("yyyy.MM.dd");
                        txtRecordTime.Text = DateTime.Now.AddHours(0).ToString("HH:mm");
                        lCDate.Text = DateTime.Now.AddHours(0).ToString("yyyy.MM.dd HH:mm");


                    }
                    ddl_dharma.Attributes.Add("onchange", "func_changedharma();");
                    //CKEditoritems: [
                    txtContent1.config.toolbar = new object[]
                    {
                    new object[] { "Bold", "Italic", "Underline", "-" , "NumberedList", "BulletedList", "Outdent", "Indent", "TextColor", "BGColor" , "-" , "Link", "Unlink" , "-" , "Font", "FontSize"  }
                    };
                    txtContent1.FontDefaultLabel = "新細明體";
                    txtContent1.FontNames = "標楷體;新細明體;微軟正黑體;Arial;Times New Roman;Verdana;";
                    txtContent1.FontSizeSizes = "14/14px;16/16px;18/18px;20/20px;22/22px;24/24px;26/26px;28/28px;";
                }
                //ddl_Course.Attributes.Add("onchange", "func_chCourse(this.value)");
                //ddl_CourseDate.Attributes.Add("onchange", "func_chCourseDate()");


                lMsg.Visible = false;
                lMsg.Text = "";
            }
            else
                Response.Redirect("Default.aspx", false);
        }

        //異動類別
        protected void rbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            DataSet objDataSet;
            string strSQL = string.Empty;

            ListItem li;

            try
            {
                if (rbType.SelectedValue == "6")
                {
                    rbCategory.SelectedIndex = 2; //記錄類別:修(預設)
                                                  //成長進度:夥伴結盟、光團建構-1+1、光團建構-2+2、光團建構-4+4、光團建構-7+1、光團建構-7+7、光團建構-14+14
                    ddl_dharma.Items.Clear();
                    strSQL = "select * from codevalue where cv_status=0 and cv_parent='0000000478' and cv_ctid='0000000015' order by cv_order";
                    objDataSet = uTool.QueryDataSet(strSQL);
                    foreach (DataRow R in objDataSet.Tables[0].Rows)
                    {
                        li = new ListItem();
                        li.Text = R["cv_name"].ToString();
                        li.Value = R["cv_name"].ToString();
                        ddl_dharma.Items.Add(li);
                    }
                    txtDharmaOther.Visible = false;
                }
                else
                {
                    rbCategory.SelectedIndex = 1;//記錄類別:煉(預設)
                                                 //成長進度:大愛手、立如松、其他
                    ddl_dharma.Items.Clear();
                    strSQL = "select * from codevalue where cv_status=1 and cv_ctid='0000000015' order by cv_order";
                    objDataSet = uTool.QueryDataSet(strSQL);
                    foreach (DataRow R in objDataSet.Tables[0].Rows)
                    {
                        li = new ListItem();
                        if (R["cv_content1"].ToString() != "")
                        {
                            li.Text = uTool.SelectOneField("select cv_name from codevalue where cv_id='" + R["cv_content1"].ToString() + "'") + "-" + R["cv_name"].ToString();
                        }
                        else
                            li.Text = R["cv_name"].ToString();
                        li.Value = R["cv_name"].ToString();
                        ddl_dharma.Items.Add(li);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandle(ex.ToString());
            }
            finally
            {
                objDataSet = null;
                uTool = null;
            }
        }

        //儲存資料
        protected void butSave_Click(object sender, EventArgs e)
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            string strSQL = string.Empty;
            string strMaxNo = string.Empty;
            string strDir = string.Empty;
            string strFileName1 = string.Empty;
            string strFileName2 = string.Empty;
            string strFileName3 = string.Empty;
            string strLabel = string.Empty;
            string strDharma = string.Empty;
            string strDharmaName = string.Empty;
            string strNo = string.Empty;
            string strLRId = string.Empty;
            string strCId = string.Empty;

            int iReturn = 0;
            try
            {
                if (Request["id"] == null)
                {
                    strMaxNo = uTool.GetNextNO("select max(gr_id) from growthrecord ", 10);
                    hfID.Value = strMaxNo;
                }
                strDir = Server.MapPath("./") + "ulfiles\\growthrecord\\";
                if (rbstatus.SelectedValue.ToString() == "-1" || rbstatus.SelectedValue.ToString() == "0")
                {
                    if (ddl_laoshi.SelectedValue != "")
                    {
                        lMsg.Text = "開放對象為不公開或限定開放(大愛光老師)無法【同時發佈於大愛光老師專欄】，開放對象請設定為【全體同修】!";
                        lMsg.Visible = true;
                        return;
                    }
                    if (ddl_Benefit.SelectedValue != "")
                    {
                        lMsg.Text = "開放對象為不公開或限定開放(大愛光老師)無法【同時發佈於法的好處】，開放對象請設定為【全體同修】!";
                        lMsg.Visible = true;
                        return;
                    }
                }

                //save file
                if (file1.HasFile)
                    strFileName1 = SaveFile(strDir, hfID.Value, "1", file1);
                if (file2.HasFile)
                    strFileName2 = SaveFile(strDir, hfID.Value, "2", file2);
                if (file3.HasFile)
                    strFileName3 = SaveFile(strDir, hfID.Value, "3", file3);
                //主法
                if (ddl_dharma.SelectedItem.Text == "其他")
                    strDharmaName = txtDharmaOther.Text;
                else
                    strDharmaName = ddl_dharma.SelectedItem.Text;
                //大愛光老師
                if (ddl_laoshi.SelectedValue != "")
                    strLRId = SaveLaoshiReply(ddl_laoshi.SelectedValue, hfLRID.Value);
                //法的好處
                if (ddl_Benefit.SelectedIndex != 0)
                {
                    strCId = "2";
                    strDharma = ddl_Benefit.SelectedValue;
                    strDharmaName = ddl_Benefit.SelectedItem.Text;
                    strDharmaName = strDharmaName.Substring(strDharmaName.IndexOf("-") + 1, strDharmaName.Length - strDharmaName.IndexOf("-") - 1);
                }
                else
                {
                    strCId = ddl_Course.SelectedValue;
                    strDharma = ddl_dharma.SelectedValue;
                }

                if (Request["id"] == null)
                {
                    // 
                    strNo = uTool.SelectOneField("select count(*)+1 as t from growthrecord where gr_uid='" + Request.Cookies["UserInfo"]["Id"] + "' and gr_status<9 and gr_recorddate like '" + DateTime.Now.Year.ToString() + "%'");
                    //加入
                    strSQL = "insert into growthrecord (gr_id,gr_uid,gr_cid,gr_type,gr_title,gr_content1,gr_content2,gr_recorddate,gr_status,gr_file1,gr_file2,gr_file3,gr_cdate,gr_cname,gr_udate,gr_uname,gr_no,gr_category,gr_grid,gr_grid2,gr_dharma,gr_dharmaname,gr_label,gr_lid,gr_lrid) values ( ";
                    strSQL += "'" + strMaxNo + "',";
                    strSQL += "'" + Request.Cookies["UserInfo"]["Id"] + "',";
                    strSQL += "'" + strCId + "',";
                    strSQL += rbType.SelectedValue + ",";
                    strSQL += "'" + txtTitle.Text + "',";
                    strSQL += "'" + txtContent1.Text + "',";
                    strSQL += "'',";
                    if (ddl_CourseDate.SelectedValue == "")
                        strSQL += "'" + txtRecordDate.Text.Replace(".", "") + txtRecordTime.Text.Replace(":", "").Replace("：", "").PadRight(6, '0') + "',";
                    else
                        strSQL += "'" + ddl_CourseDate.SelectedItem.Text.Substring(0, 10).Replace(".", "") + "210000',";
                    strSQL += rbstatus.SelectedValue + ",";
                    strSQL += "'" + strFileName1 + "',";
                    strSQL += "'" + strFileName2 + "',";
                    strSQL += "'" + strFileName3 + "',";
                    strSQL += "'" + DateTime.Now.AddHours(0).ToString("yyyyMMddHHmmss") + "',";
                    strSQL += "'" + HttpUtility.UrlDecode(Request.Cookies["UserInfo"]["Name"]) + "',";
                    strSQL += "'" + DateTime.Now.AddHours(0).ToString("yyyyMMddHHmmss") + "',";
                    strSQL += "'" + HttpUtility.UrlDecode(Request.Cookies["UserInfo"]["Name"]) + "',";
                    strSQL += "'" + DateTime.Now.Year.ToString() + "-" + strNo + "',";
                    strSQL += rbCategory.SelectedValue + ",";//gr_category,gr_grid,gr_label
                    strSQL += rbGrid1.SelectedValue + ",";
                    strSQL += rbGrid2.SelectedValue + ",";
                    strSQL += "'" + strDharma + "',";
                    strSQL += "'" + strDharmaName + "',";
                    strSQL += "'" + txtLabel.Text + "',";
                    strSQL += "'" + ddl_laoshi.SelectedValue + "',";
                    strSQL += "'" + strLRId + "'";
                    strSQL += ")";
                    iReturn = uTool.ExecuteSQL(strSQL);
                    lMsg.Text = strSQL;
                    //
                    hfID.Value = strMaxNo;
                    //加入log
                    uTool.SetAppLog(Request.Cookies["UserInfo"]["Id"], HttpUtility.UrlDecode(Request.Cookies["UserInfo"]["Name"]), Request.UserHostAddress, "Add Growth Record", "新增" + txtTitle.Text + "-" + txtContent1.Text + "");
                }
                else
                {
                    //修改
                    strSQL = "update growthrecord set ";
                    strSQL += "gr_type=" + rbType.SelectedValue + ",";
                    strSQL += "gr_cid='" + strCId + "',";
                    strSQL += "gr_title='" + txtTitle.Text + "',";
                    strSQL += "gr_content1='" + txtContent1.Text + "',";
                    strSQL += "gr_recorddate='" + txtRecordDate.Text.Replace(".", "") + txtRecordTime.Text.Replace(":", "").Replace("：", "").PadRight(6, '0') + "',";
                    strSQL += "gr_status=" + rbstatus.SelectedValue + ",";
                    if (strFileName1.Trim() != "")
                        strSQL += "gr_file1='" + strFileName1 + "',";
                    if (strFileName2.Trim() != "")
                        strSQL += "gr_file2='" + strFileName2 + "',";
                    if (strFileName3.Trim() != "")
                        strSQL += "gr_file3='" + strFileName3 + "',";
                    strSQL += "gr_udate='" + DateTime.Now.AddHours(0).ToString("yyyyMMddHHmmss") + "',";
                    strSQL += "gr_uname='" + HttpUtility.UrlDecode(Request.Cookies["UserInfo"]["Name"]) + "',";
                    strSQL += "gr_category=" + rbCategory.SelectedValue + ",";
                    strSQL += "gr_grid=" + rbGrid1.SelectedValue + ",";
                    strSQL += "gr_grid2=" + rbGrid2.SelectedValue + ",";
                    strSQL += "gr_dharma='" + strDharma + "',";
                    strSQL += "gr_dharmaname='" + strDharmaName + "',";
                    if (ddl_laoshi.SelectedValue != "")
                    {
                        strSQL += "gr_lid='" + ddl_laoshi.SelectedValue + "',";
                        //如果LRID不存在要重新產生
                        if (hfLRID.Value == "")
                        {
                            strSQL += "gr_lrid='" + strLRId + "',";
                        }
                    }
                    strSQL += "gr_label='" + txtLabel.Text + "'";
                    strSQL += " where gr_id='" + hfID.Value + "'";
                    iReturn = uTool.ExecuteSQL(strSQL);
                    //加入log
                    uTool.SetAppLog(Request.Cookies["UserInfo"]["Id"], HttpUtility.UrlDecode(Request.Cookies["UserInfo"]["Name"]), Request.UserHostAddress, "Update Growth Record", "異動" + txtTitle.Text + "-" + txtContent1.Text + "");
                }

                //確認是否存檔成功            
                if (iReturn == 0)
                {
                    lMsg.Visible = true;
                    lMsg.Text = "儲存失敗!請在檢查一次是否輸入正確!";
                }
                else
                {
                    Response.Redirect("GrowthRecord.aspx", false);
                }
            }
            catch (Exception ex)
            {

                lMsg.Visible = true;
                lMsg.Text = "無法儲存!";
                ErrorHandle(ex.ToString());
            }
            finally
            {
                uTool = null;
            }
        }

        //取得資料
        private void GetData(string strID)
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            DataSet objDataSet;
            string strSQL = string.Empty;
            int idx = 0;
            try
            {
                strSQL = "select * from growthrecord where gr_id='" + strID + "' order by gr_id desc limit 0,1";
                objDataSet = uTool.QueryDataSet(strSQL);
                foreach (DataRow R in objDataSet.Tables[0].Rows)
                {
                    //類別:日誌,進度
                    for (idx = 0; idx < rbType.Items.Count; idx++)
                    {
                        if (R["gr_type"].ToString() == rbType.Items[idx].Value)
                            rbType.SelectedIndex = idx;
                    }
                    txtRecordDate.Text = uTool.DateFromSQL(R["gr_recorddate"].ToString(), "D");
                    txtRecordTime.Text = uTool.DateFromSQL(R["gr_recorddate"].ToString(), "m").Substring(11, 5);
                    txtContent1.Text = R["gr_content1"].ToString().Trim();
                    txtTitle.Text = R["gr_title"].ToString().Trim();
                    //班會
                    for (idx = 0; idx < ddl_Course.Items.Count; idx++)
                    {
                        if (R["gr_cid"].ToString() == ddl_Course.Items[idx].Value)
                            ddl_Course.SelectedIndex = idx;
                    }
                    lCDate.Text = uTool.DateFromSQL(R["gr_cdate"].ToString(), "m");
                    //記錄類別
                    for (idx = 0; idx < rbCategory.Items.Count; idx++)
                    {
                        if (R["gr_category"].ToString() == rbCategory.Items[idx].Value)
                            rbCategory.SelectedIndex = idx;
                    }
                    //九宮格
                    for (idx = 0; idx < rbGrid1.Items.Count; idx++)
                    {
                        if (R["gr_grid"].ToString() == rbGrid1.Items[idx].Value)
                            rbGrid1.SelectedIndex = idx;
                    }
                    for (idx = 0; idx < rbGrid2.Items.Count; idx++)
                    {
                        if (R["gr_grid2"].ToString() == rbGrid2.Items[idx].Value)
                            rbGrid2.SelectedIndex = idx;
                    }
                    for (idx = 0; idx < ddl_dharma.Items.Count; idx++)
                    {
                        if (R["gr_dharma"].ToString() == ddl_dharma.Items[idx].Value)
                            ddl_dharma.SelectedIndex = idx;
                    }
                    //主法
                    if (ddl_dharma.SelectedItem.Text == "其他")
                    {
                        txtDharmaOther.Text = R["gr_dharmaname"].ToString();
                        txtDharmaOther.Enabled = true;
                    }
                    txtLabel.Text = R["gr_label"].ToString();
                    //狀態
                    for (idx = 0; idx < rbstatus.Items.Count; idx++)
                    {
                        if (R["gr_status"].ToString() == rbstatus.Items[idx].Value)
                            rbstatus.SelectedIndex = idx;
                    }
                    //同時發佈於大愛光老師專欄
                    if (R["gr_lid"].ToString() != "")
                    {
                        for (idx = 0; idx < ddl_laoshi.Items.Count; idx++)
                        {
                            if (R["gr_lid"].ToString() == ddl_laoshi.Items[idx].Value)
                                ddl_laoshi.SelectedIndex = idx;
                        }
                    }
                    hfLID.Value = R["gr_lid"].ToString();
                    hfLRID.Value = R["gr_lrid"].ToString();
                    //file1
                    if (R["gr_file1"].ToString().Trim() != "" && R["gr_file1"].ToString().Trim().ToLower().IndexOf("jpg") != -1)
                        lImg.Text += "檔案1：<br><img src='ulfiles/growthrecord/" + R["gr_file1"].ToString() + "' alt='圖1'></a>";
                    else if (R["gr_file1"].ToString().Trim() != "" && (R["gr_file1"].ToString().Trim().ToLower().IndexOf("mp3") != -1 || R["gr_file1"].ToString().Trim().ToLower().IndexOf("aac") != -1))
                        lImg.Text += "檔案1：" + R["gr_file1"].ToString() + "<audio src='ulfiles/growthrecord/" + R["gr_file1"].ToString() + "' preload='auto' loop=1></audio>";
                    else if (R["gr_file1"].ToString().Trim() != "")
                        lImg.Text += "檔案1：<a href='ulfiles/growthrecord/" + R["gr_file1"].ToString() + "' target=_blank>" + R["gr_file1"].ToString() + "　下載/閱覽</a>";
                    if (lImg.Text != "")
                        lImg.Text += "<br>";
                    //file2
                    if (R["gr_file2"].ToString().Trim() != "" && R["gr_file2"].ToString().Trim().ToLower().IndexOf("jpg") != -1)
                        lImg.Text += "檔案2：<br><img src='ulfiles/growthrecord/" + R["gr_file2"].ToString() + "' alt='圖2'>";
                    else if (R["gr_file2"].ToString().Trim() != "" && (R["gr_file2"].ToString().Trim().ToLower().IndexOf("mp3") != -1 || R["gr_file2"].ToString().Trim().ToLower().IndexOf("aac") != -1))
                        lImg.Text += "檔案2：" + R["gr_file2"].ToString() + "<audio src='ulfiles/growthrecord/" + R["gr_file2"].ToString() + "' preload='auto' loop=1></audio>";
                    else if (R["gr_file2"].ToString().Trim() != "")
                        lImg.Text += "檔案2：<a href='ulfiles/growthrecord/" + R["gr_file2"].ToString() + "' target=_blank>" + R["gr_file2"].ToString() + "　下載/閱覽</a>";
                    if (lImg.Text != "")
                        lImg.Text += "<br>";
                    //file3
                    if (R["gr_file3"].ToString().Trim() != "" && R["gr_file3"].ToString().Trim().ToLower().IndexOf("jpg") != -1)
                        lImg.Text += "檔案3：<br><img src='ulfiles/growthrecord/" + R["gr_file3"].ToString() + "' alt='圖3'>";
                    else if (R["gr_file3"].ToString().Trim() != "" && (R["gr_file3"].ToString().Trim().ToLower().IndexOf("mp3") != -1 || R["gr_file3"].ToString().Trim().ToLower().IndexOf("aac") != -1))
                        lImg.Text += "檔案3：" + R["gr_file3"].ToString() + "<audio src='ulfiles/growthrecord/" + R["gr_file3"].ToString() + "' preload='auto' loop=1></audio>";
                    else if (R["gr_file3"].ToString().Trim() != "")
                        lImg.Text += "檔案3：<a href='ulfiles/growthrecord/" + R["gr_file3"].ToString() + "' target=_blank>" + R["gr_file3"].ToString() + "　下載/閱覽</a>";
                    if (lImg.Text != "")
                        lImg.Text += "<hr>";
                }
                //成長進度
                if (rbType.SelectedValue == "6")
                {
                    ListItem li;
                    string strDharma = string.Empty;
                    //成長進度:夥伴結盟、光團建構-1+1、光團建構-2+2、光團建構-4+4、光團建構-7+1、光團建構-7+7、光團建構-14+14
                    ddl_dharma.Items.Clear();
                    strSQL = "select * from codevalue where cv_status=0 and cv_parent='0000000478' and cv_ctid='0000000015' order by cv_order";
                    objDataSet = uTool.QueryDataSet(strSQL);
                    foreach (DataRow R in objDataSet.Tables[0].Rows)
                    {
                        li = new ListItem();
                        li.Text = R["cv_name"].ToString();
                        li.Value = R["cv_name"].ToString();
                        ddl_dharma.Items.Add(li);
                    }
                    strDharma = uTool.SelectOneField("select gr_dharma from growthrecord where gr_id='" + strID + "'");
                    for (idx = 0; idx < ddl_dharma.Items.Count; idx++)
                    {
                        if (strDharma == ddl_dharma.Items[idx].Value)
                            ddl_dharma.SelectedIndex = idx;
                    }
                    txtDharmaOther.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandle(ex.ToString());
            }
            finally
            {
                objDataSet = null;
                uTool = null;
            }
        }

        //儲存大愛光老師專欄回應
        private string SaveLaoshiReply(string strLID, string strLRID)
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            string strSQL = string.Empty;
            string strMaxNo = string.Empty;
            string strDir = string.Empty;
            string strNo = string.Empty;
            string strFileName1 = string.Empty;
            string strFileName2 = string.Empty;
            string strFileName3 = string.Empty;
            string strQuestionNo = string.Empty;
            string strYearQuestionNo = string.Empty;

            int iReturn = 0;
            string strResult = string.Empty;

            try
            {
                strMaxNo = uTool.GetNextNO("select max(lr_id) from laoshireply ", 10);
                strDir = Server.MapPath("./") + "ulfiles\\laoshi\\";

                //save file
                if (file1.HasFile)
                    strFileName1 = SaveFile(strDir, "r" + strMaxNo, "1", file1);
                if (file2.HasFile)
                    strFileName2 = SaveFile(strDir, "r" + strMaxNo, "2", file2);
                if (file3.HasFile)
                    strFileName3 = SaveFile(strDir, "r" + strMaxNo, "3", file3);

                if (strLRID.Trim() == "")
                {
                    strNo = uTool.SelectOneField("select count(*)+1 from laoshireply where lr_uid='" + Request.Cookies["UserInfo"]["Id"] + "' and lr_status<9 and lr_cdate like '" + DateTime.Now.AddHours(0).Year.ToString() + "%'");
                    if (rbType.SelectedValue == "3")
                    {
                        strQuestionNo = uTool.SelectOneField("select count(*)+1 from laoshireply where lr_q=1 and lr_status<9 and lr_lid='" + ddl_laoshi.SelectedValue + "' ");
                        strYearQuestionNo = DateTime.Now.AddHours(0).Year.ToString() + "-" + uTool.SelectOneField("select count(*)+1 from laoshireply where lr_q=1 and lr_status<9 and lr_cdate like '" + DateTime.Now.AddHours(0).Year.ToString() + "%'");
                    }
                    else
                    {
                        strQuestionNo = "";
                        strYearQuestionNo = "";
                    }
                    //加入
                    strSQL = "insert into laoshireply (lr_id,lr_lid,lr_uid,lr_content,lr_status,lr_file1,lr_file2,lr_file3,lr_cdate,lr_cname,lr_udate,lr_uname,lr_parent,lr_q,lr_isshowdaily,lr_no,lr_qno,lr_yqno) values ( ";
                    strSQL += "'" + strMaxNo + "',";
                    strSQL += "'" + ddl_laoshi.SelectedValue + "',";
                    strSQL += "'" + Request.Cookies["UserInfo"]["Id"] + "',";
                    strSQL += "'" + txtContent1.Text + "',";
                    strSQL += "1,";
                    strSQL += "'" + strFileName1 + "',";
                    strSQL += "'" + strFileName2 + "',";
                    strSQL += "'" + strFileName3 + "',";
                    strSQL += "'" + DateTime.Now.AddHours(0).ToString("yyyyMMddHHmmss") + "',";
                    strSQL += "'" + HttpUtility.UrlDecode(Request.Cookies["UserInfo"]["Name"]) + "',";
                    strSQL += "'" + DateTime.Now.AddHours(0).ToString("yyyyMMddHHmmss") + "',";
                    strSQL += "'" + HttpUtility.UrlDecode(Request.Cookies["UserInfo"]["Name"]) + "',";
                    strSQL += "'',";
                    if (rbType.SelectedValue == "3")
                        strSQL += "1,";
                    else
                        strSQL += "0,";
                    strSQL += "1,";
                    strSQL += "'" + DateTime.Now.AddHours(0).Year.ToString() + "-" + strNo + "',";
                    strSQL += "'" + strQuestionNo + "',";
                    strSQL += "'" + strYearQuestionNo + "'";
                    strSQL += ")";
                    iReturn = uTool.ExecuteSQL(strSQL);
                    //
                    strResult = strMaxNo;
                }
                else
                {
                    //修改
                    strSQL = "update laoshireply set ";
                    if (rbType.SelectedValue == "3")
                        strSQL += "lr_q=1,";
                    else
                    {
                        strSQL += "lr_q=0,";
                        strSQL += "lr_qno='',";
                        strSQL += "lr_yqno='',";
                    }
                    if (ddl_laoshi.SelectedValue != "")
                        strSQL += " lr_lid='" + ddl_laoshi.SelectedValue + "',";
                    strSQL += " lr_content='" + txtContent1.Text + "',";
                    strSQL += " lr_udate='" + DateTime.Now.AddHours(0).ToString("yyyyMMddHHmmss") + "',";
                    strSQL += " lr_uname='" + HttpUtility.UrlDecode(Request.Cookies["UserInfo"]["Name"]) + "'";
                    strSQL += " where lr_id='" + hfLRID.Value + "'";
                    iReturn = uTool.ExecuteSQL(strSQL);
                    //更新同步的回應
                    if (ddl_laoshi.SelectedValue != hfLID.Value)
                    {
                        strSQL = "update laoshireply set ";
                        strSQL += " lr_lid='" + ddl_laoshi.SelectedValue + "'";
                        strSQL += " where lr_parent='" + hfLRID.Value + "'";
                        iReturn = uTool.ExecuteSQL(strSQL);
                    }
                    //
                    strResult = hfLRID.Value;
                }
                //提問篇數
                ReCalNotReply(hfID.Value);
                //重新計算提問編號
                ReCalQuestion(hfID.Value);
            }
            catch (Exception ex)
            {
                ErrorHandle(ex.ToString());
            }
            finally
            {
                uTool = null;
            }
            return strResult;
        }

        //課程日期
        protected void ddl_Course_SelectedIndexChanged(object sender, EventArgs e)
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            DataSet objDataSet;
            string strSQL = string.Empty;
            ListItem li;
            try
            {
                //判斷是否為幸福班
                if (ddl_Course.SelectedValue == "0000000001")
                {
                    ddl_CourseDate.Visible = true;
                    ddl_CourseDate.Items.Clear();
                    strSQL = "select * from coursedetail where cd_cid='0000000001' and  cd_status=1 and cd_week='1' order by cd_coursedate desc";
                    objDataSet = uTool.QueryDataSet(strSQL);
                    foreach (DataRow R in objDataSet.Tables[0].Rows)
                    {
                        li = new ListItem();
                        li.Text = uTool.DateFromSQL(R["cd_coursedate"].ToString(), "D");
                        li.Value = R["cd_id"].ToString();
                        ddl_CourseDate.Items.Add(li);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandle(ex.ToString());
            }
            finally
            {
                objDataSet = null;
                uTool = null;
            }
        }

        //----------------
        //- 取得班會
        //----------------
        private void GetItemList()
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            DataSet objDataSet;
            string strSQL = string.Empty;
            ListItem li;
            try
            {
                //取得班會名稱-晨光玉成、幸福班一階
                strSQL = "select * from course where c_status=1 and c_grstatus=1 and c_id < '1000000000' order by c_begdate limit 0,2";
                objDataSet = uTool.QueryDataSet(strSQL);
                foreach (DataRow R in objDataSet.Tables[0].Rows)
                {
                    li = new ListItem();
                    li.Text = R["c_name"].ToString();
                    li.Value = R["c_id"].ToString();
                    ddl_Course.Items.Add(li);
                }
                //取得班會名稱-最新三個課程名稱
                strSQL = "select * from course where c_status=1 and c_grstatus=1 and c_id < '1000000000' order by c_begdate desc limit 0,3";
                objDataSet = uTool.QueryDataSet(strSQL);
                foreach (DataRow R in objDataSet.Tables[0].Rows)
                {
                    li = new ListItem();
                    li.Text = R["c_name"].ToString();
                    li.Value = R["c_id"].ToString();
                    ddl_Course.Items.Add(li);
                }
                li = new ListItem();
                li.Text = "非班會期間";
                li.Value = "0";
                ddl_Course.Items.Add(li);
                li = new ListItem();
                li.Text = "2022年識透生命真相讀書會第四期尋光階講師玉成班會";
                li.Value = "0";
                ddl_Course.Items.Add(li);

                //ddl_Course.SelectedIndex = 0;
                //課程日期
                strSQL = "select * from coursedetail where cd_cid='0000000001' and  cd_status=1 and cd_week='1' order by cd_coursedate desc limit 0,5";
                objDataSet = uTool.QueryDataSet(strSQL);
                foreach (DataRow R in objDataSet.Tables[0].Rows)
                {
                    li = new ListItem();
                    li.Text = uTool.DateFromSQL(R["cd_coursedate"].ToString(), "D") + "-" + R["cd_title"].ToString().Replace("<br>", "");
                    li.Value = R["cd_id"].ToString();
                    ddl_CourseDate.Items.Add(li);
                }
                //成長進度-主法&同時發佈於法的好處
                strSQL = "select * from codevalue where cv_status=1 and cv_ctid='0000000015' order by cv_order";
                objDataSet = uTool.QueryDataSet(strSQL);
                foreach (DataRow R in objDataSet.Tables[0].Rows)
                {
                    //主法
                    li = new ListItem();
                    if (R["cv_content1"].ToString() != "")
                    {
                        li.Text = uTool.SelectOneField("select cv_name from codevalue where cv_id='" + R["cv_content1"].ToString() + "'") + "-" + R["cv_name"].ToString();
                    }
                    else
                        li.Text = R["cv_name"].ToString();
                    li.Value = R["cv_name"].ToString();
                    ddl_dharma.Items.Add(li);
                    //同時發佈於法的好處
                    li = new ListItem();
                    if (R["cv_content1"].ToString() != "")
                    {
                        li.Text = uTool.SelectOneField("select cv_name from codevalue where cv_id='" + R["cv_content1"].ToString() + "'") + "-" + R["cv_name"].ToString();
                    }
                    else
                        li.Text = R["cv_name"].ToString();
                    li.Value = R["cv_id"].ToString();
                    ddl_Benefit.Items.Add(li);
                }
                //同時發佈於大愛光老師專欄
                getLaoshiItem("0", "0000000154");
                //
                getLaoshiItem("48", "0000000167");
                getLaoshiItem("35", "0000000339");
                /*
                getLaoshiItem("1", "0000000167");
                getLaoshiItem("2", "0000000168");
                getLaoshiItem("3", "0000000169");
                getLaoshiItem("4", "0000000170");
                getLaoshiItem("5", "0000000171");
                getLaoshiItem("6", "0000000176");
                getLaoshiItem("7", "0000000184");
                getLaoshiItem("8", "0000000199");
                getLaoshiItem("9", "0000000202");
                getLaoshiItem("10", "0000000226");
                */
            }
            catch (Exception ex)
            {
                ErrorHandle(ex.ToString());
            }
            finally
            {
                objDataSet = null;
                uTool = null;
            }
        }

        //大愛光老師專欄Item
        private void getLaoshiItem(string strCategory, string strFuncId)
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            DataSet objDataSet;
            string strSQL = string.Empty;
            string strPermit = string.Empty;
            ListItem li;
            try
            {
                //同時發佈於大愛光老師專欄
                //權限check-1:List,2:Add,3:Mod,4:Del
                strPermit = uTool.GetPermit(Request.Cookies["UserInfo"]["Id"], strFuncId);
                if (strPermit.IndexOf("1") != -1)
                {
                    li = new ListItem();
                    switch (strCategory)
                    {
                        case "0":
                            li.Text = "【大愛光老師專欄】";
                            li.Value = "";
                            break;
                        case "1":
                            li.Text = "【認識和氣大愛】";
                            li.Value = "";
                            break;
                        case "2":
                            li.Text = "【九大法流體系守護】";
                            li.Value = "";
                            break;
                        case "3":
                            li.Text = "【四大五體群組】";
                            li.Value = "";
                            break;
                        case "4":
                            li.Text = "【四大七體】";
                            li.Value = "";
                            break;
                        case "5":
                            li.Text = "【中心】";
                            li.Value = "";
                            break;
                        case "6":
                            li.Text = "【春風化雨】";
                            li.Value = "";
                            break;
                        case "7":
                            li.Text = "【築體講師玉成】";
                            li.Value = "";
                            break;
                        case "8":
                            li.Text = "【築體初階講師入選】";
                            li.Value = "";
                            break;
                        case "9":
                            li.Text = "【血脈報恩】";
                            li.Value = "";
                            break;
                        case "10":
                            li.Text = "【幸福印記】";
                            li.Value = "";
                            break;
                        case "48":
                            li.Text = "【老師行腳】";
                            li.Value = "";
                            break;
                        case "35":
                            li.Text = "【光之子】";
                            li.Value = "";
                            break;
                    }
                    ddl_laoshi.Items.Add(li);
                    strSQL = "select * from laoshi where l_category='" + strCategory + "' and l_status=2 and l_recorddate < '" + DateTime.Now.AddHours(0).ToString("yyyyMMddHHmmss") + "' and l_title not like '%成長紀錄%' order by l_no desc limit 0,20";
                    objDataSet = uTool.QueryDataSet(strSQL);
                    foreach (DataRow R in objDataSet.Tables[0].Rows)
                    {
                        li = new ListItem();
                        li.Text = "　#" + R["l_no"].ToString() + "-" + R["l_title"].ToString().Replace("br", "");
                        li.Value = R["l_id"].ToString();
                        ddl_laoshi.Items.Add(li);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandle(ex.ToString());
            }
            finally
            {
                objDataSet = null;
                uTool = null;
            }
        }

        //重新計算提問已回應篇數
        private void ReCalNotReply(string strID)
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            DataSet objDataSet;
            string strSQL = string.Empty;
            int idx = 0;
            try
            {
                strSQL = "select * from laoshireply where lr_lid='" + strID + "' and lr_status<9 and lr_q=1 ";
                objDataSet = uTool.QueryDataSet(strSQL);
                foreach (DataRow R in objDataSet.Tables[0].Rows)
                {
                    if (R["lr_parent"].ToString() == "")
                    {
                        strSQL = "select count(*) as t from laoshireply where lr_lid='" + strID + "' and lr_status<9 and lr_parent='" + R["lr_id"].ToString() + "'";
                        if (uTool.SelectOneField(strSQL) != "0")
                            idx++;
                    }
                    else
                    {
                        strSQL = "select count(*) as t from laoshireply where lr_lid='" + strID + "' and lr_status<9 and lr_cdate<'" + R["lr_cdate"].ToString() + "'";
                        if (uTool.SelectOneField(strSQL) != "0")
                            idx++;
                    }
                }
                strSQL = "update laoshi set l_rq=" + idx.ToString() + " where l_id='" + strID + "'";
                uTool.ExecuteSQL(strSQL);
            }
            catch (Exception ex)
            {
                ErrorHandle(ex.ToString());
            }
            finally
            {
                objDataSet = null;
                uTool = null;
            }
        }

        //重新計算提問序號
        private void ReCalQuestion(string strID)
        {
            SAJDac.Dac uTool = new SAJDac.Dac();
            DataSet objDataSet;
            string strSQL = string.Empty;
            string strYear = string.Empty;
            int idx = 0;
            try
            {
                //重新計算單篇提問編號
                strSQL = "select * from laoshireply where lr_lid='" + strID + "' and lr_status<9 and lr_q=1 order by lr_cdate";
                objDataSet = uTool.QueryDataSet(strSQL);
                idx = 1;
                foreach (DataRow R in objDataSet.Tables[0].Rows)
                {
                    strSQL = "update laoshireply set lr_qno='" + idx.ToString() + "' where lr_id='" + R["lr_id"].ToString() + "'";
                    uTool.ExecuteSQL(strSQL);
                    idx++;
                }
                //重新計算年度提問編號
                strSQL = "select lr_cdate from laoshireply where lr_id='" + ddl_laoshi.SelectedValue + "'";
                strYear = uTool.SelectOneField(strSQL).Substring(0, 4);
                strSQL = "select * from laoshireply where lr_status<9 and lr_q=1 and lr_cdate like '" + strYear + "%' order by lr_cdate";
                objDataSet = uTool.QueryDataSet(strSQL);
                idx = 1;
                foreach (DataRow R in objDataSet.Tables[0].Rows)
                {
                    strSQL = "update laoshireply set lr_yqno='" + strYear + "-" + idx.ToString() + "' where lr_id='" + R["lr_id"].ToString() + "'";
                    uTool.ExecuteSQL(strSQL);
                    idx++;
                }
            }
            catch (Exception ex)
            {
                ErrorHandle(ex.ToString());
            }
            finally
            {
                objDataSet = null;
                uTool = null;
            }
        }

        //save file
        private string SaveFile(string strDir, string strId, string strNo, FileUpload fu)
        {
            string strFileName = string.Empty;

            if (fu.FileName.ToLower().IndexOf("jpg") != -1 || fu.FileName.ToLower().IndexOf("gif") != -1)
            {
                strFileName = strId + "_" + DateTime.Now.AddHours(0).ToString("HHmm") + strNo + fu.FileName.ToLower().Substring(fu.FileName.ToLower().LastIndexOf("."), fu.FileName.Length - fu.FileName.ToLower().LastIndexOf("."));
                if (File.Exists(strDir + strFileName))
                    File.Delete(strDir + strFileName);
                fu.SaveAs(strDir + strFileName);
                GenerateImage(strDir, strFileName, 500);
            }
            else if (fu.FileName.ToLower().IndexOf("mp3") != -1 || fu.FileName.ToLower().IndexOf("aac") != -1 || fu.FileName.ToLower().IndexOf("doc") != -1 || fu.FileName.ToLower().IndexOf("xls") != -1 || fu.FileName.ToLower().IndexOf("pp") != -1 || fu.FileName.ToLower().IndexOf("pdf") != -1)
            {
                strFileName = strId + "_" + DateTime.Now.AddHours(0).ToString("HHmm") + strNo + fu.FileName.ToLower().Substring(fu.FileName.ToLower().LastIndexOf("."), fu.FileName.Length - fu.FileName.ToLower().LastIndexOf("."));
                if (File.Exists(strDir + strFileName))
                    File.Delete(strDir + strFileName);
                fu.SaveAs(strDir + strFileName);
            }
            else
            {
                lMsg.Visible = true;
                if (strNo != "0")
                    lMsg.Text = "檔案" + strNo + "格式錯誤！";
                else
                    lMsg.Text = "檔案格式錯誤！";
                return "";
            }
            return strFileName;
        }

        //reise image
        private void GenerateImage(string strDir, string strFileName, int width)
        {
            System.Drawing.Image baseImage = System.Drawing.Image.FromFile(strDir + "\\" + strFileName);
            Bitmap img;
            Graphics graphic;

            Single ratio = 0.0F;
            Single h = baseImage.Height;
            Single w = baseImage.Width;

            int ht = 0;
            int wt = 0;


            string strNewName = "";

            ratio = width / w;

            if (width < w)
            {
                ht = Convert.ToInt32(ratio * h);
                wt = width;
            }
            else
            {
                ht = Convert.ToInt32(baseImage.Height);
                wt = Convert.ToInt32(baseImage.Width);
            }

            strNewName = strDir + "\\" + strFileName;
            strNewName = strNewName.Replace(".jpg", "_s.jpg");

            img = new Bitmap(wt, ht);
            graphic = Graphics.FromImage(img);
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(baseImage, 0, 0, wt, ht);

            img.Save(strNewName, ImageFormat.Jpeg);
            img.Dispose();
            graphic.Dispose();
            baseImage.Dispose();
            File.Delete(strDir + "\\" + strFileName);
            File.Copy(strNewName, strDir + "\\" + strFileName);
            File.Delete(strNewName);
        }

        //----------------
        //- 寫入Log
        //----------------
        private void ErrorHandle(string e)
        {
            string strLogFile = string.Empty;
            try
            {
                strLogFile = Server.MapPath("./log/") + DateTime.Now.ToString("yyyy_MM_dd") + ".log";
                using (StreamWriter sw = new StreamWriter(strLogFile, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("----------------------------------------------");
                    sw.WriteLine(e);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch
            {
                Response.Redirect("500.html", false);
            }
        }


        protected void rbstatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbstatus.SelectedValue == "0")
                ph_open.Visible = true;
            else
                ph_open.Visible = false;
        }
    }
}