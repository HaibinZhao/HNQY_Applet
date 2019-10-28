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
        /// 窗体唯一标识符
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
        /// 语音播报
        /// </summary>
        VoiceSpeaker voiceSpeaker = new VoiceSpeaker();

        bool inductorCoil1 = false;
        /// <summary>
        /// 地感1状态 true=有信号  false=无信号
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.地感1信号.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil1Port;
        /// <summary>
        /// 地感1端口
        /// </summary>
        public int InductorCoil1Port
        {
            get { return inductorCoil1Port; }
            set { inductorCoil1Port = value; }
        }

        bool inductorCoil2 = false;
        /// <summary>
        /// 地感2状态 true=有信号  false=无信号
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.地感2信号.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil2Port;
        /// <summary>
        /// 地感2端口
        /// </summary>
        public int InductorCoil2Port
        {
            get { return inductorCoil2Port; }
            set { inductorCoil2Port = value; }
        }

        bool inductorCoil3 = false;
        /// <summary>
        /// 地感3状态 true=有信号  false=无信号
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.地感3信号.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil3Port;
        /// <summary>
        /// 地感3端口
        /// </summary>
        public int InductorCoil3Port
        {
            get { return inductorCoil3Port; }
            set { inductorCoil3Port = value; }
        }

        bool inductorCoil4 = false;
        /// <summary>
        /// 地感4状态 true=有信号  false=无信号
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.地感4信号.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil4Port;
        /// <summary>
        /// 地感4端口
        /// </summary>
        public int InductorCoil4Port
        {
            get { return inductorCoil4Port; }
            set { inductorCoil4Port = value; }
        }

        bool inductorCoil5 = false;
        /// <summary>
        /// 地感5状态 true=有信号  false=无信号
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.地感5信号.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil5Port;
        /// <summary>
        /// 地感5端口
        /// </summary>
        public int InductorCoil5Port
        {
            get { return inductorCoil5Port; }
            set { inductorCoil5Port = value; }
        }

        bool inductorCoil6 = false;
        /// <summary>
        /// 地感6状态 true=有信号  false=无信号
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.地感6信号.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil6Port;
        /// <summary>
        /// 地感6端口
        /// </summary>
        public int InductorCoil6Port
        {
            get { return inductorCoil6Port; }
            set { inductorCoil6Port = value; }
        }


        bool inductorCoil7 = false;
        /// <summary>
        /// 地感7状态 true=有信号  false=无信号
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.地感7信号.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil7Port;
        /// <summary>
        /// 地感7端口
        /// </summary>
        public int InductorCoil7Port
        {
            get { return inductorCoil7Port; }
            set { inductorCoil7Port = value; }
        }


        bool inductorCoil8 = false;
        /// <summary>
        /// 地感8状态 true=有信号  false=无信号
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.地感8信号.ToString(), value ? "1" : "0");
            }
        }

        int inductorCoil8Port;
        /// <summary>
        /// 地感8端口
        /// </summary>
        public int InductorCoil8Port
        {
            get { return inductorCoil8Port; }
            set { inductorCoil8Port = value; }
        }

        bool autoHandMode = true;
        /// <summary>
        /// 自动模式=true  手动模式=false
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
        /// 识别或选择的车辆凭证
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
                    panCurrentCarNumber.Text = "等待车辆";
            }
        }

        ImperfectCar currentImperfectCar2;
        /// <summary>
        /// 识别或选择的车辆凭证
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
                    panCurrentCarNumber.Text = "等待车辆";
            }
        }

        eFlowFlag currentFlowFlag1 = eFlowFlag.等待车辆;
        eFlowFlag currentFlowFlag2 = eFlowFlag.等待车辆;
        /// <summary>
        /// 当前业务流程标识
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
        /// 当前业务流程标识
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
        /// 当前车
        /// </summary>
        public CmcsAutotruck CurrentAutotruck1
        {
            get { return currentAutotruck1; }
            set
            {
                currentAutotruck1 = value;

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车Id.ToString() + 1, value.Id);
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车号.ToString() + 1, value.CarNumber);

                    CmcsEPCCard ePCCard = Dbers.GetInstance().SelfDber.Get<CmcsEPCCard>(value.EPCCardId);
                    if (value.CarType == eCarType.销售煤.ToString())
                    {
                        if (ePCCard != null) txtTagId_SaleFuel1.Text = ePCCard.TagId;

                        txtCarNumber_SaleFuel1.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_SaleFuel1;
                    }


                    panCurrentCarNumber.Text = value.CarNumber;


                }
                else
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车Id.ToString() + 1, string.Empty);
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车号.ToString() + 1, string.Empty);

                    txtCarNumber_SaleFuel1.ResetText();

                    txtTagId_SaleFuel1.ResetText();

                    panCurrentCarNumber.ResetText();
                }
            }
        }


        CmcsAutotruck currentAutotruck2;
        /// <summary>
        /// 当前车
        /// </summary>
        public CmcsAutotruck CurrentAutotruck2
        {
            get { return currentAutotruck2; }
            set
            {
                currentAutotruck2 = value;

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车Id.ToString() + 2, value.Id);
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车号.ToString() + 2, value.CarNumber);

                    CmcsEPCCard ePCCard = Dbers.GetInstance().SelfDber.Get<CmcsEPCCard>(value.EPCCardId);
                    if (value.CarType == eCarType.销售煤.ToString())
                    {
                        if (ePCCard != null) txtTagId_SaleFuel2.Text = ePCCard.TagId;

                        txtCarNumber_SaleFuel2.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_SaleFuel2;
                    }


                    panCurrentCarNumber.Text = value.CarNumber;


                }
                else
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车Id.ToString() + 2, string.Empty);
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车号.ToString() + 2, string.Empty);

                    txtCarNumber_SaleFuel2.ResetText();

                    txtTagId_SaleFuel2.ResetText();

                    panCurrentCarNumber.ResetText();
                }
            }
        }
        #endregion

        /// <summary>
        /// 窗体初始化
        /// </summary>
        private void InitForm()
        {
#if DEBUG
            lblFlowFlag.Visible = true;
            FrmDebugConsole.GetInstance().Show();
#else
            //lblFlowFlag.Visible = false;
#endif

            // 默认自动
            sbtnChangeAutoHandMode.Value = true;

            // 重置程序远程控制命令
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


        #region 设备相关

        #region IO控制器

        void Iocer_StatusChange(bool status)
        {
            // 接收设备状态 
            InvokeEx(() =>
            {
                slightIOC.LightColor = (status ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.IO控制器_连接状态.ToString(), status ? "1" : "0");
            });
        }

        /// <summary>
        /// IO控制器接收数据时触发
        /// </summary>
        /// <param name="receiveValue"></param>
        void Iocer_Received(int[] receiveValue)
        {
            // 接收地感状态  
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
        /// 后方1升杆
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
        /// 后方1降杆
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
        /// 后方2降杆
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
        /// 后方2升杆
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
        /// 前方1升杆
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
        /// 前方1降杆
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
        /// 前方2降杆
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
        /// 前方2升杆
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

        #region 读卡器

        void Rwer1_OnScanError(Exception ex)
        {
            Log4Neter.Error("读卡器1", ex);
        }

        void Rwer1_OnStatusChange(bool status)
        {
            // 接收设备状态 
            InvokeEx(() =>
            {
                slightRwer1.LightColor = (status ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.读卡器1_连接状态.ToString(), status ? "1" : "0");
            });
        }

        void Rwer2_OnScanError(Exception ex)
        {
            Log4Neter.Error("读卡器2", ex);
        }

        void Rwer2_OnStatusChange(bool status)
        {
            // 接收设备状态 
            InvokeEx(() =>
            {
                slightRwer2.LightColor = (status ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.读卡器2_连接状态.ToString(), status ? "1" : "0");
            });
        }

        #endregion

        #region LED显示屏

        /// <summary>
        /// 生成12字节的文本内容
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
        /// 更新LED动态区域
        /// </summary>
        /// <param name="value1">第一行内容</param>
        /// <param name="value2">第二行内容</param>
        private void UpdateLedShow(string value1 = "", string value2 = "")
        {
            UpdateLed1Show(value1, value2);
            UpdateLed2Show(value1, value2);
        }

        #region LED1控制卡

        /// <summary>
        /// LED1控制卡屏号
        /// </summary>
        int LED1nScreenNo = 1;
        /// <summary>
        /// LED1动态区域号
        /// </summary>
        int LED1DYArea_ID = 1;
        /// <summary>
        /// LED1更新标识
        /// </summary>
        bool LED1m_bSendBusy = false;

        private bool _LED1ConnectStatus;
        /// <summary>
        /// LED1连接状态
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.LED屏1_连接状态.ToString(), value ? "1" : "0");
            }
        }

        /// <summary>
        /// LED1显示内容文本
        /// </summary>
        string LED1TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Led1TempFile.txt");

        /// <summary>
        /// LED1上一次显示内容
        /// </summary>
        string LED1PrevLedFileContent = string.Empty;

        /// <summary>
        /// 更新LED1动态区域
        /// </summary>
        /// <param name="value1">第一行内容</param>
        /// <param name="value2">第二行内容</param>
        private void UpdateLed1Show(string value1 = "", string value2 = "")
        {
            FrmDebugConsole.GetInstance().Output("更新LED1:|" + value1 + "|" + value2 + "|");

            if (!this.LED1ConnectStatus) return;
            if (this.LED1PrevLedFileContent == value1 + value2) return;

            string ledContent = GenerateFillLedContent12(value1) + GenerateFillLedContent12(value2);

            File.WriteAllText(this.LED1TempFile, ledContent, Encoding.UTF8);

            if (LED1m_bSendBusy == false)
            {
                LED1m_bSendBusy = true;

                //int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID);
                //if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("更新LED动态区域", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

                LED1m_bSendBusy = false;
            }

            this.LED1PrevLedFileContent = value1 + value2;
        }

        #endregion

        #region LED2控制卡

        /// <summary>
        /// LED2控制卡屏号
        /// </summary>
        int LED2nScreenNo = 1;
        /// <summary>
        /// LED2动态区域号
        /// </summary>
        int LED2DYArea_ID = 1;
        /// <summary>
        /// LED2更新标识
        /// </summary>
        bool LED2m_bSendBusy = false;

        private bool _LED2ConnectStatus;
        /// <summary>
        /// LED2连接状态
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

                commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.LED屏2_连接状态.ToString(), value ? "1" : "0");
            }
        }

        /// <summary>
        /// LED2显示内容文本
        /// </summary>
        string LED2TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Led2TempFile.txt");

        /// <summary>
        /// LED2上一次显示内容
        /// </summary>
        string LED2PrevLedFileContent = string.Empty;

        /// <summary>
        /// 更新LED2动态区域
        /// </summary>
        /// <param name="value1">第一行内容</param>
        /// <param name="value2">第二行内容</param>
        private void UpdateLed2Show(string value1 = "", string value2 = "")
        {
            FrmDebugConsole.GetInstance().Output("更新LED2:|" + value1 + "|" + value2 + "|");

            if (!this.LED1ConnectStatus) return;
            if (this.LED2PrevLedFileContent == value1 + value2) return;

            string ledContent = GenerateFillLedContent12(value1) + GenerateFillLedContent12(value2);

            File.WriteAllText(this.LED1TempFile, ledContent, Encoding.UTF8);

            if (LED2m_bSendBusy == false)
            {
                LED2m_bSendBusy = true;

                //int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED2nScreenNo, this.LED2DYArea_ID);
                //if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("更新LED动态区域", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

                LED2m_bSendBusy = false;
            }

            this.LED2PrevLedFileContent = value1 + value2;
        }

        #endregion

        #endregion

        #region 设备初始化与卸载

        /// <summary>
        /// 初始化外接设备
        /// </summary>
        private void InitHardware()
        {
            try
            {
                bool success = false;

                this.InductorCoil1Port = commonDAO.GetAppletConfigInt32("IO控制器_地感1端口");
                this.InductorCoil2Port = commonDAO.GetAppletConfigInt32("IO控制器_地感2端口");
                this.InductorCoil3Port = commonDAO.GetAppletConfigInt32("IO控制器_地感3端口");
                this.InductorCoil4Port = commonDAO.GetAppletConfigInt32("IO控制器_地感4端口");
                this.InductorCoil5Port = commonDAO.GetAppletConfigInt32("IO控制器_地感5端口");
                this.InductorCoil6Port = commonDAO.GetAppletConfigInt32("IO控制器_地感6端口");
                this.InductorCoil7Port = commonDAO.GetAppletConfigInt32("IO控制器_地感7端口");
                this.InductorCoil8Port = commonDAO.GetAppletConfigInt32("IO控制器_地感8端口");


                // IO控制器
                Hardwarer.Iocer.OnReceived += new IOC.JMDM20DIOV2.JMDM20DIOV2Iocer.ReceivedEventHandler(Iocer_Received);
                Hardwarer.Iocer.OnStatusChange += new IOC.JMDM20DIOV2.JMDM20DIOV2Iocer.StatusChangeHandler(Iocer_StatusChange);
                success = Hardwarer.Iocer.OpenCom(commonDAO.GetAppletConfigInt32("IO控制器_串口"), commonDAO.GetAppletConfigInt32("IO控制器_波特率"), commonDAO.GetAppletConfigInt32("IO控制器_数据位"), (StopBits)commonDAO.GetAppletConfigInt32("IO控制器_停止位"), (Parity)commonDAO.GetAppletConfigInt32("IO控制器_校验位"));
                if (!success) MessageBoxEx.Show("IO控制器连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.iocControler = new IocControler(Hardwarer.Iocer);


                // 读卡器1
                Hardwarer.Rwer1.StartWith = commonDAO.GetAppletConfigString("读卡器_标签过滤");
                Hardwarer.Rwer1.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer1_OnStatusChange);
                Hardwarer.Rwer1.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer1_OnScanError);
                success = Hardwarer.Rwer1.OpenCom(commonDAO.GetAppletConfigInt32("读卡器1_串口"));
                if (!success) MessageBoxEx.Show("读卡器1连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // 读卡器2
                Hardwarer.Rwer2.StartWith = commonDAO.GetAppletConfigString("读卡器_标签过滤");
                Hardwarer.Rwer2.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer2_OnStatusChange);
                Hardwarer.Rwer2.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer2_OnScanError);
                success = Hardwarer.Rwer2.OpenCom(commonDAO.GetAppletConfigInt32("读卡器2_串口"));
                if (!success) MessageBoxEx.Show("读卡器2连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                #region LED控制卡1

                string led1SocketIP = commonDAO.GetAppletConfigString("LED显示屏1_IP地址");
                if (!string.IsNullOrEmpty(led1SocketIP))
                {
                    int nResult = YB14DynamicAreaLeder.AddScreen(YB14DynamicAreaLeder.CONTROLLER_BX_5E1, this.LED1nScreenNo, YB14DynamicAreaLeder.SEND_MODE_NETWORK, 96, 32, 1, 1, "", 0, led1SocketIP, 5005, "");
                    if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                    {
                        nResult = YB14DynamicAreaLeder.AddScreenDynamicArea(this.LED1nScreenNo, this.LED1DYArea_ID, 0, 10, 1, "", 0, 0, 0, 96, 32, 255, 0, 255, 7, 6, 1);
                        if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                        {
                            nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED1nScreenNo, this.LED1DYArea_ID, this.LED1TempFile, 0, "宋体", 12, 0, 120, 1, 3, 0);
                            if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                            {
                                // 初始化成功
                                this.LED1ConnectStatus = true;
                                UpdateLed1Show("  等待车辆");

                            }
                            else
                            {
                                this.LED1ConnectStatus = false;
                                Log4Neter.Error("初始化LED1控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicAreaFile", nResult)));
                                MessageBoxEx.Show("LED1控制卡连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            this.LED1ConnectStatus = false;
                            Log4Neter.Error("初始化LED1控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicArea", nResult)));
                            MessageBoxEx.Show("LED1控制卡连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        this.LED1ConnectStatus = false;
                        Log4Neter.Error("初始化LED1控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreen", nResult)));
                        MessageBoxEx.Show("LED1控制卡连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                #endregion

                #region LED控制卡2

                string led2SocketIP = commonDAO.GetAppletConfigString("LED显示屏2_IP地址");
                if (!string.IsNullOrEmpty(led2SocketIP))
                {
                    int nResult = YB14DynamicAreaLeder.AddScreen(YB14DynamicAreaLeder.CONTROLLER_BX_5E1, this.LED2nScreenNo, YB14DynamicAreaLeder.SEND_MODE_NETWORK, 96, 32, 1, 1, "", 0, led2SocketIP, 5005, "");
                    if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                    {
                        nResult = YB14DynamicAreaLeder.AddScreenDynamicArea(this.LED2nScreenNo, this.LED2DYArea_ID, 0, 10, 1, "", 0, 0, 0, 96, 32, 255, 0, 255, 7, 6, 1);
                        if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                        {
                            nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED2nScreenNo, this.LED2DYArea_ID, this.LED2TempFile, 0, "宋体", 12, 0, 120, 1, 3, 0);
                            if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
                            {
                                // 初始化成功
                                this.LED2ConnectStatus = true;
                                UpdateLed2Show("  等待车辆");
                            }
                            else
                            {
                                this.LED2ConnectStatus = false;
                                Log4Neter.Error("初始化LED2控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicAreaFile", nResult)));
                                MessageBoxEx.Show("LED2控制卡连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            this.LED2ConnectStatus = false;
                            Log4Neter.Error("初始化LED2控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreenDynamicArea", nResult)));
                            MessageBoxEx.Show("LED2控制卡连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        this.LED2ConnectStatus = false;
                        Log4Neter.Error("初始化LED2控制卡", new Exception(YB14DynamicAreaLeder.GetErrorMessage("AddScreen", nResult)));
                        MessageBoxEx.Show("LED2控制卡连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                #endregion

                timer1.Enabled = true;
            }
            catch (Exception ex)
            {
                Log4Neter.Error("设备初始化", ex);
            }
        }

        /// <summary>
        /// 卸载设备
        /// </summary>
        private void UnloadHardware()
        {
            // 注意此段代码
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

            // 卸载设备
            UnloadHardware();
        }
        #endregion

        #endregion

        #region 道闸控制按钮

        /// <summary>
        /// 道闸1升杆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate1Up_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate1Up();
        }

        /// <summary>
        /// 道闸1降杆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate1Down_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate1Down();
        }
        /// <summary>
        /// 道闸2升杆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate2Up_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate2Up();
        }

        /// <summary>
        /// 道闸2降杆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate2Down_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate2Down();
        }

        /// <summary>
        /// 道闸3升杆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate3Up_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate3Up();
        }

        /// <summary>
        /// 道闸3降杆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate3Down_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate3Down();
        }
        /// <summary>
        /// 道闸4升杆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate4Up_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate4Up();
        }

        /// <summary>
        /// 道闸4降杆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGate4Down_Click(object sender, EventArgs e)
        {
            if (this.iocControler != null) this.iocControler.Gate4Down();
        }
        #endregion

        #region 公共业务

        /// <summary>
        /// 读卡、车号识别任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Interval = 2000;

            try
            {
                // 执行远程命令
                ExecAppRemoteControlCmd();

                try
                {
                    switch (this.CurrentFlowFlag1)
                    {
                        case eFlowFlag.等待车辆:
                            #region

                            // PassWay.Way1
                            if (this.InductorCoil1)
                            {
                                // 当读卡区域地感有信号，触发读卡或者车号识别

                                List<string> tags = Hardwarer.Rwer1.ScanTags();
                                if (tags.Count > 0) passCarQueuer1.Enqueue(ePassWay.Way1, tags[0], true);
                            }
                            if (passCarQueuer1.Count > 0) this.CurrentFlowFlag1 = eFlowFlag.识别车辆;

                            #endregion
                            break;

                        case eFlowFlag.识别车辆:
                            #region

                            // 队列中无车时，等待车辆
                            if (passCarQueuer1.Count == 0)
                            {
                                this.CurrentFlowFlag1 = eFlowFlag.等待车辆;
                                break;
                            }

                            this.CurrentImperfectCar1 = passCarQueuer1.Dequeue();

                            // 方式一：根据识别的车牌号查找车辆信息
                            this.CurrentAutotruck1 = carTransportDAO.GetAutotruckByCarNumber(this.CurrentImperfectCar1.Voucher);
                            UpdateLed1Show(this.CurrentImperfectCar1.Voucher);
                            //// 方式二：根据识别的标签卡查找车辆信息
                            ////this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);

                            if (this.CurrentAutotruck1 != null)
                            {
                                if (this.CurrentAutotruck1.IsUse == 1)
                                {
                                    if (this.CurrentAutotruck1.CarType == eCarType.销售煤.ToString())
                                    {
                                        this.timer_SaleFuel1_Cancel = false;
                                        this.CurrentFlowFlag1 = eFlowFlag.验证信息;
                                    }
                                }
                                else
                                {
                                    UpdateLed1Show(this.CurrentAutotruck1.CarNumber, "已停用");
                                    this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck1.CarNumber + " 已停用，禁止通过", 2, false);

                                    timer1.Interval = 20000;
                                }
                            }
                            else
                            {
                                UpdateLed1Show(this.CurrentImperfectCar1.Voucher, "未登记");

                                // 方式一：车号识别
                                this.voiceSpeaker.Speak("车牌号 " + this.CurrentImperfectCar1.Voucher + " 未登记，禁止通过", 2, false);
                                //// 方式二：刷卡方式
                                //this.voiceSpeaker.Speak("卡号未登记，禁止通过", 2, false);

                                timer1.Interval = 20000;
                            }

                            #endregion
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log4Neter.Error("timer1_Tick：CurrentFlowFlag1", ex);
                }

                try
                {
                    switch (this.CurrentFlowFlag2)
                    {
                        case eFlowFlag.等待车辆:
                            #region

                            // PassWay.Way2
                            if (this.InductorCoil2)
                            {
                                // 当读卡区域地感有信号，触发读卡或者车号识别

                                List<string> tags = Hardwarer.Rwer2.ScanTags();
                                if (tags.Count > 0) passCarQueuer2.Enqueue(ePassWay.Way2, tags[0], true);
                            }
                            if (passCarQueuer2.Count > 0) this.CurrentFlowFlag2 = eFlowFlag.识别车辆;

                            #endregion
                            break;

                        case eFlowFlag.识别车辆:
                            #region

                            // 队列中无车时，等待车辆
                            if (passCarQueuer2.Count == 0)
                            {
                                this.CurrentFlowFlag2 = eFlowFlag.等待车辆;
                                break;
                            }

                            this.CurrentImperfectCar2 = passCarQueuer2.Dequeue();

                            // 方式一：根据识别的车牌号查找车辆信息
                            this.CurrentAutotruck2 = carTransportDAO.GetAutotruckByCarNumber(this.CurrentImperfectCar2.Voucher);
                            UpdateLed2Show(this.CurrentImperfectCar2.Voucher);
                            //// 方式二：根据识别的标签卡查找车辆信息
                            ////this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);

                            if (this.CurrentAutotruck2 != null)
                            {
                                if (this.CurrentAutotruck2.IsUse == 2)
                                {
                                    if (this.CurrentAutotruck2.CarType == eCarType.销售煤.ToString())
                                    {
                                        this.timer_SaleFuel2_Cancel = false;
                                        this.CurrentFlowFlag2 = eFlowFlag.验证信息;
                                    }
                                }
                                else
                                {
                                    UpdateLed2Show(this.CurrentAutotruck2.CarNumber, "已停用");
                                    this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck2.CarNumber + " 已停用，禁止通过", 2, false);

                                    timer2.Interval = 20000;
                                }
                            }
                            else
                            {
                                UpdateLed2Show(this.CurrentImperfectCar2.Voucher, "未登记");

                                // 方式一：车号识别
                                this.voiceSpeaker.Speak("车牌号 " + this.CurrentImperfectCar2.Voucher + " 未登记，禁止通过", 2, false);
                                //// 方式二：刷卡方式
                                //this.voiceSpeaker.Speak("卡号未登记，禁止通过", 2, false);

                                timer2.Interval = 20000;
                            }

                            #endregion
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log4Neter.Error("timer1_Tick：CurrentFlowFlag2", ex);
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
        /// 慢速任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            // 三分钟执行一次
            timer2.Interval = 180000;

            try
            {

                // 销售煤 
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
        /// 1道有车辆在当前道路上
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
        /// 有车辆在当前道路上
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
        /// 执行远程命令
        /// </summary>
        void ExecAppRemoteControlCmd()
        {
            // 获取最新的命令
            CmcsAppRemoteControlCmd appRemoteControlCmd = commonDAO.GetNewestAppRemoteControlCmd(CommonAppConfig.GetInstance().AppIdentifier);
            if (appRemoteControlCmd != null)
            {
                if (appRemoteControlCmd.CmdCode == "控制道闸")
                {
                    Log4Neter.Info("接收远程命令：" + appRemoteControlCmd.CmdCode + "，参数：" + appRemoteControlCmd.Param);

                    if (appRemoteControlCmd.Param.Equals("Gate1Up", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate1Up();
                    else if (appRemoteControlCmd.Param.Equals("Gate1Down", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate1Down();
                    else if (appRemoteControlCmd.Param.Equals("Gate3Up", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate3Up();
                    else if (appRemoteControlCmd.Param.Equals("Gate3Down", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate3Down();

                    // 更新执行结果
                    commonDAO.SetAppRemoteControlCmdResultCode(appRemoteControlCmd, eEquInfCmdResultCode.成功);
                }
            }
        }

        /// <summary>
        /// 切换手动/自动模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sbtnChangeAutoHandMode_ValueChanged(object sender, EventArgs e)
        {
            this.AutoHandMode = sbtnChangeAutoHandMode.Value;
        }

        #endregion

        #region 销售煤业务

        bool timer_SaleFuel1_Cancel = true;
        bool timer_SaleFuel2_Cancel = true;

        CmcsSaleFuelTransport currentSaleFuelTransport1;
        /// <summary>
        /// 当前运输记录1
        /// </summary>
        public CmcsSaleFuelTransport CurrentSaleFuelTransport1
        {
            get { return currentSaleFuelTransport1; }
            set
            {
                currentSaleFuelTransport1 = value;

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前运输记录Id.ToString(), value.Id);

                    txt_YBNumber1.Text = value.TransportSalesNum;
                    txt_TransportNo1.Text = value.TransportNo;
                    txt_Consignee1.Text = value.SupplierName;
                    txt_TransportCompayName1.Text = value.TransportCompanyName;
                }
                else
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前运输记录Id.ToString(), string.Empty);
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
        /// 当前运输记录2
        /// </summary>
        public CmcsSaleFuelTransport CurrentSaleFuelTransport2
        {
            get { return currentSaleFuelTransport2; }
            set
            {
                currentSaleFuelTransport2 = value;

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前运输记录Id.ToString(), value.Id);

                    txt_YBNumber1.Text = value.TransportSalesNum;
                    txt_TransportNo1.Text = value.TransportNo;
                    txt_Consignee1.Text = value.SupplierName;
                    txt_TransportCompayName1.Text = value.TransportCompanyName;
                }
                else
                {
                    commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前运输记录Id.ToString(), string.Empty);
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

        #region 其他函数

        Pen redPen3 = new Pen(Color.Red, 3);
        Pen greenPen3 = new Pen(Color.Lime, 3);
        Pen greenPen1 = new Pen(Color.Lime, 1);

        /// <summary>
        /// 当前仪表重量面板绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panCurrentWeight_Paint(object sender, PaintEventArgs e)
        {
            PanelEx panel = sender as PanelEx;

            // 绘制地感1
            e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, 10, 15, 30);
            // 绘制地感2                                                               
            e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, 25, 10, 25, 30);
            // 绘制地感3
            e.Graphics.DrawLine(this.InductorCoil3 ? redPen3 : greenPen3, 35, 10, 35, 30);
            // 绘制地感4                                                               
            e.Graphics.DrawLine(this.InductorCoil4 ? redPen3 : greenPen3, 45, 10, 45, 30);
            // 绘制分割线
            e.Graphics.DrawLine(greenPen1, 5, 34, 55, 34);
            // 绘制地感5
            e.Graphics.DrawLine(this.InductorCoil5 ? redPen3 : greenPen3, 15, 38, 15, 58);
            // 绘制地感6                                                               
            e.Graphics.DrawLine(this.InductorCoil6 ? redPen3 : greenPen3, 25, 38, 25, 58);
            // 绘制地感7
            e.Graphics.DrawLine(this.InductorCoil7 ? redPen3 : greenPen3, 35, 38, 35, 58);
            // 绘制地感8                                                               
            e.Graphics.DrawLine(this.InductorCoil8 ? redPen3 : greenPen3, 45, 38, 45, 58);
        }

        private void superGridControl_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // 取消进入编辑
            e.Cancel = true;
        }

        /// <summary>
        /// 设置行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl_GetRowHeaderText(object sender, DevComponents.DotNetBar.SuperGrid.GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.RowIndex + 1).ToString();
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

        private void btnSaveTransport_SaleFuel1_Click(object sender, EventArgs e)
        {
            if (!SaveSaleFuelTransport1()) MessageBoxEx.Show("保存失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// 重置信息1
        /// </summary>
        void ResetSelFuel1()
        {
            this.timer_SaleFuel1_Cancel = true;

            this.CurrentFlowFlag1 = eFlowFlag.等待车辆;

            this.CurrentAutotruck1 = null;
            this.CurrentSaleFuelTransport1 = null;

            txtTagId_SaleFuel1.ResetText();

            btnSaveTransport_SaleFuel1.Enabled = false;

            BackGateDown1();
            FrontGateDown1();

            UpdateLedShow("  等待车辆");

            // 最后重置
            this.CurrentImperfectCar1 = null;
        }
        /// <summary>
        /// 重置信息2
        /// </summary>
        void ResetSelFuel2()
        {
            this.timer_SaleFuel2_Cancel = true;

            this.CurrentFlowFlag2 = eFlowFlag.等待车辆;

            this.CurrentAutotruck2 = null;
            this.CurrentSaleFuelTransport2 = null;

            txtTagId_SaleFuel2.ResetText();

            btnSaveTransport_SaleFuel2.Enabled = false;

            BackGateDown2();
            FrontGateDown2();

            UpdateLedShow("  等待车辆");

            // 最后重置
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
                    case eFlowFlag.验证信息:
                        #region

                        // 查找该车未完成的运输记录
                        CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck1.Id, eCarType.销售煤.ToString());
                        if (unFinishTransport != null)
                        {
                            this.CurrentSaleFuelTransport1 = commonDAO.SelfDber.Get<CmcsSaleFuelTransport>(unFinishTransport.TransportId);
                            if (this.CurrentSaleFuelTransport1 != null)
                            {
                                // 判断路线设置
                                string nextPlace;
                                if (carTransportDAO.CheckNextTruckInFactoryWay(this.CurrentAutotruck1.CarType, this.CurrentSaleFuelTransport1.StepName, "装载", CommonAppConfig.GetInstance().AppIdentifier, out nextPlace))
                                {
                                    if (CommonAppConfig.GetInstance().AppIdentifier.Contains(this.CurrentSaleFuelTransport1.LoadArea))
                                    {
                                        this.CurrentFlowFlag1 = eFlowFlag.保存信息;
                                    }
                                    else
                                    {
                                        UpdateLed1Show("路线错误", "禁止通过");
                                        this.voiceSpeaker.Speak("路线错误 禁止通过 " + (!string.IsNullOrEmpty(this.CurrentSaleFuelTransport1.LoadArea) ? "请前往" + this.CurrentSaleFuelTransport1.LoadArea : ""), 2, false);

                                        timer_SaleFuel1.Interval = 20000;
                                    }
                                }
                                else
                                {
                                    UpdateLed1Show("路线错误", "禁止通过");
                                    this.voiceSpeaker.Speak("路线错误 禁止通过 " + (!string.IsNullOrEmpty(nextPlace) ? "请前往" + nextPlace : ""), 2, false);

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
                            UpdateLed1Show(this.CurrentAutotruck1.CarNumber, "未排队");
                            this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck1.CarNumber + " 未找到排队记录", 2, false);

                            timer_SaleFuel1.Interval = 20000;
                        }

                        #endregion
                        break;
                    case eFlowFlag.保存信息:
                        // 提高灵敏度
                        timer_SaleFuel1.Interval = 1000;

                        btnSaveTransport_SaleFuel1.Enabled = true;



                        if (this.AutoHandMode)
                        {
                            // 自动模式
                            if (!SaveSaleFuelTransport1())
                            {
                                BackGateUp1();
                                UpdateLed1Show(this.CurrentAutotruck1.CarNumber, "保存失败");
                                this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck1.CarNumber + " 保存失败，请联系管理员", 2, false);
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            // 手动模式 
                        }
                        break;
                    case eFlowFlag.等待进入:
                        #region

                        // 所有地感无信号时重置
                        if (!this.InductorCoil1 && !this.InductorCoil2)
                        {
                            FrontGateUp1();
                            this.CurrentFlowFlag1 = eFlowFlag.等待离开;
                        }
                        // 降低灵敏度
                        timer_SaleFuel1.Interval = 4000;

                        #endregion
                        break;
                    case eFlowFlag.等待离开:
                        #region
                        // 所有地感无信号时重置
                        if (!HasCarOnLeaveWay1())
                        {
                            ResetSelFuel1();
                        }
                        // 降低灵敏度
                        timer_SaleFuel1.Interval = 4000;

                        #endregion
                        break;
                }

                // 当前地磅重量小于最小称重且所有地感、对射无信号时重置
                if (!HasCarOnEnterWay1() && !HasCarOnLeaveWay1() && this.CurrentFlowFlag1 != eFlowFlag.等待车辆
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
                    case eFlowFlag.验证信息:
                        #region

                        // 查找该车未完成的运输记录
                        CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck2.Id, eCarType.销售煤.ToString());
                        if (unFinishTransport != null)
                        {
                            this.CurrentSaleFuelTransport2 = commonDAO.SelfDber.Get<CmcsSaleFuelTransport>(unFinishTransport.TransportId);
                            if (this.CurrentSaleFuelTransport2 != null)
                            {
                                // 判断路线设置
                                string nextPlace;
                                if (carTransportDAO.CheckNextTruckInFactoryWay(this.CurrentAutotruck2.CarType, this.CurrentSaleFuelTransport2.StepName, "装载", CommonAppConfig.GetInstance().AppIdentifier, out nextPlace))
                                {
                                    if (CommonAppConfig.GetInstance().AppIdentifier.Contains(this.CurrentSaleFuelTransport2.LoadArea))
                                    {
                                        this.CurrentFlowFlag2 = eFlowFlag.保存信息;
                                    }
                                    else
                                    {
                                        UpdateLed2Show("路线错误", "禁止通过");
                                        this.voiceSpeaker.Speak("路线错误 禁止通过 " + (!string.IsNullOrEmpty(this.CurrentSaleFuelTransport2.LoadArea) ? "请前往" + this.CurrentSaleFuelTransport2.LoadArea : ""), 2, false);

                                        timer_SaleFuel2.Interval = 20000;
                                    }
                                }
                                else
                                {
                                    UpdateLed2Show("路线错误", "禁止通过");
                                    this.voiceSpeaker.Speak("路线错误 禁止通过 " + (!string.IsNullOrEmpty(nextPlace) ? "请前往" + nextPlace : ""), 2, false);

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
                            UpdateLed2Show(this.CurrentAutotruck2.CarNumber, "未排队");
                            this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck2.CarNumber + " 未找到排队记录", 2, false);

                            timer_SaleFuel2.Interval = 20000;
                        }

                        #endregion
                        break;
                    case eFlowFlag.保存信息:
                        // 提高灵敏度
                        timer_SaleFuel2.Interval = 2000;

                        btnSaveTransport_SaleFuel2.Enabled = true;



                        if (this.AutoHandMode)
                        {
                            // 自动模式
                            if (!SaveSaleFuelTransport2())
                            {
                                BackGateUp2();
                                UpdateLed2Show(this.CurrentAutotruck2.CarNumber, "保存失败");
                                this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck2.CarNumber + " 保存失败，请联系管理员", 2, false);
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            // 手动模式 
                        }
                        break;
                    case eFlowFlag.等待进入:
                        #region

                        // 所有地感无信号时重置
                        if (!this.InductorCoil5 && !this.InductorCoil6)
                        {
                            FrontGateUp2();
                            this.CurrentFlowFlag2 = eFlowFlag.等待离开;
                        }
                        // 降低灵敏度
                        timer_SaleFuel2.Interval = 4000;

                        #endregion
                        break;
                    case eFlowFlag.等待离开:
                        #region

                        // 所有地感无信号时重置
                        if (!HasCarOnLeaveWay2())
                        {
                            ResetSelFuel2();
                        }
                        // 降低灵敏度
                        timer_SaleFuel2.Interval = 4000;

                        #endregion
                        break;
                }

                // 当前地磅重量小于最小称重且所有地感、对射无信号时重置
                if (!HasCarOnEnterWay2() && !HasCarOnLeaveWay2() && this.CurrentFlowFlag2 != eFlowFlag.等待车辆
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

            if (!SaveSaleFuelTransport2()) MessageBoxEx.Show("保存失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 保存运输记录
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
                    this.CurrentFlowFlag1 = eFlowFlag.等待进入;
                    UpdateLed1Show("请进装载", "请稍后");
                    this.voiceSpeaker.Speak("请进装载，装载完成后离开！", 2, false);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("保存失败\r\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log4Neter.Error("保存运输记录", ex);
            }

            return false;
        }


        /// <summary>
        /// 保存运输记录
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
                    this.CurrentFlowFlag2 = eFlowFlag.等待进入;

                    UpdateLed2Show("请进装载", "请稍后");
                    this.voiceSpeaker.Speak("请进装载，装载完成后离开！", 2, false);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("保存失败\r\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log4Neter.Error("保存运输记录", ex);
            }

            return false;
        }


        /// <summary>
        /// 有车辆1在下磅的道路上
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
        /// 有车辆2在下磅的道路上
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
        /// 有车辆1在上磅的道路上
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
        /// 有车辆2在上磅的道路上
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

            FrmUnFinishTransport_Select frm = new FrmUnFinishTransport_Select("where CarType='" + eCarType.销售煤.ToString() + "' order by CreateDate desc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (this.InductorCoil1)
                    passCarQueuer1.Enqueue(ePassWay.Way1, frm.Output.CarNumber, false);

                this.CurrentFlowFlag1 = eFlowFlag.识别车辆;
            }
        }

        private void btnSelectAutotruck_SaleFuel2_Click(object sender, EventArgs e)
        {
            FrmUnFinishTransport_Select frm = new FrmUnFinishTransport_Select("where CarType='" + eCarType.销售煤.ToString() + "' order by CreateDate desc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (this.InductorCoil5)
                    passCarQueuer2.Enqueue(ePassWay.Way2, frm.Output.CarNumber, false);

                this.CurrentFlowFlag2 = eFlowFlag.识别车辆;
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
