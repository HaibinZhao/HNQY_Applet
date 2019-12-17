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
using CMCS.CarTransport.JxSampler.Core;
using CMCS.CarTransport.JxSampler.Enums;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Utilities;
using DevComponents.DotNetBar;
using LED.YB14;
using CMCS.Common.Enums;
using CMCS.CarTransport.JxSampler.Frms.Sys;
using CMCS.Common.Entities.Sys;
using CMCS.Common.Entities.Fuel;
using IOC.JMDMYTWI8DOMR;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.Common.Views;

namespace CMCS.CarTransport.JxSampler.Frms
{
	public partial class FrmJxSampler : DevComponents.DotNetBar.Metro.MetroForm
	{
		/// <summary>
		/// 窗体唯一标识符
		/// </summary>
		public static string UniqueKey = "FrmCarSampler";

		public FrmJxSampler()
		{
			InitializeComponent();
		}

		#region Vars

		CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();
		JxSamplerDAO jxSamplerDAO = JxSamplerDAO.GetInstance();
		CommonDAO commonDAO = CommonDAO.GetInstance();

		IocControler iocControler;
		/// <summary>
		/// 语音播报
		/// </summary>
		VoiceSpeaker voiceSpeaker = new VoiceSpeaker();

		bool autoHandMode = true;
		/// <summary>
		/// 手动模式=true  手动模式=false
		/// </summary>
		public bool AutoHandMode
		{
			get { return autoHandMode; }
			set
			{
				autoHandMode = value;

				btnSendSamplingPlan.Visible = !value;
				btnSelectAutotruck.Visible = !value;
				btnReset.Visible = !value;
				btnStartScan.Visible = !value;
			}
		}

		bool inFactorySampler = false;
		/// <summary>
		/// 是否入口采样机
		/// </summary>
		public bool InFactorySampler
		{
			get { return inFactorySampler; }
			set
			{
				inFactorySampler = value;
				slightLED2.Visible = value;
				labLED2.Visible = value;
			}
		}

		bool startScan = false;
		/// <summary>
		/// 是否开始读卡
		/// </summary>
		public bool StartScan
		{
			get { return startScan; }
			set { startScan = value; }
		}

		bool isCrossInductorCoil2 = false;
		/// <summary>
		/// 是否通过道闸地感
		/// </summary>
		public bool IsCrossInductorCoil2
		{
			get { return isCrossInductorCoil2; }
			set { isCrossInductorCoil2 = value; }
		}

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

		private CmcsBuyFuelTransport currentBuyFuelTransport;
		/// <summary>
		/// 当前运输记录
		/// </summary>
		public CmcsBuyFuelTransport CurrentBuyFuelTransport
		{
			get { return currentBuyFuelTransport; }
			set
			{
				currentBuyFuelTransport = value;

				if (value != null)
				{
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前运输记录Id.ToString(), value.Id);

					//txtSupplierName.Text = value.SupplierName;
					txtSupplierName.Text = "****";
					//txtMineName.Text = value.MineName;
					txtMineName.Text = "****";
					txtTicketWeight.Text = value.TicketWeight.ToString();
                    //txtTransportCompanyName.Text = value.TransportCompanyName;
                    txtTransportCompanyName.Text = "****";
                    //txtFuelKindName.Text = value.FuelKindName;
                    txtFuelKindName.Text = "****";
                    txtSamplingType.Text = value.SamplingType;
				}
				else
				{
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前运输记录Id.ToString(), string.Empty);

					txtSupplierName.ResetText();
					txtMineName.ResetText();
					txtTransportCompanyName.ResetText();
					txtFuelKindName.ResetText();
					txtSamplingType.ResetText();
					txtTicketWeight.ResetText();
				}
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

				if (value != null)
				{
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车Id.ToString(), value.Id);
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车号.ToString(), value.CarNumber);

					if (value.CarType == eCarType.入厂煤.ToString())
						superTabControl2.SelectedTab = superTabItem_BuyFuel;

