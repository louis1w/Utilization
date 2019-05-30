using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class O_number : System.Web.UI.Page
    {
        DataTable dtUtility = new DataTable();
        DataTable dtStatistic = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Login"].ToString() != "Ok")
            {
                Server.Transfer(@"~/Account/Login.aspx");
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "O_number Statistic";
            }
            if (Page.IsPostBack)
            {
                dtUtility = make_dt(dtUtility);
            }
            else
            {                
                if (t == 0)
                {                   
                    Button_horizontal.Text = @"Pie Chart";// show_statistics = 0;
                    Button_Vertical.Text = @"Vertical Chart";// show_statistics = 1;

                    ButtonRefresh.Text = @"Page Refresh";
                    GridView1.Columns[0].HeaderText = "Conn.Status";
                    GridView1.Columns[1].HeaderText = @"Conn. IP-Host";

                    GridView2.Columns[1].HeaderText = "Count";
                    GridView2.Columns[2].HeaderText = @"Percent";

                }
                dtUtility = make_dt(dtUtility);
                grideview_bind(dtUtility);
            }
        }
        //dtUtility 格式化
        private DataTable make_dt(DataTable dtUtility)
        {
            dtUtility.Columns.Add("ConnStatus", Type.GetType("System.String"));
            dtUtility.Columns.Add("ConnIP", Type.GetType("System.String"));
            dtUtility.Columns.Add("O_Num", Type.GetType("System.String"));
            dtUtility.Columns.Add("Start", Type.GetType("System.String"));

            dtStatistic.Columns.Add("O_Num", Type.GetType("System.String"));
            dtStatistic.Columns.Add("Count", Type.GetType("System.Int32"));
            dtStatistic.Columns.Add("Percent", Type.GetType("System.Int32"));
            return dtUtility;
        }
        private void grideview_bind(DataTable dtUtility)
        {
            bool nicExist = NCA_Var.check_NIC();
            string[] conn_IP;
            int Hosts_num = Convert.ToInt32(Application["Hosts_num"]);
            if (Hosts_num <= 0) return;
            int lang = Convert.ToInt32(Session["language"].ToString());
            bool eng = (lang == 0);
            for (int i = 0; i < Hosts_num; i++)
            {
                if (Application["Host" + i].ToString() == "") break;
                DataRow dr = dtUtility.NewRow();
                dr["ConnStatus"] = "";
                dr["ConnIP"] = Application["Host" + i];
                dr["O_Num"] = "";
                dr["Start"] = "";
                dtUtility.Rows.Add(dr);
                //以下讀取資料                
                conn_IP = Application["Host" + i].ToString().Split('-');
                NCA_Var.NC_IP = conn_IP[0]; NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
                if (_Default.thread_ping[i]) { NCA_Var.NC_Handle = NCA_Var.CNC_Connect(NCA_Var.NC_Handle); }
                bool connOK = false;
                if (NCA_Var.NC_Handle != 0)
                   connOK = (nicExist & _Default.thread_ping[i] ? NCA_Var.read_O_N_BC(NCA_Var.NC_Handle, false) : false);
                if (connOK)
                {
                    dtUtility.Rows[i][0] = (eng ? "Connected" : "連線正常");
                    //O_Num
                    dtUtility.Rows[i][2] = NCA_Var.O_N_BC[0];
                    bool readStart = NCA_Var.read_status(NCA_Var.NC_Handle, false);
                    //Start?
                    if (readStart && NCA_Var.status.run >= 0 && NCA_Var.status.run < NCA_Var.run.Length)
                        dtUtility.Rows[i][3] = NCA_Var.run[NCA_Var.status.run];
                }
                else
                {
                    dtUtility.Rows[i][0] = (eng ? "Disconnect" : "無法連線");
                }
                NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);

            }
            ////////////////////////////////            
            GridView1.DataSource = dtUtility;
            GridView1.DataBind();
            statistic();
        }
        //連線正常 用 黃綠色  其他用淺灰色
        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                if (Application["Host" + i].ToString() == "") break;
                if (GridView1.Rows[i].Cells[0].Text == "連線正常" || GridView1.Rows[i].Cells[0].Text == "Connected")
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.GreenYellow;
                else
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.LightGray;
                if (GridView1.Rows[i].Cells[3].Text.Trim().ToUpper().Contains("START"))
                {
                    GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.GreenYellow;
                    GridView1.Rows[i].Cells[3].BackColor = System.Drawing.Color.GreenYellow;
                }
                else
                {
                    GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.LightGray;
                    GridView1.Rows[i].Cells[3].BackColor = System.Drawing.Color.LightGray;
                }
            }
        }

        int show_statistics = 0;//預設用Pie 圖
        private void statistic()
        {
            show_statistics_Button();
            GridView2.DataSource = null; GridView2.DataBind();
            if (dtUtility.Rows.Count <= 1) { Chart1.Visible = false; return; }
            bool has_Disconnected = false;
            int num_Disconnected = 0;
            string Disconnected_text = "";
            for (int i = 0; i < dtUtility.Rows.Count; i++)
            {
                if (dtUtility.Rows[i][0].ToString().Contains("Dis") ||
                    dtUtility.Rows[i][0].ToString().Contains("無法"))
                { has_Disconnected = true; num_Disconnected += 1; Disconnected_text = dtUtility.Rows[i][0].ToString(); }           
                string tmp = dtUtility.Rows[i][1].ToString();
                string[] conn_IP;
                conn_IP = tmp.Split('-');
                bool find_in_dtUtility = false;
                if (i > 1)
                {
                    for (int k = 0; k < i; k++)
                    {
                        string[] tmp_dt=dtUtility.Rows[k][1].ToString().Split('-');
                        if (tmp_dt[0]==conn_IP[0])
                        { find_in_dtUtility = true; break; }
                    }
                }
                //以下 處理不重覆 IP-Host 的 O_Num
                if (!find_in_dtUtility && dtUtility.Rows[i][2].ToString().Trim() != "")
                {//找 dtStatistic 的 O_Num ,找到 +1 ,沒找到新增記錄
                    bool find_in_dtStatistic = false;
                    int idx_in_dtStatistic = -1;
                    string tmp_K0 = dtUtility.Rows[i][2].ToString() + "_" + dtUtility.Rows[i][3].ToString();
                    for (int k = 0; k < dtStatistic.Rows.Count; k++)
                    { 
                        string tmp_ds=dtStatistic.Rows[k][0].ToString();
                        if (tmp_ds == tmp_K0)
                        { find_in_dtStatistic = true; idx_in_dtStatistic = k; break; }
                    }
                    if (!find_in_dtStatistic)//沒找到就新增記錄
                    {
                        DataRow dr = dtStatistic.NewRow();
                        dr["O_Num"] = tmp_K0;
                        dr["Count"] = 1;

                        dtStatistic.Rows.Add(dr);
                    }
                    else//找到 +1
                    {
                        if (idx_in_dtStatistic >= 0)
                            dtStatistic.Rows[idx_in_dtStatistic][1] = (int)dtStatistic.Rows[idx_in_dtStatistic][1] + 1;
                    }
                }
            }
            //先補上尚未開機數量
            if (has_Disconnected)
            {
                DataRow dr = dtStatistic.NewRow();
                dr["O_Num"] = Disconnected_text;
                dr["Count"] = num_Disconnected;
                dtStatistic.Rows.Add(dr);
            }
            //算一下百分比
            int total = 0;
            for (int j = 0; j < dtStatistic.Rows.Count; j++)
            {
                total += (int)dtStatistic.Rows[j][1];
            }
            if (total == 0) total = 1;

            for (int j = 0; j < dtStatistic.Rows.Count; j++)
            {
                dtStatistic.Rows[j][2] = (int)((int)dtStatistic.Rows[j][1] * 100 / total);
            }
            
            GridView2.DataSource = dtStatistic;
            GridView2.DataBind();
            CreateChart();//畫圖            
        }
        private void clear_color()//統一清掉顏色
        {
            Button_Vertical.BackColor = System.Drawing.Color.Empty;
            Button_horizontal.BackColor = System.Drawing.Color.Empty;
        }
        private void show_statistics_Button()//恢復顏色 show_statistics 決定 GridView2/Chart1 要秀那一個
        {
            if (Button_horizontal.BackColor == System.Drawing.Color.GreenYellow) show_statistics = 0;
            if (Button_Vertical.BackColor == System.Drawing.Color.GreenYellow) show_statistics = 1;
            clear_color();
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
            }

        }

        private void CreateChart()
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            Chart1.Titles.Clear();            
            switch (show_statistics)
            {
                case 0://Pie 圖 show_statistics = 0;
                    Chart1.Titles.Add("O_Num %");
                    for (int i = 0; i < dtStatistic.Rows.Count; i++)
                    {
                        Chart1.Series[0].Points.AddXY(dtStatistic.Rows[i][0].ToString(), dtStatistic.Rows[i][2]);
                    }
                    Chart1.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie; //Pie 圖
                    Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true ;
                    if (dtUtility.Rows.Count > 5)
                    {
                        Chart1.Series["Series1"]["PieLabelStyle"] = "Outside"; //數值顯示在圓餅外
                        Chart1.Height = 500;
                    }
                    else
                    {                       
                        Chart1.Height = 400;
                    }
                    Chart1.Series["Series1"].Label = "#VALX\n#PERCENT{P1}"; //X軸 + 百分比 #VALX\n#PERCENT{P1}"
                    Chart1.ChartAreas[0].AxisX.Interval = 1;                   
                    break;
                case 1://直條圖 show_statistics = 1;
                    Chart1.Titles.Add(eng?"O_Num Count":"O_Num 數量");
                    for (int i = 0; i < dtStatistic.Rows.Count; i++)
                    {
                        Chart1.Series[0].Points.AddXY(dtStatistic.Rows[i][0].ToString(), dtStatistic.Rows[i][1]);
                    }
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
                    Chart1.Series["Series1"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column; //直條圖
                    break;
            }
        }
        //直條圖 show_statistics = 1;
        protected void Button_Vertical_Click(object sender, EventArgs e)
        {
            show_statistics = 1;
            Button_Vertical.BackColor = System.Drawing.Color.GreenYellow;
            Button_horizontal.BackColor = System.Drawing.Color.Empty;
            Page_refresh();
        }
        //Pie 圖 show_statistics = 0;
        protected void Button_horizontal_Click(object sender, EventArgs e)
        {
            show_statistics = 0;
            Button_horizontal.BackColor = System.Drawing.Color.GreenYellow;
            Button_Vertical.BackColor = System.Drawing.Color.Empty;
            Page_refresh();
        }
        //畫面更新
        protected void ButtonRefresh_Click(object sender, EventArgs e)
        {
            Page_refresh();
        }
        private void Page_refresh()
        {
            GridView1.DataSource = null; GridView1.DataBind();
            GridView2.DataSource = null; GridView2.DataBind();
            dtUtility.Clear(); dtStatistic.Clear();
            grideview_bind(dtUtility);
        }

    }
}