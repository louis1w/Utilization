<%@ Page Title="量測統計" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Renishaw.aspx.cs" Inherits="Utilization.Renishaw" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <asp:DropDownList ID="DropDownList1" runat="server" Height="32px" Width="220px" 
            AutoPostBack="True" Font-Size="Medium" 
        onselectedindexchanged="DropDownList1_SelectedIndexChanged">       
    </asp:DropDownList>
    <asp:Button ID="Button1" runat="server" Text="讀取" Width="131px" 
        onclick="Button1_Click" Font-Size="Large" />  
     <hr/> 
     <asp:GridView ID="GridView1" runat="server" EmptyDataText="資料表格保留連結Renishaw統計紀錄" 
        Height="94px" HorizontalAlign="Center" Width="100%">        
</asp:GridView>
</asp:Content>
