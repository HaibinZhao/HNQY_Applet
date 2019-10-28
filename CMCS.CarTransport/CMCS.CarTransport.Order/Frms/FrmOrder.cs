using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.CarTransport.Order.Core;
using System.Threading;
using System.IO;
using CMCS.CarTransport.DAO;
using CMCS.Common.DAO;
using System.IO.Ports;
using CMCS.Common.Utilities;
using LED.YB14;
using CMCS.CarTransport.Order.Enums;
using CMCS.Common.Entities;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common;
using CMCS.CarTransport.Order.Frms.Sys;
using DevComponents.DotNetBar.Controls;
using CMCS.Common.Views;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.Common.Enums;
using CMCS.Common.Entities.Sys;

namespace CMCS.CarTransport.Order.Frms
{
    public partial class FrmOrder : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// ����Ψһ��ʶ��
        /// </summary>
        public static string UniqueKey = "FrmOrder";

        public FrmOrder()
        {
            InitializeComponent();
        }

        #region Vars

        CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();
        OrderDAO orderDAO = OrderDAO.GetInstance();
        CommonDAO commonDAO = CommonDAO.GetInstance();

        IocControler iocControler;
        /// <summary>
        /// ��������
        /// </summary>
        VoiceSpeaker voiceSpeaker = new VoiceSpeaker();