					txtCarNumber.Text = value.CarNumber;
					panCurrentCarNumber.Text = value.CarNumber;
				}
				else
				{
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车Id.ToString(), string.Empty);
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.当前车号.ToString(), string.Empty);

					txtCarNumber.ResetText();
					panCurrentCarNumber.ResetText();
				}

				PreviewCarCarriage(value);
			}
		}

		private InfQCJXCYSampleCMD currentSampleCMD;
		/// <summary>
		/// 当前采样命令
		/// </summary>
		public InfQCJXCYSampleCMD CurrentSampleCMD
		{
			get { return currentSampleCMD; }
			set { currentSampleCMD = value; }
		}

		private eEquInfSamplerSystemStatus samplerSystemStatus;
		/// <summary>
		/// 采样机系统状态
		/// </summary>
		public eEquInfSamplerSystemStatus SamplerSystemStatus
		{
			get { return samplerSystemStatus; }
			set
			{
				samplerSystemStatus = value;

				if (value == eEquInfSamplerSystemStatus.就绪待机)
					slightSamplerStatus.LightColor = EquipmentStatusColors.BeReady;
				else if (value == eEquInfSamplerSystemStatus.正在运行 || value == eEquInfSamplerSystemStatus.正在卸样)
					slightSamplerStatus.LightColor = EquipmentStatusColors.Working;
				else if (value == eEquInfSamplerSystemStatus.发生故障)
					slightSamplerStatus.LightColor = EquipmentStatusColors.Breakdown;
			}
		}

		/// <summary>
		/// 采样机设备编码
		/// </summary>
		public string SamplerMachineCode;
		/// <summary>
		/// 采样机设备名称
		/// </summary>
		public string SamplerMachineName;

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

			// 采样机设备编码
			this.SamplerMachineCode = commonDAO.GetAppletConfigString("采样机设备编码");
			this.SamplerMachineName = commonDAO.GetMachineNameByCode(this.SamplerMachineCode);

			// 默认自动
			sbtnChangeAutoHandMode.Value = true;

			// 重置程序远程控制命令
			commonDAO.ResetAppRemoteControlCmd(CommonAppConfig.GetInstance().AppIdentifier);
		}

		private void FrmCarSampler_Load(object sender, EventArgs e)
		{

		}

		private void FrmCarSampler_Shown(object sender, EventArgs e)
		{
			InitHardware();

			InitForm();
		}

		private void FrmCarSampler_FormClosing(object sender, FormClosingEventArgs e)
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
			  });
		}

		/// <summary>
		/// 前方升杆
		/// </summary>
		void FrontGateUp()
		{
			this.iocControler.Gate2Up();
			this.iocControler.GreenLight2();
		}

		/// <summary>
		/// 前方降杆
		/// </summary>
		void FrontGateDown()
		{
			if (!this.InductorCoil2) this.iocControler.Gate2Down();
			this.iocControler.RedLight2();
		}

		/// <summary>
		/// 后方升杆
		/// </summary>
		void BackGateUp()
		{
			this.iocControler.Gate1Up();
			this.iocControler.GreenLight1();
		}

		/// <summary>
		/// 后方降杆
		/// </summary>
		void BackGateDown()
		{
			if (!this.InductorCoil1) this.iocControler.Gate1Down();
			this.iocControler.RedLight1();
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

				 commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.读卡器1_连接状态.ToString(), status ? "1" : "0");
			 });
		}

		#endregion

		#region LED控制卡

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

				slightLED1.LightColor = (_LED1ConnectStatus ? Color.Green : Color.Red);

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
		/// 更新LED动态区域
		/// </summary>
		/// <param name="value1">第一行内容</param>
		/// <param name="value2">第二行内容</param>
		private void UpdateLedShow(string value1 = "", string value2 = "")
		{
			FrmDebugConsole.GetInstance().Output("更新LED1:|" + value1 + "|" + value2 + "|");

			if (!this.LED1ConnectStatus) return;
			if (this.LED1PrevLedFileContent == value1 + value2) return;

			string ledContent = GenerateFillLedContent12(value1) + GenerateFillLedContent12(value2);

			File.WriteAllText(this.LED1TempFile, ledContent, Encoding.UTF8);

			if (LED1m_bSendBusy == false)
			{
				LED1m_bSendBusy = true;

				int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID);
				if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("更新LED动态区域", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

				LED1m_bSendBusy = false;
			}

			this.LED1PrevLedFileContent = value1 + value2;
		}

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

		#endregion

		#region LED控制卡2

		/// <summary>
		/// LED2控制卡屏号
		/// </summary>
		int LED2nScreenNo = 2;
		/// <summary>
		/// LED2动态区域号
		/// </summary>
		int LED2DYArea_ID = 2;
		/// <summary>
		/// LED2更新标识
		/// </summary>
		bool LED2m_bSendBusy = false;

		private bool _LED2ConnectStatus;
		/// <summary>
		/// LED1连接状态
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

				slightLED2.LightColor = (_LED2ConnectStatus ? Color.Green : Color.Red);

				commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.LED屏2_连接状态.ToString(), value ? "1" : "0");
			}
		}

		/// <summary>
		/// LED2显示内容文本
		/// </summary>
		string LED2TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Led1TempFile.txt");

		/// <summary>
		/// LED2上一次显示内容
		/// </summary>
		string LED2PrevLedFileContent = string.Empty;

		/// <summary>
		/// 更新LED动态区域
		/// </summary>
		/// <param name="value1">第一行内容</param>
		/// <param name="value2">第二行内容</param>
		private void UpdateLedShow2(string value1 = "", string value2 = "")
		{
			FrmDebugConsole.GetInstance().Output("更新LED1:|" + value1 + "|" + value2 + "|");

			if (!this.LED2ConnectStatus) return;
			if (this.LED2PrevLedFileContent == value1 + value2) return;

			string ledContent = GenerateFillLedContent12(value1) + GenerateFillLedContent12(value2);

			File.WriteAllText(this.LED2TempFile, ledContent, Encoding.UTF8);

			if (LED2m_bSendBusy == false)
			{
				LED2m_bSendBusy = true;

				int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED2nScreenNo, this.LED2DYArea_ID);
				if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("更新LED动态区域", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

				LED2m_bSendBusy = false;
			}

			this.LED2PrevLedFileContent = value1 + value2;
		}

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

				this.InFactorySampler = commonDAO.GetAppletConfigString("入口采样机") == "1";

				// IO控制器（网口）
				if (!string.IsNullOrEmpty(commonDAO.GetAppletConfigString("IO控制器_IP地址")) && commonDAO.TestPing(commonDAO.GetAppletConfigString("IO控制器_IP地址")))
				{
					Hardwarer.Iocer.OnReceived += new JMDMYTWI8DOMRIocer.ReceivedEventHandler(Iocer_Received);
					Hardwarer.Iocer.OnStatusChange += new JMDMYTWI8DOMRIocer.StatusChangeHandler(Iocer_StatusChange);
					Hardwarer.Iocer.OpenUDP(commonDAO.GetAppletConfigString("IO控制器_IP地址"), commonDAO.GetAppletConfigInt32("IO控制器_端口"));
					if (!Hardwarer.Iocer.Status) MessageBoxEx.Show("IO控制器连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				this.iocControler = new IocControler(Hardwarer.Iocer);

				// 读卡器1（网口）
				if (!string.IsNullOrEmpty(commonDAO.GetAppletConfigString("读卡器1_IP地址")) && commonDAO.TestPing(commonDAO.GetAppletConfigString("读卡器1_IP地址")))
				{
					Hardwarer.Rwer1.StartWith = commonDAO.GetAppletConfigString("读卡器_标签过滤");
					Hardwarer.Rwer1.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer1_OnStatusChange);
					Hardwarer.Rwer1.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer1_OnScanError);
					success = Hardwarer.Rwer1.OpenCom(commonDAO.GetAppletConfigString("读卡器1_IP地址"), commonDAO.GetAppletConfigInt32("读卡器1_端口"), Convert.ToByte(commonDAO.GetAppletConfigInt32("读卡器1_功率")));
					if (!success) MessageBoxEx.Show("读卡器1连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}

				#region LED控制卡1

				string led1SocketIP = commonDAO.GetAppletConfigString("LED显示屏1_IP地址");

				if (!string.IsNullOrEmpty(led1SocketIP))
				{
					if (YB14DynamicAreaLeder.PingReplyTest(led1SocketIP))
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
									nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID);
									if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
									{
										// 初始化成功
										this.LED1ConnectStatus = true;
										UpdateLedShow("  等待车辆");
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
					else
					{
						this.LED1ConnectStatus = false;
						Log4Neter.Error("初始化LED1控制卡", new Exception("网络连接失败"));
						MessageBoxEx.Show("LED1控制卡连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}

				#endregion

				#region LED控制卡2
				if (this.InFactorySampler)
				{
					string led2SocketIP = commonDAO.GetAppletConfigString("LED显示屏2_IP地址");

					if (!string.IsNullOrEmpty(led2SocketIP))
					{
						if (YB14DynamicAreaLeder.PingReplyTest(led2SocketIP))
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
										nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED2nScreenNo, this.LED2DYArea_ID);
										if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
										{
											// 初始化成功
											this.LED2ConnectStatus = true;
											UpdateLedShow("  等待车辆");
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
						else
						{
							this.LED2ConnectStatus = false;
							Log4Neter.Error("初始化LED2控制卡", new Exception("网络连接失败"));
							MessageBoxEx.Show("LED2控制卡连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}
				}
				#endregion

				try
				{
					#region 大华视频

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
				YB14DynamicAreaLeder.SendDeleteDynamicAreasCommand(this.LED1nScreenNo, 1, "");
				YB14DynamicAreaLeder.DeleteScreen(this.LED1nScreenNo);
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
			if (this.iocControler != null && !this.InductorCoil1) this.iocControler.Gate1Down();
		}

		/// <summary>
		/// 道闸2升杆
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGate2Up_Click(object sender, EventArgs e)
		{
			if (this.iocControler != null) FrontGateUp();
		}

		/// <summary>
		/// 道闸2降杆
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGate2Down_Click(object sender, EventArgs e)
		{
			if (this.iocControler != null && !this.InductorCoil2) FrontGateDown();
		}

		#endregion

		private void btnStartScan_Click(object sender, EventArgs e)
		{
			this.StartScan = true;
		}

		#region 公共业务

		/// <summary>
		/// 读卡、车号识别任务
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Stop();
			timer1.Interval = 1000;

			try
			{
				// 执行远程命令
				ExecAppRemoteControlCmd();

				switch (this.CurrentFlowFlag)
				{
					case eFlowFlag.等待车辆:
						#region
						// 当读卡区域地感有信号，触发读卡或者车号识别
						if (this.InductorCoil1)
						{
							StartScan = true;
						}
						if (StartScan)
						{
							this.panCurrentCarNumber.Text = "开始读卡";
							List<string> tags = Hardwarer.Rwer1.ScanTags();
							if (tags.Count > 0) passCarQueuer.Enqueue(tags[0]);
						}
						if (passCarQueuer.Count > 0) this.CurrentFlowFlag = eFlowFlag.验证车辆;
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

						// 方式一：根据识别的车牌号查找车辆信息
						//this.CurrentAutotruck = carTransportDAO.GetAutotruckByCarNumber(this.CurrentImperfectCar.Voucher);
						//UpdateLedShow(this.CurrentImperfectCar.Voucher);
						//// 方式二：根据识别的标签卡查找车辆信息
						this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);
						this.txtTagId.Text = this.CurrentImperfectCar.Voucher;
						if (this.CurrentAutotruck != null)
						{
							if (this.CurrentAutotruck.CarType == "入厂煤")
							{
								if (this.CurrentAutotruck.IsUse == 1)
								{
									if (this.CurrentAutotruck.CarriageLength > 0 && this.CurrentAutotruck.CarriageWidth > 0 && this.CurrentAutotruck.CarriageBottomToFloor > 0)
									{
										// 未完成运输记录
										CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, eCarType.入厂煤.ToString());
										if (unFinishTransport != null)
										{
											this.CurrentBuyFuelTransport = carTransportDAO.GetBuyFuelTransportById(unFinishTransport.TransportId);
											if (this.CurrentBuyFuelTransport != null)
											{
												if (this.CurrentBuyFuelTransport.SamplingType == "机械采样")
												{
													if (string.IsNullOrEmpty(this.CurrentBuyFuelTransport.SamplePlace))
													{
														// 判断路线设置
														string nextPlace;
														if (carTransportDAO.CheckNextTruckInFactoryWay(this.CurrentAutotruck.CarType, this.CurrentBuyFuelTransport.StepName, "采样", CommonAppConfig.GetInstance().AppIdentifier, out nextPlace))
														{
															btnSendSamplingPlan.Enabled = true;

															this.CurrentFlowFlag = eFlowFlag.等待驶入;

															//UpdateLedShow(this.CurrentAutotruck.CarNumber, "熄火下车");
															//this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 熄火下车", 2, false);
														}
														else
														{
															UpdateLedShow("路线错误", "禁止通过");
															this.voiceSpeaker.Speak("路线错误 禁止通过 " + (!string.IsNullOrEmpty(nextPlace) ? "请前往" + nextPlace : ""), 2, false);

															timer1.Interval = 20000;
														}
													}
													else
													{
														UpdateLedShow(this.CurrentAutotruck.CarNumber, "请通行");
														this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 已采样 请通行", 2, false);
														FrontGateUp();
														this.CurrentFlowFlag = eFlowFlag.等待离开;
													}
												}
												else
												{
													UpdateLedShow(this.CurrentAutotruck.CarNumber, "无需采样");
													this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 无需采样 请通行", 2, false);
													FrontGateUp();
													this.CurrentFlowFlag = eFlowFlag.等待离开;
												}
											}
											else
											{
												commonDAO.SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
											}
										}
										else
										{
											this.UpdateLedShow(this.CurrentAutotruck.CarNumber, "未排队");
											this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 未找到排队记录", 2, false);
											timer1.Interval = 20000;
										}
									}
									else
									{
										this.UpdateLedShow(this.CurrentAutotruck.CarNumber, "车厢未测量");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 车厢未测量", 2, false);

										timer1.Interval = 20000;
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
								UpdateLedShow(this.CurrentAutotruck.CarNumber, "无需采样");
								this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 无需采样 请通行", 2, false);
								FrontGateUp();
								this.CurrentFlowFlag = eFlowFlag.等待离开;
							}
						}
						else
						{
							UpdateLedShow("车辆未登记");

							// 方式一：车号识别
							this.voiceSpeaker.Speak("车辆未登记，禁止通过", 2, false);
							//// 方式二：刷卡方式
							//this.voiceSpeaker.Speak("卡号未登记，禁止通过", 2, false);

							timer1.Interval = 20000;
						}

						#endregion
						break;

					case eFlowFlag.等待驶入:
						#region
						if (this.CurrentBuyFuelTransport.FPSamplePlace == "混采")
						{
							#region 混采
							// 当采样区域地感有信号，入口道闸地感无信号
							if ((this.InFactorySampler && !this.InductorCoil1))
							{
								if (this.SamplerMachineCode == "#3汽车机械采样机")
								{
									string st = commonDAO.GetSignalDataValue("#1汽车机械采样机", eSignalDataName.系统.ToString());
									string dg1 = commonDAO.GetSignalDataValue("汽车智能化-#1机械采样机端", "地感1信号");
									string dg2 = commonDAO.GetSignalDataValue("汽车智能化-#1机械采样机端", "地感2信号");
									if (st == "就绪待机" && dg1 == "0" && dg2 == "0")
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "请通过");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 请前往前方采样机采样", 2, false);
										FrontGateUp();
										this.CurrentFlowFlag = eFlowFlag.等待离开;
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "熄火下车");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 熄火下车", 2, false);
										BackGateDown();
										this.CurrentFlowFlag = eFlowFlag.发送计划;
									}

								}
								else if (this.SamplerMachineCode == "#4汽车机械采样机")
								{
									string st = commonDAO.GetSignalDataValue("#2汽车机械采样机", eSignalDataName.系统.ToString());
									string dg1 = commonDAO.GetSignalDataValue("汽车智能化-#2机械采样机端", "地感1信号");
									string dg2 = commonDAO.GetSignalDataValue("汽车智能化-#2机械采样机端", "地感2信号");
									if (st == "就绪待机" && dg1 == "0" && dg2 == "0")
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "请通过");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 请前往前方采样机采样", 2, false);
										FrontGateUp();
										this.CurrentFlowFlag = eFlowFlag.等待离开;
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "熄火下车");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 熄火下车", 2, false);
										BackGateDown();
										this.CurrentFlowFlag = eFlowFlag.发送计划;
									}

								}
								//BackGateDown();
								//this.CurrentFlowFlag = eFlowFlag.发送计划;
							}
							else if (!this.InFactorySampler && this.InductorCoil1)
							{
								if (this.CurrentBuyFuelTransport.StepName != "入厂")
								{
									UpdateLedShow(this.CurrentAutotruck.CarNumber, "请通过");
									this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 已采样,请通过", 2, false);
									FrontGateUp();
									this.CurrentFlowFlag = eFlowFlag.等待离开;
								}
								else
								{
									UpdateLedShow(this.CurrentAutotruck.CarNumber, "熄火下车");
									this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 熄火下车", 2, false);
									this.CurrentFlowFlag = eFlowFlag.发送计划;
								}
							}
							#endregion
						}
						else
						{
							#region 定向采
							if ((this.InFactorySampler && !this.InductorCoil1))
							{

								if (this.SamplerMachineCode == this.CurrentBuyFuelTransport.FPSamplePlace)
								{

									UpdateLedShow(this.CurrentAutotruck.CarNumber, "熄火下车");
									this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 熄火下车", 2, false);
									BackGateDown();
									this.CurrentFlowFlag = eFlowFlag.发送计划;
								}
								else
								{

									if (IsTrueOrFalse(this.SamplerMachineCode, this.CurrentBuyFuelTransport.FPSamplePlace))
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "请通过");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 请前往前方采样机采样", 2, false);
										FrontGateUp();
										this.CurrentFlowFlag = eFlowFlag.等待离开;
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "走错通道");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 请前往" + this.CurrentBuyFuelTransport.FPSamplePlace + "采样机进行采样", 2, false);
									}
								}

							}
							else if (!this.InFactorySampler && this.InductorCoil1)
							{
								if (this.CurrentBuyFuelTransport.StepName != "入厂")
								{
									UpdateLedShow(this.CurrentAutotruck.CarNumber, "请通过");
									this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 已采样,请通过", 2, false);
									FrontGateUp();
									this.CurrentFlowFlag = eFlowFlag.等待离开;
								}
								else
								{
									if (this.SamplerMachineCode == this.CurrentBuyFuelTransport.FPSamplePlace)
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "熄火下车");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 熄火下车", 2, false);
										this.CurrentFlowFlag = eFlowFlag.发送计划;
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "走错采样机");
										this.voiceSpeaker.Speak("车牌号 " + this.CurrentAutotruck.CarNumber + " 请前往" + this.CurrentBuyFuelTransport.FPSamplePlace + "采样机进行采样", 2, false);
									}
								}
							}
							#endregion
						}

						// 降低灵敏度
						timer1.Interval = 4000;

						#endregion
						break;

					case eFlowFlag.发送计划:
						#region

						// 采样区域地感,检测车辆是否到指定区域
						if (!this.AutoHandMode || ((this.InFactorySampler && !this.InductorCoil1) || (!this.InFactorySampler && this.InductorCoil1)))
						{
							if (this.SamplerSystemStatus == eEquInfSamplerSystemStatus.就绪待机)
							{
								CmcsRCSampling sampling = carTransportDAO.GetRCSamplingById(this.CurrentBuyFuelTransport.SamplingId);
								if (sampling != null)
								{
									//if (sampling.SamplingType == "机械采样")
									//{
									int carCount = 0;
									int PointCount = 3;
									CmcsSamplingScheme SamplingScheme = jxSamplerDAO.SelfDber.Entity<CmcsSamplingScheme>("where  INFACTORYBATCHID=:INFACTORYBATCHID", new { INFACTORYBATCHID = sampling.InFactoryBatchId });
									if (SamplingScheme != null)
									{
										Fultbtransfer Fultbtransfer = jxSamplerDAO.SelfDber.Entity<Fultbtransfer>("where ID=:TRANSFERID", new { TRANSFERID = SamplingScheme.TRANSFERID });
										if (Fultbtransfer != null)
										{
											carCount = int.Parse(Fultbtransfer.TRANSFERNUMBER.ToString());
											PointCount = SamplingScheme.PointCount;
										}
									}
									this.CurrentSampleCMD = new InfQCJXCYSampleCMD()
									{
										MachineCode = this.SamplerMachineCode,
										CarNumber = this.CurrentBuyFuelTransport.CarNumber,
										InFactoryBatchId = this.CurrentBuyFuelTransport.InFactoryBatchId,
										SampleCode = sampling.SampleCode,
										Mt = 0,
										// 根据预报
										TicketWeight = this.CurrentBuyFuelTransport.TicketWeight,
										//TicketWeight = 0,
										// 根据预报
										CarCount = carCount,
										// 采样点数根据相关逻辑计算
										PointCount = PointCount,
										CarriageLength = this.CurrentAutotruck.CarriageLength,
										CarriageLength2 = this.CurrentAutotruck.CarriageLength2,
										CarriageWidth = this.CurrentAutotruck.CarriageWidth,
										CarriageBottomToFloor = this.CurrentAutotruck.CarriageBottomToFloor,
										CarriageBottomToFloor2 = this.CurrentAutotruck.CarriageBottomToFloor2,
										Obstacle1 = this.CurrentAutotruck.LeftObstacle1 > 0 ? this.CurrentAutotruck.LeftObstacle1 + "|" + this.CurrentAutotruck.RightObstacle1 : "",
										Obstacle2 = this.CurrentAutotruck.LeftObstacle1 > 0 ? this.CurrentAutotruck.LeftObstacle2 + "|" + this.CurrentAutotruck.RightObstacle2 : "",
										Obstacle3 = this.CurrentAutotruck.LeftObstacle1 > 0 ? this.CurrentAutotruck.LeftObstacle3 + "|" + this.CurrentAutotruck.RightObstacle3 : "",
										Obstacle4 = this.CurrentAutotruck.LeftObstacle1 > 0 ? this.CurrentAutotruck.LeftObstacle4 + "|" + this.CurrentAutotruck.RightObstacle4 : "",
										Obstacle5 = this.CurrentAutotruck.LeftObstacle1 > 0 ? this.CurrentAutotruck.LeftObstacle5 + "|" + this.CurrentAutotruck.RightObstacle5 : "",
										Obstacle6 = this.CurrentAutotruck.LeftObstacle1 > 0 ? this.CurrentAutotruck.LeftObstacle6 + "|" + this.CurrentAutotruck.RightObstacle6 : "",
										ResultCode = eEquInfCmdResultCode.默认.ToString(),
										DataFlag = 0
									};

									// 发送采样计划
									if (commonDAO.SelfDber.Insert<InfQCJXCYSampleCMD>(CurrentSampleCMD) > 0)
										this.CurrentFlowFlag = eFlowFlag.等待采样;
									//}
									//else {
									//    this.UpdateLedShow("请联系管理员!");
									//}
								}
								else
								{
									this.UpdateLedShow("未找到采样单信息");
									this.voiceSpeaker.Speak("未找到采样单信息，请联系管理员", 2, false);

									timer1.Interval = 5000;
								}
							}
							else
							{
								this.UpdateLedShow("采样机未就绪");
								this.voiceSpeaker.Speak("采样机未就绪", 1, false);

								timer1.Interval = 5000;
							}
						}

						#endregion
						break;

					case eFlowFlag.等待采样:
						#region
						// 判断采样是否完成
						//InfQCJXCYSampleCMD qCJXCYSampleCMD = commonDAO.SelfDber.Get<InfQCJXCYSampleCMD>(this.CurrentSampleCMD.Id);
						CmcsRCSampling samp = carTransportDAO.GetRCSamplingById(this.CurrentBuyFuelTransport.SamplingId);
						InfQCJXCYSampleCMD qCJXCYSampleCMD = commonDAO.SelfDber.Entity<InfQCJXCYSampleCMD>("where CarNumber=:CarNumber and SampleCode=:SampleCode and IsRead=0 and ResultCode='成功'", new { CarNumber = this.CurrentBuyFuelTransport.CarNumber, SampleCode = samp.SampleCode });
						//if (qCJXCYSampleCMD.ResultCode == eEquInfCmdResultCode.成功.ToString())
						if (qCJXCYSampleCMD != null)
						{
							jxSamplerDAO.IsRead(qCJXCYSampleCMD);
							//commonDAO.SetSignalDataValue("测试1", this.CurrentSampleCMD.Id, this.CurrentBuyFuelTransport.Id);
							if (jxSamplerDAO.SaveBuyFuelTransport(this.CurrentBuyFuelTransport.Id, DateTime.Now, CommonAppConfig.GetInstance().AppIdentifier, qCJXCYSampleCMD.BARREL, qCJXCYSampleCMD.SAMPLEWEIGHT, qCJXCYSampleCMD.Id + ".jpg"))
							{
								//commonDAO.SetSignalDataValue("测试2", this.CurrentSampleCMD.Id, this.CurrentBuyFuelTransport.Id);
								FrontGateUp();

								this.UpdateLedShow("  采样完成", "  请离开");
								this.voiceSpeaker.Speak("采样完成，请离开采样区域", 2, false);

								this.CurrentFlowFlag = eFlowFlag.等待离开;
							}
						}
						// 降低灵敏度
						timer1.Interval = 4000;

						#endregion
						break;

					case eFlowFlag.等待离开:
						if (!this.InFactorySampler && this.InductorCoil1 == false && this.InductorCoil2 == false)
						{

							ResetBuyFuel();
						}
						if ((this.InFactorySampler && this.InductorCoil2))
							IsCrossInductorCoil2 = true;
						if ((this.InFactorySampler && !this.InductorCoil2 && IsCrossInductorCoil2 == true))
						{
							ResetBuyFuel();
						}

						// 降低灵敏度
						timer1.Interval = 4000;

						break;
				}

				// 所有地感无信号时重置
				//if (this.AutoHandMode && !this.InductorCoil1 && !this.InductorCoil2 && this.CurrentFlowFlag != eFlowFlag.等待车辆
				//    && this.CurrentImperfectCar != null) ResetBuyFuel();

				RefreshEquStatus();
			}
			catch (Exception ex)
			{
				Log4Neter.Error("timer1_Tick", ex);
			}
			finally
			{
				timer1.Start();
			}

			timer1.Start();
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			timer2.Stop();
			// 三分钟执行一次
			timer2.Interval = 60000;

			try
			{
				// 入厂煤
				LoadTodayUnFinishBuyFuelTransport();
				LoadTodayFinishBuyFuelTransport();

				UpdateWaitSampleCarNumber();
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
		/// 切换手动/自动模式
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sbtnChangeAutoHandMode_ValueChanged(object sender, EventArgs e)
		{
			this.AutoHandMode = sbtnChangeAutoHandMode.Value;
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

		/// <summary>
		/// 发送采样计划
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSendSamplingPlan_Click(object sender, EventArgs e)
		{
			if (SendSamplingPlan()) MessageBoxEx.Show("发送失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// 保存入厂煤运输记录
		/// </summary>
		/// <returns></returns>
		bool SendSamplingPlan()
		{
			if (this.CurrentBuyFuelTransport == null)
			{
				MessageBoxEx.Show("请选择车辆", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			this.CurrentFlowFlag = eFlowFlag.发送计划;

			return false;
		}

		/// <summary>
		/// 重置入厂煤运输记录
		/// </summary>
		void ResetBuyFuel()
		{
			this.StartScan = false;
			this.IsCrossInductorCoil2 = false;
			this.CurrentFlowFlag = eFlowFlag.等待车辆;

			this.CurrentAutotruck = null;
			this.CurrentBuyFuelTransport = null;

			txtTagId.ResetText();

			btnSendSamplingPlan.Enabled = false;

			FrontGateDown();
			if (this.InFactorySampler)
				BackGateUp();
			UpdateLedShow("  等待车辆");

			// 最后重置
			this.CurrentImperfectCar = null;
		}

		/// <summary>
		/// 重置
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnReset_Click(object sender, EventArgs e)
		{
			ResetBuyFuel();
		}

		/// <summary>
		/// 获取未完成的入厂煤记录
		/// </summary>
		void LoadTodayUnFinishBuyFuelTransport()
		{
			superGridControl1.PrimaryGrid.DataSource = jxSamplerDAO.GetUnFinishBuyFuelTransport();
		}

		/// <summary>
		/// 获取指定日期已完成的入厂煤记录
		/// </summary>
		void LoadTodayFinishBuyFuelTransport()
		{
			superGridControl2.PrimaryGrid.DataSource = jxSamplerDAO.GetFinishedBuyFuelTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
		}

		/// <summary>
		/// 更新入口采样机LED提示
		/// </summary>
		void UpdateWaitSampleCarNumber()
		{
			if (this.InFactorySampler)
			{
				if (this.SamplerMachineCode == GlobalVars.MachineCode_QCJXCYJ_3)
				{
					CmcsBuyFuelTransport transport1 = commonDAO.SelfDber.Entity<CmcsBuyFuelTransport>("where SamplePlace is null and FPSamplePlace=:FPSamplePlace and trunc(CreateDate)=trunc(sysdate) and GrossWeight=0 order by CreateDate desc", new { FPSamplePlace = GlobalVars.MachineCode_QCJXCYJ_1 });
					CmcsBuyFuelTransport transport2 = commonDAO.SelfDber.Entity<CmcsBuyFuelTransport>("where SamplePlace is null and FPSamplePlace=:FPSamplePlace and trunc(CreateDate)=trunc(sysdate) and GrossWeight=0 order by CreateDate desc", new { FPSamplePlace = GlobalVars.MachineCode_QCJXCYJ_3 });

					UpdateLedShow2(transport1 != null ? transport1.CarNumber : "", transport2 != null ? transport2.CarNumber : "");
				}
				else if (this.SamplerMachineCode == GlobalVars.MachineCode_QCJXCYJ_4)
				{
					CmcsBuyFuelTransport transport1 = commonDAO.SelfDber.Entity<CmcsBuyFuelTransport>("where SamplePlace is null and FPSamplePlace=:FPSamplePlace and trunc(CreateDate)=trunc(sysdate) and GrossWeight=0 order by CreateDate desc", new { FPSamplePlace = GlobalVars.MachineCode_QCJXCYJ_2 });
					CmcsBuyFuelTransport transport2 = commonDAO.SelfDber.Entity<CmcsBuyFuelTransport>("where SamplePlace is null and FPSamplePlace=:FPSamplePlace and trunc(CreateDate)=trunc(sysdate) and GrossWeight=0 order by CreateDate desc", new { FPSamplePlace = GlobalVars.MachineCode_QCJXCYJ_4 });

					UpdateLedShow2(transport1 != null ? transport1.CarNumber : "", transport2 != null ? transport2.CarNumber : "");
				}
			}
		}
		#endregion

		#region 基础信息

		/// <summary>
		/// 选择车辆
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSelectAutotruck_Click(object sender, EventArgs e)
		{
			FrmUnFinishTransport_Select frm = new FrmUnFinishTransport_Select("where CarType='" + eCarType.入厂煤.ToString() + "' order by CreateDate desc");
			if (frm.ShowDialog() == DialogResult.OK)
			{
				passCarQueuer.Enqueue(frm.Output.TagId);
				this.CurrentFlowFlag = eFlowFlag.验证车辆;
			}
		}

		#endregion

		#region 其他

		Pen redPen3 = new Pen(Color.Red, 3);
		Pen greenPen3 = new Pen(Color.Lime, 3);

		/// <summary>
		/// 当前车号面板绘制
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panCurrentCarNumber_Paint(object sender, PaintEventArgs e)
		{
			PanelEx panel = sender as PanelEx;

			int height = 12;
			if (this.InFactorySampler)
			{
				// 绘制地感1
				e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, 1, 15, height);
				e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, panel.Height - height, 15, panel.Height - 1);
			}
			else
			{
				// 绘制地感1
				e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, panel.Width - 55, 1, panel.Width - 55, height);
				e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, panel.Width - 55, panel.Height - height, panel.Width - 55, panel.Height - 1);
			}
			// 绘制地感2
			e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, panel.Width - 15, 1, panel.Width - 15, height);
			e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, panel.Width - 15, panel.Height - height, panel.Width - 15, panel.Height - 1);
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

		private void superGridControl_DataBindingComplete(object sender, DevComponents.DotNetBar.SuperGrid.GridDataBindingCompleteEventArgs e)
		{
			foreach (GridRow gridRow in e.GridPanel.Rows)
			{
				View_BuyFuelTransport entity = gridRow.DataItem as View_BuyFuelTransport;
				if (entity == null) return;
				gridRow.Cells["clmSupplierName"].Value = "****";
				gridRow.Cells["clmMineName"].Value = "****";
                gridRow.Cells["clmTransportCompanyName"].Value = "****";
                gridRow.Cells["clmFuelKindName"].Value = "****";
            }
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

		/// <summary>
		/// 更新采样机状态
		/// </summary>
		private void RefreshEquStatus()
		{
			string systemStatus = commonDAO.GetSignalDataValue(this.SamplerMachineCode, eSignalDataName.系统.ToString());
			eEquInfSamplerSystemStatus result;
			if (Enum.TryParse(systemStatus, out result)) SamplerSystemStatus = result;
		}

		/// <summary>
		/// 预览车辆拉筋信息图
		/// </summary>
		/// <param name="autotruck"></param>
		private void PreviewCarCarriage(CmcsAutotruck autotruck)
		{
			pboxMeiChe.BackgroundImageLayout = ImageLayout.Stretch;

			if (autotruck != null && autotruck.CarriageLength > 0 && autotruck.CarriageWidth > 0)
			{
				Bitmap res = new Bitmap(CMCS.CarTransport.JxSampler.Properties.Resources.Autotruck);
				PreviewCarBmp carBmp = new PreviewCarBmp(autotruck);
				pboxMeiChe.BackgroundImage = carBmp.GetPreviewBitmap(res, res.Width, res.Height);
			}
			else
			{
				pboxMeiChe.BackgroundImage = CMCS.CarTransport.JxSampler.Properties.Resources.Autotruck;
			}
		}


		/// <summary>
		/// 判断当前车辆所分配的采样机是否是当前入口采样机读卡的道路
		/// </summary>
		/// <param name="cyj1"></param>
		/// <param name="cyj2"></param>
		/// <returns></returns>
		private bool IsTrueOrFalse(string cyj1, string cyj2)
		{
			string[] s1 = new[] { "#1汽车机械采样机", "#3汽车机械采样机" };
			string[] s2 = new[] { "#2汽车机械采样机", "#4汽车机械采样机" };
			if ((s1.Contains(cyj1) && s1.Contains(cyj2)) || (s2.Contains(cyj1) && s2.Contains(cyj2)))
			{
				return true;
			}
			return false;
		}
		#endregion

	}
}
