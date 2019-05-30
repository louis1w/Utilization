<%@ Page Title="網站管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="admin.aspx.cs" Inherits="Utilization.admin"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="Label1" runat="server" BackColor="#FFFF66" Height="27px" Width="930px"
        Style="text-align: center" Font-Size="Medium"></asp:Label>
    <hr />
    <asp:Button ID="Button2" runat="server" Text="DEMO Mode (To protect [Web Site] turn on DEMO Mode--->Green)" Width="930px" OnClick="Button2_Click" />    
    <br />
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Input 1~4" ControlToValidate="TextBox0" BackColor="Yellow" Display="Dynamic"></asp:RequiredFieldValidator>
    <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Input 1~4" ControlToValidate="TextBox0" MinimumValue="1" MaximumValue="4" BackColor="Yellow" Display="Dynamic"></asp:RangeValidator>  
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Input Plant_I name,Empty space is not allowed" ControlToValidate="TextBox1" BackColor="Yellow" Display="Dynamic"></asp:RequiredFieldValidator>          
    <asp:RadioButtonList ID="RadioButtonList1" runat="server" 
                    RepeatDirection="Horizontal" AutoPostBack="True" 
                     onselectedindexchanged="RadioButtonList1_SelectedIndexChanged" >
                    <asp:ListItem>自動清除七日前歷史資料</asp:ListItem>
                    <asp:ListItem Selected="True">手動清除七日前歷史資料</asp:ListItem>
    </asp:RadioButtonList>
    <asp:Table ID="Table1" runat="server" GridLines="Both" Width="650px" HorizontalAlign="Center">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" Width="70px">預設廠區 </asp:TableCell>
            <asp:TableCell runat="server" Width="70px">第四廠區</asp:TableCell>
            <asp:TableCell runat="server" Width="70px">第三廠區</asp:TableCell>
            <asp:TableCell runat="server" Width="70px">第二廠區</asp:TableCell>
            <asp:TableCell runat="server" Width="70px">第一廠區</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" Width="70px">
                <asp:TextBox ID="TextBox0" runat="server" ToolTip="請輸入1~4" AutoPostBack="True">2</asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server" Width="70px">
                <asp:TextBox ID="TextBox4" runat="server" ToolTip="請填第四廠區名稱" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server" Width="70px">
                <asp:TextBox ID="TextBox3" runat="server" ToolTip="請填第三廠區名稱" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server" Width="70px">
                <asp:TextBox ID="TextBox2" runat="server" ToolTip="請填第二廠區名稱" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server" Width="70px">
                <asp:TextBox ID="TextBox1" runat="server" ToolTip="請填第一廠區名稱,勿空白" AutoPostBack="True">第一廠區</asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">連結網址</asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox9" runat="server" ToolTip="第四廠區連結網址,請填IP就好,不用http開頭" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox8" runat="server" ToolTip="第三廠區連結網址,請填IP就好,不用http開頭" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox7" runat="server" ToolTip="第二廠區連結網址,請填IP就好,不用http開頭" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox6" runat="server" ToolTip="第一廠區連結網址,請填IP就好,不用http開頭" AutoPostBack="True">127.0.0.1</asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">圖片</asp:TableCell>
            <asp:TableCell runat="server">
                <asp:Image ID="Image4" runat="server" Height="32px" ImageUrl="~/Ut_Data/images/Quaser1.png"
                    Width="92px" ImageAlign="AbsMiddle" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:Image ID="Image3" runat="server" Height="32px" ImageUrl="~/Ut_Data/images/Quaser1.png"
                    Width="92px" ImageAlign="AbsMiddle" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:Image ID="Image2" runat="server" Height="32px" ImageUrl="~/Ut_Data/images/Quaser1.png"
                    Width="92px" ImageAlign="AbsMiddle" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:Image ID="Image1" runat="server" Height="32px" ImageUrl="~/Ut_Data/images/Quaser1.png"
                    Width="92px" ImageAlign="AbsMiddle" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">圖片檔名</asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox13" runat="server" ToolTip="第四廠區LOGO檔名" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox12" runat="server" ToolTip="第三廠區LOGO檔名" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox11" runat="server" ToolTip="第二廠區LOGO檔名" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox10" runat="server" ToolTip="第一廠區LOGO檔名" AutoPostBack="True">Quaser1.png</asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server" HorizontalAlign="Left" VerticalAlign="Middle">
            <asp:TableCell runat="server" Width="100px" HorizontalAlign="Left">
            標題(Title)
            </asp:TableCell>
            <asp:TableCell runat="server" Width="100px" HorizontalAlign="Center">
                <asp:TextBox ID="TextBox5" runat="server" ToolTip="標題名稱" AutoPostBack="True">(跨廠)監控中心</asp:TextBox>
            </asp:TableCell>
            <asp:TableCell ID="TableCell1" runat="server" HorizontalAlign="Center" Width="150px">
                <asp:Button ID="Button1" runat="server" Text="SAVE" Width="150px" OnClick="Button1_Click" />
            </asp:TableCell>
            <asp:TableCell runat="server" BackColor="Yellow">輸入完畢,請按ENTER</asp:TableCell>
            <asp:TableCell runat="server" BackColor="Yellow">再按SAVE按紐</asp:TableCell>
        </asp:TableRow>
    </asp:Table>   
    <hr />
    <asp:Table ID="Table2" runat="server" GridLines="Both" HorizontalAlign="Center" Width="800px">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">庫存記錄/登記__連結網址</asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox16" runat="server" Width="386px" ToolTip="庫存記錄/登記__連結網址(__請以http://開頭)"
                    AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">進出料記錄/登記__連結網址</asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="TextBox17" runat="server" Width="386px" ToolTip="進出料記錄/登記__連結網址(__請以http://開頭)"
                    AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" Width="395px" HorizontalAlign="Center">
                <asp:TextBox ID="TextBox14" runat="server" Width="386px" ToolTip="頁尾公司名稱" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell runat="server" Width="395px" HorizontalAlign="Center">
                <asp:TextBox ID="TextBox15" runat="server" Width="386px" ToolTip="頁尾公司名稱連結網址(__請以http://開頭)"
                    AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <hr />
    <asp:Label ID="Label2" runat="server" BackColor="#FFFFCC" Height="27px" Width="930px"
        Style="text-align: center" Font-Size="Medium">How to identify CNC ?</asp:Label>
    <asp:Panel ID="Panel1" runat="server" Height="120px" ScrollBars="Auto" MaintainScrollPositionOnPostback="true">
   <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            Font-Size="Medium" Width="920px"  Height="120px" EmptyDataText="Can't find control table !!!" 
            onprerender="GridView1_PreRender" HorizontalAlign="Center"
            OnRowCancelingEdit="GridView1_RowCancelingEdit" 
            OnRowDeleting ="GridView1_RowDeleting"
            OnRowEditing="GridView1_RowEditing" 
            OnRowUpdating ="GridView1_RowUpdating">            
            <Columns>
                <asp:BoundField DataField="ConnStatus" HeaderText="ConnStatus" ReadOnly="True" >
                <ItemStyle Width="70px" />
                </asp:BoundField>
                <asp:BoundField DataField="ConnSeries" HeaderText="ID(AUTO)" >
                <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="ConnIP" HeaderText="ConnectingIP-Host"  >                
                </asp:BoundField>
                 <asp:BoundField DataField="CNC" HeaderText="CNC_Picture?" >
                <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField> 
                <asp:BoundField DataField="Monitoring" HeaderText="Monitoring?"  >
                <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField> 
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                            CommandName="Delete" Text="DEL"
                            OnClientClick="return confirm('Are you sure to delete this one?\nDo not press F5(refresh) to avoid delete again.\n再一次確認,您要刪除這筆資料嗎?\n刪除後請勿做刷新畫面動作(F5)\n以免多刪除一次')"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True" EditText="EDIT" CancelText="Cancel" 
                    UpdateText="Update" />
            </Columns>
        </asp:GridView>        
      </asp:Panel>
</asp:Content>
