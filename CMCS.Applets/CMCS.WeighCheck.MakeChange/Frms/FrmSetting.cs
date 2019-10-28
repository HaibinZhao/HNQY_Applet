using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.WeighCheck.MakeChange.Frms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors;

namespace CMCS.WeighCheck.MakeChange.Frms
{
    public partial class FrmSetting : DevComponents.DotNetBar.Metro.MetroForm
    {
        CommonDAO commonDAO = CommonDAO.GetInstance();
        CommonAppConfig commonAppConfig = CommonAppConfig.GetInstance();

        string Old_Param = string.Empty;
        public FrmSetting()
        {
            InitializeComponent();
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {
            try
            {
                labelX1.ForeColor = Color.Red;
                labelX4.ForeColor = Color.Red;
                labelX14.ForeColor = Color.Red;
                labelX10.ForeColor = Color.Red;

                txtAppIdentifier.Text = commonAppConfig.AppIdentifier;
                txtSelfConnStr.Text = commonAppConfig.SelfConnStr;
                // 读卡器
                iptxtIP.Value = commonDAO.GetAppletConfigString("读卡器IP");
                txtPort.Text = commonDAO.GetAppletConfigString("读卡器端口");
                SelectedComboItem(commonDAO.GetAppletConfigString("读卡器扇区"), cmbSecNumber);
                SelectedComboItem(commonDAO.GetAppletConfigString("读卡器块区"), cmbBlockNumber);

                //电子秤
                SelectedComboItem("COM" + commonDAO.GetAppletConfigInt32("电子秤串口"), cmbLibra_COM);
                SelectedComboItem(commonDAO.GetAppletConfigString("电子秤波特率"), cmbLibra_Bandrate);
                SelectedComboItem(commonDAO.GetAppletConfigString("电子秤数据位"), cmbDataBits);
                SelectedComboItem(commonDAO.GetAppletConfigString("电子秤停止位"), cmbParity);
                //电子秤最小重量
                dInputLibraWeight.Value = commonDAO.GetAppletConfigDouble("电子秤最小重量");
                //是否启用称重
                chkIsUseWeight.Checked = Convert.ToBoolean(commonDAO.GetAppletConfigInt32("启用称重"));

                //样罐
                db2Weight.Value = commonDAO.GetCommonAppletConfigDouble("0.2mm样罐重");
                db3Weight.Value = commonDAO.GetCommonAppletConfigDouble("3mm样罐重");
                db6Weight.Value = commonDAO.GetCommonAppletConfigDouble("6mm样罐重");

                db2OverWeight.Value = commonDAO.GetCommonAppletConfigDouble("0.2mm超差重");
                db3OverWeight.Value = commonDAO.GetCommonAppletConfigDouble("3mm超差重");
                db6OverWeight.Value = commonDAO.GetCommonAppletConfigDouble("6mm超差重");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("参数初始化失败" + ex.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// 选中ComboItem
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cmb"></param>
        private void SelectedComboItem(string text, ComboBoxEx cmb)
        {
            foreach (ComboItem item in cmb.Items)
            {
                if (item.Text == text)
                {
                    cmb.SelectedItem = item;
                    break;
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            commonAppConfig.AppIdentifier = txtAppIdentifier.Text.Trim();
            commonAppConfig.SelfConnStr = txtSelfConnStr.Text;
            commonAppConfig.Save();

            //电子秤
            commonDAO.SetAppletConfig("电子秤串口", (cmbLibra_COM.SelectedIndex + 1).ToString());
            commonDAO.SetAppletConfig("电子秤波特率", (cmbLibra_Bandrate.SelectedItem as ComboItem).Text);
            commonDAO.SetAppletConfig("电子秤数据位", (cmbDataBits.SelectedItem as ComboItem).Text);
            commonDAO.SetAppletConfig("电子秤停止位", (cmbParity.SelectedItem as ComboItem).Text);

            //电子秤最小重量
            commonDAO.SetAppletConfig("电子秤最小重量", dInputLibraWeight.Value.ToString());
            //是否启用称重
            commonDAO.SetAppletConfig("启用称重", (chkIsUseWeight.Checked ? 1 : 0).ToString());

            //样罐
            commonDAO.SetCommonAppletConfig("0.2mm样罐重", db2Weight.Value.ToString());
            commonDAO.SetCommonAppletConfig("3mm样罐重", db3Weight.Value.ToString());
            commonDAO.SetCommonAppletConfig("6mm样罐重", db6Weight.Value.ToString());

            commonDAO.SetCommonAppletConfig("0.2mm超差重", db2OverWeight.Value.ToString());
            commonDAO.SetCommonAppletConfig("3mm超差重", db3OverWeight.Value.ToString());
            commonDAO.SetCommonAppletConfig("6mm超差重", db6OverWeight.Value.ToString());


            //读卡器
            commonDAO.SetAppletConfig("读卡器IP", iptxtIP.Value);
            commonDAO.SetAppletConfig("读卡器端口", txtPort.Text);
            commonDAO.SetAppletConfig("读卡器扇区", (cmbSecNumber.SelectedItem as ComboItem).Text);
            commonDAO.SetAppletConfig("读卡器块区", (cmbBlockNumber.SelectedItem as ComboItem).Text);

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}