using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Utilization
{
    public partial class BulletinBoard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Bulletin Board";
            }

            string str_path = "";
            str_path = Server.MapPath("~/Ut_Data/BulletinBoard.txt");
            if (File.Exists(str_path))
            {
                try
                {
                    string text = File.ReadAllText(str_path, System.Text.Encoding.Default);
                    text = text.Replace(" ", "&nbsp;");
                    text = text.Replace("\n", "<br />");
                    Application["BulletinBoardText"] = text;
                }
                catch
                {
                    Application["BulletinBoardText"] = @"特殊格式公告 請由  關於--->網站管理(admin.aspx)  張貼公告";
                }
            }
            Label1.Text = Application["BulletinBoardText"].ToString();
        }
    }
}