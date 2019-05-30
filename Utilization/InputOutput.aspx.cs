using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class MaterialStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Raws Input/Output";
                GridView1.EmptyDataText = "Data table reserve for connecting to ERP system,Raws input/output record";
                Label1.Text = "Or address redirect to ERP system,<br>Address redirect Ex.<br> http://www.yahoo.com.tw" + "___hit and go";
            }
            DataTable InputOutputStatus = new DataTable();
            GridView1.DataSource = InputOutputStatus;
            GridView1.DataBind();
            if (Application["InputOutputHost0"].ToString() != "")
            {
                string trans = Application["InputOutputHost0"].ToString();                
                if (!trans.StartsWith(@"http") && !NCA_Var.Ping(Application["InputOutputHost0"].ToString()))
                    Response.Write("Ping 不到" + Application["InputOutputHost0"].ToString());
                else
                {
                    if (!trans.StartsWith(@"http")) trans = @"http://" + trans;
                    Response.Redirect(trans);//
                }
            }
            
        }
    }
}