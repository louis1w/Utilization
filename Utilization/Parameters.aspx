<%@ Page Title="參數/診斷/PMC資訊" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Parameters.aspx.cs" Inherits="Utilization.Parameters"  %>
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
    <asp:Table ID="Table2" runat="server" HorizontalAlign="Center" 
        Width="920px" GridLines="Horizontal">         
        <asp:TableRow ID="TableRow2" runat="server" HorizontalAlign="Center">
            <asp:TableCell ID="TableCell3" runat="server">
               <asp:Button ID="Button_CNC" runat="server" Text="CNC Parameter" Width="200px" 
                onclick="Button_CNC_Click" />  
            </asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server">
                <asp:Button ID="Button_Diagnose" runat="server" Text="Diagnose" Width="200px" 
                 onclick="Button_Diagnose_Click" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell5" runat="server">
                <asp:Button ID="Button_Address" runat="server" Text="PMC Status" Width="200px" 
                  onclick="Button_Address_Click" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server">
                <asp:Button ID="Button_PmcPara" runat="server" Text="PMC Parameter" 
                   Width="200px" onclick="Button_PmcPara_Click" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server" HorizontalAlign="Center">
            <asp:TableCell ID="TableCell7" runat="server">
                <asp:DropDownList ID="DropDownList2" runat="server" Width="100px" 
                    AutoPostBack="True" onselectedindexchanged="DropDownList2_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Label ID="Label_Drop" runat="server" Text="DropSelection" BackColor="#FFFF99" Width="100px"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell8" runat="server">
              <asp:TextBox ID="TextBox_Input" runat="server" AutoPostBack="True" 
                Width="100px" Text="0" ontextchanged="TextBox_Input_TextChanged"></asp:TextBox>
              <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="請輸入>0數字" 
                MaximumValue="99999" MinimumValue="0" Type="Integer" BackColor="#FFFF80"
                ControlToValidate="TextBox_Input"></asp:RangeValidator>  
            </asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server">
               <asp:Button ID="ButtonUP" runat="server" Text="Page_Up" Width="150px"
                onclick="ButtonUP_Click" /> 
            </asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server">
                <asp:Button ID="ButtonDown" runat="server" Text="Page_Down" 
                onclick="ButtonDown_Click" Width="150px" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <hr />    
    <asp:Table ID="Table3" runat="server" HorizontalAlign="Center" 
        Width="920px">         
        <asp:TableRow ID="TableRow4" runat="server">
            <asp:TableCell ID="TableCell11" runat="server">
           <asp:GridView ID="GridView3" runat="server" EmptyDataText="No Data !!!" 
             HorizontalAlign="Center" RowStyle-HorizontalAlign="Center" 
             Width="920px" onprerender="GridView3_PreRender">
            </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
     
</asp:Content>
