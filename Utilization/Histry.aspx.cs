using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class Histry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "OP History/Alarm msg";               
            }
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                DropDownBind();
                if (!Page.IsPostBack)///從別的頁面切回來
                {
                    show_english_or_not();
                    if (Request.Cookies["Op_num"] != null)///離開10分鐘內
                    {
                        string tmp = Request.Cookies["Op_num"].Value;
                        for (int j = 0; j < DropDownList2.Items.Count; j++)
                            if (tmp == DropDownList2.Items[j].Value)
                                DropDownList2.SelectedIndex = j;
                        Label_OpNum.Text = Request.Cookies["Op_num"].Value;
                    }
                    else
                        Label_OpNum.Text = "100";
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
                    show_selected_IP();
                }
                
            }
            else
            {
                if (Request.Cookies["Op_num"] != null)///離開10分鐘內
                {
                    Label_OpNum.Text = Request.Cookies["Op_num"].Value;
                    for (int j = 0; j < DropDownList2.Items.Count; j++)
                        if (Request.Cookies["Op_num"].Value == DropDownList2.Items[j].Value)
                            DropDownList2.SelectedIndex = j;
                }
                else///尚未做過選擇
                {
                    Label_OpNum.Text = "100";
                }
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
                Label1.Text = "Read Ophistry no:";
                Button_Activate.Text = "Activate histry record?"; 
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
        DataTable OpMsgTable = new DataTable();
        DataTable WarnTable = new DataTable();
        DataTable AlarmTable = new DataTable();
        DataTable OphistryTable = new DataTable();
        ushort Op_num =100;
        private void show_selected_IP()
        {
            GridView1.DataSource = null; GridView1.DataBind();
            GridView2.DataSource = null; GridView2.DataBind();
            GridView3.DataSource = null; GridView3.DataBind();
            GridView4.DataSource = null; GridView4.DataBind();            
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            if (conn_IP[0] == "") return;            
            Response.Cookies["Last_IP"].Value = DropDownList1.SelectedItem.Value; //再改存放的值 (重點在中文有無亂碼)
            Response.Cookies["Last_IP"].Expires = DateTime.Now.AddYears(10);            
            NCA_Var.NC_IP = conn_IP[0];
            //////////////////////////////////////////////////讀取資料區
            NCA_Var.read_CNC_Type();///這行重要 切換IP 或 由 Cookie 取回時  讀取資料之前
            if (NCA_Var.NC_Handle != 0)///優化反應速度
            {
                StatusTable = NCA_Var.read_status_Table();//GridView1
                OpMsgTable = NCA_Var.read_opmsg_Table();//GridView2          
                WarnTable = NCA_Var.read_Warn_history_Table();//GridView3
                AlarmTable = NCA_Var.read_Alarm_history_Table();//GridView4
                //Ophistry 要取數量
                Response.Cookies["Op_num"].Value = Label_OpNum.Text;
                Response.Cookies["Op_num"].Expires = DateTime.Now.AddMinutes(10); 
                //////////////////////////////////////////////////////////////////////讀取資料區結束
                Button_Activate.Visible = NCA_Var.Histry_Activate_Check() && (Session["u_name"].ToString() == "Admin");
                NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng && WarnTable != null)
                {
                    WarnTable.Columns[0].ColumnName = "No.";
                    WarnTable.Columns[1].ColumnName = "Warn_Time";
                    WarnTable.Columns[2].ColumnName = "Contents";
                }
                if (eng && AlarmTable != null)
                {
                    AlarmTable.Columns[0].ColumnName = "No.";
                    AlarmTable.Columns[1].ColumnName = "Alarm_Time";
                    AlarmTable.Columns[2].ColumnName = "Contents";
                }
                GridView1.DataSource = StatusTable; GridView1.DataBind();//GridView1
                GridView2.DataSource = OpMsgTable; GridView2.DataBind();//GridView2            
                GridView1.Width = GridView2.Width = 447;
                GridView3.DataSource = WarnTable;
                GridView4.DataSource = AlarmTable;
                GridView3.DataBind();//GridView3               
                GridView4.DataBind();//GridView4
                show_Ophistry_Table();
            }
            else
            {
                GridView5.DataSource = null; GridView5.DataBind(); DropDownList3.Visible = false;
            }
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count < 1 ) return;
            if (GridView1.Rows[0].Cells.Count < 5) return;
            if (GridView1.Rows[0].Cells[3].Text == "ALM")
                GridView1.Rows[0].Cells[3].BackColor = System.Drawing.Color.Red;
            else
                GridView1.Rows[0].Cells[3].BackColor = System.Drawing.Color.Empty;
            if (GridView1.Rows[0].Cells[4].Text == "--EMG--")
                GridView1.Rows[0].Cells[4].BackColor = System.Drawing.Color.Red;
            else
                GridView1.Rows[0].Cells[4].BackColor = System.Drawing.Color.Empty;
        }
        //選擇 Ophistry 數量
        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label_OpNum.Text  = DropDownList2.SelectedItem.Value; //移到Page_Load 當範例           
            show_selected_IP();
        }
        //OphistryTable
        protected void GridView5_PreRender(object sender, EventArgs e)
        {
            if (GridView5.Rows.Count < 1) return;
            if (GridView5.Rows[0].Cells.Count < 2) return;            
            for (int j = 0; j < GridView5.Rows.Count; j++)
            {
                GridView5.Rows[j].Cells[0].Width = 75;
                GridView5.Rows[j].Cells[1].Width = 175;
            }
        }
        //AlarmTable 
        protected void GridView4_PreRender(object sender, EventArgs e)
        {
            if (GridView4.Rows.Count < 1) return;
            if (GridView4.Rows[0].Cells.Count < 2) return;
            for(int j=0;j<GridView4.Rows.Count ;j++)
            { GridView4.Rows[j].Cells[0].Width = 75; GridView4.Rows[j].Cells[1].Width = 175; }
        }
        //WarnTable
        protected void GridView3_PreRender(object sender, EventArgs e)
        {
            if (GridView3.Rows.Count < 1) return;
            if (GridView3.Rows[0].Cells.Count < 2) return;
            for (int j = 0; j < GridView3.Rows.Count; j++)
            { GridView3.Rows[j].Cells[0].Width = 75; GridView3.Rows[j].Cells[1].Width = 175; }
        }

        private void show_Ophistry_Table()
        {
            GridView5.DataSource = null; GridView5.DataBind();
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            if (conn_IP[0] == "") return;
            DropDownList3.Visible = true;
            NCA_Var.NC_IP = conn_IP[0];
            Op_num = Convert.ToUInt16(Label_OpNum.Text);//DropDownList2的 Item 都是數字這樣最簡單
            OphistryTable = NCA_Var.read_Op_history_Table(Op_num);//GridView5
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
            DropDown3Bind();
            string Filter_select = "0";
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                if (Page.IsPostBack) Filter_select = OpFilterSelect.ToString();///最近一次的選擇
                else
                {
                    if (Request.Cookies["OpFilterSelect"] != null)///10分鐘內已有選擇過
                    {
                        Filter_select = Request.Cookies["OpFilterSelect"].Value;
                    }
                    else///尚未做過選擇
                    {
                        Filter_select = OpFilterSelect.ToString();
                    }
                }
            }
            else
            {
                if (Request.Cookies["OpFilterSelect"] != null)///離開10分鐘內
                {
                    if (Page.IsPostBack) Filter_select = OpFilterSelect.ToString();
                    else
                        Filter_select = Request.Cookies["OpFilterSelect"].Value;                    
                }
                else
                    Filter_select = "0";
            }
            OpFilterSelect = Convert.ToInt32(Filter_select);///轉換回來
            if (OpFilterSelect < DropDownList3.Items.Count)///將 離開10分鐘內 的位置歸位                                             
                DropDownList3.SelectedIndex = OpFilterSelect; 
            else
                DropDownList3.SelectedIndex = 0;
            ///加入Filter
            string Filter = DropDownList3.SelectedItem.Text;
            if(DropDownList3.SelectedIndex != 0)
            for (int j=OphistryTable.Rows.Count-1; j >= 0;j--)
            {
              if(!(OphistryTable.Rows[j][1].ToString().ToUpper().Contains(Filter.ToUpper())|(OphistryTable.Rows[j][2].ToString().ToUpper().Contains(Filter.ToUpper()))))
                  OphistryTable.Rows[j].Delete();
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            if (OphistryTable.Rows.Count < 1)
            {                
                if (eng)
                {
                    GridView5.EmptyDataText = "Seleted histry type items no: 0";
                }
            }
            else
            {
                if (eng && OphistryTable != null)
                {
                    OphistryTable.Columns[0].ColumnName = "No.";
                    OphistryTable.Columns[1].ColumnName = "OP_Histry_Type";
                    OphistryTable.Columns[2].ColumnName = "Contents";
                }
            }
            GridView5.DataSource = OphistryTable; 
            GridView5.DataBind();//GridView5   
            Response.Cookies["OpFilterSelect"].Value = DropDownList3.SelectedIndex.ToString();
            Response.Cookies["OpFilterSelect"].Expires = DateTime.Now.AddMinutes(10);            
        }
        private void DropDown3Bind()
        {
            DropDownList3.Items.Clear();
            if (NCA_Var.Is_0iD)
            {
                DropDownList3.Items.Add("ALL");
                DropDownList3.Items.Add("MDI KEY");
                DropDownList3.Items.Add("Alarm");
                DropDownList3.Items.Add("Power On");
                DropDownList3.Items.Add("Power Off");
                DropDownList3.Items.Add("Parameter");
                DropDownList3.Items.Add("Operator's message");
                DropDownList3.Items.Add("Date/Time");
                DropDownList3.Items.Add("Alarm message");
                DropDownList3.Items.Add("Alarm add_Info");
                DropDownList3.Items.Add("Work offset");
                DropDownList3.Items.Add("Tool offset");
                DropDownList3.Items.Add("Signal");
                DropDownList3.Items.Add("Macro");
                DropDownList3.Items.Add("ScreenChange");
            }
            else
            {
                DropDownList3.Items.Add("ALL");                
                DropDownList3.Items.Add("MDI KEY");
                DropDownList3.Items.Add("Alarm");
                DropDownList3.Items.Add("Power On");
                DropDownList3.Items.Add("Power Off");
                DropDownList3.Items.Add("Signal");
            }
        }
        int OpFilterSelect = 0;
        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {            
            OpFilterSelect = DropDownList3.SelectedIndex;  
            show_Ophistry_Table();
        }
        //歷史紀錄未開啟???
        protected void Button_Activate_Click(object sender, EventArgs e)
        {
            if (NCA_Var.Activate_Histry_Record())
                Button_Activate.Visible = false;
        }


    }
}