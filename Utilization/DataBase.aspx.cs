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
    public partial class DataBase : System.Web.UI.Page
    {
        string CNC_series = "----";
        DataTable dtRecord = new DataTable();
        DataTable dtHistory = new DataTable();
        private DataTable make_dt(DataTable dtRecord)
        {
            dtRecord.Columns.Add("ConnStatus", Type.GetType("System.String"));
            dtRecord.Columns.Add("ConnSeries", Type.GetType("System.String"));
            dtRecord.Columns.Add("ConnIP", Type.GetType("System.String"));
            dtRecord.Columns.Add("RecordItem", Type.GetType("System.String"));
            dtRecord.Columns.Add("Recording", Type.GetType("System.Boolean"));
            return dtRecord;
        }
        private void make_dtHistry()
        {
            if (dtHistory.Columns.Count > 0)
            { dtHistory.Rows.Clear(); dtHistory.Columns.Clear(); }
            dtHistory.Columns.Add("Time", Type.GetType("System.String"));
            dtHistory.Columns.Add("Data", Type.GetType("System.String"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Response.Write("No access right to History Data.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            if (Convert.ToBoolean(Session["clear_7"])) clear_7 = true;
             int t = Convert.ToInt32(Session["language"].ToString());
             if (t == 0)
             {
                 Page.Title = "History Data";
             }
            string str_Path = Server.MapPath("~/Ut_Data/Histry");
            if (!Directory.Exists(str_Path))
            {
                Directory.CreateDirectory(str_Path);
            }
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                DropDownBind();
                show_english_or_not();
                if (!Page.IsPostBack)///從別的頁面切回來
                {
                    DropDownList4.SelectedIndex = DateTime.Now.Hour;
                    if (Request.Cookies["Last_IP"] != null)///離開10分鐘內
                    {
                        string[] conn_IP;
                        conn_IP = Request.Cookies["Last_IP"].Value.Split('-');
                        //先修正 Label_ConnIP.Text 跟 DropDownList1 不一致
                        if (!DropDownList1.SelectedItem.Value.Contains(conn_IP[0]))
                        {
                            for (int j = 0; j < DropDownList1.Items.Count; j++)
                                if (DropDownList1.Items[j].Value.Contains(conn_IP[0]))
                                    DropDownList1.SelectedIndex = j;
                        }
                        Label_ConnIP.Text = DropDownList1.SelectedItem.Text;//.Value
                    }
                    else
                    {
                        DropDownList2Bind();
                        if (DropDownList1.SelectedIndex >= 0)
                            Label_ConnIP.Text = DropDownList1.SelectedItem.Text;//.Value
                    }
                    Get_CNC_series(); //checkDropdownlist2Items();
                    dtRecord = make_dt(dtRecord);
                    show_selected_IP();
                    make_dtHistry();
                    Show_Button3();
                    checkHistory();
                }
                else
                { dtRecord = make_dt(dtRecord); make_dtHistry(); }
            }
            else
            {
                DropDownList4.SelectedIndex = DateTime.Now.Hour;
                {//預設改成英文加上這段
                    Button1.Text = "Add new item";
                    Button2.Text = "Clear data item 7days ago";
                    Button3.Text = "Hide Control table";
                    DropDownList3.Items[0].Text = "Loading";
                    DropDownList3.Items[0].Value = "Loading";
                    DropDownList3.Items[1].Text = "PartsCount";
                    DropDownList3.Items[1].Value = "PartsCount";
                }
                if (Request.Cookies["Last_IP"] != null)///離開10分鐘內
                {                    
                    Label_ConnIP.Text = DropDownList1.SelectedItem.Text = DropDownList1.SelectedItem.Value = Request.Cookies["Last_IP"].Value;
                    Get_CNC_series(); //checkDropdownlist2Items();
                    dtRecord = make_dt(dtRecord);
                    show_selected_IP();
                    make_dtHistry();
                    Show_Button3();
                    checkHistory();
                }//下面這段 else 與 其它頁不同  不須轉回Default.aspx
                //else 
                //    Response.Redirect(@"~/Default.aspx");
            }
        }
        private void show_english_or_not()
        {            
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                
                Button1.Text = "Add new item";
                Button2.Text = "Clear data item 7days ago";
                Button3.Text = "Hide Control table";
                //if (!Page.IsPostBack)
                //     Chart1.Titles[0].Text = "Spindle_Loading %";
                GridView1.EmptyDataText = "Can't find control table !!!";
                GridView2.EmptyDataText = "Can't find history data!!!";
                GridView1.Columns[0].HeaderText = "Conn.Status";
                GridView1.Columns[2].HeaderText = "ConnectingIP-Host";
                GridView1.Columns[3].HeaderText = "RecordItems";
                GridView1.Columns[4].HeaderText = "Recording?";
                DropDownList3.Items[0].Text = "Loading";
                DropDownList3.Items[0].Value  = "Loading";
                DropDownList3.Items[1].Text = "PartsCount";
                DropDownList3.Items[1].Value = "PartsCount";
            }
        }

        private void DropDownBind()//這節跟其它頁不同
        {
            int Hosts_num = Convert.ToInt32(Application["Hosts_num"]);
            if (Hosts_num <= 0) return;
            if (DropDownList1.Items.Count != Hosts_num)
            {
                DropDownList1.Items.Clear();
                for (int i = 0; i < Hosts_num; i++)
                {
                    if (Application["Host" + i].ToString() == "") break;
                    DropDownList1.Items.Add(Application["Host" + i].ToString());
                    DropDownList1.Items[i].Value = Application["Host" + i].ToString();
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
                    DropDownList1.Items[i].Text = Application["Host" + i].ToString();
                    DropDownList1.Items[i].Value = Application["Host" + i].ToString();
                }
            }
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {                   
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            bool axes_unknown = false;
            checkDropdownlist2Items();
            GridView1.EditIndex = -1;    

            if (DropDownList3.SelectedIndex <= 0)
            {
                if (CNC_series == "----") //無法連線
                {
                    axes_unknown = true; 
                }
            }           
            
            show_selected_IP();
            checkHistory();
            if (axes_unknown)
            {
                Label1.Font.Size = System.Web.UI.WebControls.FontUnit.XLarge  ;
                Label1.Text = Label_ConnIP.Text + "------" + (eng ? "Disconncted now. Axes(names && number) unknown." : "目前無法連線,(軸名/軸數)無法判斷");
            }            
        }
        private void checkDropdownlist2Items()
        {
            if (DropDownList1.Items.Count > 0)
                Label_ConnIP.Text = DropDownList1.SelectedItem.Text;//.Value
            else
                Response.Redirect(@"~/Default.aspx");
            if (DropDownList3.SelectedIndex <= 0)
            {
                Get_CNC_series();
                if (CNC_series == "----") //無法連線
                {
                    NCA_Var.axes = 3;
                    NCA_Var.axes_name[0] = "1st";
                    NCA_Var.axes_name[1] = "2nd";
                    NCA_Var.axes_name[2] = "3rd";
                }
            }
            int old_index = DropDownList2.SelectedIndex;
            DropDownList2Bind();
            if (DropDownList2.Items.Count  >= old_index+1 )
                DropDownList2.SelectedIndex = old_index;
        }

        private void Get_CNC_series()
        {
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            NCA_Var.NC_IP = conn_IP[0];
            NCA_Var.read_axes_name();
            if (NCA_Var.NC_Handle != 0 && CNC_series != NCA_Var.cnc_serial)
                CNC_series = NCA_Var.cnc_serial;
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);
        }
        private void checkHistory()
        {            
            refresh_chart1();
            //先接手 refresh_chart1();處理 dtHistory 最後才畫圖
            refresh_LinkButton();
            //GridView2.DataSource = dtHistory; GridView2.DataBind();
        }
        private void refresh_chart1()
        {
            GridView2.DataSource = null; GridView2.DataBind();
            string imagefiles = @"~\Ut_Data\images\HistryImageFile";
            imagefiles = imagefiles + "_" + CNC_series + "_" + DropDownList2.SelectedItem.Text;
            Chart1.ImageLocation = imagefiles;
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            if (!Page.IsPostBack && eng)///從別的頁面切回來//
            {
                if (DropDownList3.SelectedIndex <= 0)
                {
                    Get_CNC_series(); 
                }
                int old_index = DropDownList2.SelectedIndex;
                DropDownList2Bind();
                if (DropDownList2.Items.Count >= old_index +1)
                    DropDownList2.SelectedIndex = old_index;             
            }
            if (DropDownList3.SelectedIndex <= 0)//DropDownList2.Visible
                Chart1.Titles[0].Text = Label_ConnIP.Text + "_" + DropDownList2.SelectedItem.Text +(eng?"_Loading%":"_負載%");
            else
                Chart1.Titles[0].Text = Label_ConnIP.Text + "_" + DropDownList2.SelectedItem.Text;
        }
        private void refresh_LinkButton()
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            //插入 因 eng 與 中文 當 Session["hisSeltItem"] 於存檔時不同 (例如: 軸負載<---> Loading)
            // item 要修正 Session["hisSeltItem"]要修改
            string temp = Session["hisSeltItem"].ToString();
            string item = temp;
            string item_eng = temp;
            switch (temp)
            {
                case "軸負載":                    
                case "Loading":
                    item = "軸負載";item_eng ="Loading";
                    break;
                case "工件數":
                case "PartsCount":
                    item = "工件數";item_eng ="PartsCount";
                    break;
            }
            Clear_LinkButtonColor();
            LinkButtonDay1.Visible = false; LinkButtonDay2.Visible = false; LinkButtonDay3.Visible = false;
            LinkButtonDay4.Visible = false; LinkButtonDay5.Visible = false; LinkButtonDay6.Visible = false;
            LinkButtonDay7.Visible = false; Button2.Visible = false; DropDownList4.Visible = false;           
            string str_Path = Server.MapPath("~/Ut_Data/Histry");
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            string targetPath = "";
            try
            {
                var files = Directory.GetDirectories(str_Path);
                for (int j = files.Length - 1; j >= 0; j--)
                {
                    if (files[j].ToString().Contains(conn_IP[0]))
                    {
                        targetPath = files[j].ToString();
                        break;
                    }
                }
            }
            catch { return; }
            if (targetPath == "") return;
            try
            {
                var targetfiles = Directory.GetFiles((targetPath));
                for (int j = targetfiles.Length - 1; j >= 0; j--)
                {
                    string filename = targetfiles[j].ToString().Substring(targetfiles[j].LastIndexOf(@"\") + 1);
                    string[] ary_filename = filename.Split('-');
                    if (ary_filename.Length < 5) continue;//長度5段
                    if (!targetPath.Contains(ary_filename[3])) continue;//序號要跟目錄同
                    if (!(ary_filename[4].Contains(item) || ary_filename[4].Contains(item_eng))) continue;//項目要相同
                    //開始比較
                    if (filename.Contains(DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        LinkButtonDay1.Visible = true; DropDownList4.Visible = true;
                        LinkButtonDay1.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    if (filename.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")))
                    {
                        LinkButtonDay2.Visible = true; DropDownList4.Visible = true;
                        LinkButtonDay2.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    }
                    if (filename.Contains(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")))
                    {
                        LinkButtonDay3.Visible = true; DropDownList4.Visible = true;
                        LinkButtonDay3.Text = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                    }
                    if (filename.Contains(DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd")))
                    {
                        LinkButtonDay4.Visible = true; DropDownList4.Visible = true;
                        LinkButtonDay4.Text = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
                    }
                    if (filename.Contains(DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd")))
                    {
                        LinkButtonDay5.Visible = true; DropDownList4.Visible = true;
                        LinkButtonDay5.Text = DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd");
                    }
                    if (filename.Contains(DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd")))
                    {
                        LinkButtonDay6.Visible = true; DropDownList4.Visible = true;
                        LinkButtonDay6.Text = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd");
                    }
                    if (filename.Contains(DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd")))
                    {
                        LinkButtonDay7.Visible = true; DropDownList4.Visible = true;
                        LinkButtonDay7.Text = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
                    }
                    bool lessthan7day = false;//預設少於7日的資料
                    lessthan7day = (filename.Contains(DateTime.Now.ToString("yyyy-MM-dd")) ||
                             filename.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")) ||
                             filename.Contains(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")) ||
                             filename.Contains(DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd")) ||
                             filename.Contains(DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd")) ||
                             filename.Contains(DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd")) ||
                             filename.Contains(DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd")));
                    if (clear_7)//是否按下清除7日前資料按紐
                    {
                        if (!lessthan7day)
                        { File.Delete(targetfiles[j]); lessthan7day = true; }
                    }
                    if (!lessthan7day) Button2.Visible = true;
                }
            }
            catch { return; }
            bool date_in7_selected = (LinkButtonDay1.Visible  ||
                            LinkButtonDay2.Visible ||
                            LinkButtonDay3.Visible ||
                            LinkButtonDay4.Visible ||
                            LinkButtonDay5.Visible ||
                            LinkButtonDay6.Visible ||
                            LinkButtonDay7.Visible);
            if (date_in7_selected) show_gridview2();
        }

        private void Clear_LinkButtonColor()
        {
            LinkButtonDay1.BackColor = System.Drawing.Color.Empty;
            LinkButtonDay2.BackColor = System.Drawing.Color.Empty;
            LinkButtonDay3.BackColor = System.Drawing.Color.Empty;
            LinkButtonDay4.BackColor = System.Drawing.Color.Empty;
            LinkButtonDay5.BackColor = System.Drawing.Color.Empty;
            LinkButtonDay6.BackColor = System.Drawing.Color.Empty;
            LinkButtonDay7.BackColor = System.Drawing.Color.Empty;
        }
        private void show_gridview2()
        {
            if (GridView2.DataSource != null)
            { GridView2.DataSource = null; GridView2.DataBind(); }
            //if (DropDownList3.SelectedIndex <= 0)//DropDownList2.Visible
            //{
                GridView2.Columns[1].HeaderText = "(";
                for (int j = 0; j < DropDownList2.Items.Count; j++)
                    GridView2.Columns[1].HeaderText += DropDownList2.Items[j].Text + (j == DropDownList2.Items.Count - 1 ? "" : "___");
                if (DropDownList3.SelectedIndex != 0) GridView2.Columns[1].HeaderText += "___" + "O_Num" + "___" + "Running?";
            GridView2.Columns[1].HeaderText += ")";
            //}
            //else
            //    GridView2.Columns[1].HeaderText = "Data";
            string date = Session["hisSeltDate"].ToString();
            //若都沒選日期且 LinkButtonDay1.visible 則修改日期
            bool date_in7_selected = (LinkButtonDay1.BackColor == System.Drawing.Color.Purple ||
                LinkButtonDay2.BackColor == System.Drawing.Color.Purple ||
                LinkButtonDay3.BackColor == System.Drawing.Color.Purple ||
                LinkButtonDay4.BackColor == System.Drawing.Color.Purple ||
                LinkButtonDay5.BackColor == System.Drawing.Color.Purple ||
                LinkButtonDay6.BackColor == System.Drawing.Color.Purple ||
                LinkButtonDay7.BackColor == System.Drawing.Color.Purple);
            if (!date_in7_selected && LinkButtonDay1.Visible) { date_in7_selected = true; Session["hisSeltDate"] = date = LinkButtonDay1.Text; LinkButtonDay1.BackColor = System.Drawing.Color.Purple; }
            if (!date_in7_selected && LinkButtonDay2.Visible) { date_in7_selected = true; Session["hisSeltDate"] = date = LinkButtonDay2.Text; LinkButtonDay2.BackColor = System.Drawing.Color.Purple; }
            if (!date_in7_selected && LinkButtonDay3.Visible) { date_in7_selected = true; Session["hisSeltDate"] = date = LinkButtonDay3.Text; LinkButtonDay3.BackColor = System.Drawing.Color.Purple; }
            if (!date_in7_selected && LinkButtonDay4.Visible) { date_in7_selected = true; Session["hisSeltDate"] = date = LinkButtonDay4.Text; LinkButtonDay4.BackColor = System.Drawing.Color.Purple; }
            if (!date_in7_selected && LinkButtonDay5.Visible) { date_in7_selected = true; Session["hisSeltDate"] = date = LinkButtonDay5.Text; LinkButtonDay5.BackColor = System.Drawing.Color.Purple; }
            if (!date_in7_selected && LinkButtonDay6.Visible) { date_in7_selected = true; Session["hisSeltDate"] = date = LinkButtonDay6.Text; LinkButtonDay6.BackColor = System.Drawing.Color.Purple; }
            if (!date_in7_selected && LinkButtonDay7.Visible) { date_in7_selected = true; Session["hisSeltDate"] = date = LinkButtonDay7.Text; LinkButtonDay7.BackColor = System.Drawing.Color.Purple; }            
            
            string str_Path = Server.MapPath("~/Ut_Data/Histry");
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            string targetPath = "";
            try
            {
                var files = Directory.GetDirectories(str_Path);
                for (int j = files.Length - 1; j >= 0; j--)
                {
                    if (files[j].ToString().Contains(conn_IP[0]))
                    {
                        targetPath = files[j].ToString();
                        break;
                    }
                }
            }
            catch { return; }
            if (targetPath == "") return;
            string[] dir_series = targetPath.Split('_');
            if (dir_series.Length < 1) return;
            
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            //插入 因 eng 與 中文 當 Session["hisSeltItem"] 於存檔時不同 (例如: 軸負載<---> Loading)
            // item 要修正 Session["hisSeltItem"]要修改
            string temp = DropDownList3.SelectedItem.Text;// Session["hisSeltItem"].ToString();
            string item = temp;
            string item_eng = temp;
            switch (temp)
            {
                case "軸負載":
                case "Loading":
                    item = "軸負載"; item_eng = "Loading";
                    break;
                case "工件數":
                case "PartsCount":
                    item = "工件數"; item_eng = "PartsCount";
                    break;
            }
            string targetFile_eng = targetPath + "\\" + date + "-" + dir_series[dir_series.Length - 1] + "-" + item_eng + ".txt";
            string targetFile = targetPath + "\\" + date + "-" + dir_series[dir_series.Length - 1] + "-" + item + ".txt";
            if (!(File.Exists(targetFile) || File.Exists(targetFile_eng)))
            {                
                //Label1.Text = (eng ? "Can't find" + targetFile_eng : "沒找到" + targetFile);
                Label1.Text = (eng ? "Can't find " + "\\" + date + "-" + dir_series[dir_series.Length - 1] + "-" + item_eng + ".txt"
                    : "沒找到" + "\\" + date + "-" + dir_series[dir_series.Length - 1] + "-" + item + ".txt");
                return;
            }
            //以下讀進檔案 拆開填入 dtHistory
            string readFile = eng ? targetFile : targetFile_eng;//先給相反
            if (File.Exists(targetFile_eng) && eng) readFile = targetFile_eng;//有存在才改回
            if (File.Exists(targetFile) && !eng) readFile = targetFile;//有存在才改回
            string FileContent = File.ReadAllText(readFile);
            FileContent = FileContent.Trim().Replace("\r\n", "\n");
            string[] str_Blok = FileContent.Split((char)0xa);
            try
            {
                foreach (string z in str_Blok)
                {
                    DataRow dr = dtHistory.NewRow();
                    dr[0] = z.Substring(0, 8);
                    dr[1] = (z.Substring(9));//.Replace("P", "_____");移到 Fill_chart1(int max)裡面作
                    dtHistory.Rows.Add(dr);
                }
            }
            catch { Label1.Text = date + "-" + dir_series[dir_series.Length - 1] + "-" + item + ".txt 的內容錯誤"; }
            GridView2.DataSource = dtHistory; GridView2.DataBind();
            //有資料就畫圖
            Fill_chart1(GridView2.Rows.Count);               
        }
        private void Fill_chart1(int max)
        { //要記得.Replace("P", "_____");
            int time =Convert.ToInt32( Session["hisSeltTime"].ToString());
            for (int k = 0; k < max; k++)
            {
                int rowK_time=-1;
                string[] str_ktime=GridView2.Rows[k].Cells[0].Text.Trim().Split('-');
                string[] str_kdata = GridView2.Rows[k].Cells[1].Text.Trim().Split('P');
                if (int.TryParse(str_ktime[0], out rowK_time) && rowK_time == time)
                {
                    //if (DropDownList3.SelectedIndex <= 0)//DropDownList2.Visible"_負載%";
                    //{
                        if(str_kdata.Length>=DropDownList2.Items.Count)
                        Chart1.Series[0].Points.AddXY(GridView2.Rows[k].Cells[0].Text, str_kdata[DropDownList2.SelectedIndex]);
                    //}
                    //else//_test1%
                    //    Chart1.Series[0].Points.AddXY(GridView2.Rows[k].Cells[0].Text, str_kdata[0]);
                }
                GridView2.Rows[k].Cells[1].Text = GridView2.Rows[k].Cells[1].Text.Replace("P", "_______");
                if (GridView2.Rows[k].Cells[1].Text.EndsWith("_______"))
                    GridView2.Rows[k].Cells[1].Text = GridView2.Rows[k].Cells[1].Text.TrimEnd('_')+"P";
            }
            
            if (Chart1.Series[0].Points.Count < 1)
            {               
                int t = Convert.ToInt32(Session["language"].ToString());
                bool eng = (t == 0);
                Label1.Text  =eng?"Please choose time interval 'with data'.": "請選擇有資料的時間區段";//Chart1.Titles[0].Text
                Chart1.Visible = false;
            }
        }


        private void DropDownList2Bind()
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            DropDownList2.Items.Clear();
            if (DropDownList3.SelectedIndex == 0)
            {
                DropDownList2.Items.Add(eng ? "Spindle" : "主軸");
                DropDownList2.Items[0].Value = eng ? "Spindle" : "主軸";
                for (int i = 0; i < NCA_Var.axes; i++)
                {
                    string tmp = NCA_Var.axes_name[i] + (eng ? "_Axis" : "軸");
                    DropDownList2.Items.Add(tmp);
                    DropDownList2.Items[i + 1].Value = tmp;
                }
            }
            if (DropDownList3.SelectedIndex == 1 || DropDownList3.SelectedIndex < 0 )
            {
                DropDownList2.Items.Add(eng ? "PartsCount" : "工件數");
                DropDownList2.Items[0].Value = eng ? "PartsCount" : "工件數";
                DropDownList2.Items.Add(eng ? "PartsReq" : "需工件數");
                DropDownList2.Items[1].Value = eng ? "PartsReq" : "需工件數";
                DropDownList2.Items.Add(eng ? "PartsTotal" : "總工件數");
                DropDownList2.Items[2].Value = eng ? "PartsTotal" : "總工件數";
            }
        }
        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {            
            GridView1.EditIndex = -1;            
            //show_Select_Table(); 
            refresh_chart1(); show_gridview2();//checkHistory();跳過refresh_LinkButton()的動作            
        }
        string targetPath = "";
        private void Get_NC_IP()
        {
            string[] conn_IP;
            conn_IP = Label_ConnIP.Text.Split('-');
            NCA_Var.NC_IP = conn_IP[0];
        }
        private string Get_targetPath()//取 CNC_series,targetPath ,NCA_Var.NC_IP
        {
            Get_NC_IP();
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
            NCA_Var.read_CNC_Type();
            if (NCA_Var.NC_Handle != 0 && CNC_series != NCA_Var.cnc_serial)
                CNC_series = NCA_Var.cnc_serial;
            NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
            string str_Path = Server.MapPath("~/Ut_Data/Histry");
            string reserveDir = Label_ConnIP.Text + "_" + CNC_series;
            string targetPath;
            if (CNC_series == "----") targetPath = str_Path + "\\" + NCA_Var.NC_IP;
            else
                targetPath = str_Path + "\\" + reserveDir;
            return targetPath;
        }
        private void show_selected_IP()//先處理Histry目錄
        {
            //先取得CNC_series
            //////////////////////////////////////////////////讀取資料區
            string Histry_Path = Server.MapPath("~/Ut_Data/Histry");
            targetPath = Get_targetPath();//取 CNC_series,targetPath ,NCA_Var.NC_IP
               
            
            bool targetPath_error = false;
            bool no_series = false;
            try//這節跟ProgCenter ProgBackup同
            {
                var files = Directory.GetDirectories(Histry_Path);
                if (CNC_series != "----") //合併同序號的目錄
                {
                    Response.Cookies["Last_IP"].Value = DropDownList1.SelectedItem.Value; //再改存放的值 (重點在中文有無亂碼)
                    Response.Cookies["Last_IP"].Expires = DateTime.Now.AddYears(10);
                    //勁勛案後修改
                    //bool find_cnc_serial = false;
                    //for (int j = files.Length - 1; j >= 0; j--)
                    //{
                    //    if (files[j].ToString() == targetPath) continue;
                    //    if (files[j].ToString().Contains(CNC_series))
                    //        find_cnc_serial = true;
                    //}
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
                    if (!Directory.Exists(targetPath))//確保沒漏勾 有序號一定建目錄
                    {
                        try
                        {
                            Directory.CreateDirectory(targetPath);
                        }
                        catch
                        { targetPath_error = true; }
                    }
                }
                else//沒序號(斷線/無法連線的) 要如何???先看看有沒有目錄
                {
                    for (int j = files.Length - 1; j >= 0; j--)
                    {
                        if (NCA_Var.NC_IP != "" && files[j].ToString().Contains(NCA_Var.NC_IP))
                            no_series = true;
                    }
                }
            }
            catch { };
            show_Select_Table();//再處理HistryControl.xls檔案做成Table
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);

            if (no_series)//沒序號有目錄的處理,----沒序號也沒目錄的不用處理(無法連線,又沒啟用過)
            {
                bool no_record = false;
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    if (GridView1.Rows[i].Cells[2].Text.Contains(NCA_Var.NC_IP)
                        && (GridView1.Rows[i].Cells[4].Text).ToUpper() == "TRUE")
                        no_record = true;
                }
                if (no_record)
                    Label1.Text = Label_ConnIP.Text +"------"+(eng?"Disconncted now.": "目前網路斷線");
            }
            else
                if (CNC_series == "----")
                    Label1.Text = Label_ConnIP.Text + "------"+(eng?"Disconncted now.":"目前無法連線");
            if (targetPath_error)
            {
                Label1.Font.Size = System.Web.UI.WebControls.FontUnit.Larger;
                Label1.Text = targetPath + "------" + (eng ? "Directory creat error,check illegal char" : "目錄創建失敗,請檢查是否目錄名稱不合規則");
            }
            else
                Label1.Font.Size = System.Web.UI.WebControls.FontUnit.XLarge;

        }
        string HistryControl = "HistryControl.xls";
        private void show_Select_Table()//再處理HistryControl.xls檔案做成Table
        {
            GridView1.Columns[1].Visible = true;
            GridView1.DataSource = null; GridView1.DataBind();

            HistryControl = Server.MapPath("~/Ut_Data/Histry/HistryControl.xls");
            dtRecord.Rows.Clear();
            int t_ip = Convert.ToInt32(Session["language"].ToString());
            bool eng_ip = (t_ip == 0);
            if (!File.Exists(HistryControl))
            {
                Label1.Text = eng_ip ? "------Please press Add new item->EDIT->Update Recording?" : "------記錄控制表不存在,請新增記錄項目後按_編輯->更新_是否記錄?";
            }
            else//有表就要載入  然後才能工作
            {
                Label1.Text = eng_ip ? "What do you need?Use control table to record it.":"您需要什麼資料項目?請使用下面記錄控制表來做控制";
            }
            bool IP_Change = dtRecord_init();
            GridView1.DataSource = dtRecord; GridView1.DataBind();
            if (IP_Change)
            {
                dtRecord_save(); 
            }
            GridView1.Columns[1].Visible = false;
        }
        private bool dtRecord_init()//進入前都要檢查 HistryControl有就??沒有就???
        {
            int t_init = Convert.ToInt32(Session["language"].ToString());
            bool eng_init = (t_init == 0);
            bool IP_Change = false;
            try
            {
                if (File.Exists(HistryControl))
                {//直接讀進表  Grideview1已經 GridView1.DataSource = null; GridView1.DataBind();  
                    DataTable dtExcel = make_dt(new DataTable());//差這行就可將RenderExcelToDatatable放在 NCA_Var
                    dtRecord = NCA_Var.RenderExcelToDataTable(HistryControl, dtExcel);
                    //以下該檢查 IP 是否變了 ,可否連線  若有不同的都要修正
                    if (dtRecord.Rows.Count < 1) return IP_Change;
                    string Histry_Path = Server.MapPath("~/Ut_Data/Histry");
                    var files = Directory.GetDirectories(Histry_Path);
                    if (files.Length < 1) return IP_Change;
                    bool sameIP_differSeries = false;
                    string differSeries = "";
                    bool Dir_sameIP = false;
                    int Hosts_num = Convert.ToInt32(Application["Hosts_num"]);
                    ArrayList bad_ip_list = new ArrayList();
                    for (int j = files.Length - 1; j >= 0; j--)
                    {
                        string tmp = files[j].ToString();
                        tmp = tmp.Substring(tmp.LastIndexOf(@"\") + 1);
                        string[] conn_IP;
                        conn_IP = tmp.Split('_');
                        if (conn_IP.Length < 1) continue;
                        Dir_sameIP = (Label_ConnIP.Text.Contains(conn_IP[0].Split('-')[0]));//勁勛案後修改 Label_ConnIP.Text == conn_IP[0]==>Label_ConnIP.Text.Contains(conn_IP[0].Split('-')[0])

                        //勁勛案後修改
                        //if (!sameIP_differSeries && Dir_sameIP && CNC_series != "----")//只處裡一次 序號不同 IP 相同 的
                        //{
                        bool rec_SameSeries = (conn_IP[conn_IP.Length - 1] == CNC_series);//dtRecord.Rows[i][1].ToString()==>conn_IP[conn_IP.Length-1]
                            //bool rec_SampIP = (dtRecord.Rows[i][2].ToString() == conn_IP[0]);
                            if (!Dir_sameIP) continue;//不相同 IP 不做處裡 //!rec_SampIP==>! Dir_sameIP
                            //if (rec_SameSeries) continue;//相同序號不做處裡
                            //if (dtRecord.Rows[i][0].ToString() == "連線正常" || dtRecord.Rows[i][0].ToString() == "Connected")
                            if (!rec_SameSeries || Label_ConnIP.Text != conn_IP[0]) //勁勛案後修改  ==> || Label_ConnIP.Text != conn_IP[0]
                            {
                                sameIP_differSeries = true;
                                //if (eng_init)
                                    differSeries = tmp;// dtRecord.Rows[i][2].ToString() + "_Row " + (i + 1).ToString();
                                //else
                                //    differSeries = dtRecord.Rows[i][2].ToString() + "_第" + (i + 1).ToString() + "列記錄";
                            }
                        //}
                        for (int i = 0; i < dtRecord.Rows.Count; i++)
                        {
                            //勁勛案後修改 序號同 而IP 不同時要改IP===>
                            //if (dtRecord.Rows[i][1].ToString() == conn_IP[1] && dtRecord.Rows[i][2].ToString() != conn_IP[0])
                            //{
                            //    IP_Change = true; dtRecord.Rows[i][2] = conn_IP[0];//序號同 而IP 不同時要改IP
                            //}
                            if (Hosts_num <= 0) Hosts_num = 0;//不需回返 
                            //改好IP 檢查可否連線 
                            bool connectable = true;
                            string[] str_connectable = dtRecord.Rows[i][2].ToString().Split('-');
                            
                            bool find_in_BadIpList = false;                           
                            //先找 bad_ip_list
                            for (int badiplistidx = 0; badiplistidx < bad_ip_list.Count; badiplistidx++)
                            {
                                if (str_connectable[0]==bad_ip_list[badiplistidx].ToString())
                                { find_in_BadIpList = true; connectable = false; break; }
                            }
                            
                            //再做一次 Ping測試
                            if (connectable)
                                try
                                {
                                    if (str_connectable[0].Trim() != "")
                                        connectable = NCA_Var.Ping(str_connectable[0]);
                                    else
                                        connectable = false;
                                }
                                catch { connectable = false; }

                            if (!connectable) bad_ip_list.Add(str_connectable[0]);
                            
                            
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
                            if (eng_init)
                            {
                                //連線狀況不同則修改
                                dtRecord.Rows[i][0] = (connectable ? "Connected"  :  "Disconnect" );
                                //序號(1) 空白的 要改成啟動記錄=false
                                if (dtRecord.Rows[i][1].ToString().Trim() == "")
                                    dtRecord.Rows[i][4] = false;
                                //軸負載的英文處理
                                if (dtRecord.Rows[i][3].ToString().Trim() == "軸負載") dtRecord.Rows[i][3] = "Loading";
                                //工件數的英文處理
                                if (dtRecord.Rows[i][3].ToString().Trim() == "工件數") dtRecord.Rows[i][3] = "PartsCount";
                            }
                            else
                            {
                                //連線狀況不同則修改
                                dtRecord.Rows[i][0] = (connectable ? "連線正常" :  "無法連線");
                                //序號(1) 空白的 要改成啟動記錄=false
                                if (dtRecord.Rows[i][1].ToString().Trim() == "")
                                    dtRecord.Rows[i][4] = false;
                                //軸負載的中文處理
                                if (dtRecord.Rows[i][3].ToString().Trim() == "Loading") dtRecord.Rows[i][3] = "軸負載";
                                //工件數的中文處理
                                if (dtRecord.Rows[i][3].ToString().Trim() == "PartsCount") dtRecord.Rows[i][3] = "工件數";
                            }
                            
                            
                        }
                    }
                    if (sameIP_differSeries)
                    {
                        if(eng_init)
                            Label1.Text = "Different CNC Serials :" + differSeries + ",Please Del it";
                        else
                            Label1.Text = "有不同(序號)的機台 :" + differSeries + ",請刪除該記錄";
                    }
                }
            }
            catch { Label1.Text =eng_init?"read control table error": "讀取記錄控制表時遇到意外"; }
            return IP_Change;
        }

        private void dtRecord_save()//進入前都要檢查 HistryControl 有就??沒有就???
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            //不管中英文都轉存英文
            {
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    if (GridView1.Rows[i].Cells[0].Text == "Connected" || GridView1.Rows[i].Cells[0].Text == "連線正常")
                        GridView1.Rows[i].Cells[0].Text = eng ? "Connected" : "連線正常";
                    else
                        GridView1.Rows[i].Cells[0].Text = eng ? "Disconnect" : "無法連線";
                    if (GridView1.Rows[i].Cells[3].Text == "Loading" || GridView1.Rows[i].Cells[3].Text == "軸負載")
                        GridView1.Rows[i].Cells[3].Text = eng ? "Loading" : "軸負載";
                    if (GridView1.Rows[i].Cells[3].Text == "PartsCount" || GridView1.Rows[i].Cells[3].Text == "工件數")
                        GridView1.Rows[i].Cells[3].Text = eng ? "PartsCount" : "工件數";
                }
            }
            RenderGridViewToExcel(GridView1, HistryControl);
            //如果 eng 存檔後 GV1 要轉回英文
            if (eng)//轉回英文
            {
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    if (GridView1.Rows[i].Cells[0].Text == "Connected" || GridView1.Rows[i].Cells[0].Text == "連線正常")
                        GridView1.Rows[i].Cells[0].Text = "Connected";
                    else
                        GridView1.Rows[i].Cells[0].Text = "Disconnect";
                    if (GridView1.Rows[i].Cells[3].Text == "軸負載")
                        GridView1.Rows[i].Cells[3].Text = "Loading";
                    if (GridView1.Rows[i].Cells[3].Text == "工件數")
                        GridView1.Rows[i].Cells[3].Text = "PartsCount";
                }
            }
        }
        protected void RenderGridViewToExcel(GridView dgv, string FileName)
        {
            DataTable dt = new DataTable();
            for (int count = 0; count < dgv.Columns.Count - 2; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].HeaderText);
                dt.Columns.Add(dc);
            }
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count - 2; countsub++)
                {
                    dr[countsub] = dgv.Rows[count].Cells[countsub].Text;
                }
                dt.Rows.Add(dr);
            }
            NCA_Var.RenderDataTableToExcel(dt, FileName);
        }
        //新增列
        protected void Button1_Click(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            string tmp = DropDownList3.SelectedValue;           
            if (DropDownList1.SelectedIndex < 0) return;
            //首先排除非連線中的IP 否則無法取得序號
            targetPath = Get_targetPath();//取 CNC_series,targetPath ,NCA_Var.NC_IP
            if (CNC_series == "----")
            {
                if(eng)
                    Label1.Text = "Check whether " + Label_ConnIP.Text + " is Connected";
                else
                    Label1.Text = @"請先確定 " + Label_ConnIP.Text + @" 是否連線正常";
                return;
            }
            //確定沒有重覆項目時才加上去
            bool repeatItem = false;
            for (int j = 0; j < GridView1.Rows.Count; j++)
            {
                string conn_ip = GridView1.Rows[j].Cells[2].Text;
                string item = GridView1.Rows[j].Cells[3].Text;
                if (conn_ip == Label_ConnIP.Text && tmp == item)
                {
                    repeatItem = true; break;
                }
            }
            if (!repeatItem)
            {
                GridView1.EditIndex = -1;
                HistryControl = Server.MapPath("~/Ut_Data/Histry/HistryControl.xls");
                GridView1.Columns[1].Visible = true;
                GridView1.DataSource = null; GridView1.DataBind();
                dtRecord.Rows.Clear();
                dtRecord_init();
                DataRow dr = dtRecord.NewRow();
                dr["ConnStatus"] =eng?"Connected":"連線正常";
                dr["ConnSeries"] = CNC_series;
                dr["ConnIP"] = Label_ConnIP.Text;
                dr["RecordItem"] = tmp;
                dr["Recording"] = "False";
                dtRecord.Rows.Add(dr);
                GridView1.DataSource = dtRecord; GridView1.DataBind();
                //最後存檔,不管原先有沒有檔案
                dtRecord_save();
                GridView1.Columns[1].Visible = false;

            }
            else
            {
                Label1.Text =eng?"This item already exist": @"已有相同的記錄項目了";
            }

        }
        //刪除列
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);           
            bool DEMO_Mode = Convert.ToBoolean(Application["DEMO_Mode"].ToString());
            if (DEMO_Mode)
            {
                GridView1.EditIndex = -1;
                show_selected_IP();
                Label1.Text = eng ? "DEMO_Mode" : "展示模式";
                return;
            }
            string tmp_Label1 = Label1.Text;
            if (GridView1.Rows.Count <= 1)
            {
                if (GridView1.Rows.Count > 0)
                {
                    Label1.Text =eng?"no history data !!!": "已無最後一筆資料";
                    HistryControl = Server.MapPath("~/Ut_Data/Histry/HistryControl.xls");
                    File.Delete(HistryControl);
                    GridView1.DataSource = null; GridView1.DataBind();
                }
                return;
            }
            int delrow = e.RowIndex;
            targetPath = Get_targetPath();

            GridView1.Columns[1].Visible = true;//設為null 前要顯示Series欄位
            GridView1.EditIndex = -1;
            GridView1.DataSource = null; GridView1.DataBind();

            HistryControl = Server.MapPath("~/Ut_Data/Histry/HistryControl.xls");
            dtRecord.Rows.Clear();

            dtRecord_init();//讀入表格
            if (dtRecord.Rows.Count > delrow)
                dtRecord.Rows[delrow].Delete();
            GridView1.DataSource = dtRecord; GridView1.DataBind();
            //最後存檔,不管原先有沒有檔案
            dtRecord_save();
            //再讀一次表格            
            GridView1.DataSource = null; GridView1.DataBind();
            dtRecord.Rows.Clear();
            dtRecord_init();//讀入表格
            GridView1.DataSource = dtRecord; GridView1.DataBind();
            if (Label1.Text == tmp_Label1)
                Label1.Text =eng ? "What do you need?Use control table to record it.": "您需要什麼資料項目?請使用下面記錄控制表來做控制";
            
            GridView1.Columns[1].Visible = false;//Save之後再作隱藏Series欄位
            if (GridView1.Rows.Count <= 1) Label1.Text = eng ?"last one":"剩最後一筆資料";
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);   
            bool DEMO_Mode = Convert.ToBoolean(Application["DEMO_Mode"].ToString());
            if (DEMO_Mode)
            {
                GridView1.EditIndex = -1;
                show_selected_IP();
                Label1.Text = eng ? "DEMO_Mode" : "展示模式";
                return;
            }
            GridView1.EditIndex = e.NewEditIndex;
            show_selected_IP();
            if (GridView1.Rows.Count - 1 >= GridView1.EditIndex)
                GridView1.Rows[e.NewEditIndex].Cells[4].Focus();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            HistryControl = Server.MapPath("~/Ut_Data/Histry/HistryControl.xls");
            GridView1.Columns[1].Visible = true;
            if (((TextBox)GridView1.Rows[e.RowIndex].Cells[4].Controls[0]).Text == "")
            {
                ((TextBox)GridView1.Rows[e.RowIndex].Cells[4].Controls[0]).Text = "False";
            }
            DataTable dt = new DataTable();
            dt = make_dt(dt);
            for (int count = 0; count < GridView1.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < GridView1.Columns.Count - 3; countsub++)
                {
                    dr[countsub] = GridView1.Rows[count].Cells[countsub].Text;
                }
                if (count == e.RowIndex)
                    dr[4] = ((((TextBox)GridView1.Rows[count].Cells[4].Controls[0]).Text).ToUpper() == "TRUE" ? true : false);
                else
                    dr[4] = ((GridView1.Rows[count].Cells[4].Text).ToUpper() == "TRUE" ? true : false);
                dt.Rows.Add(dr);
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string status = dt.Rows[i][0].ToString();
                string item = dt.Rows[i][3].ToString();
                if (dt.Rows[i][0].ToString() == "Connected" || dt.Rows[i][0].ToString() == "連線正常")
                    dt.Rows[i][0] =eng?"Connected": "連線正常";
                else
                    dt.Rows[i][0] = eng ? "Disconnect" : "無法連線";
                if (dt.Rows[i][3].ToString() == "Loading" || dt.Rows[i][3].ToString() == "軸負載")
                    dt.Rows[i][3] = eng ? "Loading" : "軸負載";
                if (dt.Rows[i][3].ToString() == "PartsCount" || dt.Rows[i][3].ToString() == "工件數")
                    dt.Rows[i][3] = eng ? "PartsCount" : "工件數";
                status = ""; item = "";
            }
            NCA_Var.RenderDataTableToExcel(dt, HistryControl);
            GridView1.Columns[1].Visible = false;
            GridView1.EditIndex = -1;
            show_selected_IP();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            show_selected_IP();
        }
        //連線正常 用 黃綠色  其他用淺灰色
        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                if (GridView1.Rows[i].Cells[0].Text == "連線正常" || GridView1.Rows[i].Cells[0].Text == "Connected")
                {
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.GreenYellow;
                }
                else
                {
                    GridView1.Rows[i].Cells[0].BackColor = System.Drawing.Color.LightGray;
                }
                if (GridView1.Rows[i].Cells[4].Text.ToUpper() == "TRUE")
                {
                    GridView1.Rows[i].Cells[4].BackColor = System.Drawing.Color.GreenYellow;
                    GridView1.Rows[i].Cells[3].BackColor = System.Drawing.Color.GreenYellow;
                }
                else
                {
                    GridView1.Rows[i].Cells[4].BackColor = System.Drawing.Color.LightGray;
                    GridView1.Rows[i].Cells[3].BackColor = System.Drawing.Color.Empty;
                }
                if (GridView1.Rows[i].Cells[0].BackColor == System.Drawing.Color.GreenYellow &&
                    GridView1.Rows[i].Cells[4].BackColor == System.Drawing.Color.GreenYellow)
                    GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.GreenYellow;
                else
                    GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.Empty;
                //連線IP(2) 記錄項目(3)  若有空白 用紅色做警告
                if (GridView1.Rows[i].Cells[2].Text.Trim() == "")
                    GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.Red;
                if (GridView1.Rows[i].Cells[3].Text.Trim() == "")
                    GridView1.Rows[i].Cells[3].BackColor = System.Drawing.Color.Red;
                if (GridView1.Rows[i].Cells[2].BackColor == System.Drawing.Color.GreenYellow)
                {
                    string[] tmpIP = GridView1.Rows[i].Cells[2].Text.Trim().ToString().Split('-');
                    NCA_Var.NC_IP = tmpIP[0];
                    NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
                    NCA_Var.read_CNC_Type();
                    NCA_Var.NC_Handle = NCA_Var.CNC_Release_Handle(NCA_Var.NC_Handle);//在此釋放 NCA_Var.NC_Handle
                    if (NCA_Var.Focas_ret != 0 || GridView1.Rows[i].Cells[1].Text.Trim().ToString() != NCA_Var.cnc_serial)
                    {
                        GridView1.Rows[i].Cells[2].BackColor = System.Drawing.Color.Red;
                    }
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

        //Item 選擇
        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            //Correct_Session_hisSeltItem(eng);//因存檔時 用中文
            Session["hisSeltItem"] = DropDownList3.SelectedItem.Text;
            bool axes_unknown = false;
            if (DropDownList3.SelectedIndex < 0)//
            {
                Get_CNC_series();
                if (CNC_series == "----") //無法連線
                {
                    axes_unknown = true; ;
                    NCA_Var.axes = 3;
                    NCA_Var.axes_name[0] = "1st";
                    NCA_Var.axes_name[1] = "2nd";
                    NCA_Var.axes_name[2] = "3rd";
                }
            }
            int old_index = DropDownList2.SelectedIndex;
            DropDownList2Bind();
            if (DropDownList2.Items.Count >= old_index +1)
                DropDownList2.SelectedIndex = old_index;
            bool not_started = false;
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                if (GridView1.Rows[i].Cells[2].Text == Label_ConnIP.Text &&
                    GridView1.Rows[i].Cells[3].Text == DropDownList3.SelectedValue &&
                    GridView1.Rows[i].Cells[4].Text.ToUpper() != "TRUE")
                    not_started = true;
            }
            if (not_started) Label1.Text = Label_ConnIP.Text + "---" + DropDownList3.SelectedValue + "------"+(eng?"Recording?":"尚未啟動記錄");
            else//有表就要載入  然後才能工作
            {
                Label1.Text = eng ? "What do you need?Use control table to record it." : "您需要什麼資料項目?請使用下面記錄控制表來做控制";
            }
            checkHistory();
            if (axes_unknown)
            {
                Label1.Font.Size = System.Web.UI.WebControls.FontUnit.XLarge;
                Label1.Text = Label_ConnIP.Text + "------" + (eng ? "Disconncted now. Axes(names && number) unknown." : "目前無法連線,(軸名/軸數)無法判斷");
            }            
        }
        
       
        //時間0~23 選擇
        protected void DropDownList4_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["hisSeltTime"] = DropDownList4.SelectedValue.ToString();
            refresh_chart1();            
            show_gridview2();
        }
        //以下都是日期選擇
        protected void LinkButtonDay7_Click(object sender, EventArgs e)
        {
            Clear_LinkButtonColor(); LinkButtonDay7.BackColor = System.Drawing.Color.Purple;
            Session["hisSeltDate"] = LinkButtonDay7.Text;
            refresh_chart1(); show_gridview2();
        }
        protected void LinkButtonDay6_Click(object sender, EventArgs e)
        {
            Clear_LinkButtonColor(); LinkButtonDay6.BackColor = System.Drawing.Color.Purple;
            Session["hisSeltDate"] = LinkButtonDay6.Text;
            refresh_chart1(); show_gridview2();
        }
        protected void LinkButtonDay5_Click(object sender, EventArgs e)
        {
            Clear_LinkButtonColor(); LinkButtonDay5.BackColor = System.Drawing.Color.Purple;
            Session["hisSeltDate"] = LinkButtonDay5.Text;
            refresh_chart1(); show_gridview2();
        }
        protected void LinkButtonDay4_Click(object sender, EventArgs e)
        {
            Clear_LinkButtonColor(); LinkButtonDay4.BackColor = System.Drawing.Color.Purple;
            Session["hisSeltDate"] = LinkButtonDay4.Text;
            refresh_chart1(); show_gridview2();
        }
        protected void LinkButtonDay3_Click(object sender, EventArgs e)
        {
            Clear_LinkButtonColor(); LinkButtonDay3.BackColor = System.Drawing.Color.Purple;
            Session["hisSeltDate"] = LinkButtonDay3.Text;
            refresh_chart1(); show_gridview2();
        }
        protected void LinkButtonDay2_Click(object sender, EventArgs e)
        {
            Clear_LinkButtonColor(); LinkButtonDay2.BackColor = System.Drawing.Color.Purple;
            Session["hisSeltDate"] = LinkButtonDay2.Text;
            refresh_chart1(); show_gridview2();
        }
        protected void LinkButtonDay1_Click(object sender, EventArgs e)
        {
            Clear_LinkButtonColor(); LinkButtonDay1.BackColor = System.Drawing.Color.Purple;
            Session["hisSeltDate"] = LinkButtonDay1.Text;
            refresh_chart1(); show_gridview2();
        }
        bool clear_7 = false;
        //清掉7天前記錄
        protected void Button2_Click(object sender, EventArgs e)
        {
            clear_7 = true;
            refresh_chart1(); refresh_LinkButton();
        }
        bool HideControl = true ;
        //隱藏控制表
        protected void Button3_Click(object sender, EventArgs e)
        {
            Get_ShowControl();
            HideControl = !HideControl;
            Show_Button3();
            checkHistory(); 
        }
        private void Show_Button3()
        {
            if (HideControl)
            {
                Button3.BackColor = System.Drawing.Color.GreenYellow;
                GridView1.Visible = false;
            }
            else
            {
                Button3.BackColor = System.Drawing.Color.Empty;
                GridView1.Visible = true ;
            }
        }
        private void Get_ShowControl()
        {
            if (Button3.BackColor == System.Drawing.Color.GreenYellow)
                HideControl = true;
            else
                HideControl = false;
        }



    }
}