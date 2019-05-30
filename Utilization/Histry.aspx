<%@ Page Title="操作歷史/警告訊息" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Histry.aspx.cs" Inherits="Utilization.Histry" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:DropDownList ID="DropDownList1" runat="server" Height="30px" Width="220px" 
        AutoPostBack="True" onselectedindexchanged="DropDownList1_SelectedIndexChanged">
        <asp:ListItem>Test1</asp:ListItem>
        <asp:ListItem>Test2</asp:ListItem>
    </asp:DropDownList>    
    <asp:Label ID="Label_ConnIP" runat="server" BackColor="Yellow" 
        Height="22px" Width="200px" Font-Bold="True" Font-Size="Medium"></asp:Label>  
    <asp:Label ID="Label1" runat="server" Height="22px" Text="讀取操作記錄筆數: "></asp:Label>  
    <asp:DropDownList ID="DropDownList2" runat="server" Height="30px" Width="100px" 
        onselectedindexchanged="DropDownList2_SelectedIndexChanged" 
        AutoPostBack="True">
        <asp:ListItem>100</asp:ListItem>
        <asp:ListItem>200</asp:ListItem>
        <asp:ListItem>300</asp:ListItem>
        <asp:ListItem>400</asp:ListItem>
        <asp:ListItem>500</asp:ListItem>
        <asp:ListItem>1000</asp:ListItem>
        <asp:ListItem>2000</asp:ListItem>
        <asp:ListItem>3000</asp:ListItem>
        <asp:ListItem>5000</asp:ListItem>
        <asp:ListItem>7000</asp:ListItem>
        <asp:ListItem>8000</asp:ListItem>
    </asp:DropDownList>
    <asp:Label ID="Label_OpNum" runat="server" BackColor="#FFFF99" Font-Bold="True" 
        Font-Size="Medium" Height="24px" Text="100" Width="100px"></asp:Label>
    <asp:Button ID="Button_Activate" runat="server" onclick="Button_Activate_Click" 
        Text="歷史記錄未開啟???" Width="132px"
        OnClientClick="return confirm('開啟歷史記錄後需要重新啟動才能生效\n你確定要開啟嗎?')" 
        Visible="False" />
    <hr />
    <asp:Table ID="Table1" runat="server" HorizontalAlign="Center" 
        Width="920px">         
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell1" runat="server">
            <asp:GridView ID="GridView1" runat="server" EmptyDataText="No Data !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center" ShowHeader="False"
              onprerender="GridView1_PreRender">
            </asp:GridView>
            </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server">           
            <asp:GridView ID="GridView2" runat="server" EmptyDataText="No Operator Msg !!!" ShowHeader="False"
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center">
            </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>    
    <hr />
    <asp:GridView ID="GridView3" runat="server" HorizontalAlign="Center" 
        Width="920px" onprerender="GridView3_PreRender">
        <HeaderStyle HorizontalAlign="Left" />
        <RowStyle HorizontalAlign="Left" />
    </asp:GridView>    
    <asp:GridView ID="GridView4" runat="server" HorizontalAlign="Center" 
        Width="920px" onprerender="GridView4_PreRender">
        <HeaderStyle HorizontalAlign="Left" />
        <RowStyle HorizontalAlign="Left" />
    </asp:GridView>    
    <asp:DropDownList ID="DropDownList3" runat="server" 
        Width="926px" AutoPostBack="True" 
        onselectedindexchanged="DropDownList3_SelectedIndexChanged">
        <asp:ListItem>ALL</asp:ListItem>
        <asp:ListItem>MDI KEY</asp:ListItem>
        <asp:ListItem>Alarm</asp:ListItem>
        <asp:ListItem>Power on</asp:ListItem>
        <asp:ListItem>Power off</asp:ListItem>
        <asp:ListItem>Parameter</asp:ListItem>
        <asp:ListItem>Operator&#39;s message</asp:ListItem>
        <asp:ListItem>Date/Time</asp:ListItem>
        <asp:ListItem>Alarm message</asp:ListItem>
        <asp:ListItem>Alarm add_Info</asp:ListItem>
        <asp:ListItem>Work offset</asp:ListItem>
        <asp:ListItem>Tool offset</asp:ListItem>
        <asp:ListItem>Signal</asp:ListItem>
        <asp:ListItem>Macro</asp:ListItem>
        <asp:ListItem>ScreenChange</asp:ListItem>
    </asp:DropDownList>
    <asp:GridView ID="GridView5" runat="server" HorizontalAlign="Center" 
        Width="920px" onprerender="GridView5_PreRender" EmptyDataText="以上分類操作記錄筆數:  0">
        <HeaderStyle HorizontalAlign="Left" />
        <RowStyle HorizontalAlign="Left" />
    </asp:GridView>
</asp:Content>
