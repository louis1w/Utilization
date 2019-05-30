﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class MaterialOrder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Stock";
                GridView1.EmptyDataText = "Data table reserve for connecting to ERP system/Stock record";
                Label1.Text = "Or address redirect to ERP system,<br>Address redirect Ex.<br> http://www.quaser.com/"+"___hit and go";
            }
            DataTable Warehouse = new DataTable();
            GridView1.DataSource = Warehouse;
            GridView1.DataBind();
            if (Application["WareHost0"].ToString() != "")
            {
                string trans = Application["WareHost0"].ToString();                
                if (!trans.StartsWith(@"http") && !NCA_Var.Ping(Application["WareHost0"].ToString()))
                    Response.Write("Ping 不到" + Application["WareHost0"].ToString());
                else
                {
                    if (!trans.StartsWith(@"http")) trans = @"http://" + trans;
                    Response.Redirect(trans);//
                }
            } 
        }
    }
}