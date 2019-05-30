using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class WorkLog : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Button_Save.Enabled = true;
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Button_Save.Enabled = false;
            }
            show_english_or_not();
            //網頁載入時, 呼叫 ShowMsg() 程序顯示所有發言
            string str_Path = Server.MapPath("~/Ut_Data/Log");
            if (!Directory.Exists(str_Path)) Directory.CreateDirectory(str_Path);            
            ShowMsg();
        }
        private void show_english_or_not()
        {
            string str_Path = Server.MapPath("~/Ut_Data/Log");
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Work Log";
                Button_Save.Text = "Save log files to : " + str_Path + "\\MMdd-hhmmss.log , use Editor with word wrap function";
                ButtonSend.Text = "Send";
                FontColor.Items[0].Text = "Black"; FontColor.Items[1].Text = "Green"; FontColor.Items[2].Text = "Blue";
                FontColor.Items[3].Text = "Red"; FontColor.Items[4].Text = "Pink"; FontColor.Items[5].Text = "Purple";
            }
            else
            {
                RequiredFieldValidator1.ErrorMessage = "請輸入姓名"; UserName.ToolTip = "請輸入姓名";
                RequiredFieldValidator2.ErrorMessage = "請輸入發言"; UserMsg.ToolTip = "請輸入發言";
                Button_Save.Text = "存日誌檔到網站的   " + str_Path + "\\MMdd-hhmmss.log     開檔查看時請使用能自動換行的編輯器";
            }
        }
        //使用二維陣列設定表情符號與對應的圖示檔名, 各欄位意義如下
        // Smile[N, 0],  Smile[N, 1]
        // 文字表情符號  該表情對應的圖示檔名
        string[,] Smile = 
    {
        {":)", @"Ut_Data/images/smile.gif"}, 
        {":(", @"Ut_Data/images/angry.gif"}, 
        {":D", @"Ut_Data/images/lol.gif"}, 
        {":cry", @"Ut_Data/images/cry.gif"},
    };

        protected void ButtonSend_Click(object sender, EventArgs e)
        {
            //宣告變數 UserStr, 存放使用者輸入的發言
            string UserStr;

            //鎖定 Application 物件, 禁止其他用戶寫入
            Application.Lock();

            //將原先的 ChatMsgLast 變數 + 1, 指向下一個位置
            Application["ChatMsgLast"] = (int)Application["ChatMsgLast"] + 1;

            //如果 ChatMsgLast 變數值超過 ChatMsgMax, 則讓 ChatMsgLast 
            //的值減去 ChatMsgMax, 將指標拉回前面
            if ((int)Application["ChatMsgLast"] > (int)Application["ChatMsgMax"])
            {
                Application["ChatMsgLast"] =
                    (int)(Application["ChatMsgLast"]) - (int)Application["ChatMsgMax"];
            }

            //依照 "[時間]名稱：發言" 的格式, 將使用者輸入
            //的最新發言儲存於 UserStr 變數。
            UserStr = "[" + DateTime.Now.ToString("MM/dd hh:mm:ss") + "] " +
                UserName.Text + "：" + UserMsg.Text;

            //將發言中的空白, <, > 等三種字元取代為 HTML 語法的符號
            UserStr = UserStr.Replace(" ", "&nbsp;");
            UserStr = UserStr.Replace("<", "&lt;");
            UserStr = UserStr.Replace(">", "&gt;");

            //使用迴圈依序讀取 Smile[] 陣列
            for (int Index = 0; Index <= Smile.Length / 2 - 1; Index++)
            {
                //將表情符號代換為顯示對應的圖檔
                UserStr = UserStr.Replace(Smile[Index, 0],
                                          "<IMG SRC='" + Smile[Index, 1] + "' />");
            }


            //將使用者的發言加上 <span> 標籤與顏色設定
            UserStr = "<span style='color: " +
                FontColor.SelectedValue + "'>" + UserStr + "</span>";

            //將最新發言儲存至 ChatMsgLast 變數所指的 ChatMsgX 變數
            Application["ChatMsg" + Application["ChatMsgLast"]] = UserStr;

            //解除 Application 物件的鎖定
            Application.UnLock();

            //清空發言欄位, 方便使用者輸入新的發言
            UserMsg.Text = "";
            //Web Form 的 DefaultFocu 屬性可用來設定游標的
            //預設位置 (參見 4-6 節), 此處將游標放置於發言
            //欄位, 以方便使用者繼續輸入發言
            UserMsg.Focus();

            //因為使用者已經輸入新的發言, 所以呼叫 ShowMsg()
            //程序重新顯示所有發言
            ShowMsg();
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            //定時呼叫 ShowMsg() 程序重新顯示所有發言
            ShowMsg();
        }

        void ShowMsg()
        {
            //先清空 Label1 控制項
            Label1.Text = "";

            //宣告變數 Index, 作為存取使用者發言的索引數字
            int Index;

            //因為要由舊至新顯示發言, 所以使用迴圈從指標
            //所指之下一處開始往下讀取
            for (int i = (int)Application["ChatMsgLast"] + 1;
                i <= (int)Application["ChatMsgLast"] + (int)Application["ChatMsgMax"]; i++)
            {
                //若變數 i 超過 ChatMsgMax 所設定的範圍, 
                //則減去 ChatMsgMax 以拉回至最前面
                if (i > (int)Application["ChatMsgMax"])
                {
                    Index = i - (int)Application["ChatMsgMax"];
                }
                else
                {
                    Index = i;
                }

                //如果 ChatMsgX 變數值不是空白, 便將儲存於變數內的
                //發言附加於 Label1 控制項
                if (Application["ChatMsg" + Index] != null)
                {
                    if (Application["ChatMsg" + Index].ToString() != "")
                    {
                        Label1.Text =
                            Label1.Text + Application["ChatMsg" + Index] + "<br />";
                        Label1.Text = Label1.Text.Replace("\n", "<br />");
                    }
                }
            }
        }
        //:)
        protected void Button1_Click(object sender, EventArgs e)
        {
            UserMsg.Text += ":)";
        }
        //:(
        protected void Button2_Click(object sender, EventArgs e)
        {
            UserMsg.Text += ":(";
        }
        //:D
        protected void Button3_Click(object sender, EventArgs e)
        {
            UserMsg.Text += ":D";
        }
        //:cry
        protected void Button4_Click(object sender, EventArgs e)
        {
            UserMsg.Text += ":cry";
        }
        //存檔 Log
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            string str_Path = Server.MapPath("~/Ut_Data/Log");
            str_Path = str_Path + "\\" + DateTime.Now.ToString("MMdd-hhmmss") + ".Log";
            string text = Label1.Text.Replace(@"<br />", "");
            text = text.Replace(@"<span style=", "");
            text = text.Replace(@"&nbsp;", " ");
            text = text.Replace(@"IMG SRC='Ut_Data/images/", @"'");
            text = text.Replace(@"</span>", "\n");
            text = text.Replace(@"'color: black'>", @"[黑色]");
            text = text.Replace(@"'color: green'>", @"[綠色]");
            text = text.Replace(@"'color: blue'>", @"[藍色]");
            text = text.Replace(@"'color: red'>", @"[紅色]");
            text = text.Replace(@"'color: #FF69B4'>", @"[粉色]");
            text = text.Replace(@"'color: purple'>", @"[紫色]");
            text = text.Replace(@"<'smile.gif' />", @":)");
            text = text.Replace(@"<'angry.gif' />", @":(");
            text = text.Replace(@"<'lol.gif' />", @":D");
            text = text.Replace(@"<'cry.gif' />", @":cry");
            File.WriteAllText(str_Path, text, System.Text.Encoding.UTF8);
        }
    }
}