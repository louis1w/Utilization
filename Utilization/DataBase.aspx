<%@ Page Title="歷史資料查詢" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataBase.aspx.cs" Inherits="Utilization.DataBase" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="Label1" runat="server" BackColor="#FFFF99" Font-Size="X-Large" 
    Height="40px" Text="您需要什麼資料項目?請使用下面記錄控制表來做控制" Width="920px" 
        style="text-align: center"></asp:Label><br />
            <asp:DropDownList ID="DropDownList1" runat="server" Height="32px" Width="220px" 
            AutoPostBack="True" onselectedindexchanged="DropDownList1_SelectedIndexChanged">
          <asp:ListItem>127.0.0.1</asp:ListItem>
        <asp:ListItem>127.0.0.1</asp:ListItem>
    </asp:DropDownList>
    <asp:Label ID="Label_ConnIP" runat="server" BackColor="Yellow" 
            Height="24px" Width="223px" Font-Bold="True" Font-Size="Medium"></asp:Label>
    <asp:DropDownList ID="DropDownList3" runat="server" Height="32px" Width="110px" 
            AutoPostBack="True" Font-Size="Medium" 
        onselectedindexchanged="DropDownList3_SelectedIndexChanged">
          <asp:ListItem>軸負載</asp:ListItem>
          <asp:ListItem Selected="True">工件數</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="Button1" runat="server" Text="新增記錄項目" Width="131px" 
            onclick="Button1_Click" Font-Size="Large" />
    <asp:Button ID="Button3" runat="server" Text="隱藏控制表" Width="159px" 
            onclick="Button3_Click" Font-Size="Large" /><br />
   <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            Font-Size="Medium" Width="920px" EmptyDataText="找不到記錄控制表  !!!" 
            onrowdeleting="GridView1_RowDeleting" onprerender="GridView1_PreRender" 
            onrowediting="GridView1_RowEditing" 
            onrowcancelingedit="GridView1_RowCancelingEdit" 
            onrowupdating="GridView1_RowUpdating">
            <Columns>
                <asp:BoundField DataField="ConnStatus" HeaderText="連線狀況" ReadOnly="True" >
                <ItemStyle Width="70px" />
                </asp:BoundField>
                <asp:BoundField DataField="ConnSeries" HeaderText="Series" ReadOnly="True" Visible="False">
                <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="ConnIP" HeaderText="連線的_IP" ReadOnly="True" >                
                </asp:BoundField>
                <asp:BoundField DataField="RecordItem" HeaderText="記錄項目" ReadOnly="True" >
                <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Recording" HeaderText="是否記錄?" >
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
        <asp:DropDownList ID="DropDownList2" runat="server" Height="32px" Width="95px" 
            AutoPostBack="True" 
           onselectedindexchanged="DropDownList2_SelectedIndexChanged">
          <asp:ListItem>工件數</asp:ListItem>
        <asp:ListItem>需工件數</asp:ListItem>
          <asp:ListItem>總工件數</asp:ListItem>          
    </asp:DropDownList>
    <asp:LinkButton ID="LinkButtonDay7" runat="server" 
        onclick="LinkButtonDay7_Click" Visible="False">2013-02-22</asp:LinkButton>&nbsp;
    <asp:LinkButton ID="LinkButtonDay6" runat="server" 
        onclick="LinkButtonDay6_Click" Visible="False">2013-02-23</asp:LinkButton>&nbsp;
    <asp:LinkButton ID="LinkButtonDay5" runat="server" 
        onclick="LinkButtonDay5_Click" Visible="False">2013-02-24</asp:LinkButton>&nbsp;
    <asp:LinkButton ID="LinkButtonDay4" runat="server" 
        onclick="LinkButtonDay4_Click" Visible="False">2013-02-25</asp:LinkButton>&nbsp;
    <asp:LinkButton ID="LinkButtonDay3" runat="server" 
        onclick="LinkButtonDay3_Click" Visible="False">2013-02-26</asp:LinkButton>&nbsp;
    <asp:LinkButton ID="LinkButtonDay2" runat="server" 
        onclick="LinkButtonDay2_Click" Visible="False">2013-02-27</asp:LinkButton>&nbsp;
    <asp:LinkButton ID="LinkButtonDay1" runat="server" 
        onclick="LinkButtonDay1_Click" Visible="False">2013-02-28</asp:LinkButton>&nbsp;
    <asp:DropDownList ID="DropDownList4" runat="server" Height="32px" Width="100px" 
            AutoPostBack="True" 
           onselectedindexchanged="DropDownList4_SelectedIndexChanged" 
        Visible="False">
          <asp:ListItem Value="0">00:00~01:00</asp:ListItem>
        <asp:ListItem Value="1">01:00~02:00</asp:ListItem>
          <asp:ListItem Value="2">02:00~03:00</asp:ListItem>
          <asp:ListItem Value="3">03:00~04:00</asp:ListItem>
          <asp:ListItem Value="4">04:00~05:00</asp:ListItem>
          <asp:ListItem Value="5">05:00~06:00</asp:ListItem>
          <asp:ListItem Value="6">06:00~07:00</asp:ListItem>
          <asp:ListItem Value="7">07:00~08:00</asp:ListItem>
          <asp:ListItem Value="8">08:00~09:00</asp:ListItem>
          <asp:ListItem Value="9">09:00~10:00</asp:ListItem>
          <asp:ListItem Value="10">10:00~11:00</asp:ListItem>
          <asp:ListItem Value="11">11:00~12:00</asp:ListItem>
          <asp:ListItem Value="12">12:00~13:00</asp:ListItem>
          <asp:ListItem Value="13">13:00~14:00</asp:ListItem>
          <asp:ListItem Value="14">14:00~15:00</asp:ListItem>
          <asp:ListItem Value="15">15:00~16:00</asp:ListItem>
          <asp:ListItem Value="16">16:00~17:00</asp:ListItem>
          <asp:ListItem Value="17">17:00~18:00</asp:ListItem>
          <asp:ListItem Value="18">18:00~19:00</asp:ListItem>
          <asp:ListItem Value="19">19:00~20:00</asp:ListItem>
          <asp:ListItem Value="20">20:00~21:00</asp:ListItem>
          <asp:ListItem Value="21">21:00~22:00</asp:ListItem>
          <asp:ListItem Value="22">22:00~23:00</asp:ListItem>
          <asp:ListItem Value="23">23:00~00:00</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="Button2" runat="server" Font-Size="Medium" 
        onclick="Button2_Click" Text="清除7日前這個項目" Visible="False" 
        Width="208px" />
    <asp:Chart ID="Chart1" runat="server" Width="922px" 
            ImageLocation="~\Ut_Data\images\SpindleAxesLoad" 
                ImageStorageMode="UseImageLocation">
        <Series>
            <asp:Series Name="Series1" IsValueShownAsLabel="True" ChartType="Line">
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1">
                <AxisY MaximumAutoSize="90">
                </AxisY>
                <AxisX IsLabelAutoFit="False" IntervalAutoMode="VariableCount" 
                    LabelAutoFitStyle="IncreaseFont, DecreaseFont, StaggeredLabels, LabelsAngleStep30, LabelsAngleStep45, LabelsAngleStep90, WordWrap" 
                    MaximumAutoSize="100">
                    <MajorGrid Enabled="False" />
                    <LabelStyle Angle="90" />
                </AxisX>
            </asp:ChartArea>
        </ChartAreas>
            <Titles>
                <asp:Title Name="Spindle_Load" Text="Spindle_Loading%">
                </asp:Title>
            </Titles>
        </asp:Chart>
        <asp:GridView ID="GridView2" runat="server"
               HorizontalAlign="Center" RowStyle-HorizontalAlign="Left" 
        Width="920px" EmptyDataText="找不到歷史記錄資料!!!" AutoGenerateColumns="False" >
              <Columns>
                  <asp:BoundField DataField="Time" HeaderText="Time(HH-mm-ss)">
                  <ItemStyle Width="200px" />
                  </asp:BoundField>
                  <asp:BoundField DataField="Data" HeaderText="Data" />
              </Columns>
              <RowStyle HorizontalAlign="Center"></RowStyle>
            </asp:GridView>
</asp:Content>
