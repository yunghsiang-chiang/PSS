<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="create_new_class.aspx.cs" Inherits="purchase_sale_storeroom.purchase.create_new_class" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <section>
            <h2>類別ID不可修改,系統自動填寫</h2>
            <p>新類別ID</p>
            <div class="col-sm-12">
                <asp:TextBox ID="tb_classID" runat="server">

                </asp:TextBox>
            </div>
            <p>新類別名稱(限定使用中文)</p>
            <div class="col-sm-12">
                <asp:TextBox ID="tb_className" runat="server">

                </asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tb_className" ErrorMessage="RequiredFieldValidator">類別的名稱尚未輸入</asp:RequiredFieldValidator>
            </div>
            <p>資料庫-資料表名稱(限定使用英文)</p>
            <div class="col-sm-12">
                <asp:TextBox ID="tb_cDatatable" runat="server">

                </asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tb_cDatatable" ErrorMessage="RequiredFieldValidator">類別的英文尚未輸入</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="tb_cDatatable" ErrorMessage="RegularExpressionValidator" ValidationExpression="^[a-z]+$">請輸入小寫英文</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-12">
                <div class="row">
                    <div class="col-sm-6">
                        <asp:CheckBox ID="cb_gender_size" runat="server" Text="創立包含尺寸/性別欄位" />
                    </div>
                    <div class="col-sm-6">
                        <asp:Button ID="bt_submit" runat="server" Text="新增類別" OnClick="bt_submit_Click" />
                    </div>
                </div>
            </div>
            <%--            <asp:Panel ID="p_create" runat="server">

            </asp:Panel>--%>
        </section>
    </main>
</asp:Content>
