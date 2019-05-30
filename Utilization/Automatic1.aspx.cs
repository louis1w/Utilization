using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using System.Collections;
using NPOI.POIFS;
using NPOI.Util;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class Automatic1 : System.Web.UI.Page
    {
        string CNC_series = "----";
        string AutomaticControl = "AutomaticControl.xls";
        DataTable dtRecord = new DataTable();
        private DataTable make_dt(DataTable dtRecord)
        {
            dtRecord.Columns.Add("ConnStatus", Type.GetType("System.String"));
            dtRecord.Columns.Add("ConnSeries", Type.GetType("System.String"));
            dtRecord.Columns.Add("ConnIP", Type.GetType("System.String"));
            dtRecord.Columns.Add("CNC");//image
            dtRecord.Columns.Add("Monitoring", Type.GetType("System.Boolean"));
            return dtRecord;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Automatic Center 1";
                Label1.Text = "Automatic Center(Customized).....1. Identify CNC.";
                Label2.Text = "Steps:<br>1. Identify CNC.<br>2. Control items define. <br>3. Conditions 1.& 2.(status & output)Settings.<br>4. UI design,include(Automatic/Manual/Start/Stop)Buttons.<br>5. Repeat test untill system stablized.";
                GridView1.EmptyDataText = "Can't find control table !!!";
                GridView1.Columns[0].HeaderText = "Conn.Status";
                GridView1.Columns[1].HeaderText = @"ID(AUTO)";
                GridView1.Columns[2].HeaderText = @"ConnectingIP-Host";
                GridView1.Columns[3].HeaderText = @"CNC";
                GridView1.Columns[4].HeaderText = @"Monitoring?";
                RadioButtonList1.Items[0].Text = "Small Picture";
                RadioButtonList1.Items[1].Text = "Big Picture";
            }
            //Label2.Visible = false;
            string str_Path = Server.MapPath("~/Ut_Data/Log");
            if (!Directory.Exists(str_Path))
            {
                Directory.CreateDirectory(str_Path);
            }
            dtRecord = make_dt(dtRecord);
            show_Select_Table();
        }
        private void show_Select_Table()//處理AutomaticControl.xls檔案做成Table
        {
            GridView1.DataSource = null; GridView1.DataBind();
            AutomaticControl = Server.MapPath("~/Ut_Data/Log/AutomaticControl.xls");
            dtRecord.Rows.Clear();
            if (!File.Exists(AutomaticControl))
            {
                return;
            }
            dtRecord_init();
            GridView1.DataSource = dtRecord; GridView1.DataBind();
        }
        private void dtRecord_init()//進入前都要檢查 AutomaticControl有就??沒有就???
        {
            int t_init = Convert.ToInt32(Session["language"].ToString());
            bool eng_init = (t_init == 0);
            //string image_file = Server.MapPath("~/Ut_Data/images/");                   
            try
            {
                if (File.Exists(AutomaticControl))
                {//直接讀進表  Grideview1已經 GridView1.DataSource = null; GridView1.DataBind();                
                    DataTable dtExcel = make_dt(new DataTable());//差這行就可將RenderExcelToDatatable放在 NCA_Var
                    dtRecord = NCA_Var.RenderExcelToDataTable(AutomaticControl, dtExcel);
                    int Hosts_num = Convert.ToInt32(Application["Hosts_num"]);
                    if (Hosts_num <= 0) Hosts_num = 0;//不需回返   
                    if (dtRecord.Rows.Count < 1 && Hosts_num == 0) return;
                    ArrayList bad_ip_list = new ArrayList();
                    for (int j = 0; j < Hosts_num; j++)
                    {
                        string tmp = Application["Host" + j].ToString();
                        string[] conn_IP;
                        conn_IP = tmp.Split('-');
                        bool find_in_HostX = false;
                        for (int i = 0; i < dtRecord.Rows.Count; i++)
                        {
                            if (dtRecord.Rows[i][2].ToString().Contains(conn_IP[0]))
                            { find_in_HostX = true; break; }
                        }
                        //以下 先分 連線正常(取序號)與無法連線(順便加到bad_ip_list) 再分別加到  dtRecord.Rows
                        if (!find_in_HostX)
                        {
                            DataRow dataRowJ;
                            dataRowJ = dtRecord.NewRow();
                            dataRowJ[2] = tmp;//連線IP
                            dataRowJ[3] = "";//監控項目
                            dataRowJ[4] = false;
                            if (Utilization._Default.thread_ping[j])//連線正常
                            {
                                dataRowJ[0] = "無法連線";
                                CNC_series = "----";
                                NCA_Var.NC_IP = conn_IP[0];
                                NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
                                NCA_Var.read_CNC_Type();
                                if (NCA_Var.NC_Handle != 0 && CNC_series != NCA_Var.cnc_serial)
                                    CNC_series = NCA_Var.cnc_serial;
                                NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
                                dataRowJ[1] = CNC_series;
                            }
                            else
                            {
                                dataRowJ[0] = "無法連線";
                                dataRowJ[1] = "----";
                                bad_ip_list.Add(conn_IP[0]);
                            }
                            dtRecord.Rows.Add(dataRowJ);
                        }
                    }
                    for (int i = 0; i < dtRecord.Rows.Count; i++)
                    {
                        //檢查可否連線
                        string[] str_connectable = dtRecord.Rows[i][2].ToString().Split('-');
                        bool connectable = true;
                        try
                        {
                            if (str_connectable[0].Trim() != "")
                                connectable = NCA_Var.Ping(str_connectable[0]);
                            else
                                connectable = false;
                        }
                        catch { connectable = false; }
                        if (!connectable) bad_ip_list.Add(str_connectable[0]);
                        bool find_in_BadIpList = false;
                        //先找 bad_ip_list
                        if (connectable)
                        {
                            for (int badiplistidx = 0; badiplistidx < bad_ip_list.Count; badiplistidx++)
                            {
                                if (dtRecord.Rows[i][2].ToString().Contains(bad_ip_list[badiplistidx].ToString()))
                                { find_in_BadIpList = true; connectable = false; }
                            }
                        }
                        if (!find_in_BadIpList && connectable)
                        {
                            string[] str_IP = dtRecord.Rows[i][2].ToString().Split('-');
                            ushort testHandle = 0;
                            Focas1.cnc_allclibhndl3(str_IP[0], 8193, 1, out testHandle);
                            if (testHandle != 0)
                                connectable = true;
                            else
                                connectable = false;
                            Focas1.cnc_freelibhndl(testHandle);

                            if (!connectable) bad_ip_list.Add(str_IP[0]);
                        }
                        //連線狀況不同則修改
                        if (eng_init)
                        {
                            //連線狀況不同則修改
                            dtRecord.Rows[i][0] = (connectable ? "Connected" : "Disconnect");
                            //連線IP(2) 空白的 要改成啟動記錄=false
                            if (dtRecord.Rows[i][2].ToString().Trim() == "")
                                dtRecord.Rows[i][4] = false;
                           
                        }
                        else
                        {
                            //連線狀況不同則修改
                            dtRecord.Rows[i][0] = (connectable ? "連線正常" : "無法連線");
                            //連線IP(2) 空白的 要改成啟動記錄=false
                            if (dtRecord.Rows[i][2].ToString().Trim() == "")
                                dtRecord.Rows[i][4] = false;
                        }

                        //第三欄改圖片
                        //dtRecord.Rows[i][3] = image_file;

                        ////調整監控記錄這欄
                        bool t4 = (bool)dtRecord.Rows[i][4];
                        bool t1 = dtRecord.Rows[i][1].ToString().Trim() != "";
                        //bool t3 = dtRecord.Rows[i][3].ToString().Trim() != "";
                        if (t4 && t1)//&& t3
                            dtRecord.Rows[i][4] = true;
                        else
                            dtRecord.Rows[i][4] = false;
                    }
                }
            }
            catch { };
        }

        //連線正常 用 黃綠色  其他用淺灰色
        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            int t_init = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t_init == 0);
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                if (GridView1.Rows[i].Cells[0].Text == "連線正常" || GridView1.Rows[i].Cells[0].Text == "Connected")
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.GreenYellow;
                else
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.LightGray;
                if (GridView1.Rows[i].Cells[4].Text.ToUpper() == "TRUE" || GridView1.Rows[i].Cells[4].Text == "監控中")
                {
                    GridView1.Rows[i].Cells[4].Text =eng?"Monitoring": "監控中";
                    GridView1.Rows[i].Cells[4].BackColor = System.Drawing.Color.GreenYellow;
                    GridView1.Rows[i].Cells[3].BackColor = System.Drawing.Color.GreenYellow;                    
                }
                else
                {
                    GridView1.Rows[i].Cells[4].Text =eng?"not monitored": "未監控";
                    GridView1.Rows[i].Cells[4].BackColor = System.Drawing.Color.LightGray;
                    GridView1.Rows[i].Cells[3].BackColor = System.Drawing.Color.Empty;
                }
                if (GridView1.Rows[i].Cells[0].BackColor == System.Drawing.Color.GreenYellow &&
                    GridView1.Rows[i].Cells[4].BackColor == System.Drawing.Color.GreenYellow)
                {
                    GridView1.Rows[i].Cells[1].BackColor = System.Drawing.Color.GreenYellow;
                    GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.GreenYellow;
                }
                else
                {
                    GridView1.Rows[i].Cells[1].BackColor = System.Drawing.Color.Empty;
                    GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.Empty;
                }
                //序號(1) 連線IP(2)  若有空白 用紅色做警告
                if (GridView1.Rows[i].Cells[1].Text.Trim() == "")
                    GridView1.Rows[i].Cells[1].BackColor = System.Drawing.Color.Red;
                if (GridView1.Rows[i].Cells[2].Text.Trim() == "")
                    GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.Red;
                if (GridView1.Rows[i].Cells[1].BackColor == System.Drawing.Color.GreenYellow)
                {
                    string[] tmpIP = GridView1.Rows[i].Cells[2].Text.Trim().ToString().Split('-');
                    NCA_Var.NC_IP = tmpIP[0];
                    NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
                    NCA_Var.read_CNC_Type();
                    NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
                    if (NCA_Var.Focas_ret != 0 || GridView1.Rows[i].Cells[1].Text.Trim().ToString() != NCA_Var.cnc_serial)
                    {
                        GridView1.Rows[i].Cells[1].Text = GridView1.Rows[i].Cells[1].Text + " != " + NCA_Var.cnc_serial;
                        GridView1.Rows[i].Cells[1].BackColor = System.Drawing.Color.Red;
                    }
                }
            }
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList1.SelectedIndex == 1)
            {
                GridView1.Columns[3].ControlStyle.Width = 320;
                GridView1.Columns[3].ControlStyle.Height = 240;
            }
            else
            {
                GridView1.Columns[3].ControlStyle.Width = 160;
                GridView1.Columns[3].ControlStyle.Height = 120;
            }
        }
    }
}