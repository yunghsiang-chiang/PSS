<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GrowthRecordEditSimple.aspx.cs" Inherits="purchase_sale_storeroom.GrowthRecordEditSimple" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="css/plugins/datapicker/datepicker3.css" rel="stylesheet" />
    <link href="css/plugins/clockpicker/clockpicker.css" rel="stylesheet" />
    <link href="css/plugins/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css" rel="stylesheet" />
    <link href="js/plugins/audiojs/audiojs.css" rel="stylesheet" />
    <!-- Ladda style -->
    <link href="css/plugins/ladda/ladda-themeless.min.css" rel="stylesheet" />
    <title></title>
</head>
<body>

    <asp:HiddenField ID="hfID" runat="server" Value="" />
    <asp:HiddenField ID="hfLID" runat="server" Value="" />
    <asp:HiddenField ID="hfLRID" runat="server" Value="" />
    <asp:HiddenField ID="hfPermitUser" runat="server" Value="" />
    <form id="form1" runat="server">
        <!-- 主頁-content -->
        <div class="row wrapper border-bottom white-bg page-heading">
            <!-- 功能路徑 -->
            <div class="col-lg-10">
                <h2>成長紀錄　<small>
                    <asp:Label ID="lTitle" runat="server" Text=""></asp:Label></small></h2>
                <ol class="breadcrumb">
                    <li>
                        <a href="Main.aspx"><i class='fa fa-home'></i></a>
                    </li>
                    <li>
                        <a href="#">成長玉成</a>
                    </li>
                    <li class="active">
                        <strong>成長紀錄</strong>
                    </li>
                </ol>
            </div>
            <div class="col-lg-2">
            </div>
        </div>
        <!-- 主頁-Body -->
        <div class="wrapper wrapper-content animated fadeInRight">
            <div class="row">
                <div class="col-lg-12">
                    <div class="ibox float-e-margins">
                        <!-- table desc -->
                        <div class="ibox-title">
                            <h5>
                                <asp:Label ID="lFunc" runat="server" Text=""></asp:Label></h5>
                        </div>
                        <!-- table content -->
                        <div class="ibox-content">
                            <div class="form-group">
                                <div class="col-sm-8 col-sm-offset-2">
                                    <p class="text-danger">總結：導師每週幫光團夥伴總結</p>
                                    <asp:RadioButtonList ID="rbType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rbType_SelectedIndexChanged">
                                        <asp:ListItem Text="日誌" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="應用" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="提問" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="回應" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="新進度" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="法脈結構進度" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="總結" Value="7"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label"><span class="keypoint">＊</span>記錄類別</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <asp:RadioButtonList ID="rbCategory" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                <asp:ListItem Text="愿" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="煉" Value="0" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="修" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="行" Value="1"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label"><span class="keypoint">＊</span>九宮格</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <p class="text-danger">先選九宮格,再選屬性</p>
                                            <asp:RadioButtonList ID="rbGrid1" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                <asp:ListItem Text="身" Value="1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="心" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="靈" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="人" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="事" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="境" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="金錢" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="關係" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="時間" Value="9"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <br />
                                            <asp:RadioButtonList ID="rbGrid2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                <asp:ListItem Text="體態" Value="1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="體能" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="體質" Value="3"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group" id="data_1">
                                <label class="col-sm-2 control-label"><span class="keypoint">＊</span>記錄時間</label>
                                <div class="col-sm-3">
                                    <div class="input-group date">
                                        <span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        <asp:TextBox ID="txtRecordDate" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="input-group clockpicker" data-autoclose="true">
                                        <asp:TextBox ID="txtRecordTime" runat="server" CssClass="form-control" MaxLength="5" Text="09:30"></asp:TextBox>
                                        <span class="input-group-addon"><span class="fa fa-clock-o"></span></span>
                                    </div>
                                </div>
                                <p class="text-success">
                                    (登錄時間：<asp:Label ID="lCDate" runat="server" Text=""></asp:Label>)
                                </p>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label">課程名稱</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <asp:DropDownList ID="ddl_Course" runat="server" CssClass="form-control">
                                            </asp:DropDownList>
                                            <asp:DropDownList ID="ddl_CourseDate" runat="server" CssClass="form-control" Enabled="false" Visible="false">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label"><span class="keypoint">＊</span>成長進度</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-5">
                                            <asp:DropDownList ID="ddl_dharma" runat="server" CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-5">
                                            <asp:TextBox ID="txtDharmaOther" runat="server" CssClass="form-control" MaxLength="200" placeholder="其他:自行輸入..." Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label"><span class="keypoint">＊</span>主題</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="100" placeholder="請輸入主題..."></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label"><span class="keypoint">＊</span>內容</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <ckeditor:ckeditorcontrol id="txtContent1" basepath="~/ckeditor" runat="server" height="200" language="zh" entermode="BR" fontdefaultlabel="新細明體"></ckeditor:ckeditorcontrol>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label">同時發佈於大愛光老師專欄</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <asp:DropDownList ID="ddl_laoshi" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="---" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label">同時發佈於法的好處</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <asp:DropDownList ID="ddl_Benefit" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="---" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label">標籤</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <p class="text-danger">請以 , 分隔標籤</p>
                                            <asp:TextBox ID="txtLabel" runat="server" CssClass="form-control" MaxLength="200" placeholder="請輸入您想設定的標籤分類..."></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label">檔案上傳</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <p class="text-danger">圖片:jpg,gif / 語音:mp3,aac / 文檔:doc,docx,xls,xlsx,ppt,pptx,pdf </p>
                                            <asp:Label ID="lImg" runat="server" Text=""></asp:Label><br />
                                            檔案1：
                                            <asp:FileUpload ID="file1" runat="server" CssClass="form-control" />
                                            檔案2：
                                            <asp:FileUpload ID="file2" runat="server" CssClass="form-control" />
                                            檔案3：
                                            <asp:FileUpload ID="file3" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <label class="col-sm-2 control-label">開放對象</label>
                                <div class="col-sm-10">
                                    <div class="row">
                                        <div class="col-md-10">
                                            <div class="radio">
                                                <asp:RadioButtonList ID="rbstatus" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow">

                                                    <asp:ListItem Value="0" Text="限定開放(大愛光老師)"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="全體同修" Selected="true"></asp:ListItem>
                                                    <asp:ListItem Value="9" Text="刪除"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <asp:PlaceHolder ID="ph_open" runat="server" Visible="false">
                                <div class="hr-line-dashed"></div>
                                <div class="form-group">
                                    <label class="col-sm-2 control-label">限定開放設定</label>
                                    <div class="col-sm-10">
                                        <div class="row">
                                            <div class="col-md-10">
                                                <asp:TextBox ID="txtPermit" runat="server" CssClass="form-control" placeholder="設定限定開放設定對象..."></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <div class="col-sm-8 col-sm-offset-2">
                                    <asp:Label ID="lMsg" runat="server" Text="" CssClass="alert alert-danger" Visible="false"></asp:Label>
                                </div>
                            </div>
                            <div class="hr-line-dashed"></div>
                            <div class="form-group">
                                <div class="col-sm-8 col-sm-offset-2">
                                    <asp:Button ID="butBack" runat="server" Text="回上頁" CausesValidation="false" UseSubmitBehavior="false" CssClass="btn btn-white" PostBackUrl="GrowthRecord.aspx" />
                                    <asp:Button ID="butSave" runat="server" Text="儲存" CssClass="ladda-button btn btn-primary" OnClick="butSave_Click" data-style="slide-left" Style="left: 0px; top: 0px" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Data picker -->
        <script src="js/plugins/datapicker/bootstrap-datepicker.js"></script>
        <!-- Clock picker -->
        <script src="js/plugins/clockpicker/clockpicker.js"></script>
        <!-- Jquery Validate -->
        <script src="js/plugins/validate/jquery.validate.min.js"></script>
        <!-- audio -->
        <script src="js/plugins/audiojs/audio.min.js"></script>
        <!-- Ladda -->
        <script src="js/plugins/ladda/spin.min.js"></script>
        <script src="js/plugins/ladda/ladda.min.js"></script>
        <script src="js/plugins/ladda/ladda.jquery.min.js"></script>
        <!-- Typehead -->
        <script src="js/plugins/typehead/bootstrap3-typeahead.min.js"></script>
        <script type="text/javascript">
            $(function () {
                // 日期選擇器設定
                $('#data_1 .input-group.date').datepicker({
                    todayBtn: "linked",         // 啟用今天按鈕
                    keyboardNavigation: false,  // 禁用鍵盤導航
                    forceParse: false,          // 禁止強制解析
                    calendarWeeks: false,       // 不顯示周數
                    autoclose: true,            // 選擇日期後自動關閉
                    format: "yyyy.mm.dd"        // 日期格式
                });

                // Ladda按鈕初始化及綁定
                $('.ladda-button').ladda('bind', { timeout: 10000 });

                // 時鐘選擇器初始化
                $('.clockpicker').clockpicker();

                // 依據選項更新標籤文字
                $("input:radio[name='rbGrid1']").change(function () {
                    const grid2Labels = [
                        ["體態", "體能", "體質"],
                        ["心態", "心能", "心質"],
                        ["靈態", "靈能", "靈質"],
                        ["血脈", "靈脈", "法脈"]
                    ];
                    // 取得選擇的索引值並更新label
                    const index = $(this).val() - 1;
                    $("label[for*='rbGrid2_0']").html(grid2Labels[index][0]);
                    $("label[for*='rbGrid2_1']").html(grid2Labels[index][1]);
                    $("label[for*='rbGrid2_2']").html(grid2Labels[index][2]);
                });

                // 表單驗證設定
                $("#form1").validate({
                    ignore: [],  // 不忽略任何元素
                    rules: {
                        ctl00$ContentPlaceHolder1$txtRecordDate: { required: true }, // 必須輸入記錄日期
                        ctl00$ContentPlaceHolder1$txtTitle: {
                            required: true, // 必須輸入標題
                            maxlength: 100  // 最大長度100字
                        },
                        ctl00$ContentPlaceHolder1$txtContent1: { required: true } // 必須輸入內容
                    }
                });

                // Autocomplete 設定
                $("#txtPermit")
                    .on("keydown", function (event) {
                        // 當選中項目時，阻止Tab鍵跳轉
                        if (event.keyCode === $.ui.keyCode.TAB && $(this).autocomplete("instance").menu.active) {
                            event.preventDefault();
                        }
                    })
                    .autocomplete({
                        source: function (request, response) {
                            $.getJSON("scripts/getUser.aspx", { term: extractLast(request.term) }, response);
                        },
                        minLength: 2,  // 最少輸入2個字元
                        search: function () {
                            var term = extractLast(this.value);
                            if (term.length < 2) {
                                return false; // 停止搜尋
                            }
                        },
                        focus: function () {
                            return false; // 阻止focus時的預設行為
                        },
                        select: function (event, ui) {
                            $('#hfPermitUser').val($('#hfPermitUser').val() + ui.item.id + ",");
                            var terms = split(this.value);
                            terms.pop(); // 移除當前輸入
                            terms.push(ui.item.value); // 新增選中的項目
                            terms.push(""); // 增加一個佔位符來添加結尾的逗號
                            this.value = terms.join(", ");
                            return false;
                        }
                    });
            });

            // 音頻插件初始化
            audiojs.events.ready(function () {
                audiojs.createAll();
            });

            // 處理課程選擇變更
            function func_chCourse(strID) {
                if (strID == "0000000001") {
                    $('#ddl_CourseDate').prop("disabled", false); // 當課程ID為0000000001時啟用日期選擇
                }
            }

            // 課程日期選擇後自動更新標題
            function func_chCourseDate() {
                var strValue = $("#ddl_CourseDate option:selected").text();
                var TmpArray = strValue.split("-");
                $('#txtTitle').val(TmpArray[1]); // 使用選擇的日期來填充標題
            }

            // 分割字串
            function split(val) {
                return val.split(/,\s*/); // 根據逗號和空格分割
            }

            // 取得最後一個字串項目
            function extractLast(term) {
                return split(term).pop();
            }

            // 處理法脈類別選擇變更
            function func_changedharma() {
                var strValue = $("#ddl_dharma option:selected").text();
                if (strValue == "其他") {
                    $('#txtDharmaOther').prop("disabled", false); // 如果選擇其他，啟用其他輸入框
                } else {
                    $('#txtDharmaOther').prop("disabled", true).val(''); // 否則禁用並清空
                }
            }

            // 狀態改變提示
            function func_changestatus(strValue) {
                alert(strValue); // 當狀態改變時彈出提示
            }
        </script>

    </form>

</body>
</html>
