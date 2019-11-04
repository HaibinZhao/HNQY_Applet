using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using BasisPlatform.Util;
using CMCS.CommonADGS.Core;
using CMCS.CommonADGS.Utilities;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.Metro;
using CMCS.CommonADGS.Configurations;
using DevComponents.DotNetBar;
using System.Threading;

namespace CMCS.CommonADGS.Win
{
	public partial class FrmWin : BasisPlatform.Forms.FrmBasis
	{
		System.Timers.Timer timer1 = new System.Timers.Timer();
		TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

		public FrmWin()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			lblVersion.Text = "版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.IsSecretRunning = ClientConfiguration.Instance.IsSeccetRunning;
			this.VerifyBeforeClose = ClientConfiguration.Instance.VerifyBeforeClose;
			this.Width = this.Width + 1;//重绘窗体后才能正常显示

			taskSimpleScheduler.StartNewTask("化验数据发送", () =>
			{
				ExecuteAllTask();
			}, (int)ClientConfiguration.Instance.GrabInterval * 60 * 1000, grabPerformer_OutputError);

			this.Text = ClientConfiguration.Instance.AppIdentifier;
			if (this.IsSecretRunning)
			{
				this.WindowState = FormWindowState.Minimized;
				this.ShowInTaskbar = false;
			}
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			//timer1.Interval = (double)ClientConfiguration.Instance.GrabInterval * 60 * 1000;
			//timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Elapsed);
			//timer1_Elapsed(null, null);
			try
			{
#if DEBUG

#else
				// 添加、取消开机启动
				if (ADGSAppConfig.GetInstance().Startup)
					StartUpUtil.InsertStartUp(Application.ProductName, Application.ExecutablePath);
				else
					StartUpUtil.DeleteStartUp(Application.ProductName);
#endif
			}
			catch { }

		}

		void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer1.Stop();

			try
			{
				ExecuteAllTask();
			}
			catch (Exception ex)
			{
				grabPerformer_OutputError("执行任务失败", ex);
			}
			finally
			{
				timer1.Start();
			}
		}

		void ExecuteAllTask()
		{
			if (System.Diagnostics.Process.GetProcessesByName(ClientConfiguration.Instance.ProcessName).Length > 1)
			{
				grabPerformer_OutputInfo("化验进程正在运行，暂停上传");
				return;
			}
			DataTable dtl = new DataTable();

			switch (ClientConfiguration.Instance.DbType.ToLower())
			{
				case "access":
					dtl = new CMCS.DapperDber.Dbs.AccessDb.AccessDapperDber(ClientConfiguration.Instance.ConnStr).ExecuteDataTable(ClientConfiguration.Instance.SQL);
					break;
				case "sqlserver":
					dtl = new CMCS.DapperDber.Dbs.SqlServerDb.SqlServerDapperDber(ClientConfiguration.Instance.ConnStr).ExecuteDataTable(ClientConfiguration.Instance.SQL);
					break;
				case "sqlite":
					dtl = new CMCS.DapperDber.Dbs.SQLiteDb.SQLiteDapperDber(ClientConfiguration.Instance.ConnStr).ExecuteDataTable(ClientConfiguration.Instance.SQL);
					break;
			}
			UdpClient udpClient = new UdpClient();
			IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse(ClientConfiguration.Instance.ServerIp), ClientConfiguration.Instance.ServerPort);
			SendMessage sendMessage = new SendMessage();
			sendMessage.UpLoadIdentifier = ClientConfiguration.Instance.UpLoadIdentifier;
			sendMessage.DataColumns = CMCS.CommonADGS.Core.OracleSqlBuilder.ColumnToColumnList(dtl.Columns);

			byte[] sendByte = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(sendMessage));
			udpClient.Send(sendByte, sendByte.Length, serverPoint);
			grabPerformer_OutputInfo("数据库结构发送成功");
			int res = 0;
			foreach (DataRow item in dtl.Rows)
			{
				sendMessage = new SendMessage();
				sendMessage.UpLoadIdentifier = ClientConfiguration.Instance.UpLoadIdentifier;
				sendMessage.DataRows = CMCS.CommonADGS.Core.OracleSqlBuilder.ColumnToColumnList(item);

				sendByte = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(sendMessage));
				udpClient.Send(sendByte, sendByte.Length, serverPoint);
				Thread.Sleep(10);
				res++;
			}
			grabPerformer_OutputInfo(string.Format("发送{0}条数据", res));
		}

		void grabPerformer_OutputError(string describe, Exception ex)
		{
			OutputErrorInfo(describe, ex);

			Log4netUtil.Error(describe, ex);
		}

		void grabPerformer_OutputInfo(string info)
		{
			OutputRunInfo(rtxtOutput, info);

			Log4netUtil.Info(info);
		}

		#region Util

		/// <summary>
		/// 输出信息类型
		/// </summary>
		public enum eOutputType
		{
			/// <summary>
			/// 普通
			/// </summary>
			[Description("#BD86FA")]
			Normal,
			/// <summary>
			/// 重要
			/// </summary>
			[Description("#A50081")]
			Important,
			/// <summary>
			/// 警告
			/// </summary>
			[Description("#F9C916")]
			Warn,
			/// <summary>
			/// 错误
			/// </summary>
			[Description("#DB2606")]
			Error
		}

		/// <summary>
		/// 输出运行信息
		/// </summary>
		/// <param name="richTextBox"></param>
		/// <param name="text"></param>
		/// <param name="outputType"></param>
		private void OutputRunInfo(RichTextBoxEx richTextBox, string text, eOutputType outputType = eOutputType.Normal)
		{
			this.InvokeEx(() =>
			{
				if (richTextBox.TextLength > 100000) richTextBox.Clear();

				text = string.Format(" # {0} - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text);

				richTextBox.SelectionStart = richTextBox.TextLength;

				switch (outputType)
				{
					case eOutputType.Normal:
						richTextBox.SelectionColor = ColorTranslator.FromHtml("#BD86FA");
						break;
					case eOutputType.Important:
						richTextBox.SelectionColor = ColorTranslator.FromHtml("#A50081");
						break;
					case eOutputType.Warn:
						richTextBox.SelectionColor = ColorTranslator.FromHtml("#F9C916");
						break;
					case eOutputType.Error:
						richTextBox.SelectionColor = ColorTranslator.FromHtml("#DB2606");
						break;
					default:
						richTextBox.SelectionColor = Color.White;
						break;
				}

				richTextBox.AppendText(string.Format("{0}\r", text));

				richTextBox.ScrollToCaret();
			});
		}

		/// <summary>
		/// 输出异常信息
		/// </summary>
		/// <param name="text"></param>
		/// <param name="ex"></param>
		private void OutputErrorInfo(string text, Exception ex)
		{
			this.InvokeEx(() =>
			 {
				 text = string.Format("# {0} - {1}\r\n{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text, ex.Message);

				 OutputRunInfo(rtxtOutput, text + "", eOutputType.Error);
			 });
		}

		/// <summary>
		/// Invoke封装
		/// </summary>
		/// <param name="action"></param>
		public void InvokeEx(Action action)
		{
			if (this.IsDisposed || !this.IsHandleCreated) return;

			this.Invoke(action);
		}

		#endregion

		private void Form1_SettingsButtonClick(object sender, EventArgs e)
		{
			ConfigSetting frm = new ConfigSetting();
			if (frm.ShowDialog() == DialogResult.OK && MessageBoxEx.Show("更改的配置需要重启程序才能生效，是否立刻重启？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Application.Restart();
			}
		}

		private void FrmWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			taskSimpleScheduler.Cancal();
		}
	}
}
