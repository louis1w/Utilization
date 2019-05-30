<%@ Page Title="程式中心" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
MaintainScrollPositionOnPostback="true" CodeBehind="ProgCenter.aspx.cs" Inherits="Utilization.CallHelp"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" >

    <asp:DropDownList ID="DropDownList1" runat="server" Height="30px" Width="220px" 
        AutoPostBack="True" onselectedindexchanged="DropDownList1_SelectedIndexChanged">
        <asp:ListItem>Test1</asp:ListItem>
        <asp:ListItem>Test2</asp:ListItem>
    </asp:DropDownList>
    <asp:Label ID="Label_ConnIP" runat="server" BackColor="Yellow" 
        Height="22px" Width="200px" Font-Bold="True" Font-Size="Medium"></asp:Label>    
    <asp:Label ID="Label1" runat="server" BackColor="#FFFF66" Height="20px" 
        Width="928px"></asp:Label>
    <br />
    <asp:Panel ID="Panel1" runat="server" Height="240px" ScrollBars="Auto" MaintainScrollPositionOnPostBack ="True">
    <asp:GridView ID="GridView1" runat="server" 
        EmptyDataText="Can't find Prog. in the directory" Height="240px" HorizontalAlign="Center" 
        Width="928px" AutoGenerateColumns="False" 
            onselectedindexchanging="GridView1_SelectedIndexChanging">
        <Columns>
            <asp:CommandField ShowSelectButton="True" SelectText="SEL">
            <ItemStyle Width="30px" />
            </asp:CommandField>
            <asp:BoundField DataField="selected" HeaderText="v">
            <ItemStyle HorizontalAlign="Center" Width="30px" />
            </asp:BoundField>
            <asp:BoundField HeaderText="Program name" DataField="filename">
            <ItemStyle Width="150px" />
            </asp:BoundField>
            <asp:BoundField DataField="remark" HeaderText="Comment" />
            <asp:BoundField DataField="size" HeaderText="Size">
            <ItemStyle Width="100px" />
            </asp:BoundField>
            <asp:BoundField DataField="date" HeaderText="Date">
            <ItemStyle Width="150px" />
            </asp:BoundField>
        </Columns>
        <HeaderStyle Height="12px" />
        <SelectedRowStyle BackColor="#FF99CC" />
    </asp:GridView>
    </asp:Panel>
    <asp:Table ID="Table1" runat="server" Width="928px">
    <asp:TableRow ID="TableRow1" runat="server">
    <asp:TableCell ID="TableCell1" runat="server"  HorizontalAlign="Left">
        <asp:Button ID="Button3" runat="server" Text="SEL ALL" Width="66px" Height="30px"
         onclick="Button3_Click" HorizontalAlign="Left"/>
        <asp:DropDownList ID="DropDownList2" runat="server" 
        onselectedindexchanged="DropDownList2_SelectedIndexChanged" Visible="False" 
        Width="155px" AutoPostBack="True" HorizontalAlign="Center">
        <asp:ListItem Value="//CNC_MEM/SYSTEM/">SYSTEM</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/MTB1/">MTB1</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/MTB2/">MTB2</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/USER/LIBRARY/">LIBRARY</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/USER/PATH1/" Selected="True">PATH1</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/USER/PATH2/">PATH2</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/">root</asp:ListItem>
         </asp:DropDownList>     
    </asp:TableCell>
    <asp:TableCell ID="TableCell5" runat="server"  HorizontalAlign="Left">
        <asp:Button ID="Button2" runat="server" Text="▲" Width="150px" Height="30px" 
        onclick="Button2_Click" />
    </asp:TableCell>   
     <asp:TableCell ID="TableCell7" runat="server" HorizontalAlign="Left">
     <asp:Button ID="Button1" runat="server" Text="▼" Width="150px" Height="30px" 
        onclick="Button1_Click"  /> 
    </asp:TableCell>
    </asp:TableRow>
    </asp:Table>
    <asp:Panel ID="Panel2" runat="server" Height="240px" ScrollBars="Auto" MaintainScrollPositionOnPostback="true">
    <asp:GridView ID="GridView2" runat="server" 
        EmptyDataText="Can't find CNC Prog." Height="240px" HorizontalAlign="Center" 
        Width="928px" AutoGenerateColumns="False" 
            onselectedindexchanging="GridView2_SelectedIndexChanging">
        <Columns>
            <asp:CommandField ShowSelectButton="True" SelectText="SEL">
            <ItemStyle Width="30px" />
            </asp:CommandField>
            <asp:BoundField DataField="selected" HeaderText="v">
            <ItemStyle HorizontalAlign="Center" Width="30px" />
            </asp:BoundField>
            <asp:BoundField DataField="filename" HeaderText="Program name">
            <ItemStyle Width="150px" />
            </asp:BoundField>
            <asp:BoundField HeaderText="Comment" DataField="remark"></asp:BoundField>
            <asp:BoundField DataField="size" HeaderText="Size">
            <ItemStyle Width="100px" />
            </asp:BoundField>
            <asp:BoundField DataField="date" HeaderText="Date">
            <ItemStyle Width="150px" />
            </asp:BoundField>
        </Columns>
        <HeaderStyle Height="12px" />
        <SelectedRowStyle BackColor="#FF99CC" />
    </asp:GridView>
    </asp:Panel>
</asp:Content>
