using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int t = Convert.ToInt32(Session["language"].ToString());
        if (t == 0)
        {
            Page.Title = "About this Site";
        }
        string str_path = "";
        str_path = Server.MapPath("~/Ut_Data/About.txt");
        if (File.Exists(str_path))
        {
            try
            {
                string text = File.ReadAllText(str_path, System.Text.Encoding.Default);
                text = text.Replace(" ", "&nbsp;");
                text = text.Replace("\n", "<br />");
                Application["About"] = text;
            }
            catch
            {
                Application["About"] = @"請用 About.txt  張貼內容"; 
            }
        }
        Label1.Text = Application["About"].ToString();
    }
}
