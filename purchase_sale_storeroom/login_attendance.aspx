﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Attendance.Master" AutoEventWireup="true" CodeBehind="login_attendance.aspx.cs" Inherits="purchase_sale_storeroom.login_attendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <section>
            <div class="container">
                <div class="col-sm-12">
                    帳號:
                </div>
                <div class="col-sm-12">
                    <input id="tb_account" type="text" name="tb_account" />
                </div>
                <div>
                    密碼:
                </div>
                <div class="col-sm-12">
                    <input id="tb_password" type="text" name="tb_password" />
                </div>
                <div class="col-sm-12">
                    <asp:Button ID="bt_submit" runat="server" Text="登入" OnClick="bt_submit_Click" />
                </div>
            </div>
        </section>
    </main>
</asp:Content>
