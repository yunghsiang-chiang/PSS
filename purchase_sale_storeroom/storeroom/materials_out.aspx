<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="materials_out.aspx.cs" Inherits="purchase_sale_storeroom.storeroom.materials_out" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <main>
        <section>
            <p>庫房撿料內容</p>
            <div class="accordion" id="accordionExample" >
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingOne">
                        <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseOne" aria-expanded="true" aria-controls="collapseOne">
                            庫存資訊_撿料作業 使用『＋』『－』按鈕方便使用手機撿料
                        </button>
                    </h2>
                    <div id="collapseOne" class="accordion-collapse collapse show" aria-labelledby="headingOne" data-bs-parent="#accordionExample">
                        <div class="accordion-body">
                            <strong>登入前此頁無資訊!登入後使用者僅能對所屬慈場庫房進行撿料作業!</strong>快篩=類別按鈕,<br />
                            <div class="row">
                                <section class="col-md-3" aria-labelledby="left_Title">
                                    <h2 id="left_Title">條件快篩</h2>
                                    <asp:Panel ID="p_quick_filter" runat="server">
                                        快篩區<br />
                                    </asp:Panel>
                                </section>
                                <section class="col-md-9" aria-labelledby="right_Title">
                                    <h2 id="right_TItle">庫存資訊</h2>
                                    <asp:Panel ID="p_content" runat="server" CssClass="myPanelClass">
                                        <div class="row justify-content-around">
                                            <div class="col-2">內容區</div>
                                            <div class="col-2">
                                                <asp:Button ID="bt_temp_submit" runat="server" Text="揀料提交" OnClick="bt_temp_submit_Click" /></div>
                                        </div>
                                        <asp:GridView ID="gv_item_realtime" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" OnRowDataBound="gv_item_realtime_RowDataBound">
                                            <AlternatingRowStyle BackColor="White" />
                                            <EditRowStyle BackColor="#2461BF" />
                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <RowStyle BackColor="#EFF3FB" />
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                            <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </section>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingTwo">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseTwo" aria-expanded="false" aria-controls="collapseTwo">
                            撿料提醒_使用自動化列印出貨單_出貨單物品清單</button>
                    </h2>
                    <div id="collapseTwo" class="accordion-collapse collapse" aria-labelledby="headingTwo" data-bs-parent="#accordionExample">
                        <div class="accordion-body">
                            <strong>登入前此頁無資訊!登入後使用者僅能對所屬慈場庫房進行撿料作業!</strong>若有使用"每日自動列印出貨單",此區會顯示出貨單對應物品清單,方便撿料作業對比使用.
                        </div>
                    </div>
                </div>
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingThree">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseThree" aria-expanded="false" aria-controls="collapseThree">
                            撿料確認
                        </button>
                    </h2>
                    <div id="collapseThree" class="accordion-collapse collapse" aria-labelledby="headingThree" data-bs-parent="#accordionExample">
                        <div class="accordion-body">
                            <strong>登入前此頁無資訊!登入後使用者僅能對所屬慈場庫房進行撿料作業!</strong>此區提供 撿料作業數量vs出貨單物品清單對比,讓同修進行最後一次確認,點選<b style="color: forestgreen">"確認撿料"</b>後即完成撿料作業<br />
                            <asp:GridView ID="gv_temp_pick" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None">
                                <AlternatingRowStyle BackColor="White" />
                                <EditRowStyle BackColor="#2461BF" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </main>
    <script>
        var currentValue = 0;
        var table, tbody, i, rowLen, row, j, colLen, cell;
        table = document.getElementById("MainContent_gv_item_realtime");
        function handleClick(myRadio) {
            //console.log('Old value: ' + currentValue); /* 點選前的數值 */
            //console.log('New value: ' + myRadio.value); /* 點選後的數值 */
            currentValue = myRadio.value; /* 點選後數值 覆蓋 點選前數值 */
            var i;
            tbody = table.tBodies[0]; /* 表格內容 */
            var conform_qty = 0;
            for (i = 1, rowLen = tbody.rows.length; i < rowLen; i++) { /* 第一行 欄位名稱 跳過 */
                row = tbody.rows[i]; /* 表格 每一行 隨者 變數 i */
                if (row.cells[0].innerHTML == myRadio.value) { /* 第一欄位資訊 class_name */
                    conform_qty += 1;
                }
            }
            console.log(conform_qty);
            $('tr').each(function () { /* Jquery 尋找 tr */
                const className = $(this).attr('class'); /* Jquery 尋找 tr 的 class 屬性,沒有的話會回傳 沒有*/
                if (className == myRadio.value || className=="標題") {
                    $(this).css("display", "table-row"); /* 顯示 */
                } else {
                    $(this).css("display", "none"); /* 隱藏 */
                }
            })
        }
        window.document.body.onbeforeunload = function () {
            return '★★ 您尚未將編輯過的表單資料送出，請問您確定要離開網頁嗎？ ★★';
        }
    </script>

</asp:Content>
