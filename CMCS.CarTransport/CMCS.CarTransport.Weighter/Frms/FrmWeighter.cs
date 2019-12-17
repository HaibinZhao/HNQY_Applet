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
using CMCS.CarTransport.Weighter.Core;
using CMCS.CarTransport.Weighter.Enums;
using CMCS.CarTransport.Weighter.Frms.Sys;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.Sys;
using CMCS.Common.Enums;
using CMCS.Common.Utilities;
using CMCS.Common.Views;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.SuperGrid;
using HikVisionSDK.Core;
using LED.YB14;
using IOC.JMDMYTWI8DOMR;
using CMCS.CarTransport.Weighter.Utilities;


namespace CMCS.CarTransport.Weighter.Frms
{
	public partial class FrmWeighter : DevComponents.DotNetBar.Metro.MetroForm
	{
		/// <summary>
		/// ����Ψһ��ʶ��
		/// </summary>
		public static string UniqueKey = "FrmWeighter";

		public FrmWeighter()
		{
			InitializeComponent();
		}

		#region Vars

		/// <summary>
		/// �Ƿ������ϰ�
		/// </summary>
		private bool IsUpGateing = false;

		/// <summary>
		/// �Ƿ��ϰ����
		/// </summary>
		private bool IsUpGate = false;

		/// <summary>
		/// ��ʶ�Ƿ��ѿ����
		/// </summary>
		private bool IsUpRed = false;

		/// <summary>
		/// �Ƿ��Ѹ���LED
		/// </summary>
		private bool IsUpdateLED = false;

		CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();
		WeighterDAO weighterDAO = WeighterDAO.GetInstance();
		CommonDAO commonDAO = CommonDAO.GetInstance();
		TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();
		CommonAppConfig commonAppConfig = CommonAppConfig.GetInstance();

		/// <summary>
		/// �ȴ��ϴ���ץ��
		/// </summary>
		Queue<string> waitForUpload = new Queue<string>();

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

				panCurrentWeight.Refresh();

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

				panCurrentWeight.Refresh();

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

				panCurrentWeight.Refresh();

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

				panCurrentWeight.Refresh();

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

		bool infraredSensor1 = false;
		/// <summary>
		/// ����1״̬ true=�ڵ�  false=��ͨ
		/// </summary>
		public bool InfraredSensor1
		{
			get
			{
				return infraredSensor1;
			}
			set
			{
				infraredSensor1 = value;

				panCurrentWeight.Refresh();

				commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.����1�ź�.ToString(), value ? "1" : "0");
			}
		}

		int infraredSensor1Port;
		/// <summary>
		/// ����1�˿�
		/// </summary>
		public int InfraredSensor1Port
		{
			get { return infraredSensor1Port; }
			set { infraredSensor1Port = value; }
		}

