using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Data;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;

namespace Utilization
{
    
    public  class Global : System.Web.HttpApplication
    {
        
        void Application_Start(object sender, EventArgs e)
        {
            //DEMO_Mode
            if(Application["DEMO_Mode"]==null)
               Application["DEMO_Mode"] = "False";
            // 應用程式啟動時執行的程式碼
            NCA_Var.Wireless = true;
            read_hosts();
            //公告欄文字
            Application["BulletinBoardText"] = @"請由  關於--->網站管理(admin.aspx)  張貼公告";
            Application["About"] = @"請用 About.txt  張貼內容";
            //設定儲存發言的最多數量
            Application["ChatMsgMax"] = 100;
            //初始化 ChatMsgLast 變數, 此變數用來指向目前最新發言的位置
            Application["ChatMsgLast"] = 0;
            
            //庫存記錄/登記__連結網址
            string str_path = "";
            Application["WareHost0"] = "";
            str_path = Server.MapPath("~/Ut_Data/Warehouse.xml");
            if (File.Exists(str_path)) 
            {
                DataSet ds = new DataSet();
                try
                {
                    ds.ReadXmlSchema(str_path);
                    ds.ReadXml(str_path);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i][0].ToString() == "" || i>0) break;
                        Application["WareHost" + i] = ds.Tables[0].Rows[i][0].ToString();
                    }
                }
                catch {}//http://www.yahoo.com.tw--->""    
            }
            //進出料記錄/登記__連結網址            
            str_path = Server.MapPath("~/Ut_Data/InputOutput.xml");
            Application["InputOutputHost0"] = "";
            if (File.Exists(str_path)) //return;
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds.ReadXmlSchema(str_path);
                    ds.ReadXml(str_path);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i][0].ToString() == "" || i > 0) break;
                        Application["InputOutputHost" + i] = ds.Tables[0].Rows[i][0].ToString();
                    }
                }
                catch {}//http://www.yahoo.com.tw--->""
            }
            //自定義公司名稱
            Application["company"] = "我的公司";
            Application["company_hyperlink"] = "http://www.yahoo.com.tw";
            str_path = "";
            str_path = Server.MapPath("~/Ut_Data/company.txt");
            if (File.Exists(str_path))
            {
                try
                {
                    string text = File.ReadAllText(str_path, System.Text.Encoding.Default);
                    text = text.Replace(" ", "&nbsp;");
                    text = text.Replace("\n", "<br />");
                    Application["company"] = text;
                }
                catch
                {
                    Application["company"] = "我的公司";
                }
            }
            str_path = Server.MapPath("~/Ut_Data/company_hyperlink.txt");
            if (File.Exists(str_path))
            {
                try
                {
                    string text = File.ReadAllText(str_path, System.Text.Encoding.Default);
                    text = text.Replace(" ", "&nbsp;");
                    text = text.Replace("\n", "<br />");
                    Application["company_hyperlink"] = text;
                }
                catch
                {
                    Application["company_hyperlink"] = "http://www.yahoo.com.tw";
                }
            }
        }

        public void read_hosts()///這個跟Default裡面的不一樣
        {
            //以下改抓取 Hosts.xml
            str_path = make_XMLfile();
            DataSet ds = new DataSet();
            ds.ReadXmlSchema(str_path);
            ds.ReadXml(str_path);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i][0].ToString() == "") break;
                Application["Host" + i] = ds.Tables[0].Rows[i][0].ToString();
                Application["Hosts_num"] = (i + 1).ToString();
            }
            int Hosts_num = Convert.ToInt32(Application["Hosts_num"]);
            Utilization._Default.thread_ping = new bool[Hosts_num];
            for (int j = 0; j < Hosts_num; j++) Utilization._Default.thread_ping[j] = true;
        }
        string str_path = "";
        public string make_XMLfile()
        {
            string TempImageFiles = Server.MapPath("~/Ut_Data/Log");
            if (!Directory.Exists(TempImageFiles)) Directory.CreateDirectory(TempImageFiles);
            str_path = Server.MapPath("~/Ut_Data/Hosts.xml");
            if (!File.Exists(str_path)) //return;
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

        void Application_End(object sender, EventArgs e)
        {
            //  應用程式關閉時執行的程式碼

        }

        void Application_Error(object sender, EventArgs e)
        {
            // 發生未處理錯誤時執行的程式碼

        }
        private void decrypt(string pwd_text)
        {
            string[] decrypt = pwd_text.Split('\n');
            char[] encrypt1, encrypt2, encrypt3;
            encrypt1 = new char[decrypt[0].Length];
            for (int i = 0; i < decrypt[0].Length; ++i)
                encrypt1[i] = (char)(decrypt[0][i] ^ 127);
            Session["Guest"] = new string(encrypt1);

            if (decrypt.Length  > 1)
            {
                encrypt2 = new char[decrypt[1].Length];
                for (int j = 0; j < decrypt[1].Length; ++j)
                    encrypt2[j] = (char)(decrypt[1][j] ^ 127);

            }
            else encrypt2 = new char[1];
            Session["User"] = new string(encrypt2);

            if (decrypt.Length > 2)
            {
                encrypt3 = new char[decrypt[2].Length];
                for (int k = 0; k < decrypt[2].Length; ++k)
                    encrypt3[k] = (char)(decrypt[2][k] ^ 127);
            }
            else encrypt3 = new char[1];
            Session["Admin"] = new string(encrypt3);
            
            

        }
        void Session_Start(object sender, EventArgs e)
        {
            string str_path = "";
            str_path = Server.MapPath("~/Ut_Data/images/dwp.txt");            
            if (File.Exists(str_path))
            {
                string pwd_text = File.ReadAllText(str_path);
                decrypt(pwd_text);
            }
            if (Session["clear_7"] == null) Session["clear_7"] = "False";
            if (Session["u_name"] == null) Session["u_name"] = "Guest";
            if (Session["u_rank"] == null) Session["u_rank"] = "Guest";
            if (Session["Login"] == null) Session["Login"] = "";
            //若無pwd.txt 預設  自由設定(非空白)
            if (Session["Guest"] == null) Session["Guest"] = "";
            if (Session["User"] == null) Session["User"] = "";
            if (Session["Admin"] == null) Session["Admin"] = "";
            // 啟動新工作階段時執行的程式碼
            if (Session["hisSeltDate"] == null) Session["hisSeltDate"] = DateTime.Now.ToString("yyyy-MM-dd");
            if (Session["hisSeltTime"] == null) Session["hisSeltTime"] = DateTime.Now.ToString("HH");
            if (Session["hisSeltItem"] == null) Session["hisSeltItem"] = "工件數";
            if (Session["language"] == null) Session["language"] = "1";//0:English 1:中文
            if (Session["EMG_Red"] == null) Session["EMG_Red"] = "Empty";
            if (Session["ALM_Red"] == null) Session["ALM_Red"] = "Empty";
            if (Session["SpindleLoadIndex"] == null) Session["SpindleLoadIndex"] = "0";
            if (Session["default_Title"] == null) Session["default_Title"] = @"CNC 分散式(跨廠)監控中心";
            for (int i = 0; i < 60; i++)
            {
                if (Session["SpindleLoad" + i] == null) Session["SpindleLoad" + i] = "0";
                if (Session["SpindleLoadTime" + i] == null) Session["SpindleLoadTime" + i] = "";
            }
            sitemaster_Load();
            //製令單
            if (Session["jobs_Path"] == null) Session["jobs_Path"] = Server.MapPath("~/Ut_Data/jobs");
        }
        private void sitemaster_Load()
        {            
            string str_path = "";
            str_path = Server.MapPath("~/Ut_Data/Area_no.txt");
            int area = 0;///下面-----也要改---
            if (File.Exists(str_path))
            {
                string area_text = File.ReadAllText(str_path);
                int.TryParse(area_text, out area);
            }
            Session["area"] = area.ToString();

            str_path = Server.MapPath("~/Ut_Data/PageTitle.txt");
            if (File.Exists(str_path))
            {
                Session["default_Title"] = File.ReadAllText(str_path, System.Text.Encoding.Default); //
            }

            ///TransAddress
            str_path = Server.MapPath("~/Ut_Data/TransAddress.xml");
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
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXmlSchema(str_path);//
                ds.ReadXml(str_path);

                string[] TransHosts;//轉址
                TransHosts = new string[ds.Tables[0].Rows.Count];
                Session["TransHosts.Length"] = TransHosts.Length.ToString();
            }
            catch
            {
                Session["TransHosts.Length"] = "0";
                Session["TransHosts0"] = Session["TransHosts1"] = Session["TransHosts2"] = Session["TransHosts3"] = "127.0.0.1";
            }
            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "") break;
                    Session["TransHosts" + i] = ds.Tables[0].Rows[i][0].ToString();  //Application["TransHost" + i]               
                }
            ///Areas_name
            str_path = Server.MapPath("~/Ut_Data/Areas_name.xml");
            if (!File.Exists(str_path))
            {
                XDocument doc = new XDocument(
                                new XDeclaration("1.0", "utf-8", "yes"),
                                new XElement("Areas",
                                          new XElement("Name", "第一廠區"),
                                          new XElement("Name", "第二廠區")
                                    )
                                );
                doc.Save(str_path);
            }
            ds = new DataSet();
            try
            {
                ds.ReadXmlSchema(str_path);
                ds.ReadXml(str_path);

                string[] Areas_Name;//廠區
                Areas_Name = new string[ds.Tables[0].Rows.Count];
                Session["Areas_Name.Length"] = Areas_Name.Length.ToString();
            }
            catch
            {
                Session["Areas_Name.Length"] = "1";
                Session["Areas_Name0"] = "第一廠區";
                Session["Areas_Name1"] = "第二廠區";
                Session["Areas_Name2"] = "第三廠區";
                Session["Areas_Name3"] = "第四廠區";
            }
            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "") break;
                    Session["Areas_Name" + i] = ds.Tables[0].Rows[i][0].ToString();
                }
            ///Areas_Pics
            str_path = Server.MapPath("~/Ut_Data/Areas_Pics.xml");
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
            ds = new DataSet();
            try
            {
                ds.ReadXmlSchema(str_path);
                ds.ReadXml(str_path);

                string[] Areas_Pics;//圖片
                Areas_Pics = new string[ds.Tables[0].Rows.Count];
                Session["Areas_Pics.Length"] = Areas_Pics.Length.ToString();
            }
            catch
            {
                Session["Areas_Pics.Length"] = "1";
                Session["Areas_Pics0"] = "Quaser1.png";
                Session["Areas_Pics1"] = "Quaser2.jpg";
                Session["Areas_Pics2"] = "Quaser1.png";
                Session["Areas_Pics3"] = "Quaser2.jpg";
            }
            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "") break;
                    Session["Areas_Pics" + i] = ds.Tables[0].Rows[i][0].ToString();
                }
        }
        void Session_End(object sender, EventArgs e)
        {
            // 工作階段結束時執行的程式碼。 
            // 注意: 只有在 Web.config 檔將 sessionstate 模式設定為 InProc 時，
            // 才會引發 Session_End 事件。如果將工作階段模式設定為 StateServer 
            // 或 SQLServer，就不會引發這個事件。

        }

    }
}
