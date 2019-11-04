using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using CMCS.CommonADGS.Configurations;

namespace CMCS.CommonADGS.Win
{
	public partial class ConfigSetting : Form
	{
		public ConfigSetting()
		{
			InitializeComponent();
		}

		private void ConfigSetting_Load(object sender, EventArgs e)
		{
			try
			{
				initPage();
			}
			catch (Exception ex)
			{
				MessageBox.Show("配置文件读取失败，原因：" + ex.Message);
			}
		}

		#region 读取配置文件后内容显示
		private void initPage()
		{
			FillUIFromConfig();
		}

		private void FillUIFromConfig()
		{
			this.txtAppIdentifier.Text = ClientConfiguration.Instance.AppIdentifier;
			this.txtGrabInterval.Value = ClientConfiguration.Instance.GrabInterval;
			this.txtUpLoadIdentifier.Text = ClientConfiguration.Instance.UpLoadIdentifier;
			this.txtConnStr.Text = ClientConfiguration.Instance.ConnStr;
			this.txtIP.Text = ClientConfiguration.Instance.ServerIp;
			this.txtPort.Value = ClientConfiguration.Instance.ServerPort;
			this.txtSQL.Text = ClientConfiguration.Instance.SQL;
			this.ddlDbType.Text = ClientConfiguration.Instance.DbType;
			this.txtProcessName.Text = ClientConfiguration.Instance.ProcessName;
			this.ckStartup.Checked = ClientConfiguration.Instance.Startup;
			this.ckIsSeccetRunning.Checked = ClientConfiguration.Instance.IsSeccetRunning;
			this.ckVerifyBeforeClose.Checked = ClientConfiguration.Instance.VerifyBeforeClose;
		}
		#endregion

		#region 保存基础配置
		private void btnBase_Click(object sender, EventArgs e)
		{
			String notEmpty = String.Empty;
			if (String.IsNullOrWhiteSpace(txtAppIdentifier.Text.Trim()))
				notEmpty += "程序唯一标识、";
			if (String.IsNullOrWhiteSpace(txtUpLoadIdentifier.Text.Trim()))
				notEmpty += "上传标识、";
			if (String.IsNullOrWhiteSpace(txtIP.Text.Trim()))
				notEmpty += "服务器IP、";
			if (String.IsNullOrWhiteSpace(ddlDbType.Text.Trim()))
				notEmpty += "数据库类型、";
			if (String.IsNullOrWhiteSpace(txtConnStr.Text.Trim()))
				notEmpty += "数据库连接字符串、";
			if (!String.IsNullOrWhiteSpace(notEmpty))
			{
				MessageBox.Show(String.Format("{0}不能为空！", notEmpty.Trim('、')));
				return;
			}
			if (txtGrabInterval.Value <= 0)
			{
				MessageBox.Show("提取时间间隔不能小于等于0！");
				return;
			}
			try
			{
				ClientConfiguration.Instance.AppIdentifier = txtAppIdentifier.Text.Trim();
				ClientConfiguration.Instance.UpLoadIdentifier = txtUpLoadIdentifier.Text.Trim();
				ClientConfiguration.Instance.ServerIp = txtIP.Text.Trim();
				ClientConfiguration.Instance.ServerPort = (int)txtPort.Value;
				ClientConfiguration.Instance.ConnStr = txtConnStr.Text.Trim();
				ClientConfiguration.Instance.SQL = txtSQL.Text.Trim();
				ClientConfiguration.Instance.DbType = ddlDbType.Text;
				ClientConfiguration.Instance.ProcessName = txtProcessName.Text;
				ClientConfiguration.Instance.GrabInterval = txtGrabInterval.Value;
				ClientConfiguration.Instance.Startup = ckStartup.Checked;
				ClientConfiguration.Instance.IsSeccetRunning = ckIsSeccetRunning.Checked;
				ClientConfiguration.Instance.VerifyBeforeClose = ckVerifyBeforeClose.Checked;
				ClientConfiguration.Instance.Save();
				//MessageBox.Show("基础配置保存成功！");
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("基础配置保存失败，原因：" + ex.Message);
			}
		}
		#endregion
	}
}
