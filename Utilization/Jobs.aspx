<%@ Page Title="製令派工單" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Jobs.aspx.cs" Inherits="Utilization.Jobs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent"  runat="server">
    <div style="height: 670px; overflow: auto; width: 926px; background-color: #ffffcc;" 
         align="center">
    <asp:DropDownList ID="DropDownList1" runat="server" Height="32px" Width="220px" 
            AutoPostBack="True" onselectedindexchanged="DropDownList1_SelectedIndexChanged">
          <asp:ListItem>.</asp:ListItem>
        <asp:ListItem>..</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="DropDownList3" runat="server" Height="32px" Width="220px" 
            AutoPostBack="True" Font-Size="Medium" 
        onselectedindexchanged="DropDownList3_SelectedIndexChanged">
          <asp:ListItem>empty_form1.doc</asp:ListItem>
          <asp:ListItem>空白製令單.doc</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="Button1" runat="server" Text="讀取" Width="131px" 
            onclick="Button1_Click" Font-Size="Large" />    
        <asp:Label ID="Label_Path" runat="server" BackColor="Yellow" 
            Height="24px" Width="623px" Font-Bold="True" Font-Size="Medium"></asp:Label>
         <hr/> 
    <iframe id=iframe src="" style="width: 924px; height: 620px"></iframe>
    <script = "text/javascript">
        //<script = "text/javascript">  = 之間要有兩個空白
        history.forward();
        //改變 IFRAME 元素內的網頁
        function chgFrame(url) {
            var o = document.getElementById('iframe');
            o.src = url;
            // o.contentWindow.location.href=url;
            //frames[1].location.href=url;
        }
        </script>
  </div>
</asp:Content>
