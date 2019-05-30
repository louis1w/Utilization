using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class Holiday : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Holidays";               
            }            
        }
    }
}