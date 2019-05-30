using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization.Account
{
    public partial class Login : System.Web.UI.Page
    {
        string u_name, u_rank, Login_ok;
        protected void Page_Load(object sender, EventArgs e)
        {
            u_name = Session["u_name"].ToString();
            u_rank = Session["u_rank"].ToString();
            Login_ok = Session["Login"].ToString();
            if(!Page.IsPostBack) LoginUser.UserName = Session["u_name"].ToString();
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {            
            switch (LoginUser.UserName)
            {
                case "Admin":
                    if (Session["Admin"].ToString() == "" || LoginUser.Password == Session["Admin"].ToString())
                    {
                        Session["u_name"] = "Admin";
                        Session["u_rank"] = "Admin";
                        Session["Login"] = "Ok";
                        Response.Redirect(@"~/Admin.aspx");
                    }
                    else
                    {
                        Response.Write("Password error !!! 密碼錯誤 !!!");
                        Response.End();
                    }
                    break;
                case "User":
                    if (Session["User"].ToString() == "" || LoginUser.Password == Session["User"].ToString())
                    {
                        Session["u_name"] = "User";
                        Session["u_rank"] = "User";
                        Session["Login"] = "Ok";
                    }
                    else
                    {
                        Response.Write("Password error !!! 密碼錯誤 !!!");
                        Response.End();
                    }
                    break;
                case "Guest":
                    Session["u_name"] = "Guest";
                    Session["u_rank"] = "Guest";
                    Session["Login"] = "Ok";                    
                    break;
                default:
                    Session["u_name"] = "Guest";
                    Session["u_rank"] = "Guest";
                    Session["Login"] = "";
                    Response.Write("Account error !!! 無此帳號<br> Try:(請改用)<br>Admin<br>User<br>Guest");
                    Response.End();
                    break;
            }
            Response.Redirect(@"~/Default.aspx");
        }

        
    }
}
