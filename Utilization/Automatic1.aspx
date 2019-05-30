<%@ Page Title="自動化---1.識別 監控CNC" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Automatic1.aspx.cs" Inherits="Utilization.Automatic1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="Label1" runat="server" BackColor="#FFFF99" Font-Size="X-Large" 
    Height="33px" 
        Text="自動化中心(可依客戶需求定製).....1. 識別 監控CNC/非監控CNC" Width="922px" 
        style="text-align: center"></asp:Label>
<asp:Label ID="Label2" runat="server" BackColor="#FFFF99" Font-Size="Medium" 
    Height="130px"
        Text="自動化步驟:<br>1. 識別 監控CNC<br>2. 控制項目定義 <br>3. 滿足1.& 2.時的(狀態與輸出條件)設定<br>4. 監控畫面設計,放入操作(自動/手動/啟動/停止)按鈕 <br>5. 反覆測試修改,直到系統穩定" Width="922px" 
        style="text-align: left"></asp:Label>       
    <asp:RadioButtonList ID="RadioButtonList1" runat="server"  
                    RepeatDirection="Horizontal" AutoPostBack="True" 
                     onselectedindexchanged="RadioButtonList1_SelectedIndexChanged" Width="100%" Height="10px" >
                    <asp:ListItem Selected="True">小圖片</asp:ListItem>
                    <asp:ListItem>大圖片</asp:ListItem>
    </asp:RadioButtonList>  
              
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            Font-Size="Medium" Width="920px" EmptyDataText="找不到監控記錄表  !!!" 
            onprerender="GridView1_PreRender" Height="125px">
            <Columns>
                <asp:BoundField DataField="ConnStatus" HeaderText="連線狀況" ReadOnly="True" >
                <ItemStyle Width="70px" />
                </asp:BoundField>
                <asp:BoundField DataField="ConnSeries" HeaderText="識別碼(自動)" ReadOnly="True">
                <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="ConnIP" HeaderText="IP-機台名稱" ReadOnly="True" >                
                </asp:BoundField>
                <asp:ImageField DataImageUrlField="CNC" DataImageUrlFormatString="~\Ut_Data\images\{0}" HeaderText="CNC" >
                    <ItemStyle HorizontalAlign="Center" /><ControlStyle Width="160" Height="120" />
                </asp:ImageField>
                <asp:BoundField DataField="Monitoring" HeaderText="監控記錄" >
                <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField> 
            </Columns>
        </asp:GridView>
</asp:Content>
