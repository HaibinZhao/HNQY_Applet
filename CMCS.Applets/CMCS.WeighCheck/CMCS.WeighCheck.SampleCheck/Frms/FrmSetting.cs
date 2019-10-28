using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.WeighCheck.SampleCheck.Frms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors;

namespace CMCS.WeighCheck.SampleCheck.Frms
{
    public partial class FrmSetting : DevComponents.DotNetBar.Metro.MetroForm
    {
        CommonDAO commonDAO = CommonDAO.GetInstance();

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

                txtCommonAppConfig.Text = CommonAppConfig.GetInstance().AppIdentifier;

                //���ӳ�
                SelectedComboItem("COM" + commonDAO.GetAppletConfigInt32("���ӳӴ���"), cmbLibra_COM);
                SelectedComboItem(commonDAO.GetAppletConfigString("���ӳӲ�����"), cmbLibra_Bandrate);
                SelectedComboItem(commonDAO.GetAppletConfigString("���ӳ�����λ"), cmbDataBits);
                SelectedComboItem(commonDAO.GetAppletConfigString("���ӳ�ֹͣλ"), cmbParity);
                // ������
                //SelectedComboItem("COM" + commonDAO.GetAppletConfigInt32("Read_Write_COM"), cmbRead_Write_COM);
                //SelectedComboItem(commonDAO.GetAppletConfigString("Read_Write_Bandrate"), cmbRead_Write_Bandrate);
                //���ӳ���С����
                dInputLibraWeight.Value = commonDAO.GetAppletConfigDouble("���ӳ���С����");
                //�Ƿ����ó���
                chkIsUseWeight.Checked = Convert.ToBoolean(commonDAO.GetAppletConfigInt32("���ó���"));

                // ȫ�ֲ���
                Old_Param = (cmbLibra_COM.SelectedIndex + 1).ToString() + (cmbLibra_Bandrate.SelectedItem as ComboItem).Text
                    + (cmbDataBits.SelectedItem as ComboItem).Text + (cmbParity.SelectedItem as ComboItem).Text
                    + dInputLibraWeight.Value.ToString() + chkIsUseWeight.Checked.ToString();
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
            //���ӳ�
            commonDAO.SetAppletConfig("���ӳӴ���", (cmbLibra_COM.SelectedIndex + 1).ToString());
            commonDAO.SetAppletConfig("���ӳӲ�����", (cmbLibra_Bandrate.SelectedItem as ComboItem).Text);
            commonDAO.SetAppletConfig("���ӳ�����λ", (cmbDataBits.SelectedItem as ComboItem).Text);
            commonDAO.SetAppletConfig("���ӳ�ֹͣλ", (cmbParity.SelectedItem as ComboItem).Text);
            //������
            //commonDAO.SetAppletConfig("Read_Write_COM", (cmbRead_Write_COM.SelectedIndex + 1).ToString());
            //commonDAO.SetAppletConfig("Read_Write_Bandrate", (cmbRead_Write_Bandrate.SelectedItem as ComboItem).Text);
            //���ӳ���С����
            commonDAO.SetAppletConfig("���ӳ���С����", dInputLibraWeight.Value.ToString());
            //�Ƿ����ó���
            commonDAO.SetAppletConfig("���ó���", (chkIsUseWeight.Checked ? 1 : 0).ToString());

            // �رճ������³�ʼ���豸
            if (Old_Param != (cmbLibra_COM.SelectedIndex + 1).ToString() + (cmbLibra_Bandrate.SelectedItem as ComboItem).Text
                + (cmbDataBits.SelectedItem as ComboItem).Text + (cmbParity.SelectedItem as ComboItem).Text
                    + dInputLibraWeight.Value.ToString() + chkIsUseWeight.Checked.ToString())
                Application.Restart();
            else
                this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}