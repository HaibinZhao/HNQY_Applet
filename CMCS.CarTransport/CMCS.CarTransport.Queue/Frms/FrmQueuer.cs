using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CMCS.CarTransport.DAO;
using CMCS.CarTransport.Queue.Core;
using CMCS.CarTransport.Queue.Enums;
using CMCS.CarTransport.Queue.Frms.BaseInfo.Autotruck;
using CMCS.CarTransport.Queue.Frms.Sys;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Entities.Sys;
using CMCS.Common.Enums;
using CMCS.Common.Utilities;
using CMCS.Common.Views;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.SuperGrid;
using LED.YB14;
using System.Security.Cryptography;

namespace CMCS.CarTransport.Queue.Frms
{
    public partial class FrmQueuer : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// ����Ψһ��ʶ��
        /// </summary>
        public static string UniqueKey = "FrmQueuer";

        public FrmQueuer()
        {
            InitializeComponent();
        }

        #region Vars

        CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();
        QueuerDAO queuerDAO = QueuerDAO.GetInstance();
        CommonDAO commonDAO = CommonDAO.GetInstance();
        CommonAppConfig commonAppConfig = CommonAppConfig.GetInstance();

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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.�ظ�1�ź�.ToString(), value ? "1" : "0");
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.�ظ�2�ź�.ToString(), value ? "1" : "0");
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.�ظ�3�ź�.ToString(), value ? "1" : "0");
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.�ظ�4�ź�.ToString(), value ? "1" : "0");
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

        bool rwer2OpenStatus = false;
        /// <summary>
        /// ����������״̬
        /// </summary>
        public bool Rwer2OpenStatus
        {
            get { return rwer2OpenStatus; }
            set
            {
                rwer2OpenStatus = value;
            }
        }

        bool rwer2Reading = false;
        /// <summary>
        /// ����������״̬
        /// </summary>
        public bool Rwer2Reading
        {
            get { return rwer2Reading; }
            set
            {
                rwer2Reading = value;
            }
        }


        public static PassCarQueuer passCarQueuer = new PassCarQueuer();

        ImperfectCar currentImperfectCar;
        /// <summary>
        /// ʶ���ѡ��ĳ���ƾ֤
        /// </summary>
        public ImperfectCar CurrentImperfectCar
        {
            get { return currentImperfectCar; }
            set
            {
                currentImperfectCar = value;

                if (value != null)
                    panCurrentCarNumber.Text = value.Voucher;
                else
                    panCurrentCarNumber.Text = "�ȴ�����";
            }
        }

        eFlowFlag currentFlowFlag = eFlowFlag.�ȴ�����;
        /// <summary>
        /// ��ǰҵ�����̱�ʶ
        /// </summary>
        public eFlowFlag CurrentFlowFlag
        {
            get { return currentFlowFlag; }
            set
            {
                currentFlowFlag = value;

                lblFlowFlag.Text = value.ToString();
            }
        }

        CmcsAutotruck currentAutotruck;
        /// <summary>
        /// ��ǰ��
        /// </summary>
        public CmcsAutotruck CurrentAutotruck
        {
            get { return currentAutotruck; }
            set
            {
                currentAutotruck = value;

                txtCarNumber_BuyFuel.Text = string.Empty; ;
                txtCarNumber_Goods.Text = string.Empty; ;
                txtCarNumber_Visit.Text = string.Empty; ;

                txtTagId_BuyFuel.Text = string.Empty; ;
                txtTagId_Goods.Text = string.Empty; ;
                txtTagId_Visit.Text = string.Empty; ;

                panCurrentCarNumber.Text = "�ȴ�����";

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ��Id.ToString(), value.Id);
                    commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ����.ToString(), value.CarNumber);


                    CmcsEPCCard ePCCard = Dbers.GetInstance().SelfDber.Get<CmcsEPCCard>(value.EPCCardId);
                    if (value.CarType == eCarType.�볧ú.ToString())
                    {
                        if (ePCCard != null) txtTagId_BuyFuel.Text = ePCCard.TagId;

                        txtCarNumber_BuyFuel.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_BuyFuel;

                        CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, this.CurrentAutotruck.CarType);

                        if (unFinishTransport != null)
                        {
                            ////���ݶ�ȡ���ı�ǩ����ȡ�Ŷӹ��ĳ�����Ϣ���ֳ�����������������ϢȻ�����Ŷӣ��ڰѳ���������
                            CmcsBuyFuelTransport CmcsBuyFuelTransport = commonDAO.SelfDber.Get<CmcsBuyFuelTransport>(unFinishTransport.TransportId);

                            if (CmcsBuyFuelTransport != null)
                            {
                                txtSupplierName_BuyFuel.Text = CmcsBuyFuelTransport.SupplierName;
                                txtTransportCompanyName_BuyFuel.Text = CmcsBuyFuelTransport.TransportCompanyName;
                                txtMineName_BuyFuel.Text = CmcsBuyFuelTransport.MineName;
                                cmbFuelName_BuyFuel.SelectedValue = CmcsBuyFuelTransport.FuelKindName;
                                txtTicketWeight_BuyFuel.Text = CmcsBuyFuelTransport.TicketWeight.ToString();
                                cmbSamplingType_BuyFuel.SelectedValue = CmcsBuyFuelTransport.SamplingType;
                                cmbSampling_BuyFuel.SelectedValue = CmcsBuyFuelTransport.FPSamplePlace;
                                btnSaveTransport_BuyFuel.Enabled = false;
                                btnSaveTransport_BuyFuel.Text = "���Ŷ�";
                            }
                        }
                        else
                        {
                            btnSaveTransport_BuyFuel.Enabled = true;
                            btnSaveTransport_BuyFuel.Text = "�� ��";
                        }
                    }
                    else if (value.CarType == eCarType.��������.ToString())
                    {
                        if (ePCCard != null) txtTagId_Goods.Text = ePCCard.TagId;

                        txtCarNumber_Goods.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_Goods;


                        CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, this.CurrentAutotruck.CarType);

                        if (unFinishTransport != null)
                        {
                            ////���ݶ�ȡ���ı�ǩ����ȡ�Ŷӹ��ĳ�����Ϣ���ֳ�����������������ϢȻ�����Ŷӣ��ڰѳ���������
                            CmcsGoodsTransport CmcsGoodsTransport = commonDAO.SelfDber.Get<CmcsGoodsTransport>(unFinishTransport.TransportId);

                            if (CmcsGoodsTransport != null)
                            {
                                txtSupplyUnitName_Goods.Text = CmcsGoodsTransport.SupplyUnitName;
                                txtReceiveUnitName_Goods.Text = CmcsGoodsTransport.ReceiveUnitName;
                                txtGoodsTypeName_Goods.Text = CmcsGoodsTransport.GoodsTypeName;

                                btnSaveTransport_Goods.Enabled = false;
                                btnSaveTransport_Goods.Text = "���Ŷ�";
                            }
                        }
                        else
                        {
                            btnSaveTransport_Goods.Enabled = true;
                            btnSaveTransport_Goods.Text = "�� ��";
                        }
                    }
                    else if (value.CarType == eCarType.���ó���.ToString())
                    {
                        if (ePCCard != null) txtTagId_Visit.Text = ePCCard.TagId;

                        txtCarNumber_Visit.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_Visit;
                    }

                    panCurrentCarNumber.Text = value.CarNumber;
                }
                else
                {
                    commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ��Id.ToString(), string.Empty);
                    commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ����.ToString(), string.Empty);

                    txtCarNumber_BuyFuel.Text = string.Empty;
                    txtCarNumber_Goods.Text = string.Empty;
                    txtCarNumber_Visit.Text = string.Empty;

                    txtTagId_BuyFuel.Text = string.Empty;
                    txtTagId_Goods.Text = string.Empty;
                    txtTagId_Visit.Text = string.Empty;

                    panCurrentCarNumber.Text = "�ȴ�����";


                    this.txtSupplierName_BuyFuel.Text = string.Empty; ;
                    txtSupplierName_BuyFuel.Text = string.Empty; ;
                    txtTransportCompanyName_BuyFuel.Text = string.Empty; ;
                    txtMineName_BuyFuel.Text = string.Empty; ;
                    txtTicketWeight_BuyFuel.Text = "0.00";

                    btnSaveTransport_BuyFuel.Enabled = true;
                    btnSaveTransport_BuyFuel.Text = "�� ��";
                }
            }
        }

        /// <summary>
        /// �Ƿ��в鿴��Ӧ�̼�ú��Ȩ��
        /// </summary>
        public bool HasShowSupplier = false;

        private CmcsLMYB lMYB;
        /// <summary>
        /// Ԥ�����
        /// </summary>
        public CmcsLMYB LMYB
        {
            get { return lMYB; }
            set
            {
                lMYB = value;
                if (value != null)
                    txt_YbNum.Text = value.YbNum;
                else
                    txt_YbNum.ResetText();
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
            lblFlowFlag.Visible = false;
#endif

            // ���ó���Զ�̿�������
            commonDAO.ResetAppRemoteControlCmd(commonAppConfig.AppIdentifier);

            LoadFuelkind(cmbFuelName_BuyFuel);
            LoadSampleType(cmbSamplingType_BuyFuel);
            LoadSample(cmbSampling_BuyFuel);
        }

        private void FrmQueuer_Load(object sender, EventArgs e)
        {
            HasShowSupplier = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "01", SelfVars.LoginUser);

        }

        private void FrmQueuer_Shown(object sender, EventArgs e)
        {
            InitHardware();

            InitForm();
        }

        private void FrmQueuer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ж���豸
            UnloadHardware();
        }

        #region �豸���

        #region IO������

        void Iocer_StatusChange(bool status)
        {
            // ����IO������״̬ 
            InvokeEx(() =>
            {
                slightIOC.LightColor = (status ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.IO������_����״̬.ToString(), status ? "1" : "0");
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
            });
        }

        /// <summary>
        /// ����ͨ��
        /// </summary>
        void LetPass()
        {
            if (this.CurrentImperfectCar == null) return;

            if (this.CurrentImperfectCar.PassWay == ePassWay.Way1)
            {
                this.iocControler.Gate1Up();
                this.iocControler.GreenLight1();
            }
            else if (this.CurrentImperfectCar.PassWay == ePassWay.Way2)
            {
                this.iocControler.Gate2Up();
                this.iocControler.GreenLight2();
            }
        }

        /// <summary>
        /// ���ǰ��
        /// </summary>
        void LetBlocking()
        {
            if (this.CurrentImperfectCar == null) return;

            if (this.CurrentImperfectCar.PassWay == ePassWay.Way1)
            {
                this.iocControler.Gate1Down();
                this.iocControler.RedLight1();
            }
            else if (this.CurrentImperfectCar.PassWay == ePassWay.Way2)
            {
                this.iocControler.Gate2Down();
                this.iocControler.RedLight2();
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
            // ���ն�����1״̬ 
            InvokeEx(() =>
             {
                 slightRwer1.LightColor = (status ? Color.Green : Color.Red);

                 commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.������1_����״̬.ToString(), status ? "1" : "0");
             });
        }

        void Rwer2_OnScanError(Exception ex)
        {
            Log4Neter.Error("������2", ex);
        }

        void Rwer2_OnStatusChange(bool status)
        {
            // ���ն�����2״̬ 
            InvokeEx(() =>
              {
                  slightRwer2.LightColor = (status ? Color.Green : Color.Red);

                  commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.������2_����״̬.ToString(), status ? "1" : "0");
              });
        }

        void Rwer2_OnScanSuccess(List<string> tags)
        {
            InvokeEx(() =>
             {
                 if (tags.Count > 0)
                 {
                     btnUPFK.Enabled = true;
                     Hardwarer.Rwer2.StopRead();
                     Rwer2Reading = false;

                     passCarQueuer.Enqueue(ePassWay.Way1, tags[0], true);
                     if (passCarQueuer.Count > 0)
                     {
                         this.CurrentAutotruck = null;
                         this.CurrentFlowFlag = eFlowFlag.��֤����;
                         timer1_Tick(null, null);
                     }
                 }
             });
        }

        /// <summary>
        /// ������ť
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUPFK_Click(object sender, EventArgs e)
        {
            if (Rwer2OpenStatus)
            {
                btnUPFK.Enabled = false;
                Hardwarer.Rwer2.StartRead();
                Rwer2Reading = true;
            }
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
            if (this.CurrentImperfectCar == null) return;

            if (this.CurrentImperfectCar.PassWay == ePassWay.Way1)
                UpdateLed1Show(value1, value2);
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.LED��1_����״̬.ToString(), value ? "1" : "0");
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
#if DEBUG
            FrmDebugConsole.GetInstance().Output("����LED1:|" + value1 + "|" + value2 + "|");
