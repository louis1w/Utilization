<%@ Page Title="庫存紀錄/登記" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Warehouse.aspx.cs" Inherits="Utilization.MaterialOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="GridView1" runat="server" EmptyDataText="資料表格保留連結ERP系統/庫存記錄" 
        Height="94px" HorizontalAlign="Center">
        <Columns>
            <asp:BoundField HeaderText="庫存紀錄" />
        </Columns>
</asp:GridView>
<div align="center">
        <asp:Label ID="Label1" runat="server" 
        Text="或轉址 &nbsp;&nbsp;連結ERP系統,<br>轉址範例<br> http://www.quaser.com/" >
        </asp:Label>
        <a href="http://www.quaser.com/">點擊後轉址</a>
</div>
</asp:Content>
