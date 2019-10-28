using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using CMCS.CarTransport.Weighter.Core;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Utilities;
using CMCS.DapperDber.Dbs.OracleDb;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.CarTransport;

namespace CMCS.CarTransport.Weighter.Frms
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
            InitComPortComboBoxs(cmbWberCom);
            InitBandrateComboBoxs(cmbWberBandrate);
            InitNumberAscComboBoxs(5, 8, cmbWberDataBits);
            InitNumberAscComboBoxs(1, 15, cmbInductorCoil1Port, cmbInductorCoil2Port, cmbInductorCoil3Port, cmbInductorCoil4Port, cmbInfraredSensor1Port, cmbInfraredSensor2Port, cmbGate1UpPort, cmbGate1DownPort, cmbGate2UpPort, cmbGate2DownPort, cmbSignalLight1Port, cmbSignalLight2Port, cmbButtonSensorPort);
            InitNumberAscComboBox(1, 30, cmbRwerPower);
            InitStopBitsComboBoxs(cmbWberStopBits);
            InitParityComboBoxs(cmbWberParity);
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
            chkIsNormal.Checked = commonDAO.GetCommonAppletConfigString("�Ƿ�PDAȷ��") == "1";
            // IO������
            iptxtIOIP.Value = commonDAO.GetAppletConfigString("IO������_IP��ַ");
            txtIOPort.Text = commonDAO.GetAppletConfigString("IO������_�˿�");
            SelectedComboBoxItem(cmbInductorCoil1Port, commonDAO.GetAppletConfigString("IO������_�ظ�1�˿�"));
            SelectedComboBoxItem(cmbInductorCoil2Port, commonDAO.GetAppletConfigString("IO������_�ظ�2�˿�"));
            SelectedComboBoxItem(cmbInductorCoil3Port, commonDAO.GetAppletConfigString("IO������_�ظ�3�˿�"));
            SelectedComboBoxItem(cmbInductorCoil4Port, commonDAO.GetAppletConfigString("IO������_�ظ�4�˿�"));
            SelectedComboBoxItem(cmbInfraredSensor1Port, commonDAO.GetAppletConfigString("IO������_����1�˿�"));
            SelectedComboBoxItem(cmbInfraredSensor2Port, commonDAO.GetAppletConfigString("IO������_����2�˿�"));
            SelectedComboBoxItem(cmbGate1UpPort, commonDAO.GetAppletConfigString("IO������_��բ1���˶˿�"));
            SelectedComboBoxItem(cmbGate1DownPort, commonDAO.GetAppletConfigString("IO������_��բ1���˶˿�"));
            SelectedComboBoxItem(cmbGate2UpPort, commonDAO.GetAppletConfigString("IO������_��բ2���˶˿�"));
            SelectedComboBoxItem(cmbGate2DownPort, commonDAO.GetAppletConfigString("IO������_��բ2���˶˿�"));
            SelectedComboBoxItem(cmbSignalLight1Port, commonDAO.GetAppletConfigString("IO������_�źŵ�1�˿�"));
            SelectedComboBoxItem(cmbSignalLight2Port, commonDAO.GetAppletConfigString("IO������_�źŵ�2�˿�"));
            SelectedComboBoxItem(cmbButtonSensorPort, commonDAO.GetAppletConfigString("IO������_��ť�˿�"));

            // �ذ��Ǳ�
            SelectedComboBoxItem(cmbWberCom, commonDAO.GetAppletConfigString("�ذ��Ǳ�_����"));
            SelectedComboBoxItem(cmbWberBandrate, commonDAO.GetAppletConfigString("�ذ��Ǳ�_������"));
            SelectedComboBoxItem(cmbWberDataBits, commonDAO.GetAppletConfigString("�ذ��Ǳ�_����λ"));
            SelectedComboBoxItem(cmbWberStopBits, commonDAO.GetAppletConfigString("�ذ��Ǳ�_ֹͣλ"));
            SelectedComboBoxItem(cmbWberParity, commonDAO.GetAppletConfigString("�ذ��Ǳ�_У��λ"));
            dbtxtMinWeight.Value = commonDAO.GetAppletConfigDouble("�ذ��Ǳ�_��С����");

            // LED��ʾ��
            iptxtLED1IP.Value = commonDAO.GetAppletConfigString("LED��ʾ��1_IP��ַ");

            // ����������
            iptxtRwerIP.Value = commonDAO.GetAppletConfigString("������1_IP��ַ");
            txtRwerPort.Text = commonDAO.GetAppletConfigString("������1_�˿�");
            txtRwerTagStartWith.Text = commonDAO.GetAppletConfigString("������_��ǩ����");
            SelectedComboBoxItem(cmbRwerPower, commonDAO.GetAppletConfigString("������1_����"));

            //�����
            //chbUseCamera.Checked = commonDAO.GetAppletConfigString("���������") == "1";
            //CmcsCamare video1 = commonDAO.SelfDber.Entity<CmcsCamare>("where EquipmentCode=:EquipmentCode", new { EquipmentCode = commonAppConfig.AppIdentifier + "����ͷ1" });
            //if (video1 != null)
            //{
            //    iptxtCamera1IP.Value = video1.Ip;
            //    txtCamera1Port.Text = video1.Port.ToString();
            //    txtCamera1UserName.Text = video1.UserName;
            //    txtCamera1Pwd.Text = video1.Password;
            //    txtCamera1Channel.Text = video1.Channel.ToString();
            //}

            //CmcsCamare video2 = commonDAO.SelfDber.Entity<CmcsCamare>("where EquipmentCode=:EquipmentCode", new { EquipmentCode = commonAppConfig.AppIdentifier + "����ͷ2" });
            //if (video2 != null)
            //{
            //    iptxtCamera2IP.Value = video2.Ip;
            //    txtCamera2Port.Text = video2.Port.ToString();
            //    txtCamera2UserName.Text = video2.UserName;
            //    txtCamera2Pwd.Text = video2.Password;
            //    txtCamera2Channel.Text = video2.Channel.ToString();
            //}
            //жú��LED
            iptxtUnloadLED1.Value = commonDAO.GetCommonAppletConfigString("жú��LED��ʾ��1_IP��ַ");
            iptxtUnloadLED2.Value = commonDAO.GetCommonAppletConfigString("жú��LED��ʾ��2_IP��ַ");
            iptxtUnloadLED3.Value = commonDAO.GetCommonAppletConfigString("жú��LED��ʾ��3_IP��ַ");
            iptxtUnloadLED4.Value = commonDAO.GetCommonAppletConfigString("жú��LED��ʾ��4_IP��ַ");
            iptxtUnloadLED5.Value = commonDAO.GetCommonAppletConfigString("жú��LED��ʾ��5_IP��ַ");
            iptxtUnloadLED6.Value = commonDAO.GetCommonAppletConfigString("жú��LED��ʾ��6_IP��ַ");
            iptxtUnloadLED7.Value = commonDAO.GetCommonAppletConfigString("жú��LED��ʾ��7_IP��ַ");
            iptxtUnloadLED8.Value = commonDAO.GetCommonAppletConfigString("жú��LED��ʾ��8_IP��ַ");

            CmcsUnLoadLED unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=1");
            if (unLoadLed != null)
            {
                txtUnload1Number.Text = unLoadLed.UnLoadName;
                iptxtUnloadLED1.Value = unLoadLed.IP;
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=2");
            if (unLoadLed != null)
            {
                txtUnload2Number.Text = unLoadLed.UnLoadName;
                iptxtUnloadLED2.Value = unLoadLed.IP;
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=3");
            if (unLoadLed != null)
            {
                txtUnload3Number.Text = unLoadLed.UnLoadName;
                iptxtUnloadLED3.Value = unLoadLed.IP;
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=4");
            if (unLoadLed != null)
            {
                txtUnload4Number.Text = unLoadLed.UnLoadName;
                iptxtUnloadLED4.Value = unLoadLed.IP;
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=5");
            if (unLoadLed != null)
            {
                txtUnload5Number.Text = unLoadLed.UnLoadName;
                iptxtUnloadLED5.Value = unLoadLed.IP;
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=6");
            if (unLoadLed != null)
            {
                txtUnload6Number.Text = unLoadLed.UnLoadName;
                iptxtUnloadLED6.Value = unLoadLed.IP;
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=7");
            if (unLoadLed != null)
            {
                txtUnload7Number.Text = unLoadLed.UnLoadName;
                iptxtUnloadLED7.Value = unLoadLed.IP;
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=8");
            if (unLoadLed != null)
            {
                txtUnload8Number.Text = unLoadLed.UnLoadName;
                iptxtUnloadLED8.Value = unLoadLed.IP;
            }
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
            commonDAO.SetCommonAppletConfig("�Ƿ�PDAȷ��", chkIsNormal.Checked ? "1" : "0");
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
            commonDAO.SetAppletConfig("IO������_����1�˿�", (cmbInfraredSensor1Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_����2�˿�", (cmbInfraredSensor2Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��բ1���˶˿�", (cmbGate1UpPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��բ1���˶˿�", (cmbGate1DownPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��բ2���˶˿�", (cmbGate2UpPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��բ2���˶˿�", (cmbGate2DownPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_�źŵ�1�˿�", (cmbSignalLight1Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_�źŵ�2�˿�", (cmbSignalLight2Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO������_��ť�˿�", (cmbButtonSensorPort.SelectedItem as DataItem).Value);

            // �ذ��Ǳ�
            commonDAO.SetAppletConfig("�ذ��Ǳ�_����", (cmbWberCom.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("�ذ��Ǳ�_������", (cmbWberBandrate.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("�ذ��Ǳ�_����λ", (cmbWberDataBits.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("�ذ��Ǳ�_ֹͣλ", (cmbWberStopBits.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("�ذ��Ǳ�_У��λ", (cmbWberParity.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("�ذ��Ǳ�_��С����", dbtxtMinWeight.Value.ToString());

            // LED��ʾ��
            commonDAO.SetAppletConfig("LED��ʾ��1_IP��ַ", iptxtLED1IP.Value);

            // ����������
            commonDAO.SetAppletConfig("������1_IP��ַ", iptxtRwerIP.Value);
            commonDAO.SetAppletConfig("������1_�˿�", txtRwerPort.Text);
            commonDAO.SetAppletConfig("������_��ǩ����", txtRwerTagStartWith.Text);
            commonDAO.SetAppletConfig("������1_����", (cmbRwerPower.SelectedItem as DataItem).Value);

            //����� 
            //commonDAO.SetAppletConfig("���������", chbUseCamera.Checked ? "1" : "0");
            //CmcsCamare video1 = commonDAO.SelfDber.Entity<CmcsCamare>("where EquipmentCode=:EquipmentCode", new { EquipmentCode = commonAppConfig.AppIdentifier + "����ͷ1" });
            //if (video1 != null)
            //{
            //    video1.Ip = iptxtCamera1IP.Value;
            //    video1.Port = Convert.ToInt32(txtCamera1Port.Text);
            //    video1.UserName = txtCamera1UserName.Text;
            //    video1.Password = txtCamera1Pwd.Text;
            //    video1.Channel = Convert.ToInt32(txtCamera1Channel.Text);
            //    commonDAO.SelfDber.Update(video1);
            //}
            //else
            //{
            //    video1 = new CmcsCamare();
            //    video1.Ip = iptxtCamera1IP.Value;
            //    video1.Port = Convert.ToInt32(txtCamera1Port.Text);
            //    video1.UserName = txtCamera1UserName.Text;
            //    video1.Password = txtCamera1Pwd.Text;
            //    video1.Channel = Convert.ToInt32(txtCamera1Channel.Text);
            //    video1.ParentId = "-1";
            //    commonDAO.SelfDber.Insert(video1);
            //}
            //CmcsCamare video2 = commonDAO.SelfDber.Entity<CmcsCamare>("where EquipmentCode=:EquipmentCode", new { EquipmentCode = commonAppConfig.AppIdentifier + "����ͷ2" });
            //if (video2 != null)
            //{
            //    video2.Ip = iptxtCamera2IP.Value;
            //    video2.Port = Convert.ToInt32(txtCamera2Port.Text);
            //    video2.UserName = txtCamera2UserName.Text;
            //    video2.Password = txtCamera2Pwd.Text;
            //    video2.Channel = Convert.ToInt32(txtCamera2Channel.Text);
            //    commonDAO.SelfDber.Update(video2);
            //}
            //else
            //{
            //    video2 = new CmcsCamare();
            //    video2.Ip = iptxtCamera2IP.Value;
            //    video2.Port = Convert.ToInt32(txtCamera2Port.Text);
            //    video2.UserName = txtCamera2UserName.Text;
            //    video2.Password = txtCamera2Pwd.Text;
            //    video2.Channel = Convert.ToInt32(txtCamera2Channel.Text);
            //    video1.ParentId = "-1";
            //    commonDAO.SelfDber.Insert(video2);
            //}

            //жú��LED
            //commonDAO.SetCommonAppletConfig("жú��LED��ʾ��1_IP��ַ", iptxtUnloadLED1.Value);
            //commonDAO.SetCommonAppletConfig("жú��LED��ʾ��2_IP��ַ", iptxtUnloadLED2.Value);
            //commonDAO.SetCommonAppletConfig("жú��LED��ʾ��3_IP��ַ", iptxtUnloadLED3.Value);
            //commonDAO.SetCommonAppletConfig("жú��LED��ʾ��4_IP��ַ", iptxtUnloadLED4.Value);
            //commonDAO.SetCommonAppletConfig("жú��LED��ʾ��5_IP��ַ", iptxtUnloadLED5.Value);
            //commonDAO.SetCommonAppletConfig("жú��LED��ʾ��6_IP��ַ", iptxtUnloadLED6.Value);
            //commonDAO.SetCommonAppletConfig("жú��LED��ʾ��7_IP��ַ", iptxtUnloadLED7.Value);
            //commonDAO.SetCommonAppletConfig("жú��LED��ʾ��8_IP��ַ", iptxtUnloadLED8.Value);

            CmcsUnLoadLED unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=1");
            if (unLoadLed != null)
            {
                unLoadLed.UnLoadName = txtUnload1Number.Text;
                unLoadLed.IP = iptxtUnloadLED1.Value;
                commonDAO.SelfDber.Update(unLoadLed);
            }
            else
            {
                unLoadLed = new CmcsUnLoadLED();
                unLoadLed.UnLoadNumber = 1;
                unLoadLed.UnLoadName = txtUnload1Number.Text;
                unLoadLed.IP = iptxtUnloadLED1.Value;
                unLoadLed.IsUse = 0;
                commonDAO.SelfDber.Insert(unLoadLed);
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=2");
            if (unLoadLed != null)
            {
                unLoadLed.UnLoadName = txtUnload2Number.Text;
                unLoadLed.IP = iptxtUnloadLED2.Value;
                commonDAO.SelfDber.Update(unLoadLed);
            }
            else
            {
                unLoadLed = new CmcsUnLoadLED();
                unLoadLed.UnLoadNumber = 2;
                unLoadLed.UnLoadName = txtUnload2Number.Text;
                unLoadLed.IP = iptxtUnloadLED2.Value;
                unLoadLed.IsUse = 0;
                commonDAO.SelfDber.Insert(unLoadLed);
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=3");
            if (unLoadLed != null)
            {
                unLoadLed.UnLoadName = txtUnload3Number.Text;
                unLoadLed.IP = iptxtUnloadLED3.Value;
                commonDAO.SelfDber.Update(unLoadLed);
            }
            else
            {
                unLoadLed = new CmcsUnLoadLED();
                unLoadLed.UnLoadNumber = 3;
                unLoadLed.UnLoadName = txtUnload3Number.Text;
                unLoadLed.IP = iptxtUnloadLED3.Value;
                unLoadLed.IsUse = 0;
                commonDAO.SelfDber.Insert(unLoadLed);
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=4");
            if (unLoadLed != null)
            {
                unLoadLed.UnLoadName = txtUnload4Number.Text;
                unLoadLed.IP = iptxtUnloadLED4.Value;
                commonDAO.SelfDber.Update(unLoadLed);
            }
            else
            {
                unLoadLed = new CmcsUnLoadLED();
                unLoadLed.UnLoadNumber = 4;
                unLoadLed.UnLoadName = txtUnload4Number.Text;
                unLoadLed.IP = iptxtUnloadLED4.Value;
                unLoadLed.IsUse = 0;
                commonDAO.SelfDber.Insert(unLoadLed);
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=5");
            if (unLoadLed != null)
            {
                unLoadLed.UnLoadName = txtUnload5Number.Text;
                unLoadLed.IP = iptxtUnloadLED5.Value;
                commonDAO.SelfDber.Update(unLoadLed);
            }
            else
            {
                unLoadLed = new CmcsUnLoadLED();
                unLoadLed.UnLoadNumber = 5;
                unLoadLed.UnLoadName = txtUnload5Number.Text;
                unLoadLed.IP = iptxtUnloadLED5.Value;
                unLoadLed.IsUse = 0;
                commonDAO.SelfDber.Insert(unLoadLed);
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=6");
            if (unLoadLed != null)
            {
                unLoadLed.UnLoadName = txtUnload6Number.Text;
                unLoadLed.IP = iptxtUnloadLED6.Value;
                commonDAO.SelfDber.Update(unLoadLed);
            }
            else
            {
                unLoadLed = new CmcsUnLoadLED();
                unLoadLed.UnLoadNumber = 6;
                unLoadLed.UnLoadName = txtUnload6Number.Text;
                unLoadLed.IP = iptxtUnloadLED6.Value;
                unLoadLed.IsUse = 0;
                commonDAO.SelfDber.Insert(unLoadLed);
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=7");
            if (unLoadLed != null)
            {
                unLoadLed.UnLoadName = txtUnload7Number.Text;
                unLoadLed.IP = iptxtUnloadLED7.Value;
                commonDAO.SelfDber.Update(unLoadLed);
            }
            else
            {
                unLoadLed = new CmcsUnLoadLED();
                unLoadLed.UnLoadNumber = 7;
                unLoadLed.UnLoadName = txtUnload7Number.Text;
                unLoadLed.IP = iptxtUnloadLED7.Value;
                unLoadLed.IsUse = 0;
                commonDAO.SelfDber.Insert(unLoadLed);
            }

            unLoadLed = commonDAO.SelfDber.Entity<CmcsUnLoadLED>("where UnLoadNumber=8");
            if (unLoadLed != null)
            {
                unLoadLed.UnLoadName = txtUnload8Number.Text;
                unLoadLed.IP = iptxtUnloadLED8.Value;
                commonDAO.SelfDber.Update(unLoadLed);
            }
            else
            {
                unLoadLed = new CmcsUnLoadLED();
                unLoadLed.UnLoadNumber = 8;
                unLoadLed.UnLoadName = txtUnload8Number.Text;
                unLoadLed.IP = iptxtUnloadLED8.Value;
                unLoadLed.IsUse = 0;
                commonDAO.SelfDber.Insert(unLoadLed);
            }

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