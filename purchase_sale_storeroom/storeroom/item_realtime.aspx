<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="item_realtime.aspx.cs" Inherits="purchase_sale_storeroom.storeroom.item_realtime" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>

</script>
    <main>
        <div class="row">
            庫存資訊 內容,<br />
            已先登入:預設呈現所屬慈場庫房 資訊<br />
        </div>
        <div class="row">
            <section class="col-md-3" aria-labelledby="left_Title">
                <h2 id="left_Title">條件快篩</h2>
                <asp:Panel ID="p_quick_filter" runat="server">
                    快篩區<br />
                </asp:Panel>
            </section>
            <section class="col-md-9" aria-labelledby="right_Title">
                <h2 id="right_TItle">庫存資訊</h2>
                <asp:Panel ID="p_content" runat="server">
                    內容區<br />
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
                if (className == myRadio.value || className == "標題") {
                    $(this).css("display", "table-row"); /* 顯示 */
                } else {
                    $(this).css("display", "none"); /* 隱藏 */
                }
            })
        }
    </script>
</asp:Content>
