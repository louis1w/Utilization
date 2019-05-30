<%@ Page Title="程式號統計" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="O_number.aspx.cs" Inherits="Utilization.O_number" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <asp:Button ID="Button_Vertical" runat="server" Text="直條圖" Height="27px" 
            onclick="Button_Vertical_Click" CausesValidation="False" />
                <asp:Button ID="Button_horizontal" runat="server" Text="圓形圖" Height="27px" 
            onclick="Button_horizontal_Click" CausesValidation="False" />
                    <asp:Button ID="ButtonRefresh" runat="server" Height="27px" 
            onclick="ButtonRefresh_Click" Text="畫面更新" Width="150px" 
                CausesValidation="False" />
        <asp:Chart ID="Chart1" runat="server" Width="920px" 
            ImageLocation="~\Ut_Data\images\UTImageFiles" 
                ImageStorageMode="UseImageLocation">
        <Series>
            <asp:Series Name="Series1" IsValueShownAsLabel="True" ChartType="Pie">
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
        <asp:GridView ID="GridView2" runat="server" Width="920px" 
            AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="O_Num" HeaderText="O_Num" >
            <ItemStyle Width="240px" />
            </asp:BoundField>
            <asp:BoundField DataField="Count" HeaderText="數量"  />
            <asp:BoundField DataField="Percent" HeaderText="百分比" />
        </Columns>
        <RowStyle HorizontalAlign="Center" />
        </asp:GridView>
         <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            Font-Size="Medium" Width="920px" EmptyDataText="No data  !!!" 
            onprerender="GridView1_PreRender"
            >
            <Columns>
                <asp:BoundField DataField="ConnStatus" HeaderText="連線狀況" ReadOnly="True" >
                <ItemStyle Width="70px" />
                </asp:BoundField>
                <asp:BoundField DataField="ConnIP" HeaderText="連線的_IP" >
                <ControlStyle Width="200px" />
                </asp:BoundField>
                <asp:BoundField DataField="O_Num" HeaderText="O_Num" ReadOnly="True" >
                </asp:BoundField>
                <asp:BoundField DataField="Start" HeaderText="START?" ReadOnly="True" >
                </asp:BoundField>
            </Columns>
            <HeaderStyle HorizontalAlign="Left" />
        </asp:GridView>

</asp:Content>
