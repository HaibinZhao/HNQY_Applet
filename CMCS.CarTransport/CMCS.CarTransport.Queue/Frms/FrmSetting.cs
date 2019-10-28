using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using System.IO.Ports;
using CMCS.Common.DAO;
using CMCS.Common;
using CMCS.DapperDber.Dbs.OracleDb;
using CMCS.Common.Utilities;
using CMCS.CarTransport.Queue.Core;

namespace CMCS.CarTransport.Queue.Frms
{
    public partial class FrmSetting : DevComponents.DotNetBar.Metro.MetroForm
    {
        CommonDAO commonDAO = CommonDAO.GetInstance();

        CommonAppConfig commonAppConfig = CommonAppConfig.GetInstance();

        public FrmSetting()
        {
            InitializeComponent();
        }

        void InitForm()
        {
            InitComPortComboBoxs(cmbRwer2Com);
            InitNumberAscComboBoxs(1, 15, cmbInductorCoil1Port, cmbInductorCoil2Port, cmbInductorCoil3Port, cmbInductorCoil4Port, cmbGate1UpPort, cmbGate1DownPort, cmbGate2UpPort, cmbGate2DownPort, cmbSignalLight1Port, cmbSignalLight2Port);
            InitNumberAscComboBox(1, 30, cmbRwerPower);
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {

        }

        private void FrmSetting_Shown(object sender, EventArgs e)
        {
            InitForm();

            LoadAppConfig();
        }

        /// <summary>
        /// �������ݿ�����
        /// </summary>
        /// <returns></returns>
        private bool TestDBConnect()
        {
            if (string.IsNullOrEmpty(txtSelfConnStr.Text.Trim()))
            {
                MessageBoxEx.Show("�����������ݿ������ַ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            try
            {
                OracleDapperDber dber = new OracleDapperDber(txtSelfConnStr.Text.Trim());
                using (System.Data.OracleClient.OracleConnection conn = dber.CreateConnection() as System.Data.OracleClient.OracleConnection)
                {
                    conn.Open();
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("���ݿ�����ʧ�ܣ�" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        void LoadAppConfig()
        {
            txtAppIdentifier.Text = commonAppConfig.AppIdentifier;
            txtSelfConnStr.Text = commonAppConfig.SelfConnStr;
            chbStartup.Checked = (commonDAO.GetAppletConfigString("��������") == "1");
            dbtxtProfinWeight.Value = commonDAO.GetAppletConfigDouble("ӯ�쳣����");
            dbtxtProfinWeight2.Value = commonDAO.GetAppletConfigDouble("���쳣����");
            // IO������
            iptxtIOIP.Value = commonDAO.GetAppletConfigString("IO������_IP��ַ");
            txtIOPort.Text = commonDAO.GetAppletConfigString("IO������_�˿�");
            SelectedComboBoxItem(cmbInductorCoil1Port, commonDAO.GetAppletConfigString("IO������_�ظ�1�˿�"));
            SelectedComboBoxItem(cmbInductorCoil2Port, commonDAO.GetAppletConfigString("IO������_�ظ�2�˿�"));
            SelectedComboBoxItem(cmbInductorCoil3Port, commonDAO.GetAppletConfigString("IO������_�ظ�3�˿�"));
            SelectedComboBoxItem(cmbInductorCoil4Port, commonDAO.GetAppletConfigString("IO������_�ظ�4�˿�"));
            SelectedComboBoxItem(cmbGate1UpPort, commonDAO.GetAppletConfigString("IO������_��բ1���˶˿�"));
            SelectedComboBoxItem(cmbGate1DownPort, commonDAO.GetAppletConfigString("IO������_��բ1���˶˿�"));
            SelectedComboBoxItem(cmbGate2UpPort, commonDAO.GetAppletConfigString("IO������_��բ2���˶˿�"));
            SelectedComboBoxItem(cmbGate2DownPort, commonDAO.GetAppletConfigString("IO������_��բ2���˶˿�"));
            SelectedComboBoxItem(cmbSignalLight1Port, commonDAO.GetAppletConfigString("IO������_�źŵ�1�˿�"));
            SelectedComboBoxItem(cmbSignalLight2Port, commonDAO.GetAppletConfigString("IO������_�źŵ�2�˿�"));

            // LED��ʾ��
            iptxtLED1IP.Value = commonDAO.GetAppletConfigString("LED��ʾ��1_IP��ַ");

            // ����������
            ipAddressInput1.Value = commonDAO.GetAppletConfigString("������1IP��ַ");
            ipAddressPort1.Text = commonDAO.GetAppletConfigString("������1�˿�");
            txtRwerTagStartWith.Text = commonDAO.GetAppletConfigString("������_��ǩ����");
            SelectedComboBoxItem(cmbRwerPower, commonDAO.GetAppletConfigString("������1����"));

            // ������
            SelectedComboBoxItem(cmbRwer2Com, commonDAO.GetAppletConfigString("������_����"));

        }

        /// <summary>
        /// ��������
        /// </summary>
        bool SaveAppConfig()
        {
            if (!TestDBConnect()) return false;

            commonAppConfig.AppIdentifier = txtAppIdentifier.Text.Trim();
            commonAppConfig.SelfConnStr = txtSelfConnStr.Text;
            commonAppConfig.Save();
            commonDAO.SetAppletConfig("��������", Convert.ToInt16(chbStartup.Checked).ToString());
            commonDAO.SetAppletConfig("ӯ�쳣����", dbtxtProfinWeight.Value.ToString());
            commonDAO.SetAppletConfig("���쳣����", dbtxtProfinWeight2.Value.ToString());
            try
            {
#if DEBUG

#else
                // ��ӡ�ȡ����������
                if (chbStartup.Checked)
                    StartUpUtil.InsertStartUp(Application.ProductName, Application.ExecutablePath);
                else
                    StartUpUtil.DeleteStartUp(Application.ProductName);
#endif
            }
            catch { }

            // IO������
            commonDAO.SetAppletConfig("IO������_IP��ַ", iptxtIOIP.Value);
            commonDAO.SetAppletConfig("IO������_�˿�", txtIOPort.Text);
            commonDAO.SetAppletConfig("IO������_�ظ�1�˿�", (cmbInductorCoil1Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_�ظ�2�˿�", (cmbInductorCoil2Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_�ظ�3�˿�", (cmbInductorCoil3Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_�ظ�4�˿�", (cmbInductorCoil4Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��բ1���˶˿�", (cmbGate1UpPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��բ1���˶˿�", (cmbGate1DownPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��բ2���˶˿�", (cmbGate2UpPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��բ2���˶˿�", (cmbGate2DownPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_�źŵ�1�˿�", (cmbSignalLight1Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_�źŵ�2�˿�", (cmbSignalLight2Port.SelectedItem as DataItem).Value);

            // LED��ʾ��
            commonDAO.SetAppletConfig("LED��ʾ��1_IP��ַ", iptxtLED1IP.Value);

            // ����������
            commonDAO.SetAppletConfig("������1IP��ַ", ipAddressInput1.Value);
            commonDAO.SetAppletConfig("������1�˿�", ipAddressPort1.Text);
            commonDAO.SetAppletConfig("������_��ǩ����", txtRwerTagStartWith.Text);
            commonDAO.SetAppletConfig("������1����", (cmbRwerPower.SelectedItem as DataItem).Value);

            // ������
            commonDAO.SetAppletConfig("������_����", (cmbRwer2Com.SelectedItem as DataItem).Value);


            return true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!ValidateInputEmpty(new List<string> { "����Ψһ��ʶ��", "���ݿ������ַ���" }, new List<Control> { txtAppIdentifier, txtSelfConnStr })) return;

            if (!SaveAppConfig()) return;

            if (MessageBoxEx.Show("���ĵ�������Ҫ�������������Ч���Ƿ�����������", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Restart();
            else
                this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region ��������

        /// <summary>
        /// ��֤�����ؼ�Ϊ�գ�����ʾ
        /// </summary>
        /// <param name="tipsNames"></param>
        /// <param name="controls"></param>
        /// <returns></returns>
        public static bool ValidateInputEmpty(List<string> tipsNames, List<Control> controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                Control control = controls[i];

                if (control is TextBoxX && string.IsNullOrEmpty(((TextBoxX)control).Text))
                {
                    control.Focus();
                    MessageBoxEx.Show("������" + tipsNames[i] + "��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// ѡ��������ѡ��
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="text"></param>
        private void SelectedComboBoxItem(ComboBoxEx cmb, string value)
        {
            foreach (DataItem dataItem in cmb.Items)
            {
                if (dataItem.Value == value) cmb.SelectedItem = dataItem;
            }
        }

        /// <summary>
        /// ��ʼ������������
        /// </summary>
        /// <param name="cmb"></param>
        void InitComPortComboBox(ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            for (int i = 1; i < 20; i++)
            {
                cmb.Items.Add(new DataItem("COM" + i.ToString(), i.ToString()));
            }

            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// ��ʼ������������
        /// </summary>
        /// <param name="cmbs"></param>
        void InitComPortComboBoxs(params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitComPortComboBox(cmb);
            }
        }

        /// <summary>
        /// ��ʼ��������������
        /// </summary>
        /// <param name="cmb"></param>
        private void InitBandrateComboBox(ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            cmb.Items.Add(new DataItem("1200"));
            cmb.Items.Add(new DataItem("4800"));
            cmb.Items.Add(new DataItem("9600"));
            cmb.Items.Add(new DataItem("14400"));
            cmb.Items.Add(new DataItem("19200"));
            cmb.Items.Add(new DataItem("38400"));
            cmb.Items.Add(new DataItem("56000"));
            cmb.Items.Add(new DataItem("57600"));
            cmb.Items.Add(new DataItem("115200"));

            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// ��ʼ��������������
        /// </summary>
        /// <param name="cmbs"></param>
        void InitBandrateComboBoxs(params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitBandrateComboBox(cmb);
            }
        }

        /// <summary>
        /// ��ʼ������������
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void InitNumberAscComboBox(int start, int end, ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            for (int i = start; i <= end; i++)
            {
                cmb.Items.Add(new DataItem(i.ToString()));
            }

            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// ��ʼ������������
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void InitNumberAscComboBoxs(int start, int end, params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitNumberAscComboBox(start, end, cmb);
            }
        }

        /// <summary>
        /// ��ʼ��ֹͣλ������
        /// </summary>
        /// <param name="cmb"></param>
        void InitStopBitsComboBox(ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            cmb.Items.Add(new DataItem(StopBits.None.ToString(), ((int)StopBits.None).ToString()));
            cmb.Items.Add(new DataItem(StopBits.One.ToString(), ((int)StopBits.One).ToString()));
            cmb.Items.Add(new DataItem(StopBits.OnePointFive.ToString(), ((int)StopBits.OnePointFive).ToString()));
            cmb.Items.Add(new DataItem(StopBits.Two.ToString(), ((int)StopBits.Two).ToString()));

            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// ��ʼ��ֹͣλ������
        /// </summary>
        /// <param name="cmbs"></param>
        void InitStopBitsComboBoxs(params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitStopBitsComboBox(cmb);
            }
        }

        /// <summary>
        /// ��ʼ��У��λ������
        /// </summary>
        /// <param name="cmb"></param>
        void InitParityComboBox(ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            cmb.Items.Add(new DataItem(Parity.None.ToString(), ((int)Parity.None).ToString()));
            cmb.Items.Add(new DataItem(Parity.Odd.ToString(), ((int)Parity.Odd).ToString()));
            cmb.Items.Add(new DataItem(Parity.Even.ToString(), ((int)Parity.Even).ToString()));
            cmb.Items.Add(new DataItem(Parity.Mark.ToString(), ((int)Parity.Mark).ToString()));
            cmb.Items.Add(new DataItem(Parity.Space.ToString(), ((int)Parity.Space).ToString()));

            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// ��ʼ��У��λ������
        /// </summary>
        /// <param name="cmbs"></param>
        void InitParityComboBoxs(params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitParityComboBox(cmb);
            }
        }

        #endregion
    }
}