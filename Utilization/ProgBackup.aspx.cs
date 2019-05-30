using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class ProgBackup : System.Web.UI.Page
    {
        DataTable dtBackup = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Response.Write("No access right to Backup Center.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Backup Center";
            }
            string str_Path = Server.MapPath("~/Ut_Data/NC_Prog");
            if (!Directory.Exists(str_Path))
            {
                Directory.CreateDirectory(str_Path);
            }
            Label1.Text = "備份位置在網站的   " + str_Path + "\\a.b.c.d(ip)_(id)\\   還原時請取出  使用單機版 QF.exe/Quaser_Link.exe 作還原";
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
                    dtBackup = make_dt(dtBackup);
                    show_selected_IP();
                }
                else
                {
                    dtBackup = make_dt(dtBackup);
                }
            }
            else
            {
                if (Request.Cookies["Last_IP"] != null)///離開10分鐘內
                {
                    Label_ConnIP.Text = Request.Cookies["Last_IP"].Value;
                    DropDownList1.SelectedItem.Text = DropDownList1.SelectedItem.Value = Request.Cookies["Last_IP"].Value;
                    dtBackup = make_dt(dtBackup);
                    show_selected_IP();//if (!Page.IsPostBack) 
                }
                else
                    Response.Redirect(@"~/Default.aspx");
            }
        }
        private DataTable make_dt(DataTable dtBackup)
        {
            dtBackup.Columns.Add("ConnStatus", Type.GetType("System.String"));
            dtBackup.Columns.Add("ConnType", Type.GetType("System.String"));
            dtBackup.Columns.Add("ConnFile", Type.GetType("System.String"));
            dtBackup.Columns.Add("ConnDate", Type.GetType("System.String"));
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
        private void show_selected_IP()//先處理目錄
        {
            Get_ShowButton();
            //先取得CNC_series
            //////////////////////////////////////////////////讀取資料區
            string file_Path = Server.MapPath("~/Ut_Data/NC_Prog");
            string targetPath = Get_targetPath();//取 CNC_series,targetPath ,NCA_Var.NC_IP
            if (NCA_Var.NC_IP == "") return;
            Response.Cookies["Last_IP"].Value = DropDownList1.SelectedItem.Value; //再改存放的值 (重點在中文有無亂碼)
            Response.Cookies["Last_IP"].Expires = DateTime.Now.AddYears(10);
            //勁勛案後修改
            //bool find_cnc_serial = false;
            //try
            //{
            //    var files = Directory.GetDirectories(file_Path);
            //    for (int j = files.Length - 1; j >= 0; j--)
            //    {
            //        if (CNC_series == "----") break;//無序號不用比了
            //        if (files[j].ToString() == targetPath) continue;
            //        if (files[j].ToString().Contains(CNC_series))
            //            find_cnc_serial = true;
            //    }                
                //if (find_cnc_serial)//合併同序號的目錄
                //{
                //    for (int j = files.Length - 1; j >= 0; j--)
                //    {
                //        if (files[j].ToString().Contains(CNC_series))
                //        {
                //            if (files[j].ToString() == targetPath) continue;
                //            CopyDirectory(files[j].ToString(), targetPath, true);
                //            Directory.Delete(files[j].ToString(), true);
                //        }
                //    }
                //}
            //}
            //catch { };
            bool targetPath_error = false;
            if (!Directory.Exists(targetPath))//確保沒漏勾
            {
                try
                {
                    Directory.CreateDirectory(targetPath);
                }
                catch
                { targetPath_error = true; }
            }
            show_Select_Table();
            if (targetPath_error)
            {
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng)
                    Label1.Text = targetPath + "------Directory creat error,check illegal char";
                else
                    Label1.Text = targetPath + "------目錄創建失敗,請檢查是否目錄名稱不合規則";
            }
        }

        string CNC_series = "----";
        private void show_Select_Table()//再處理檔案做成Table
        {
            GridView1.DataSource = null; GridView1.DataBind();
            Button1.BackColor = System.Drawing.Color.Empty;
            Button2.BackColor = System.Drawing.Color.Empty;
            Button3.BackColor = System.Drawing.Color.Empty;
            Button4.BackColor = System.Drawing.Color.Empty;
            Button5.BackColor = System.Drawing.Color.Empty;
            Button6.BackColor = System.Drawing.Color.Empty;
            switch (Button_sel)
            {
                case 1:
                    Button1.BackColor = System.Drawing.Color.GreenYellow;
                    break;
                case 2:
                    Button2.BackColor = System.Drawing.Color.GreenYellow;
                    break;
                case 3:
                    Button3.BackColor = System.Drawing.Color.GreenYellow;
                    break;
                case 4:
                    Button4.BackColor = System.Drawing.Color.GreenYellow;
                    break;
                case 5:
                    Button5.BackColor = System.Drawing.Color.GreenYellow;
                    break;
                case 6:
                    Button6.BackColor = System.Drawing.Color.GreenYellow;
                    break;
            }
            //////////////////////////////////////////////////////////////////////讀取資料區結束            
            string targetPath = Get_targetPath();
            if (!Directory.Exists(targetPath)) return;

            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            if (eng)
                Label1.Text = "Backup files at " + targetPath + "\\   Please Use QF.exe/Quaser_Link.exe to restore it";
            else
                Label1.Text = "備份位置在網站的   " + targetPath + "\\   還原時請取出  使用單機版 QF.exe/Quaser_Link.exe 作還原";
            //以下再處理檔案做成Table
            //程式備份
            DataRow dT = dtBackup.NewRow();
            dT["ConnType"] = Button1.Text;
            dT["ConnFile"] = "0001_to_9999.nc";//0001_to_9999.nc ; EXT_WKS.TXT ; TOOLOFST.TXT ; CNC-PARA.TXT ; MACRO.TXT ; PITCH.TXT
            if (NCA_Var.Is_30i) dT["ConnFile"] += " (PATH1)";
            string dt_file = targetPath + "\\" + "0001_to_9999.nc";
            if (File.Exists(dt_file))
            {
                FileInfo Files = new FileInfo(dt_file);
                dT["ConnStatus"] = (eng ? "Done" : "已做好");
                dT["ConnDate"] = Files.LastWriteTime.ToString();///1202
            }
            else
            {
                dT["ConnStatus"] = (eng ? "Not yet" : "尚未做");
                dT["ConnDate"] = "";
            }
            dtBackup.Rows.Add(dT);
            //座標備份
            dT = dtBackup.NewRow();
            dT["ConnType"] = Button2.Text;
            dT["ConnFile"] = "EXT_WKS.TXT";//0001_to_9999.nc ; EXT_WKS.TXT ; TOOLOFST.TXT ; CNC-PARA.TXT ; MACRO.TXT ; PITCH.TXT
            dt_file = targetPath + "\\" + "EXT_WKS.TXT";
            if (File.Exists(dt_file))
            {
                FileInfo Files = new FileInfo(dt_file);
                dT["ConnStatus"] = (eng ? "Done" : "已做好");
                dT["ConnDate"] = Files.LastWriteTime.ToString();///1202
            }
            else
            {
                dT["ConnStatus"] = (eng ? "Not yet" : "尚未做");
                dT["ConnDate"] = "";
            }
            dtBackup.Rows.Add(dT);
            //刀具備份
            dT = dtBackup.NewRow();
            dT["ConnType"] = Button3.Text;
            dT["ConnFile"] = "TOOLOFST.TXT";//0001_to_9999.nc ; EXT_WKS.TXT ; TOOLOFST.TXT ; CNC-PARA.TXT ; MACRO.TXT ; PITCH.TXT
            dt_file = targetPath + "\\" + "TOOLOFST.TXT";
            if (File.Exists(dt_file))
            {
                FileInfo Files = new FileInfo(dt_file);
                dT["ConnStatus"] = (eng ? "Done" : "已做好");
                dT["ConnDate"] = Files.LastWriteTime.ToString();///1202
            }
            else
            {
                dT["ConnStatus"] = (eng ? "Not yet" : "尚未做");
                dT["ConnDate"] = "";
            }
            dtBackup.Rows.Add(dT);
            //參數備份
            dT = dtBackup.NewRow();
            dT["ConnType"] = Button4.Text;
            dT["ConnFile"] = "CNC-PARA.TXT";//0001_to_9999.nc ; EXT_WKS.TXT ; TOOLOFST.TXT ; CNC-PARA.TXT ; MACRO.TXT ; PITCH.TXT
            dt_file = targetPath + "\\" + "CNC-PARA.TXT";
            if (File.Exists(dt_file))
            {
                FileInfo Files = new FileInfo(dt_file);
                dT["ConnStatus"] = (eng ? "Done" : "已做好");
                dT["ConnDate"] = Files.LastWriteTime.ToString();///1202
            }
            else
            {
                dT["ConnStatus"] = (eng ? "Not yet" : "尚未做");
                dT["ConnDate"] = "";
            }
            dtBackup.Rows.Add(dT);
            //Macro備份
            dT = dtBackup.NewRow();
            dT["ConnType"] = Button5.Text;
            dT["ConnFile"] = "MACRO.TXT";//0001_to_9999.nc ; EXT_WKS.TXT ; TOOLOFST.TXT ; CNC-PARA.TXT ; MACRO.TXT ; PITCH.TXT
            dt_file = targetPath + "\\" + "MACRO.TXT";
            if (File.Exists(dt_file))
            {
                FileInfo Files = new FileInfo(dt_file);
                dT["ConnStatus"] = (eng ? "Done" : "已做好");
                dT["ConnDate"] = Files.LastWriteTime.ToString();///1202
            }
            else
            {
                dT["ConnStatus"] = (eng ? "Not yet" : "尚未做");
                dT["ConnDate"] = "";
            }
            dtBackup.Rows.Add(dT);
            //Pitch備份
            dT = dtBackup.NewRow();
            dT["ConnType"] = Button6.Text;
            dT["ConnFile"] = "PITCH.TXT";//0001_to_9999.nc ; EXT_WKS.TXT ; TOOLOFST.TXT ; CNC-PARA.TXT ; MACRO.TXT ; PITCH.TXT
            dt_file = targetPath + "\\" + "PITCH.TXT";
            if (File.Exists(dt_file))
            {
                FileInfo Files = new FileInfo(dt_file);
                dT["ConnStatus"] = (eng ? "Done" : "已做好");
                dT["ConnDate"] = Files.LastWriteTime.ToString();///1202
            }
            else
            {
                dT["ConnStatus"] = (eng ? "Not yet" : "尚未做");
                dT["ConnDate"] = "";
            }
            dtBackup.Rows.Add(dT);
            GridView1.DataSource = dtBackup; GridView1.DataBind();
        }

        int Button_sel = 0;
        private void Get_ShowButton()
        {
            if (Button1.BackColor == System.Drawing.Color.GreenYellow)
                Button_sel = 1;
            else
                if (Button2.BackColor == System.Drawing.Color.GreenYellow)
                    Button_sel = 2;
                else
                    if (Button3.BackColor == System.Drawing.Color.GreenYellow)
                        Button_sel = 3;
                    else
                        if (Button4.BackColor == System.Drawing.Color.GreenYellow)
                            Button_sel = 4;
                        else
                            if (Button5.BackColor == System.Drawing.Color.GreenYellow)
                                Button_sel = 5;
                            else
                                if (Button6.BackColor == System.Drawing.Color.GreenYellow)
                                    Button_sel = 6;
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
        //程式
        protected void Button1_Click(object sender, EventArgs e)
        {
            Button_sel = 1;
            NCA_Var.Pack_index = 0; NCA_Var.from_O = 1; NCA_Var.to_O = 9999;
            NCA_Var.file_name = "0001_to_9999.nc";
            NCA_Var.NCPRG_Path = Get_targetPath();            
            if (!Directory.Exists(NCA_Var.NCPRG_Path))
            {
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng)
                    Label1.Text = NCA_Var.NCPRG_Path + "------Directory creat error,check illegal char";
                else
                    Label1.Text = NCA_Var.NCPRG_Path + "------目錄創建失敗,請檢查是否目錄名稱不合規則";
                return;
            }
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(NCA_Var.NC_IP, handle);
            NCA_Var.ASPNET_BackUp_MEM_To_IPC(handle, true);
            show_Select_Table();            
        }
        //座標
        protected void Button2_Click(object sender, EventArgs e)
        {
            Button_sel = 2;
            NCA_Var.Pack_index = 5; NCA_Var.from_O = 1; NCA_Var.to_O = 9999;
            NCA_Var.file_name = "EXT_WKS.TXT";
            NCA_Var.NCPRG_Path = Get_targetPath();            
            if (!Directory.Exists(NCA_Var.NCPRG_Path))
            {
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng)
                    Label1.Text = NCA_Var.NCPRG_Path + "------Directory creat error,check illegal char";
                else
                    Label1.Text = NCA_Var.NCPRG_Path + "------目錄創建失敗,請檢查是否目錄名稱不合規則";
                return;
            }
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(NCA_Var.NC_IP, handle);
            NCA_Var.ASPNET_BackUp_MEM_To_IPC(handle, true);
            show_Select_Table();            
        }
        //刀具
        protected void Button3_Click(object sender, EventArgs e)
        {
            Button_sel = 3;
            NCA_Var.Pack_index = 1; NCA_Var.from_O = 1; NCA_Var.to_O = 9999;
            NCA_Var.file_name = "TOOLOFST.TXT";
            NCA_Var.NCPRG_Path = Get_targetPath();
            if (!Directory.Exists(NCA_Var.NCPRG_Path))
            {
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng)
                    Label1.Text = NCA_Var.NCPRG_Path + "------Directory creat error,check illegal char";
                else
                    Label1.Text = NCA_Var.NCPRG_Path + "------目錄創建失敗,請檢查是否目錄名稱不合規則";
                return;
            }
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(NCA_Var.NC_IP, handle);
            NCA_Var.ASPNET_BackUp_MEM_To_IPC(handle, true);
            show_Select_Table();
        }
        //參數
        protected void Button4_Click(object sender, EventArgs e)
        {
            Button_sel = 4;
            NCA_Var.Pack_index = 2; NCA_Var.from_O = 1; NCA_Var.to_O = 9999;
            NCA_Var.file_name = "CNC-PARA.TXT";
            NCA_Var.NCPRG_Path = Get_targetPath();
            if (!Directory.Exists(NCA_Var.NCPRG_Path))
            {
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng)
                    Label1.Text = NCA_Var.NCPRG_Path + "------Directory creat error,check illegal char";
                else
                    Label1.Text = NCA_Var.NCPRG_Path + "------目錄創建失敗,請檢查是否目錄名稱不合規則";
                return;
            }
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(NCA_Var.NC_IP, handle);
            NCA_Var.ASPNET_BackUp_MEM_To_IPC(handle, true);
            show_Select_Table();
        }
        //Macro
        protected void Button5_Click(object sender, EventArgs e)
        {
            Button_sel = 5;
            NCA_Var.Pack_index = 4; NCA_Var.from_O = 1; NCA_Var.to_O = 9999;
            NCA_Var.file_name = "MACRO.TXT";
            NCA_Var.NCPRG_Path = Get_targetPath();
            if (!Directory.Exists(NCA_Var.NCPRG_Path))
            {
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng)
                    Label1.Text = NCA_Var.NCPRG_Path + "------Directory creat error,check illegal char";
                else
                    Label1.Text = NCA_Var.NCPRG_Path + "------目錄創建失敗,請檢查是否目錄名稱不合規則";
                return;
            }
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(NCA_Var.NC_IP, handle);
            NCA_Var.ASPNET_BackUp_MEM_To_IPC(handle, true);
            show_Select_Table();
        }
        //Pitch
        protected void Button6_Click(object sender, EventArgs e)
        {
            Button_sel = 6;
            NCA_Var.Pack_index = 3; NCA_Var.from_O = 1; NCA_Var.to_O = 9999;
            NCA_Var.file_name = "PITCH.TXT";
            NCA_Var.NCPRG_Path = Get_targetPath();
            if (!Directory.Exists(NCA_Var.NCPRG_Path))
            {
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                if (eng)
                    Label1.Text = NCA_Var.NCPRG_Path + "------Directory creat error,check illegal char";
                else
                    Label1.Text = NCA_Var.NCPRG_Path + "------目錄創建失敗,請檢查是否目錄名稱不合規則";
                return;
            }
            ushort handle = 0;
            handle = NCA_Var.CNC_MultiConnect(NCA_Var.NC_IP, handle);
            NCA_Var.ASPNET_BackUp_MEM_To_IPC(handle, true);
            show_Select_Table();
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                if (Button_sel == 0) GridView1.Rows[i].BackColor = System.Drawing.Color.Empty;
                if (i + 1 == Button_sel)
                    GridView1.Rows[i].BackColor = System.Drawing.Color.LightBlue;
                else
                    GridView1.Rows[i].BackColor = System.Drawing.Color.Empty;
                if (GridView1.Rows[i].Cells[0].Text.Contains("未") || GridView1.Rows[i].Cells[0].Text.Contains("Not"))
                {
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.LightPink;
                    GridView1.Rows[i].Cells[2].Text = "";
                    if (Button_sel == i+1)
                        Label1.Text = GridView1.Rows[i].Cells[1].Text + " Fail !!!";
                }
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