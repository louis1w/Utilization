<%@ Page Title="公告欄" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BulletinBoard.aspx.cs" Inherits="Utilization.BulletinBoard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div style="height: 420px; overflow: auto; width: 924px; background-color: #ffffcc;" 
        id="AllMsg" align="center">  
<h1>公告欄(BulletinBoard)</h1>  
<p></p>            
<asp:Label ID="Label1" runat="server" Font-Size="Large" Width="904px">請由  "關於"--->"網站管理"(admin.aspx)  張貼公告</asp:Label>
</div>
</asp:Content>