#else

#endif

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

                // IO������
                //Hardwarer.Iocer.OnReceived += new IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer.ReceivedEventHandler(Iocer_Received);
                //Hardwarer.Iocer.OnStatusChange += new IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer.StatusChangeHandler(Iocer_StatusChange);
                //Hardwarer.Iocer.OpenUDP(commonDAO.GetAppletConfigString("IO������_IP��ַ"), commonDAO.GetAppletConfigInt32("IO������_�˿�"));
                //if (!Hardwarer.Iocer.Status) MessageBoxEx.Show("IO����������ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.iocControler = new IocControler(Hardwarer.Iocer);

                // ������1
                #region �ĺ��Ŷ���������,�����ж���ʱע�͵�
                //Hardwarer.Rwer1.StartWith = commonDAO.GetAppletConfigString("������_��ǩ����");
                //Hardwarer.Rwer1.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer1_OnStatusChange);
                //Hardwarer.Rwer1.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer1_OnScanError);
                //success = Hardwarer.Rwer1.OpenCom(commonDAO.GetAppletConfigString("������1IP��ַ"), commonDAO.GetAppletConfigInt32("������1�˿�"), Convert.ToByte(commonDAO.GetAppletConfigInt32("������1����")));
                //if (!success) MessageBoxEx.Show("������1����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                #endregion
                // ������2
                Hardwarer.Rwer2.OnStatusChange += new RW.LZR12.Lzr12Rwer_Com.StatusChangeHandler(Rwer2_OnStatusChange);
                Hardwarer.Rwer2.OnScanError += new RW.LZR12.Lzr12Rwer_Com.ScanErrorEventHandler(Rwer2_OnScanError);
                Hardwarer.Rwer2.OnScanSuccess += new RW.LZR12.Lzr12Rwer_Com.ScanSuccessEventHandler(Rwer2_OnScanSuccess);
                success = Hardwarer.Rwer2.OpenCom(commonDAO.GetAppletConfigInt32("������_����"));
                Rwer2OpenStatus = success;
                if (!success) MessageBoxEx.Show("����������ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.LED��1_����״̬.ToString(), this.LED1ConnectStatus ? "1" : "0");

                #endregion

                #region ����Ƶ
                try
                {
                    string strIP = commonDAO.GetAppletConfigString("��������", "��Ƶ������IP��ַ");
                    string strPort = commonDAO.GetAppletConfigString("��������", "��Ƶ�������˿ں�");
                    string strID1 = commonDAO.GetAppletConfigString("1����Ƶͨ��ID");
                    string strID2 = commonDAO.GetAppletConfigString("2����Ƶͨ��ID");

                    IntPtr nPDLLHandle = (IntPtr)0;
                    IntPtr result1 = DHSDK.DPSDK_Create(DHSDK.dpsdk_sdk_type_e.DPSDK_CORE_SDK_SERVER, ref nPDLLHandle);//��ʼ�����ݽ����ӿ�
                    IntPtr result2 = DHSDK.DPSDK_InitExt();//��ʼ�����벥�Žӿ�
                    if (result1 == (IntPtr)0 && result2 == (IntPtr)0)
                    {

                        if (DHSDK.Logion(strIP, int.Parse(strPort), "system", "admin123", nPDLLHandle))
                        {
                            #region 1����Ƶ
                            IntPtr realseq = default(IntPtr);
                            string szCameraId1 = strID1;
                            if (DHSDK.StartPreview(panelEx3.Handle, szCameraId1, nPDLLHandle, realseq))
                            {
                                panelEx3.Refresh();
                            }
                            #endregion
                            #region 2����Ƶ
                            string szCameraId2 = strID2;
                            if (DHSDK.StartPreview(panelEx4.Handle, szCameraId2, nPDLLHandle, realseq))
                            {
                                panelEx4.Refresh();
                            }
                            #endregion
                        }

                    }
                    else
                    {
                        //return err
                        MessageBox.Show("��ʼ��ʧ��");
                    }

                }
                catch
                {


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
                Hardwarer.Iocer.OnReceived -= new IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer.ReceivedEventHandler(Iocer_Received);
                Hardwarer.Iocer.OnStatusChange -= new IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer.StatusChangeHandler(Iocer_StatusChange);

                Hardwarer.Iocer.ClostUDP();
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
                if (this.LED1ConnectStatus)
                {
                    YB14DynamicAreaLeder.SendDeleteDynamicAreasCommand(this.LED1nScreenNo, 1, "");
                    YB14DynamicAreaLeder.DeleteScreen(this.LED1nScreenNo);
                }
            }
            catch { }

            try
            {
                IntPtr nPDLLHandle = (IntPtr)0;
                IntPtr realseq = default(IntPtr);
                if (DHSDK.closevideo(nPDLLHandle, realseq))
                {

                    panelEx3.Refresh();
                    panelEx4.Refresh();
                }

            }
            catch { }
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

                switch (this.CurrentFlowFlag)
                {
                    case eFlowFlag.�ȴ�����:
                        #region

                        // PassWay.Way1

                        if (!this.Rwer2Reading)
                        {
                            List<string> tags = Hardwarer.Rwer1.ScanTags();
                            if (tags.Count > 0) passCarQueuer.Enqueue(ePassWay.Way1, tags[0], true);

                            if (passCarQueuer.Count > 0)
                            {
                                this.CurrentAutotruck = null;

                                this.CurrentFlowFlag = eFlowFlag.��֤����;
                            }
                        }

                        #endregion
                        break;

                    case eFlowFlag.��֤����:
                        #region

                        // �������޳�ʱ���ȴ�����
                        if (passCarQueuer.Count == 0)
                        {
                            this.CurrentFlowFlag = eFlowFlag.�ȴ�����;
                            break;
                        }

                        this.CurrentImperfectCar = passCarQueuer.Dequeue();

                        //// ��ʽһ������ʶ��ĳ��ƺŲ��ҳ�����Ϣ
                        this.CurrentAutotruck = carTransportDAO.GetAutotruckByCarNumber(this.CurrentImperfectCar.Voucher);

                        if (this.CurrentAutotruck == null)
                            //��ʽ��������ʶ��ı�ǩ�����ҳ�����Ϣ
                            this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);

                        if (this.CurrentAutotruck != null)
                        {
                            if (this.CurrentAutotruck.CarType == eCarType.�볧ú.ToString() && (this.CurrentAutotruck.CarriageLength <= 0 || this.CurrentAutotruck.CarriageWidth <= 0 || this.CurrentAutotruck.CarriageBottomToFloor <= 0))
                            {
                                MessageBoxEx.Show("������Ϣδ��������ֹ�Ǽ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ResetBuyFuel();
                                return;
                            }
                            UpdateLedShow(this.CurrentAutotruck.CarNumber);
                            if (this.CurrentAutotruck.IsUse == 1)
                            {
                                // �ж��Ƿ����δ���������¼�����ܴ����ȵ������Ŷӣ�Ȼ�����ڽ������û�ȷ�������ε��ˡ�
                                bool hasUnFinish = false;
                                CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, this.CurrentAutotruck.CarType);
                                if (unFinishTransport != null)
                                {
                                    timer1.Interval = 10000;
                                    hasUnFinish = true;
                                    this.CurrentFlowFlag = eFlowFlag.�ȴ�����;
                                }

                                if (!hasUnFinish)
                                {
                                    if (this.CurrentAutotruck.CarType == eCarType.�볧ú.ToString())
                                    {
                                        this.timer_BuyFuel_Cancel = false;
                                        this.CurrentFlowFlag = eFlowFlag.����¼��;
                                    }
                                    else if (this.CurrentAutotruck.CarType == eCarType.��������.ToString())
                                    {
                                        this.timer_Goods_Cancel = false;
                                        this.CurrentFlowFlag = eFlowFlag.����¼��;
                                    }
                                    else if (this.CurrentAutotruck.CarType == eCarType.���ó���.ToString())
                                    {
                                        this.timer_Visit_Cancel = false;
                                        this.CurrentFlowFlag = eFlowFlag.����¼��;
                                    }
                                }
                            }
                            else
                            {
                                UpdateLedShow(this.CurrentAutotruck.CarNumber, "��ͣ��");
                                this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ��ͣ�ã���ֹͨ��", 2, false);

                                timer1.Interval = 20000;
                            }
                        }
                        else
                        {
                            // ��ʽһ������ʶ��
                            this.voiceSpeaker.Speak("����δ�Ǽǣ���ֹͨ��", 2, false);
                            UpdateLedShow(this.CurrentImperfectCar.Voucher, "δ�Ǽ�");
                            //// ��ʽ����ˢ����ʽ
                            this.voiceSpeaker.Speak("����δ�Ǽǣ���ֹͨ��", 2, false);

                            timer1.Interval = 20000;
                        }

                        #endregion
                        break;
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
                // �볧ú
                LoadTodayUnFinishBuyFuelTransport();
                LoadTodayFinishBuyFuelTransport();

                // ��������
                LoadTodayUnFinishGoodsTransport();
                LoadTodayFinishGoodsTransport();

                // ���ó���
                LoadTodayUnFinishVisitTransport();
                LoadTodayFinishVisitTransport();
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

        /// <summary>
        /// �г����ڵ�ǰ��·��
        /// </summary>
        /// <returns></returns>
        bool HasCarOnCurrentWay()
        {
            if (this.CurrentImperfectCar == null) return false;

            if (this.CurrentImperfectCar.PassWay == ePassWay.UnKnow)
                return false;
            else if (this.CurrentImperfectCar.PassWay == ePassWay.Way1)
                return this.InductorCoil1 || this.InductorCoil2;
            else if (this.CurrentImperfectCar.PassWay == ePassWay.Way2)
                return this.InductorCoil3 || this.InductorCoil4;

            return true;
        }

        /// <summary>
        /// ����ú��
        /// </summary>
        void LoadFuelkind(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "FuelName";
            comboBoxEx.ValueMember = "Id";
            IList<CmcsFuelKind> list = Dbers.GetInstance().SelfDber.Entities<CmcsFuelKind>("where  IsUse='1' and ParentId is not null");
            list.Insert(0, new CmcsFuelKind { Id = "****", FuelName = "****" });
            comboBoxEx.DataSource = list;
        }

        private void cmbFuelName_BuyFuel_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedFuelKind_BuyFuel = cmbFuelName_BuyFuel.SelectedItem as CmcsFuelKind;
        }

        /// <summary>
        /// ���ز�����ʽ
        /// </summary>
        void LoadSampleType(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "Content";
            comboBoxEx.ValueMember = "Code";
            comboBoxEx.DataSource = commonDAO.GetCodeContentByKind("������ʽ");

            //comboBoxEx.Text = eSamplingType.��е����.ToString();
        }

        /// <summary>
        /// ���ز�����
        /// </summary>
        void LoadSample(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "Content";
            comboBoxEx.ValueMember = "Code";
            comboBoxEx.DataSource = commonDAO.GetCodeContentByKind("������е������");

            //comboBoxEx.Text = eSamplingType.��е����.ToString();
        }

        /// <summary>
        /// ִ��Զ������
        /// </summary>
        void ExecAppRemoteControlCmd()
        {
            // ��ȡ���µ�����
            CmcsAppRemoteControlCmd appRemoteControlCmd = commonDAO.GetNewestAppRemoteControlCmd(commonAppConfig.AppIdentifier);
            if (appRemoteControlCmd != null)
            {
                if (appRemoteControlCmd.CmdCode == "���Ƶ�բ")
                {
                    Log4Neter.Info("����Զ�����" + appRemoteControlCmd.CmdCode + "��������" + appRemoteControlCmd.Param);

                    if (appRemoteControlCmd.Param.Equals("Gate1Up", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate1Up();
                    else if (appRemoteControlCmd.Param.Equals("Gate1Down", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate1Down();
                    else if (appRemoteControlCmd.Param.Equals("Gate2Up", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate2Up();
                    else if (appRemoteControlCmd.Param.Equals("Gate2Down", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate2Down();

                    // ����ִ�н��
                    commonDAO.SetAppRemoteControlCmdResultCode(appRemoteControlCmd, eEquInfCmdResultCode.�ɹ�);
                }
            }
        }

        #endregion

        #region �볧úҵ��

        bool timer_BuyFuel_Cancel = true;

        private CmcsSupplier selectedSupplier_BuyFuel;
        /// <summary>
        /// ѡ��Ĺ�ú��λ
        /// </summary>
        public CmcsSupplier SelectedSupplier_BuyFuel
        {
            get { return selectedSupplier_BuyFuel; }
            set
            {
                selectedSupplier_BuyFuel = value;

                if (value != null)
                {
                    if (!HasShowSupplier)
                        txtSupplierName_BuyFuel.Text = "****";
                    else
                        txtSupplierName_BuyFuel.Text = value.Name;
                }
                else
                {
                    txtSupplierName_BuyFuel.ResetText();
                }
            }
        }

        private CmcsTransportCompany selectedTransportCompany_BuyFuel;
        /// <summary>
        /// ѡ������䵥λ
        /// </summary>
        public CmcsTransportCompany SelectedTransportCompany_BuyFuel
        {
            get { return selectedTransportCompany_BuyFuel; }
            set
            {
                selectedTransportCompany_BuyFuel = value;

                if (value != null)
                {
                    if (!HasShowSupplier)
                        txtTransportCompanyName_BuyFuel.Text = "****";
                    else
                        txtTransportCompanyName_BuyFuel.Text = value.Name;
                }
                else
                {
                    txtTransportCompanyName_BuyFuel.ResetText();
                }
            }
        }

        private CmcsMine selectedMine_BuyFuel;
        /// <summary>
        /// ѡ��Ŀ��
        /// </summary>
        public CmcsMine SelectedMine_BuyFuel
        {
            get { return selectedMine_BuyFuel; }
            set
            {
                selectedMine_BuyFuel = value;

                if (value != null)
                {
                    if (!HasShowSupplier)
                        txtMineName_BuyFuel.Text = "****";
                    else
                        txtMineName_BuyFuel.Text = value.Name;
                }
                else
                {
                    txtMineName_BuyFuel.ResetText();
                }
            }
        }

        private CmcsFuelKind selectedFuelKind_BuyFuel;
        /// <summary>
        /// ѡ���ú��
        /// </summary>
        public CmcsFuelKind SelectedFuelKind_BuyFuel
        {
            get { return selectedFuelKind_BuyFuel; }
            set
            {
                selectedFuelKind_BuyFuel = value;
                if (value != null)
                {
                    if (!HasShowSupplier)
                        cmbFuelName_BuyFuel.Text = "****";
                    else
                        cmbFuelName_BuyFuel.Text = value.FuelName;
                }
                else
                {
                    cmbFuelName_BuyFuel.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// ѡ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAutotruck_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmAutotruck_Select frm = new FrmAutotruck_Select("and CarType='" + eCarType.�볧ú.ToString() + "' and IsUse=1 order by CarNumber asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                passCarQueuer.Enqueue(ePassWay.UnKnow, frm.Output.CarNumber, false);
                this.CurrentFlowFlag = eFlowFlag.��֤����;
            }
        }

        /// <summary>
        /// ѡ��ú��λ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectSupplier_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmSupplier_Select frm = new FrmSupplier_Select("where IsUse='1' order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedSupplier_BuyFuel = frm.Output;
            }
        }

        /// <summary>
        /// ѡ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectMine_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmMine_Select frm = new FrmMine_Select("where IsUse='1' order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedMine_BuyFuel = frm.Output;
            }
        }

        /// <summary>
        /// ѡ�����䵥λ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectTransportCompany_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmTransportCompany_Select frm = new FrmTransportCompany_Select("where IsUse=1 order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedTransportCompany_BuyFuel = frm.Output;
            }
        }

        /// <summary>
        /// �³��Ǽ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewAutotruck_BuyFuel_Click(object sender, EventArgs e)
        {
            new FrmAutotruck_Oper("", true).Show();
        }

        /// <summary>
        /// ɨ���ά��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_QRCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_QRCode.Text))
                {
                    string rulst = DecryptNew(txt_QRCode.Text);
                    this.LMYB = commonDAO.SelfDber.Entity<CmcsLMYB>("where YbNum=:YbNum order by InFactoryTime desc", new { YbNum = rulst });
                    if (rulst.Split('-')[1] != DateTime.Now.ToString("yyyyMMdd"))
                        MessageBoxEx.Show("�ó�����Ԥ�����ڲ��ǽ���!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //MessageBoxEx.Show("�����쳣", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    if (this.LMYB != null)
                    {
                        this.SelectedSupplier_BuyFuel = commonDAO.SelfDber.Get<CmcsSupplier>(this.LMYB.SupplierId);
                        this.SelectedTransportCompany_BuyFuel = commonDAO.SelfDber.Get<CmcsTransportCompany>(this.LMYB.TransportCompanyId);
                        this.SelectedMine_BuyFuel = commonDAO.SelfDber.Get<CmcsMine>(this.LMYB.MineId);
                        this.SelectedFuelKind_BuyFuel = commonDAO.SelfDber.Get<CmcsFuelKind>(this.LMYB.FuelKindId);
                    }
                    //string[] values = rulst.Split('|');
                    //if (values.Length == 4)
                    //{
                    //    this.SelectedSupplier_BuyFuel = commonDAO.SelfDber.Entity<CmcsSupplier>("where Name=:Name and IsUse=1 order by CreateDate desc", new { Name = values[0] });
                    //    this.SelectedTransportCompany_BuyFuel = commonDAO.SelfDber.Entity<CmcsTransportCompany>("where Name=:Name and IsUse=1 order by CreateDate desc", new { Name = values[1] });
                    //    this.SelectedMine_BuyFuel = commonDAO.SelfDber.Entity<CmcsMine>("where Name=:Name and IsUse=1 order by CreateDate desc", new { Name = values[2] });
                    //    this.SelectedFuelKind_BuyFuel = commonDAO.SelfDber.Entity<CmcsFuelKind>("where FuelName=:FuelName and IsUse=1 order by CreateDate desc", new { FuelName = values[3] });
                    //    txt_QRCode.ResetText();
                    //}
                }
            }
            catch (Exception ex)
            {
                Log4Neter.Error("ɨ���ά��", ex);
            }
        }

        /// <summary>
        /// ѡ���볧ú��úԤ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectForecast_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmBuyFuelForecast_Select frm = new FrmBuyFuelForecast_Select();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedFuelKind_BuyFuel = commonDAO.SelfDber.Get<CmcsFuelKind>(frm.Output.FuelKindId);
                this.SelectedMine_BuyFuel = commonDAO.SelfDber.Get<CmcsMine>(frm.Output.MineId);
                this.SelectedSupplier_BuyFuel = commonDAO.SelfDber.Get<CmcsSupplier>(frm.Output.SupplierId);
                this.SelectedTransportCompany_BuyFuel = commonDAO.SelfDber.Get<CmcsTransportCompany>(frm.Output.TransportCompanyId);
            }
        }

        /// <summary>
        /// �����볧ú�����¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTransport_BuyFuel_Click(object sender, EventArgs e)
        {
            SaveBuyFuelTransport();
        }

        /// <summary>
        /// ���������¼
        /// </summary>
        /// <returns></returns>
        bool SaveBuyFuelTransport()
        {
            if (this.CurrentAutotruck == null)
            {
                MessageBoxEx.Show("��ѡ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedFuelKind_BuyFuel == null)
            {
                MessageBoxEx.Show("��ѡ��ú��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedMine_BuyFuel == null)
            {
                MessageBoxEx.Show("��ѡ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedSupplier_BuyFuel == null)
            {
                MessageBoxEx.Show("��ѡ��ú��λ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedTransportCompany_BuyFuel == null)
            {
                MessageBoxEx.Show("��ѡ�����䵥λ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (txtTicketWeight_BuyFuel.Value <= 0)
            {
                MessageBoxEx.Show("��������Ч�Ŀ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (isRulst(this.CurrentAutotruck))
            {
                MessageBoxEx.Show("�ó����ŷŲ����Ϲ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // �����볧ú�ŶӼ�¼��ͬʱ����������Ϣ�Լ����ƻ���������
                if (queuerDAO.JoinQueueBuyFuelTransport(this.CurrentAutotruck, this.SelectedSupplier_BuyFuel, this.SelectedMine_BuyFuel, this.SelectedTransportCompany_BuyFuel, this.SelectedFuelKind_BuyFuel, (decimal)txtTicketWeight_BuyFuel.Value, DateTime.Now, txtRemark_BuyFuel.Text, commonAppConfig.AppIdentifier, cmbSamplingType_BuyFuel.Text, cmbSampling_BuyFuel.Text, false, false, this.LMYB))
                {
                    btnSaveTransport_BuyFuel.Enabled = false;

                    UpdateLedShow("�Ŷӳɹ�", "���뿪");
                    MessageBoxEx.Show("�Ŷӳɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;

                    LoadTodayUnFinishBuyFuelTransport();
                    LoadTodayFinishBuyFuelTransport();

                    //LetPass();

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
        /// ���ݳ����жϸó����ŷ��Ƿ���ϱ�׼(��V���Ϸ���)
        /// </summary>
        /// <param name="autotruck"></param>
        /// <returns></returns>
        private bool isRulst(CmcsAutotruck autotruck)
        {
            bool rulst = false;
            string state = commonDAO.GetAppletConfigString("�����ŷű�׼��֤");
            string strList = commonDAO.GetAppletConfigString("���ϸ����ŷű�׼");
            if (state == "1")
            {
                if(strList.Contains(autotruck.EmissionStandard))
                {
                    return true;
                }

            }
            return rulst;
        }

        /// <summary>
        /// �����볧ú�����¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_BuyFuel_Click(object sender, EventArgs e)
        {
            ResetBuyFuel();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        void ResetBuyFuel()
        {
            this.timer_BuyFuel_Cancel = true;

            this.CurrentFlowFlag = eFlowFlag.�ȴ�����;

            this.CurrentAutotruck = null;
            this.SelectedMine_BuyFuel = null;
            this.SelectedSupplier_BuyFuel = null;
            this.SelectedTransportCompany_BuyFuel = null;
            this.LMYB = null;

            txtTagId_BuyFuel.ResetText();
            txtTicketWeight_BuyFuel.Value = 0;
            txtRemark_BuyFuel.ResetText();
            txt_QRCode.ResetText();
            btnSaveTransport_BuyFuel.Enabled = true;
            LetBlocking();

            UpdateLedShow("  �ȴ�����");

            // �������
            this.CurrentImperfectCar = null;
        }

        /// <summary>
        /// �볧ú�����¼ҵ��ʱ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_BuyFuel_Tick(object sender, EventArgs e)
        {
            if (this.timer_BuyFuel_Cancel) return;

            timer_BuyFuel.Stop();
            timer_BuyFuel.Interval = 2000;

            try
            {
                switch (this.CurrentFlowFlag)
                {
                    case eFlowFlag.����¼��:
                        #region

                        #endregion
                        break;

                    case eFlowFlag.�ȴ��뿪:
                        #region

                        // ��ǰ��·�ظ����ź�ʱ����
                        // if (!HasCarOnCurrentWay()) 

                        ResetBuyFuel();

                        // ����������
                        timer_BuyFuel.Interval = 4000;

                        #endregion
                        break;
                }

                // ��ǰ��·�ظ����ź�ʱ����
                //  if (!HasCarOnCurrentWay() && this.CurrentFlowFlag != eFlowFlag.�ȴ����� && (this.CurrentImperfectCar != null && this.CurrentImperfectCar.IsFromDevice)) ResetBuyFuel();
            }
            catch (Exception ex)
            {
                Log4Neter.Error("timer_BuyFuel_Tick", ex);
            }
            finally
            {
                timer_BuyFuel.Start();
            }
        }

        /// <summary>
        /// ��ȡδ��ɵ��볧ú��¼
        /// </summary>
        void LoadTodayUnFinishBuyFuelTransport()
        {
            superGridControl1_BuyFuel.PrimaryGrid.DataSource = queuerDAO.GetUnFinishBuyFuelTransport();
        }

        /// <summary>
        /// ��ȡָ����������ɵ��볧ú��¼
        /// </summary>
        void LoadTodayFinishBuyFuelTransport()
        {
            superGridControl2_BuyFuel.PrimaryGrid.DataSource = queuerDAO.GetFinishedBuyFuelTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        }

        /// <summary>
        /// ��ȡԤ����Ϣ
        /// </summary>
        /// <param name="lMYB">��úԤ��</param>
        void BorrowForecast_BuyFuel(CmcsLMYB lMYB)
        {
            if (lMYB == null) return;

            this.SelectedFuelKind_BuyFuel = commonDAO.SelfDber.Get<CmcsFuelKind>(lMYB.FuelKindId);
            this.SelectedMine_BuyFuel = commonDAO.SelfDber.Get<CmcsMine>(lMYB.MineId);
            this.SelectedSupplier_BuyFuel = commonDAO.SelfDber.Get<CmcsSupplier>(lMYB.SupplierId);
            this.SelectedTransportCompany_BuyFuel = commonDAO.SelfDber.Get<CmcsTransportCompany>(lMYB.TransportCompanyId);
        }

        /// <summary>
        /// ˫����ʱ���Զ���乩ú��λ��������Ϣ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl_BuyFuel_CellDoubleClick(object sender, DevComponents.DotNetBar.SuperGrid.GridCellDoubleClickEventArgs e)
        {
            GridRow gridRow = (sender as SuperGridControl).PrimaryGrid.ActiveRow as GridRow;
            if (gridRow == null) return;

            View_BuyFuelTransport entity = (gridRow.DataItem as View_BuyFuelTransport);
            if (entity != null)
            {
                this.SelectedFuelKind_BuyFuel = commonDAO.SelfDber.Get<CmcsFuelKind>(entity.FuelKindId);
                this.SelectedMine_BuyFuel = commonDAO.SelfDber.Get<CmcsMine>(entity.MineId);
                this.SelectedSupplier_BuyFuel = commonDAO.SelfDber.Get<CmcsSupplier>(entity.SupplierId);
                this.SelectedTransportCompany_BuyFuel = commonDAO.SelfDber.Get<CmcsTransportCompany>(entity.TransportCompanyId);
                cmbSamplingType_BuyFuel.Text = entity.SamplingType;
                cmbSampling_BuyFuel.Text = entity.FpSamplePlace;
            }
        }

        private void superGridControl1_BuyFuel_CellClick(object sender, GridCellClickEventArgs e)
        {
            View_BuyFuelTransport entity = e.GridCell.GridRow.DataItem as View_BuyFuelTransport;
            if (entity == null) return;

            // ������Ч״̬
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeBuyFuelTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl1_BuyFuel_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                View_BuyFuelTransport entity = gridRow.DataItem as View_BuyFuelTransport;
                if (entity == null) return;

                // �����Ч״̬
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
                if (!HasShowSupplier)
                {
                    gridRow.Cells["clmSupplierName"].Value = "****";
                    gridRow.Cells["clmTransportCompany"].Value = "****";
                    gridRow.Cells["clmFuelKind"].Value = "****";
                    gridRow.Cells["clmMineName"].Value = "****";
                }
            }
        }

        private void superGridControl2_BuyFuel_CellClick(object sender, GridCellClickEventArgs e)
        {
            View_BuyFuelTransport entity = e.GridCell.GridRow.DataItem as View_BuyFuelTransport;
            if (entity == null) return;

            // ������Ч״̬
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeBuyFuelTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl2_BuyFuel_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                View_BuyFuelTransport entity = gridRow.DataItem as View_BuyFuelTransport;
                if (entity == null) return;

                // �����Ч״̬
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
                if (!HasShowSupplier)
                {
                    gridRow.Cells["clmSupplierName"].Value = "****";
                    gridRow.Cells["clmTransportCompany"].Value = "****";
                    gridRow.Cells["clmFuelKind"].Value = "****";
                    gridRow.Cells["clmMineName"].Value = "****";
                }
            }
        }

        #endregion

        #region ��������ҵ��

        bool timer_Goods_Cancel = true;

        private CmcsSupplyReceive selectedSupplyUnit_Goods;
        /// <summary>
        /// ѡ��Ĺ�����λ
        /// </summary>
        public CmcsSupplyReceive SelectedSupplyUnit_Goods
        {
            get { return selectedSupplyUnit_Goods; }
            set
            {
                selectedSupplyUnit_Goods = value;

                if (value != null)
                {
                    txtSupplyUnitName_Goods.Text = value.UnitName;
                }
                else
                {
                    txtSupplyUnitName_Goods.ResetText();
                }
            }
        }

        private CmcsSupplyReceive selectedReceiveUnit_Goods;
        /// <summary>
        /// ѡ����ջ���λ
        /// </summary>
        public CmcsSupplyReceive SelectedReceiveUnit_Goods
        {
            get { return selectedReceiveUnit_Goods; }
            set
            {
                selectedReceiveUnit_Goods = value;

                if (value != null)
                {
                    txtReceiveUnitName_Goods.Text = value.UnitName;
                }
                else
                {
                    txtReceiveUnitName_Goods.ResetText();
                }
            }
        }

        private CmcsGoodsType selectedGoodsType_Goods;
        /// <summary>
        /// ѡ�����������
        /// </summary>
        public CmcsGoodsType SelectedGoodsType_Goods
        {
            get { return selectedGoodsType_Goods; }
            set
            {
                selectedGoodsType_Goods = value;

                if (value != null)
                {
                    txtGoodsTypeName_Goods.Text = value.GoodsName;
                }
                else
                {
                    txtGoodsTypeName_Goods.ResetText();
                }
            }
        }

        /// <summary>
        /// ѡ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAutotruck_Goods_Click(object sender, EventArgs e)
        {
            FrmAutotruck_Select frm = new FrmAutotruck_Select("and CarType='" + eCarType.��������.ToString() + "' and IsUse=1 order by CarNumber asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                passCarQueuer.Enqueue(ePassWay.UnKnow, frm.Output.CarNumber, false);
                this.CurrentFlowFlag = eFlowFlag.��֤����;
            }
        }

        /// <summary>
        /// ѡ�񹩻���λ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnbtnSelectSupply_Goods_Click(object sender, EventArgs e)
        {
            FrmSupplyReceive_Select frm = new FrmSupplyReceive_Select("where IsValid=1 order by UnitName asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedSupplyUnit_Goods = frm.Output;
            }
        }

        /// <summary>
        /// ѡ���ջ���λ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectReceive_Goods_Click(object sender, EventArgs e)
        {
            FrmSupplyReceive_Select frm = new FrmSupplyReceive_Select("where IsValid=1 order by UnitName asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedReceiveUnit_Goods = frm.Output;
            }
        }

        /// <summary>
        /// ѡ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectGoodsType_Goods_Click(object sender, EventArgs e)
        {
            FrmGoodsType_Select frm = new FrmGoodsType_Select();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedGoodsType_Goods = frm.Output;
            }
        }

        /// <summary>
        /// �³��Ǽ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewAutotruck_Goods_Click(object sender, EventArgs e)
        {
            // eCarType.�������� 

            new FrmAutotruck_Oper().Show();
        }

        /// <summary>
        /// �����ŶӼ�¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTransport_Goods_Click(object sender, EventArgs e)
        {
            SaveGoodsTransport();
        }

        /// <summary>
        /// ���������¼
        /// </summary>
        /// <returns></returns>
        bool SaveGoodsTransport()
        {
            if (this.CurrentAutotruck == null)
            {
                MessageBoxEx.Show("��ѡ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedSupplyUnit_Goods == null)
            {
                MessageBoxEx.Show("��ѡ�񹩻���λ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedReceiveUnit_Goods == null)
            {
                MessageBoxEx.Show("��ѡ���ջ���λ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedGoodsType_Goods == null)
            {
                MessageBoxEx.Show("��ѡ����������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // �����ŶӼ�¼
                if (queuerDAO.JoinQueueGoodsTransport(this.CurrentAutotruck, this.SelectedSupplyUnit_Goods, this.SelectedReceiveUnit_Goods, this.SelectedGoodsType_Goods, DateTime.Now, txtRemark_Goods.Text, commonAppConfig.AppIdentifier))
                {

                    MessageBoxEx.Show("�Ŷӳɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSaveTransport_Goods.Enabled = false;
                    this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;

                    UpdateLedShow("�Ŷӳɹ�", "���뿪");


                    LoadTodayUnFinishGoodsTransport();
                    LoadTodayFinishGoodsTransport();
                    //LetPass();
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
        /// ������Ϣ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Goods_Click(object sender, EventArgs e)
        {
            ResetGoods();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        void ResetGoods()
        {
            this.timer_Goods_Cancel = true;

            this.CurrentFlowFlag = eFlowFlag.�ȴ�����;

            this.CurrentAutotruck = null;
            this.SelectedSupplyUnit_Goods = null;
            this.SelectedReceiveUnit_Goods = null;
            this.SelectedGoodsType_Goods = null;

            txtTagId_Goods.Text = string.Empty;
            txtRemark_Goods.Text = string.Empty;

            btnSaveTransport_Goods.Enabled = true;

            LetBlocking();
            UpdateLedShow("  �ȴ�����");

            // �������
            this.CurrentImperfectCar = null;
        }

        /// <summary>
        /// �������������¼ҵ��ʱ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Goods_Tick(object sender, EventArgs e)
        {
            if (this.timer_Goods_Cancel) return;

            timer_Goods.Stop();
            timer_Goods.Interval = 2000;

            try
            {
                switch (this.CurrentFlowFlag)
                {
                    case eFlowFlag.����¼��:
                        #region



                        #endregion
                        break;

                    case eFlowFlag.�ȴ��뿪:
                        #region

                        // ��ǰ��·�ظ����ź�ʱ����
                        //if (!HasCarOnCurrentWay())
                        ResetGoods();

                        #endregion
                        break;
                }

                // ��ǰ��·�ظ����ź�ʱ����
                //if (!HasCarOnCurrentWay() && this.CurrentFlowFlag != eFlowFlag.�ȴ����� && (this.CurrentImperfectCar != null && this.CurrentImperfectCar.IsFromDevice)) ResetGoods();
            }
            catch (Exception ex)
            {
                Log4Neter.Error("timer_Goods_Tick", ex);
            }
            finally
            {
                timer_Goods.Start();
            }
        }

        /// <summary>
        /// ��ȡδ��ɵ��������ʼ�¼
        /// </summary>
        void LoadTodayUnFinishGoodsTransport()
        {
            superGridControl1_Goods.PrimaryGrid.DataSource = queuerDAO.GetUnFinishGoodsTransport();
        }

        /// <summary>
        /// ��ȡָ����������ɵ��������ʼ�¼
        /// </summary>
        void LoadTodayFinishGoodsTransport()
        {
            superGridControl2_Goods.PrimaryGrid.DataSource = queuerDAO.GetFinishedGoodsTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        }

        /// <summary>
        /// ˫����ʱ���Զ����¼����Ϣ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl_Goods_CellDoubleClick(object sender, DevComponents.DotNetBar.SuperGrid.GridCellDoubleClickEventArgs e)
        {
            GridRow gridRow = (sender as SuperGridControl).PrimaryGrid.ActiveRow as GridRow;
            if (gridRow == null) return;

            CmcsGoodsTransport entity = (gridRow.DataItem as CmcsGoodsTransport);
            if (entity != null)
            {
                this.SelectedSupplyUnit_Goods = commonDAO.SelfDber.Get<CmcsSupplyReceive>(entity.SupplyUnitId);
                this.SelectedReceiveUnit_Goods = commonDAO.SelfDber.Get<CmcsSupplyReceive>(entity.ReceiveUnitId);
                this.SelectedGoodsType_Goods = commonDAO.SelfDber.Get<CmcsGoodsType>(entity.GoodsTypeId);
            }
        }

        private void superGridControl1_Goods_CellClick(object sender, GridCellClickEventArgs e)
        {
            CmcsGoodsTransport entity = e.GridCell.GridRow.DataItem as CmcsGoodsTransport;
            if (entity == null) return;

            // ������Ч״̬
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeGoodsTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl1_Goods_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsGoodsTransport entity = gridRow.DataItem as CmcsGoodsTransport;
                if (entity == null) return;

                // �����Ч״̬
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
            }
        }

        private void superGridControl2_Goods_CellClick(object sender, GridCellClickEventArgs e)
        {
            CmcsGoodsTransport entity = e.GridCell.GridRow.DataItem as CmcsGoodsTransport;
            if (entity == null) return;

            // ������Ч״̬
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeGoodsTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl2_Goods_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsGoodsTransport entity = gridRow.DataItem as CmcsGoodsTransport;
                if (entity == null) return;

                // �����Ч״̬
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
            }
        }

        #endregion

        #region ���ó���ҵ��

        bool timer_Visit_Cancel = true;

        /// <summary>
        /// ѡ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAutotruck_Visit_Click(object sender, EventArgs e)
        {
            FrmAutotruck_Select frm = new FrmAutotruck_Select("and CarType='" + eCarType.���ó���.ToString() + "' and IsUse=1 order by CarNumber asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                passCarQueuer.Enqueue(ePassWay.UnKnow, frm.Output.CarNumber, false);
                this.CurrentFlowFlag = eFlowFlag.��֤����;
            }
        }

        /// <summary>
        /// �³��Ǽ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewAutotruck_Visit_Click(object sender, EventArgs e)
        {
            // eCarType.��������

            new FrmAutotruck_Oper().Show();
        }

        /// <summary>
        /// �����ŶӼ�¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTransport_Visit_Click(object sender, EventArgs e)
        {
            SaveVisitTransport();
        }

        /// <summary>
        /// ���������¼
        /// </summary>
        /// <returns></returns>
        bool SaveVisitTransport()
        {
            if (this.CurrentAutotruck == null)
            {
                MessageBoxEx.Show("��ѡ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // �������ó����ŶӼ�¼
                if (queuerDAO.JoinQueueVisitTransport(this.CurrentAutotruck, DateTime.Now, txtRemark_Visit.Text, commonAppConfig.AppIdentifier))
                {
                    LetPass();

                    btnSaveTransport_Visit.Enabled = false;
                    this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;

                    UpdateLedShow("�Ŷӳɹ�", "���뿪");
                    MessageBoxEx.Show("�Ŷӳɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadTodayUnFinishVisitTransport();
                    LoadTodayFinishVisitTransport();

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
        /// ������Ϣ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Visit_Click(object sender, EventArgs e)
        {
            ResetVisit();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        void ResetVisit()
        {
            this.timer_Visit_Cancel = true;

            this.CurrentFlowFlag = eFlowFlag.�ȴ�����;

            this.CurrentAutotruck = null;

            txtTagId_Visit.ResetText();
            txtRemark_Visit.ResetText();

            btnSaveTransport_Visit.Enabled = true;

            LetBlocking();
            UpdateLedShow("  �ȴ�����");

            // �������
            this.CurrentImperfectCar = null;
        }

        private void timer_Visit_Tick(object sender, EventArgs e)
        {
            if (this.timer_Visit_Cancel) return;

            timer_Visit.Stop();
            timer_Visit.Interval = 2000;

            try
            {
                switch (this.CurrentFlowFlag)
                {
                    case eFlowFlag.����¼��:
                        #region



                        #endregion
                        break;

                    case eFlowFlag.�ȴ��뿪:
                        #region

                        // ��ǰ��·�ظ����ź�ʱ����
                        //if (!HasCarOnCurrentWay())
                        ResetVisit();

                        #endregion
                        break;
                }

                // ��ǰ��·�ظ����ź�ʱ����
                //if (!HasCarOnCurrentWay() && this.CurrentFlowFlag != eFlowFlag.�ȴ����� && (this.CurrentImperfectCar != null && this.CurrentImperfectCar.IsFromDevice)) ResetVisit();
            }
            catch (Exception ex)
            {
                Log4Neter.Error("timer_Visit_Tick", ex);
            }
            finally
            {
                timer_Visit.Start();
            }
        }

        /// <summary>
        /// ��ȡδ��ɵ����ó�����¼
        /// </summary>
        void LoadTodayUnFinishVisitTransport()
        {
            superGridControl1_Visit.PrimaryGrid.DataSource = queuerDAO.GetUnFinishVisitTransport();
        }

        /// <summary>
        /// ��ȡָ����������ɵ����ó�����¼
        /// </summary>
        void LoadTodayFinishVisitTransport()
        {
            superGridControl2_Visit.PrimaryGrid.DataSource = queuerDAO.GetFinishedVisitTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        }

        private void superGridControl1_Visit_CellClick(object sender, GridCellClickEventArgs e)
        {
            CmcsVisitTransport entity = e.GridCell.GridRow.DataItem as CmcsVisitTransport;
            if (entity == null) return;

            // ������Ч״̬
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeVisitTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl1_Visit_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsVisitTransport entity = gridRow.DataItem as CmcsVisitTransport;
                if (entity == null) return;

                // �����Ч״̬
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
            }
        }

        private void superGridControl2_Visit_CellClick(object sender, GridCellClickEventArgs e)
        {
            CmcsVisitTransport entity = e.GridCell.GridRow.DataItem as CmcsVisitTransport;
            if (entity == null) return;

            // ������Ч״̬
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeVisitTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl2_Visit_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsVisitTransport entity = gridRow.DataItem as CmcsVisitTransport;
                if (entity == null) return;

                // �����Ч״̬
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
            }
        }

        #endregion

        #region ��������

        Pen redPen3 = new Pen(Color.Red, 3);
        Pen greenPen3 = new Pen(Color.Lime, 3);
        Pen greenPen1 = new Pen(Color.Lime, 1);

        /// <summary>
        /// ��ǰ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panCurrentCarNumber_Paint(object sender, PaintEventArgs e)
        {
            PanelEx panel = sender as PanelEx;

            // ���Ƶظ�1
            e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, 10, 15, 30);
            // ���Ƶظ�2                                                               
            e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, 25, 10, 25, 30);
            // ���Ʒָ���
            e.Graphics.DrawLine(greenPen1, 5, 34, 35, 34);
            // ���Ƶظ�3
            e.Graphics.DrawLine(this.InductorCoil3 ? redPen3 : greenPen3, 15, 38, 15, 58);
            // ���Ƶظ�4                                                               
            e.Graphics.DrawLine(this.InductorCoil4 ? redPen3 : greenPen3, 25, 38, 25, 58);
        }

        private void superGridControl_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            if (e.GridCell.GridColumn.DataPropertyName != "IsUse")
            {
                // ȡ������༭
                e.Cancel = true;
            }
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

        #region �ӡ������ַ���(���������)
        static string encryptKey = "Oyea";    //������Կ
        /// <summary>
        /// �����ַ���
        /// </summary>  
        /// <param name="str">Ҫ���ܵ��ַ���</param>  
        /// <returns>���ܺ���ַ���</returns>  
        public static string EncryptNew(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            try
            {
                String result = String.Empty;
                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //ʵ������/���������   

                byte[] key = Encoding.Unicode.GetBytes(encryptKey); //�����ֽ����飬�����洢��Կ    

                byte[] data = Encoding.Unicode.GetBytes(str);//�����ֽ����飬�����洢Ҫ���ܵ��ַ���  

                MemoryStream MStream = new MemoryStream(); //ʵ�����ڴ�������      

                //ʹ���ڴ���ʵ��������������   
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

                CStream.Write(data, 0, data.Length);  //���������д������      

                CStream.FlushFinalBlock();              //�ͷż�����    
                byte[] data1 = MStream.ToArray();
                for (int i = 0; i < data1.Length; i++)
                {
                    result += String.Format("{0:X}", Convert.ToInt32(data1[i])).PadLeft(2, '0');
                }
                return result;
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("����ʧ��" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return str;
            }
        }

        /// <summary>  
        /// �����ַ���   
        /// </summary>  
        /// <param name="str">Ҫ���ܵ��ַ���</param>  
        /// <returns>���ܺ���ַ���</returns>  
        public static string DecryptNew(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            try
            {
                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //ʵ������/���������    

                byte[] key = Encoding.Unicode.GetBytes(encryptKey); //�����ֽ����飬�����洢��Կ    

                char[] des = str.ToCharArray();
                byte[] data = new byte[des.Length / 2];
                for (int i = 0; i < des.Length; i++)
                {
                    String code = des[i].ToString() + des[i + 1].ToString();
                    data[i / 2] = Convert.ToByte(code, 16);
                    i++;
                }

                MemoryStream MStream = new MemoryStream(); //ʵ�����ڴ�������      

                //ʹ���ڴ���ʵ��������������       
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

                CStream.Write(data, 0, data.Length);      //���������д������     

                CStream.FlushFinalBlock();               //�ͷŽ�����      

                return Encoding.Unicode.GetString(MStream.ToArray());       //���ؽ��ܺ���ַ���  
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("����ʧ��" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return str;
            }
        }
        #endregion
    }
}
