using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class Monitor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Response.Write("No access right to CNC Information.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "CNC Status";
            }
            //session 變數移到Global.asax.cs 去生成
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                DropDownBind();
                if (!Page.IsPostBack)///從別的頁面切回來
                {
                    show_english_or_not();
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
                        if(DropDownList1.SelectedIndex>=0)
                           Label_ConnIP.Text = DropDownList1.SelectedItem.Value;
                    show_selected_IP();
                }
            }
            else
            {
                if (Request.Cookies["Last_IP"] != null)///離開10分鐘內
                {
                    Label_ConnIP.Text = Request.Cookies["Last_IP"].Value;
                    DropDownList1.SelectedItem.Text = DropDownList1.SelectedItem.Value = Request.Cookies["Last_IP"].Value;
                    show_selected_IP();//if (!Page.IsPostBack) 
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
                
                Button_ShowXYZ.Text = @"Show XYZ";
                ButtonRefresh.Text = @"Page Refresh";
                Button3.Text = "WORK"; Button4.Text = "OFFSET";
                Button6.Text = "Magazine Table"; Button5.Text = "NC Program";
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
            Label_ConnIP.Text = DropDownList1.SelectedItem.Value;
            show_selected_IP();
        }
        DataTable StatusTable = new DataTable();
        DataTable O_N_BC_Table = new DataTable();
        DataTable FeedSpeed_Table = new DataTable();
        DataTable HDTM_Table = new DataTable();
        DataTable ModalG_Table = new DataTable();
        DataTable CNC_Info_Table = new DataTable();
        DataTable PMC_Info_Table = new DataTable();
        DataTable OpMsgTable = new DataTable();
        DataTable ToolPot_table = new DataTable();
        DataTable Limits_table = new DataTable();
        DataTable XYZ_table = new DataTable();
        private void show_selected_IP()
        {
            GridView1.DataSource = null; GridView1.DataBind();
            GridView2.DataSource = null; GridView2.DataBind();
            GridView3.DataSource = null; GridView3.DataBind();
            GridView4.DataSource = null; GridView4.DataBind();
            GridView5.DataSource = null; GridView5.DataBind();
            GridView6.DataSource = null; GridView6.DataBind();
            GridView7.DataSource = null; GridView7.DataBind();
            GridView8.DataSource = null; GridView8.DataBind();
            GridView11.DataSource = null; GridView11.DataBind();
            Get_ShowXYZ();
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            if (conn_IP[0] == "") return;
            Response.Cookies["Last_IP"].Value = DropDownList1.SelectedItem.Value; //再改存放的值 (重點在中文有無亂碼)
            Response.Cookies["Last_IP"].Expires = DateTime.Now.AddYears(10);
            NCA_Var.NC_IP = conn_IP[0];
            //////////////////////////////////////////////////讀取資料區            
            CNC_Info_Table = NCA_Var.read_CNC_Type_Table();//GridView6  取代上行 CNC_Type()  
            if (NCA_Var.NC_Handle != 0)///優化反應速度
            {
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (ShowXYZ)
                {
                    Thread_XYZ_read((object)conn_IP[0]);
                }
                StatusTable = NCA_Var.read_status_Table();//GridView1
                if (eng)
                {
                    StatusTable.Columns[0].ColumnName = "mode";
                    StatusTable.Columns[1].ColumnName = "exec";
                    StatusTable.Columns[2].ColumnName = "motion";
                    StatusTable.Columns[3].ColumnName = "alarm";
                    StatusTable.Columns[4].ColumnName = "EMG";
                }
                O_N_BC_Table = NCA_Var.read_O_N_BC_Table();//GridView2
                FeedSpeed_Table = NCA_Var.read_feed_speed_Table();//GridView3                
                if (eng)
                {
                    FeedSpeed_Table.Columns[0].ColumnName = "Item";
                    FeedSpeed_Table.Columns[1].ColumnName = "Set_Value";
                    FeedSpeed_Table.Columns[2].ColumnName = "ratio%";
                    FeedSpeed_Table.Columns[3].ColumnName = "Actual_Value";
                }
                HDTM_Table = NCA_Var.read_HDTM_Table();//GridView4
                ModalG_Table = NCA_Var.read_Modal_G_Table();//GridView5
                PMC_Info_Table = NCA_Var.read_PMC_Info_Table();//GridView7
                if (eng)
                {
                    PMC_Info_Table.Columns[0].ColumnName = "PMC_Title";
                    PMC_Info_Table.Columns[1].ColumnName = "Title_data";
                }
                OpMsgTable = NCA_Var.read_opmsg_Table();//GridView8
                //////////////////////////////////////////////////////////////////////讀取資料區結束
                NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
                GridView1.DataSource = StatusTable; GridView1.DataBind();//GridView1
                GridView2.DataSource = O_N_BC_Table; GridView2.DataBind();//GridView2
                GridView3.DataSource = FeedSpeed_Table; GridView3.DataBind();//GridView3 
                GridView4.DataSource = HDTM_Table; GridView4.DataBind();//GridView4
                GridView5.DataSource = ModalG_Table; GridView5.DataBind();//GridView5
                GridView6.DataSource = CNC_Info_Table; GridView6.DataBind();//GridView6
                GridView7.DataSource = PMC_Info_Table; GridView7.DataBind();//GridView7
                GridView8.DataSource = OpMsgTable; GridView8.DataBind();//GridView8
                if (ShowXYZ)
                { GridView11.DataSource = XYZ_table; GridView11.DataBind(); }//GridView11
                GridView1.Width = GridView2.Width = GridView3.Width = GridView4.Width = 447;
                GridView5.Width = GridView6.Width = GridView7.Width = GridView8.Width = 447;
                /////////////////////////////////////////////////////////////////////////////////////////////////
                show_Select_Table();
            }
            else
            {
                GridView10.DataSource = null; GridView10.DataBind();
                GridView9.DataSource = null; GridView9.DataBind();
                DropDownList2.Visible = false;
            }
        }

        private void Thread_XYZ_read(object tmp_IP)
        {
            string conn_IP = (string)tmp_IP;
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(conn_IP, handle);
            XYZ_table = NCA_Var.read_XYZ_Table_multi(handle);//GridView11
            handle = NCA_Var.CNC_Release_Handle(handle);
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count < 1) { Timer1.Enabled = false; Timer2.Enabled = false; return; }
            if (GridView1.Rows[0].Cells.Count < 5) { Timer1.Enabled = false; Timer2.Enabled = false; return; }
            if (GridView1.Rows[0].Cells[3].Text == "ALM" || GridView1.Rows[0].Cells[4].Text == "--EMG--")
            { Timer1.Enabled = true; Timer2.Enabled = true; }
            else
            { Timer1.Enabled = false; Timer2.Enabled = false; }
            if (GridView1.Rows[0].Cells[3].Text == "ALM" && Session["ALM_Red"].ToString() == "Empty")
                GridView1.Rows[0].Cells[3].BackColor = System.Drawing.Color.Red;
            else
                GridView1.Rows[0].Cells[3].BackColor = System.Drawing.Color.Empty;
            if (GridView1.Rows[0].Cells[4].Text == "--EMG--" && Session["EMG_Red"].ToString() == "Empty")
                GridView1.Rows[0].Cells[4].BackColor = System.Drawing.Color.Red;
            else
                GridView1.Rows[0].Cells[4].BackColor = System.Drawing.Color.Empty;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (GridView1.Rows[0].Cells[3].BackColor == System.Drawing.Color.Red && GridView1.Rows[0].Cells[3].Text == "ALM")
                Session["ALM_Red"] = "Red";
            else
                Session["ALM_Red"] = "Empty";
            if (GridView1.Rows[0].Cells[4].BackColor == System.Drawing.Color.Red && GridView1.Rows[0].Cells[4].Text == "--EMG--")
                Session["EMG_Red"] = "Red";
            else
                Session["EMG_Red"] = "Empty";
        }
        protected void Timer2_Tick(object sender, EventArgs e)
        {
            if (Button_ShowXYZ.BackColor == System.Drawing.Color.GreenYellow && GridView9.Rows.Count < 2)
            {
                show_selected_IP();
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        int Monitor_Table = 0;
        private void show_Select_Table()
        {
            Button1.BackColor = System.Drawing.Color.Empty;
            Button2.BackColor = System.Drawing.Color.Empty;
            Button3.BackColor = System.Drawing.Color.Empty;
            Button4.BackColor = System.Drawing.Color.Empty;
            Button5.BackColor = System.Drawing.Color.Empty;
            Button6.BackColor = System.Drawing.Color.Empty;
            GridView10.DataSource = null; GridView10.DataBind();
            GridView9.DataSource = null; GridView9.DataBind();
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            if (conn_IP[0] == "") return;
            Get_MonitorTable();//取得 Monitor_Table 值並寫入cookie
            NCA_Var.NC_IP = conn_IP[0];
            bool correct_30i_firstTimePathError = false;
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            switch (Monitor_Table)
            {
                case 0:
                    GridView10.DataSource = null;
                    GridView9.DataSource = null;
                    DropDownList2.Visible = false;
                    break;
                case 1://Macro
                    Button1.BackColor = System.Drawing.Color.GreenYellow;
                    MacroTable = NCA_Var.read_macro_Table();//GridView9
                    if (eng && MacroTable!=null )
                       MacroTable.Columns[2].ColumnName = "Decimal Places";
                    Response.Cookies["Monitor_Table"].Value = "1";
                    Response.Cookies["Monitor_Table"].Expires = DateTime.Now.AddMinutes(10);
                    GridView9.DataSource = MacroTable;
                    break;
                case 2://Pitch
                    Button2.BackColor = System.Drawing.Color.GreenYellow;
                    PitchTable = NCA_Var.read_pitch_Table();//GridView9
                    Response.Cookies["Monitor_Table"].Value = "2";
                    Response.Cookies["Monitor_Table"].Expires = DateTime.Now.AddMinutes(10);
                    GridView9.DataSource = PitchTable;
                    break;
                case 3://WORK
                    Limits_table = NCA_Var.read_limits_Table();//GridView10                   
                    Button3.BackColor = System.Drawing.Color.GreenYellow;
                    WorkTable = NCA_Var.read_WS_Table();//GridView9
                    Response.Cookies["Monitor_Table"].Value = "3";
                    Response.Cookies["Monitor_Table"].Expires = DateTime.Now.AddMinutes(10);
                    GridView10.DataSource = Limits_table;
                    GridView9.DataSource = WorkTable;
                    break;
                case 4://OFFSET
                    Button4.BackColor = System.Drawing.Color.GreenYellow;
                    ToolsTable = NCA_Var.read_tools_Table();//GridView9
                    if (eng && ToolsTable != null)
                    {
                        if (ToolsTable.Columns[1].ColumnName.Contains("刀長")) ToolsTable.Columns[1].ColumnName = "Tool Length";
                        if (ToolsTable.Columns.Count > 2)
                        {
                            if (ToolsTable.Columns[2].ColumnName.Contains("刀長磨耗")) ToolsTable.Columns[2].ColumnName = "Tool Length Wear";
                            if (ToolsTable.Columns.Count > 4)
                            {
                                if (ToolsTable.Columns[3].ColumnName.Contains("刀徑")) ToolsTable.Columns[3].ColumnName = "Tool Radius";
                                if (ToolsTable.Columns[4].ColumnName.Contains("刀徑磨耗")) ToolsTable.Columns[4].ColumnName = "Tool Radius Wear";
                            }
                        }
                    }
                    Response.Cookies["Monitor_Table"].Value = "4";
                    Response.Cookies["Monitor_Table"].Expires = DateTime.Now.AddMinutes(10);
                    GridView9.DataSource = ToolsTable;
                    break;
                case 5://NC Prog
                    Button5.BackColor = System.Drawing.Color.GreenYellow;
                    string dir_select = "4";
                    if (!NCA_Var.Is_0iD) NCA_Var.cur_30iPATH = "";
                    if (!Session.IsNewSession)///尚未退出瀏覽器
                    {
                        if (Page.IsPostBack) dir_select = dir_30i.ToString();///最近一次的選擇
                        else
                        {
                            if (Request.Cookies["30i_dir"] != null)///10分鐘內已有選擇過
                            {
                                dir_select = Request.Cookies["30i_dir"].Value;
                            }
                            else///尚未做過選擇
                            {
                                dir_select = dir_30i.ToString();
                            }
                        }
                    }
                    else
                    {
                        if (Request.Cookies["30i_dir"] != null)///離開10分鐘內
                        {
                            if (Page.IsPostBack) dir_select = dir_30i.ToString();
                            else
                                dir_select = Request.Cookies["30i_dir"].Value;
                        }
                        else
                        {
                            dir_select = "4";
                        }
                    }
                    dir_30i = Convert.ToInt32(dir_select);///轉換回來
                    DropDownList2.SelectedIndex = dir_30i;///這行將 離開10分鐘內 的位置歸位                    
                    NCPRGTable = NCA_Var.read_NCDir_Table();//GridView9
                    if (eng && NCPRGTable != null)
                    {
                        NCPRGTable.Columns[1].ColumnName = "Filename";
                        NCPRGTable.Columns[2].ColumnName = "Comment";
                        NCPRGTable.Columns[3].ColumnName = "Size";
                        NCPRGTable.Columns[4].ColumnName = "Date";
                    }
                    if (NCA_Var.cur_30iPATH != DropDownList2.SelectedItem.Value) correct_30i_firstTimePathError = true;
                    Response.Cookies["Monitor_Table"].Value = "5";
                    Response.Cookies["Monitor_Table"].Expires = DateTime.Now.AddMinutes(10);
                    GridView9.DataSource = NCPRGTable;
                    break;
                case 6://刀庫表
                    Button6.BackColor = System.Drawing.Color.GreenYellow;
                    ToolPot_table = NCA_Var.read_ToolPot_table();//GridView9
                    if (eng && ToolPot_table != null)
                        if (ToolPot_table.Rows.Count > 1)
                        {
                            ToolPot_table.Columns[0].ColumnName = "Pot no.";
                            ToolPot_table.Columns[1].ColumnName = "Pragram Tool no.(Tx)";
                            ToolPot_table.Columns[2].ColumnName = "Comment";
                        }
                    Response.Cookies["Monitor_Table"].Value = "6";
                    Response.Cookies["Monitor_Table"].Expires = DateTime.Now.AddMinutes(10);
                    if (ToolPot_table == null) ToolPot_table = new DataTable();
                    if (ToolPot_table.Rows.Count <= 0)
                    {
                        DataRow tr = ToolPot_table.NewRow();
                        tr.Table.Columns.Add(eng?"Magazine Table read error":"刀庫表讀取失敗");
                        tr.Table.Columns.Add(eng ? "" : "欄位1"); tr.Table.Columns.Add(eng ? "" : "欄位2");
                        ToolPot_table.Rows.Add(tr);
                        ToolPot_table.Rows[0][0] = "No Data !!! (目前只支援 百德刀庫表 only support Quaser)";
                    }
                    GridView9.DataSource = ToolPot_table;
                    break;
                default:
                    GridView10.DataSource = null;
                    GridView9.DataSource = null; DropDownList2.Visible = false;
                    break;
            }
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
            GridView9.DataBind();//GridView9
            if (Monitor_Table == 3) GridView10.DataBind();//GridView10 
            if (Monitor_Table == 3 & GridView9.Rows.Count > 1 & GridView10.Rows.Count > 1)
            {
                for (int j = 0; j < GridView10.Rows.Count; j++)
                    for (int k = 0; k < GridView9.Rows[0].Cells.Count; k++)
                        GridView10.Rows[j].Cells[k].Width = GridView9.Rows[j].Cells[k].Width = 100;
            }
            if (Monitor_Table == 5 & NCA_Var.Is_30i)
            {
                if (correct_30i_firstTimePathError)
                {
                    for (int j = 0; j < DropDownList2.Items.Count - 1; j++)
                    {
                        if (NCA_Var.cur_30iPATH == DropDownList2.Items[j].Value)
                            dir_30i = DropDownList2.SelectedIndex = j;
                    }
                }
                else
                    dir_30i = DropDownList2.SelectedIndex;
                Response.Cookies["30i_dir"].Value = DropDownList2.SelectedIndex.ToString();
                Response.Cookies["30i_dir"].Expires = DateTime.Now.AddMinutes(10);
                DropDownList2.Visible = true;
            }
            else
                DropDownList2.Visible = false;
            if (Monitor_Table == 5 & GridView9.Rows.Count > 0)
            {
                for (int j = 0; j < GridView9.Rows.Count; j++)
                {
                    if (NCPRGTable.Rows[j][2].ToString().Contains("使用中程式") )
                    {
                        if (eng)
                            GridView9.Rows[j].Cells[2].Text = GridView9.Rows[j].Cells[2].Text.Trim().Replace(@"使用中程式", @"Running Program");
                        GridView9.Rows[j].Cells[1].BackColor = System.Drawing.Color.Yellow;
                        GridView9.Rows[j].Cells[2].BackColor = System.Drawing.Color.Yellow;
                        GridView9.Rows[j].Cells[3].BackColor = System.Drawing.Color.Yellow;
                        GridView9.Rows[j].Cells[4].BackColor = System.Drawing.Color.Yellow;
                    }
                    else
                    {
                        GridView9.Rows[j].Cells[1].BackColor = System.Drawing.Color.Empty;
                        GridView9.Rows[j].Cells[2].BackColor = System.Drawing.Color.Empty;
                        GridView9.Rows[j].Cells[3].BackColor = System.Drawing.Color.Empty;
                        GridView9.Rows[j].Cells[4].BackColor = System.Drawing.Color.Empty;
                    }
                }
            }
            if (Monitor_Table == 6 & GridView9.Rows.Count > 0)
            {
                for (int i = 1; i <= GridView9.Rows.Count; ++i)
                {
                    GridView9.Rows[i - 1].BackColor = System.Drawing.Color.Empty;
                    if (GridView9.Rows[i - 1].Cells[2].Text.Contains(@"-"))
                        GridView9.Rows[i - 1].Cells[2].BackColor = System.Drawing.Color.LightBlue;
                    if (GridView9.Rows[i - 1].Cells[2].Text.Contains("主軸刀"))
                    {
                        if (eng)
                            GridView9.Rows[i - 1].Cells[2].Text = GridView9.Rows[i - 1].Cells[2].Text.Trim().Replace(@"主軸刀", @"Spindle");
                        GridView9.Rows[i - 1].Cells[1].BackColor = System.Drawing.Color.LightPink;
                        GridView9.Rows[i - 1].Cells[2].BackColor = System.Drawing.Color.LightPink;
                    }
                    if (GridView9.Rows[i - 1].Cells[2].Text.Contains("待命刀套"))
                    {
                        if (eng)
                            GridView9.Rows[i - 1].Cells[2].Text = GridView9.Rows[i - 1].Cells[2].Text.Trim().Replace(@"待命刀套", @"Standby_pot");
                        GridView9.Rows[i - 1].Cells[0].BackColor = System.Drawing.Color.Yellow;
                        GridView9.Rows[i - 1].Cells[1].BackColor = System.Drawing.Color.Yellow;
                        GridView9.Rows[i - 1].Cells[2].BackColor = System.Drawing.Color.Yellow;
                    }
                    if (eng && GridView9.Rows[i - 1].Cells[2].Text.Contains("空刀") )
                        GridView9.Rows[i - 1].Cells[2].Text = GridView9.Rows[i - 1].Cells[2].Text.Trim().Replace(@"空刀", @"Empty");
                    if (eng && GridView9.Rows[i - 1].Cells[2].Text.Contains("大刀"))
                        GridView9.Rows[i - 1].Cells[2].Text = GridView9.Rows[i - 1].Cells[2].Text.Trim().Replace(@"大刀", @"Large");
                    if (eng && GridView9.Rows[i - 1].Cells[2].Text.Contains("固定刀"))
                        GridView9.Rows[i - 1].Cells[2].Text = GridView9.Rows[i - 1].Cells[2].Text.Trim().Replace(@"固定刀", @"Static");
                    if (eng && GridView9.Rows[i - 1].Cells[2].Text.Contains("破損刀"))
                        GridView9.Rows[i - 1].Cells[2].Text = GridView9.Rows[i - 1].Cells[2].Text.Trim().Replace(@"破損刀", @"Broken");
                }
            }

        }

        private void Get_MonitorTable()
        {
            string tmp_select = "0";
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                if (Page.IsPostBack) tmp_select = Monitor_Table.ToString();///最近一次的選擇
                else
                {
                    if (Request.Cookies["Monitor_Table"] != null)///10分鐘內已有選擇過
                    {
                        tmp_select = Request.Cookies["Monitor_Table"].Value;
                    }
                    else///尚未做過選擇
                    {
                        tmp_select = "5";//修改預設值 原先是 Monitor_Table.ToString();
                    }
                }
            }
            else
            {
                if (Request.Cookies["Monitor_Table"] != null)///離開10分鐘內
                {
                    if (Page.IsPostBack) tmp_select = Monitor_Table.ToString();
                    else
                        tmp_select = Request.Cookies["Monitor_Table"].Value;
                }
                else
                    tmp_select = "5";//修改預設值
            }
            if (tmp_select == "") tmp_select = "0";
            Monitor_Table = Convert.ToInt32(tmp_select);///轉換回來
        }
        //macro
        DataTable MacroTable = new DataTable();
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Button1.BackColor != System.Drawing.Color.GreenYellow)
                Monitor_Table = 1;
            show_Select_Table();
        }
        //pitch
        DataTable PitchTable = new DataTable();
        protected void Button2_Click(object sender, EventArgs e)
        {
            if (Button2.BackColor != System.Drawing.Color.GreenYellow)
                Monitor_Table = 2;
            show_Select_Table();
        }
        //works
        DataTable WorkTable = new DataTable();
        protected void Button3_Click(object sender, EventArgs e)
        {
            if (Button3.BackColor != System.Drawing.Color.GreenYellow)
                Monitor_Table = 3;
            show_Select_Table();
        }
        //tools
        DataTable ToolsTable = new DataTable();
        protected void Button4_Click(object sender, EventArgs e)
        {
            if (GridView6.Rows.Count >= 2)
            {
                if (!(GridView6.Rows[1].Cells[1].Text.Contains("M") && Button4.BackColor == System.Drawing.Color.GreenYellow))
                    Monitor_Table = 4;
            }
            show_Select_Table();
        }
        //NC_PRG
        DataTable NCPRGTable = new DataTable();
        protected void Button5_Click(object sender, EventArgs e)
        {
            if (Button5.BackColor != System.Drawing.Color.GreenYellow)
                Monitor_Table = 5;
            show_Select_Table();
        }
        //刀庫表
        protected void Button6_Click(object sender, EventArgs e)
        {
            if (Button6.BackColor != System.Drawing.Color.GreenYellow)
                Monitor_Table = 6;
            show_Select_Table();
        }


        int dir_30i = 4;
        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Monitor_Table = 5;
            dir_30i = DropDownList2.SelectedIndex;
            NCA_Var.cur_30iPATH = DropDownList2.SelectedItem.Value;
            show_Select_Table();
        }
        //畫面更新
        protected void ButtonRefresh_Click(object sender, EventArgs e)
        {
            show_selected_IP();
        }

        bool ShowXYZ = false;
        //顯示座標
        protected void Button_ShowXYZ_Click(object sender, EventArgs e)
        {
            Get_ShowXYZ();
            ShowXYZ = !ShowXYZ;
            ShowXYZ_Button();
            show_selected_IP();
        }
        private void ShowXYZ_Button()
        {
            if (ShowXYZ)
                Button_ShowXYZ.BackColor = System.Drawing.Color.GreenYellow;
            else
                Button_ShowXYZ.BackColor = System.Drawing.Color.Empty;
        }
        private void Get_ShowXYZ()
        {
            if (Button_ShowXYZ.BackColor == System.Drawing.Color.GreenYellow)
                ShowXYZ = true;
            else
                ShowXYZ = false;
        }

    }
}