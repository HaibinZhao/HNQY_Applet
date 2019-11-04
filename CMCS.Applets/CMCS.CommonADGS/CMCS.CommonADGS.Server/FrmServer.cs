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
using CMCS.DapperDber.Dbs.OracleDb;
using Oracle.ManagedDataAccess.Client;
using DevComponents.DotNetBar;

namespace CMCS.CommonADGS.Server
{
	public partial class FrmServer : BasisPlatform.Forms.FrmBasis
	{
		TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

		public FrmServer()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			lblVersion.Text = "版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.IsSecretRunning = ServerConfiguration.Instance.IsSeccetRunning;
			this.VerifyBeforeClose = ServerConfiguration.Instance.VerifyBeforeClose;

			taskSimpleScheduler.StartNewTask("化验数据接收", () =>
			{
				ExecuteAllTask();
			}, 0, grabPerformer_OutputError);
			this.Width = this.Width + 1;//重绘窗体后才能正常显示
			if (this.IsSecretRunning) this.WindowState = FormWindowState.Minimized;
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
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

		void ExecuteAllTask()
		{
			IPEndPoint receivePoint = new IPEndPoint(IPAddress.Any, ServerConfiguration.Instance.Port);
			UdpClient receiveUdp = new UdpClient(receivePoint);
			grabPerformer_OutputInfo("端口初始化成功");
			IPEndPoint sendPoint = null;
			CMCS.CommonADGS.Core.OracleSqlBuilder.OracleKeywords = ServerConfiguration.Instance.OracleKeywords.Split('|');
			while (true)
			{
				try
				{
					byte[] receiveByte = receiveUdp.Receive(ref sendPoint);
					if (receiveByte != null && receiveByte.Length > 0)
					{
						string receiveString = Encoding.UTF8.GetString(receiveByte);
						SendMessage receiveMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<SendMessage>(receiveString);
						AnalyticData(receiveMessage);
					}
				}
				catch (Exception ex)
				{
					grabPerformer_OutputError("解析数据", ex);
				}
			}
		}

		void AnalyticData(SendMessage data)
		{
			int syncCount = 0;
			bool IsMatch = false;
			//MachineCodes item = ServerConfiguration.Instance.Details.Where(a => a.UpLoadIdentifer == data.UpLoadIdentifier).FirstOrDefault();
			foreach (MachineCodes item in ServerConfiguration.Instance.Details.Where(a => a.UpLoadIdentifer == data.UpLoadIdentifier))
			{
				if (data.UpLoadIdentifier == item.UpLoadIdentifer)
				{
					IsMatch = true;
					// 未设置主键名则跳过
					if (string.IsNullOrEmpty(item.PrimarKeys))
					{
						grabPerformer_OutputInfo(string.Format("{0} 提取未执行，原因：未设置主键(PrimaryKeys)参数", item.UpLoadIdentifer));
						break;
					}

					OracleDapperDber selfDber = new OracleDapperDber(ServerConfiguration.Instance.SelfConnStr);
					using (OracleConnection connection = selfDber.CreateConnection() as OracleConnection)
					{

						if (data.DataColumns != null)
						{
							// 在数据中创建表
							if (connection.ExecuteScalar<int>(CMCS.CommonADGS.Core.OracleSqlBuilder.BuildHasTableSQL(item.DataTableName)) == 0)
								connection.Execute(CMCS.CommonADGS.Core.OracleSqlBuilder.BuildTableSQL(item.DataTableName, data.DataColumns));
						}
						else if (data.DataRows != null)
						{
							string execSql = string.Empty;

							// 生成主键值
							string primaryKeyValue = item.UpLoadIdentifer + "-" + CMCS.CommonADGS.Core.OracleSqlBuilder.BuildPrimaryKeyValue(item.PrimarKeys, data.DataRows);

							if (connection.ExecuteScalar<int>(CMCS.CommonADGS.Core.OracleSqlBuilder.BuildHasRecordSQL(item.DataTableName, primaryKeyValue)) == 0)
								execSql = CMCS.CommonADGS.Core.OracleSqlBuilder.BuildInsertSQL(item.DataTableName, primaryKeyValue, item.UpLoadIdentifer, data.DataRows);
							else
								execSql = CMCS.CommonADGS.Core.OracleSqlBuilder.BuildUpdateSQL(item.DataTableName, primaryKeyValue, item.UpLoadIdentifer, data.DataRows);

							syncCount += connection.Execute(execSql);
						}
					}
				}
			}
			if (!IsMatch) grabPerformer_OutputInfo(string.Format("接收到:{0}数据,但未找到对应配置", data.UpLoadIdentifier));
			if (syncCount > 0) grabPerformer_OutputInfo(string.Format("成功接收:{0}一条数据", data.UpLoadIdentifier));
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

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			taskSimpleScheduler.Cancal();
		}

		private void Form1_SettingsButtonClick(object sender, EventArgs e)
		{
			ConfigSetting frm = new ConfigSetting();
			if (frm.ShowDialog() == DialogResult.OK && MessageBoxEx.Show("更改的配置需要重启程序才能生效，是否立刻重启？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Application.Restart();
			}
		}
	}
}
