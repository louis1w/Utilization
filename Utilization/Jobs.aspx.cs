using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilization
{
    public partial class Jobs : System.Web.UI.Page
    {       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Response.Write("No access right to Manufacturing Order.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Manufacturing Order";
                Button1.Text = "Read";
            }
            string str_Path = Session["jobs_Path"].ToString();
            if (!Directory.Exists(str_Path))
            {
                Directory.CreateDirectory(str_Path);
            }
            if (!Page.IsPostBack)///從別的頁面切回來 if (!Session.IsNewSession)//剛開啟瀏覽器 
            {                               
                DropDown1Bind(str_Path);                
            }
        }
        private void DropDown1Bind(string Dir_Path)
        {  
            if(Dir_Path.EndsWith("\\")) Dir_Path=Dir_Path.TrimEnd('\\');
            Session["jobs_Path"] = Dir_Path;
            Label_Path.Text = Session["jobs_Path"].ToString();
            DropDownList1.Items.Clear();
            DropDownList1.Items.Add(".");
            DropDownList1.Items.Add("..");
            try
            {
                var files = Directory.GetDirectories(Dir_Path);
                for (int j = files.Length - 1; j >= 0; j--)
                {
                    string newitem = files[j].ToString();
                    string pathEnd =newitem.Substring(newitem.LastIndexOf('\\')+1);
                    DropDownList1.Items.Add(pathEnd);
                }
            }
            catch {  }
            DropDown3Bind(Session["jobs_Path"].ToString());
        }
        private void DropDown3Bind(string file_Path)
        {
            DropDownList3.Items.Clear();
            DirectoryInfo dirinfo;
            FileInfo[] files;
            dirinfo = new DirectoryInfo(file_Path);
            files = dirinfo.GetFiles();
            //處理檔案 
            for (int j = 0; j < files.Length; j++)
            { 
                string tmp=files[j].Name;
                if (tmp.ToUpper() == "WEB.CONFIG" ||                    
                    tmp.ToUpper().EndsWith(".EXE") ||
                    tmp.ToUpper().EndsWith(".MDB")                     
                    ) continue;
                DropDownList3.Items.Add(files[j].Name);                
            }


        }

        private void Change_iframe(string pdf_path)
        {
            string someScript = "chgFrame";
            string file_path = pdf_path;// "Ut_Data/Holidays.pdf";
            if (file_path != "")
            {
                string test=Server.MapPath("~/Ut_Data/jobs");
                file_path = file_path.Replace(test, "Ut_Data/jobs");
                file_path = file_path.Replace("\\", "/");
                if (!file_path.ToUpper().EndsWith(".PDF"))
                    Response.Redirect(file_path);
            } 
            if (!ClientScript.IsStartupScriptRegistered(this.GetType(), someScript))
            {
                ClientScript.RegisterStartupScript(this.GetType(),
                someScript, "chgFrame('" + file_path + "')", true);
            }
            
        }
        //選取目錄 , 要先清空
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str_Path = Server.MapPath("~/Ut_Data/jobs");
            string Dir_Path = "";
            Change_iframe("");
            if (DropDownList1.SelectedIndex >= 1)
            {
                if (DropDownList1.SelectedIndex == 1)
                {
                    string tmp = Session["jobs_Path"].ToString();
                    tmp = tmp.Substring(0, tmp.LastIndexOf("\\")+1);
                    tmp = tmp.TrimEnd('\\');//回上一層不可少這行
                    if (Directory.Exists(tmp) && tmp.Length>=str_Path.Length) Dir_Path = tmp;
                    else
                        Dir_Path = Session["jobs_Path"].ToString();
                }
                else
                {
                    string tmp1 = str_Path + "\\" + DropDownList1.SelectedItem.Text;
                    string tmp2 = Session["jobs_Path"].ToString() + "\\" + DropDownList1.SelectedItem.Text;
                    Dir_Path = (tmp1.Length > tmp2.Length ? tmp1 : tmp2);
                }
                if (Directory.Exists(Dir_Path))
                {
                    DropDown1Bind(Dir_Path);                   
                }
            }
            else
                return;                     
        }
        //選取檔案  ,目錄存在 && 檔案存在 時讀取
        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {            
        }
       
        protected void Button1_Click(object sender, EventArgs e)
        {            
            string str_Path = Server.MapPath("~/Ut_Data/jobs");
            if (DropDownList1.SelectedIndex==1) return;
            string Dir_Path = str_Path + "\\" + DropDownList1.SelectedItem.Text;//這行只是除錯用
            if (DropDownList3.SelectedIndex < 0) return;
            Dir_Path = "";//這行只是除錯用
            string file_Path = Session["jobs_Path"].ToString()+"\\" + DropDownList3.SelectedItem.Text;
            if (File.Exists(file_Path))
            {
                Change_iframe(file_Path);
            }
        }
    }
}