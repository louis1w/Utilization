using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Threading;
using System.Net;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class _Default : System.Web.UI.Page
    {
        DataTable dtUtility = new DataTable();
        DataTable dtStatistic = new DataTable();
        string str_path = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Login"].ToString() != "Ok")
            {               
                Server.Transfer(@"~/Account/Login.aspx");
            }
            Button1.Enabled = Button2.Enabled=ButtonSave.Enabled = true;
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Button1.Enabled = Button2.Enabled = ButtonSave.Enabled = false;
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Utilization Statistic";
            }
            if (Page.IsPostBack)
            {
                dtUtility = make_dt(dtUtility);
            }
            else
            {
                if (t == 0)
                {
                    Button1.Text = @"Add IP-Host(CNC)";
                    ButtonSave.Text = "SAVE IP";
                    ButtonReload.Text = "Reload IP-Host";
                    Button_Count.Text = @"Parts Count";
                    Button_TotalCount.Text = @"Parts Total";
                    Button_horizontal.Text = "Horizontal";
                    Button_Vertical.Text = "Vertical";
                    Button_Pie.Text = "Pie Chart";
                    Button_OrderClock.Text = "Sort IP ascend";
                    Button_OrderCC.Text = "Sort IP descend";
                    Button2.Text = @"Scan IP(Auto add CNC)";
                    ButtonRefresh.Text = @"Page Refresh";
                    GridView1.Columns[0].HeaderText = "Conn.Status";
                    GridView1.Columns[1].HeaderText = @"Conn. IP-Host";
                    GridView1.Columns[2].HeaderText = "PowerOn";
                    GridView1.Columns[3].HeaderText = @"Op.Time";
                    GridView1.Columns[4].HeaderText = "CuttingTime";
                    GridView1.Columns[5].HeaderText = "CycleTime";
                    GridView1.Columns[6].HeaderText = @"PartsTotal";
                    GridView1.Columns[7].HeaderText = @"PartsReq.";
                    GridView1.Columns[8].HeaderText = @"PartsCount";
                }
                else
                {
                   TextBox1.ToolTip = "請輸入IP,例如:192.168.100.1,加上 \"-\" 接機台名稱 Ex.192.168.100.1-MV154,不可有空格";
                   RegularExpressionValidator1.ErrorMessage = "輸入IP-機台名稱,例如:192.168.100.1-MV154";
                }
                dtUtility = make_dt(dtUtility);
                grideview_bind(dtUtility);
            }
        }
        public string make_XMLfile()
        {
            str_path = Server.MapPath("~/Ut_Data/Hosts.xml");
            if (!File.Exists(str_path))
            {
                XDocument doc = new XDocument(
                                new XDeclaration("1.0", "utf-8", "yes"),
                                new XElement("Hosts",
                                          new XElement("Host", "127.0.0.1")
                                    )
                                );
                doc.Save(str_path);
            }
            return str_path;
        }
        //dtUtility 格式化
        private DataTable make_dt(DataTable dtUtility)
        {
            dtUtility.Columns.Add("ConnStatus", Type.GetType("System.String"));
            dtUtility.Columns.Add("ConnIP", Type.GetType("System.String"));
            dtUtility.Columns.Add("PowerOn", Type.GetType("System.String"));
            dtUtility.Columns.Add("OpTime", Type.GetType("System.String"));
            dtUtility.Columns.Add("CutTime", Type.GetType("System.String"));
            dtUtility.Columns.Add("CycleTime", Type.GetType("System.String"));
            dtUtility.Columns.Add("PartTotal", Type.GetType("System.String"));
            dtUtility.Columns.Add("PartRequired", Type.GetType("System.String"));
            dtUtility.Columns.Add("PartCount", Type.GetType("System.String"));

            dtStatistic.Columns.Add("ConnIP", Type.GetType("System.String"));
            dtStatistic.Columns.Add("image");
            dtStatistic.Columns.Add("PartTotal", Type.GetType("System.String"));
            return dtUtility;
        }

        //grideview1 與 dtUtility 做綁定 
        public static bool[] thread_ping;
        private void grideview_bind(DataTable dtUtility)
        {
            bool nicExist = NCA_Var.check_NIC();
            string[] conn_IP;
            int PowerOn_H = 0, PowerOn_M = 0;
            int OpTime_H = 0, OpTime_M = 0, OpTime_S = 0;
            int CutTime_H = 0, CutTime_M = 0, CutTime_S = 0;
            int CycleTime_H = 0, CycleTime_M = 0, CycleTime_S = 0;
            int PartTotal = 0, PartRequired = 0, PartCount = 0;
            int Hosts_num = Convert.ToInt32(Application["Hosts_num"]);
            if (Hosts_num <= 0) return;
            thread_ping = new bool[Hosts_num];
            for (int j = 0; j < Hosts_num; j++) thread_ping[j] = true;
            if (Hosts_num > 3 & nicExist)
            {
                for (int k = 0; k < Hosts_num; k++)
                {
                    int t = k;
                    new Thread(() => Ping((object)t)).Start();
                }
            }
            int lang = Convert.ToInt32(Session["language"].ToString());
            bool eng = (lang == 0);
            for (int i = 0; i < Hosts_num; i++)
            {
                if (Application["Host" + i].ToString() == "") break;
                DataRow dr = dtUtility.NewRow();
                dr["ConnStatus"] = "";
                dr["ConnIP"] = Application["Host" + i];
                dr["PowerOn"] = "";
                dr["OpTime"] = "";
                dr["CutTime"] = "";
                dr["CycleTime"] = "";
                dr["PartTotal"] = "";
                dr["PartRequired"] = "";
                dr["PartCount"] = "";
                dtUtility.Rows.Add(dr);
                //以下讀取資料                
                conn_IP = Application["Host" + i].ToString().Split('-');
                NCA_Var.NC_IP = conn_IP[0];
                bool connOK = (nicExist & thread_ping[i] ? NCA_Var.read_time() : false);
                if (connOK)
                {
                    dtUtility.Rows[i][0] =(eng?"Connected": "連線正常");
                    //PowerOn
                    dtUtility.Rows[i][2] = NCA_Var.cnc_time[0, 0] + "H" + NCA_Var.cnc_time[0, 1] + "M";
                    PowerOn_H += Convert.ToInt32(NCA_Var.cnc_time[0, 0]); PowerOn_M += Convert.ToInt32(NCA_Var.cnc_time[0, 1]);
                    //OpTime
                    dtUtility.Rows[i][3] = NCA_Var.cnc_time[1, 0] + "H" + NCA_Var.cnc_time[1, 1] + "M" + NCA_Var.cnc_time[1, 2] + "S";
                    OpTime_H += Convert.ToInt32(NCA_Var.cnc_time[1, 0]);
                    OpTime_M += Convert.ToInt32(NCA_Var.cnc_time[1, 1]);
                    OpTime_S += Convert.ToInt32(NCA_Var.cnc_time[1, 2]);
                    //CutTime
                    dtUtility.Rows[i][4] = NCA_Var.cnc_time[2, 0] + "H" + NCA_Var.cnc_time[2, 1] + "M" + NCA_Var.cnc_time[2, 2] + "S";
                    CutTime_H += Convert.ToInt32(NCA_Var.cnc_time[2, 0]);
                    CutTime_M += Convert.ToInt32(NCA_Var.cnc_time[2, 1]);
                    CutTime_S += Convert.ToInt32(NCA_Var.cnc_time[2, 2]);
                    //CycleTime
                    dtUtility.Rows[i][5] = NCA_Var.cnc_time[3, 0] + "H" + NCA_Var.cnc_time[3, 1] + "M" + NCA_Var.cnc_time[3, 2] + "S";
                    CycleTime_H += Convert.ToInt32(NCA_Var.cnc_time[3, 0]);
                    CycleTime_M += Convert.ToInt32(NCA_Var.cnc_time[3, 1]);
                    CycleTime_S += Convert.ToInt32(NCA_Var.cnc_time[3, 2]);
                    bool readPartOk = NCA_Var.read_parts();
                    if (readPartOk)
                    {
                        dtUtility.Rows[i][6] = NCA_Var.cnc_parts[0]; PartTotal += Convert.ToInt32(NCA_Var.cnc_parts[0]);
                        dtUtility.Rows[i][7] = NCA_Var.cnc_parts[1]; PartRequired += Convert.ToInt32(NCA_Var.cnc_parts[1]);
                        dtUtility.Rows[i][8] = NCA_Var.cnc_parts[2]; PartCount += Convert.ToInt32(NCA_Var.cnc_parts[2]);
                    }
                }
                else
                {
                    dtUtility.Rows[i][0] = (eng?"Disconnect":"無法連線"); thread_ping[i] = false;
                }
                NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
            }
            //////////////////////////////            
            DataRow dT = dtUtility.NewRow();
            dT["ConnStatus"] = "";
            dT["ConnIP"] =(eng?"Total": "以上總共");
            dT["PowerOn"] = (PowerOn_M / 60 + PowerOn_H).ToString() + "H" + (PowerOn_M % 60).ToString() + "M";
            dT["OpTime"] = (((OpTime_S / 60) + OpTime_M) / 60 + OpTime_H).ToString() + "H" + (((OpTime_S / 60) + OpTime_M) % 60).ToString() + "M" + (OpTime_S % 60).ToString() + "S";
            dT["CutTime"] = (((CutTime_S / 60) + CutTime_M) / 60 + CutTime_H).ToString() + "H" + (((CutTime_S / 60) + CutTime_M) % 60).ToString() + "M" + (CutTime_S % 60).ToString() + "S";
            dT["CycleTime"] = (((CycleTime_S / 60) + CycleTime_M) / 60 + CycleTime_H).ToString() + "H" + (((CycleTime_S / 60) + CycleTime_M) % 60).ToString() + "M" + (CycleTime_S % 60).ToString() + "S";
            dT["PartTotal"] = PartTotal.ToString();
            dT["PartRequired"] = PartRequired.ToString();
            dT["PartCount"] = PartCount.ToString();
            dtUtility.Rows.Add(dT);
            ////////////////////////////////            
            GridView1.DataSource = dtUtility;
            statistic();
            GridView1.DataBind();///改善掃描區網的圖形異常
            GridView1.Rows[GridView1.Rows.Count - 1].Cells[GridView1.Columns.Count - 1].Visible = false;
            GridView1.Rows[GridView1.Rows.Count - 1].Cells[GridView1.Columns.Count - 2].Visible = false;
        }
        public void Ping(object tmp)
        {
            int t = (int)tmp;
            string[] str_IP = Application["Host" + t].ToString().Split('-');
            string ip = str_IP[0];
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
            options.DontFragment = true;
            string data = "Test Data!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000; // Timeout 时间，单位：毫秒
            System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, options);
            thread_ping[t] = (reply.Status == System.Net.NetworkInformation.IPStatus.Success ? true : false);
        }
        //統計圖
        int statistics = 0, show_statistics = 1;
        private void statistic()
        {
            show_statistics_Button();
            GridView2.DataSource = null; GridView2.DataBind();
            if (dtUtility.Rows.Count <= 1) return;
            string item_name = (statistics == 0 ? "PartCount" : "PartTotal");
            if (dtUtility.Rows[dtUtility.Rows.Count - 1][item_name].ToString() == "") return;
            int total_all = Convert.ToInt32(dtUtility.Rows[dtUtility.Rows.Count - 1][item_name].ToString());
            if (total_all == 0) total_all = 100;
            int[] pic_width = new int[dtUtility.Rows.Count];
            int maxWidth = 0;
            string preText = (statistics == 0 ? Button_Count.Text : Button_TotalCount.Text);
            for (int i = 0; i < dtUtility.Rows.Count - 1; i++)
            {
                DataRow dr = dtStatistic.NewRow();
                String end_str = null;
                end_str = dtUtility.Rows[i]["ConnIP"].ToString();
                end_str += @" _" + preText + ": ";
                int each_total = 0;
                if (dtUtility.Rows[i][item_name].ToString() != "")
                    each_total = Convert.ToInt32(dtUtility.Rows[i][item_name].ToString());
                pic_width[i] = (each_total * 620 / total_all);   //-- 圖片寬度，以得票率（％）來加乘幾倍，讓圖片差異比較顯著！
                maxWidth = Math.Max(maxWidth, pic_width[i]);
                dr[0] = end_str + (each_total).ToString();
                string imageName = "sum" + (i % 21).ToString() + ".gif";
                dr["image"] = imageName;
                dr[2] = (each_total * 100 / total_all).ToString() + "%";   //--這個候選人的得票數 (each_total).ToString()

                dtStatistic.Rows.Add(dr);
            }
            if (show_statistics == 1 || show_statistics == 2) CreateChart();
            else
            {
                GridView2.DataSource = dtStatistic;
                GridView2.DataBind();
                if (maxWidth == 0 | maxWidth >= 620) maxWidth = 620;
                int factor = 1;
                if (maxWidth < 310)
                {
                    factor = 2;
                    if (maxWidth < 200) factor = 3;
                }
                for (int i = 0; i < dtUtility.Rows.Count - 1; i++)
                {
                    Image x = GridView2.Rows[i].Cells[1].Controls[0] as Image;
                    x.Height = 20;
                    int p_Width = pic_width[i] * factor;
                    if (pic_width[i] == 0) p_Width = 1;
                    x.Width = p_Width;
                    GridView2.Rows[i].Cells[1].ControlStyle.Width = p_Width;
                }
            }

        }
        private void clear_color()//統一清掉顏色
        {
            Button_Count.BackColor = System.Drawing.Color.Empty;
            Button_TotalCount.BackColor = System.Drawing.Color.Empty;
            Button_Vertical.BackColor = System.Drawing.Color.Empty;
            Button_horizontal.BackColor = System.Drawing.Color.Empty;
            Button_Pie.BackColor = System.Drawing.Color.Empty;
            Button_OrderCC.BackColor = System.Drawing.Color.Empty;
            Button_OrderClock.BackColor = System.Drawing.Color.Empty;
        }
        private void show_statistics_Button()//恢復顏色 show_statistics 決定 GridView2/Chart1 要秀那一個
        {
            if (Button_TotalCount.BackColor == System.Drawing.Color.GreenYellow) statistics = 1;
            if (Button_horizontal.BackColor == System.Drawing.Color.GreenYellow) show_statistics = 0;
            if (Button_Pie.BackColor == System.Drawing.Color.GreenYellow) show_statistics = 2;
            if (Button_OrderClock.BackColor == System.Drawing.Color.GreenYellow) OrderClock = 1;
            if (Button_OrderCC.BackColor == System.Drawing.Color.GreenYellow) OrderClock = 2;
            clear_color();
            switch (statistics)
            {
                case 0:
                    Button_Count.BackColor = System.Drawing.Color.GreenYellow;
                    break;
                case 1:
                    Button_TotalCount.BackColor = System.Drawing.Color.GreenYellow;
                    break;
            }
            Chart1.Visible = false;
            GridView2.Visible = false;
            switch (show_statistics)
            {
                case 0:
                    Button_horizontal.BackColor = System.Drawing.Color.GreenYellow;
                    GridView2.Visible = true;
                    break;
                case 1:
                    Button_Vertical.BackColor = System.Drawing.Color.GreenYellow;
                    Chart1.Visible = true;
                    break;
                case 2:
                    Button_Pie.BackColor = System.Drawing.Color.GreenYellow;
                    Chart1.Visible = true;
                    break;
            }
            switch (OrderClock)
            {
                case 1:
                    Button_OrderClock.BackColor = System.Drawing.Color.GreenYellow;
                    break;
                case 2:
                    Button_OrderCC.BackColor = System.Drawing.Color.GreenYellow;
                    break;
            }
        }
        private void CreateChart()
        {
            Chart1.Titles.Clear();
            switch (statistics)
            {
                case 0:
                    Chart1.Titles.Add(Button_Count.Text);
                    break;
                case 1:
                    Chart1.Titles.Add(Button_TotalCount.Text);
                    break;
            }
            string item_name = (statistics == 0 ? "PartCount" : "PartTotal");            
            switch (show_statistics)
            {
                case 1:
                    if (dtUtility.Rows.Count > 5)
                    {
                        Chart1.ChartAreas[0].AxisX.LabelStyle.Angle = 90;
                        Chart1.Height = 500;
                    }
                    else
                    {
                        Chart1.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                        Chart1.Height = 400;
                    }
                    Chart1.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column; //直條圖
                    break;
                case 2:
                    if (dtUtility.Rows.Count > 5)
                    {
                        Chart1.Series[0]["PieLabelStyle"] = "Outside"; //數值顯示在圓餅外
                        Chart1.Height = 500;
                    }
                    else
                    {
                        Chart1.Height = 400;
                    }
                    Chart1.Series["Series1"].Label = "#VALY\n#VALX\n"; //數值\nX軸 
                    Chart1.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie; //Pie 圖
                    Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true ;
                    break;
            }
            int t = Convert.ToInt32(Session["language"].ToString());  
            string Disconnected_text =(t == 0?"Disconnect" : "無法連線");
            for (int i = 0; i < dtUtility.Rows.Count - 1; i++)
            {
                //Pie 修正 無法連線的 部分
                if(show_statistics==2 && dtUtility.Rows[i][0].ToString().Contains(Disconnected_text))
                    Chart1.Series[0].Points.AddXY("???"+Disconnected_text, dtUtility.Rows[i][item_name]);
                else
                    Chart1.Series[0].Points.AddXY(dtUtility.Rows[i]["ConnIP"].ToString(), dtUtility.Rows[i][item_name]);
            }

        }

        //刪除列
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {                
                GridView1.EditIndex = -1;
                grideview_bind(dtUtility);                
                Literal mymsg = new Literal();
                mymsg.Text = "<Script>alert('" + "No access right to delete data" + "')</Script>";
                Page.Controls.Add(mymsg);
                return;
            }
            if (GridView1.Rows.Count <= 2)
            {
                TextBox1.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                TextBox1.Text = eng ? "Last one can't be deleted,\n empty textbox before reload page" : "最後一筆資料不能刪除,重新\n載入連線的_IP前先清空此欄";
                return;
            }
            TextBox1.TextMode = System.Web.UI.WebControls.TextBoxMode.SingleLine;
            if (TextBox1.Text != "") TextBox1.Text = "";
            GridView1.DataSource = null; GridView1.DataBind();
            Application.Lock();
            for (int i = e.RowIndex; i < Convert.ToInt32(Application["Hosts_num"]) - 1; i++)
            {
                Application["Host" + i] = Application["Host" + (i + 1)];
            }
            Application["Hosts_num"] = (Convert.ToInt32(Application["Hosts_num"]) - 1).ToString();
            Application.UnLock();
            dtUtility.Clear();
            grideview_bind(dtUtility);
        }
        //新增列
        protected void Button1_Click(object sender, EventArgs e)
        {
            TextBox1.TextMode = System.Web.UI.WebControls.TextBoxMode.SingleLine;
            if (!RegularExpressionValidator1.IsValid | TextBox1.Text == "") return;
            GridView1.DataSource = null; GridView1.DataBind();
            Application.Lock();
            Application["Hosts_num"] = (Convert.ToInt32(Application["Hosts_num"]) + 1).ToString();
            for (int i = Convert.ToInt32(Application["Hosts_num"]) - 2; i >= 0; i--)
            {
                Application["Host" + (i + 1)] = Application["Host" + i];
            }
            Application["Host" + 0] = TextBox1.Text;
            Application.UnLock();
            dtUtility.Clear();
            grideview_bind(dtUtility);
        }
        //儲存連線的_IP
        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            bool DEMO_Mode = Convert.ToBoolean(Application["DEMO_Mode"].ToString());
            if (DEMO_Mode)
            {
                TextBox1.Text = eng ? "DEMO_Mode" : "展示模式";
                return;
            }
            //取得 Hosts.XML 路徑
            string str_path = make_XMLfile();
            //以下開始存檔
            if (GridView1.Rows.Count <= 1) return;
            DataSet ds = new DataSet();
            ds.ReadXmlSchema(str_path);
            DataTable dtIP = new DataTable();
            dtIP.Columns.Add("ConnIP", Type.GetType("System.String"));
            for (int i = 0; i < GridView1.Rows.Count - 1; i++)
            {
                if (Application["Host" + i].ToString() == "") break;
                DataRow dr = dtIP.NewRow();
                dr["ConnIP"] = Application["Host" + i];
                dtIP.Rows.Add(dr);
            }
            ds.Tables.Add(dtIP);
            ds.WriteXml(str_path);
        }
        public void read_hosts()
        {
            //以下改抓取 Hosts.xml
            str_path = make_XMLfile();
            DataSet ds = new DataSet();
            ds.ReadXmlSchema(str_path);
            ds.ReadXml(str_path);
            Application.Lock();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i][0].ToString() == "") break;
                Application["Host" + i] = ds.Tables[0].Rows[i][0].ToString();
                Application["Hosts_num"] = (i + 1).ToString();
            }
            Application["Host" + ds.Tables[0].Rows.Count] = "";
            Application.UnLock();
        }
        //重新載入連線的_IP
        protected void ButtonReload_Click(object sender, EventArgs e)
        {
            TextBox1.TextMode = System.Web.UI.WebControls.TextBoxMode.SingleLine;
            GridView1.DataSource = null; GridView1.DataBind();
            dtUtility.Clear();
            read_hosts();
            grideview_bind(dtUtility);
        }
        //連線正常 用 黃綠色  其他用淺灰色
        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            for (int i = 0; i < GridView1.Rows.Count - 1; i++)
            {
                if (Application["Host" + i].ToString() == "") break;
                if (GridView1.Rows[i].Cells[0].Text == "連線正常" || GridView1.Rows[i].Cells[0].Text == "Connected")
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.GreenYellow;
                else
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.LightGray;
            }
        }
        //掃描區網
        static string IP_preText = "";

        protected void Button2_Click(object sender, EventArgs e)
        {
            ///先取得伺服的IP 再掃描區網
            System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;//.GetHostByName
            if (addressList.Length <= 0) return;
            for (int addr = 0; addr < addressList.Length; addr++)
            {
                IP_preText = "";
                string tt = addressList[addr].ToString();
                if (tt.LastIndexOf('.') < 0) continue;
                IP_preText = tt.Substring(0, tt.LastIndexOf('.'));

                if (IP_preText == "" || IP_preText.Contains("127.0.0") || IP_preText.Contains("169.254")) continue;
                ///以下取得  192.168.1   尾巴要加"."
                IP_preText += ".";
                testHandle = new ushort[254];
                newIP = new bool[254];
                testIP_ary = new string[254];//定義 區網內的IP                
                for (int k = 0; k < 254; k++)
                {
                    int t = k; //int tmp = k + 1;
                    testIP_ary[t] = IP_preText + (t + 1).ToString();//1~254
                    newIP[t] = false ;
                    new Thread(() => testConnect((object)t)).Start();///這裡重要 要先假設不能連線  以免浪費時間 
                    ///test_Ping((object)tmp))
                }
                Thread.Sleep(1000);
                ///開工啦            
                for (int p = 0; p < 254; p++)
                {
                    int Hosts_num = Convert.ToInt32(Application["Hosts_num"]);///目前數量 放迴圈內  動態變化
                    if (newIP[p])///要檢查不曾出現的才算  否則 newIP改設成 false
                    {
                        //testConnect() 
                        for (int i = 0; i < Hosts_num; i++)
                        {
                            if (Application["Host" + i].ToString().Contains(testIP_ary[p]))
                            { newIP[p] = false; break; }
                        }
                    }
                    if (!newIP[p])
                        continue;
                    
                    if (newIP[p])///這裡才確定要加入
                    {
                        Application.Lock();
                        Application["Hosts_num"] = (Convert.ToInt32(Application["Hosts_num"]) + 1).ToString();
                        NCA_Var.NC_IP = testIP_ary[p];
                        string cncType = NCA_Var.read_CNC_Type().Replace("-", "");
                        NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
                        Application["Host" + Hosts_num] = testIP_ary[p] +(cncType==""?"": "-" + cncType);
                        Application.UnLock();
                    }
                }
            }
            Page_refresh();
        }
        private static string[] testIP_ary;
        private static ushort[] testHandle;
        private static bool[] newIP;
        private void testConnect(object t)
        {
            //以下測試OK 保留 
            int p = (int)t;
            testHandle[p] = 0;
            newIP[p] = false;//newIP[t] = false;//移到testConnect()內
            Focas1.cnc_allclibhndl3(testIP_ary[p], 8193, 1, out testHandle[p]);
            if (testHandle[p] != 0)
                newIP[p] = true;            
                
            Focas1.cnc_freelibhndl(testHandle[p]);
        }
       
        //畫面更新
        protected void ButtonRefresh_Click(object sender, EventArgs e)
        {
            Page_refresh();
        }
        private void Page_refresh()
        {
            TextBox1.TextMode = System.Web.UI.WebControls.TextBoxMode.SingleLine;
            GridView1.DataSource = null; GridView1.DataBind();
            dtUtility.Clear();
            grideview_bind(dtUtility);
        }

        int OrderClock = 0;
        //順向排序
        protected void Button_OrderClock_Click(object sender, EventArgs e)
        {
            OrderClock = 1;
            Button_OrderClock.BackColor = System.Drawing.Color.GreenYellow;
            Button_OrderCC.BackColor = System.Drawing.Color.Empty;
            if (GridView1.Rows.Count <= 1) return;
            string[] str_Order = new String[GridView1.Rows.Count - 1];
            for (int i = 0; i < GridView1.Rows.Count - 1; i++)
            {
                if (Application["Host" + i].ToString() == "") break;
                str_Order[i] = (Application["Host" + i]).ToString();
            }
            Array.Sort(str_Order);
            Application.Lock();
            for (int i = 0; i < GridView1.Rows.Count - 1; i++)
            {
                Application["Host" + i] = str_Order[i];
            }
            Application.UnLock();
            Page_refresh();
        }
        //反向排序
        protected void Button_OrderCC_Click(object sender, EventArgs e)
        {
            OrderClock = 2;
            Button_OrderCC.BackColor = System.Drawing.Color.GreenYellow;
            Button_OrderClock.BackColor = System.Drawing.Color.Empty;
            if (GridView1.Rows.Count <= 1) return;
            string[] str_Order = new String[GridView1.Rows.Count - 1];
            for (int i = 0; i < GridView1.Rows.Count - 1; i++)
            {
                if (Application["Host" + i].ToString() == "") break;
                str_Order[i] = (Application["Host" + i]).ToString();
            }
            Array.Sort(str_Order);
            Application.Lock();
            for (int i = 0; i < GridView1.Rows.Count - 1; i++)
            {
                Application["Host" + i] = str_Order[str_Order.Length - 1 - i];
            }
            Application.UnLock();
            Page_refresh();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {                
                GridView1.EditIndex = -1;
                grideview_bind(dtUtility);
                Literal mymsg = new Literal();
                mymsg.Text = "<Script>alert('" + "No access right to edit data" + "')</Script>";
                Page.Controls.Add(mymsg);                
                return;
            }
            GridView1.EditIndex = e.NewEditIndex;
            Page_refresh();
            GridView1.Rows[e.NewEditIndex].Cells[1].Focus();
            if (!(((TextBox)GridView1.Rows[e.NewEditIndex].Cells[1].Controls[0]).Text).Contains('-'))
                ((TextBox)GridView1.Rows[e.NewEditIndex].Cells[1].Controls[0]).Text += "-";
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] conn_IP = ((TextBox)GridView1.Rows[e.RowIndex].Cells[1].Controls[0]).Text.Split('-');
            if (conn_IP[0] == "")
            {
                ((TextBox)GridView1.Rows[e.RowIndex].Cells[1].Controls[0]).Text = (Application["Host" + e.RowIndex]).ToString();
            }
            else
            {
                Literal warnMSg = new Literal();
                int t = Convert.ToInt32(Session["language"].ToString());
                if (t == 0)
                {
                    warnMSg.Text = "<script>alert('Histry Control Table need modify too !!!')</script>";
                }
                else
                {
                    warnMSg.Text = "<script>alert('歷史紀錄控制表也需要修改 !!!')</script>";
                }
                Page.Controls.Add(warnMSg);
                Application.Lock();
                Application["Host" + e.RowIndex] = ((TextBox)GridView1.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
                Application.UnLock();
            }
            GridView1.EditIndex = -1;
            grideview_bind(dtUtility);
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            grideview_bind(dtUtility);
        }

        ///統計的選項
        protected void Button_Count_Click(object sender, EventArgs e)
        {
            statistics = 0;
            Button_Count.BackColor = System.Drawing.Color.GreenYellow;
            Button_TotalCount.BackColor = System.Drawing.Color.Empty;
            Page_refresh();
        }
        protected void Button_TotalCount_Click(object sender, EventArgs e)
        {
            statistics = 1;
            Button_TotalCount.BackColor = System.Drawing.Color.GreenYellow;
            Button_Count.BackColor = System.Drawing.Color.Empty;
            Page_refresh();
        }

        protected void Button_horizontal_Click(object sender, EventArgs e)
        {
            show_statistics = 0;
            Button_horizontal.BackColor = System.Drawing.Color.GreenYellow;
            Button_Pie.BackColor = Button_Vertical.BackColor = System.Drawing.Color.Empty;
            Page_refresh();
        }

        protected void Button_Vertical_Click(object sender, EventArgs e)
        {
            show_statistics = 1;
            Button_Vertical.BackColor = System.Drawing.Color.GreenYellow;
            Button_Pie.BackColor = Button_horizontal.BackColor = System.Drawing.Color.Empty;
            Page_refresh();
        }

        protected void Button_Pie_Click(object sender, EventArgs e)
        {
            show_statistics = 2;
            Button_Pie.BackColor = System.Drawing.Color.GreenYellow;
            Button_Vertical.BackColor = Button_horizontal.BackColor = System.Drawing.Color.Empty;
            Page_refresh();
        }

















    }
}
