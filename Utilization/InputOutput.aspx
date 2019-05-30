<%@ Page Title="進出料紀錄/登記" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InputOutput.aspx.cs" Inherits="Utilization.MaterialStatus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="GridView1" runat="server" EmptyDataText="資料表格保留連結ERP系統/進出料紀錄" 
        Height="94px" HorizontalAlign="Center">
        <Columns>
            <asp:BoundField HeaderText="進/出料紀錄" />
        </Columns>
</asp:GridView>
<div align="center">
        <asp:Label ID="Label1" runat="server" 
        Text="或轉址 &nbsp;&nbsp;連結ERP系統,<br>轉址範例<br> http://www.yahoo.com.tw" >
        </asp:Label>
        <a href="http://www.yahoo.com.tw">點擊後轉址</a>
</div>
    
</asp:Content>
