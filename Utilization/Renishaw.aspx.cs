using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.Util;

namespace Utilization
{
    public partial class Renishaw : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["u_name"].ToString() == "Admin" || Session["u_name"].ToString() == "User"))
            {
                Response.Write("No access right to Renishaw Statistic.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            int t = Convert.ToInt32(Session["language"].ToString());
            if (t == 0)
            {
                Page.Title = "Renishaw Statistic";
                GridView1.EmptyDataText = "Data table reserve for connecting to Renishaw Statistic";
                Button1.Text = "Read";
            }
            string str_Path = Server.MapPath("~/Ut_Data/Renishaw");
            if (!Directory.Exists(str_Path))
            {
                Response.Write("There are no Renishaw Statistic data.<br>");
                Response.End();//Server.Transfer(@"~/Default.aspx");
            }
            if (!Page.IsPostBack)///從別的頁面切回來 if (!Session.IsNewSession)//剛開啟瀏覽器 
            {
                DropDown1Bind(str_Path);
            }
        }

        private DataTable make_dt(DataTable dtRecord)
        {            
            dtRecord.Columns.Add("名稱", Type.GetType("System.String"));
            dtRecord.Columns.Add("標準值", Type.GetType("System.String"));//System.Double
            dtRecord.Columns.Add("實際值", Type.GetType("System.String"));//System.Double
            dtRecord.Columns.Add("誤差值", Type.GetType("System.String"));//System.Double
            dtRecord.Columns.Add("容許量(下限)", Type.GetType("System.String"));//System.Double
            dtRecord.Columns.Add("容許量(上限)", Type.GetType("System.String")); //System.Double
            dtRecord.Columns.Add("備用欄位", Type.GetType("System.String"));
            dtRecord.Columns.Add("合格/失敗", Type.GetType("System.String"));
            return dtRecord;
        }
        private void DropDown1Bind(string Dir_Path)
        {
            DirectoryInfo dirinfo;
            FileInfo[] files;
            dirinfo = new DirectoryInfo(Dir_Path);
            files = dirinfo.GetFiles();
            //處理檔案 
            for (int j = 0; j < files.Length; j++)
            {
                string tmp = files[j].Name;
                if (!tmp.ToUpper().EndsWith(".XLS") 
                    ) continue;
                DropDownList1.Items.Add(files[j].Name);
            }
        }
        //選擇
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        //讀取
        protected void Button1_Click(object sender, EventArgs e)
        {
            string str_Path = Server.MapPath("~/Ut_Data/Renishaw");
            if (DropDownList1.SelectedIndex < 0) return;
            string Dir_Path = str_Path + "\\" + DropDownList1.SelectedItem.Text;//這行只是除錯用
            if (DropDownList1.SelectedIndex < 0) return;
            if (File.Exists(Dir_Path))//載入 Table
            {
                GridView1.DataSource = null; GridView1.DataBind();
                DataTable RenishawTable = new DataTable();
                DataTable dtExcel = make_dt(new DataTable());//差這行就可將RenderExcelToDatatable放在 NCA_Var
                RenishawTable = RenderExcelToDataTable(Dir_Path, dtExcel);
                GridView1.DataSource = RenishawTable; GridView1.DataBind();
            }
        }
        public DataTable RenderExcelToDataTable(string FileName, DataTable dtExcel)
        {
            HSSFWorkbook excelworkbook;
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            excelworkbook = new HSSFWorkbook(fs);
            HSSFSheet Excelsheet = excelworkbook.GetSheetAt(0);
            int EmptyRowCount = 0;
            bool EmptyRow = false, EmptyRow2 = false;
            DataRow dataRow1;
            for (int i = 9; i < Excelsheet.LastRowNum; i++)
            {                
                HSSFRow excelrow = Excelsheet.GetRow(i);                          
                dataRow1 = dtExcel.NewRow();
                for (int j = 0; j < excelrow.Cells.Count; j++)
                {
                    try
                    {
                        dataRow1[j] = excelrow.Cells[j].ToString();
                    }
                    catch
                    {                      
                       break;
                    }
                }
                if (dataRow1[0].ToString() == "" ||  EmptyRow)
                {
                    if (dataRow1[0].ToString() == "" && EmptyRow) EmptyRow2 = true;
                    if (dataRow1[0].ToString() == "") EmptyRow = true;
                    else
                        EmptyRow = EmptyRow2 = false;
                }
                if (EmptyRow  & EmptyRow2) EmptyRowCount+=1;
                if (EmptyRowCount > 9) break; 
                dtExcel.Rows.Add(dataRow1);
            }
            fs.Close();
            fs.Dispose();
            return dtExcel;
        }
    }
}