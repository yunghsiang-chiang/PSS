<%@ Page Title="" Language="C#" MasterPageFile="~/Attendance.Master" AutoEventWireup="true" CodeBehind="TEST.aspx.cs" Inherits="purchase_sale_storeroom.TEST" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:TextBox ID="textGmail" runat="server" placeholder="Username or Gmail"></asp:TextBox>
        <asp:Button ID="btnAuthorize" runat="server" Text="Google Authorize" OnClick="btnAuthorize_Click" />
        <asp:Button ID="btnUpdateEvent" runat="server" Text="Update Calendar Evnet" OnClick="btnUpdateEvent_Click" />
        <asp:Button ID="btnDeleteEvent" runat="server" Text="Delete Calendar Event" OnClick="btnDeleteEvent_Click" />
    </div>
</asp:Content>
