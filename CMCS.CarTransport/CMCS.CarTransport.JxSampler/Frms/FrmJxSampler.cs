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
		/// ����Ψһ��ʶ��
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
		/// ��������
		/// </summary>
		VoiceSpeaker voiceSpeaker = new VoiceSpeaker();

		bool autoHandMode = true;
		/// <summary>
		/// �ֶ�ģʽ=true  �ֶ�ģʽ=false
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
		/// �Ƿ���ڲ�����
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
		/// �Ƿ�ʼ����
		/// </summary>
		public bool StartScan
		{
			get { return startScan; }
			set { startScan = value; }
		}

		bool isCrossInductorCoil2 = false;
		/// <summary>
		/// �Ƿ�ͨ����բ�ظ�
		/// </summary>
		public bool IsCrossInductorCoil2
		{
			get { return isCrossInductorCoil2; }
			set { isCrossInductorCoil2 = value; }
		}

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

		private CmcsBuyFuelTransport currentBuyFuelTransport;
		/// <summary>
		/// ��ǰ�����¼
		/// </summary>
		public CmcsBuyFuelTransport CurrentBuyFuelTransport
		{
			get { return currentBuyFuelTransport; }
			set
			{
				currentBuyFuelTransport = value;

				if (value != null)
				{
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), value.Id);

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
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), string.Empty);

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
		/// ��ǰ��
		/// </summary>
		public CmcsAutotruck CurrentAutotruck
		{
			get { return currentAutotruck; }
			set
			{
				currentAutotruck = value;

				if (value != null)
				{
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ��Id.ToString(), value.Id);
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ����.ToString(), value.CarNumber);

					if (value.CarType == eCarType.�볧ú.ToString())
						superTabControl2.SelectedTab = superTabItem_BuyFuel;

					txtCarNumber.Text = value.CarNumber;
					panCurrentCarNumber.Text = value.CarNumber;
				}
				else
				{
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ��Id.ToString(), string.Empty);
					commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.��ǰ����.ToString(), string.Empty);

					txtCarNumber.ResetText();
					panCurrentCarNumber.ResetText();
				}

				PreviewCarCarriage(value);
			}
		}

		private InfQCJXCYSampleCMD currentSampleCMD;
		/// <summary>
		/// ��ǰ��������
		/// </summary>
		public InfQCJXCYSampleCMD CurrentSampleCMD
		{
			get { return currentSampleCMD; }
			set { currentSampleCMD = value; }
		}

		private eEquInfSamplerSystemStatus samplerSystemStatus;
		/// <summary>
		/// ������ϵͳ״̬
		/// </summary>
		public eEquInfSamplerSystemStatus SamplerSystemStatus
		{
			get { return samplerSystemStatus; }
			set
			{
				samplerSystemStatus = value;

				if (value == eEquInfSamplerSystemStatus.��������)
					slightSamplerStatus.LightColor = EquipmentStatusColors.BeReady;
				else if (value == eEquInfSamplerSystemStatus.�������� || value == eEquInfSamplerSystemStatus.����ж��)
					slightSamplerStatus.LightColor = EquipmentStatusColors.Working;
				else if (value == eEquInfSamplerSystemStatus.��������)
					slightSamplerStatus.LightColor = EquipmentStatusColors.Breakdown;
			}
		}

		/// <summary>
		/// �������豸����
		/// </summary>
		public string SamplerMachineCode;
		/// <summary>
		/// �������豸����
		/// </summary>
		public string SamplerMachineName;

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

			// �������豸����
			this.SamplerMachineCode = commonDAO.GetAppletConfigString("�������豸����");
			this.SamplerMachineName = commonDAO.GetMachineNameByCode(this.SamplerMachineCode);

			// Ĭ���Զ�
			sbtnChangeAutoHandMode.Value = true;

			// ���ó���Զ�̿�������
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
			  });
		}

		/// <summary>
		/// ǰ������
		/// </summary>
		void FrontGateUp()
		{
			this.iocControler.Gate2Up();
			this.iocControler.GreenLight2();
		}

		/// <summary>
		/// ǰ������
		/// </summary>
		void FrontGateDown()
		{
			if (!this.InductorCoil2) this.iocControler.Gate2Down();
			this.iocControler.RedLight2();
		}

		/// <summary>
		/// ������
		/// </summary>
		void BackGateUp()
		{
			this.iocControler.Gate1Up();
			this.iocControler.GreenLight1();
		}

		/// <summary>
		/// �󷽽���
		/// </summary>
		void BackGateDown()
		{
			if (!this.InductorCoil1) this.iocControler.Gate1Down();
			this.iocControler.RedLight1();
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

				 commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.������1_����״̬.ToString(), status ? "1" : "0");
			 });
		}

		#endregion

		#region LED���ƿ�

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

				slightLED1.LightColor = (_LED1ConnectStatus ? Color.Green : Color.Red);

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
		/// ����LED��̬����
		/// </summary>
		/// <param name="value1">��һ������</param>
		/// <param name="value2">�ڶ�������</param>
		private void UpdateLedShow(string value1 = "", string value2 = "")
		{
			FrmDebugConsole.GetInstance().Output("����LED1:|" + value1 + "|" + value2 + "|");

			if (!this.LED1ConnectStatus) return;
			if (this.LED1PrevLedFileContent == value1 + value2) return;

			string ledContent = GenerateFillLedContent12(value1) + GenerateFillLedContent12(value2);

			File.WriteAllText(this.LED1TempFile, ledContent, Encoding.UTF8);

			if (LED1m_bSendBusy == false)
			{
				LED1m_bSendBusy = true;

				int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID);
				if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("����LED��̬����", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

				LED1m_bSendBusy = false;
			}

			this.LED1PrevLedFileContent = value1 + value2;
		}

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

		#endregion

		#region LED���ƿ�2

		/// <summary>
		/// LED2���ƿ�����
		/// </summary>
		int LED2nScreenNo = 2;
		/// <summary>
		/// LED2��̬�����
		/// </summary>
		int LED2DYArea_ID = 2;
		/// <summary>
		/// LED2���±�ʶ
		/// </summary>
		bool LED2m_bSendBusy = false;

		private bool _LED2ConnectStatus;
		/// <summary>
		/// LED1����״̬
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

				commonDAO.SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.LED��2_����״̬.ToString(), value ? "1" : "0");
			}
		}

		/// <summary>
		/// LED2��ʾ�����ı�
		/// </summary>
		string LED2TempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Led1TempFile.txt");

		/// <summary>
		/// LED2��һ����ʾ����
		/// </summary>
		string LED2PrevLedFileContent = string.Empty;

		/// <summary>
		/// ����LED��̬����
		/// </summary>
		/// <param name="value1">��һ������</param>
		/// <param name="value2">�ڶ�������</param>
		private void UpdateLedShow2(string value1 = "", string value2 = "")
		{
			FrmDebugConsole.GetInstance().Output("����LED1:|" + value1 + "|" + value2 + "|");

			if (!this.LED2ConnectStatus) return;
			if (this.LED2PrevLedFileContent == value1 + value2) return;

			string ledContent = GenerateFillLedContent12(value1) + GenerateFillLedContent12(value2);

			File.WriteAllText(this.LED2TempFile, ledContent, Encoding.UTF8);

			if (LED2m_bSendBusy == false)
			{
				LED2m_bSendBusy = true;

				int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED2nScreenNo, this.LED2DYArea_ID);
				if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("����LED��̬����", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

				LED2m_bSendBusy = false;
			}

			this.LED2PrevLedFileContent = value1 + value2;
		}

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

				this.InFactorySampler = commonDAO.GetAppletConfigString("��ڲ�����") == "1";

				// IO�����������ڣ�
				if (!string.IsNullOrEmpty(commonDAO.GetAppletConfigString("IO������_IP��ַ")) && commonDAO.TestPing(commonDAO.GetAppletConfigString("IO������_IP��ַ")))
				{
					Hardwarer.Iocer.OnReceived += new JMDMYTWI8DOMRIocer.ReceivedEventHandler(Iocer_Received);
					Hardwarer.Iocer.OnStatusChange += new JMDMYTWI8DOMRIocer.StatusChangeHandler(Iocer_StatusChange);
					Hardwarer.Iocer.OpenUDP(commonDAO.GetAppletConfigString("IO������_IP��ַ"), commonDAO.GetAppletConfigInt32("IO������_�˿�"));
					if (!Hardwarer.Iocer.Status) MessageBoxEx.Show("IO����������ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				this.iocControler = new IocControler(Hardwarer.Iocer);

				// ������1�����ڣ�
				if (!string.IsNullOrEmpty(commonDAO.GetAppletConfigString("������1_IP��ַ")) && commonDAO.TestPing(commonDAO.GetAppletConfigString("������1_IP��ַ")))
				{
					Hardwarer.Rwer1.StartWith = commonDAO.GetAppletConfigString("������_��ǩ����");
					Hardwarer.Rwer1.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer1_OnStatusChange);
					Hardwarer.Rwer1.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer1_OnScanError);
					success = Hardwarer.Rwer1.OpenCom(commonDAO.GetAppletConfigString("������1_IP��ַ"), commonDAO.GetAppletConfigInt32("������1_�˿�"), Convert.ToByte(commonDAO.GetAppletConfigInt32("������1_����")));
					if (!success) MessageBoxEx.Show("������1����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}

				#region LED���ƿ�1

				string led1SocketIP = commonDAO.GetAppletConfigString("LED��ʾ��1_IP��ַ");

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
								nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED1nScreenNo, this.LED1DYArea_ID, this.LED1TempFile, 0, "����", 12, 0, 120, 1, 3, 0);
								if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
								{
									nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID);
									if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
									{
										// ��ʼ���ɹ�
										this.LED1ConnectStatus = true;
										UpdateLedShow("  �ȴ�����");
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
					else
					{
						this.LED1ConnectStatus = false;
						Log4Neter.Error("��ʼ��LED1���ƿ�", new Exception("��������ʧ��"));
						MessageBoxEx.Show("LED1���ƿ�����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}

				#endregion

				#region LED���ƿ�2
				if (this.InFactorySampler)
				{
					string led2SocketIP = commonDAO.GetAppletConfigString("LED��ʾ��2_IP��ַ");

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
									nResult = YB14DynamicAreaLeder.AddScreenDynamicAreaFile(this.LED2nScreenNo, this.LED2DYArea_ID, this.LED2TempFile, 0, "����", 12, 0, 120, 1, 3, 0);
									if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
									{
										nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED2nScreenNo, this.LED2DYArea_ID);
										if (nResult == YB14DynamicAreaLeder.RETURN_NOERROR)
										{
											// ��ʼ���ɹ�
											this.LED2ConnectStatus = true;
											UpdateLedShow("  �ȴ�����");
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
						else
						{
							this.LED2ConnectStatus = false;
							Log4Neter.Error("��ʼ��LED2���ƿ�", new Exception("��������ʧ��"));
							MessageBoxEx.Show("LED2���ƿ�����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}
				}
				#endregion

				try
				{
					#region ����Ƶ

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
			if (this.iocControler != null && !this.InductorCoil1) this.iocControler.Gate1Down();
		}

		/// <summary>
		/// ��բ2����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGate2Up_Click(object sender, EventArgs e)
		{
			if (this.iocControler != null) FrontGateUp();
		}

		/// <summary>
		/// ��բ2����
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

		#region ����ҵ��

		/// <summary>
		/// ����������ʶ������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Stop();
			timer1.Interval = 1000;

			try
			{
				// ִ��Զ������
				ExecAppRemoteControlCmd();

				switch (this.CurrentFlowFlag)
				{
					case eFlowFlag.�ȴ�����:
						#region
						// ����������ظ����źţ������������߳���ʶ��
						if (this.InductorCoil1)
						{
							StartScan = true;
						}
						if (StartScan)
						{
							this.panCurrentCarNumber.Text = "��ʼ����";
							List<string> tags = Hardwarer.Rwer1.ScanTags();
							if (tags.Count > 0) passCarQueuer.Enqueue(tags[0]);
						}
						if (passCarQueuer.Count > 0) this.CurrentFlowFlag = eFlowFlag.��֤����;
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

						// ��ʽһ������ʶ��ĳ��ƺŲ��ҳ�����Ϣ
						//this.CurrentAutotruck = carTransportDAO.GetAutotruckByCarNumber(this.CurrentImperfectCar.Voucher);
						//UpdateLedShow(this.CurrentImperfectCar.Voucher);
						//// ��ʽ��������ʶ��ı�ǩ�����ҳ�����Ϣ
						this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);
						this.txtTagId.Text = this.CurrentImperfectCar.Voucher;
						if (this.CurrentAutotruck != null)
						{
							if (this.CurrentAutotruck.CarType == "�볧ú")
							{
								if (this.CurrentAutotruck.IsUse == 1)
								{
									if (this.CurrentAutotruck.CarriageLength > 0 && this.CurrentAutotruck.CarriageWidth > 0 && this.CurrentAutotruck.CarriageBottomToFloor > 0)
									{
										// δ��������¼
										CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, eCarType.�볧ú.ToString());
										if (unFinishTransport != null)
										{
											this.CurrentBuyFuelTransport = carTransportDAO.GetBuyFuelTransportById(unFinishTransport.TransportId);
											if (this.CurrentBuyFuelTransport != null)
											{
												if (this.CurrentBuyFuelTransport.SamplingType == "��е����")
												{
													if (string.IsNullOrEmpty(this.CurrentBuyFuelTransport.SamplePlace))
													{
														// �ж�·������
														string nextPlace;
														if (carTransportDAO.CheckNextTruckInFactoryWay(this.CurrentAutotruck.CarType, this.CurrentBuyFuelTransport.StepName, "����", CommonAppConfig.GetInstance().AppIdentifier, out nextPlace))
														{
															btnSendSamplingPlan.Enabled = true;

															this.CurrentFlowFlag = eFlowFlag.�ȴ�ʻ��;

															//UpdateLedShow(this.CurrentAutotruck.CarNumber, "Ϩ���³�");
															//this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " Ϩ���³�", 2, false);
														}
														else
														{
															UpdateLedShow("·�ߴ���", "��ֹͨ��");
															this.voiceSpeaker.Speak("·�ߴ��� ��ֹͨ�� " + (!string.IsNullOrEmpty(nextPlace) ? "��ǰ��" + nextPlace : ""), 2, false);

															timer1.Interval = 20000;
														}
													}
													else
													{
														UpdateLedShow(this.CurrentAutotruck.CarNumber, "��ͨ��");
														this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " �Ѳ��� ��ͨ��", 2, false);
														FrontGateUp();
														this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
													}
												}
												else
												{
													UpdateLedShow(this.CurrentAutotruck.CarNumber, "�������");
													this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ������� ��ͨ��", 2, false);
													FrontGateUp();
													this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
												}
											}
											else
											{
												commonDAO.SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
											}
										}
										else
										{
											this.UpdateLedShow(this.CurrentAutotruck.CarNumber, "δ�Ŷ�");
											this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " δ�ҵ��ŶӼ�¼", 2, false);
											timer1.Interval = 20000;
										}
									}
									else
									{
										this.UpdateLedShow(this.CurrentAutotruck.CarNumber, "����δ����");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ����δ����", 2, false);

										timer1.Interval = 20000;
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
								UpdateLedShow(this.CurrentAutotruck.CarNumber, "�������");
								this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ������� ��ͨ��", 2, false);
								FrontGateUp();
								this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
							}
						}
						else
						{
							UpdateLedShow("����δ�Ǽ�");

							// ��ʽһ������ʶ��
							this.voiceSpeaker.Speak("����δ�Ǽǣ���ֹͨ��", 2, false);
							//// ��ʽ����ˢ����ʽ
							//this.voiceSpeaker.Speak("����δ�Ǽǣ���ֹͨ��", 2, false);

							timer1.Interval = 20000;
						}

						#endregion
						break;

					case eFlowFlag.�ȴ�ʻ��:
						#region
						if (this.CurrentBuyFuelTransport.FPSamplePlace == "���")
						{
							#region ���
							// ����������ظ����źţ���ڵ�բ�ظ����ź�
							if ((this.InFactorySampler && !this.InductorCoil1))
							{
								if (this.SamplerMachineCode == "#3������е������")
								{
									string st = commonDAO.GetSignalDataValue("#1������е������", eSignalDataName.ϵͳ.ToString());
									string dg1 = commonDAO.GetSignalDataValue("�������ܻ�-#1��е��������", "�ظ�1�ź�");
									string dg2 = commonDAO.GetSignalDataValue("�������ܻ�-#1��е��������", "�ظ�2�ź�");
									if (st == "��������" && dg1 == "0" && dg2 == "0")
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "��ͨ��");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ��ǰ��ǰ������������", 2, false);
										FrontGateUp();
										this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "Ϩ���³�");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " Ϩ���³�", 2, false);
										BackGateDown();
										this.CurrentFlowFlag = eFlowFlag.���ͼƻ�;
									}

								}
								else if (this.SamplerMachineCode == "#4������е������")
								{
									string st = commonDAO.GetSignalDataValue("#2������е������", eSignalDataName.ϵͳ.ToString());
									string dg1 = commonDAO.GetSignalDataValue("�������ܻ�-#2��е��������", "�ظ�1�ź�");
									string dg2 = commonDAO.GetSignalDataValue("�������ܻ�-#2��е��������", "�ظ�2�ź�");
									if (st == "��������" && dg1 == "0" && dg2 == "0")
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "��ͨ��");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ��ǰ��ǰ������������", 2, false);
										FrontGateUp();
										this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "Ϩ���³�");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " Ϩ���³�", 2, false);
										BackGateDown();
										this.CurrentFlowFlag = eFlowFlag.���ͼƻ�;
									}

								}
								//BackGateDown();
								//this.CurrentFlowFlag = eFlowFlag.���ͼƻ�;
							}
							else if (!this.InFactorySampler && this.InductorCoil1)
							{
								if (this.CurrentBuyFuelTransport.StepName != "�볧")
								{
									UpdateLedShow(this.CurrentAutotruck.CarNumber, "��ͨ��");
									this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " �Ѳ���,��ͨ��", 2, false);
									FrontGateUp();
									this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
								}
								else
								{
									UpdateLedShow(this.CurrentAutotruck.CarNumber, "Ϩ���³�");
									this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " Ϩ���³�", 2, false);
									this.CurrentFlowFlag = eFlowFlag.���ͼƻ�;
								}
							}
							#endregion
						}
						else
						{
							#region �����
							if ((this.InFactorySampler && !this.InductorCoil1))
							{

								if (this.SamplerMachineCode == this.CurrentBuyFuelTransport.FPSamplePlace)
								{

									UpdateLedShow(this.CurrentAutotruck.CarNumber, "Ϩ���³�");
									this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " Ϩ���³�", 2, false);
									BackGateDown();
									this.CurrentFlowFlag = eFlowFlag.���ͼƻ�;
								}
								else
								{

									if (IsTrueOrFalse(this.SamplerMachineCode, this.CurrentBuyFuelTransport.FPSamplePlace))
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "��ͨ��");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ��ǰ��ǰ������������", 2, false);
										FrontGateUp();
										this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "�ߴ�ͨ��");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ��ǰ��" + this.CurrentBuyFuelTransport.FPSamplePlace + "���������в���", 2, false);
									}
								}

							}
							else if (!this.InFactorySampler && this.InductorCoil1)
							{
								if (this.CurrentBuyFuelTransport.StepName != "�볧")
								{
									UpdateLedShow(this.CurrentAutotruck.CarNumber, "��ͨ��");
									this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " �Ѳ���,��ͨ��", 2, false);
									FrontGateUp();
									this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
								}
								else
								{
									if (this.SamplerMachineCode == this.CurrentBuyFuelTransport.FPSamplePlace)
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "Ϩ���³�");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " Ϩ���³�", 2, false);
										this.CurrentFlowFlag = eFlowFlag.���ͼƻ�;
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "�ߴ������");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ��ǰ��" + this.CurrentBuyFuelTransport.FPSamplePlace + "���������в���", 2, false);
									}
								}
							}
							#endregion
						}

						// ����������
						timer1.Interval = 4000;

						#endregion
						break;

					case eFlowFlag.���ͼƻ�:
						#region

						// ��������ظ�,��⳵���Ƿ�ָ������
						if (!this.AutoHandMode || ((this.InFactorySampler && !this.InductorCoil1) || (!this.InFactorySampler && this.InductorCoil1)))
						{
							if (this.SamplerSystemStatus == eEquInfSamplerSystemStatus.��������)
							{
								CmcsRCSampling sampling = carTransportDAO.GetRCSamplingById(this.CurrentBuyFuelTransport.SamplingId);
								if (sampling != null)
								{
									//if (sampling.SamplingType == "��е����")
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
										// ����Ԥ��
										TicketWeight = this.CurrentBuyFuelTransport.TicketWeight,
										//TicketWeight = 0,
										// ����Ԥ��
										CarCount = carCount,
										// ����������������߼�����
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
										ResultCode = eEquInfCmdResultCode.Ĭ��.ToString(),
										DataFlag = 0
									};

									// ���Ͳ����ƻ�
									if (commonDAO.SelfDber.Insert<InfQCJXCYSampleCMD>(CurrentSampleCMD) > 0)
										this.CurrentFlowFlag = eFlowFlag.�ȴ�����;
									//}
									//else {
									//    this.UpdateLedShow("����ϵ����Ա!");
									//}
								}
								else
								{
									this.UpdateLedShow("δ�ҵ���������Ϣ");
									this.voiceSpeaker.Speak("δ�ҵ���������Ϣ������ϵ����Ա", 2, false);

									timer1.Interval = 5000;
								}
							}
							else
							{
								this.UpdateLedShow("������δ����");
								this.voiceSpeaker.Speak("������δ����", 1, false);

								timer1.Interval = 5000;
							}
						}

						#endregion
						break;

					case eFlowFlag.�ȴ�����:
						#region
						// �жϲ����Ƿ����
						//InfQCJXCYSampleCMD qCJXCYSampleCMD = commonDAO.SelfDber.Get<InfQCJXCYSampleCMD>(this.CurrentSampleCMD.Id);
						CmcsRCSampling samp = carTransportDAO.GetRCSamplingById(this.CurrentBuyFuelTransport.SamplingId);
						InfQCJXCYSampleCMD qCJXCYSampleCMD = commonDAO.SelfDber.Entity<InfQCJXCYSampleCMD>("where CarNumber=:CarNumber and SampleCode=:SampleCode and IsRead=0 and ResultCode='�ɹ�'", new { CarNumber = this.CurrentBuyFuelTransport.CarNumber, SampleCode = samp.SampleCode });
						//if (qCJXCYSampleCMD.ResultCode == eEquInfCmdResultCode.�ɹ�.ToString())
						if (qCJXCYSampleCMD != null)
						{
							jxSamplerDAO.IsRead(qCJXCYSampleCMD);
							//commonDAO.SetSignalDataValue("����1", this.CurrentSampleCMD.Id, this.CurrentBuyFuelTransport.Id);
							if (jxSamplerDAO.SaveBuyFuelTransport(this.CurrentBuyFuelTransport.Id, DateTime.Now, CommonAppConfig.GetInstance().AppIdentifier, qCJXCYSampleCMD.BARREL, qCJXCYSampleCMD.SAMPLEWEIGHT, qCJXCYSampleCMD.Id + ".jpg"))
							{
								//commonDAO.SetSignalDataValue("����2", this.CurrentSampleCMD.Id, this.CurrentBuyFuelTransport.Id);
								FrontGateUp();

								this.UpdateLedShow("  �������", "  ���뿪");
								this.voiceSpeaker.Speak("������ɣ����뿪��������", 2, false);

								this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
							}
						}
						// ����������
						timer1.Interval = 4000;

						#endregion
						break;

					case eFlowFlag.�ȴ��뿪:
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

						// ����������
						timer1.Interval = 4000;

						break;
				}

				// ���еظ����ź�ʱ����
				//if (this.AutoHandMode && !this.InductorCoil1 && !this.InductorCoil2 && this.CurrentFlowFlag != eFlowFlag.�ȴ�����
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
			// ������ִ��һ��
			timer2.Interval = 60000;

			try
			{
				// �볧ú
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
		/// �л��ֶ�/�Զ�ģʽ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sbtnChangeAutoHandMode_ValueChanged(object sender, EventArgs e)
		{
			this.AutoHandMode = sbtnChangeAutoHandMode.Value;
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

		/// <summary>
		/// ���Ͳ����ƻ�
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSendSamplingPlan_Click(object sender, EventArgs e)
		{
			if (SendSamplingPlan()) MessageBoxEx.Show("����ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// �����볧ú�����¼
		/// </summary>
		/// <returns></returns>
		bool SendSamplingPlan()
		{
			if (this.CurrentBuyFuelTransport == null)
			{
				MessageBoxEx.Show("��ѡ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			this.CurrentFlowFlag = eFlowFlag.���ͼƻ�;

			return false;
		}

		/// <summary>
		/// �����볧ú�����¼
		/// </summary>
		void ResetBuyFuel()
		{
			this.StartScan = false;
			this.IsCrossInductorCoil2 = false;
			this.CurrentFlowFlag = eFlowFlag.�ȴ�����;

			this.CurrentAutotruck = null;
			this.CurrentBuyFuelTransport = null;

			txtTagId.ResetText();

			btnSendSamplingPlan.Enabled = false;

			FrontGateDown();
			if (this.InFactorySampler)
				BackGateUp();
			UpdateLedShow("  �ȴ�����");

			// �������
			this.CurrentImperfectCar = null;
		}

		/// <summary>
		/// ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnReset_Click(object sender, EventArgs e)
		{
			ResetBuyFuel();
		}

		/// <summary>
		/// ��ȡδ��ɵ��볧ú��¼
		/// </summary>
		void LoadTodayUnFinishBuyFuelTransport()
		{
			superGridControl1.PrimaryGrid.DataSource = jxSamplerDAO.GetUnFinishBuyFuelTransport();
		}

		/// <summary>
		/// ��ȡָ����������ɵ��볧ú��¼
		/// </summary>
		void LoadTodayFinishBuyFuelTransport()
		{
			superGridControl2.PrimaryGrid.DataSource = jxSamplerDAO.GetFinishedBuyFuelTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
		}

		/// <summary>
		/// ������ڲ�����LED��ʾ
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

		#region ������Ϣ

		/// <summary>
		/// ѡ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSelectAutotruck_Click(object sender, EventArgs e)
		{
			FrmUnFinishTransport_Select frm = new FrmUnFinishTransport_Select("where CarType='" + eCarType.�볧ú.ToString() + "' order by CreateDate desc");
			if (frm.ShowDialog() == DialogResult.OK)
			{
				passCarQueuer.Enqueue(frm.Output.TagId);
				this.CurrentFlowFlag = eFlowFlag.��֤����;
			}
		}

		#endregion

		#region ����

		Pen redPen3 = new Pen(Color.Red, 3);
		Pen greenPen3 = new Pen(Color.Lime, 3);

		/// <summary>
		/// ��ǰ����������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panCurrentCarNumber_Paint(object sender, PaintEventArgs e)
		{
			PanelEx panel = sender as PanelEx;

			int height = 12;
			if (this.InFactorySampler)
			{
				// ���Ƶظ�1
				e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, 1, 15, height);
				e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, panel.Height - height, 15, panel.Height - 1);
			}
			else
			{
				// ���Ƶظ�1
				e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, panel.Width - 55, 1, panel.Width - 55, height);
				e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, panel.Width - 55, panel.Height - height, panel.Width - 55, panel.Height - 1);
			}
			// ���Ƶظ�2
			e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, panel.Width - 15, 1, panel.Width - 15, height);
			e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, panel.Width - 15, panel.Height - height, panel.Width - 15, panel.Height - 1);
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
		/// Invoke��װ
		/// </summary>
		/// <param name="action"></param>
		public void InvokeEx(Action action)
		{
			if (this.IsDisposed || !this.IsHandleCreated) return;

			this.Invoke(action);
		}

		/// <summary>
		/// ���²�����״̬
		/// </summary>
		private void RefreshEquStatus()
		{
			string systemStatus = commonDAO.GetSignalDataValue(this.SamplerMachineCode, eSignalDataName.ϵͳ.ToString());
			eEquInfSamplerSystemStatus result;
			if (Enum.TryParse(systemStatus, out result)) SamplerSystemStatus = result;
		}

		/// <summary>
		/// Ԥ������������Ϣͼ
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
		/// �жϵ�ǰ����������Ĳ������Ƿ��ǵ�ǰ��ڲ����������ĵ�·
		/// </summary>
		/// <param name="cyj1"></param>
		/// <param name="cyj2"></param>
		/// <returns></returns>
		private bool IsTrueOrFalse(string cyj1, string cyj2)
		{
			string[] s1 = new[] { "#1������е������", "#3������е������" };
			string[] s2 = new[] { "#2������е������", "#4������е������" };
			if ((s1.Contains(cyj1) && s1.Contains(cyj2)) || (s2.Contains(cyj1) && s2.Contains(cyj2)))
			{
				return true;
			}
			return false;
		}
		#endregion

	}
}
