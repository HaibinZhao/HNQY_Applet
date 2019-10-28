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
                // ������
                iptxtIP.Value = commonDAO.GetAppletConfigString("������IP");
                txtPort.Text = commonDAO.GetAppletConfigString("�������˿�");
                SelectedComboItem(commonDAO.GetAppletConfigString("����������"), cmbSecNumber);
                SelectedComboItem(commonDAO.GetAppletConfigString("����������"), cmbBlockNumber);

                //���ӳ�
                SelectedComboItem("COM" + commonDAO.GetAppletConfigInt32("���ӳӴ���"), cmbLibra_COM);
                SelectedComboItem(commonDAO.GetAppletConfigString("���ӳӲ�����"), cmbLibra_Bandrate);
                SelectedComboItem(commonDAO.GetAppletConfigString("���ӳ�����λ"), cmbDataBits);
                SelectedComboItem(commonDAO.GetAppletConfigString("���ӳ�ֹͣλ"), cmbParity);
                //���ӳ���С����
                dInputLibraWeight.Value = commonDAO.GetAppletConfigDouble("���ӳ���С����");
                //�Ƿ����ó���
                chkIsUseWeight.Checked = Convert.ToBoolean(commonDAO.GetAppletConfigInt32("���ó���"));

                //����
                db2Weight.Value = commonDAO.GetCommonAppletConfigDouble("0.2mm������");
                db3Weight.Value = commonDAO.GetCommonAppletConfigDouble("3mm������");
                db6Weight.Value = commonDAO.GetCommonAppletConfigDouble("6mm������");

                db2OverWeight.Value = commonDAO.GetCommonAppletConfigDouble("0.2mm������");
                db3OverWeight.Value = commonDAO.GetCommonAppletConfigDouble("3mm������");
                db6OverWeight.Value = commonDAO.GetCommonAppletConfigDouble("6mm������");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("������ʼ��ʧ��" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// ѡ��ComboItem
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

            //���ӳ�
            commonDAO.SetAppletConfig("���ӳӴ���", (cmbLibra_COM.SelectedIndex + 1).ToString());
            commonDAO.SetAppletConfig("���ӳӲ�����", (cmbLibra_Bandrate.SelectedItem as ComboItem).Text);
            commonDAO.SetAppletConfig("���ӳ�����λ", (cmbDataBits.SelectedItem as ComboItem).Text);
            commonDAO.SetAppletConfig("���ӳ�ֹͣλ", (cmbParity.SelectedItem as ComboItem).Text);

            //���ӳ���С����
            commonDAO.SetAppletConfig("���ӳ���С����", dInputLibraWeight.Value.ToString());
            //�Ƿ����ó���
            commonDAO.SetAppletConfig("���ó���", (chkIsUseWeight.Checked ? 1 : 0).ToString());

            //����
            commonDAO.SetCommonAppletConfig("0.2mm������", db2Weight.Value.ToString());
            commonDAO.SetCommonAppletConfig("3mm������", db3Weight.Value.ToString());
            commonDAO.SetCommonAppletConfig("6mm������", db6Weight.Value.ToString());

            commonDAO.SetCommonAppletConfig("0.2mm������", db2OverWeight.Value.ToString());
            commonDAO.SetCommonAppletConfig("3mm������", db3OverWeight.Value.ToString());
            commonDAO.SetCommonAppletConfig("6mm������", db6OverWeight.Value.ToString());


            //������
            commonDAO.SetAppletConfig("������IP", iptxtIP.Value);
            commonDAO.SetAppletConfig("�������˿�", txtPort.Text);
            commonDAO.SetAppletConfig("����������", (cmbSecNumber.SelectedItem as ComboItem).Text);
            commonDAO.SetAppletConfig("����������", (cmbBlockNumber.SelectedItem as ComboItem).Text);

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}