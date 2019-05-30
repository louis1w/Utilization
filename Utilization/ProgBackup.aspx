<%@ Page Title="備份中心" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProgBackup.aspx.cs" Inherits="Utilization.ProgBackup" %>
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
    <br />
    <asp:Label ID="Label1" runat="server" BackColor="#FFFF66" Height="20px" 
        Width="926px"></asp:Label>
    <hr />
    <asp:Table ID="Table3" runat="server" HorizontalAlign="Center" 
        Width="920px" GridLines="Both">
        <asp:TableRow ID="TableRow3" runat="server"> 
        <asp:TableCell ID="TableCell5" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
        <asp:Button ID="Button1" runat="server" Text="ALL Prog.Backup" Width="150px" Height="30px" 
        onclick="Button1_Click" /> 
    </asp:TableCell>   
    <asp:TableCell ID="TableCell7" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
        <asp:Button ID="Button2" runat="server" Text="WORK Backup" Width="150px" Height="30px" 
        onclick="Button2_Click" />
    </asp:TableCell> 
    <asp:TableCell ID="TableCell8" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
        <asp:Button ID="Button3" runat="server" Text="OFFSET Backup" Width="150px" Height="30px" 
        onclick="Button3_Click" />
    </asp:TableCell> 
    <asp:TableCell ID="TableCell9" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
        <asp:Button ID="Button4" runat="server" Text="Parameter Backup" Width="150px" Height="30px" 
        onclick="Button4_Click" />
    </asp:TableCell>
    <asp:TableCell ID="TableCell11" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
        <asp:Button ID="Button5" runat="server" Text="Macro Backup" Width="150px" Height="30px" 
        onclick="Button5_Click" />
    </asp:TableCell>
      <asp:TableCell ID="TableCell10" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
         <asp:Button ID="Button6" runat="server" Text="Pitch Backup" Width="150px" Height="30px" 
        onclick="Button6_Click" />
    </asp:TableCell>     
    </asp:TableRow>
    </asp:Table>
    <hr />  
    <asp:GridView ID="GridView1" runat="server" EmptyDataText="No Data !!! 請選擇備份項目" 
        AutoGenerateColumns="False" Width="920px" 
        onprerender="GridView1_PreRender">
        <Columns>
            <asp:BoundField DataField="ConnStatus" HeaderText="Backup Status" >
            <ItemStyle HorizontalAlign="Center" Width="150px" />
            </asp:BoundField>
            <asp:BoundField DataField="ConnType" HeaderText="Backup Items" >
            <ItemStyle HorizontalAlign="Center" Width="150px" />
            </asp:BoundField>
            <asp:BoundField DataField="ConnFile" HeaderText="Backup Filename" >
            <ItemStyle Width="150px" />
            </asp:BoundField>
            <asp:BoundField DataField="ConnDate" HeaderText="Backup Date" />
        </Columns>
    </asp:GridView>    
 </asp:Content>