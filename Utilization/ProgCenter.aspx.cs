using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class CallHelp : System.Web.UI.Page
    {
        DataTable dtProgCenter = new DataTable();
        DataTable dtCNCProg = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Response.Write("No access right to Program Center.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);           
            string str_Path = Server.MapPath("~/Ut_Data/NC_Prog");
            if (!Directory.Exists(str_Path))
            {
                Directory.CreateDirectory(str_Path);
            }
            if (eng)
            {
                Page.Title = "Program Center";
                Label1.Text = @"Programs put at " + str_Path + @"\ ,for Advance Function--->Use QF.exe/Quaser_Link.exe";
            }
            else
                Label1.Text = "程式中心位置在網站的   " + str_Path + "\\a.b.c.d(ip)_(id)\\ , 進階功能--->請使用單機版 QF.exe/Quaser_Link.exe ";
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
                    dtProgCenter = make_dt(dtProgCenter);
                    dtCNCProg = make_dt(dtCNCProg);
                    show_selected_IP();
                }
                else
                {
                    dtProgCenter = make_dt(dtProgCenter);
                    dtCNCProg = make_dt(dtCNCProg);
                }
            }
            else
            {
                if (Request.Cookies["Last_IP"] != null)///離開10分鐘內
                {
                    Label_ConnIP.Text = Request.Cookies["Last_IP"].Value;
                    DropDownList1.SelectedItem.Text = DropDownList1.SelectedItem.Value = Request.Cookies["Last_IP"].Value;
                    dtProgCenter = make_dt(dtProgCenter);
                    dtCNCProg = make_dt(dtCNCProg);
                    show_selected_IP();//if (!Page.IsPostBack) 
                }
                else
                    Response.Redirect(@"~/Default.aspx");
            }

            HandlePanelScrolBar1();
            HandlePanelScrolBar2();
        }
        private DataTable make_dt(DataTable dtBackup)
        {
            dtBackup.Columns.Add("selected", Type.GetType("System.String"));
            dtBackup.Columns.Add("filename", Type.GetType("System.String"));
            dtBackup.Columns.Add("remark", Type.GetType("System.String"));
            dtBackup.Columns.Add("size", Type.GetType("System.String"));
            dtBackup.Columns.Add("date", Type.GetType("System.String"));
            return dtBackup;
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
        string targetPath = "";
        private void show_selected_IP()//先處理目錄
        {
            //先取得CNC_series
            //////////////////////////////////////////////////讀取資料區
            string file_Path = Server.MapPath("~/Ut_Data/NC_Prog");
            targetPath = Get_targetPath();//取 CNC_series,targetPath ,NCA_Var.NC_IP
            if (NCA_Var.NC_IP == "") return;
            Response.Cookies["Last_IP"].Value = DropDownList1.SelectedItem.Value; //再改存放的值 (重點在中文有無亂碼)
            Response.Cookies["Last_IP"].Expires = DateTime.Now.AddYears(10);            
            bool targetPath_error = false;
            var files = Directory.GetDirectories(file_Path);

            //勁勛案後修改
            //bool find_cnc_serial = false;
            //for (int j = files.Length - 1; j >= 0; j--)
            //{
            //    if (CNC_series == "----") break;//無序號不用比了
            //    if (files[j].ToString() == targetPath) continue;
            //    if (files[j].ToString().Contains(CNC_series))
            //        find_cnc_serial = true;
            //}
            //try
            //{
            //    if (find_cnc_serial)//合併同序號的目錄
            //    {
            //        for (int j = files.Length - 1; j >= 0; j--)
            //        {
            //            if (files[j].ToString().Contains(CNC_series))
            //            {
            //                if (files[j].ToString() == targetPath) continue;
            //                CopyDirectory(files[j].ToString(), targetPath, true);
            //                Directory.Delete(files[j].ToString(), true);                            
            //            }
            //        }
            //    }
            //}
            //catch { };
            if (!Directory.Exists(targetPath))//確保沒漏勾
            {
                try
                {
                    Directory.CreateDirectory(targetPath);
                }
                catch
                { targetPath_error = true; }
            }
            GridView1.SelectedIndex = -1; show_Select_Table1();            
            GridView2.SelectedIndex = -1; show_Select_Table2();
            if (targetPath_error)
            {                
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng)
                    Label1.Text = targetPath + "------Directory creat error,check illegal char";
                else
                    Label1.Text = targetPath + "------目錄創建失敗,請檢查是否目錄名稱不合規則";
                Button1.Enabled = false; Button2.Enabled = false;
            }
            else
            {
                Button1.Enabled = true ; Button2.Enabled = true ;
            }
        }
        string CNC_series = "----";
        private static string check_IPC_Commnent(FileInfo File_Info)/// 讀取 PC 端 Comment
        {
            string IPC_PrgComment = "";
            string IPC_File = File_Info.DirectoryName + "\\" + File_Info.Name;
            FileStream Fs;
            byte[] Byte_buf;
            try
            {
                Fs = File.Open(IPC_File, FileMode.Open);
                Byte_buf = new byte[128];//Fs.Length
                Fs.Read(Byte_buf, 0, 128);//(int)Fs.Length
                Fs.Close();
            }
            catch { return IPC_PrgComment; }

            string IPC_Prog_Content = System.Text.Encoding.UTF8.GetString(Byte_buf);
            if (!(IPC_Prog_Content.StartsWith("%") || IPC_Prog_Content.IndexOf("O") == -1))
                IPC_PrgComment = IPC_Prog_Content;
            else
            {
                if (IPC_Prog_Content.Contains("(") && IPC_Prog_Content.Contains(")"))
                {
                    string left_side_out = IPC_Prog_Content.Split('(')[1];
                    IPC_PrgComment = left_side_out.Split(')')[0];
                }
                else
                    IPC_PrgComment = IPC_Prog_Content;
            }        
            return IPC_PrgComment;
        }
        private void show_Select_Table1()//再處理檔案做成Table
        {
            ShowCNCSelect_Button();
            GridView1.DataSource = null; GridView1.DataBind();
            if (targetPath == "") targetPath = Get_targetPath();
            if (!Directory.Exists(targetPath)) return;
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            if (eng)
                Label1.Text = @"Programs are at " + targetPath + @"\ ,for Advance Function--->Use QF.exe/Quaser_Link.exe";
            else
                Label1.Text = @"程式位置在網站的   " + targetPath + @"\ , 進階功能--->請使用單機版 QF.exe/Quaser_Link.exe";
            //更新磁碟目錄
            DirectoryInfo dirinfo;
            DirectoryInfo[] dirs;
            FileInfo[] files;
            dirinfo = new DirectoryInfo(targetPath);
            dirs = dirinfo.GetDirectories();
            files = dirinfo.GetFiles();
            int file_index = -1;
            //處理檔案 
            for (int j = 0; j < files.Length; j++)
            {
                bool nc_file = false;
                int P = 0;
                nc_file = ((files[j].Name.ToUpper().StartsWith("O")) && (files[j].Name.Length >= 5) && (int.TryParse(files[j].Name.ToString().Substring(1, 4), out P))
                 || files[j].Name.ToUpper().Contains(".NC") || files[j].Name.ToUpper().Contains(".TAP"));
                if (!nc_file) continue;
                else file_index++;
                string PrgComment = "";
                DataRow row = dtProgCenter.NewRow();//以 row 為單位做處理
                row["selected"] = file_index == GridView1.SelectedIndex ? "v" : "";
                row["filename"] = files[j].Name;
                PrgComment = check_IPC_Commnent(files[j]);
                row["remark"] = PrgComment;
                row["size"] = string.Format("{0:N0}", files[j].Length);
                row["date"] = files[j].LastWriteTime.ToString("yyyy/MM/dd  HH:mm");
                //nc 檔才要列入處理                
                dtProgCenter.Rows.Add(row);
            }
            GridView1.DataSource = dtProgCenter; GridView1.DataBind();
        }
        private void show_Select_Table2()//再處理檔案做成Table
        {
            ShowCNCSelect_Button();
            GridView2.DataSource = null; GridView2.DataBind();
            if (targetPath == "") targetPath = Get_targetPath();
            if (!Directory.Exists(targetPath)) return;
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            if (targetPath != "")
            {                
                if (eng)
                    Label1.Text = @"Programs are at " + targetPath + @"\ ,for Advance Function--->Use QF.exe/Quaser_Link.exe";
                else
                    Label1.Text = @"程式位置在網站的   " + targetPath + @"\ , 進階功能--->請使用單機版 QF.exe/Quaser_Link.exe";
            }
            if (!NCA_Var.Is_0iD) NCA_Var.cur_30iPATH = "";

            string dir_select = "4";
            bool correct_30i_firstTimePathError = false;
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
            if (NCA_Var.NC_IP == "") return;
            dtCNCProg = NCA_Var.read_NCDir_Table();//GridView9
            if (NCA_Var.cur_30iPATH != DropDownList2.SelectedItem.Value) correct_30i_firstTimePathError = true;
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
            if (dtCNCProg == null) return;
            GridView2.DataSource = dtCNCProg;
            if (dtCNCProg.Columns.Count > 4)
            {
                dtCNCProg.Columns[0].ColumnName = "selected";
                dtCNCProg.Columns[1].ColumnName = "filename";
                dtCNCProg.Columns[2].ColumnName = "remark";
                dtCNCProg.Columns[3].ColumnName = "size";
                dtCNCProg.Columns[4].ColumnName = "date";
            }
            GridView2.DataBind();
            if (NCA_Var.Is_30i)
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
            else DropDownList2.Visible = false;
            if (GridView2.Rows.Count > 0)
            {
                for (int j = 0; j < GridView2.Rows.Count; j++)
                {
                    if (CNC_select) GridView2.Rows[j].Cells[1].Text = "v";
                    else
                    {
                        if (j == GridView2.SelectedIndex)
                            GridView2.Rows[j].Cells[1].Text = "v";
                        else
                            GridView2.Rows[j].Cells[1].Text = "";
                    }
                    if (dtCNCProg.Rows[j][2].ToString().Contains("使用中程式"))
                    {
                        if (eng)
                          GridView2.Rows[j].Cells[3].Text = GridView2.Rows[j].Cells[3].Text.Trim().Replace(@"使用中程式",@"Running Program");
                        GridView2.Rows[j].Cells[1].BackColor = System.Drawing.Color.Yellow;
                        GridView2.Rows[j].Cells[2].BackColor = System.Drawing.Color.Yellow;
                        GridView2.Rows[j].Cells[3].BackColor = System.Drawing.Color.Yellow;
                        GridView2.Rows[j].Cells[4].BackColor = System.Drawing.Color.Yellow;
                    }
                    else
                    {
                        GridView2.Rows[j].Cells[1].BackColor = System.Drawing.Color.Empty;
                        GridView2.Rows[j].Cells[2].BackColor = System.Drawing.Color.Empty;
                        GridView2.Rows[j].Cells[3].BackColor = System.Drawing.Color.Empty;
                        GridView2.Rows[j].Cells[4].BackColor = System.Drawing.Color.Empty;
                    }
                }
            }
        }

        int dir_30i = 4;
        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count > 0 & GridView1.SelectedIndex >= 0)
            {
                GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text = "";
                GridView1.SelectedIndex = -1;
            }
            dir_30i = DropDownList2.SelectedIndex;
            NCA_Var.cur_30iPATH = DropDownList2.SelectedItem.Value;
            show_Select_Table2();
        }

        private void Get_NC_IP()
        {
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            NCA_Var.NC_IP = conn_IP[0];
        }
        private string Get_targetPath()
        {
            Get_NC_IP();
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
            NCA_Var.read_CNC_Type();
            if (NCA_Var.NC_Handle != 0 && CNC_series != NCA_Var.cnc_serial)
                CNC_series = NCA_Var.cnc_serial;
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
            string str_Path = Server.MapPath("~/Ut_Data/NC_Prog");
            string resverDir = Label_ConnIP.Text + "_" + CNC_series;
            string targetPath; ;
            if (CNC_series == "----") targetPath = str_Path + "\\" + NCA_Var.NC_IP;
            else
                targetPath = str_Path + "\\" + resverDir;
            return targetPath;
        }
        //IPC--->CNC
        protected void Button1_Click(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            bool DEMO_Mode = Convert.ToBoolean(Application["DEMO_Mode"].ToString());
            if (DEMO_Mode)
            {
                Label1.Text  = eng ? "DEMO_Mode" : "展示模式";
                return;
            }
            if (DropDownList2.Visible)
            {
                dir_30i = DropDownList2.SelectedIndex;
                NCA_Var.cur_30iPATH = DropDownList2.SelectedItem.Value;
            }
            if (GridView1.Rows.Count <= 0) return;
            if (GridView1.SelectedIndex < 0) return;//沒選到就不用往下做
            targetPath = NCA_Var.NCPRG_Path = Get_targetPath();
            if (!Directory.Exists(targetPath)) return;
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(NCA_Var.NC_IP, handle);
            NCA_Var.Pack_index = 0;
            NCA_Var.file_name = GridView1.SelectedRow.Cells[2].Text;
            
            if (NCA_Var.file_name.ToUpper().Contains("_TO_"))///插入檢查 是否 打包程式的檢查
            {
                Focas1.IODBPSD_3 prmData = new Focas1.IODBPSD_3();
                Focas1.IODBPSD_1 prmDataNoAxis = new Focas1.IODBPSD_1();
                Focas1.cnc_rdparam(handle, 3201, -1, 5, prmData);//要改3201#2REP 
                if ((prmData.cdatas[0] & 4) == 0)
                {                   
                    if(eng)
                        Label1.Text = "Check 3201#2 REP=1? Writting will fail";
                    else
                       Label1.Text = "是否參數3201#2 REP=1?若否 可能覆寫CNC程式時會寫入失敗";                                   
                }
            }
            NCA_Var.ASPNET_Sent_IPC_To_MEM(handle, false);
            handle = NCA_Var.CNC_Release_Handle(handle);
            GridView2.SelectedIndex = -1; show_Select_Table2();
            Label1.Text = NCA_Var.Focas_ret_message;
            if (eng)
            {
                if (Label1.Text.Contains("傳送")) Label1.Text = Label1.Text.Replace("傳送", "Send_");
                if (Label1.Text.Contains("成功")) Label1.Text = Label1.Text.Replace("成功", "Success");
                if (Label1.Text.Contains("失敗")) Label1.Text = Label1.Text.Replace("失敗", "Fail");
                if (Label1.Text.Contains("結束")) Label1.Text = Label1.Text.Replace("結束", "Success");
                string tmp = "請按至 EDIT 模式再試一次 ,檢查程式是否正在執行或背景編輯";
                if (Label1.Text.Contains(tmp)) Label1.Text = Label1.Text.Replace(tmp, "Switch to EDIT mode and retry,check whether bg_EDIT or running?");
            }
        }
        //CNC--->IPC
        protected void Button2_Click(object sender, EventArgs e)
        {
            if (DropDownList2.Visible)
            {
                dir_30i = DropDownList2.SelectedIndex;
                NCA_Var.cur_30iPATH = DropDownList2.SelectedItem.Value;
            }
            if (GridView2.Rows.Count <= 0) return;
            bool[] dgv2selected = new bool[GridView2.Rows.Count];
            bool standby = false;
            for (int j = 0; j < GridView2.Rows.Count; j++)
            {
                if (GridView2.Rows[j].Cells[1].Text == "v")
                { dgv2selected[j] = true; standby = true; }
            }
            if (!standby) return;//沒選到就不用往下做

            NCA_Var.NCPRG_Path = Get_targetPath();
            //先暫存變數  待迴圈時取出使用
            string IP = NCA_Var.NC_IP;
            string targetPath = NCA_Var.NCPRG_Path;
            if (!Directory.Exists(targetPath)) return;
            bool Is_0iD = NCA_Var.Is_0iD;
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(NCA_Var.NC_IP, handle);
            for (int i = 0; i < GridView2.Rows.Count; ++i)
            {
                if (!dgv2selected[i]) continue;
                NCA_Var.Pack_index = 0;
                string delCNCFile = GridView2.Rows[i].Cells[2].Text;
                short iCNCProgNum = 0;
                if (!Is_0iD)///插入0i/30i的處理
                {
                    if (!short.TryParse(delCNCFile.Remove(0, 1), out iCNCProgNum)) break;
                    NCA_Var.file_name = "O" + string.Format("{0:0000}", iCNCProgNum);
                }
                else
                    NCA_Var.file_name = delCNCFile;
                NCA_Var.from_O = NCA_Var.to_O = iCNCProgNum;
                NCA_Var.NCPRG_Path = targetPath;
                if (!NCA_Var.ASPNET_BackUp_MEM_To_IPC(handle, false)) break;
            }
            handle = NCA_Var.CNC_Release_Handle(handle);
            GridView1.SelectedIndex = -1; show_Select_Table1();
            Label1.Text = NCA_Var.Focas_ret_message;
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            if (eng)
            {
                if (Label1.Text.Contains("傳送")) Label1.Text = Label1.Text.Replace("傳送", "Send_");
                if (Label1.Text.Contains("成功")) Label1.Text = Label1.Text.Replace("成功", "Success");
                if (Label1.Text.Contains("失敗")) Label1.Text = Label1.Text.Replace("失敗", "Fail");
                if (Label1.Text.Contains("結束")) Label1.Text = Label1.Text.Replace("結束", "Success");
            }
        }

        //CNC全選
        bool CNC_select = false;
        protected void Button3_Click(object sender, EventArgs e)
        {
            Get_CNCSelect();
            CNC_select = !CNC_select;
            if (GridView1.Rows.Count > 0 & GridView1.SelectedIndex >= 0)
            {
                GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text = "";
                GridView1.SelectedIndex = -1;
            }
            if (DropDownList2.Visible)
            {
                dir_30i = DropDownList2.SelectedIndex;
                NCA_Var.cur_30iPATH = DropDownList2.SelectedItem.Value;
            }
            show_Select_Table2();
        }
        private void ShowCNCSelect_Button()
        {
            if (CNC_select)
                Button3.BackColor = System.Drawing.Color.GreenYellow;
            else
                Button3.BackColor = System.Drawing.Color.Empty;
        }
        private void Get_CNCSelect()
        {
            if (Button3.BackColor == System.Drawing.Color.GreenYellow)
                CNC_select = true;
            else
                CNC_select = false;
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GridView1.SelectedIndex = e.NewSelectedIndex;
            GridView2.SelectedIndex = -1;
            for (int j = 0; j < GridView2.Rows.Count; j++)
            {
                GridView2.Rows[j].Cells[1].Text = "";
            }
            if (DropDownList2.Visible)
            {
                dir_30i = DropDownList2.SelectedIndex;
                NCA_Var.cur_30iPATH = DropDownList2.SelectedItem.Value;
            }
            show_Select_Table1();
        }

        protected void GridView2_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GridView2.SelectedIndex = e.NewSelectedIndex;
            if (GridView1.Rows.Count > 0 & GridView1.SelectedIndex >= 0)
            {
                GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text = "";
                GridView1.SelectedIndex = -1;
            }
            if (DropDownList2.Visible)
            {
                dir_30i = DropDownList2.SelectedIndex;
                NCA_Var.cur_30iPATH = DropDownList2.SelectedItem.Value;
            }
            show_Select_Table2();
        }

        private void HandlePanelScrolBar1()//控制Panel內的捲軸位置
        {
            //定义两个HiddenField，分别纪录Panel的ScrollBar的X与Y位置
            HiddenField HF_ScrollPosX = new HiddenField();
            HiddenField HF_ScrollPosY = new HiddenField();

            HF_ScrollPosX.ID = "ScrollPosX1";
            HF_ScrollPosY.ID = "ScrollPosY1";
            Panel1.Controls.Add(HF_ScrollPosX);
            Panel1.Controls.Add(HF_ScrollPosY);

            //生成JS：将Panel的ScrollBar的X,Y位置设置给两个HiddenField
            string script;
            script = "window.document.getElementById('" + HF_ScrollPosX.ClientID + "').value = "
                      + "window.document.getElementById('" + Panel1.ClientID + "').scrollLeft;"
                      + "window.document.getElementById('" + HF_ScrollPosY.ClientID + "').value = "
                      + "window.document.getElementById('" + Panel1.ClientID + "').scrollTop;";

            this.ClientScript.RegisterOnSubmitStatement(this.GetType(), "SavePanelScroll", script);

            if (IsPostBack) //如果是PostBack，将保存在HiddenField的ScrollBar的X,Y值重设回给Panel的ScrollBar
            {
                script = "window.document.getElementById('" + Panel1.ClientID + "').scrollLeft = "
                        + "window.document.getElementById('" + HF_ScrollPosX.ClientID + "').value;"
                        + "window.document.getElementById('" + Panel1.ClientID + "').scrollTop = "
                        + "window.document.getElementById('" + HF_ScrollPosY.ClientID + "').value;";

                this.ClientScript.RegisterStartupScript(this.GetType(), "SetPanelScroll", script, true);
            }
        }
        private void HandlePanelScrolBar2()//控制Panel內的捲軸位置
        {
            //定义两个HiddenField，分别纪录Panel的ScrollBar的X与Y位置
            HiddenField HF_ScrollPosX = new HiddenField();
            HiddenField HF_ScrollPosY = new HiddenField();

            HF_ScrollPosX.ID = "ScrollPosX2";
            HF_ScrollPosY.ID = "ScrollPosY2";
            Panel2.Controls.Add(HF_ScrollPosX);
            Panel2.Controls.Add(HF_ScrollPosY);

            //生成JS：将Panel的ScrollBar的X,Y位置设置给两个HiddenField
            string script;
            script = "window.document.getElementById('" + HF_ScrollPosX.ClientID + "').value = "
                      + "window.document.getElementById('" + Panel2.ClientID + "').scrollLeft;"
                      + "window.document.getElementById('" + HF_ScrollPosY.ClientID + "').value = "
                      + "window.document.getElementById('" + Panel2.ClientID + "').scrollTop;";

            this.ClientScript.RegisterOnSubmitStatement(this.GetType(), "SavePanelScrol2", script);

            if (IsPostBack) //如果是PostBack，将保存在HiddenField的ScrollBar的X,Y值重设回给Panel的ScrollBar
            {
                script = "window.document.getElementById('" + Panel2.ClientID + "').scrollLeft = "
                        + "window.document.getElementById('" + HF_ScrollPosX.ClientID + "').value;"
                        + "window.document.getElementById('" + Panel2.ClientID + "').scrollTop = "
                        + "window.document.getElementById('" + HF_ScrollPosY.ClientID + "').value;";

                this.ClientScript.RegisterStartupScript(this.GetType(), "SetPanelScrol2", script, true);
            }
        }

        private void CopyDirectory(string SourceFolder, string DestinationFolder, bool IsOverWrite = true)
        {
            if (!Directory.Exists(SourceFolder)) return;
            //目標目錄不存在則新建
            if (!Directory.Exists(DestinationFolder))
                Directory.CreateDirectory(DestinationFolder);
            DirectoryInfo srcDir = new DirectoryInfo(SourceFolder);
            try
            {
                //先複製目錄下檔案過去
                foreach (FileInfo fi in srcDir.EnumerateFiles())
                    File.Copy(fi.FullName, DestinationFolder + Path.DirectorySeparatorChar + fi.Name, IsOverWrite);
                //目錄下的目錄再用遞迴方式複製
                foreach (DirectoryInfo di in srcDir.EnumerateDirectories())
                    CopyDirectory(di.FullName, DestinationFolder + Path.DirectorySeparatorChar + di.Name);
            }
            catch
            {
                throw;
            }
        }
    }
}