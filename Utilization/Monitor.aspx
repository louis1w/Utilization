<%@ Page Title="機台監控資訊" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Monitor.aspx.cs" Inherits="Utilization.Monitor" MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:Timer ID="Timer2" runat="server" OnTick="Timer2_Tick" Interval="8000" 
             Enabled="False">
            </asp:Timer>
    <asp:DropDownList ID="DropDownList1" runat="server" Height="32px" Width="220px" 
            AutoPostBack="True" onselectedindexchanged="DropDownList1_SelectedIndexChanged">
          <asp:ListItem>Test1</asp:ListItem>
        <asp:ListItem>Test2</asp:ListItem>
    </asp:DropDownList>
    <asp:Label ID="Label_ConnIP" runat="server" BackColor="Yellow" 
            Height="22px" Width="200px" Font-Bold="True" Font-Size="Medium"></asp:Label>
            <asp:Button ID="Button_ShowXYZ" runat="server" Height="27px" Text="顯示座標" 
        onclick="Button_ShowXYZ_Click" />
            <asp:Button ID="ButtonRefresh" runat="server" Height="27px" 
            onclick="ButtonRefresh_Click" Text="刷新畫面" Width="150px" />
        <hr/> 
    <asp:Table ID="Table5" runat="server" HorizontalAlign="Center" 
        Width="906px"> 
        <asp:TableRow ID="TableRow5"  runat="server"> 
          <asp:TableCell ID="TableCell12" runat="server">
          <asp:GridView ID="GridView11" runat="server" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center" Width="906px" Font-Size="Medium" Font-Bold="True">
            </asp:GridView>
          </asp:TableCell>
        </asp:TableRow>    
    </asp:Table>
    <asp:Table ID="Table1" runat="server" HorizontalAlign="Center" 
        Width="920px">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
            <asp:UpdatePanel ID="UpdatePanelInput" runat="server" UpdateMode="Conditional"> 
            <ContentTemplate>
            <asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="1000" 
             Enabled="False">
            </asp:Timer>
            <asp:GridView ID="GridView1" runat="server" EmptyDataText="No Data !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center" 
              onprerender="GridView1_PreRender">
            </asp:GridView>
            </ContentTemplate>
            </asp:UpdatePanel>
            </asp:TableCell>
            <asp:TableCell runat="server">           
             <asp:GridView ID="GridView8" runat="server" EmptyDataText="No Operator Msg !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Left" ShowHeader="False">
            </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server"> 
        <asp:TableCell ID="TableCell1" runat="server"> 
        <asp:GridView ID="GridView3" runat="server" EmptyDataText="No Data !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center">
            </asp:GridView>
        </asp:TableCell>
        <asp:TableCell ID="TableCell2" runat="server"> 
        <asp:GridView ID="GridView2" runat="server" EmptyDataText="No Data !!!" ShowHeader="False"
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center">
            </asp:GridView>
        <asp:GridView ID="GridView4" runat="server" EmptyDataText="No Data !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center">
            </asp:GridView>
        </asp:TableCell>
        </asp:TableRow>        
    </asp:Table>
    <asp:Table ID="Table2" runat="server" HorizontalAlign="Center" 
        Width="920px">
        <asp:TableRow ID="TableRow2" runat="server"> 
         <asp:TableCell ID="TableCell3" runat="server" VerticalAlign="Top" >
         <asp:GridView ID="GridView6" runat="server" EmptyDataText="No Data !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center">
            </asp:GridView>
         <asp:GridView ID="GridView5" runat="server" EmptyDataText="No Data !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Center" ShowHeader="False">
            </asp:GridView>
         </asp:TableCell>
         <asp:TableCell ID="TableCell4" runat="server" VerticalAlign="Top">
         <asp:GridView ID="GridView7" runat="server" EmptyDataText="No Data !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Left">
            </asp:GridView>         
         </asp:TableCell>         
        </asp:TableRow>
    </asp:Table>
     <hr /> 
    <asp:Table ID="Table3" runat="server" HorizontalAlign="Center" 
        Width="920px" GridLines="Both">
        <asp:TableRow ID="TableRow3" runat="server"> 
        <asp:TableCell ID="TableCell5" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
         <asp:Button ID="Button1" runat="server" Text="Macro" Width="150px" Height="30px" 
        onclick="Button1_Click" />
    </asp:TableCell>   
    <asp:TableCell ID="TableCell7" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
    <asp:Button ID="Button2" runat="server" Text="Pitch" Width="150px" Height="30px" 
        onclick="Button2_Click" />
    </asp:TableCell> 
    <asp:TableCell ID="TableCell8" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
    <asp:Button ID="Button3" runat="server" Text="工件座標" Width="150px" Height="30px" 
        onclick="Button3_Click" />
    </asp:TableCell> 
    <asp:TableCell ID="TableCell9" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
    <asp:Button ID="Button4" runat="server" Text="刀具補償" Width="150px" Height="30px" 
        onclick="Button4_Click" />
    </asp:TableCell>
    <asp:TableCell ID="TableCell11" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
    <asp:Button ID="Button6" runat="server" Text="刀庫表" Width="150px" Height="30px" 
        onclick="Button6_Click" />
    </asp:TableCell>
      <asp:TableCell ID="TableCell10" runat="server" VerticalAlign="Top" HorizontalAlign="Center">
    <asp:Button ID="Button5" runat="server" Text="NC 程式" Width="150px" Height="30px" 
        onclick="Button5_Click" />
    </asp:TableCell>     
    </asp:TableRow>
    </asp:Table>    
    <hr />
    <asp:GridView ID="GridView10" runat="server"
               HorizontalAlign="Center" RowStyle-HorizontalAlign="Left" Width="922px" >
              <RowStyle HorizontalAlign="Center"></RowStyle>
            </asp:GridView> 
    <asp:Table ID="Table4" runat="server" HorizontalAlign="Center" 
        Width="920px" GridLines="Both">         
        <asp:TableRow ID="TableRow4"  runat="server">   
        <asp:TableCell ID="TableCell6" runat="server" VerticalAlign="Top" HorizontalAlign="Center">        
        <asp:DropDownList ID="DropDownList2" runat="server" 
        onselectedindexchanged="DropDownList2_SelectedIndexChanged" Visible="False" 
        Width="150px" AutoPostBack="True">
        <asp:ListItem Value="//CNC_MEM/SYSTEM/">SYSTEM</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/MTB1/">MTB1</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/MTB2/">MTB2</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/USER/LIBRARY/">LIBRARY</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/USER/PATH1/" Selected="True">PATH1</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/USER/PATH2/">PATH2</asp:ListItem>
        <asp:ListItem Value="//CNC_MEM/">root</asp:ListItem>
         </asp:DropDownList>            
        <asp:GridView ID="GridView9" runat="server" EmptyDataText="No Data !!!" 
              HorizontalAlign="Center" RowStyle-HorizontalAlign="Left" Width="920px" HeaderStyle-HorizontalAlign="NotSet">
              <RowStyle HorizontalAlign="Center"></RowStyle>
            </asp:GridView>
        </asp:TableCell>    
        </asp:TableRow>         
    </asp:Table>
    


</asp:Content>
