<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="entry_system.aspx.cs" Inherits="purchase_sale_storeroom.purchase.entry_system" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <section class="col-sm-10">
            <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="View1" runat="server">
                    <main>
                        <section class="col-sm-10">
                            <div class="row">
                                請選擇入庫<b style="color: forestgreen">類別</b><br />
                                <asp:RadioButtonList ID="rbl_ClassList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" OnSelectedIndexChanged="rbl_ClassList_SelectedIndexChanged" AutoPostBack="True"></asp:RadioButtonList>
                            </div>
                        </section>
                    </main>
                </asp:View>
                <asp:View ID="View2" runat="server">
                    <main>
                        <div class="row">
                            <asp:GridView ID="gv_item_append" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" OnRowDataBound="gv_item_append_RowDataBound">
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
                        <div class="row col-sm-12" >
                            <asp:Button ID="bt_submit" runat="server" Text="入庫" OnClick="bt_submit_Click" />
                        </div>
                        <hr />
                        <div class="row col-sm-12">
                            <asp:Button ID="bt_create_item" runat="server" Text="創新項目入庫" OnClick="bt_create_item_Click" />
                        </div>
                    </main>
                </asp:View>
            </asp:MultiView>
        </section>
    </main>
    <script>
        $('td').each(function () { /* Jquery 尋找 tr */
            const className = $(this).attr('class'); /* Jquery 尋找 td 的 class 屬性,沒有的話會回傳 沒有*/
            if (className == "隱藏") {
                $(this).css("display", "none"); /* 隱藏 */
            } 
        })
        $('th').each(function () { /* Jquery 尋找 tr */
            const className = $(this).attr('class'); /* Jquery 尋找 th 的 class 屬性,沒有的話會回傳 沒有*/
            if (className == "標題") {
                $(this).css("display", "none"); /* 隱藏 */
            }
        })
    </script>
</asp:Content>