		bool infraredSensor2 = false;
		/// <summary>
		/// ����2״̬ true=�ڵ�  false=��ͨ
		/// </summary>
		public bool InfraredSensor2
		{
			get
			{
				return infraredSensor2;
			}
			set
			{
				infraredSensor2 = value;

				panCurrentWeight.Refresh();

				commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.����2�ź�.ToString(), value ? "1" : "0");
			}
		}

		int infraredSensor2Port;
		/// <summary>
		/// ����2�˿�
		/// </summary>
		public int InfraredSensor2Port
		{
			get { return infraredSensor2Port; }
			set { infraredSensor2Port = value; }
		}




		bool buttonSensor = false;
		/// <summary>
		/// ��ť״̬ true=��  false=��
		/// </summary>
		public bool ButtonSensor
		{
			get
			{
				return buttonSensor;
			}
			set
			{
				buttonSensor = value;
				slightButtonSensor.LightColor = value ? Color.Green : Color.Red;

				commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ť�ź�.ToString(), value ? "1" : "0");
			}
		}


		int buttonSensorPort;
		/// <summary>
		/// ��ť�˿�
		/// </summary>
		public int ButtonSensorPort
		{
			get { return buttonSensorPort; }
			set { buttonSensorPort = value; }
		}



		bool wbSteady = false;
		/// <summary>
		/// �ذ��Ǳ��ȶ�״̬
		/// </summary>
		public bool WbSteady
		{
			get { return wbSteady; }
			set
			{
				wbSteady = value;

				this.panCurrentWeight.Style.ForeColor.Color = (value ? Color.Lime : Color.Red);

				panCurrentWeight.Refresh();

				commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.�ذ��Ǳ�_�ȶ�.ToString(), value ? "1" : "0");
			}
		}

		double wbMinWeight = 0;
		/// <summary>
		/// �ذ��Ǳ���С���� ��λ����
		/// </summary>
		public double WbMinWeight
		{
			get { return wbMinWeight; }
			set
			{
				wbMinWeight = value;
			}
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

				btnSelectAutotruck_BuyFuel.Visible = !value;
				btnSelectAutotruck_Goods.Visible = !value;

				btnSaveTransport_BuyFuel.Visible = !value;
				btnSaveTransport_Goods.Visible = !value;

				btnReset_BuyFuel.Visible = !value;
				btnReset_Goods.Visible = !value;
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
					}
					else if (value.CarType == eCarType.��������.ToString())
					{
						if (ePCCard != null) txtTagId_Goods.Text = ePCCard.TagId;

						txtCarNumber_Goods.Text = value.CarNumber;
						superTabControl2.SelectedTab = superTabItem_Goods;
					}

					panCurrentCarNumber.Text = value.CarNumber;
				}
				else
				{
					commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ��Id.ToString(), string.Empty);
					commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ����.ToString(), string.Empty);

					txtCarNumber_BuyFuel.ResetText();
					txtCarNumber_Goods.ResetText();

					txtTagId_BuyFuel.ResetText();
					txtTagId_Goods.ResetText();
				}
			}
		}

		String printTransportId;
		/// <summary>
		/// ��ǰҪ��ӡ�������¼��Id
		/// </summary>
		public String PrintTransportId
		{
			get { return printTransportId; }
			set { printTransportId = value; }
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
			commonDAO.ResetAppRemoteControlCmd(commonAppConfig.AppIdentifier);
		}

		private void FrmWeighter_Load(object sender, EventArgs e)
		{
		}

		private void FrmWeighter_Shown(object sender, EventArgs e)
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
			// �����豸״̬ 
			InvokeEx(() =>
			{
				slightIOC.LightColor = (status ? Color.Green : Color.Red);
				if (!commonDAO.TestPing(commonDAO.GetAppletConfigString("IO������_IP��ַ"))) Log4Neter.Info("IO����������Ͽ�");
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
				this.InfraredSensor1 = (receiveValue[this.InfraredSensor1Port - 1] == 1);
				this.InfraredSensor2 = (receiveValue[this.InfraredSensor2Port - 1] == 1);
				this.ButtonSensor = (receiveValue[this.ButtonSensorPort - 1] == 1);
			});
		}

		/// <summary>
		/// ǰ������
		/// </summary>
		void FrontGateUp()
		{
			if (this.CurrentImperfectCar == null || this.iocControler == null) return;

			if (this.CurrentImperfectCar.PassWay == eDirection.Way1)
			{
				this.iocControler.Gate2Up();
				this.iocControler.GreenLight2();
			}
			else if (this.CurrentImperfectCar.PassWay == eDirection.Way2)
			{
				this.iocControler.Gate1Up();
				this.iocControler.GreenLight1();
			}
		}

		/// <summary>
		/// ǰ������
		/// </summary>
		void FrontGateDown()
		{
			if (this.CurrentImperfectCar == null || this.iocControler == null) return;

			if (this.CurrentImperfectCar.PassWay == eDirection.Way1)
			{
				this.iocControler.Gate2Down();
				this.iocControler.RedLight2();
			}
			else if (this.CurrentImperfectCar.PassWay == eDirection.Way2)
			{
				this.iocControler.Gate1Down();
				this.iocControler.RedLight1();
			}
		}


		void ResetGate()
		{
			if (!this.InductorCoil2)
				this.iocControler.Gate2Down();
			this.iocControler.RedLight2();


			this.iocControler.Gate1Up();
			this.iocControler.GreenLight1();
		}

		/// <summary>
		/// ������
		/// </summary>
		void BackGateUp()
		{
			if (this.CurrentImperfectCar == null || this.iocControler == null) return;

			if (this.CurrentImperfectCar.PassWay == eDirection.Way1)
			{
				this.iocControler.Gate1Up();
				this.iocControler.GreenLight1();
			}
			else if (this.CurrentImperfectCar.PassWay == eDirection.Way2)
			{
				this.iocControler.Gate2Up();
				this.iocControler.GreenLight2();
			}
		}

		/// <summary>
		/// �󷽽���
		/// </summary>
		void BackGateDown()
		{
			if (this.CurrentImperfectCar == null || this.iocControler == null) return;

			if (this.CurrentImperfectCar.PassWay == eDirection.Way1)
			{
				this.iocControler.Gate1Down();
				this.iocControler.RedLight1();
			}
			else if (this.CurrentImperfectCar.PassWay == eDirection.Way2)
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
			// �����豸״̬ 
			InvokeEx(() =>
			{
				slightRwer1.LightColor = (status ? Color.Green : Color.Red);

				commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.������1_����״̬.ToString(), status ? "1" : "0");
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

				int nResult = YB14DynamicAreaLeder.SendDynamicAreaInfoCommand(this.LED1nScreenNo, this.LED1DYArea_ID);
				if (nResult != YB14DynamicAreaLeder.RETURN_NOERROR) Log4Neter.Error("����LED��̬����", new Exception(YB14DynamicAreaLeder.GetErrorMessage("SendDynamicAreaInfoCommand", nResult)));

				LED1m_bSendBusy = false;
			}

			this.LED1PrevLedFileContent = value1 + value2;
		}

		#endregion

		#endregion

		#region �ذ��Ǳ�

		/// <summary>
		/// �����ȶ��¼�
		/// </summary>
		/// <param name="steady"></param>
		void Wber_OnSteadyChange(bool steady)
		{
			InvokeEx(() =>
			  {
				  this.WbSteady = steady;
			  });
		}

		/// <summary>
		/// �ذ��Ǳ�״̬�仯
		/// </summary>
		/// <param name="status"></param>
		void Wber_OnStatusChange(bool status)
		{
			// �����豸״̬ 
			InvokeEx(() =>
			{
				slightWber.LightColor = (status ? Color.Green : Color.Red);

				commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.�ذ��Ǳ�_����״̬.ToString(), status ? "1" : "0");
			});
		}

		void Wber_OnWeightChange(double weight)
		{
			InvokeEx(() =>
			{
				panCurrentWeight.Text = weight.ToString() + "��";
			});
		}

		#endregion

		#region ������Ƶ

		/// <summary>
		/// �������������
		/// </summary>
		IPCer iPCer1 = new IPCer();
		IPCer iPCer2 = new IPCer();

		/// <summary>
		/// ִ������ͷץ�ģ�����������
		/// </summary>
		/// <param name="transportId">�����¼Id</param>
		private void CamareCapturePicture(string transportId)
		{
			try
			{
				// ץ����Ƭ������������ַ
				string pictureWebUrl = commonDAO.GetCommonAppletConfigString("�������ܻ�_ץ����Ƭ����·��");

				// �����1
				string picture1FileName = Path.Combine(SelfVars.CapturePicturePath, Guid.NewGuid().ToString() + ".bmp");
				if (iPCer1.CapturePicture(picture1FileName))
				{
					CmcsTransportPicture transportPicture = new CmcsTransportPicture()
					{
						CaptureTime = DateTime.Now,
						CaptureType = commonAppConfig.AppIdentifier,
						TransportId = transportId,
						PicturePath = pictureWebUrl + Path.GetFileName(picture1FileName),
						IsUpLoad = 0
					};

					if (commonDAO.SelfDber.Insert(transportPicture) > 0) waitForUpload.Enqueue(picture1FileName);
				}

				// �����2
				string picture2FileName = Path.Combine(SelfVars.CapturePicturePath, "Camera", Guid.NewGuid().ToString() + ".bmp");
				if (iPCer2.CapturePicture(picture2FileName))
				{
					CmcsTransportPicture transportPicture = new CmcsTransportPicture()
					{
						CaptureTime = DateTime.Now,
						CaptureType = commonAppConfig.AppIdentifier,
						TransportId = transportId,
						PicturePath = pictureWebUrl + Path.GetFileName(picture1FileName),
						IsUpLoad = 0
					};

					if (commonDAO.SelfDber.Insert(transportPicture) > 0) waitForUpload.Enqueue(picture2FileName);
				}
			}
			catch (Exception ex)
			{
				Log4Neter.Error("�����ץ��", ex);
			}
		}

		/// <summary>
		/// �ϴ�ץ����Ƭ�������������ļ���
		/// </summary>
		private void UploadCapturePicture()
		{
			string serverPath = commonDAO.GetCommonAppletConfigString("�������ܻ�_ץ����Ƭ����������·��");
			if (string.IsNullOrEmpty(serverPath)) return;

			string fileName = string.Empty;
			while (this.waitForUpload.Count > 0)
			{
				fileName = this.waitForUpload.Dequeue();
				if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
				{
					try
					{
						if (Directory.Exists(serverPath)) File.Copy(fileName, Path.Combine(serverPath, Path.GetFileName(fileName)), true);
					}
					catch (Exception ex)
					{
						Log4Neter.Error("�ϴ�ץ����Ƭ", ex);

						break;
					}
				}
			}
		}

		/// <summary>
		/// ������ڵ�ץ����Ƭ
		/// </summary> 
		public void ClearExpireCapturePicture()
		{
			foreach (string item in Directory.GetFiles(SelfVars.CapturePicturePath).Where(a =>
			{
				return new FileInfo(a).LastWriteTime > DateTime.Now.AddMonths(-3);
			}))
			{
				try
				{
					File.Delete(item);
				}
				catch { }
			}
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
				this.InductorCoil3Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�3�˿�");
				this.InductorCoil4Port = commonDAO.GetAppletConfigInt32("IO������_�ظ�4�˿�");
				this.InfraredSensor1Port = commonDAO.GetAppletConfigInt32("IO������_����1�˿�");
				this.InfraredSensor2Port = commonDAO.GetAppletConfigInt32("IO������_����2�˿�");
				this.ButtonSensorPort = commonDAO.GetAppletConfigInt32("IO������_��ť�˿�");

				this.WbMinWeight = commonDAO.GetAppletConfigDouble("�ذ��Ǳ�_��С����");

				// IO�����������ڣ�
				if (!string.IsNullOrEmpty(commonDAO.GetAppletConfigString("IO������_IP��ַ")) && commonDAO.TestPing(commonDAO.GetAppletConfigString("IO������_IP��ַ")))
				{
					Hardwarer.Iocer.OnReceived += new JMDMYTWI8DOMRIocer.ReceivedEventHandler(Iocer_Received);
					Hardwarer.Iocer.OnStatusChange += new JMDMYTWI8DOMRIocer.StatusChangeHandler(Iocer_StatusChange);
					Hardwarer.Iocer.OpenUDP(commonDAO.GetAppletConfigString("IO������_IP��ַ"), commonDAO.GetAppletConfigInt32("IO������_�˿�"));
					if (!Hardwarer.Iocer.Status) MessageBoxEx.Show("IO����������ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				this.iocControler = new IocControler(Hardwarer.Iocer);

				// �ذ��Ǳ�
				Hardwarer.Wber.OnStatusChange += new WB.TOLEDO.IND245.TOLEDO_IND245Wber.StatusChangeHandler(Wber_OnStatusChange);
				Hardwarer.Wber.OnSteadyChange += new WB.TOLEDO.IND245.TOLEDO_IND245Wber.SteadyChangeEventHandler(Wber_OnSteadyChange);
				Hardwarer.Wber.OnWeightChange += new WB.TOLEDO.IND245.TOLEDO_IND245Wber.WeightChangeEventHandler(Wber_OnWeightChange);
				success = Hardwarer.Wber.OpenCom(commonDAO.GetAppletConfigInt32("�ذ��Ǳ�_����"), commonDAO.GetAppletConfigInt32("�ذ��Ǳ�_������"), commonDAO.GetAppletConfigInt32("�ذ��Ǳ�_����λ"), commonDAO.GetAppletConfigInt32("�ذ��Ǳ�_ֹͣλ"));

				// ������1�����ڣ�
				if (!string.IsNullOrEmpty(commonDAO.GetAppletConfigString("������1_IP��ַ")) && commonDAO.TestPing(commonDAO.GetAppletConfigString("������1_IP��ַ")))
				{
					Hardwarer.Rwer1.StartWith = commonDAO.GetAppletConfigString("������_��ǩ����");
					Hardwarer.Rwer1.OnStatusChange += new RW.LZR12.Lzr12Rwer.StatusChangeHandler(Rwer1_OnStatusChange);
					Hardwarer.Rwer1.OnScanError += new RW.LZR12.Lzr12Rwer.ScanErrorEventHandler(Rwer1_OnScanError);
					success = Hardwarer.Rwer1.OpenCom(commonDAO.GetAppletConfigString("������1_IP��ַ"), commonDAO.GetAppletConfigInt32("������1_�˿�"), Convert.ToByte(commonDAO.GetAppletConfigInt32("������1_����")));
					if (!success) MessageBoxEx.Show("������1����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}

				#region ������Ƶ

				if (commonDAO.GetAppletConfigString("���������") == "1")
				{
					IPCer.InitSDK();

					CmcsCamare video1 = commonDAO.SelfDber.Entity<CmcsCamare>("where EquipmentCode=:EquipmentCode", new { EquipmentCode = commonAppConfig.AppIdentifier + "����ͷ1" });
					if (video1 != null)
					{
						if (iPCer1.Login(video1.Ip, video1.Port, video1.UserName, video1.Password))
						{
							iPCer1.StartPreview(panVideo1.Handle, video1.Channel);
						}
					}

					CmcsCamare video2 = commonDAO.SelfDber.Entity<CmcsCamare>("where EquipmentCode=:EquipmentCode", new { EquipmentCode = commonAppConfig.AppIdentifier + "����ͷ2" });
					if (video2 != null)
					{
						if (iPCer2.Login(video2.Ip, video2.Port, video2.UserName, video2.Password))
							iPCer2.StartPreview(panVideo2.Handle, video2.Channel);
					}
				}
				#endregion
				
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

				//UnLoadLEDDAO.GetInstance().SendUnLoadLED("��SFV535", false);//����ʹ��

				timer1.Enabled = true;
				timer2.Enabled = true;
				timer2_Tick(null, null);

				#region ����Ƶ
				try
				{
					if (commonDAO.GetAppletConfigString("���������") == "1")
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
								if (DHSDK.StartPreview(panVideo1.Handle, szCameraId1, nPDLLHandle, realseq))
								{
									panVideo1.Refresh();
								}
								#endregion
								#region 2����Ƶ
								string szCameraId2 = strID2;
								if (DHSDK.StartPreview(panVideo2.Handle, szCameraId2, nPDLLHandle, realseq))
								{
									panVideo2.Refresh();
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
				}
				catch
				{

				}





				#endregion

				ResetGate();//��ʼ����բ ���̵�״̬
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
				//IO ������

				Hardwarer.Iocer.OnReceived -= new JMDMYTWI8DOMRIocer.ReceivedEventHandler(Iocer_Received);
				Hardwarer.Iocer.OnStatusChange -= new JMDMYTWI8DOMRIocer.StatusChangeHandler(Iocer_StatusChange);

				Hardwarer.Iocer.ClostUDP();
				taskSimpleScheduler.Cancal();
			}
			catch { }


			try
			{
				// �ذ��Ǳ�

				Hardwarer.Wber.OnStatusChange -= new WB.TOLEDO.IND245.TOLEDO_IND245Wber.StatusChangeHandler(Wber_OnStatusChange);
				Hardwarer.Wber.OnSteadyChange -= new WB.TOLEDO.IND245.TOLEDO_IND245Wber.SteadyChangeEventHandler(Wber_OnSteadyChange);
				Hardwarer.Wber.OnWeightChange -= new WB.TOLEDO.IND245.TOLEDO_IND245Wber.WeightChangeEventHandler(Wber_OnWeightChange);
				Hardwarer.Wber.CloseCom();
			}
			catch { }



			try
			{
				//������

				Hardwarer.Rwer1.CloseCom();
			}
			catch { }

			try
			{
				//LED1 
				if (LED1ConnectStatus)
				{
					YB14DynamicAreaLeder.SendDeleteDynamicAreasCommand(this.LED1nScreenNo, 1, "");
					YB14DynamicAreaLeder.DeleteScreen(this.LED1nScreenNo);
				}
			}
			catch { }
			try
			{
				IPCer.CleanupSDK();
			}
			catch { }

			try
			{
				IntPtr nPDLLHandle = (IntPtr)0;
				IntPtr realseq = default(IntPtr);
				if (DHSDK.closevideo(nPDLLHandle, realseq))
				{

					panVideo1.Refresh();
					panVideo2.Refresh();
				}

			}
			catch { }

		}

		#endregion

		void OutputError(string text, Exception ex)
		{
			Log4Neter.Error(text, ex);
		}
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
			if (this.iocControler != null && !this.InductorCoil2) this.iocControler.Gate1Down();
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
			if (this.iocControler != null && !this.InductorCoil3) this.iocControler.Gate2Down();
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

			try
			{
				// ִ��Զ������
				ExecAppRemoteControlCmd();

				switch (this.CurrentFlowFlag)
				{
					case eFlowFlag.�ȴ�����:
						#region
						if ((this.InductorCoil1 && this.InductorCoil2 && this.InfraredSensor1) || Hardwarer.Wber.Weight >= this.WbMinWeight)
						{
							Log4Neter.Info(getLog("�ȴ�����"));
							UpdateLedShow(" �����ϰ���");

							commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.�ϰ�����.ToString(), "1");

							this.CurrentFlowFlag = eFlowFlag.�ȴ��ϰ�;
						}
						#endregion
						break;

					case eFlowFlag.�ȴ��ϰ�:
						#region
						Log4Neter.Info(getLog("�ȴ��ϰ�1"));
						// ���ذ��Ǳ�����������С��������������ĵظ����������źţ����ж����Ѿ���ȫ�ϰ�
						if (Hardwarer.Wber.Weight > this.WbMinWeight && !this.InductorCoil1 && !this.InductorCoil2 && InfraredSensor1 == false) // 
						{
							Log4Neter.Info(getLog("�ȴ��ϰ�2"));
							this.CurrentFlowFlag = eFlowFlag.��ʼ����;
							this.iocControler.RedLight1();
							this.iocControler.Gate1Down();

							UpdateLedShow(" ��ʼ����");
							this.voiceSpeaker.Speak("��ʼ����", 1, true);
							Log4Neter.Info(getLog("�ȴ��ϰ�3"));
						}

						#endregion
						break;

					case eFlowFlag.��ʼ����:
						#region
						timer1.Interval = 100;//���������

						List<string> tags = Hardwarer.Rwer1.ScanTags();//��ʼ����
						if (tags.Count > 0)
						{
							passCarQueuer.Enqueue(eDirection.Way1, tags[0]);

							if (passCarQueuer.Count > 0) this.CurrentFlowFlag = eFlowFlag.ʶ����;
						}

						#endregion
						break;

					case eFlowFlag.ʶ����:
						#region

						// �������޳�ʱ���ȴ�����
						if (passCarQueuer.Count == 0)
						{
							this.CurrentFlowFlag = eFlowFlag.�ȴ�����;
							break;
						}

						this.CurrentImperfectCar = passCarQueuer.Dequeue();

						this.CurrentAutotruck = carTransportDAO.GetAutotruckByTagId(this.CurrentImperfectCar.Voucher);

						if (this.CurrentAutotruck != null)
						{
							if (this.CurrentAutotruck.IsUse == 1)
							{
								if (this.CurrentAutotruck.CarType == eCarType.�볧ú.ToString())
								{
									//CamareCapturePicture(this.CurrentBuyFuelTransport.Id);
									this.timer_BuyFuel_Cancel = false;
									this.CurrentFlowFlag = eFlowFlag.��֤��Ϣ;
									timer_BuyFuel_Tick(null, null);
								}
								else if (this.CurrentAutotruck.CarType == eCarType.��������.ToString())
								{
									//CamareCapturePicture(this.CurrentGoodsTransport.Id);
									this.timer_Goods_Cancel = false;
									this.CurrentFlowFlag = eFlowFlag.��֤��Ϣ;
									timer_Goods_Tick(null, null);
								}
							}
							else
							{
								UpdateLedShow(this.CurrentAutotruck.CarNumber, "��ͣ��");

								this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ��ͣ�ã�����ϵ����Ա", 2, false);

								timer1.Interval = 20000;
							}
						}
						else
						{
							UpdateLedShow("����δ�Ǽ�");

							// ��ʽһ������ʶ��
							this.voiceSpeaker.Speak("����δ�Ǽǣ�����ϵ����Ա", 2, false);

							timer1.Interval = 20000;
						}

						#endregion
						break;
				}
				if (!this.InductorCoil1 && !this.InductorCoil2 && !this.InductorCoil3 && !this.InductorCoil3 && !this.InductorCoil4 && Hardwarer.Wber.Weight < this.WbMinWeight && this.CurrentFlowFlag != eFlowFlag.�ȴ�����) { ResetBuyFuel(); ResetGoods(); };
				commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.�ذ��Ǳ�_ʵʱ����.ToString(), Hardwarer.Wber.Weight.ToString());
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

				// �ϴ�ץ����Ƭ
				UploadCapturePicture();
				// ����ץ����Ƭ
				if (DateTime.Now.Hour == 0) ClearExpireCapturePicture();
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
		/// �г������ϰ��ĵ�·��
		/// </summary>
		/// <returns></returns>
		bool HasCarOnEnterWay()
		{
			if (this.CurrentImperfectCar == null) return false;

			if (this.CurrentImperfectCar.PassWay == eDirection.UnKnow)
				return false;
			else if (this.CurrentImperfectCar.PassWay == eDirection.Way1)
				return this.InductorCoil1 || this.InductorCoil2 || this.InfraredSensor1;
			else if (this.CurrentImperfectCar.PassWay == eDirection.Way2)
				return this.InductorCoil3 || this.InductorCoil4 || this.InfraredSensor2;

			return true;
		}

		/// <summary>
		/// �г������°��ĵ�·��
		/// </summary>
		/// <returns></returns>
		bool HasCarOnLeaveWay()
		{
			if (this.CurrentImperfectCar == null) return false;

			if (this.CurrentImperfectCar.PassWay == eDirection.UnKnow)
				return false;
			else if (this.CurrentImperfectCar.PassWay == eDirection.Way1)
				return this.InductorCoil3 || this.InductorCoil4 || this.InfraredSensor2;
			else if (this.CurrentImperfectCar.PassWay == eDirection.Way2)
				return this.InductorCoil1 || this.InductorCoil2 || this.InfraredSensor1;

			return true;
		}

		/// <summary>
		/// �г����ڵ�·��
		/// </summary>
		/// <returns></returns>
		bool HasCarOnWay()
		{
			if (this.CurrentImperfectCar == null) return false;
			return this.InductorCoil3 || this.InductorCoil4 || this.InfraredSensor2 || this.InductorCoil1 || this.InductorCoil2 || this.InfraredSensor1;
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

		#region �볧úҵ��

		bool timer_BuyFuel_Cancel = true;

		CmcsBuyFuelTransport currentBuyFuelTransport;
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
					commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), value.Id);

					txtFuelKindName_BuyFuel.Text = value.FuelKindName;
					//txtMineName_BuyFuel.Text = value.MineName;
					//txtSupplierName_BuyFuel.Text = value.SupplierName;
					txtMineName_BuyFuel.Text = "****";
					txtSupplierName_BuyFuel.Text = "****";
					txtTransportCompanyName_BuyFuel.Text = value.TransportCompanyName;

					txtGrossWeight_BuyFuel.Text = value.GrossWeight.ToString("F2");
					txtTicketWeight_BuyFuel.Text = value.TicketWeight.ToString("F2");
					txtTareWeight_BuyFuel.Text = value.TareWeight.ToString("F2");
					txtDeductWeight_BuyFuel.Text = value.DeductWeight.ToString("F2");
					txtSuttleWeight_BuyFuel.Text = value.SuttleWeight.ToString("F2");
					txtProfitWeight_BuyFuel.Text = value.ProfitWeight.ToString("F2");
				}
				else
				{
					commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), string.Empty);

					txtFuelKindName_BuyFuel.ResetText();
					txtMineName_BuyFuel.ResetText();
					txtSupplierName_BuyFuel.ResetText();
					txtTransportCompanyName_BuyFuel.ResetText();

					txtGrossWeight_BuyFuel.ResetText();
					txtTicketWeight_BuyFuel.ResetText();
					txtTareWeight_BuyFuel.ResetText();
					txtDeductWeight_BuyFuel.ResetText();
					txtSuttleWeight_BuyFuel.ResetText();
					txtProfitWeight_BuyFuel.ResetText();
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
			//ѡ���볧ú��������δ��ɵ�
			FrmUnFinishTransport_Select frm = new FrmUnFinishTransport_Select("where CarType='" + eCarType.�볧ú.ToString() + "' and (select ISFINISH from cmcstbbuyfueltransport where id=transportid)=0 order by CreateDate desc");
			if (frm.ShowDialog() == DialogResult.OK)
			{
				passCarQueuer.Enqueue(eDirection.Way1, frm.Output.TagId);

				this.CurrentFlowFlag = eFlowFlag.ʶ����;

				if (!this.AutoHandMode)
				{
					btnSaveTransport_BuyFuel.Enabled = true;
				}
				this.printTransportId = frm.Output.TransportId;
				this.btnPrintCode_BuyFuel.Enabled = true;
			}
		}

		/// <summary>
		/// �����볧ú�����¼
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSaveTransport_BuyFuel_Click(object sender, EventArgs e)
		{
			if (!SaveBuyFuelTransport()) MessageBoxEx.Show("����ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// ���������¼
		/// </summary>
		/// <returns></returns>
		bool SaveBuyFuelTransport()
		{
			if (this.CurrentBuyFuelTransport == null) return false;

			try
			{
				if (weighterDAO.SaveBuyFuelTransport(this.CurrentBuyFuelTransport.Id, (decimal)Hardwarer.Wber.Weight, DateTime.Now, commonAppConfig.AppIdentifier))
				{
					this.CurrentBuyFuelTransport = commonDAO.SelfDber.Get<CmcsBuyFuelTransport>(this.CurrentBuyFuelTransport.Id);
					if (weighterDAO.CheckBuyFuelTransport(this.CurrentBuyFuelTransport))
					{
						FrontGateUp();

						btnSaveTransport_BuyFuel.Enabled = false;
						this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;

						UpdateLedShow("���سɹ�", "���°�");
						this.voiceSpeaker.Speak("���ڳɹ����°�", 2, false);

						LoadTodayUnFinishBuyFuelTransport();
						LoadTodayFinishBuyFuelTransport();

						CamareCapturePicture(this.CurrentBuyFuelTransport.Id);

						return true;
					}
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
		/// �����볧ú�����¼
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnReset_BuyFuel_Click(object sender, EventArgs e)
		{
			ResetBuyFuel();
		}

		/// <summary>
		/// ��ӡ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnPrintCode_BuyFuel_Click(object sender, EventArgs e)
		{
			if (this.CurrentBuyFuelTransport == null || String.IsNullOrEmpty(this.CurrentBuyFuelTransport.Id)) return;
			FrmWeighter_Print print = new FrmWeighter_Print(this.CurrentBuyFuelTransport.Id);
			print.ShowDialog();
		}

		/// <summary>
		/// ������Ϣ
		/// </summary>
		void ResetBuyFuel()
		{
			this.timer_BuyFuel_Cancel = true;
			this.timer_Goods_Cancel = true;
			this.CurrentFlowFlag = eFlowFlag.�ȴ�����;
			this.CurrentBuyFuelTransport = null;
			this.CurrentAutotruck = null;


			txtTagId_BuyFuel.ResetText();

			btnSaveTransport_BuyFuel.Enabled = !this.AutoHandMode;
			this.PrintTransportId = null;

			System.Threading.Thread.Sleep(1500);

			FrontGateDown();

			BackGateUp();

			UpdateLedShow("  �ȴ�����");
			label_ToolTip.Text = "";
			// �������
			this.CurrentImperfectCar = null;

			this.IsUpGate = false;
			this.IsUpGateing = false;
			this.IsUpRed = false;
			this.IsUpdateLED = false;
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
					case eFlowFlag.��֤��Ϣ:
						#region

						// ���Ҹó�δ��ɵ������¼
						CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, eCarType.�볧ú.ToString());
						if (unFinishTransport != null)
						{
							this.CurrentBuyFuelTransport = commonDAO.SelfDber.Get<CmcsBuyFuelTransport>(unFinishTransport.TransportId);
							if (this.CurrentBuyFuelTransport != null)
							{

								if (this.CurrentBuyFuelTransport.GrossWeight > 0 && (commonDAO.GetCommonAppletConfigString("�Ƿ�PDAȷ��") == "1" && this.CurrentBuyFuelTransport.IsNormal == 0))
								{
									UpdateLedShow(this.CurrentAutotruck.CarNumber, "����жú��");
									label_ToolTip.ForeColor = Color.Red;
									label_ToolTip.Text = "����жú��";
									this.voiceSpeaker.Speak(string.Format("���ƺ�{0}���̲������뷵��жú��"), 2, true);

									FrontGateUp();

									this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
									break;
								}
								this.btnPrintCode_BuyFuel.Enabled = true;
								this.printTransportId = this.CurrentBuyFuelTransport.Id;
								// �ж�·������
								string nextPlace;
								if (carTransportDAO.CheckNextTruckInFactoryWay(this.CurrentAutotruck.CarType, this.CurrentBuyFuelTransport.StepName, "�س�|�ᳵ", commonAppConfig.AppIdentifier, out nextPlace))
								{
									if (this.CurrentBuyFuelTransport.SuttleWeight == 0)
									{
										if (this.CurrentBuyFuelTransport.GrossWeight > 0 || this.CurrentBuyFuelTransport.StepName == "����" || this.CurrentBuyFuelTransport.SamplingType != "��е����")
										{
											this.CurrentFlowFlag = eFlowFlag.ʶ��ť;

											UpdateLedShow(this.CurrentAutotruck.CarNumber, "׼������");

											this.voiceSpeaker.Speak("��Ϩ���³����������ذ�ť", 1, true);
										}
										else
										{
											UpdateLedShow(this.CurrentAutotruck.CarNumber, "δ����");
											label_ToolTip.ForeColor = Color.Red;
											label_ToolTip.Text = "δ����";
											this.voiceSpeaker.Speak("����δ����,����ǰ�����������в���", 2, false);
											timer_BuyFuel.Interval = 20000;
										}
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "�ѳ���");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " �ѳ���", 2, false);

										timer_BuyFuel.Interval = 20000;
									}
								}
								else
								{
									UpdateLedShow("·�ߴ���", "��ֹͨ��");

									this.voiceSpeaker.Speak("·�ߴ��� ��ֹͨ�� " + (!string.IsNullOrEmpty(nextPlace) ? "��ǰ��" + nextPlace : ""), 2, false);

									timer_BuyFuel.Interval = 20000;

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
							UpdateLedShow(this.CurrentAutotruck.CarNumber, "δ�Ŷ�");
							this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " δ�ҵ��ŶӼ�¼", 2, false);

							timer_BuyFuel.Interval = 20000;
						}

						#endregion
						break;

					case eFlowFlag.ʶ��ť:
						#region

						// ���������
						timer_BuyFuel.Interval = 500;

						if (ButtonSensor)
						{
							this.CurrentFlowFlag = eFlowFlag.�ȴ��ȶ�;

							this.voiceSpeaker.Speak("���ڳ��ڣ���ȴ����ɿ���ť", 1, true);

							UpdateLedShow("���ɿ���ť", "�ȴ�����");
							CamareCapturePicture(this.CurrentBuyFuelTransport.Id);
						}

						#endregion
						break;

					case eFlowFlag.�ȴ��ȶ�:
						#region

						// ���������
						timer_BuyFuel.Interval = 1000;

						btnSaveTransport_BuyFuel.Enabled = this.WbSteady;

						UpdateLedShow(this.CurrentAutotruck.CarNumber, Hardwarer.Wber.Weight.ToString("#0.######"));
						// ���ذ��Ǳ�����������С��������������ĵظ����������źţ����ж����Ѿ���ȫ�ϰ�
						if (Hardwarer.Wber.Weight >= this.WbMinWeight && !HasCarOnEnterWay())
						{
							BackGateDown();
						}

						if (this.WbSteady)
						{
							if (this.AutoHandMode)
							{
								// �Զ�ģʽ
								if (!SaveBuyFuelTransport())
								{
									//UpdateLedShow("����ʧ��", "�ٴγ���");

									//this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ����ʧ�ܣ����ڽ��ж��γ���", 1, false);
								}
								else
								{
									this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;
								}
							}
							else
							{
								// �ֶ�ģʽ 
							}
						}

						#endregion
						break;

					case eFlowFlag.�ȴ��뿪:
						#region

						try
						{
							//����жú��LED
							if (!this.IsUpdateLED)
							{
								if (this.CurrentBuyFuelTransport != null && this.CurrentBuyFuelTransport.SuttleWeight == 0)
								{
									string ledName = UnLoadLEDDAO.GetInstance().SendUnLoadLED(this.CurrentBuyFuelTransport.CarNumber, true);
									if (!string.IsNullOrEmpty(ledName))
									{
										UpdateLedShow(string.Format("��ǰ��{0}", ledName), "����жú");
										this.voiceSpeaker.Speak(string.Format("��ǰ��{0}жú������жú", ledName), 1, false);
									}
								}
								else if (this.CurrentBuyFuelTransport != null && this.CurrentBuyFuelTransport.SuttleWeight > 0)
									UnLoadLEDDAO.GetInstance().SendUnLoadLED(this.CurrentBuyFuelTransport.CarNumber, false);
								this.IsUpdateLED = true;
							}
						}
						catch { }

						// ��ǰ�ذ�����С����С���������еظС��������ź�ʱ����
						if (Hardwarer.Wber.Weight <= this.WbMinWeight && !HasCarOnLeaveWay()) ResetBuyFuel();

						// ����������
						timer_BuyFuel.Interval = 4000;

						#endregion
						break;
				}

				// ��ǰ�ذ�����С����С���������еظС��������ź�ʱ����
				if (Hardwarer.Wber.Weight < this.WbMinWeight && !HasCarOnEnterWay() && !HasCarOnLeaveWay() && this.CurrentFlowFlag != eFlowFlag.�ȴ�����
					&& this.CurrentImperfectCar != null)
					ResetBuyFuel();
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
			superGridControl1_BuyFuel.PrimaryGrid.DataSource = weighterDAO.GetUnFinishBuyFuelTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
		}

		/// <summary>
		/// ��ȡָ����������ɵ��볧ú��¼
		/// </summary>
		void LoadTodayFinishBuyFuelTransport()
		{
			superGridControl2_BuyFuel.PrimaryGrid.DataSource = weighterDAO.GetFinishedBuyFuelTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
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


		#endregion

		#region ��������ҵ��

		bool timer_Goods_Cancel = true;

		CmcsGoodsTransport currentGoodsTransport;
		/// <summary>
		/// ��ǰ�����¼
		/// </summary>
		public CmcsGoodsTransport CurrentGoodsTransport
		{
			get { return currentGoodsTransport; }
			set
			{
				currentGoodsTransport = value;

				if (value != null)
				{
					commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), value.Id);

					txtSupplyUnitName_Goods.Text = value.SupplyUnitName;
					txtReceiveUnitName_Goods.Text = value.ReceiveUnitName;
					txtGoodsTypeName_Goods.Text = value.GoodsTypeName;

					txtFirstWeight_Goods.Text = value.FirstWeight.ToString("F2");
					txtSecondWeight_Goods.Text = value.SecondWeight.ToString("F2");
					txtSuttleWeight_Goods.Text = value.SuttleWeight.ToString("F2");
				}
				else
				{
					txtSupplyUnitName_Goods.Text = string.Empty;
					txtReceiveUnitName_Goods.Text = string.Empty;
					txtGoodsTypeName_Goods.Text = string.Empty;

					txtFirstWeight_Goods.Text = string.Empty;
					txtSecondWeight_Goods.Text = string.Empty;
					txtSuttleWeight_Goods.Text = string.Empty;

					//txtSupplyUnitName_Goods.ResetText();
					//txtReceiveUnitName_Goods.ResetText();
					//txtGoodsTypeName_Goods.ResetText();

					//txtFirstWeight_Goods.ResetText();
					//txtSecondWeight_Goods.ResetText();
					//txtSuttleWeight_Goods.ResetText();
					commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, eSignalDataName.��ǰ�����¼Id.ToString(), string.Empty);
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
			FrmUnFinishTransport_Select frm = new FrmUnFinishTransport_Select("where CarType='" + eCarType.��������.ToString() + "' order by CreateDate desc");
			if (frm.ShowDialog() == DialogResult.OK)
			{
				passCarQueuer.Enqueue(eDirection.Way1, frm.Output.TagId);

				this.CurrentFlowFlag = eFlowFlag.ʶ����;
			}
		}

		/// <summary>
		/// �����ŶӼ�¼
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSaveTransport_Goods_Click(object sender, EventArgs e)
		{
			if (!SaveGoodsTransport()) MessageBoxEx.Show("����ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// ���������¼
		/// </summary>
		/// <returns></returns>
		bool SaveGoodsTransport()
		{
			if (this.CurrentGoodsTransport == null) return false;

			try
			{
				if (weighterDAO.SaveGoodsTransport(this.CurrentGoodsTransport.Id, (decimal)Hardwarer.Wber.Weight, DateTime.Now, commonAppConfig.AppIdentifier))
				{
					this.CurrentGoodsTransport = commonDAO.SelfDber.Get<CmcsGoodsTransport>(this.CurrentGoodsTransport.Id);
					if (weighterDAO.CheckGoodsTransport(this.CurrentGoodsTransport))
					{
						FrontGateUp();

						btnSaveTransport_Goods.Enabled = false;
						this.CurrentFlowFlag = eFlowFlag.�ȴ��뿪;

						UpdateLedShow("���سɹ�", "���°�");
						this.voiceSpeaker.Speak("���ڳɹ����°�", 2, false);

						LoadTodayUnFinishGoodsTransport();
						LoadTodayFinishGoodsTransport();

						//CamareCapturePicture(this.CurrentGoodsTransport.Id);

						return true;
					}
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
			this.CurrentGoodsTransport = null;

			txtTagId_Goods.ResetText();

			btnSaveTransport_Goods.Enabled = false;

			FrontGateDown();

			BackGateUp();

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
					case eFlowFlag.��֤��Ϣ:
						#region

						// ���Ҹó�δ��ɵ������¼
						CmcsUnFinishTransport unFinishTransport = carTransportDAO.GetUnFinishTransportByAutotruckId(this.CurrentAutotruck.Id, eCarType.��������.ToString());
						if (unFinishTransport != null)
						{
							this.CurrentGoodsTransport = commonDAO.SelfDber.Get<CmcsGoodsTransport>(unFinishTransport.TransportId);
							if (this.CurrentGoodsTransport != null)
							{
								// �ж�·������
								string nextPlace;
								if (carTransportDAO.CheckNextTruckInFactoryWay(this.CurrentAutotruck.CarType, this.CurrentGoodsTransport.StepName, "��һ�γ���|�ڶ��γ���", commonAppConfig.AppIdentifier, out nextPlace))
								{
									if (this.CurrentGoodsTransport.SuttleWeight == 0)
									{
										this.CurrentFlowFlag = eFlowFlag.ʶ��ť;

										UpdateLedShow(this.CurrentAutotruck.CarNumber, "׼������");

										this.voiceSpeaker.Speak("��Ϩ���³����������ڰ�ť", 1, true);
									}
									else
									{
										UpdateLedShow(this.CurrentAutotruck.CarNumber, "�ѳ���");
										this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " �ѳ���", 2, false);

										timer_Goods.Interval = 20000;
									}
								}
								else
								{
									UpdateLedShow("·�ߴ���", "��ֹͨ��");

									this.voiceSpeaker.Speak("·�ߴ��� ��ֹͨ�� " + (!string.IsNullOrEmpty(nextPlace) ? "��ǰ��" + nextPlace : ""), 2, false);

									timer_Goods.Interval = 20000;

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
							UpdateLedShow(this.CurrentAutotruck.CarNumber, "δ�Ŷ�");
							this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " δ�ҵ��ŶӼ�¼", 2, false);

							timer_Goods.Interval = 20000;

						}

						#endregion
						break;

					case eFlowFlag.ʶ��ť:
						#region

						// ���������
						timer_Goods.Interval = 500;

						if (ButtonSensor)
						{
							this.voiceSpeaker.Speak("���ڳ��ڣ���ȴ����ɿ���ť", 2, true);

							UpdateLedShow("���ɿ���ť", "�ȴ�����");

							this.CurrentFlowFlag = eFlowFlag.�ȴ��ȶ�;
						}
						#endregion
						break;

					case eFlowFlag.�ȴ��ȶ�:
						#region

						// ���������
						timer_Goods.Interval = 1000;

						btnSaveTransport_Goods.Enabled = this.WbSteady;

						UpdateLedShow(this.CurrentAutotruck.CarNumber, Hardwarer.Wber.Weight.ToString("#0.######"));

						if (this.WbSteady)
						{
							if (this.AutoHandMode)
							{
								// �Զ�ģʽ
								if (!SaveGoodsTransport())
								{
									//UpdateLedShow("����ʧ��", "�ٴγ���");

									//this.voiceSpeaker.Speak("���ƺ� " + this.CurrentAutotruck.CarNumber + " ����ʧ�ܣ����ڽ��ж��γ���", 1, false);
								}
							}
							else
							{
								// �ֶ�ģʽ 
							}
						}

						#endregion
						break;

					case eFlowFlag.�ȴ��뿪:
						#region

						// ��ǰ�ذ�����С����С���������еظС��������ź�ʱ����
						if (Hardwarer.Wber.Weight <= this.WbMinWeight && !HasCarOnLeaveWay())
						{
							btnReset_Goods_Click(null, null);
						}
						// ����������
						timer_Goods.Interval = 4000;

						#endregion
						break;
				}

				// ��ǰ�ذ�����С����С���������еظС��������ź�ʱ����
				if (Hardwarer.Wber.Weight < this.WbMinWeight && !HasCarOnEnterWay() && !HasCarOnLeaveWay() && this.CurrentFlowFlag != eFlowFlag.�ȴ�����
					&& this.CurrentImperfectCar != null) btnReset_Goods_Click(null, null);
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
			superGridControl1_Goods.PrimaryGrid.DataSource = weighterDAO.GetUnFinishGoodsTransport();
		}

		/// <summary>
		/// ��ȡָ����������ɵ��������ʼ�¼
		/// </summary>
		void LoadTodayFinishGoodsTransport()
		{
			superGridControl2_Goods.PrimaryGrid.DataSource = weighterDAO.GetFinishedGoodsTransport(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
		}

		#endregion

		#region ��������

		Font directionFont = new Font("΢���ź�", 16);

		Pen redPen1 = new Pen(Color.Red, 1);
		Pen greenPen1 = new Pen(Color.Lime, 1);
		Pen redPen3 = new Pen(Color.Red, 3);
		Pen greenPen3 = new Pen(Color.Lime, 3);

		/// <summary>
		/// ��ǰ�Ǳ�����������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panCurrentWeight_Paint(object sender, PaintEventArgs e)
		{
			PanelEx panel = sender as PanelEx;

			int height = 12;

			// ���Ƶظ�1
			e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, 1, 15, height);
			e.Graphics.DrawLine(this.InductorCoil1 ? redPen3 : greenPen3, 15, panel.Height - height, 15, panel.Height - 1);
			// ���Ƶظ�2
			e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, 25, 1, 25, height);
			e.Graphics.DrawLine(this.InductorCoil2 ? redPen3 : greenPen3, 25, panel.Height - height, 25, panel.Height - 1);
			// ���ƶ���1
			e.Graphics.DrawLine(this.InfraredSensor1 ? redPen1 : greenPen1, 35, 1, 35, height);
			e.Graphics.DrawLine(this.InfraredSensor1 ? redPen1 : greenPen1, 35, panel.Height - height, 35, panel.Height - 1);

			// ���ƶ���2
			e.Graphics.DrawLine(this.InfraredSensor2 ? redPen1 : greenPen1, panel.Width - 35, 1, panel.Width - 35, height);
			e.Graphics.DrawLine(this.InfraredSensor2 ? redPen1 : greenPen1, panel.Width - 35, panel.Height - height, panel.Width - 35, panel.Height - 1);
			// ���Ƶظ�3
			e.Graphics.DrawLine(this.InductorCoil3 ? redPen3 : greenPen3, panel.Width - 25, 1, panel.Width - 25, height);
			e.Graphics.DrawLine(this.InductorCoil3 ? redPen3 : greenPen3, panel.Width - 25, panel.Height - height, panel.Width - 25, panel.Height - 1);
			// ���Ƶظ�4
			e.Graphics.DrawLine(this.InductorCoil4 ? redPen3 : greenPen3, panel.Width - 15, 1, panel.Width - 15, height);
			e.Graphics.DrawLine(this.InductorCoil4 ? redPen3 : greenPen3, panel.Width - 15, panel.Height - height, panel.Width - 15, panel.Height - 1);

			// �ϰ�����
			eDirection direction = eDirection.UnKnow;
			if (this.CurrentImperfectCar != null) direction = this.CurrentImperfectCar.PassWay;
			e.Graphics.DrawString("��>", directionFont, direction == eDirection.Way1 ? Brushes.Red : Brushes.Lime, 2, 17);
			e.Graphics.DrawString("<��", directionFont, direction == eDirection.Way2 ? Brushes.Red : Brushes.Lime, panel.Width - 47, 17);
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

		/// <summary>
		/// ��ӡ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void stripPrintCode_Click(object sender, EventArgs e)
		{
			GridRow gridRow = superGridControl1_BuyFuel.PrimaryGrid.ActiveRow as GridRow;
			if (gridRow == null) return;
			View_BuyFuelTransport entity = gridRow.DataItem as View_BuyFuelTransport;
			FrmWeighter_Print print = new FrmWeighter_Print(entity.Id);
			print.ShowDialog();
		}
		//��־����ź�
		private string getLog(string type)
		{
			string str = type + ":" + "�ظ�1�ź�:" + InductorCoil1 + ",�ظ�2�ź�:" + InductorCoil2 + ",�����ź�1:" + InfraredSensor1 + ",��ǰ����:" + Hardwarer.Wber.Weight;
			return str;
		}
	}
}
