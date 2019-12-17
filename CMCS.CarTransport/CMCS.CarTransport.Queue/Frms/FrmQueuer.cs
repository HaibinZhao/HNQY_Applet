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
        /// 窗体唯一标识符
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.地感1信号.ToString(), value ? "1" : "0");
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.地感2信号.ToString(), value ? "1" : "0");
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.地感3信号.ToString(), value ? "1" : "0");
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.地感4信号.ToString(), value ? "1" : "0");
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

        bool rwer2OpenStatus = false;
        /// <summary>
        /// 发卡器连接状态
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
        /// 发卡器读卡状态
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
        /// 识别或选择的车辆凭证
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
                    panCurrentCarNumber.Text = "等待车辆";
            }
        }

        eFlowFlag currentFlowFlag = eFlowFlag.等待车辆;
        /// <summary>
        /// 当前业务流程标识
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
        /// 当前车
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

                panCurrentCarNumber.Text = "等待车辆";

                if (value != null)
                {
                    commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.当前车Id.ToString(), value.Id);
                    commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.当前车号.ToString(), value.CarNumber);


                    CmcsEPCCard ePCCard = Dbers.GetInstance().SelfDber.Get<CmcsEPCCard>(value.EPCCardId);
                    if (value.CarType == eCarType.入厂煤.ToString())
                    {
                        if (ePCCard != null) txtTagId_BuyFuel.Text = ePCCard.TagId;

                        txtCarNumber_BuyFuel.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_BuyFuel;

                        CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, this.CurrentAutotruck.CarType);

                        if (unFinishTransport != null)
                        {
                            ////根据读取到的标签卡获取排队过的车辆信息，现场可能先填完所有信息然后点击排队，在把车辆开进来
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
                                btnSaveTransport_BuyFuel.Text = "已排队";
                            }
                        }
                        else
                        {
                            btnSaveTransport_BuyFuel.Enabled = true;
                            btnSaveTransport_BuyFuel.Text = "排 队";
                        }
                    }
                    else if (value.CarType == eCarType.其他物资.ToString())
                    {
                        if (ePCCard != null) txtTagId_Goods.Text = ePCCard.TagId;

                        txtCarNumber_Goods.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_Goods;


                        CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, this.CurrentAutotruck.CarType);

                        if (unFinishTransport != null)
                        {
                            ////根据读取到的标签卡获取排队过的车辆信息，现场可能先填完所有信息然后点击排队，在把车辆开进来
                            CmcsGoodsTransport CmcsGoodsTransport = commonDAO.SelfDber.Get<CmcsGoodsTransport>(unFinishTransport.TransportId);

                            if (CmcsGoodsTransport != null)
                            {
                                txtSupplyUnitName_Goods.Text = CmcsGoodsTransport.SupplyUnitName;
                                txtReceiveUnitName_Goods.Text = CmcsGoodsTransport.ReceiveUnitName;
                                txtGoodsTypeName_Goods.Text = CmcsGoodsTransport.GoodsTypeName;

                                btnSaveTransport_Goods.Enabled = false;
                                btnSaveTransport_Goods.Text = "已排队";
                            }
                        }
                        else
                        {
                            btnSaveTransport_Goods.Enabled = true;
                            btnSaveTransport_Goods.Text = "排 队";
                        }
                    }
                    else if (value.CarType == eCarType.来访车辆.ToString())
                    {
                        if (ePCCard != null) txtTagId_Visit.Text = ePCCard.TagId;

                        txtCarNumber_Visit.Text = value.CarNumber;
                        superTabControl2.SelectedTab = superTabItem_Visit;
                    }

                    panCurrentCarNumber.Text = value.CarNumber;
                }
                else
                {
                    commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.当前车Id.ToString(), string.Empty);
                    commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.当前车号.ToString(), string.Empty);

                    txtCarNumber_BuyFuel.Text = string.Empty;
                    txtCarNumber_Goods.Text = string.Empty;
                    txtCarNumber_Visit.Text = string.Empty;

                    txtTagId_BuyFuel.Text = string.Empty;
                    txtTagId_Goods.Text = string.Empty;
                    txtTagId_Visit.Text = string.Empty;

                    panCurrentCarNumber.Text = "等待车辆";


                    this.txtSupplierName_BuyFuel.Text = string.Empty; ;
                    txtSupplierName_BuyFuel.Text = string.Empty; ;
                    txtTransportCompanyName_BuyFuel.Text = string.Empty; ;
                    txtMineName_BuyFuel.Text = string.Empty; ;
                    txtTicketWeight_BuyFuel.Text = "0.00";

                    btnSaveTransport_BuyFuel.Enabled = true;
                    btnSaveTransport_BuyFuel.Text = "排 队";
                }
            }
        }

        /// <summary>
        /// 是否有查看供应商及煤种权限
        /// </summary>
        public bool HasShowSupplier = false;

        private CmcsLMYB lMYB;
        /// <summary>
        /// 预报编号
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
        /// 窗体初始化
        /// </summary>
        private void InitForm()
        {
#if DEBUG
            lblFlowFlag.Visible = true;
            FrmDebugConsole.GetInstance().Show();
#else
            lblFlowFlag.Visible = false;
#endif

            // 重置程序远程控制命令
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
            // 卸载设备
            UnloadHardware();
        }

        #region 设备相关

        #region IO控制器

        void Iocer_StatusChange(bool status)
        {
            // 接收IO控制器状态 
            InvokeEx(() =>
            {
                slightIOC.LightColor = (status ? Color.Green : Color.Red);

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.IO控制器_连接状态.ToString(), status ? "1" : "0");
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
            });
        }

        /// <summary>
        /// 允许通行
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
        /// 阻断前行
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

        #region 读卡器

        void Rwer1_OnScanError(Exception ex)
        {
            Log4Neter.Error("读卡器1", ex);
        }

        void Rwer1_OnStatusChange(bool status)
        {
            // 接收读卡器1状态 
            InvokeEx(() =>
             {
                 slightRwer1.LightColor = (status ? Color.Green : Color.Red);

                 commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.读卡器1_连接状态.ToString(), status ? "1" : "0");
             });
        }

        void Rwer2_OnScanError(Exception ex)
        {
            Log4Neter.Error("读卡器2", ex);
        }

        void Rwer2_OnStatusChange(bool status)
        {
            // 接收读卡器2状态 
            InvokeEx(() =>
              {
                  slightRwer2.LightColor = (status ? Color.Green : Color.Red);

                  commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.读卡器2_连接状态.ToString(), status ? "1" : "0");
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
                         this.CurrentFlowFlag = eFlowFlag.验证车辆;
                         timer1_Tick(null, null);
                     }
                 }
             });
        }

        /// <summary>
        /// 读卡按钮
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
            if (this.CurrentImperfectCar == null) return;

            if (this.CurrentImperfectCar.PassWay == ePassWay.Way1)
                UpdateLed1Show(value1, value2);
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

                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.LED屏1_连接状态.ToString(), value ? "1" : "0");
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
#if DEBUG
            FrmDebugConsole.GetInstance().Output("更新LED1:|" + value1 + "|" + value2 + "|");
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
                //if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("更新LED动态区域", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

                LED1m_bSendBusy = false;
            }

            this.LED1PrevLedFileContent = value1 + value2;
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

                // IO控制器
                //Hardwarer.Iocer.OnReceived += new IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer.ReceivedEventHandler(Iocer_Received);
                //Hardwarer.Iocer.OnStatusChange += new IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer.StatusChangeHandler(Iocer_StatusChange);
                //Hardwarer.Iocer.OpenUDP(commonDAO.GetAppletConfigString("IO控制器_IP地址"), commonDAO.GetAppletConfigInt32("IO控制器_端口"));
                //if (!Hardwarer.Iocer.Status) MessageBoxEx.Show("IO控制器连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.iocControler = new IocControler(Hardwarer.Iocer);

                // 读卡器1
                #region 四号门读卡器坏的,代码判断临时注释掉
                //Hardwarer.Rwer1.StartWith = commonDAO.GetAppletConfigString("读卡器_标签过滤");
                //Hardwarer.Rwer1.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer1_OnStatusChange);
                //Hardwarer.Rwer1.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer1_OnScanError);
                //success = Hardwarer.Rwer1.OpenCom(commonDAO.GetAppletConfigString("读卡器1IP地址"), commonDAO.GetAppletConfigInt32("读卡器1端口"), Convert.ToByte(commonDAO.GetAppletConfigInt32("读卡器1功率")));
                //if (!success) MessageBoxEx.Show("读卡器1连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                #endregion
                // 发卡器2
                Hardwarer.Rwer2.OnStatusChange += new RW.LZR12.Lzr12Rwer_Com.StatusChangeHandler(Rwer2_OnStatusChange);
                Hardwarer.Rwer2.OnScanError += new RW.LZR12.Lzr12Rwer_Com.ScanErrorEventHandler(Rwer2_OnScanError);
                Hardwarer.Rwer2.OnScanSuccess += new RW.LZR12.Lzr12Rwer_Com.ScanSuccessEventHandler(Rwer2_OnScanSuccess);
                success = Hardwarer.Rwer2.OpenCom(commonDAO.GetAppletConfigInt32("发卡器_串口"));
                Rwer2OpenStatus = success;
                if (!success) MessageBoxEx.Show("发卡器连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.LED屏1_连接状态.ToString(), this.LED1ConnectStatus ? "1" : "0");

                #endregion

                #region 大华视频
                try
                {
                    string strIP = commonDAO.GetAppletConfigString("公共配置", "视频服务器IP地址");
                    string strPort = commonDAO.GetAppletConfigString("公共配置", "视频服务器端口号");
                    string strID1 = commonDAO.GetAppletConfigString("1号视频通道ID");
                    string strID2 = commonDAO.GetAppletConfigString("2号视频通道ID");

                    IntPtr nPDLLHandle = (IntPtr)0;
                    IntPtr result1 = DHSDK.DPSDK_Create(DHSDK.dpsdk_sdk_type_e.DPSDK_CORE_SDK_SERVER, ref nPDLLHandle);//初始化数据交互接口
                    IntPtr result2 = DHSDK.DPSDK_InitExt();//初始化解码播放接口
                    if (result1 == (IntPtr)0 && result2 == (IntPtr)0)
                    {

                        if (DHSDK.Logion(strIP, int.Parse(strPort), "system", "admin123", nPDLLHandle))
                        {
                            #region 1号视频
                            IntPtr realseq = default(IntPtr);
                            string szCameraId1 = strID1;
                            if (DHSDK.StartPreview(panelEx3.Handle, szCameraId1, nPDLLHandle, realseq))
                            {
                                panelEx3.Refresh();
                            }
                            #endregion
                            #region 2号视频
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
                        MessageBox.Show("初始化失败");
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

                switch (this.CurrentFlowFlag)
                {
                    case eFlowFlag.等待车辆:
                        #region

                        // PassWay.Way1

                        if (!this.Rwer2Reading)
                        {
                            List<string> tags = Hardwarer.Rwer1.ScanTags();
                            if (tags.Count > 0) passCarQueuer.Enqueue(ePassWay.Way1, tags[0], true);

                            if (passCarQueuer.Count > 0)
                            {
                                this.CurrentAutotruck = null;

                                this.CurrentFlowFlag = eFlowFlag.验证车辆;
                            }
                        }

                        #endregion
                        break;

                    case eFlowFlag.验证车辆:
                        #region

                        // 队列中无车时，等待车辆
                        if (passCarQueuer.Count == 0)
                        {
                            this.CurrentFlowFlag = eFlowFlag.等待车辆;
                            break;
                        }

                        this.CurrentImperfectCar = passCarQueuer.Dequeue();

                        //// 方式一：根据识别的车牌号查找车辆信息
                        this.CurrentAutotruck = carTransportDAO.GetAutotruckByCarNumber(this.CurrentImperfectCar.Voucher);

                        if (this.CurrentAutotruck == null)
                            //方式二：根据识别的标签卡查找车辆信息
                            this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);

                        if (this.CurrentAutotruck != null)
                        {
                            if (this.CurrentAutotruck.CarType == eCarType.入厂煤.ToString() && (this.CurrentAutotruck.CarriageLength <= 0 || this.CurrentAutotruck.CarriageWidth <= 0 || this.CurrentAutotruck.CarriageBottomToFloor <= 0))
                            {
                                MessageBoxEx.Show("车厢信息未测量，禁止登记", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ResetBuyFuel();
                                return;
                            }
                            UpdateLedShow(this.CurrentAutotruck.CarNumber);
                            if (this.CurrentAutotruck.IsUse == 1)
                            {
                                // 判断是否存在未完结的运输记录，可能存在先点完了排队，然后车辆在进厂，用户确认暂屏蔽掉了。
                                bool hasUnFinish = false;
                                CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, this.CurrentAutotruck.CarType);
                                if (unFinishTransport != null)
                                {
                                    timer1.Interval = 10000;
                                    hasUnFinish = true;
                                    this.CurrentFlowFlag = eFlowFlag.等待车辆;
                                }

                                if (!hasUnFinish)
                                {
                                    if (this.CurrentAutotruck.CarType == eCarType.入厂煤.ToString())
                                    {
                                        this.timer_BuyFuel_Cancel = false;
                                        this.CurrentFlowFlag = eFlowFlag.数据录入;
                                    }
                                    else if (this.CurrentAutotruck.CarType == eCarType.其他物资.ToString())
                                    {
                                        this.timer_Goods_Cancel = false;
                                        this.CurrentFlowFlag = eFlowFlag.数据录入;
                                    }
                                    else if (this.CurrentAutotruck.CarType == eCarType.来访车辆.ToString())
                                    {
                                        this.timer_Visit_Cancel = false;
                                        this.CurrentFlowFlag = eFlowFlag.数据录入;
                                    }
                                }
                            }
                            else
                            {
                                UpdateLedShow(this.CurrentAutotruck.CarNumber, "已停用");
                                this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 已停用，禁止通过", 2, false);

                                timer1.Interval = 20000;
                            }
                        }
                        else
                        {
                            // 方式一：车号识别
                            this.voiceSpeaker.Speak("卡号未登记，禁止通过", 2, false);
                            UpdateLedShow(this.CurrentImperfectCar.Voucher, "未登记");
                            //// 方式二：刷卡方式
                            this.voiceSpeaker.Speak("卡号未登记，禁止通过", 2, false);

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
                // 入厂煤
                LoadTodayUnFinishBuyFuelTransport();
                LoadTodayFinishBuyFuelTransport();

                // 其他物资
                LoadTodayUnFinishGoodsTransport();
                LoadTodayFinishGoodsTransport();

                // 来访车辆
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
        /// 有车辆在当前道路上
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
        /// 加载煤种
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
        /// 加载采样方式
        /// </summary>
        void LoadSampleType(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "Content";
            comboBoxEx.ValueMember = "Code";
            comboBoxEx.DataSource = commonDAO.GetCodeContentByKind("采样方式");

            //comboBoxEx.Text = eSamplingType.机械采样.ToString();
        }

        /// <summary>
        /// 加载采样机
        /// </summary>
        void LoadSample(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "Content";
            comboBoxEx.ValueMember = "Code";
            comboBoxEx.DataSource = commonDAO.GetCodeContentByKind("汽车机械采样机");

            //comboBoxEx.Text = eSamplingType.机械采样.ToString();
        }

        /// <summary>
        /// 执行远程命令
        /// </summary>
        void ExecAppRemoteControlCmd()
        {
            // 获取最新的命令
            CmcsAppRemoteControlCmd appRemoteControlCmd = commonDAO.GetNewestAppRemoteControlCmd(commonAppConfig.AppIdentifier);
            if (appRemoteControlCmd != null)
            {
                if (appRemoteControlCmd.CmdCode == "控制道闸")
                {
                    Log4Neter.Info("接收远程命令：" + appRemoteControlCmd.CmdCode + "，参数：" + appRemoteControlCmd.Param);

                    if (appRemoteControlCmd.Param.Equals("Gate1Up", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate1Up();
                    else if (appRemoteControlCmd.Param.Equals("Gate1Down", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate1Down();
                    else if (appRemoteControlCmd.Param.Equals("Gate2Up", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate2Up();
                    else if (appRemoteControlCmd.Param.Equals("Gate2Down", StringComparison.CurrentCultureIgnoreCase))
                        this.iocControler.Gate2Down();

                    // 更新执行结果
                    commonDAO.SetAppRemoteControlCmdResultCode(appRemoteControlCmd, eEquInfCmdResultCode.成功);
                }
            }
        }

        #endregion

        #region 入厂煤业务

        bool timer_BuyFuel_Cancel = true;

        private CmcsSupplier selectedSupplier_BuyFuel;
        /// <summary>
        /// 选择的供煤单位
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
        /// 选择的运输单位
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
        /// 选择的矿点
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
        /// 选择的煤种
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
        /// 选择车辆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAutotruck_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmAutotruck_Select frm = new FrmAutotruck_Select("and CarType='" + eCarType.入厂煤.ToString() + "' and IsUse=1 order by CarNumber asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                passCarQueuer.Enqueue(ePassWay.UnKnow, frm.Output.CarNumber, false);
                this.CurrentFlowFlag = eFlowFlag.验证车辆;
            }
        }

        /// <summary>
        /// 选择供煤单位
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
        /// 选择矿点
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
        /// 选择运输单位
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
        /// 新车登记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewAutotruck_BuyFuel_Click(object sender, EventArgs e)
        {
            new FrmAutotruck_Oper("", true).Show();
        }

        /// <summary>
        /// 扫描二维码
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
                        MessageBoxEx.Show("该车辆的预报日期不是今天!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //MessageBoxEx.Show("亏吨异常", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                Log4Neter.Error("扫描二维码", ex);
            }
        }

        /// <summary>
        /// 选择入厂煤来煤预报
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
        /// 保存入厂煤运输记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTransport_BuyFuel_Click(object sender, EventArgs e)
        {
            SaveBuyFuelTransport();
        }

        /// <summary>
        /// 保存运输记录
        /// </summary>
        /// <returns></returns>
        bool SaveBuyFuelTransport()
        {
            if (this.CurrentAutotruck == null)
            {
                MessageBoxEx.Show("请选择车辆", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedFuelKind_BuyFuel == null)
            {
                MessageBoxEx.Show("请选择煤种", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedMine_BuyFuel == null)
            {
                MessageBoxEx.Show("请选择矿点", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedSupplier_BuyFuel == null)
            {
                MessageBoxEx.Show("请选择供煤单位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedTransportCompany_BuyFuel == null)
            {
                MessageBoxEx.Show("请选择运输单位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (txtTicketWeight_BuyFuel.Value <= 0)
            {
                MessageBoxEx.Show("请输入有效的矿发量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (isRulst(this.CurrentAutotruck))
            {
                MessageBoxEx.Show("该车辆排放不符合国标", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // 生成入厂煤排队记录，同时生成批次信息以及采制化三级编码
                if (queuerDAO.JoinQueueBuyFuelTransport(this.CurrentAutotruck, this.SelectedSupplier_BuyFuel, this.SelectedMine_BuyFuel, this.SelectedTransportCompany_BuyFuel, this.SelectedFuelKind_BuyFuel, (decimal)txtTicketWeight_BuyFuel.Value, DateTime.Now, txtRemark_BuyFuel.Text, commonAppConfig.AppIdentifier, cmbSamplingType_BuyFuel.Text, cmbSampling_BuyFuel.Text, false, false, this.LMYB))
                {
                    btnSaveTransport_BuyFuel.Enabled = false;

                    UpdateLedShow("排队成功", "请离开");
                    MessageBoxEx.Show("排队成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.CurrentFlowFlag = eFlowFlag.等待离开;

                    LoadTodayUnFinishBuyFuelTransport();
                    LoadTodayFinishBuyFuelTransport();

                    //LetPass();

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
        /// 根据车辆判断该车辆排放是否符合标准(国V以上符合)
        /// </summary>
        /// <param name="autotruck"></param>
        /// <returns></returns>
        private bool isRulst(CmcsAutotruck autotruck)
        {
            bool rulst = false;
            string state = commonDAO.GetAppletConfigString("车辆排放标准验证");
            string strList = commonDAO.GetAppletConfigString("不合格车辆排放标准");
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
        /// 重置入厂煤运输记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_BuyFuel_Click(object sender, EventArgs e)
        {
            ResetBuyFuel();
        }

        /// <summary>
        /// 重置信息
        /// </summary>
        void ResetBuyFuel()
        {
            this.timer_BuyFuel_Cancel = true;

            this.CurrentFlowFlag = eFlowFlag.等待车辆;

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

            UpdateLedShow("  等待车辆");

            // 最后重置
            this.CurrentImperfectCar = null;
        }

        /// <summary>
        /// 入厂煤运输记录业务定时器
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
                    case eFlowFlag.数据录入:
                        #region

                        #endregion
                        break;

                    case eFlowFlag.等待离开:
                        #region

                        // 当前道路地感无信号时重置
                        // if (!HasCarOnCurrentWay()) 

                        ResetBuyFuel();

                        // 降低灵敏度
                        timer_BuyFuel.Interval = 4000;

                        #endregion
                        break;
                }

                // 当前道路地感无信号时重置
                //  if (!HasCarOnCurrentWay() && this.CurrentFlowFlag != eFlowFlag.等待车辆 && (this.CurrentImperfectCar != null && this.CurrentImperfectCar.IsFromDevice)) ResetBuyFuel();
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
        /// 获取未完成的入厂煤记录
        /// </summary>
        void LoadTodayUnFinishBuyFuelTransport()
        {
            superGridControl1_BuyFuel.PrimaryGrid.DataSource = queuerDAO.GetUnFinishBuyFuelTransport();
        }

        /// <summary>
        /// 获取指定日期已完成的入厂煤记录
        /// </summary>
        void LoadTodayFinishBuyFuelTransport()
        {
            superGridControl2_BuyFuel.PrimaryGrid.DataSource = queuerDAO.GetFinishedBuyFuelTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        }

        /// <summary>
        /// 提取预报信息
        /// </summary>
        /// <param name="lMYB">来煤预报</param>
        void BorrowForecast_BuyFuel(CmcsLMYB lMYB)
        {
            if (lMYB == null) return;

            this.SelectedFuelKind_BuyFuel = commonDAO.SelfDber.Get<CmcsFuelKind>(lMYB.FuelKindId);
            this.SelectedMine_BuyFuel = commonDAO.SelfDber.Get<CmcsMine>(lMYB.MineId);
            this.SelectedSupplier_BuyFuel = commonDAO.SelfDber.Get<CmcsSupplier>(lMYB.SupplierId);
            this.SelectedTransportCompany_BuyFuel = commonDAO.SelfDber.Get<CmcsTransportCompany>(lMYB.TransportCompanyId);
        }

        /// <summary>
        /// 双击行时，自动填充供煤单位、矿点等信息
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

            // 更改有效状态
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeBuyFuelTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl1_BuyFuel_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                View_BuyFuelTransport entity = gridRow.DataItem as View_BuyFuelTransport;
                if (entity == null) return;

                // 填充有效状态
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

            // 更改有效状态
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeBuyFuelTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl2_BuyFuel_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                View_BuyFuelTransport entity = gridRow.DataItem as View_BuyFuelTransport;
                if (entity == null) return;

                // 填充有效状态
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

        #region 其他物资业务

        bool timer_Goods_Cancel = true;

        private CmcsSupplyReceive selectedSupplyUnit_Goods;
        /// <summary>
        /// 选择的供货单位
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
        /// 选择的收货单位
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
        /// 选择的物资类型
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
        /// 选择车辆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAutotruck_Goods_Click(object sender, EventArgs e)
        {
            FrmAutotruck_Select frm = new FrmAutotruck_Select("and CarType='" + eCarType.其他物资.ToString() + "' and IsUse=1 order by CarNumber asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                passCarQueuer.Enqueue(ePassWay.UnKnow, frm.Output.CarNumber, false);
                this.CurrentFlowFlag = eFlowFlag.验证车辆;
            }
        }

        /// <summary>
        /// 选择供货单位
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
        /// 选择收货单位
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
        /// 选择物资类型
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
        /// 新车登记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewAutotruck_Goods_Click(object sender, EventArgs e)
        {
            // eCarType.其他物资 

            new FrmAutotruck_Oper().Show();
        }

        /// <summary>
        /// 保存排队记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTransport_Goods_Click(object sender, EventArgs e)
        {
            SaveGoodsTransport();
        }

        /// <summary>
        /// 保存运输记录
        /// </summary>
        /// <returns></returns>
        bool SaveGoodsTransport()
        {
            if (this.CurrentAutotruck == null)
            {
                MessageBoxEx.Show("请选择车辆", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedSupplyUnit_Goods == null)
            {
                MessageBoxEx.Show("请选择供货单位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedReceiveUnit_Goods == null)
            {
                MessageBoxEx.Show("请选择收货单位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.SelectedGoodsType_Goods == null)
            {
                MessageBoxEx.Show("请选择物资类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // 生成排队记录
                if (queuerDAO.JoinQueueGoodsTransport(this.CurrentAutotruck, this.SelectedSupplyUnit_Goods, this.SelectedReceiveUnit_Goods, this.SelectedGoodsType_Goods, DateTime.Now, txtRemark_Goods.Text, commonAppConfig.AppIdentifier))
                {

                    MessageBoxEx.Show("排队成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSaveTransport_Goods.Enabled = false;
                    this.CurrentFlowFlag = eFlowFlag.等待离开;

                    UpdateLedShow("排队成功", "请离开");


                    LoadTodayUnFinishGoodsTransport();
                    LoadTodayFinishGoodsTransport();
                    //LetPass();
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
        /// 重置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Goods_Click(object sender, EventArgs e)
        {
            ResetGoods();
        }

        /// <summary>
        /// 重置信息
        /// </summary>
        void ResetGoods()
        {
            this.timer_Goods_Cancel = true;

            this.CurrentFlowFlag = eFlowFlag.等待车辆;

            this.CurrentAutotruck = null;
            this.SelectedSupplyUnit_Goods = null;
            this.SelectedReceiveUnit_Goods = null;
            this.SelectedGoodsType_Goods = null;

            txtTagId_Goods.Text = string.Empty;
            txtRemark_Goods.Text = string.Empty;

            btnSaveTransport_Goods.Enabled = true;

            LetBlocking();
            UpdateLedShow("  等待车辆");

            // 最后重置
            this.CurrentImperfectCar = null;
        }

        /// <summary>
        /// 其他物资运输记录业务定时器
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
                    case eFlowFlag.数据录入:
                        #region



                        #endregion
                        break;

                    case eFlowFlag.等待离开:
                        #region

                        // 当前道路地感无信号时重置
                        //if (!HasCarOnCurrentWay())
                        ResetGoods();

                        #endregion
                        break;
                }

                // 当前道路地感无信号时重置
                //if (!HasCarOnCurrentWay() && this.CurrentFlowFlag != eFlowFlag.等待车辆 && (this.CurrentImperfectCar != null && this.CurrentImperfectCar.IsFromDevice)) ResetGoods();
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
        /// 获取未完成的其他物资记录
        /// </summary>
        void LoadTodayUnFinishGoodsTransport()
        {
            superGridControl1_Goods.PrimaryGrid.DataSource = queuerDAO.GetUnFinishGoodsTransport();
        }

        /// <summary>
        /// 获取指定日期已完成的其他物资记录
        /// </summary>
        void LoadTodayFinishGoodsTransport()
        {
            superGridControl2_Goods.PrimaryGrid.DataSource = queuerDAO.GetFinishedGoodsTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        }

        /// <summary>
        /// 双击行时，自动填充录入信息
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

            // 更改有效状态
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeGoodsTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl1_Goods_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsGoodsTransport entity = gridRow.DataItem as CmcsGoodsTransport;
                if (entity == null) return;

                // 填充有效状态
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
            }
        }

        private void superGridControl2_Goods_CellClick(object sender, GridCellClickEventArgs e)
        {
            CmcsGoodsTransport entity = e.GridCell.GridRow.DataItem as CmcsGoodsTransport;
            if (entity == null) return;

            // 更改有效状态
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeGoodsTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl2_Goods_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsGoodsTransport entity = gridRow.DataItem as CmcsGoodsTransport;
                if (entity == null) return;

                // 填充有效状态
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
            }
        }

        #endregion

        #region 来访车辆业务

        bool timer_Visit_Cancel = true;

        /// <summary>
        /// 选择车辆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAutotruck_Visit_Click(object sender, EventArgs e)
        {
            FrmAutotruck_Select frm = new FrmAutotruck_Select("and CarType='" + eCarType.来访车辆.ToString() + "' and IsUse=1 order by CarNumber asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                passCarQueuer.Enqueue(ePassWay.UnKnow, frm.Output.CarNumber, false);
                this.CurrentFlowFlag = eFlowFlag.验证车辆;
            }
        }

        /// <summary>
        /// 新车登记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewAutotruck_Visit_Click(object sender, EventArgs e)
        {
            // eCarType.其他物资

            new FrmAutotruck_Oper().Show();
        }

        /// <summary>
        /// 保存排队记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTransport_Visit_Click(object sender, EventArgs e)
        {
            SaveVisitTransport();
        }

        /// <summary>
        /// 保存运输记录
        /// </summary>
        /// <returns></returns>
        bool SaveVisitTransport()
        {
            if (this.CurrentAutotruck == null)
            {
                MessageBoxEx.Show("请选择车辆", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // 生成来访车辆排队记录
                if (queuerDAO.JoinQueueVisitTransport(this.CurrentAutotruck, DateTime.Now, txtRemark_Visit.Text, commonAppConfig.AppIdentifier))
                {
                    LetPass();

                    btnSaveTransport_Visit.Enabled = false;
                    this.CurrentFlowFlag = eFlowFlag.等待离开;

                    UpdateLedShow("排队成功", "请离开");
                    MessageBoxEx.Show("排队成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadTodayUnFinishVisitTransport();
                    LoadTodayFinishVisitTransport();

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
        /// 重置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Visit_Click(object sender, EventArgs e)
        {
            ResetVisit();
        }

        /// <summary>
        /// 重置信息
        /// </summary>
        void ResetVisit()
        {
            this.timer_Visit_Cancel = true;

            this.CurrentFlowFlag = eFlowFlag.等待车辆;

            this.CurrentAutotruck = null;

            txtTagId_Visit.ResetText();
            txtRemark_Visit.ResetText();

            btnSaveTransport_Visit.Enabled = true;

            LetBlocking();
            UpdateLedShow("  等待车辆");

            // 最后重置
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
                    case eFlowFlag.数据录入:
                        #region



                        #endregion
                        break;

                    case eFlowFlag.等待离开:
                        #region

                        // 当前道路地感无信号时重置
                        //if (!HasCarOnCurrentWay())
                        ResetVisit();

                        #endregion
                        break;
                }

                // 当前道路地感无信号时重置
                //if (!HasCarOnCurrentWay() && this.CurrentFlowFlag != eFlowFlag.等待车辆 && (this.CurrentImperfectCar != null && this.CurrentImperfectCar.IsFromDevice)) ResetVisit();
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
        /// 获取未完成的来访车辆记录
        /// </summary>
        void LoadTodayUnFinishVisitTransport()
        {
            superGridControl1_Visit.PrimaryGrid.DataSource = queuerDAO.GetUnFinishVisitTransport();
        }

        /// <summary>
        /// 获取指定日期已完成的来访车辆记录
        /// </summary>
        void LoadTodayFinishVisitTransport()
        {
            superGridControl2_Visit.PrimaryGrid.DataSource = queuerDAO.GetFinishedVisitTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        }

        private void superGridControl1_Visit_CellClick(object sender, GridCellClickEventArgs e)
        {
            CmcsVisitTransport entity = e.GridCell.GridRow.DataItem as CmcsVisitTransport;
            if (entity == null) return;

            // 更改有效状态
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeVisitTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl1_Visit_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsVisitTransport entity = gridRow.DataItem as CmcsVisitTransport;
                if (entity == null) return;

                // 填充有效状态
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
            }
        }

        private void superGridControl2_Visit_CellClick(object sender, GridCellClickEventArgs e)
        {
            CmcsVisitTransport entity = e.GridCell.GridRow.DataItem as CmcsVisitTransport;
            if (entity == null) return;

            // 更改有效状态
            if (e.GridCell.GridColumn.Name == "ChangeIsUse") queuerDAO.ChangeVisitTransportToInvalid(entity.Id, Convert.ToBoolean(e.GridCell.Value));
        }

        private void superGridControl2_Visit_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsVisitTransport entity = gridRow.DataItem as CmcsVisitTransport;
                if (entity == null) return;

                // 填充有效状态
                gridRow.Cells["ChangeIsUse"].Value = Convert.ToBoolean(entity.IsUse);
            }
        }

        #endregion

        #region 其他函数

        Pen redPen3 = new Pen(Color.Red, 3);
        Pen greenPen3 = new Pen(Color.Lime, 3);
        Pen greenPen1 = new Pen(Color.Lime, 1);

        /// <summary>
        /// 当前车号面板绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panCurrentCarNumber_Paint(object sender, PaintEventArgs e)
        {
            PanelEx panel = sender as PanelEx;

            // 绘制地感1
            e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, 10, 15, 30);
            // 绘制地感2                                                               
            e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, 25, 10, 25, 30);
            // 绘制分割线
            e.Graphics.DrawLine(greenPen1, 5, 34, 35, 34);
            // 绘制地感3
            e.Graphics.DrawLine(this.InductorCoil3 ? redPen3 : greenPen3, 15, 38, 15, 58);
            // 绘制地感4                                                               
            e.Graphics.DrawLine(this.InductorCoil4 ? redPen3 : greenPen3, 25, 38, 25, 58);
        }

        private void superGridControl_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            if (e.GridCell.GridColumn.DataPropertyName != "IsUse")
            {
                // 取消进入编辑
                e.Cancel = true;
            }
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

        #region 加、解密字符串(无特殊符号)
        static string encryptKey = "Oyea";    //定义密钥
        /// <summary>
        /// 加密字符串
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string EncryptNew(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            try
            {
                String result = String.Empty;
                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   

                byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

                byte[] data = Encoding.Unicode.GetBytes(str);//定义字节数组，用来存储要加密的字符串  

                MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

                //使用内存流实例化加密流对象   
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

                CStream.Write(data, 0, data.Length);  //向加密流中写入数据      

                CStream.FlushFinalBlock();              //释放加密流    
                byte[] data1 = MStream.ToArray();
                for (int i = 0; i < data1.Length; i++)
                {
                    result += String.Format("{0:X}", Convert.ToInt32(data1[i])).PadLeft(2, '0');
                }
                return result;
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("加密失败" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return str;
            }
        }

        /// <summary>  
        /// 解密字符串   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string DecryptNew(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            try
            {
                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    

                byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

                char[] des = str.ToCharArray();
                byte[] data = new byte[des.Length / 2];
                for (int i = 0; i < des.Length; i++)
                {
                    String code = des[i].ToString() + des[i + 1].ToString();
                    data[i / 2] = Convert.ToByte(code, 16);
                    i++;
                }

                MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

                //使用内存流实例化解密流对象       
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

                CStream.Write(data, 0, data.Length);      //向解密流中写入数据     

                CStream.FlushFinalBlock();               //释放解密流      

                return Encoding.Unicode.GetString(MStream.ToArray());       //返回解密后的字符串  
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("解密失败" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return str;
            }
        }
        #endregion
    }
}