        bool inductorCoil1 = false;
        /// <summary>
        /// �ظ�1״̬ true=���ź�  false=���ź�
        /// </summary>
        public bool InductorCoil1
        {
            get
            {
                return inductorCoil1;
            }
            set
            {
                inductorCoil1 = value;

                panCurrentCarNumber.Refresh();

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.�ظ�1�ź�.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil1Port;
        /// <summary>
        /// �ظ�1�˿�
        /// </summary>
        public int InductorCoil1Port
        {
            get { return inductorCoil1Port; }
            set { inductorCoil1Port = value; }
        }

        bool inductorCoil2 = false;
        /// <summary>
        /// �ظ�2״̬ true=���ź�  false=���ź�
        /// </summary>
        public bool InductorCoil2
        {
            get
            {
                return inductorCoil2;
            }
            set
            {
                inductorCoil2 = value;

                panCurrentCarNumber.Refresh();

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.�ظ�2�ź�.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil2Port;
        /// <summary>
        /// �ظ�2�˿�
        /// </summary>
        public int InductorCoil2Port
        {
            get { return inductorCoil2Port; }
            set { inductorCoil2Port = value; }
        }

        bool inductorCoil3 = false;
        /// <summary>
        /// �ظ�3״̬ true=���ź�  false=���ź�
        /// </summary>
        public bool InductorCoil3
        {
            get
            {
                return inductorCoil3;
            }
            set
            {
                inductorCoil3 = value;

                panCurrentCarNumber.Refresh();

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.�ظ�3�ź�.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil3Port;
        /// <summary>
        /// �ظ�3�˿�
        /// </summary>
        public int InductorCoil3Port
        {
            get { return inductorCoil3Port; }
            set { inductorCoil3Port = value; }
        }

        bool inductorCoil4 = false;
        /// <summary>
        /// �ظ�4״̬ true=���ź�  false=���ź�
        /// </summary>
        public bool InductorCoil4
        {
            get
            {
                return inductorCoil4;
            }
            set
            {
                inductorCoil4 = value;

                panCurrentCarNumber.Refresh();

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.�ظ�4�ź�.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil4Port;
        /// <summary>
        /// �ظ�4�˿�
        /// </summary>
        public int InductorCoil4Port
        {
            get { return inductorCoil4Port; }
            set { inductorCoil4Port = value; }
        }

        bool inductorCoil5 = false;
        /// <summary>
        /// �ظ�5״̬ true=���ź�  false=���ź�
        /// </summary>
        public bool InductorCoil5
        {
            get
            {
                return inductorCoil5;
            }
            set
            {
                inductorCoil5 = value;

                panCurrentCarNumber.Refresh();

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.�ظ�5�ź�.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil5Port;
        /// <summary>
        /// �ظ�5�˿�
        /// </summary>
        public int InductorCoil5Port
        {
            get { return inductorCoil5Port; }
            set { inductorCoil5Port = value; }
        }

        bool inductorCoil6 = false;
        /// <summary>
        /// �ظ�6״̬ true=���ź�  false=���ź�
        /// </summary>
        public bool InductorCoil6
        {
            get
            {
                return inductorCoil6;
            }
            set
            {
                inductorCoil6 = value;

                panCurrentCarNumber.Refresh();

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.�ظ�6�ź�.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil6Port;
        /// <summary>
        /// �ظ�6�˿�
        /// </summary>
        public int InductorCoil6Port
        {
            get { return inductorCoil6Port; }
            set { inductorCoil6Port = value; }
        }


        bool inductorCoil7 = false;
        /// <summary>
        /// �ظ�7״̬ true=���ź�  false=���ź�
        /// </summary>
        public bool InductorCoil7
        {
            get
            {
                return inductorCoil7;
            }
            set
            {
                inductorCoil7 = value;

                panCurrentCarNumber.Refresh();

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.�ظ�7�ź�.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil7Port;
        /// <summary>
        /// �ظ�7�˿�
        /// </summary>
        public int InductorCoil7Port
        {
            get { return inductorCoil7Port; }
            set { inductorCoil7Port = value; }
        }


        bool inductorCoil8 = false;
        /// <summary>
        /// �ظ�8״̬ true=���ź�  false=���ź�
        /// </summary>
        public bool InductorCoil8
        {
            get
            {
                return inductorCoil8;
            }
            set
            {
                inductorCoil8 = value;

                panCurrentCarNumber.Refresh();

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.�ظ�8�ź�.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil8Port;
        /// <summary>
        /// �ظ�8�˿�
        /// </summary>
        public int InductorCoil8Port
        {
            get { return inductorCoil8Port; }
            set { inductorCoil8Port = value; }
        }

        bool autoHandMode = true;
        /// <summary>
        /// �Զ�ģʽ=true  �ֶ�ģʽ=false
        /// </summary>
        public bool AutoHandMode
        {
            get { return autoHandMode; }
            set
            {
                autoHandMode = value;

                btnSelectAutotruck_SaleFuel1.Visible = !value;

                btnSaveTransport_SaleFuel1.Visible = !value;

                btnReset_SaleFuel1.Visible = !value;

                btnSelectAutotruck_SaleFuel2.Visible = !value;

                btnSaveTransport_SaleFuel2.Visible = !value;

                btnReset_SaleFuel2.Visible = !value;
            }
        }

        public static PassCarQueuer passCarQueuer1 = new PassCarQueuer();
        public static PassCarQueuer passCarQueuer2 = new PassCarQueuer();

        ImperfectCar currentImperfectCar1;
        /// <summary>
        /// ʶ���ѡ��ĳ���ƾ֤
        /// </summary>
        public ImperfectCar CurrentImperfectCar1
        {
            get { return currentImperfectCar1; }
            set
            {
                currentImperfectCar1 = value;

                if (value != null)
                    panCurrentCarNumber.Text = value.Voucher;
                else
                    panCurrentCarNumber.Text = "�ȴ�����";
            }
        }

        ImperfectCar currentImperfectCar2;
        /// <summary>
        /// ʶ���ѡ��ĳ���ƾ֤
        /// </summary>
        public ImperfectCar CurrentImperfectCar2
        {
            get { return currentImperfectCar2; }
            set
            {
                currentImperfectCar2 = value;

                if (value != null)
                    panCurrentCarNumber.Text = value.Voucher;
                else
                    panCurrentCarNumber.Text = "�ȴ�����";
            }
        }

        eFlowFlag currentFlowFlag1 = eFlowFlag.�ȴ�����;
        eFlowFlag currentFlowFlag2 = eFlowFlag.�ȴ�����;
        /// <summary>
        /// ��ǰҵ�����̱�ʶ
        /// </summary>
        public eFlowFlag CurrentFlowFlag1
        {
            get { return currentFlowFlag1; }
            set
            {
                currentFlowFlag1 = value;

                lblFlowFlag.Text = value.ToString() + "|" + lblFlowFlag.Text.Split('|')[1];
            }
        }
        /// <summary>
        /// ��ǰҵ�����̱�ʶ
        /// </summary>
        public eFlowFlag CurrentFlowFlag2
        {
            get { return currentFlowFlag2; }
            set
            {
                currentFlowFlag2 = value;

                lblFlowFlag.Text = lblFlowFlag.Text.Split('|')[0] + "|" + value.ToString();
            }
        }

        CmcsAutotruck currentAutotruck1;
        /// <summary>
        /// ��ǰ��
        /// </summary>
        public CmcsAutotruck CurrentAutotruck1
        {
            get { return currentAutotruck1; }
            set
            {
                currentAutotruck1 = value;

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ��Id.ToString() + 1, value.Id);
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ����.ToString() + 1, value.CarNumber);

                    CmcsEPCCard ePCCard = Dbers.GetInstance().SelfDber.Get<CmcsEPCCard>(value.EPCCardId);
                    if (value.CarType == eCarType.����ú.ToString())
                    {
                        if (ePCCard != null) txtTagId_SaleFuel1.Text = ePCCard.TagId;

                        txtCarNumber_SaleFuel1.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_SaleFuel1;
                    }


                    panCurrentCarNumber.Text = value.CarNumber;


                }
                else
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ��Id.ToString() + 1, string.Empty);
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ����.ToString() + 1, string.Empty);

                    txtCarNumber_SaleFuel1.ResetText();

                    txtTagId_SaleFuel1.ResetText();

                    panCurrentCarNumber.ResetText();
                }
            }
        }


        CmcsAutotruck currentAutotruck2;
        /// <summary>
        /// ��ǰ��
        /// </summary>
        public CmcsAutotruck CurrentAutotruck2
        {
            get { return currentAutotruck2; }
            set
            {
                currentAutotruck2 = value;

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ��Id.ToString() + 2, value.Id);
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ����.ToString() + 2, value.CarNumber);

                    CmcsEPCCard ePCCard = Dbers.GetInstance().SelfDber.Get<CmcsEPCCard>(value.EPCCardId);
                    if (value.CarType == eCarType.����ú.ToString())
                    {
                        if (ePCCard != null) txtTagId_SaleFuel2.Text = ePCCard.TagId;

                        txtCarNumber_SaleFuel2.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_SaleFuel2;
                    }


                    panCurrentCarNumber.Text = value.CarNumber;


                }
                else
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ��Id.ToString() + 2, string.Empty);
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ����.ToString() + 2, string.Empty);

                    txtCarNumber_SaleFuel2.ResetText();

                    txtTagId_SaleFuel2.ResetText();

                    panCurrentCarNumber.ResetText();
                }
            }
        }
        #endregion

        /// <summary>
        /// �����ʼ��
        /// </summary>
        private void InitForm()
        {
#if DEBUG
            lblFlowFlag.Visible = true;
            FrmDebugConsole.GetInstance().Show();
#else
            //lblFlowFlag.Visible = false;
#endif

            // Ĭ���Զ�
            sbtnChangeAutoHandMode.Value = true;

            // ���ó���Զ�̿�������
            commonDAO.ResetAppRemoteControlCmd(CommonAppConfig.GetInstance().AppIdentifier);
        }

        private void FrmWeighter_Load(object sender, EventArgs e)
        {
        }

        private void FrmWeighter_Shown(object sender, EventArgs e)
        {
            InitHardware();

            InitForm();
        }


        #region �豸���

        #region IO������

        void Iocer_StatusChange(bool status)
        {
            // �����豸״̬ 
            InvokeEx(() =>
            {
                slightIOC.LightColor = (status ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.IO������_����״̬.ToString(), status ? "1" : "0");
            });
        }

        /// <summary>
        /// IO��������������ʱ����
        /// </summary>
        /// <param name="receiveValue"></param>
        void Iocer_Received(int[] receiveValue)
        {
            // ���յظ�״̬  
            InvokeEx(() =>
            {
                this.InductorCoil1 = (receiveValue[this.InductorCoil1Port - 1] == 1);
                this.InductorCoil2 = (receiveValue[this.InductorCoil2Port - 1] == 1);
                this.InductorCoil3 = (receiveValue[this.InductorCoil3Port - 1] == 1);
                this.InductorCoil4 = (receiveValue[this.InductorCoil4Port - 1] == 1);
                this.InductorCoil5 = (receiveValue[this.InductorCoil5Port - 1] == 1);
                this.InductorCoil6 = (receiveValue[this.InductorCoil6Port - 1] == 1);
                this.InductorCoil7 = (receiveValue[this.InductorCoil7Port - 1] == 1);
                this.InductorCoil8 = (receiveValue[this.InductorCoil8Port - 1] == 1);
            });
        }

        /// <summary>
        /// ��1����
        /// </summary>
        void BackGateUp1()
        {
            if (this.CurrentImperfectCar1 == null) return;

            if (this.CurrentImperfectCar1.PassWay == ePassWay.Way1)
            {
                this.iocControler.Gate1Up();
                this.iocControler.GreenLight1();
            }
        }


        /// <summary>
        /// ��1����
        /// </summary>
        void BackGateDown1()
        {
            if (this.CurrentImperfectCar1 == null) return;

            if (this.CurrentImperfectCar1.PassWay == ePassWay.Way1)
            {
                this.iocControler.Gate1Down();
                this.iocControler.RedLight1();
            }
        }

        /// <summary>
        /// ��2����
        /// </summary>
        void BackGateDown2()
        {
            if (this.CurrentImperfectCar2 == null) return;

            if (this.CurrentImperfectCar2.PassWay == ePassWay.Way2)
            {
                this.iocControler.Gate3Down();
                this.iocControler.RedLight2();
            }
        }

        /// <summary>
        /// ��2����
        /// </summary>
        void BackGateUp2()
        {
            if (this.CurrentImperfectCar2 == null) return;

            if (this.CurrentImperfectCar2.PassWay == ePassWay.Way2)
            {
                this.iocControler.Gate3Up();
                this.iocControler.GreenLight2();
            }
        }




        /// <summary>
        /// ǰ��1����
        /// </summary>
        void FrontGateUp1()
        {
            if (this.CurrentImperfectCar1 == null) return;

            if (this.CurrentImperfectCar1.PassWay == ePassWay.Way1)
            {
                this.iocControler.Gate2Up();
            }
        }

        /// <summary>
        /// ǰ��1����
        /// </summary>
        void FrontGateDown1()
        {
            if (this.CurrentImperfectCar1 == null) return;

            if (this.CurrentImperfectCar1.PassWay == ePassWay.Way1)
            {
                this.iocControler.Gate2Down();
            }
        }

        /// <summary>
        /// ǰ��2����
        /// </summary>
        void FrontGateDown2()
        {
            if (this.CurrentImperfectCar2 == null) return;

            if (this.CurrentImperfectCar2.PassWay == ePassWay.Way2)
            {
                this.iocControler.Gate4Down();
            }
        }


        /// <summary>
        /// ǰ��2����
        /// </summary>
        void FrontGateUp2()
        {
            if (this.CurrentImperfectCar2 == null) return;

            if (this.CurrentImperfectCar2.PassWay == ePassWay.Way2)
            {
                this.iocControler.Gate4Up();
            }
        }

        #endregion

        #region ������

        void Rwer1_OnScanError(Exception ex)
        {
            Log4Neter.Error("������1", ex);
        }

        void Rwer1_OnStatusChange(bool status)
        {
            // �����豸״̬ 
            InvokeEx(() =>
            {
                slightRwer1.LightColor = (status ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.������1_����״̬.ToString(), status ? "1" : "0");
            });
        }

        void Rwer2_OnScanError(Exception ex)
        {
            Log4Neter.Error("������2", ex);
        }

        void Rwer2_OnStatusChange(bool status)
        {
            // �����豸״̬ 
            InvokeEx(() =>
            {
                slightRwer2.LightColor = (status ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.������2_����״̬.ToString(), status ? "1" : "0");
            });
        }

        #endregion

        #region LED��ʾ��

        /// <summary>
        /// ����12�ֽڵ��ı�����
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GenerateFillLedContent12(string value)
        {
            int length = Encoding.Default.GetByteCount(value);
            if (length < 12) return value + "".PadRight(12 - length, ' ');

            return value;
        }

        /// <summary>
        /// ����LED��̬����
        /// </summary>
        /// <param name="value1">��һ������</param>
        /// <param name="value2">�ڶ�������</param>
        private void UpdateLedShow(string value1 = "", string value2 = "")
        {
            UpdateLed1Show(value1, value2);
            UpdateLed2Show(value1, value2);
        }

        #region LED1���ƿ�

        /// <summary>
        /// LED1���ƿ�����
        /// </summary>
        int LED1nScreenNo = 1;
        /// <summary>
        /// LED1��̬�����
        /// </summary>
        int LED1DYArea_ID = 1;
        /// <summary>
        /// LED1���±�ʶ
        /// </summary>
        bool LED1m_bSendBusy = false;

        private bool _LED1ConnectStatus;
        /// <summary>
        /// LED1����״̬
        /// </summary>
        public bool LED1ConnectStatus
        {
            get
            {
                return _LED1ConnectStatus;
            }

            set
            {
                _LED1ConnectStatus = value;

                slightLED1.LightColor = (value ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.LED��1_����״̬.ToString(), value ? "1" : "0");
            }
        }

        /// <summary>
        /// LED1��ʾ�����ı�
        /// </summary>
        string LED1TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Led1TempFile.txt");

        /// <summary>
        /// LED1��һ����ʾ����
        /// </summary>
        string LED1PrevLedFileContent = string.Empty;

        /// <summary>
        /// ����LED1��̬����
        /// </summary>
        /// <param name="value1">��һ������</param>
        /// <param name="value2">�ڶ�������</param>
        private void UpdateLed1Show(string value1 = "", string value2 = "")
        {
            FrmDebugConsole.GetInstance().Output("����LED1:|" + value1 + "|" + value2 + "|");

            if (!this.LED1ConnectStatus) return;
            if (this.LED1PrevLedFileContent == value1 + value2) return;

            string ledContent = GenerateFillLedContent12(value1) + GenerateFillLedContent12(value2);

            File.WriteAllText(this.LED1TempFile, ledContent, Encoding.UTF8);

            if (LED1m_bSendBusy == false)
            {
                LED1m_bSendBusy = true;

                //int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID);
                //if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("����LED��̬����", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

                LED1m_bSendBusy = false;
            }

            this.LED1PrevLedFileContent = value1 + value2;
        }

        #endregion

        #region LED2���ƿ�

        /// <summary>
        /// LED2���ƿ�����
        /// </summary>
        int LED2nScreenNo = 1;
        /// <summary>
        /// LED2��̬�����
        /// </summary>
        int LED2DYArea_ID = 1;
        /// <summary>
        /// LED2���±�ʶ
        /// </summary>
        bool LED2m_bSendBusy = false;

        private bool _LED2ConnectStatus;
        /// <summary>
        /// LED2����״̬
        /// </summary>
        public bool LED2ConnectStatus
        {
            get
            {
                return _LED2ConnectStatus;
            }

            set
            {
                _LED2ConnectStatus = value;

                slightLED2.LightColor = (value ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.LED��2_����״̬.ToString(), value ? "1" : "0");
            }
        }

        /// <summary>
        /// LED2��ʾ�����ı�
        /// </summary>
        string LED2TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Led2TempFile.txt");

        /// <summary>
        /// LED2��һ����ʾ����
        /// </summary>
        string LED2PrevLedFileContent = string.Empty;

        /// <summary>
        /// ����LED2��̬����
        /// </summary>
        /// <param name="value1">��һ������</param>
        /// <param name="value2">�ڶ�������</param>
        private void UpdateLed2Show(string value1 = "", string value2 = "")
        {
            FrmDebugConsole.GetInstance().Output("����LED2:|" + value1 + "|" + value2 + "|");

            if (!this.LED1ConnectStatus) return;
            if (this.LED2PrevLedFileContent == value1 + value2) return;

            string ledContent = GenerateFillLedContent12(value1) + GenerateFillLedContent12(value2);

            File.WriteAllText(this.LED1TempFile, ledContent, Encoding.UTF8);

            if (LED2m_bSendBusy == false)
            {
                LED2m_bSendBusy = true;

                //int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED2nScreenNo, this.LED2DYArea_ID);
                //if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("����LED��̬����", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

                LED2m_bSendBusy = false;
            }

            this.LED2PrevLedFileContent = value1 + value2;
        }

        #endregion

        #endregion

        #region �豸��ʼ����ж��

        /// <summary>
        /// ��ʼ������豸
        /// </summary>
        private void InitHardware()
        {
            try
            {
                bool success = false;

                this.InductorCoil1Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�1�˿�");
                this.InductorCoil2Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�2�˿�");
                this.InductorCoil3Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�3�˿�");
                this.InductorCoil4Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�4�˿�");
                this.InductorCoil5Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�5�˿�");
                this.InductorCoil6Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�6�˿�");
                this.InductorCoil7Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�7�˿�");
                this.InductorCoil8Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�8�˿�");


                // IO������
                Hardwarer.Iocer.OnReceived += new IOC.JMDM20DIOV2.JMDM20DIOV2Iocer.ReceivedEventHandler(Iocer_Received);
                Hardwarer.Iocer.OnStatusChange += new IOC.JMDM20DIOV2.JMDM20DIOV2Iocer.StatusChangeHandler(Iocer_StatusChange);
                success = Hardwarer.Iocer.OpenCom(commonDAO.GetAppletConfigInt32("IO������_����"), commonDAO.GetAppletConfigInt32("IO������_������"), commonDAO.GetAppletConfigInt32("IO������_����λ"), (StopBits)commonDAO.GetAppletConfigInt32("IO������_ֹͣλ"), (Parity)commonDAO.GetAppletConfigInt32("IO������_У��λ"));
                if (!success) MessageBoxEx.Show("IO����������ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.iocControler = new IocControler(Hardwarer.Iocer);


                // ������1
                Hardwarer.Rwer1.StartWith = commonDAO.GetAppletConfigString("������_��ǩ����");
                Hardwarer.Rwer1.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer1_OnStatusChange);
                Hardwarer.Rwer1.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer1_OnScanError);
                success = Hardwarer.Rwer1.OpenCom(commonDAO.GetAppletConfigInt32("������1_����"));
                if (!success) MessageBoxEx.Show("������1����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // ������2
                Hardwarer.Rwer2.StartWith = commonDAO.GetAppletConfigString("������_��ǩ����");
                Hardwarer.Rwer2.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer2_OnStatusChange);
                Hardwarer.Rwer2.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer2_OnScanError);
                success = Hardwarer.Rwer2.OpenCom(commonDAO.GetAppletConfigInt32("������2_����"));
                if (!success) MessageBoxEx.Show("������2����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                #region LED���ƿ�1

                string led1SocketIP = commonDAO.GetAppletConfigString("LED��ʾ��1_IP��ַ");
                if (!string.IsNullOrEmpty(led1SocketIP))
                {
                    int nResult = YB14DynamicAreaLeder.AddScreen(YB14DynamicAreaLeder.CONTROLLER_BX_5E1, this.LED1nScreenNo, YB14DynamicAreaLeder.SEND_MODE_NETWORK, 96, 32, 1, 1, "", 0, led1SocketIP, 5005, "");
                    if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                    {
                        nResult = YB14DynamicAreaLeder.AddScreenDynamicArea(this.LED1nScreenNo, this.LED1DYArea_ID, 0, 10, 1, "", 0, 0, 0, 96, 32, 255, 0, 255, 7, 6, 1);
                        if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                        {
                            nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED1nScreenNo, this.LED1DYArea_ID, this.LED1TempFile, 0, "����", 12, 0, 120, 1, 3, 0);
                            if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                            {
                                // ��ʼ���ɹ�
                                this.LED1ConnectStatus = true;
                                UpdateLed1Show("  �ȴ�����");

                            }
                            else
                            {
                                this.LED1ConnectStatus = false;
                                Log4Neter.Error("��ʼ��LED1���ƿ�", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicAreaFile", nResult)));
                                MessageBoxEx.Show("LED1���ƿ�����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            this.LED1ConnectStatus = false;
                            Log4Neter.Error("��ʼ��LED1���ƿ�", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicArea", nResult)));
                            MessageBoxEx.Show("LED1���ƿ�����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        this.LED1ConnectStatus = false;
                        Log4Neter.Error("��ʼ��LED1���ƿ�", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreen", nResult)));
                        MessageBoxEx.Show("LED1���ƿ�����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                #endregion

                #region LED���ƿ�2

                string led2SocketIP = commonDAO.GetAppletConfigString("LED��ʾ��2_IP��ַ");
                if (!string.IsNullOrEmpty(led2SocketIP))
                {
                    int nResult = YB14DynamicAreaLeder.AddScreen(YB14DynamicAreaLeder.CONTROLLER_BX_5E1, this.LED2nScreenNo, YB14DynamicAreaLeder.SEND_MODE_NETWORK, 96, 32, 1, 1, "", 0, led2SocketIP, 5005, "");
                    if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                    {
                        nResult = YB14DynamicAreaLeder.AddScreenDynamicArea(this.LED2nScreenNo, this.LED2DYArea_ID, 0, 10, 1, "", 0, 0, 0, 96, 32, 255, 0, 255, 7, 6, 1);
                        if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                        {
                            nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED2nScreenNo, this.LED2DYArea_ID, this.LED2TempFile, 0, "����", 12, 0, 120, 1, 3, 0);
                            if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                            {
                                // ��ʼ���ɹ�
                                this.LED2ConnectStatus = true;
                                UpdateLed2Show("  �ȴ�����");
                            }
                            else
                            {
                                this.LED2ConnectStatus = false;
                                Log4Neter.Error("��ʼ��LED2���ƿ�", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicAreaFile", nResult)));
                                MessageBoxEx.Show("LED2���ƿ�����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            this.LED2ConnectStatus = false;
                            Log4Neter.Error("��ʼ��LED2���ƿ�", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicArea", nResult)));
                            MessageBoxEx.Show("LED2���ƿ�����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        this.LED2ConnectStatus = false;
                        Log4Neter.Error("��ʼ��LED2���ƿ�", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreen", nResult)));
                        MessageBoxEx.Show("LED2���ƿ�����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                #endregion

                timer1.Enabled = true;
            }
            catch (Exception ex)
            {
                Log4Neter.Error("�豸��ʼ��", ex);
            }
        }

        /// <summary>
        /// ж���豸
        /// </summary>
        private void UnloadHardware()
        {
            // ע��˶δ���
            Application.DoEvents();

            try
            {
                Hardwarer.Iocer.OnReceived -= new IOC.JMDM20DIOV2.JMDM20DIOV2Iocer.ReceivedEventHandler(Iocer_Received);
                Hardwarer.Iocer.OnStatusChange -= new IOC.JMDM20DIOV2.JMDM20DIOV2Iocer.StatusChangeHandler(Iocer_StatusChange);

                Hardwarer.Iocer.CloseCom();
            }
            catch { }
            try
            {
                Hardwarer.Rwer1.CloseCom();
            }
            catch { }
            try
            {
                Hardwarer.Rwer2.CloseCom();
            }
            catch { }
            try
            {
                YB14DynamicAreaLeder.SendDeleteDynamicAreasCommand(this.LED1nScreenNo, 1, "");
                YB14DynamicAreaLeder.DeleteScreen(this.LED1nScreenNo);
            }
            catch { }
            try
            {
                YB14DynamicAreaLeder.SendDeleteDynamicAreasCommand(this.LED2nScreenNo, 1, "");
                YB14DynamicAreaLeder.DeleteScreen(this.LED2nScreenNo);
            }
            catch { }
        }


        private void FrmOrder_FormClosing(object sender, FormClosingEventArgs e)
        {

            // ж���豸
            UnloadHardware();
        }
        #endregion

        #endregion

        #region ��բ���ư�ť

        /// <summary>
        /// ��բ1����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate1Up_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate1Up();
        }

        /// <summary>
        /// ��բ1����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate1Down_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate1Down();
        }
        /// <summary>
        /// ��բ2����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate2Up_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate2Up();
        }

        /// <summary>
        /// ��բ2����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate2Down_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate2Down();
        }

        /// <summary>
        /// ��բ3����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate3Up_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate3Up();
        }

        /// <summary>
        /// ��բ3����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate3Down_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate3Down();
        }
        /// <summary>
        /// ��բ4����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate4Up_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate4Up();
        }

        /// <summary>
        /// ��բ4����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate4Down_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate4Down();
        }
        #endregion

        #region ����ҵ��

        /// <summary>
        /// ����������ʶ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Interval = 2000;

            try
            {
                // ִ��Զ������
                ExecAppRemoteControlCmd();

                try
                {
                    switch (this.CurrentFlowFlag1)
                    {
                        case eFlowFlag.�ȴ�����:
                            #region

                            // PassWay.Way1
                            if (this.InductorCoil1)
                            {
                                // ����������ظ����źţ������������߳���ʶ��

                                List<string> tags = Hardwarer.Rwer1.ScanTags();
                                if (tags.Count > 0) passCarQueuer1.Enqueue(ePassWay.Way1, tags[0], true);
                            }
                            if (passCarQueuer1.Count > 0) this.CurrentFlowFlag1 = eFlowFlag.ʶ����;

                            #endregion
                            break;

                        case eFlowFlag.ʶ����:
                            #region

                            // �������޳�ʱ���ȴ�����
                            if (passCarQueuer1.Count == 0)
                            {
                                this.CurrentFlowFlag1 = eFlowFlag.�ȴ�����;
                                break;
                            }

                            this.CurrentImperfectCar1 = passCarQueuer1.Dequeue();

                            // ��ʽһ������ʶ��ĳ��ƺŲ��ҳ�����Ϣ
                            this.CurrentAutotruck1 = carTransportDAO.GetAutotruckByCarNumber(this.CurrentImperfectCar1.Voucher);
                            UpdateLed1Show(this.CurrentImperfectCar1.Voucher);
                            //// ��ʽ��������ʶ��ı�ǩ�����ҳ�����Ϣ
                            ////this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);

                            if (this.CurrentAutotruck1 != null)
                            {
                                if (this.CurrentAutotruck1.IsUse == 1)
                                {
                                    if (this.CurrentAutotruck1.CarType == eCarType.����ú.ToString())
                                    {
                                        this.timer_SaleFuel1_Cancel = false;
                                        this.CurrentFlowFlag1 = eFlowFlag.��֤��Ϣ;
                                    }
                                }
                                else
                                {
                                    UpdateLed1Show(this.CurrentAutotruck1.CarNumber, "��ͣ��");
                                    this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck1.CarNumber + " ��ͣ�ã���ֹͨ��", 2, false);

                                    timer1.Interval = 20000;
                                }
                            }
                            else
                            {
                                UpdateLed1Show(this.CurrentImperfectCar1.Voucher, "δ�Ǽ�");

                                // ��ʽһ������ʶ��
                                this.voiceSpeaker.Speak("���ƺ� " + this.CurrentImperfectCar1.Voucher + " δ�Ǽǣ���ֹͨ��", 2, false);
                                //// ��ʽ����ˢ����ʽ
                                //this.voiceSpeaker.Speak("����δ�Ǽǣ���ֹͨ��", 2, false);

                                timer1.Interval = 20000;
                            }

                            #endregion
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log4Neter.Error("timer1_Tick��CurrentFlowFlag1", ex);
                }

                try
                {
                    switch (this.CurrentFlowFlag2)
                    {
                        case eFlowFlag.�ȴ�����:
                            #region

                            // PassWay.Way2
                            if (this.InductorCoil2)
                            {
                                // ����������ظ����źţ������������߳���ʶ��

                                List<string> tags = Hardwarer.Rwer2.ScanTags();
                                if (tags.Count > 0) passCarQueuer2.Enqueue(ePassWay.Way2, tags[0], true);
                            }
                            if (passCarQueuer2.Count > 0) this.CurrentFlowFlag2 = eFlowFlag.ʶ����;

                            #endregion
                            break;

                        case eFlowFlag.ʶ����:
                            #region

                            // �������޳�ʱ���ȴ�����
                            if (passCarQueuer2.Count == 0)
                            {
                                this.CurrentFlowFlag2 = eFlowFlag.�ȴ�����;
                                break;
                            }

                            this.CurrentImperfectCar2 = passCarQueuer2.Dequeue();

                            // ��ʽһ������ʶ��ĳ��ƺŲ��ҳ�����Ϣ
                            this.CurrentAutotruck2 = carTransportDAO.GetAutotruckByCarNumber(this.CurrentImperfectCar2.Voucher);
                            UpdateLed2Show(this.CurrentImperfectCar2.Voucher);
                            //// ��ʽ��������ʶ��ı�ǩ�����ҳ�����Ϣ
                            ////this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);

                            if (this.CurrentAutotruck2 != null)
                            {
                                if (this.CurrentAutotruck2.IsUse == 2)
                                {
                                    if (this.CurrentAutotruck2.CarType == eCarType.����ú.ToString())
                                    {
                                        this.timer_SaleFuel2_Cancel = false;
                                        this.CurrentFlowFlag2 = eFlowFlag.��֤��Ϣ;
                                    }
                                }
                                else
                                {
                                    UpdateLed2Show(this.CurrentAutotruck2.CarNumber, "��ͣ��");
                                    this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck2.CarNumber + " ��ͣ�ã���ֹͨ��", 2, false);

                                    timer2.Interval = 20000;
                                }
                            }
                            else
                            {
                                UpdateLed2Show(this.CurrentImperfectCar2.Voucher, "δ�Ǽ�");

                                // ��ʽһ������ʶ��
                                this.voiceSpeaker.Speak("���ƺ� " + this.CurrentImperfectCar2.Voucher + " δ�Ǽǣ���ֹͨ��", 2, false);
                                //// ��ʽ����ˢ����ʽ
                                //this.voiceSpeaker.Speak("����δ�Ǽǣ���ֹͨ��", 2, false);

                                timer2.Interval = 20000;
                            }

                            #endregion
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log4Neter.Error("timer1_Tick��CurrentFlowFlag2", ex);
                }
            }
            catch (Exception ex)
            {
                Log4Neter.Error("timer1_Tick", ex);
            }
            finally
            {
                timer1.Start();
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            // ������ִ��һ��
            timer2.Interval = 180000;

            try
            {

                // ����ú 
                LoadTodayUnFinishSaleFuelTransport();
                LoadTodayFinishSaleFuelTransport();

            }
            catch (Exception ex)
            {
                Log4Neter.Error("timer2_Tick", ex);
            }
            finally
            {
                timer2.Start();
            }
        }



        void LoadTodayUnFinishSaleFuelTransport()
        {
            superGridControl1_SaleFuel.PrimaryGrid.DataSource = orderDAO.GetUnFinishSaleFuelTransport();
            superGridControl3_SaleFuel.PrimaryGrid.DataSource = orderDAO.GetUnFinishSaleFuelTransport();
        }

        void LoadTodayFinishSaleFuelTransport()
        {
            superGridControl2_SaleFuel.PrimaryGrid.DataSource = orderDAO.GetFinishedSaleFuelTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
            superGridControl4_SaleFuel.PrimaryGrid.DataSource = orderDAO.GetFinishedSaleFuelTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        }


        /// <summary>
        /// 1���г����ڵ�ǰ��·��
        /// </summary>
        /// <returns></returns>
        bool HasCarOnCurrentWay1()
        {
            if (this.CurrentImperfectCar1 == null) return false;

            if (this.CurrentImperfectCar1.PassWay == ePassWay.UnKnow)
                return false;
            else if (this.CurrentImperfectCar1.PassWay == ePassWay.Way1)
                return this.InductorCoil1 || this.InductorCoil2;
            return true;
        }

        /// <summary>
        /// �г����ڵ�ǰ��·��
        /// </summary>
        /// <returns></returns>
        bool HasCarOnCurrentWay2()
        {
            if (this.CurrentImperfectCar2 == null) return false;

            if (this.CurrentImperfectCar2.PassWay == ePassWay.UnKnow)
                return false;
            else if (this.CurrentImperfectCar2.PassWay == ePassWay.Way2)
                return this.InductorCoil5 || this.InductorCoil6;

            return true;
        }


        /// <summary>
        /// ִ��Զ������
        /// </summary>
        void ExecAppRemoteControlCmd()
        {
            // ��ȡ���µ�����
            CmcsAppRemoteControlCmd appRemoteControlCmd = commonDAO.GetNewestAppRemoteControlCmd(CommonAppConfig.GetInstance().AppIdentifier);
            if (appRemoteControlCmd != null)
            {
                if (appRemoteControlCmd.CmdCode == "���Ƶ�բ")
                {
                    Log4Neter.Info("����Զ�����" + appRemoteControlCmd.CmdCode + "��������" + appRemoteControlCmd.Param);

                    if (appRemoteControlCmd.Param.Equals("Gate1Up", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate1Up();
                    else if (appRemoteControlCmd.Param.Equals("Gate1Down", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate1Down();
                    else if (appRemoteControlCmd.Param.Equals("Gate3Up", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate3Up();
                    else if (appRemoteControlCmd.Param.Equals("Gate3Down", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate3Down();

                    // ����ִ�н��
                    commonDAO.SetAppRemoteControlCmdResultCode(appRemoteControlCmd, eEquInfCmdResultCode.�ɹ�);
                }
            }
        }

        /// <summary>
        /// �л��ֶ�/�Զ�ģʽ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sbtnChangeAutoHandMode_ValueChanged(object sender, EventArgs e)
        {
            this.AutoHandMode = sbtnChangeAutoHandMode.Value;
        }

        #endregion

        #region ����úҵ��

        bool timer_SaleFuel1_Cancel = true;
        bool timer_SaleFuel2_Cancel = true;

        CmcsSaleFuelTransport currentSaleFuelTransport1;
        /// <summary>
        /// ��ǰ�����¼1
        /// </summary>
        public CmcsSaleFuelTransport CurrentSaleFuelTransport1
        {
            get { return currentSaleFuelTransport1; }
            set
            {
                currentSaleFuelTransport1 = value;

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), value.Id);

                    txt_YBNumber1.Text = value.TransportSalesNum;
                    txt_TransportNo1.Text = value.TransportNo;
                    txt_Consignee1.Text = value.SupplierName;
                    txt_TransportCompayName1.Text = value.TransportCompanyName;
                }
                else
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), string.Empty);
                    txtCarNumber_SaleFuel1.ResetText();
                    txtTagId_SaleFuel1.ResetText();
                    txt_YBNumber1.ResetText();
                    txt_TransportNo1.ResetText();
                    txt_Consignee1.ResetText();
                    txt_TransportCompayName1.ResetText();
                    txt_ReMark1.ResetText();
                }
            }
        }

        CmcsSaleFuelTransport currentSaleFuelTransport2;
        /// <summary>
        /// ��ǰ�����¼2
        /// </summary>
        public CmcsSaleFuelTransport CurrentSaleFuelTransport2
        {
            get { return currentSaleFuelTransport2; }
            set
            {
                currentSaleFuelTransport2 = value;

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), value.Id);

                    txt_YBNumber1.Text = value.TransportSalesNum;
                    txt_TransportNo1.Text = value.TransportNo;
                    txt_Consignee1.Text = value.SupplierName;
                    txt_TransportCompayName1.Text = value.TransportCompanyName;
                }
                else
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), string.Empty);
                    txtCarNumber_SaleFuel2.ResetText();
                    txtTagId_SaleFuel2.ResetText();
                    txt_YBNumber2.ResetText();
                    txt_TransportNo2.ResetText();
                    txt_Consignee2.ResetText();
                    txt_TransportCompayName2.ResetText();
                    txt_ReMark2.ResetText();
                }
            }
        }

        #endregion

        #region ��������

        Pen redPen3 = new Pen(Color.Red, 3);
        Pen greenPen3 = new Pen(Color.Lime, 3);
        Pen greenPen1 = new Pen(Color.Lime, 1);

        /// <summary>
        /// ��ǰ�Ǳ�����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panCurrentWeight_Paint(object sender, PaintEventArgs e)
        {
            PanelEx panel = sender as PanelEx;

            // ���Ƶظ�1
            e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, 10, 15, 30);
            // ���Ƶظ�2                                                               
            e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, 25, 10, 25, 30);
            // ���Ƶظ�3
            e.Graphics.DrawLine(this.InductorCoil3 ? redPen3 : greenPen3, 35, 10, 35, 30);
            // ���Ƶظ�4                                                               
            e.Graphics.DrawLine(this.InductorCoil4 ? redPen3 : greenPen3, 45, 10, 45, 30);
            // ���Ʒָ���
            e.Graphics.DrawLine(greenPen1, 5, 34, 55, 34);
            // ���Ƶظ�5
            e.Graphics.DrawLine(this.InductorCoil5 ? redPen3 : greenPen3, 15, 38, 15, 58);
            // ���Ƶظ�6                                                               
            e.Graphics.DrawLine(this.InductorCoil6 ? redPen3 : greenPen3, 25, 38, 25, 58);
            // ���Ƶظ�7
            e.Graphics.DrawLine(this.InductorCoil7 ? redPen3 : greenPen3, 35, 38, 35, 58);
            // ���Ƶظ�8                                                               
            e.Graphics.DrawLine(this.InductorCoil8 ? redPen3 : greenPen3, 45, 38, 45, 58);
        }

        private void superGridControl_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // ȡ������༭
            e.Cancel = true;
        }

        /// <summary>
        /// �����к�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl_GetRowHeaderText(object sender, DevComponents.DotNetBar.SuperGrid.GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.RowIndex + 1).ToString();
        }

        /// <summary>
        /// Invoke��װ
        /// </summary>
        /// <param name="action"></param>
        public void InvokeEx(Action action)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            this.Invoke(action);
        }

        #endregion

        private void btnSaveTransport_SaleFuel1_Click(object sender, EventArgs e)
        {
            if (!SaveSaleFuelTransport1()) MessageBoxEx.Show("����ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// ������Ϣ1
        /// </summary>
        void ResetSelFuel1()
        {
            this.timer_SaleFuel1_Cancel = true;

            this.CurrentFlowFlag1 = eFlowFlag.�ȴ�����;

            this.CurrentAutotruck1 = null;
            this.CurrentSaleFuelTransport1 = null;

            txtTagId_SaleFuel1.ResetText();

            btnSaveTransport_SaleFuel1.Enabled = false;

            BackGateDown1();
            FrontGateDown1();

            UpdateLedShow("  �ȴ�����");

            // �������
            this.CurrentImperfectCar1 = null;
        }
        /// <summary>
        /// ������Ϣ2
        /// </summary>
        void ResetSelFuel2()
        {
            this.timer_SaleFuel2_Cancel = true;

            this.CurrentFlowFlag2 = eFlowFlag.�ȴ�����;

            this.CurrentAutotruck2 = null;
            this.CurrentSaleFuelTransport2 = null;

            txtTagId_SaleFuel2.ResetText();

            btnSaveTransport_SaleFuel2.Enabled = false;

            BackGateDown2();
            FrontGateDown2();

            UpdateLedShow("  �ȴ�����");

            // �������
            this.CurrentImperfectCar2 = null;
        }
        private void timer_SaleFuel1_Tick(object sender, EventArgs e)
        {
            if (this.timer_SaleFuel1_Cancel) return;

            timer_SaleFuel1.Stop();
            timer_SaleFuel1.Interval = 2000;

            try
            {
                switch (this.CurrentFlowFlag1)
                {
                    case eFlowFlag.��֤��Ϣ:
                        #region

                        // ���Ҹó�δ��ɵ������¼
                        CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck1.Id, eCarType.����ú.ToString());
                        if (unFinishTransport != null)
                        {
                            this.CurrentSaleFuelTransport1 = commonDAO.SelfDber.Get<CmcsSaleFuelTransport>(unFinishTransport.TransportId);
                            if (this.CurrentSaleFuelTransport1 != null)
                            {
                                // �ж�·������
                                string nextPlace;
                                if (carTransportDAO.CheckNextTruckInFactoryWay(this.CurrentAutotruck1.CarType, this.CurrentSaleFuelTransport1.StepName, "װ��", CommonAppConfig.GetInstance().AppIdentifier, out nextPlace))
                                {
                                    if (CommonAppConfig.GetInstance().AppIdentifier.Contains(this.CurrentSaleFuelTransport1.LoadArea))
                                    {
                                        this.CurrentFlowFlag1 = eFlowFlag.������Ϣ;
                                    }
                                    else
                                    {
                                        UpdateLed1Show("·�ߴ���", "��ֹͨ��");
                                        this.voiceSpeaker.Speak("·�ߴ��� ��ֹͨ�� " + (!string.IsNullOrEmpty(this.CurrentSaleFuelTransport1.LoadArea) ? "��ǰ��" + this.CurrentSaleFuelTransport1.LoadArea : ""), 2, false);

                                        timer_SaleFuel1.Interval = 20000;
                                    }
                                }
                                else
                                {
                                    UpdateLed1Show("·�ߴ���", "��ֹͨ��");
                                    this.voiceSpeaker.Speak("·�ߴ��� ��ֹͨ�� " + (!string.IsNullOrEmpty(nextPlace) ? "��ǰ��" + nextPlace : ""), 2, false);

                                    timer_SaleFuel1.Interval = 20000;
                                }
                            }
                            else
                            {
                                commonDAO.SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                            }
                        }
                        else
                        {
                            UpdateLed1Show(this.CurrentAutotruck1.CarNumber, "δ�Ŷ�");
                            this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck1.CarNumber + " δ�ҵ��ŶӼ�¼", 2, false);

                            timer_SaleFuel1.Interval = 20000;
                        }

                        #endregion
                        break;
                    case eFlowFlag.������Ϣ:
                        // ���������
                        timer_SaleFuel1.Interval = 1000;

                        btnSaveTransport_SaleFuel1.Enabled = true;



                        if (this.AutoHandMode)
                        {
                            // �Զ�ģʽ
                            if (!SaveSaleFuelTransport1())
                            {
                                BackGateUp1();
                                UpdateLed1Show(this.CurrentAutotruck1.CarNumber, "����ʧ��");
                                this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck1.CarNumber + " ����ʧ�ܣ�����ϵ����Ա", 2, false);
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            // �ֶ�ģʽ 
                        }
                        break;
                    case eFlowFlag.�ȴ�����:
                        #region

                        // ���еظ����ź�ʱ����
                        if (!this.InductorCoil1 && !this.InductorCoil2)
                        {
                            FrontGateUp1();
                            this.CurrentFlowFlag1 = eFlowFlag.�ȴ��뿪;
                        }
                        // ����������
                        timer_SaleFuel1.Interval = 4000;

                        #endregion
                        break;
                    case eFlowFlag.�ȴ��뿪:
                        #region
                        // ���еظ����ź�ʱ����
                        if (!HasCarOnLeaveWay1())
                        {
                            ResetSelFuel1();
                        }
                        // ����������
                        timer_SaleFuel1.Interval = 4000;

                        #endregion
                        break;
                }

                // ��ǰ�ذ�����С����С���������еظС��������ź�ʱ����
                if (!HasCarOnEnterWay1() && !HasCarOnLeaveWay1() && this.CurrentFlowFlag1 != eFlowFlag.�ȴ�����
                    && this.CurrentImperfectCar1 != null) ResetSelFuel1();
            }
            catch (Exception ex)
            {
                Log4Neter.Error("timer_SaleFuel1_Tick", ex);
            }
            finally
            {
                timer_SaleFuel1.Start();
            }
        }


        private void timer_SaleFuel2_Tick(object sender, EventArgs e)
        {
            if (this.timer_SaleFuel2_Cancel) return;

            timer_SaleFuel2.Stop();
            timer_SaleFuel2.Interval = 2000;

            try
            {
                switch (this.CurrentFlowFlag2)
                {
                    case eFlowFlag.��֤��Ϣ:
                        #region

                        // ���Ҹó�δ��ɵ������¼
                        CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck2.Id, eCarType.����ú.ToString());
                        if (unFinishTransport != null)
                        {
                            this.CurrentSaleFuelTransport2 = commonDAO.SelfDber.Get<CmcsSaleFuelTransport>(unFinishTransport.TransportId);
                            if (this.CurrentSaleFuelTransport2 != null)
                            {
                                // �ж�·������
                                string nextPlace;
                                if (carTransportDAO.CheckNextTruckInFactoryWay(this.CurrentAutotruck2.CarType, this.CurrentSaleFuelTransport2.StepName, "װ��", CommonAppConfig.GetInstance().AppIdentifier, out nextPlace))
                                {
                                    if (CommonAppConfig.GetInstance().AppIdentifier.Contains(this.CurrentSaleFuelTransport2.LoadArea))
                                    {
                                        this.CurrentFlowFlag2 = eFlowFlag.������Ϣ;
                                    }
                                    else
                                    {
                                        UpdateLed2Show("·�ߴ���", "��ֹͨ��");
                                        this.voiceSpeaker.Speak("·�ߴ��� ��ֹͨ�� " + (!string.IsNullOrEmpty(this.CurrentSaleFuelTransport2.LoadArea) ? "��ǰ��" + this.CurrentSaleFuelTransport2.LoadArea : ""), 2, false);

                                        timer_SaleFuel2.Interval = 20000;
                                    }
                                }
                                else
                                {
                                    UpdateLed2Show("·�ߴ���", "��ֹͨ��");
                                    this.voiceSpeaker.Speak("·�ߴ��� ��ֹͨ�� " + (!string.IsNullOrEmpty(nextPlace) ? "��ǰ��" + nextPlace : ""), 2, false);

                                    timer_SaleFuel2.Interval = 20000;
                                }
                            }
                            else
                            {
                                commonDAO.SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                            }
                        }
                        else
                        {
                            UpdateLed2Show(this.CurrentAutotruck2.CarNumber, "δ�Ŷ�");
                            this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck2.CarNumber + " δ�ҵ��ŶӼ�¼", 2, false);

                            timer_SaleFuel2.Interval = 20000;
                        }

                        #endregion
                        break;
                    case eFlowFlag.������Ϣ:
                        // ���������
                        timer_SaleFuel2.Interval = 2000;

                        btnSaveTransport_SaleFuel2.Enabled = true;



                        if (this.AutoHandMode)
                        {
                            // �Զ�ģʽ
                            if (!SaveSaleFuelTransport2())
                            {
                                BackGateUp2();
                                UpdateLed2Show(this.CurrentAutotruck2.CarNumber, "����ʧ��");
                                this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck2.CarNumber + " ����ʧ�ܣ�����ϵ����Ա", 2, false);
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            // �ֶ�ģʽ 
                        }
                        break;
                    case eFlowFlag.�ȴ�����:
                        #region

                        // ���еظ����ź�ʱ����
                        if (!this.InductorCoil5 && !this.InductorCoil6)
                        {
                            FrontGateUp2();
                            this.CurrentFlowFlag2 = eFlowFlag.�ȴ��뿪;
                        }
                        // ����������
                        timer_SaleFuel2.Interval = 4000;

                        #endregion
                        break;
                    case eFlowFlag.�ȴ��뿪:
                        #region

                        // ���еظ����ź�ʱ����
                        if (!HasCarOnLeaveWay2())
                        {
                            ResetSelFuel2();
                        }
                        // ����������
                        timer_SaleFuel2.Interval = 4000;

                        #endregion
                        break;
                }

                // ��ǰ�ذ�����С����С���������еظС��������ź�ʱ����
                if (!HasCarOnEnterWay2() && !HasCarOnLeaveWay2() && this.CurrentFlowFlag2 != eFlowFlag.�ȴ�����
                    && this.CurrentImperfectCar2 != null) ResetSelFuel2();
            }
            catch (Exception ex)
            {
                Log4Neter.Error("timer_SaleFuel2_Tick", ex);
            }
            finally
            {
                timer_SaleFuel2.Start();
            }
        }
        private void btnSaveTransport_SaleFuel2_Click(object sender, EventArgs e)
        {

            if (!SaveSaleFuelTransport2()) MessageBoxEx.Show("����ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// ���������¼
        /// </summary>
        /// <returns></returns>
        bool SaveSaleFuelTransport1()
        {
            if (this.CurrentSaleFuelTransport1 == null) return false;

            try
            {
                if (orderDAO.SaveSaleFuelTransport(this.CurrentSaleFuelTransport1.Id, DateTime.Now))
                {
                    this.CurrentSaleFuelTransport1 = commonDAO.SelfDber.Get<CmcsSaleFuelTransport>(this.CurrentSaleFuelTransport1.Id);


                    btnSaveTransport_SaleFuel1.Enabled = false;
                    this.CurrentFlowFlag1 = eFlowFlag.�ȴ�����;
                    UpdateLed1Show("���װ��", "���Ժ�");
                    this.voiceSpeaker.Speak("���װ�أ�װ����ɺ��뿪��", 2, false);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("����ʧ��\r\n" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log4Neter.Error("���������¼", ex);
            }

            return false;
        }


        /// <summary>
        /// ���������¼
        /// </summary>
        /// <returns></returns>
        bool SaveSaleFuelTransport2()
        {
            if (this.CurrentSaleFuelTransport2 == null) return false;

            try
            {
                if (orderDAO.SaveSaleFuelTransport(this.CurrentSaleFuelTransport2.Id, DateTime.Now))
                {
                    this.CurrentSaleFuelTransport2 = commonDAO.SelfDber.Get<CmcsSaleFuelTransport>(this.CurrentSaleFuelTransport2.Id);


                    btnSaveTransport_SaleFuel2.Enabled = false;
                    this.CurrentFlowFlag2 = eFlowFlag.�ȴ�����;

                    UpdateLed2Show("���װ��", "���Ժ�");
                    this.voiceSpeaker.Speak("���װ�أ�װ����ɺ��뿪��", 2, false);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("����ʧ��\r\n" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log4Neter.Error("���������¼", ex);
            }

            return false;
        }


        /// <summary>
        /// �г���1���°��ĵ�·��
        /// </summary>
        /// <returns></returns>
        bool HasCarOnLeaveWay1()
        {
            if (this.CurrentImperfectCar1 == null) return false;

            if (this.CurrentImperfectCar1.PassWay == ePassWay.UnKnow)
                return false;
            else if (this.CurrentImperfectCar1.PassWay == ePassWay.Way1)
                return this.InductorCoil2 || this.InductorCoil3 || this.InductorCoil4;

            return true;
        }
        /// <summary>
        /// �г���2���°��ĵ�·��
        /// </summary>
        /// <returns></returns>
        bool HasCarOnLeaveWay2()
        {
            if (this.CurrentImperfectCar2 == null) return false;

            if (this.CurrentImperfectCar2.PassWay == ePassWay.UnKnow)
                return false;
            else if (this.CurrentImperfectCar2.PassWay == ePassWay.Way2)
                return this.InductorCoil6 || this.InductorCoil7 || this.InductorCoil8;

            return true;
        }

        /// <summary>
        /// �г���1���ϰ��ĵ�·��
        /// </summary>
        /// <returns></returns>
        bool HasCarOnEnterWay1()
        {
            if (this.CurrentImperfectCar1 == null) return false;

            if (this.CurrentImperfectCar1.PassWay == ePassWay.UnKnow)
                return false;
            else if (this.CurrentImperfectCar1.PassWay == ePassWay.Way1)
                return this.InductorCoil1 || this.InductorCoil2;

            return true;
        }
        /// <summary>
        /// �г���2���ϰ��ĵ�·��
        /// </summary>
        /// <returns></returns>
        bool HasCarOnEnterWay2()
        {
            if (this.CurrentImperfectCar2 == null) return false;

            if (this.CurrentImperfectCar2.PassWay == ePassWay.UnKnow)
                return false;
            else if (this.CurrentImperfectCar2.PassWay == ePassWay.Way2)
                return this.InductorCoil5 || this.InductorCoil6;

            return true;
        }

        private void btnSelectAutotruck_SaleFuel1_Click(object sender, EventArgs e)
        {

            FrmUnFinishTransport_Select frm = new FrmUnFinishTransport_Select("where CarType='" + eCarType.����ú.ToString() + "' order by CreateDate desc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (this.InductorCoil1)
                    passCarQueuer1.Enqueue(ePassWay.Way1, frm.Output.CarNumber, false);

                this.CurrentFlowFlag1 = eFlowFlag.ʶ����;
            }
        }

        private void btnSelectAutotruck_SaleFuel2_Click(object sender, EventArgs e)
        {
            FrmUnFinishTransport_Select frm = new FrmUnFinishTransport_Select("where CarType='" + eCarType.����ú.ToString() + "' order by CreateDate desc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (this.InductorCoil5)
                    passCarQueuer2.Enqueue(ePassWay.Way2, frm.Output.CarNumber, false);

                this.CurrentFlowFlag2 = eFlowFlag.ʶ����;
            }
        }

        private void btnReset_SaleFuel2_Click(object sender, EventArgs e)
        {
            ResetSelFuel2();
        }

        private void btnReset_SaleFuel1_Click(object sender, EventArgs e)
        {
            ResetSelFuel1();
        }

    }
}
