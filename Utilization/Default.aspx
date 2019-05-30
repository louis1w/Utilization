<%@ Page Title="稼動率統計" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Utilization._Default" MaintainScrollPositionOnPostback="true"%>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
        <asp:Button ID="Button2" runat="server" Text="掃描區網自動加入連線IP" Width="160px" 
            Height="27px"  
            OnClientClick="if(confirm('Scan take about one minute,are you sure?\n掃描區網需要數十秒的時間\n掃描結束時若圖形異常請按一下---(畫面更新)按紐\n確定要執行嗎?')==false){return false;}" 
            UseSubmitBehavior="False" onclick="Button2_Click" CausesValidation="False" />
        <asp:TextBox ID="TextBox1" runat="server" Height="21px" Width="160px"
    ToolTip="Input IP Ex:192.168.100.1 &amp;&quot;-Host name&quot; Like:&quot;192.168.100.1-MV154&quot;,no space key allowed"></asp:TextBox>
      <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
            ControlToValidate="TextBox1" ErrorMessage="Iput IP-Host_name,like 192.168.100.1-MV154" 
            BackColor="#FFFF80" 
            ValidationExpression="\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b(\-\S{1,})*" Display="Dynamic"></asp:RegularExpressionValidator>  
    <asp:Button ID="Button1" runat="server" Text="手動新增連線" Width="117px"  Height="27px"
            onclick="Button1_Click" />
        <asp:Button ID="ButtonSave" runat="server" Text="儲存連線" Width="78px"  Height="27px"
            onclick="ButtonSave_Click" CausesValidation="False" />
        <asp:Button ID="ButtonReload" runat="server" Text="重新載入連線" Width="113px"  Height="27px"
            onclick="ButtonReload_Click" CausesValidation="False" />        
       <br /> 
       <asp:Button ID="Button_Count" runat="server" Height="27px" 
        onclick="Button_Count_Click" Text="工件計數" CausesValidation="False" />
        <asp:Button ID="Button_TotalCount" runat="server" Height="27px" 
        onclick="Button_TotalCount_Click" Text="總工件數" CausesValidation="False" />---        
        <asp:Button ID="Button_horizontal" runat="server" Text="橫條圖" Height="27px" 
            onclick="Button_horizontal_Click" CausesValidation="False" />
        <asp:Button ID="Button_Vertical" runat="server" Text="直條圖" Height="27px" 
            onclick="Button_Vertical_Click" CausesValidation="False" />
       <asp:Button ID="Button_Pie" runat="server" Text="圓形圖" Height="27px" 
            onclick="Button_Pie_Click" CausesValidation="False" />---
        <asp:Button ID="Button_OrderClock" runat="server" Height="27px" 
            onclick="Button_OrderClock_Click" Text="順向排序" CausesValidation="False" />
        <asp:Button ID="Button_OrderCC" runat="server" Height="27px" 
            onclick="Button_OrderCC_Click" Text="反向排序" CausesValidation="False" />---        
        <asp:Button ID="ButtonRefresh" runat="server" Height="27px" 
            onclick="ButtonRefresh_Click" Text="畫面更新" Width="150px" 
                CausesValidation="False" />
        
        <p />
        <asp:GridView ID="GridView2" runat="server" GridLines="None" 
        ShowHeader="False" Width="920px" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="ConnIP" >
            <ItemStyle Width="240px" />
            </asp:BoundField>
             <asp:ImageField DataImageUrlField="image" 
                DataImageUrlFormatString="~\Ut_Data\images\{0}" 
                NullDisplayText="no sum picture file" >
            </asp:ImageField>
            <asp:BoundField DataField="PartTotal"  />
        </Columns>
        <RowStyle HorizontalAlign="Left" />
        </asp:GridView>
        <asp:Chart ID="Chart1" runat="server" Width="920px" 
            ImageLocation="~\Ut_Data\images\UTImageFiles" 
                ImageStorageMode="UseImageLocation">
        <Series>
            <asp:Series Name="Series1" IsValueShownAsLabel="True">
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1">
                <AxisY MaximumAutoSize="90">
                </AxisY>
                <AxisX IsLabelAutoFit="False" IntervalAutoMode="VariableCount" 
                    LabelAutoFitStyle="IncreaseFont, DecreaseFont, StaggeredLabels, LabelsAngleStep30, LabelsAngleStep45, LabelsAngleStep90, WordWrap" 
                    MaximumAutoSize="90">
                    <MajorGrid Enabled="False" />
                    <LabelStyle Angle="90" />
                </AxisX>
            </asp:ChartArea>
        </ChartAreas>
        </asp:Chart>
        <br />
        
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            Font-Size="Medium" Width="920px" EmptyDataText="No data  !!!" 
            onrowdeleting="GridView1_RowDeleting" onprerender="GridView1_PreRender" 
            onrowediting="GridView1_RowEditing" 
            onrowcancelingedit="GridView1_RowCancelingEdit" 
            onrowupdating="GridView1_RowUpdating">
            <Columns>
                <asp:BoundField DataField="ConnStatus" HeaderText="連線狀況" ReadOnly="True" >
                </asp:BoundField>
                <asp:BoundField DataField="ConnIP" HeaderText="連線的_IP" >
                <ControlStyle Width="200px" />
                </asp:BoundField>
                <asp:BoundField DataField="PowerOn" HeaderText="通電時間" ReadOnly="True" >
                </asp:BoundField>
                <asp:BoundField DataField="OpTime" HeaderText="運轉時間" ReadOnly="True" >
                </asp:BoundField>
                <asp:BoundField DataField="CutTime" HeaderText="切削時間" ReadOnly="True" >
                </asp:BoundField>
                <asp:BoundField DataField="CycleTime" HeaderText="循環時間" ReadOnly="True" >
                </asp:BoundField>
                <asp:BoundField DataField="PartTotal" HeaderText="總工件數" ReadOnly="True" >
                </asp:BoundField>
                <asp:BoundField DataField="PartRequired" HeaderText="需工件數" ReadOnly="True" >
                </asp:BoundField>
                <asp:BoundField DataField="PartCount" HeaderText="工件計數" ReadOnly="True" >
                </asp:BoundField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                            CommandName="Delete" Text="DEL"
                            OnClientClick="return confirm('Are you sure to delete this one?\nDo not press F5(refresh) to avoid delete again.\n再一次確認,您要刪除這筆資料嗎?\n刪除後請勿做刷新畫面動作(F5)\n以免多刪除一次')"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True" DeleteText="Del" EditText="EDIT" 
                    UpdateText="Update" CancelText="Cancel" />
            </Columns>
            <HeaderStyle HorizontalAlign="Left" />
        </asp:GridView>    
 </asp:Content>  
