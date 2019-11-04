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

namespace CMCS.CommonADGS.Server
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
                this.dataGridView1.AutoGenerateColumns = false;
                initPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("配置文件读取失败，原因：" + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MachineCodes machineCode = new MachineCodes();
            ServerConfiguration.Instance.Details.Add(machineCode);
            this.dataGridView1.DataSource = new List<MachineCodes>();
            this.dataGridView1.DataSource = new BindingList<MachineCodes>(ServerConfiguration.Instance.Details);
        }

        #region 读取配置文件后内容显示
        private void initPage()
        {
            FillUIFromConfig();
        }

        private void FillUIFromConfig()
        {
            this.txtAppIdentifier.Text = ServerConfiguration.Instance.AppIdentifier;
            this.txtSelfConnStr.Text = ServerConfiguration.Instance.SelfConnStr;
            this.txtPort.Value = ServerConfiguration.Instance.Port;
            this.txtOracleKeywords.Text = ServerConfiguration.Instance.OracleKeywords;
            this.ckStartup.Checked = ServerConfiguration.Instance.Startup;
            this.ckIsSeccetRunning.Checked = ServerConfiguration.Instance.IsSeccetRunning;
            this.ckVerifyBeforeClose.Checked = ServerConfiguration.Instance.VerifyBeforeClose;

            this.dataGridView1.DataSource = new BindingList<MachineCodes>(ServerConfiguration.Instance.Details);
        }
        #endregion

        #region 保存基础配置
        private void btnSave_Click(object sender, EventArgs e)
        {
            String notEmpty = String.Empty;
            if (String.IsNullOrWhiteSpace(txtAppIdentifier.Text.Trim()))
                notEmpty += "程序唯一标识、";
            if (String.IsNullOrWhiteSpace(txtOracleKeywords.Text.Trim()))
                notEmpty += "Oracle关键字、";
            if (String.IsNullOrWhiteSpace(txtSelfConnStr.Text.Trim()))
                notEmpty += "数据库连接字符串、";
            if (!String.IsNullOrWhiteSpace(notEmpty))
            {
                MessageBox.Show(String.Format("{0}不能为空！", notEmpty.Trim('、')));
                return;
            }

            try
            {
                ServerConfiguration.Instance.AppIdentifier = txtAppIdentifier.Text.Trim();
                ServerConfiguration.Instance.Port = (int)txtPort.Value;
                ServerConfiguration.Instance.OracleKeywords = txtOracleKeywords.Text.Trim();
                ServerConfiguration.Instance.SelfConnStr = txtSelfConnStr.Text.Trim();
                ServerConfiguration.Instance.Startup = ckStartup.Checked;
                ServerConfiguration.Instance.IsSeccetRunning = ckIsSeccetRunning.Checked;
                ServerConfiguration.Instance.VerifyBeforeClose = ckVerifyBeforeClose.Checked;

                //ServerConfiguration.Instance.Details.Clear();
                //foreach (DataGridViewRow item in this.dataGridView1.Rows)
                //{
                //    MachineCodes machineCode = new MachineCodes();
                //    if (item.Cells[1].Value == null) continue;
                //    machineCode.UpLoadIdentifer = item.Cells[1].Value.ToString();
                //    machineCode.DataTableName = item.Cells[2].Value.ToString();
                //    machineCode.PrimarKeys = item.Cells[3].Value.ToString();
                //    ServerConfiguration.Instance.Details.Add(machineCode);
                //}

                ServerConfiguration.Instance.Save();
                //MessageBox.Show("基础配置保存成功！");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("基础配置保存失败，原因：" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == -1 || e.RowIndex == -1)
                {
                    return;
                }
                string headText = this.dataGridView1.Columns[e.ColumnIndex].Name;
                switch (headText)
                {
                    case "Delete":
                        this.dataGridView1.Rows.RemoveAt(e.RowIndex);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
	}
}
