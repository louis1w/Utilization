using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class Parameters : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Response.Write("No access right to Parameter/Diagnose/PMC info.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Parameter/Diagnose/PMC info.";
                RangeValidator1.ErrorMessage = "Input number must >0";
            }
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                DropDownBind();
                if (!Page.IsPostBack)///從別的頁面切回來
                {
                    
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
        private void show_selected_IP()
        {
            GridView1.DataSource = null; GridView1.DataBind();
            GridView2.DataSource = null; GridView2.DataBind();
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
                //////////////////////////////////////////////////////////////////////讀取資料區結束
                NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
                GridView1.DataSource = StatusTable; GridView1.DataBind();//GridView1
                GridView2.DataSource = OpMsgTable; GridView2.DataBind();//GridView2            
                GridView1.Width = GridView2.Width = 447;
                /////////////////////////////////////////////////////////////////////
                DropDown2Bind();///這裡與其他監控網頁不同
            }
            else
            {
                GridView3.DataSource = null; GridView3.DataBind(); 
            }
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count < 1) return;
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


        /////////////////////////////////////////////////////////////////////////////////////////////////////        
        private void Parameter_Table_Takeback()
        {
            string tmp_select = "0";
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                if (Page.IsPostBack) tmp_select = Parameter_Table.ToString();///最近一次的選擇
                else
                {
                    if (Request.Cookies["Parameter_Table"] != null)///10分鐘內已有選擇過
                    {
                        tmp_select = Request.Cookies["Parameter_Table"].Value;
                    }
                    else///尚未做過選擇
                    {
                        tmp_select = "1";// 修改預設值
                    }
                }
            }
            else
            {
                if (Request.Cookies["Parameter_Table"] != null)///離開10分鐘內
                {
                    if (Page.IsPostBack) tmp_select = Parameter_Table.ToString();
                    else
                        tmp_select = Request.Cookies["Parameter_Table"].Value;
                }
                else
                    tmp_select = "0";// 修改預設值
            }
            Parameter_Table = Convert.ToInt32(tmp_select);///轉換回來            
        }
        private void DropDown2Bind()
        {            
            Button_CNC.BackColor = System.Drawing.Color.Empty;
            Button_Diagnose.BackColor = System.Drawing.Color.Empty;
            Button_Address.BackColor = System.Drawing.Color.Empty;
            Button_PmcPara.BackColor = System.Drawing.Color.Empty;
            DropDownList2.Visible = false; Label_Drop.Visible = false;

            Parameter_Table_Takeback();///取得----Parameter_Table---擷取出函式共用了

            switch (Parameter_Table)
            {                
                case 1:
                    Button_CNC.BackColor = System.Drawing.Color.GreenYellow;
                    Response.Cookies["Parameter_Table"].Value = "1";
                    Response.Cookies["Parameter_Table"].Expires = DateTime.Now.AddMinutes(10);                    
                    break;
                case 2:
                    Button_Diagnose.BackColor = System.Drawing.Color.GreenYellow;
                    Response.Cookies["Parameter_Table"].Value = "2";
                    Response.Cookies["Parameter_Table"].Expires = DateTime.Now.AddMinutes(10);                    
                    break;
                case 3: //GFYXARTKCDMNE
                    DropDownList2.Items.Clear(); Label_Drop.Text = "";
                    Button_Address.BackColor = System.Drawing.Color.GreenYellow;
                    DropDownList2.Items.Add("G");
                    DropDownList2.Items.Add("F");
                    DropDownList2.Items.Add("Y");
                    DropDownList2.Items.Add("X");
                    DropDownList2.Items.Add("A");
                    DropDownList2.Items.Add("R");
                    DropDownList2.Items.Add("T");
                    DropDownList2.Items.Add("K");
                    DropDownList2.Items.Add("C");
                    DropDownList2.Items.Add("D");
                    DropDownList2.Items.Add("M");
                    DropDownList2.Items.Add("N");
                    DropDownList2.Items.Add("E"); 
                    DropDownList2.Visible = true; Label_Drop.Visible = true;
                    if (Page.IsPostBack)///Button_Address_Click時  不然一率到 show_Select_Table();再處理
                    {
                        DropDownList2.SelectedIndex = parameter_drop_select;
                        Label_Drop.Text = DropDownList2.SelectedItem.Value;
                    }
                    Response.Cookies["Parameter_Table"].Value = "3";
                    Response.Cookies["Parameter_Table"].Expires = DateTime.Now.AddMinutes(10);
                    break;
                case 4:
                    DropDownList2.Items.Clear(); Label_Drop.Text = "";
                    Button_PmcPara.BackColor = System.Drawing.Color.GreenYellow;
                    DropDownList2.Items.Add("Timer");
                    DropDownList2.Items.Add("Counter");
                    DropDownList2.Items.Add("Keeprl");
                    DropDownList2.Items.Add("Data");
                    DropDownList2.Visible = true; Label_Drop.Visible = true;                    
                    Response.Cookies["Parameter_Table"].Value = "4";
                    Response.Cookies["Parameter_Table"].Expires = DateTime.Now.AddMinutes(10);
                    break;
                default:
                    DropDownList2.Visible = false;Label_Drop.Visible=false;
                    break;
            }            
            show_Select_Table();            
        }
       
        int Parameter_Table = 0;
        private void show_Select_Table()
        {
            int gidview3rows = GridView3.Rows.Count;
            GridView3.DataSource = null; GridView3.DataBind();
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            NCA_Var.NC_IP = conn_IP[0];
            if (conn_IP[0] == "") return; 
            #region///處理 TextBox_Input.Text
            string str_tmp_num = "0";
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                if (Page.IsPostBack) str_tmp_num = TextBox_Input.Text;
                else
                {
                    if (Request.Cookies["parameter_InputNum"] != null)///10分鐘內已有選擇過
                    {
                        str_tmp_num = Request.Cookies["parameter_InputNum"].Value;
                    }
                    else///尚未做過選擇
                    {
                        str_tmp_num = TextBox_Input.Text = "0";
                    }
                }
            }
            else
            {
                if (Request.Cookies["parameter_InputNum"] != null)///離開10分鐘內
                {
                    str_tmp_num = Request.Cookies["parameter_InputNum"].Value;
                }
                else
                    str_tmp_num = "0";
            }
            //TextBox_Input.Text = str_tmp_num;///不急著轉換回來 稍微修改這行
            int tmp_Input_num = 0;
            ///合併 不處理的狀況
            if (conn_IP[0] == "" | TextBox_Input.Text == "" | !int.TryParse(str_tmp_num, out tmp_Input_num)) return;///這行與其他監控網頁不同            
            if (tmp_Input_num < 0) ///只先處理小於0  其餘的在後面處理
            {
                tmp_Input_num = 0; //TextBox_Input.Text = "0";            
            }

            #endregion

            ///Parameter_Table 的取得 只需針對 ButtonUP_Click || ButtonDown_Click || DropDownList2_SelectedIndexChanged  的時候做這段
            if (jumpTo_show_Select_Table)
            {
                ///這裡簡化處理  以下沒取到 Parameter_Table 要放 1 ***重要重要
                string tmp_Parameter_Table = Request.Cookies["Parameter_Table"] == null ? "1" : Request.Cookies["Parameter_Table"].Value;
                Parameter_Table = Convert.ToInt32(tmp_Parameter_Table);///轉換回來
            }
            if (conn_IP[0] == "127.0.0.1")///修改
            {
                if (Button_CNC.BackColor == System.Drawing.Color.GreenYellow) Parameter_Table = 1;
                if (Button_Diagnose.BackColor == System.Drawing.Color.GreenYellow) Parameter_Table = 2;
                if (Button_Address.BackColor == System.Drawing.Color.GreenYellow) Parameter_Table = 3;
                if (Button_PmcPara.BackColor == System.Drawing.Color.GreenYellow) Parameter_Table = 4;
            }
            #region///抓取 parameter_drop_select
            string tmp_drop_select = "0";
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                if (Page.IsPostBack) tmp_drop_select = DropDownList2.SelectedIndex.ToString();///最近一次的選擇
                else
                {
                    if (Request.Cookies["parameter_drop_select"] != null)///10分鐘內已有選擇過
                    {
                        tmp_drop_select = Request.Cookies["parameter_drop_select"].Value;
                    }
                    else///尚未做過選擇
                    {
                        tmp_drop_select = "0";
                    }
                }
            }
            else
            {
                if (Request.Cookies["parameter_drop_select"] != null)///離開10分鐘內
                {
                    if (Page.IsPostBack) tmp_drop_select = DropDownList2.SelectedIndex.ToString();
                    else
                        tmp_drop_select = Request.Cookies["parameter_drop_select"].Value;
                }
                else
                    tmp_drop_select = "0";
            }
            parameter_drop_select = Convert.ToInt32(tmp_drop_select); ///先轉換回 parameter_drop_select

            if (Parameter_Table == 3 || Parameter_Table == 4)///針對 case3 case4 的 DropDownList2 轉換回來 
            {
                if (parameter_drop_select < 0) parameter_drop_select = 0;//先修正範圍
                if (parameter_drop_select >= DropDownList2.Items.Count)
                    parameter_drop_select = 0;
                DropDownList2.SelectedIndex = parameter_drop_select;
                Label_Drop.Text = DropDownList2.SelectedItem.Value;
            }
            #endregion

            //之前處理 TextBox_Input.Text 已將 TextBox_Input.Text 換成數字處理 ===>tmp_Input_num    
            #region switch case
            GridView3.EmptyDataText = "No Data !!!";
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            switch (Parameter_Table)
            {
                case 1://Parameter
                    switch (Up_Down_direct)//gidview3rows
                    {
                        case 1:
                            if (tmp_Input_num <= 30) tmp_Input_num = 0;
                            else
                                tmp_Input_num -= 100;
                            break;
                        case 2:
                            if (gidview3rows > 0)
                                tmp_Input_num += 0;
                            else
                                tmp_Input_num += 100;
                            break;
                    }
                    if (tmp_Input_num < 0 | tmp_Input_num > 99999) tmp_Input_num = 0;
                    CNC_Table = NCA_Var.read_CNC_Parameter_Table(tmp_Input_num);
                    if (CNC_Table != null)
                    {
                        if (CNC_Table.Rows.Count > 0)///修正數字
                        {
                            switch (Up_Down_direct)
                            {
                                case 1:
                                    if (tmp_Input_num > Convert.ToInt32(CNC_Table.Rows[0][0].ToString()) - 100)
                                        tmp_Input_num = Convert.ToInt32(CNC_Table.Rows[0][0].ToString());//切到第一個
                                    else { tmp_Input_num -= 100; }
                                    if (tmp_Input_num < 0) tmp_Input_num = 0;
                                    break;
                                case 2:
                                    tmp_Input_num = Convert.ToInt32(CNC_Table.Rows[CNC_Table.Rows.Count - 1][0].ToString());//切到最後一個再修正
                                    break;
                            }
                            if (eng)
                            {
                                CNC_Table.Columns[0].ColumnName = "No.";
                                CNC_Table.Columns[1].ColumnName = "Non-axis";
                                CNC_Table.Columns[2].ColumnName = "Axis-1";
                                CNC_Table.Columns[3].ColumnName = "Axis-2";
                                CNC_Table.Columns[4].ColumnName = "Axis-3";
                                CNC_Table.Columns[5].ColumnName = "Axis-4";
                                CNC_Table.Columns[6].ColumnName = "Axis-5";
                                CNC_Table.Columns[10].ColumnName = "Type";
                                CNC_Table.Columns[13].ColumnName = "Description";
                            }
                        }
                    }
                    GridView3.DataSource = CNC_Table;
                    break;
                case 2://Diagnose
                    switch (Up_Down_direct)//gidview3rows
                    {
                        case 1:
                            if (tmp_Input_num <= 30) tmp_Input_num = 0;
                            else
                                tmp_Input_num -= 11;
                            break;
                        case 2:
                            if (gidview3rows > 0)
                                tmp_Input_num += 1;
                            else
                                tmp_Input_num += 11;
                            break;
                    }
                    if (tmp_Input_num < 0 | tmp_Input_num > 9999) tmp_Input_num = 0; 
                    DiagnoseTable = NCA_Var.read_Diagnose_Table(tmp_Input_num);                    
                    if (DiagnoseTable != null)
                    {
                        if (DiagnoseTable.Rows.Count > 0)///修正數字
                        {
                            switch (Up_Down_direct)
                            {
                                case 1:
                                    tmp_Input_num = Convert.ToInt32(DiagnoseTable.Rows[0][0].ToString().Substring(0, 4));//切到第一個
                                    break;
                                case 2:
                                    tmp_Input_num = Convert.ToInt32(DiagnoseTable.Rows[DiagnoseTable.Rows.Count - 1][0].ToString().Substring(0, 4));//切到最後一個再修正
                                    break;
                            }
                            if (eng)
                            {
                                DiagnoseTable.Columns[1].ColumnName = "Data_Value";
                            }
                        }
                    }
                    GridView3.DataSource = DiagnoseTable;
                    break;
                case 3://PMC Address
                    switch (Up_Down_direct)
                    {
                        case 1:
                            tmp_Input_num -= 80;
                            break;
                        case 2:
                            if (gidview3rows <= 79 & gidview3rows > 1)
                                tmp_Input_num += gidview3rows;
                            else
                                tmp_Input_num += 80;
                            break;
                    }
                    if (tmp_Input_num < 0) tmp_Input_num = 0;
                    PMC_AddressTable = NCA_Var.read_PMC_Address_Table(tmp_Input_num, (short)parameter_drop_select);
                    if (PMC_AddressTable == null & Up_Down_direct == 1)///減少找的時間
                    {
                        tmp_Input_num += 40;
                        PMC_AddressTable = NCA_Var.read_PMC_Address_Table(tmp_Input_num, (short)parameter_drop_select);
                        if (PMC_AddressTable == null)///減少找的時間
                        {
                            tmp_Input_num += 20;//第二次 
                            PMC_AddressTable = NCA_Var.read_PMC_Address_Table(tmp_Input_num, (short)parameter_drop_select);
                            if (PMC_AddressTable == null) tmp_Input_num -= 60;//回復原先
                            else
                                tmp_Input_num = Convert.ToInt32(PMC_AddressTable.Rows[0][0].ToString().Substring(1));//切到第一個
                        }
                        else
                            tmp_Input_num = Convert.ToInt32(PMC_AddressTable.Rows[0][0].ToString().Substring(1));//切到第一個
                    }
                    if (PMC_AddressTable != null)
                    {
                        if (PMC_AddressTable.Rows.Count > 0)///修正數字
                        {
                            if (eng)
                            {
                                PMC_AddressTable.Columns[1].ColumnName = "Data_Value";
                            }
                        }
                    }
                    GridView3.DataSource = PMC_AddressTable;
                    break;
                case 4://PMC Parameter 
                    switch (Up_Down_direct)
                    {
                        case 1:
                            tmp_Input_num -= 10;
                            break;
                        case 2:
                            tmp_Input_num += 10;
                            break;
                    }
                    if (tmp_Input_num < 0) tmp_Input_num = 0;
                    short PMC_para_type = 0;
                    switch (parameter_drop_select)
                    {
                        case 0://Timer
                            PMC_para_type = 6;
                            break;
                        case 1://Counter
                            PMC_para_type = 8;
                            break;
                        case 2://Keeprl
                            PMC_para_type = 7;
                            break;
                        case 3://Data
                            PMC_para_type = 9;
                            break;
                    }
                    PMC_ParaTable = NCA_Var.read_PMC_Para_Table(PMC_para_type);//GridView3   
                    if (eng && parameter_drop_select==2)
                    {
                        PMC_ParaTable.Columns[2].ColumnName = "Data_Value";
                    }
                    GridView3.DataSource = PMC_ParaTable;
                    break;
                default:
                    GridView3.DataSource = null;
                    break;
            }
            #endregion
            TextBox_Input.Text = tmp_Input_num.ToString();///要等switch case 處理完才能 轉換回來  
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
            GridView3.DataBind();//GridView3
            Response.Cookies["parameter_drop_select"].Value = DropDownList2.SelectedIndex.ToString();
            Response.Cookies["parameter_drop_select"].Expires = DateTime.Now.AddMinutes(10);
            Response.Cookies["parameter_InputNum"].Value = TextBox_Input.Text;
            Response.Cookies["parameter_InputNum"].Expires = DateTime.Now.AddMinutes(10);            
            TextBox_Input.Focus();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////      
        
        DataTable CNC_Table = new DataTable();
        protected void Button_CNC_Click(object sender, EventArgs e)
        {
            Parameter_Table = 1;
            DropDown2Bind();
        }
        
        DataTable DiagnoseTable = new DataTable();
        protected void Button_Diagnose_Click(object sender, EventArgs e)
        {
            Parameter_Table = 2;
            DropDown2Bind();
        }
        
        DataTable PMC_AddressTable = new DataTable();
        protected void Button_Address_Click(object sender, EventArgs e)
        {
            Parameter_Table = 3;
            parameter_drop_select = 5;
            DropDown2Bind();
        }
        
        DataTable PMC_ParaTable = new DataTable();
        protected void Button_PmcPara_Click(object sender, EventArgs e)
        {
            Parameter_Table = 4;  
            TextBox_Input.Text = "0";                          
            DropDown2Bind();
        }
        
        int parameter_drop_select = 0;
        bool jumpTo_show_Select_Table = false;
        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            parameter_drop_select = DropDownList2.SelectedIndex;
            Label_Drop.Text = DropDownList2.SelectedItem.Value;
            jumpTo_show_Select_Table = true;            
            show_Select_Table();
        }       
        
        int Up_Down_direct = 0;

        protected void ButtonUP_Click(object sender, EventArgs e)
        {            
            Up_Down_direct = 1;
            jumpTo_show_Select_Table = true;
            show_Select_Table();
        }

        protected void ButtonDown_Click(object sender, EventArgs e)
        {            
            Up_Down_direct = 2;
            jumpTo_show_Select_Table = true;
            show_Select_Table();
        }

        protected void TextBox_Input_TextChanged(object sender, EventArgs e)
        {
            jumpTo_show_Select_Table = true;
            show_Select_Table();
        }

        protected void GridView3_PreRender(object sender, EventArgs e)
        {
            if (GridView3.Rows.Count < 1) return;
            switch (Parameter_Table)
            {
                case 1://CNC Parameter
                    if (GridView3.Rows[0].Cells.Count < 13) return;
                    GridView3.HeaderRow.Cells[11].Visible = false;
                    GridView3.HeaderRow.Cells[12].Visible = false;
                    for (int k=NCA_Var.axes+2;k<=9;k++)
                        GridView3.HeaderRow.Cells[k].Visible = false;
                    for (int i = 0; i < GridView3.Rows.Count; i++)
                    {
                        GridView3.Rows[i].Cells[11].Visible = false;
                        GridView3.Rows[i].Cells[12].Visible = false;
                        for (int k = NCA_Var.axes + 2; k <= 9; k++)
                            GridView3.Rows[i].Cells[k].Visible = false;
                        for (int j = 1; j < 1 + NCA_Var.axes; j++)
                        {
                            if (CNC_Table.Rows[i][13].ToString().Contains("_"))///"_axis" 或 "_spdl"
                            {
                                GridView3.Rows[i].Cells[1].Text  = "";
                                GridView3.Rows[i].Cells[1].BackColor = System.Drawing.Color.Empty;
                                GridView3.Rows[i].Cells[j + 1].BackColor = System.Drawing.Color.LightBlue ;
                                if (NCA_Var.NC_IP == "127.0.0.1" & NCA_Var.spdl == 0) NCA_Var.spdl = 1;
                                if (CNC_Table.Rows[i][13].ToString().Contains("spdl") & j>=1+NCA_Var.spdl)
                                {
                                    GridView3.Rows[i].Cells[j+1].Text = "";
                                    GridView3.Rows[i].Cells[j+1].BackColor = System.Drawing.Color.Empty;
                                }
                            }
                            else
                            {
                                GridView3.Rows[i].Cells[j + 1].BackColor = System.Drawing.Color.Empty;
                                GridView3.Rows[i].Cells[j + 1].Text  = "";
                                GridView3.Rows[i].Cells[1].BackColor = System.Drawing.Color.Yellow;
                            }
                        }
                    }
                    break;
                case 2://Diagnose
                    if (GridView3.Rows[0].Cells.Count < 3) return;                    
                    for (int i = 0; i < GridView3.Rows.Count; i++)
                    {
                        int Data_num;                       
                        if (Convert.ToString(DiagnoseTable.Rows[i][1]).Contains(".") || int.TryParse((Convert.ToString(DiagnoseTable.Rows[i][1])), out Data_num))
                        {
                            GridView3.Rows[i].Cells[1].BackColor = System.Drawing.Color.Yellow;
                        }
                        else
                        { 
                            GridView3.Rows[i].Cells[1].BackColor = System.Drawing.Color.Empty; 
                        }
                        for (int j = 2; j < GridView3.Rows[0].Cells.Count; j++)///先清空
                            GridView3.Rows[i].Cells[j].BackColor = System.Drawing.Color.Empty;
                        if (Convert.ToString(DiagnoseTable.Rows[i][1]).ToUpper().Contains("BIT"))///bit_type 上色 
                            for (int j = 2; j < GridView3.Rows[0].Cells.Count; j++)
                                if (Convert.ToBoolean(DiagnoseTable.Rows[i][j]))
                                    GridView3.Rows[i].Cells[j].BackColor = System.Drawing.Color.Red;                           
                                
                    }
                    break;
                case 3://PMC Address
                    if (GridView3.Rows[0].Cells.Count < 3) return;
                    for (int i = 0; i < GridView3.Rows.Count; i++)
                        for (int j = 2; j < GridView3.Rows[0].Cells.Count;j++ )
                            if (Convert.ToBoolean(PMC_AddressTable.Rows[i][j]))
                                GridView3.Rows[i].Cells[j].BackColor = System.Drawing.Color.Red  ;
                            else
                                GridView3.Rows[i].Cells[j].BackColor = System.Drawing.Color.Empty;
                    break;
            }
        }

    }
}