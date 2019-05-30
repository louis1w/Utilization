<%@ Page Title="主軸/伺服軸負載" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SpindleLoad.aspx.cs" Inherits="Utilization.SpindleLoad" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:UpdatePanel ID="UpdatePanelInput" runat="server" UpdateMode="Conditional"> 
   <ContentTemplate>
   <asp:DropDownList ID="DropDownList1" runat="server" Height="32px" Width="220px" 
            AutoPostBack="True" onselectedindexchanged="DropDownList1_SelectedIndexChanged">
          <asp:ListItem>Test1</asp:ListItem>
        <asp:ListItem>Test2</asp:ListItem>
    </asp:DropDownList>
    <asp:Label ID="Label_ConnIP" runat="server" BackColor="Yellow" 
            Height="22px" Width="200px" Font-Bold="True" Font-Size="Medium"></asp:Label>
  <asp:DropDownList ID="DropDownList2" runat="server" Height="32px" Width="80px" 
            AutoPostBack="True" 
           onselectedindexchanged="DropDownList2_SelectedIndexChanged">
          <asp:ListItem>主軸</asp:ListItem>
        <asp:ListItem>X軸</asp:ListItem>
          <asp:ListItem>Y軸</asp:ListItem>
          <asp:ListItem>Z軸</asp:ListItem>
    </asp:DropDownList>
   </ContentTemplate>
</asp:UpdatePanel>
 <asp:UpdatePanel ID="UpdatePanelShow" runat="server">
   <ContentTemplate>
        <asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="1000">
        </asp:Timer>
        <asp:Chart ID="Chart1" runat="server" Width="922px" 
            ImageLocation="~\Ut_Data\images\SpindleLoadImageFiles" 
                ImageStorageMode="UseImageLocation" Height="500px">
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
                <asp:Title Name="Spindle_Load" Text="主軸_負載%">
                </asp:Title>
            </Titles>
        </asp:Chart>

   </ContentTemplate>
 </asp:UpdatePanel>
</asp:Content>
