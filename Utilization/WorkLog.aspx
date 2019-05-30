<%@ Page Title="工作日誌" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="WorkLog.aspx.cs" Inherits="Utilization.WorkLog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanelShow" runat="server">
        <ContentTemplate>
            <asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="5000">
            </asp:Timer>
            <div style="height: 400px; overflow: auto; width: 924px; background-color: #ffffcc;"
                id="AllMsg">
                <asp:Label ID="Label1" runat="server" Font-Size="Large" Width="904px"></asp:Label>
            </div>
            <hr />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ButtonSend" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="UpdatePanelInput" runat="server" UpdateMode="Conditional">    
        <ContentTemplate>        
            <div style="width: 924px;">                
                Name:
                <asp:TextBox ID="UserName" runat="server" Width="111px" Height="27px" Font-Size="Medium"
                    ToolTip="Input your name please">
                    </asp:TextBox>
                &nbsp;
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UserName"
                    Display="Dynamic" ErrorMessage="Input name." BackColor="Yellow"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Input speak please."
                    Display="Dynamic" ControlToValidate="UserMsg" BackColor="Yellow"></asp:RequiredFieldValidator>
                <asp:Button ID="Button1" runat="server" Text=":)" CausesValidation="False" 
                    Height="27px" onclick="Button1_Click" Width="30px" />
                <asp:Image ID="Image1" runat="server" Height="25px" 
                    ImageUrl="Ut_Data/images/smile.gif" Width="30px" />
                <asp:Button ID="Button2" runat="server" Text=":(" CausesValidation="False" 
                    Height="27px" onclick="Button2_Click" Width="30px" />
                    <asp:Image ID="Image2" runat="server" Height="25px" 
                    ImageUrl="Ut_Data/images/angry.gif" Width="30px" />
                <asp:Button ID="Button3" runat="server" Text=":D" CausesValidation="False" 
                    Height="27px" onclick="Button3_Click" Width="30px" />
                    <asp:Image ID="Image3" runat="server" Height="25px" 
                    ImageUrl="Ut_Data/images/lol.gif" Width="30px" />
                <asp:Button ID="Button4" runat="server" Text=":cry" CausesValidation="False" 
                    Height="27px" onclick="Button4_Click" Width="35px" />
                     <asp:Image ID="Image4" runat="server" Height="25px" 
                    ImageUrl="Ut_Data/images/cry.gif" Width="30px" />
                &nbsp;&nbsp;&nbsp;<asp:DropDownList ID="FontColor" runat="server" 
                    Font-Size="Medium">
                    <asp:ListItem Selected="True" Value="black">黑色</asp:ListItem>
                    <asp:ListItem Value="green">綠色</asp:ListItem>
                    <asp:ListItem Value="blue">藍色</asp:ListItem>
                    <asp:ListItem Value="red">紅色</asp:ListItem>
                    <asp:ListItem Value="#FF69B4">粉色</asp:ListItem>
                    <asp:ListItem Value="purple">紫色</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="ButtonSend" runat="server" Text="送出" OnClick="ButtonSend_Click" 
                    Height="27px" Width="70px" Font-Bold="True" Font-Size="Large" />
                <br />
                Speak:
                <asp:TextBox ID="UserMsg" runat="server" Width="860px" TextMode="MultiLine" 
                    Font-Size="Large" Height="70px" ToolTip="Input your speak please"></asp:TextBox>                
            </div>
            <br />
            <asp:Button ID="Button_Save" runat="server" Height="27px" Width="924px" 
                onclick="Button_Save_Click" CausesValidation="False" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">

        //使用 add_endRequest 方法, 設定網頁每次 PostBack 時, 
        //都要呼叫 JavaScript 的 ScrollAllMsg 程序
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(ScrollAllMsg);

        //定義 ScrollAllMsg 程序
        function ScrollAllMsg() {
            //取得網頁上名為 AllMsg 的物件
            var objDiv = document.getElementById("AllMsg");
            //scrollHeight 屬性可取得該物件能捲動的高度, 然後
            //設定 scrollTop = scrollHeight 即可捲動到最下面
            objDiv.scrollTop = objDiv.scrollHeight;
        }
    </script>
   
</asp:Content>
