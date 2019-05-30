using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.IO;


namespace Utilization
{
    public partial class admin : System.Web.UI.Page
    {
        string CNC_series = "----";
        string AutomaticControl = "AutomaticControl.xls";
        DataTable dtRecord = new DataTable();
        private DataTable make_dt(DataTable dtRecord)
        {
            dtRecord.Columns.Add("ConnStatus", Type.GetType("System.String"));
            dtRecord.Columns.Add("ConnSeries", Type.GetType("System.String"));
            dtRecord.Columns.Add("ConnIP", Type.GetType("System.String"));
            dtRecord.Columns.Add("CNC", Type.GetType("System.String"));//Picture
            dtRecord.Columns.Add("Monitoring", Type.GetType("System.Boolean"));
            return dtRecord;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["u_name"].ToString() != "Admin")
            {
                Response.Write("No access right to Site Management<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            bool green = false;
            green = Get_DEMO_Mode();
            Show_DEMO_Mode(green);
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Site Management";
                RadioButtonList1.Items[0].Text = "Auto clear history data(7 days ago)";
                RadioButtonList1.Items[1].Text = "Manual clear history data(7 days ago)";
            }
            else
                Button2.Text = "展示模式 (保護 [此網站] 請打開展示模式--->綠色)";
            if (!Page.IsPostBack)
            {
                if(Convert.ToBoolean(Session["clear_7"])) RadioButtonList1.SelectedIndex = 0;
                if (t == 0)
                {
                    TextBox0.ToolTip = "Input 1~4"; TextBox1.ToolTip = "Input Plant_I name,Empty space is not allowed";
                    TextBox2.ToolTip = "Input Plant_II name"; TextBox3.ToolTip = "Input Plant_III name";
                    TextBox4.ToolTip = "Input Plant_IV name"; TextBox5.ToolTip = "Plant Surname";
                    TextBox6.ToolTip = "Plant_I link address,IP only,use no http"; TextBox7.ToolTip = "Plant_II link address,IP only,use no http";
                    TextBox8.ToolTip = "Plant_III link address,IP only,use no http"; TextBox9.ToolTip = "Plant_IV link address,IP only,use no http";
                    TextBox10.ToolTip = "Plant_I LOGO"; TextBox11.ToolTip = "Plant_II LOGO";
                    TextBox12.ToolTip = "Plant_III LOGO"; TextBox13.ToolTip = "Plant_IV LOGO";
                    TextBox14.ToolTip = "Page end company name";
                    TextBox15.ToolTip = "Page end company name address(http://)";
                    TextBox16.ToolTip = "Stock address(http://)";
                    TextBox17.ToolTip = "Raws Input/Output address(http://)";
                    Table1.Rows[0].Cells[0].Text = "Default Plant";
                    Table1.Rows[0].Cells[1].Text = "Plant_IV name"; Table1.Rows[0].Cells[2].Text = "Plant_III name";
                    Table1.Rows[0].Cells[3].Text = "Plant_II name"; Table1.Rows[0].Cells[4].Text = "Plant_I name";
                    Table1.Rows[2].Cells[0].Text = "Plant link address";
                    Table1.Rows[3].Cells[0].Text = "LOGO";
                    Table1.Rows[4].Cells[0].Text = "LOGO filename";
                    Table1.Rows[5].Cells[0].Text = "Plant Surname";
                    Table1.Rows[5].Cells[3].Text = "Each text Enter data first,"; Table1.Rows[5].Cells[4].Text = "then press SAVE";
                    Table2.Rows[0].Cells[0].Text = "Stock address(http://)";
                    Table2.Rows[1].Cells[0].Text = "Raws Input/Output address(http://)";
                }

                TextBox0.Text = Session["area"].ToString();

                int Areas_Name_Length = Convert.ToInt32(Session["Areas_Name.Length"].ToString());
                TextBox1.Text = Session["Areas_Name0"].ToString();
                if (Areas_Name_Length > 1) TextBox2.Text = Session["Areas_Name1"].ToString();
                if (Areas_Name_Length > 2) TextBox3.Text = Session["Areas_Name2"].ToString();
                if (Areas_Name_Length > 3) TextBox4.Text = Session["Areas_Name3"].ToString();

                TextBox5.Text = Session["default_Title"].ToString();

                int TransHosts_Length = Convert.ToInt32(Session["TransHosts.Length"].ToString());
                TextBox6.Text = Session["TransHosts0"].ToString();
                if (TransHosts_Length > 1) TextBox7.Text = Session["TransHosts1"].ToString();
                if (TransHosts_Length > 2) TextBox8.Text = Session["TransHosts2"].ToString();
                if (TransHosts_Length > 3) TextBox9.Text = Session["TransHosts3"].ToString();

                int Areas_Pics_Length = Convert.ToInt32(Session["Areas_Pics.Length"].ToString());
                TextBox10.Text = Session["Areas_Pics0"].ToString();
                if (Areas_Pics_Length > 1) TextBox11.Text = Session["Areas_Pics1"].ToString();
                if (Areas_Pics_Length > 2) TextBox12.Text = Session["Areas_Pics2"].ToString();
                if (Areas_Pics_Length > 3) TextBox13.Text = Session["Areas_Pics3"].ToString();
                if (Areas_Pics_Length >= 1)
                    Image1.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics0"].ToString();
                else
                    Image1.ImageUrl = null;

                if (Areas_Pics_Length >= 2)
                    Image2.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics1"].ToString();
                else
                    Image2.ImageUrl = null;

                if (Areas_Pics_Length >= 3)
                    Image3.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics2"].ToString();
                else
                    Image3.ImageUrl = null;

                if (Areas_Pics_Length >= 4)
                    Image4.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics3"].ToString();
                else
                    Image4.ImageUrl = null;
                //圖片檔名位置
                string str_path = Server.MapPath("~/Ut_Data/images/");
                Label1.Text = @"圖片存檔位置[ " + str_path + @" ]";
                if (t == 0)
                    Label1.Text = @"LOGO files directory [ " + str_path + @" ]";
                Label2.Text = @"How to identify CNC ? [ " + str_path + @" ]";
                //自定義公司名稱
                TextBox14.Text = Application["company"].ToString();
                TextBox15.Text = Application["company_hyperlink"].ToString();
                if (!TextBox15.Text.StartsWith(@"http")) TextBox15.Text = @"http://" + TextBox15.Text;
                //庫存記錄/登記__連結網址
                if (Application["WareHost0"].ToString() != "")//null --->""
                {
                    TextBox16.Text = Application["WareHost0"].ToString();
                    if (!TextBox16.Text.StartsWith(@"http")) TextBox16.Text = @"http://" + TextBox16.Text;
                }
                //進出料記錄/登記__連結網址
                if (Application["InputOutputHost0"].ToString() != "")//null --->""
                {
                    TextBox17.Text = Application["InputOutputHost0"].ToString();
                    if (!TextBox17.Text.StartsWith(@"http")) TextBox17.Text = @"http://" + TextBox17.Text;
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            if (!Session.IsNewSession)///尚未退出瀏覽器
            {
                if (Page.IsPostBack)///從別的頁面切回來
                {
                    dtRecord = make_dt(dtRecord); 
                }
                else
                { dtRecord = make_dt(dtRecord); show_Select_Table(); }

            }
        }
        //SAVE
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (TextBox6.Text.StartsWith(@"http://")) TextBox6.Text = TextBox6.Text.Replace(@"http://", "");
            if (TextBox7.Text.StartsWith(@"http://")) TextBox7.Text = TextBox7.Text.Replace(@"http://", "");
            if (TextBox8.Text.StartsWith(@"http://")) TextBox8.Text = TextBox8.Text.Replace(@"http://", "");
            if (TextBox9.Text.StartsWith(@"http://")) TextBox9.Text = TextBox9.Text.Replace(@"http://", "");

            Label1.Text = "Check settings ......檢查設定更改中......";
            bool changed = false;
            bool[] changed_item = new bool[21];
            for (int i = 0; i < changed_item.Length; i++) changed_item[i] = false;

            if (TextBox0.Text != "" && TextBox0.Text != Session["area"].ToString())
            { Session["area"] = TextBox0.Text; changed_item[0] = changed = true; }

            int old_Areas_Name_Length = Convert.ToInt32(Session["Areas_Name.Length"]);
            Session["Areas_Name.Length"] = "1";
            if (TextBox2.Text != "") Session["Areas_Name.Length"] = "2";
            if (TextBox2.Text != "" && TextBox3.Text != "") Session["Areas_Name.Length"] = "3";
            if (TextBox2.Text != "" && TextBox3.Text != "" && TextBox4.Text != "") Session["Areas_Name.Length"] = "4";
            int Areas_Name_Length = Convert.ToInt32(Session["Areas_Name.Length"].ToString());
            if (old_Areas_Name_Length != Areas_Name_Length) changed_item[1] = changed = true;

            if (TextBox1.Text != "" && TextBox1.Text != Session["Areas_Name0"].ToString())
            { Session["Areas_Name0"] = TextBox1.Text; changed_item[2] = changed = true; }
            if (Areas_Name_Length > 1 && Session["Areas_Name1"] == null) Session["Areas_Name1"] = "";
            if (Areas_Name_Length > 1 && TextBox2.Text != Session["Areas_Name1"].ToString()) { Session["Areas_Name1"] = TextBox2.Text; changed_item[3] = changed = true; }
            if (Areas_Name_Length > 2 && Session["Areas_Name2"] == null) Session["Areas_Name2"] = "";
            if (Areas_Name_Length > 2 && TextBox3.Text != Session["Areas_Name2"].ToString()) { Session["Areas_Name2"] = TextBox3.Text; changed_item[4] = changed = true; }
            if (Areas_Name_Length > 3 && Session["Areas_Name3"] == null) Session["Areas_Name3"] = "";
            if (Areas_Name_Length > 3 && TextBox4.Text != Session["Areas_Name3"].ToString()) { Session["Areas_Name3"] = TextBox4.Text; changed_item[5] = changed = true; }

            if (TextBox5.Text != Session["default_Title"].ToString()) { Session["default_Title"] = TextBox5.Text; changed_item[6] = changed = true; }

            int old_TransHosts_Length = Convert.ToInt32(Session["TransHosts.Length"]);
            Session["TransHosts.Length"] = "1";
            if (TextBox7.Text != "") Session["TransHosts.Length"] = "2";
            if (TextBox7.Text != "" && TextBox8.Text != "") Session["TransHosts.Length"] = "3";
            if (TextBox7.Text != "" && TextBox8.Text != "" && TextBox9.Text != "") Session["TransHosts.Length"] = "4";
            int TransHosts_Length = Convert.ToInt32(Session["TransHosts.Length"].ToString());
            if (old_TransHosts_Length != TransHosts_Length) changed_item[7] = changed = true;

            if (TextBox6.Text != "" && TextBox6.Text != Session["TransHosts0"].ToString())
            { Session["TransHosts0"] = TextBox6.Text; changed_item[8] = changed = true; }
            if (TransHosts_Length > 1 && Session["TransHosts1"] == null) Session["TransHosts1"] = "";
            if (TransHosts_Length > 1 && TextBox7.Text != Session["TransHosts1"].ToString()) { Session["TransHosts1"] = TextBox7.Text; changed_item[9] = changed = true; }
            if (TransHosts_Length > 2 && Session["TransHosts2"] == null) Session["TransHosts2"] = "";
            if (TransHosts_Length > 2 && TextBox8.Text != Session["TransHosts2"].ToString()) { Session["TransHosts2"] = TextBox8.Text; changed_item[10] = changed = true; }
            if (TransHosts_Length > 3 && Session["TransHosts3"] == null) Session["TransHosts3"] = "";
            if (TransHosts_Length > 3 && TextBox9.Text != Session["TransHosts3"].ToString()) { Session["TransHosts3"] = TextBox9.Text; changed_item[11] = changed = true; }

            int old_Areas_Pics_Length = Convert.ToInt32(Session["Areas_Pics.Length"]);
            Session["Areas_Pics.Length"] = "1";
            if (TextBox11.Text != "") Session["Areas_Pics.Length"] = "2";
            if (TextBox11.Text != "" && TextBox12.Text != "") Session["Areas_Pics.Length"] = "3";
            if (TextBox11.Text != "" && TextBox12.Text != "" && TextBox13.Text != "") Session["Areas_Pics.Length"] = "4";
            int Areas_Pics_Length = Convert.ToInt32(Session["Areas_Pics.Length"].ToString());
            if (old_Areas_Pics_Length != Areas_Pics_Length) changed_item[12] = changed = true;

            if (TextBox10.Text != Session["Areas_Pics0"].ToString())
            { Session["Areas_Pics0"] = TextBox10.Text; changed_item[13] = changed = true; }
            if (Areas_Pics_Length > 1 && Session["Areas_Pics1"] == null) Session["Areas_Pics1"] = "";
            if (Areas_Pics_Length > 1 && TextBox11.Text != Session["Areas_Pics1"].ToString()) { Session["Areas_Pics1"] = TextBox11.Text; changed_item[14] = changed = true; }
            if (Areas_Pics_Length > 2 && Session["Areas_Pics2"] == null) Session["Areas_Pics2"] = "";
            if (Areas_Pics_Length > 2 && TextBox12.Text != Session["Areas_Pics2"].ToString()) { Session["Areas_Pics2"] = TextBox12.Text; changed_item[15] = changed = true; }
            if (Areas_Pics_Length > 3 && Session["Areas_Pics3"] == null) Session["Areas_Pics3"] = "";
            if (Areas_Pics_Length > 3 && TextBox13.Text != Session["Areas_Pics3"].ToString()) { Session["Areas_Pics3"] = TextBox13.Text; changed_item[16] = changed = true; }
            if (Areas_Pics_Length >= 1)
                Image1.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics0"].ToString();
            else
                Image1.ImageUrl = null;

            if (Areas_Pics_Length >= 2)
                Image2.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics1"].ToString();
            else
                Image2.ImageUrl = null;

            if (Areas_Pics_Length >= 3)
                Image3.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics2"].ToString();
            else
                Image3.ImageUrl = null;

            if (Areas_Pics_Length >= 4)
                Image4.ImageUrl = "~/Ut_data/images/" + Session["Areas_Pics3"].ToString();
            else
                Image4.ImageUrl = null;
            /////////////////////////////////////////////////////////////////////////////////////////////////
            bool Demo_Mode = false;
            Demo_Mode = Get_DEMO_Mode();
            //Application 的變數 不要在 Demo_Mode 改變
            {
                if (TextBox14.Text != Application["company"].ToString())
                { changed_item[17] = changed = true; }

                if (TextBox15.Text != Application["company_hyperlink"].ToString())
                { changed_item[18] = changed = true; }

                //庫存記錄/登記__連結網址
                if (TextBox16.Text != Application["WareHost0"].ToString())
                { changed_item[19] = changed = true; }
         
                //進出料記錄/登記__連結網址
                if (TextBox17.Text != Application["InputOutputHost0"].ToString())
                { changed_item[20] = changed = true; }
           
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            if (changed)
            {
                if (!Demo_Mode) Save_Changed(changed_item);
                Response.Redirect(@"~/BulletinBoard.aspx");               
            }
            else
            {
                Label1.Text =(eng?"Nothing changed.":"設定未更改");
                Literal mymsg = new Literal();
                mymsg.Text = "<Script>alert('" + (eng ? "Each text Press Enter to input data first,then hit SAVE button" : "輸入完畢,請按ENTER,再按SAVE按紐") + "')</Script>";
                Page.Controls.Add(mymsg);   
            }
        }
        private void Save_Changed(bool[] item)
        {
            string str_path = "";
            if (item[0])// Session["area"]
            {
                str_path = Server.MapPath("~/Ut_Data/Area_no.txt");
                File.WriteAllText(str_path, Session["area"].ToString());
            }
            if (item[1] | item[2] | item[3] | item[4] | item[5])// Session["Areas_Name.Length"],
            //Session["Areas_Name0"],Session["Areas_Name1"],Session["Areas_Name2"],Session["Areas_Name3"]
            {
                int Areas_Name_Length = Convert.ToInt32(Session["Areas_Name.Length"]);
                str_path = Server.MapPath("~/Ut_Data/Areas_name.xml");
                DataSet ds = new DataSet();
                if (!File.Exists(str_path))
                {
                    XDocument doc = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                     new XElement("Areas",
                                          new XElement("Name", "Plant_I"),
                                          new XElement("Name", "Plant_II")
                                    )
                                );
                    doc.Save(str_path);
                }
                try
                {
                    ds.ReadXmlSchema(str_path);
                    ds.ReadXml(str_path);
                }
                catch
                { }
                if(ds.Tables.Count>0)
                {
                    ds.Tables[0].Rows.Clear();
                    for (int i = 0; i < Areas_Name_Length; i++)
                    {                       
                        if (ds.Tables[0].Rows.Count < Areas_Name_Length)
                        {
                            DataRow dr = ds.Tables[0].NewRow();
                            ds.Tables[0].Rows.Add(dr);
                        }
                        ds.Tables[0].Rows[i][0]= Session["Areas_Name" + i].ToString();
                    }
                    FileStream fs = new FileStream(str_path, FileMode.Create);
                    System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(fs, System.Text.Encoding.UTF8);
                    xtw.WriteProcessingInstruction("xml", "version='1.0'");
                    ds.WriteXml(xtw);
                    xtw.Close();
                }
            }
            if (item[6])// Session["default_Title"]
            {
                str_path = Server.MapPath("~/Ut_Data/PageTitle.txt");
                File.WriteAllText(str_path, Session["default_Title"].ToString(), System.Text.Encoding.Default);
            }
            if (item[7] | item[8] | item[9] | item[10] | item[11])// Session["TransHosts.Length"],
            //Session["TransHosts0"],Session["TransHosts1"],Session["TransHosts2"],Session["TransHosts3"]
            {
                int TransHosts_Length = Convert.ToInt32(Session["TransHosts.Length"]);
                str_path = Server.MapPath("~/Ut_Data/TransAddress.xml");
                DataSet ds = new DataSet();
                if (!File.Exists(str_path))
                {
                    XDocument doc = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement("Hosts",
                                              new XElement("Host", Session["TransHosts0"].ToString())
                                        )
                                    );
                    doc.Save(str_path);
                }
                try
                {
                    ds.ReadXmlSchema(str_path);
                    ds.ReadXml(str_path);
                }
                catch
                { }
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].Rows.Clear();
                    for (int i = 0; i < TransHosts_Length; i++)
                    {
                        if (ds.Tables[0].Rows.Count < TransHosts_Length)
                        {
                            DataRow dr = ds.Tables[0].NewRow();
                            ds.Tables[0].Rows.Add(dr);
                        }
                        ds.Tables[0].Rows[i][0] = Session["TransHosts" + i].ToString();
                    }
                    FileStream fs = new FileStream(str_path, FileMode.Create);
                    System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(fs, System.Text.Encoding.UTF8);
                    xtw.WriteProcessingInstruction("xml", "version='1.0'");
                    ds.WriteXml(xtw);
                    xtw.Close();
                }
            }
            if (item[12] | item[13] | item[14] | item[15] | item[16])// Session["Areas_Pics.Length"],
            // Session["Areas_Pics0"]Session["Areas_Pics1"],Session["Areas_Pics2"],Session["Areas_Pics3"]
            {
                int Areas_Pics_Length = Convert.ToInt32(Session["Areas_Pics.Length"]);
                str_path = Server.MapPath("~/Ut_Data/Areas_Pics.xml");
                DataSet ds = new DataSet();
                if (!File.Exists(str_path))
                {
                    XDocument doc = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement("Areas",
                                          new XElement("Pics", "Quaser1.png"),
                                          new XElement("Pics", "Quaser2.jpg")
                                    )
                                );
                    doc.Save(str_path);
                }
                try
                {
                    ds.ReadXmlSchema(str_path);
                    ds.ReadXml(str_path);
                }
                catch
                { }
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].Rows.Clear();
                    for (int i = 0; i < Areas_Pics_Length; i++)
                    {
                        if (ds.Tables[0].Rows.Count < Areas_Pics_Length)
                        {
                            DataRow dr = ds.Tables[0].NewRow();
                            ds.Tables[0].Rows.Add(dr);
                        }
                        ds.Tables[0].Rows[i][0] = Session["Areas_Pics" + i].ToString();
                    }
                    FileStream fs = new FileStream(str_path, FileMode.Create);
                    System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(fs, System.Text.Encoding.UTF8);
                    xtw.WriteProcessingInstruction("xml", "version='1.0'");
                    ds.WriteXml(xtw);
                    xtw.Close();
                }
            }
            //Application 的變數 不要在 Demo_Mode 改變
            if (item[17])// Application["company"]
            {
                Application["company"] = TextBox14.Text; 
                str_path = Server.MapPath("~/Ut_Data/company.txt");
                File.WriteAllText(str_path, Application["company"].ToString(), System.Text.Encoding.Default);
            }
            if (item[18])// Application["company_hyperlink"]
            {
                if (TextBox15.Text!="" && !TextBox15.Text.StartsWith(@"http")) TextBox15.Text = @"http://" + TextBox15.Text;
                Application["company_hyperlink"] = TextBox15.Text; 
                str_path = Server.MapPath("~/Ut_Data/company_hyperlink.txt");
                File.WriteAllText(str_path, Application["company_hyperlink"].ToString(), System.Text.Encoding.Default);
            }
            if (item[19])//  Application["WareHost0"]           
            {
                if (TextBox16.Text!="" && !TextBox16.Text.StartsWith(@"http")) TextBox16.Text = @"http://" + TextBox16.Text;
                Application["WareHost0"] = TextBox16.Text; 
                str_path = Server.MapPath("~/Ut_Data/Warehouse.xml");
                DataSet ds = new DataSet();
                if (!File.Exists(str_path))
                {
                    XDocument doc = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement("Addresses",
                                              new XElement("Address", Application["WareHost0"].ToString())
                                        )
                                    );
                    doc.Save(str_path);
                }
                try
                {
                    ds.ReadXmlSchema(str_path);
                    ds.ReadXml(str_path);
                }
                catch
                { }
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].Rows.Clear();
                    for (int i = 0; i < 1; i++)
                    {
                        if (ds.Tables[0].Rows.Count < 1)
                        {
                            DataRow dr = ds.Tables[0].NewRow();
                            ds.Tables[0].Rows.Add(dr);
                        }
                        ds.Tables[0].Rows[i][0] = Application["WareHost" + i].ToString();
                    }
                    FileStream fs = new FileStream(str_path, FileMode.Create);
                    System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(fs, System.Text.Encoding.UTF8);
                    xtw.WriteProcessingInstruction("xml", "version='1.0'");
                    ds.WriteXml(xtw);
                    xtw.Close();
                }
            }
            if (item[20])//  Application["InputOutputHost0"]        
            {
                if (TextBox17.Text!="" && !TextBox17.Text.StartsWith(@"http")) TextBox17.Text = @"http://" + TextBox17.Text;
                Application["InputOutputHost0"] = TextBox17.Text; 
                str_path = Server.MapPath("~/Ut_Data/InputOutput.xml");
                DataSet ds = new DataSet();
                if (!File.Exists(str_path))
                {
                    XDocument doc = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement("Addresses",
                                              new XElement("Address", Application["InputOutputHost0"].ToString())
                                        )
                                    );
                    doc.Save(str_path);
                }
                try
                {
                    ds.ReadXmlSchema(str_path);
                    ds.ReadXml(str_path);
                }
                catch
                { }
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].Rows.Clear();
                    for (int i = 0; i < 1; i++)
                    {
                        if (ds.Tables[0].Rows.Count < 1)
                        {
                            DataRow dr = ds.Tables[0].NewRow();
                            ds.Tables[0].Rows.Add(dr);
                        }
                        ds.Tables[0].Rows[i][0] = Application["InputOutputHost"+i].ToString();
                    }
                    FileStream fs = new FileStream(str_path, FileMode.Create);
                    System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(fs, System.Text.Encoding.UTF8);
                    xtw.WriteProcessingInstruction("xml", "version='1.0'");
                    ds.WriteXml(xtw);
                    xtw.Close();
                }
            }
        }
        //DEMO Mode
        protected void Button2_Click(object sender, EventArgs e)
        {
            bool green = false;
            green = Get_DEMO_Mode();
            green = !green;
            if (green)
                Application["DEMO_Mode"] = "True";
            else
                Application["DEMO_Mode"] = "False";
            Show_DEMO_Mode(green);
        }
        //顯示座標        
        private void Show_DEMO_Mode(bool green)
        {
            if (green)
                Button2.BackColor = System.Drawing.Color.GreenYellow;
            else
                Button2.BackColor = System.Drawing.Color.Empty;
        }
        private bool Get_DEMO_Mode()
        {
            return Convert.ToBoolean(Application["DEMO_Mode"].ToString());
        }

        //Control Table       
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
            //string image_file =Server.MapPath("~/Ut_Data/images/");                   
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

                        //第三欄圖片
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
        private void dtRecord_save()//進入前都要檢查 AutomaticControl 有就??沒有就???
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
                }
            }
            RenderGridViewToExcel(GridView1, AutomaticControl);           
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
                    //GridView1.Rows[i].Cells[4].Text = eng ? "Monitoring" : "監控中";
                    GridView1.Rows[i].Cells[4].BackColor = System.Drawing.Color.GreenYellow;
                    GridView1.Rows[i].Cells[3].BackColor = System.Drawing.Color.GreenYellow;
                }
                else
                {
                    //GridView1.Rows[i].Cells[4].Text = eng ? "not monitored" : "未監控";
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

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            Page_refresh(); 
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            bool DEMO_Mode = Convert.ToBoolean(Application["DEMO_Mode"].ToString());
            if (DEMO_Mode)
            {
                Literal mymsg = new Literal();
                mymsg.Text = "<Script>alert('" + (eng ? "DEMO_Mode" : "展示模式") + "')</Script>";
                Page.Controls.Add(mymsg);
                GridView1.EditIndex = -1;
                Page_refresh();
                return;
            }
            string tmp_Label1 = Label2.Text;
            AutomaticControl = Server.MapPath("~/Ut_Data/Log/AutomaticControl.xls");
            if (GridView1.Rows.Count <= 1)
            {
                if (GridView1.Rows.Count > 0)
                {
                    Label2.Text = eng ? "no data !!!" : "已無最後一筆資料";
                    try
                    {
                        File.Delete(AutomaticControl);
                    }
                    catch { }
                    GridView1.DataSource = null; GridView1.DataBind();
                }
                return;
            }
            int delrow = e.RowIndex;
            GridView1.EditIndex = -1;
            GridView1.DataSource = null; GridView1.DataBind();
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
            
        }
        private void Page_refresh()
        {            
            GridView1.DataSource = null; GridView1.DataBind();
            show_Select_Table();
        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            bool DEMO_Mode = Convert.ToBoolean(Application["DEMO_Mode"].ToString());
            if (DEMO_Mode)
            {
                Literal mymsg = new Literal();
                mymsg.Text = "<Script>alert('" + (eng ? "DEMO_Mode" : "展示模式") + "')</Script>";
                Page.Controls.Add(mymsg);
                GridView1.EditIndex = -1;
                Page_refresh();
                return;
            }
            GridView1.EditIndex = e.NewEditIndex;
            Page_refresh();            
            if (GridView1.Rows.Count - 1 >= GridView1.EditIndex)
                GridView1.Rows[e.NewEditIndex].Cells[3].Focus();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int t = Convert.ToInt32(Session["language"].ToString());
            bool eng = (t == 0);
            AutomaticControl = Server.MapPath("~/Ut_Data/Log/AutomaticControl.xls");
            DataTable dt = new DataTable();
            dt = make_dt(dt);
            DataRow dr;
            dr = dt.NewRow();
            dr[0] = GridView1.Rows[e.RowIndex].Cells[0].Text;
            TextBox[] str_dr = new TextBox[4];
            for (int countsub = 1; countsub < GridView1.Columns.Count - 3; countsub++)
            {
                str_dr[countsub - 1] = ((TextBox)GridView1.Rows[e.RowIndex].Cells[countsub].Controls[0]);
                dr[countsub] = str_dr[countsub - 1].Text;
            }
            dr[4] = ((((TextBox)GridView1.Rows[e.RowIndex].Cells[4].Controls[0]).Text).ToUpper() == "TRUE" ? true : false);
            dt.Rows.Add(dr);
            
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {                
                if (dt.Rows[i][0].ToString() == "Connected" || dt.Rows[i][0].ToString() == "連線正常")
                    dt.Rows[i][0] = eng ? "Connected" : "連線正常";
                else
                    dt.Rows[i][0] = eng ? "Disconnect" : "無法連線";               
            }
            int delrow = e.RowIndex;
            GridView1.EditIndex = -1;
            GridView1.DataSource = null; GridView1.DataBind();
            dtRecord.Rows.Clear();
            dtRecord_init();//讀入表格
            if (dt.Rows.Count > 0)
                for (int count = 0; count < dt.Columns.Count; count++)
                {
                    dtRecord.Rows[delrow][count] = dt.Rows[0][count];
                }
            NCA_Var.RenderDataTableToExcel(dtRecord, AutomaticControl);//dt            
            GridView1.DataSource = dtRecord; GridView1.DataBind();
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["clear_7"] = (RadioButtonList1.SelectedIndex == 0).ToString();
        }
    }
}