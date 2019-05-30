using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {///需要 轉址 
               
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["u_name"].ToString() != "Guest") HeadLoginView.Visible = false;            
            HyperLink2.Text = Application["company"].ToString();
            string company_hyperlink = Application["company_hyperlink"].ToString();
            if (!company_hyperlink.StartsWith(@"http")) company_hyperlink = @"http://" + company_hyperlink;
            HyperLink2.NavigateUrl = company_hyperlink;
            show_area();
        }

        private void clear_color()
        {
            Button_Area1.BackColor = System.Drawing.Color.Empty;
            Button_Area2.BackColor = System.Drawing.Color.Empty;
            Button_Area3.BackColor = System.Drawing.Color.Empty;
            Button_Area4.BackColor = System.Drawing.Color.Empty;
            if (Convert.ToInt32(Session["Areas_Name.Length"]) >= 1) Button_Area1.Text =Session["Areas_Name0"].ToString();
            if (Convert.ToInt32(Session["Areas_Name.Length"]) >= 2) { Button_Area2.Visible = true; Button_Area2.Text = Session["Areas_Name1"].ToString(); }
            if (Convert.ToInt32(Session["Areas_Name.Length"]) >= 3) { Button_Area3.Visible = true; Button_Area3.Text = Session["Areas_Name2"].ToString(); }
            if (Convert.ToInt32(Session["Areas_Name.Length"]) >= 4) { Button_Area4.Visible = true; Button_Area4.Text = Session["Areas_Name3"].ToString(); }
        }
        
        protected void Button_Area1_Click(object sender, EventArgs e)
        {            
            if (Convert.ToInt32(Session["TransHosts.Length"])  < 1)
            { Response.Write("沒有設定 連結的 IP"); return; }
            string trans = @"http://" + Session["TransHosts0"].ToString() + @"/Utilization/";
            Response.Redirect(trans);
        }
        protected void Button_Area2_Click(object sender, EventArgs e)
        {            
            if (Convert.ToInt32(Session["TransHosts.Length"]) < 2)
            { Response.Write("沒有設定 連結的 IP"); return; }
            string trans = @"http://" + Session["TransHosts1"].ToString() + @"/Utilization/";
            Response.Redirect(trans);
        }
        protected void Button_Area3_Click(object sender, EventArgs e)
        {            
            if (Convert.ToInt32(Session["TransHosts.Length"]) < 3)
            { Response.Write("沒有設定 連結的 IP"); return; }
            string trans = @"http://" + Session["TransHosts2"].ToString() + @"/Utilization/";
            Response.Redirect(trans);
        }
        protected void Button_Area4_Click(object sender, EventArgs e)
        {            
            if (Convert.ToInt32(Session["TransHosts.Length"]) < 4)
            { Response.Write("沒有設定 連結的 IP"); return; }
            string trans = @"http://" + Session["TransHosts3"].ToString() + @"/Utilization/";
            Response.Redirect(trans);
        }
        
        private void show_area()
        {            
            if (!Page.IsPostBack)///從別的頁面切回來
            {
                if (!(Menu1.Visible || Menu1.Visible)) change_language();
                string default_Title = @"CNC 分散式(跨廠)監控中心"; //About CNC Distributed Control Center  
                int t = Convert.ToInt32(Session["language"].ToString());
                if (t == 0)
                {
                    default_Title = "CNC Distributed Control Center";
                }
                bool map_exist = true;
                try
                {
                    map_exist = NCA_Var.Ping("maps.google.com.tw");
                }
                catch { map_exist = false; }
                if (!map_exist)
                    HyperLink1.NavigateUrl = "~/Ut_Data/images/QuaserMap.JPG";
                clear_color();
                int area = 1;
                if (!int.TryParse(Session["area"].ToString(), out area))
                    Session["area"] = 1;
                switch (area)
                {
                    case 1:
                        Button_Area1.BackColor = System.Drawing.Color.GreenYellow;
                        Label1.Text = Button_Area1.Text + Session["default_Title"];
                        if (Convert.ToInt32(Session["Areas_Pics.Length"]) >= 1)
                            Image1.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics0"].ToString();
                        else
                            Image1.ImageUrl = null;
                        break;
                    case 2:
                        Button_Area2.BackColor = System.Drawing.Color.GreenYellow;
                        Label1.Text = Button_Area2.Text + Session["default_Title"];
                        if (Convert.ToInt32(Session["Areas_Pics.Length"]) >= 2)
                            Image1.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics1"].ToString();
                        else
                            Image1.ImageUrl = null;
                        break;
                    case 3:
                        Button_Area3.BackColor = System.Drawing.Color.GreenYellow;
                        Label1.Text = Button_Area3.Text + Session["default_Title"];
                        if (Convert.ToInt32(Session["Areas_Pics.Length"]) >= 3)
                            Image1.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics2"].ToString();
                        else
                            Image1.ImageUrl = null;
                        break;
                    case 4:
                        Button_Area4.BackColor = System.Drawing.Color.GreenYellow;
                        Label1.Text = Button_Area4.Text + Session["default_Title"];
                        if (Convert.ToInt32(Session["Areas_Pics.Length"]) >= 4)
                            Image1.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics3"].ToString();
                        else
                            Image1.ImageUrl = null;
                        break;
                    default:
                        Label1.Text = "????" + Session["default_Title"].ToString();
                        Image1.ImageUrl = null;
                        break;
                }
                if (Page.Title.ToString().Contains("關於") || Page.Title.ToString().Contains("About"))
                    Label1.Text = default_Title;
            }            
        }

        private void change_language()
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            RadioButtonList1.SelectedIndex = t;
            if (t == 0)
            { Menu1.Visible = false; Menu0.Visible = true; }
            else
            { Menu0.Visible = false; Menu1.Visible = true; }
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["language"] = RadioButtonList1.SelectedIndex.ToString();            
            Response.Redirect(@"~/Default.aspx");
        }       

       

        

        
        
    }
}
