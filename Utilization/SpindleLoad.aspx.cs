using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class SpindleLoad : System.Web.UI.Page
    {
        string CNC_series = "----";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Response.Write("No access right to SpindleLoad.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
             int t = Convert.ToInt32(Session["language"].ToString());
             if (t == 0)
             {
                 Page.Title = "SpindleLoad";
             }
            //session 變數移到Global.asax.cs 去生成
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                DropDownBind();
                if (!Page.IsPostBack)///從別的頁面切回來
                {
                    show_english_or_not();
                    //修正 Session["SpindleLoad" + i] 
                    Session["SpindleLoadIndex"] = "0";
                    for (int i = 0; i < 60; i++)
                    {
                        Session["SpindleLoad" + i] = "0";
                        Session["SpindleLoadTime" + i] = "";
                    }
                    if (Request.Cookies["Last_IP"] != null)///離開10分鐘內                    
                    {
                        string[] conn_IP;
                        conn_IP = Request.Cookies["Last_IP"].Value.Split('-');
                        //先修正 Label_ConnIP.Text 跟 DropDownList1 不一致
                        if (DropDownList1.Items.Count > 0)
                        {
                            if (!DropDownList1.SelectedItem.Value.Contains(conn_IP[0]))
                            {

                                for (int j = 0; j < DropDownList1.Items.Count; j++)
                                    if (DropDownList1.Items[j].Value.Contains(conn_IP[0]))
                                        DropDownList1.SelectedIndex = j;
                            }
                            Label_ConnIP.Text = DropDownList1.SelectedItem.Value;
                        }
                    }
                    else
                        if (DropDownList1.SelectedIndex >= 0)
                            Label_ConnIP.Text = DropDownList1.SelectedItem.Value;
                    checkDropdownlist2Items();
                    show_selected_IP();
                }
                
            }
            else
            {
                //修正 Session["SpindleLoad" + i] 
                Session["SpindleLoadIndex"] = "0";
                for (int i = 0; i < 60; i++)
                {
                    Session["SpindleLoad" + i] = "0";
                    Session["SpindleLoadTime" + i] = "";
                }
                if (Request.Cookies["Last_IP"] != null)///離開10分鐘內
                {
                    Label_ConnIP.Text = Request.Cookies["Last_IP"].Value;
                    DropDownList1.SelectedItem.Text = DropDownList1.SelectedItem.Value = Request.Cookies["Last_IP"].Value;
                    checkDropdownlist2Items();
                    show_selected_IP();
                }
                else
                    Response.Redirect(@"~/Default.aspx");
            }
        }
        private void show_english_or_not()
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {               
                Chart1.Titles[0].Text = "Spindle_Loading%";
                DropDownList2.Items[0].Text = "Spindle";
                DropDownList2.Items[0].Value = "Spindle";
                DropDownList2.Items[1].Text = "X_Axis";
                DropDownList2.Items[1].Value = "X_Axis";
                DropDownList2.Items[2].Text = "Y_Axis";
                DropDownList2.Items[2].Value = "Y_Axis";
                DropDownList2.Items[3].Text = "Z_Axis";
                DropDownList2.Items[3].Value = "Z_Axis";
            }
        }
        private void DropDownBind()
        {
            int Hosts_num = Convert.ToInt32(Application["Hosts_num"]);
            if (Hosts_num <= 0) return;
            int item_no = 0, item_count = 0;
            for (int i = 0; i < Hosts_num; i++)
            {
                if (Utilization._Default.thread_ping[i]) item_count++;
            }
            if (DropDownList1.Items.Count != item_count)
            {
                DropDownList1.Items.Clear();
                for (int i = 0; i < Hosts_num; i++)
                {
                    if (Utilization._Default.thread_ping[i])
                    {
                        DropDownList1.Items.Add(Application["Host" + i].ToString());
                        DropDownList1.Items[item_no].Value = Application["Host" + i].ToString();
                        item_no++;
                    }
                }
                if (DropDownList1.Items.Count > 0)//2013/01/29 測試效果用
                {
                    Label_ConnIP.Text = DropDownList1.SelectedItem.Value; //DropDownList1.SelectedItem.Text;
                }
            }
            else
            {
                for (int i = 0; i < Hosts_num; i++)
                {
                    if (Utilization._Default.thread_ping[i])
                    {
                        DropDownList1.Items[item_no].Text = Application["Host" + i].ToString();
                        DropDownList1.Items[item_no].Value = Application["Host" + i].ToString();
                        item_no++;
                    }
                }

            }
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SpindleLoadIndex"] = "0";
            for (int i = 0; i < 60; i++)
            {
                Session["SpindleLoad" + i] = "0";
                Session["SpindleLoadTime" + i] = "";
            }
            checkDropdownlist2Items();
            show_selected_IP();
        }

        private void checkDropdownlist2Items()
        {
            if (DropDownList1.Items.Count > 0)
                Label_ConnIP.Text = DropDownList1.SelectedItem.Value;
            else
                Response.Redirect(@"~/Default.aspx");
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            //if (conn_IP[0] == "") return;
            Response.Cookies["Last_IP"].Value = DropDownList1.SelectedItem.Value; //再改存放的值 (重點在中文有無亂碼)
            Response.Cookies["Last_IP"].Expires = DateTime.Now.AddYears(10);
            NCA_Var.NC_IP = conn_IP[0];
            NCA_Var.read_axes_name();
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
            //修正  DropDownList2
            if (DropDownList2.Items.Count != (NCA_Var.axes + 1))
            {
                DropDownList2Bind(); DropDownList2.SelectedIndex = 0;
            }
            
        }

        private void DropDownList2Bind()
        {
            DropDownList2.Items.Clear();

            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);//&& !Page.IsPostBack
            if (eng)
            {
                DropDownList2.Items.Add("Spindle");
                DropDownList2.Items[0].Value = "Spindle";
            }
            else
            {
                DropDownList2.Items.Add("主軸");
                DropDownList2.Items[0].Value = "主軸";
            }
            for (int i = 0; i < NCA_Var.axes; i++)
            {
                string tmp = NCA_Var.axes_name[i] + (eng ? "_Axis" : "軸");
                DropDownList2.Items.Add(tmp);
                DropDownList2.Items[i + 1].Value = tmp;
            }
        }
        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SpindleLoadIndex"] = "0";
            for (int i = 0; i < 60; i++)
            {
                Session["SpindleLoad" + i] = "0";
                Session["SpindleLoadTime" + i] = "";
            }
            show_selected_IP();
        }
        private double read_SpindlLoad()
        {
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            ushort handle = 0;
            short spdl_count = 4;
            double[] spdl_data = new double[4];//spdl_count
            Focas1.ODBSPLOAD spnMeter = new Focas1.ODBSPLOAD();
            double y0 = 0, y1 = 0, y2 = 0, y3 = 0, y4 = 0, y5 = 0;
            Focas1.ODBSVLOAD ServerLoads = new Focas1.ODBSVLOAD();
            short srvs = 5;
            ///以上 變數準備完畢
            handle = NCA_Var.CNC_MultiConnect(conn_IP[0], handle);
            //y0
            Focas1.cnc_rdspmeter(handle, 0, ref spdl_count, spnMeter);
            //y1,y2,y3,y4,y5
            Focas1.cnc_rdsvmeter(handle, ref srvs, ServerLoads);
            try
            {
                spdl_data[0] = spnMeter.spload1.spload.data / Math.Pow(10, spnMeter.spload1.spload.dec);
                //Random ranY1 = new Random();
                //y4 = ranY1.NextDouble();
                y0 = spdl_data[0];
                y1 = ServerLoads.svload1.data / Math.Pow(10, ServerLoads.svload1.dec);
                y2 = ServerLoads.svload2.data / Math.Pow(10, ServerLoads.svload2.dec);
                y3 = ServerLoads.svload3.data / Math.Pow(10, ServerLoads.svload3.dec);
                y4 = ServerLoads.svload4.data / Math.Pow(10, ServerLoads.svload4.dec);
                y5 = ServerLoads.svload5.data / Math.Pow(10, ServerLoads.svload5.dec);
            }
            catch
            {
            }
            Focas1.ODBSYS sysinfo = new Focas1.ODBSYS();
            short ret = Focas1.cnc_sysinfo(handle, sysinfo);
            CNC_series = new string(sysinfo.series);
            handle = NCA_Var.CNC_Release_Handle(handle);
            string imagefiles = @"~\Ut_Data\images\LoadingImageFile";
            imagefiles = imagefiles + "_" + CNC_series + "_" + DropDownList2.SelectedItem.Text;
            Chart1.ImageLocation = imagefiles;
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0 );
            Chart1.Titles[0].Text = DropDownList2.SelectedItem.Text + (eng?"_Loading%":"_負載%");
            if (ret == 0 || ret == 6)///spdl load
            {
                switch (DropDownList2.SelectedIndex)
                {
                    case 1:
                        y0 = y1;
                        break;
                    case 2:
                        y0 = y2;
                        break;
                    case 3:
                        y0 = y3;
                        break;
                    case 4:
                        y0 = y4;
                        break;
                    case 5:
                        y0 = y5;
                        break;
                }
            }
            else
                Response.Redirect(@"~/Default.aspx");
            return y0;
        }
        private void show_selected_IP()
        {
            //讀取 CNC 資料
            double SpindleLoad = read_SpindlLoad();
            Chart1.Series[0].Points.Clear();
            int index = Convert.ToInt32(Session["SpindleLoadIndex"]);
            if (index >= 60)
            {
                for (int j = 1; j < 60; j++)
                {
                    Session["SpindleLoad" + (j - 1)] = Session["SpindleLoad" + j];
                    Session["SpindleLoadTime" + (j - 1)] = Session["SpindleLoadTime" + j];
                }
                //CNC 資料放入 SpindleLoadIndex=59 這位置即可
                Session["SpindleLoad59"] = SpindleLoad.ToString("N");
                Session["SpindleLoadTime59"] = DateTime.Now.ToString("hh:mm:ss");
                Fill_chart1(59);
                index = 59;
            }
            else
            {
                //CNC 資料放入 SpindleLoadIndex=index 這位置即可
                Session["SpindleLoad" + index] = SpindleLoad.ToString("N");
                Session["SpindleLoadTime" + index] = DateTime.Now.ToString("hh:mm:ss");
                Fill_chart1(index);
            }
            index += 1;
            Session["SpindleLoadIndex"] = (index).ToString();
        }

        private void Fill_chart1(int max)
        {
            for (int k = 0; k <= max; k++)
            {
                Chart1.Series[0].Points.AddXY(Session["SpindleLoadTime" + k], Session["SpindleLoad" + k]);
            }
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            show_selected_IP();
        }
    }
}