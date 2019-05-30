<%@ Page Title="關於本網站" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="About" Codebehind="About.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .style1
        {
            font-size: large;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="height: 420px; overflow: auto; width: 924px; background-color: #ffffcc;" 
         align="center"> 
    <h2 align="center">
        關於&nbsp; 
        CNC 分散式(跨廠)監控中心</h2>
    <p align="center" class="style1">
        About CNC Distributed Control Center</p>
    <p align="center">
        <asp:Label ID="Label1" runat="server" Font-Size="Large" Width="904px"> 2013年1月 完成工作日誌,程式中心,備份中心,主軸負載&nbsp; 之後</asp:Label>       
    </p>
    </div>
</asp:Content>
