/*
 * Created by SharpDevelop.
 * User: guoxifeng
 * Date: 2016/9/14
 * Time: 14:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ExportDataTable
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			initdata();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public void initdata()
		{
			this.connection = new SqlConnection(connectURL);
			try {
				this.connection.Open();
				isConnected = this.connection.State == ConnectionState.Open;
			} catch (SqlException) {
				isConnected = false;
				//throw;
			}
			this.dataSet = new DataSet();
		
		}
		
		private SqlConnection connection;
		private DataSet dataSet;
		private bool isConnected = false;
		string connectURL = @"server=192.168.7.172;Initial Catalog=icma;uid=icma;pwd=abc123*;Trusted_Connection=false;";
		
		public DataTable GetData()
		{
			string StatementOrderList = "SELECT TOP 1000 [ID],[TRUENAME008],[IDCARD007] FROM [ICMA].[dbo].[TSX2015021101_BJ]";
		    var dataAdapter = new SqlDataAdapter(StatementOrderList, this.connection);
			dataAdapter.Fill(dataSet);
			return dataSet.Tables[0];
		}
		
		
        /// <summary>
        /// 导出文件，使用文件流。该方法使用的数据源为DataTable,导出的Excel文件没有具体的样式。
        /// </summary>
        /// <param name="dt"></param>
        public static string ExportToExcel(System.Data.DataTable dt, string path)
        {
            //KillSpecialExcel();
            string result = string.Empty;
            try
            {
                // 实例化流对象，以特定的编码向流中写入字符。
                StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("gb2312"));

                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    // 添加列名称
                    sb.Append(dt.Columns[k].ColumnName.ToString() + "\t");
                }
                sb.Append(Environment.NewLine);
                // 添加行数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        // 根据列数追加行数据
                        sb.Append(row[j].ToString() + "\t");
                    }
                    sb.Append(Environment.NewLine);
                }
                sw.Write(sb.ToString());
                sw.Flush();
                sw.Close();
                sw.Dispose();

                // 导出成功后打开
                //System.Diagnostics.Process.Start(path);
            }
            catch (Exception)
            {
                result = "请保存或关闭可能已打开的Excel文件";
            }
            finally
            {
                dt.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 结束进程
        /// </summary>
        private static void KillSpecialExcel()
        {
            foreach (System.Diagnostics.Process theProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
            {
                if (!theProc.HasExited)
                {
                    bool b = theProc.CloseMainWindow();
                    if (b == false)
                    {
                        theProc.Kill();
                    }
                    theProc.Close();
                }
            }
        }

		
		void BtnExpClick(object sender, EventArgs e)
		{
			DataTable dt = GetData();
			this.dataGrid1.DataSource = dt;
			
			string str = "log\\test.xls";
			string str2 = "log\\test.txt";
			
			ExportToExcel(dt,str);
			ExportToExcel(dt,str2);
	
		}
	}
}
