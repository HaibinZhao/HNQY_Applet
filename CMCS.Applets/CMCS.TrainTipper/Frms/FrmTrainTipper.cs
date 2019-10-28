using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities.AutoMaker;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.BeltSampler;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Entities.Inf;
using CMCS.Common.Entities.TrainInFactory;
using CMCS.Common.Enums;
using CMCS.Common.Utilities;
using CMCS.Common.Views;
using CMCS.TrainTipper.DAO;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.SuperGrid;

namespace CMCS.TrainTipper.Frms
{
    public partial class FrmTrainTipper : DevComponents.DotNetBar.Metro.MetroForm
    {
        CommonDAO commonDAO = CommonDAO.GetInstance();
        TrainTipperDAO trainTipperDAO = TrainTipperDAO.GetInstance();
        BeltSamplerDAO beltSamplerDAO = BeltSamplerDAO.GetInstance();
        CarriageRecognitionerDAO carriageRecognitionerDAO = CarriageRecognitionerDAO.GetInstance();

        /// <summary>
        /// ������
        /// </summary>
        CmcsCMEquipment trainTipper;
        /// <summary>
        /// ����ʶ���豸����
        /// </summary>
        string carriageRecognitionerMachineCode;
        /// <summary>
        /// Ƥ��������
        /// </summary>
        CmcsCMEquipment trainBeltSampler;
        /// <summary>
        /// ���һ�μ�¼
        /// </summary>
        View_TrainTipperQueue lastView_TrainTipperQueue;

        CmcsTrainCarriagePass currentTrainCarriagePass;
        /// <summary>
        /// ��ǰ����ʶ���¼
        /// </summary>
        public CmcsTrainCarriagePass CurrentTrainCarriagePass
        {
            get { return currentTrainCarriagePass; }
            set
            {
                currentTrainCarriagePass = value;

                this.InvokeEx(() =>
                {
                    lblCurrentTrainNumber.Text = currentTrainCarriagePass.TrainNumber;
                });
            }
        }

        Color[] CellColors = new Color[] { ColorTranslator.FromHtml("#7D00FFFF"), ColorTranslator.FromHtml("#7DFFFF00"), ColorTranslator.FromHtml("#7D7CFC00"), ColorTranslator.FromHtml("#7DFF69B4"), ColorTranslator.FromHtml("#7DFF00FF"), ColorTranslator.FromHtml("#7DADD8E6"), ColorTranslator.FromHtml("#7D00FF00"), ColorTranslator.FromHtml("#7DFFC0CB") };
        /// <summary>
        /// �������ɫ
        /// </summary>
        Dictionary<string, Color> dicCellColors = new Dictionary<string, Color>();

        List<View_TrainTipperQueue> view_TrainTipperQueue_All;
        List<View_TrainTipperQueue> view_TrainTipperQueue_YF;
        List<View_TrainTipperQueue> view_TrainTipperQueue_DF;

        /// <summary>
        /// FrmTrainTipper
        /// </summary>
        /// <param name="trainTipper">�������豸����</param>
        /// <param name="carriageRecognitionerMachineCode">����ʶ���豸����</param> 
        public FrmTrainTipper(CmcsCMEquipment trainTipper, string carriageRecognitionerMachineCode)
        {
            InitializeComponent();

            this.trainTipper = trainTipper;
            this.carriageRecognitionerMachineCode = carriageRecognitionerMachineCode;
        }

        private void FrmTrainTipper_Load(object sender, EventArgs e)
        {
            InitForm();
        }

        /// <summary>
        /// �����ʼ��
        /// </summary>
        private void InitForm()
        {
            // Ĭ�ϵ���
            dtiptStartArriveTime.Value = DateTime.Now.Date;
            dtiptEndArriveTime.Value = DateTime.Now.Date.AddDays(1);

            // ��δ����ĳ���ʶ��ͨ����¼���Ϊ�Ѵ���
            carriageRecognitionerDAO.ChangeTrainCarriagePassToHandledByMachineCode(this.carriageRecognitionerMachineCode);

            timer2_Tick(null, null);
        }

        #region ��ʱ��

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            try
            {

            }
            catch (Exception ex) { Log4Neter.Error("��ʱ����timer_Tick", ex); }

            timer1.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();

            UpdateLinkBeltSamplerStatus();

            timer2.Start();
        }

        #endregion

        #region �߼�����

        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

        System.Threading.AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// ����ҵ���߼����
        /// </summary>
        private void CreateMainTask()
        {
            taskSimpleScheduler = new TaskSimpleScheduler();

            autoResetEvent.Reset();

            taskSimpleScheduler.StartNewTask("����ҵ���߼�", () =>
            {
                // ��ȡ������ĳ���ʶ���¼ - �볧����
                CmcsTrainCarriagePass inTrainCarriagePass = carriageRecognitionerDAO.GetUnHandleTrainCarriagePass(this.carriageRecognitionerMachineCode, eTrainPassDirection.����);
                if (inTrainCarriagePass != null)
                {
                    this.CurrentTrainCarriagePass = inTrainCarriagePass;

                    // ��������ϵͳ��״̬
                    string samplerSystemStatue = commonDAO.GetSignalDataValue(this.trainBeltSampler.EquipmentCode, eSignalDataName.ϵͳ.ToString());
                    if (samplerSystemStatue == eEquInfSamplerSystemStatus.��������.ToString() || samplerSystemStatue == eEquInfSamplerSystemStatus.����ж��.ToString())
                    {
                        MessageBoxEx2Show("��ֹ������ " + this.trainBeltSampler.EquipmentName + "�������ϻ�����ж��������ͣ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        btnStartTurnover.Enabled = true;

                        // ȡ������
                        this.taskSimpleScheduler.Cancal();

                        return;
                    }

                    StartTippingTask(this.CurrentTrainCarriagePass);

                    autoResetEvent.WaitOne();
                }
            }, 4000);
        }

        /// <summary>
        /// ���ճ���ʶ�����ݣ���ʼ�����߼�����
        /// </summary>
        /// <param name="inTrainCarriagePass"></param>
        private void StartTippingTask(CmcsTrainCarriagePass trainCarriagePass)
        {
            Task task = new Task((state) =>
            {
                CmcsTrainCarriagePass inTrainCarriagePass = state as CmcsTrainCarriagePass;
                if (inTrainCarriagePass != null)
                {
                    #region ����Ϊ��ʱ��ִ�г��Ų�¼

                    if (string.IsNullOrEmpty(inTrainCarriagePass.TrainNumber))
                    {
                        Log4Neter.Info(this.trainTipper.EquipmentName + " - ����ʶ��ʧ�ܣ�Ҫ�����복��");

                        this.InvokeEx(() => { Form1.superTabControlManager.ChangeToTab(this.trainTipper.EquipmentCode); });

                        // ���������Ҫ�����복���
                        FrmInput frmInput = new FrmInput("���������������", (input) =>
                        {
                            if (string.IsNullOrEmpty(input))
                            {
                                MessageBoxEx.Show("�����복���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            if (!this.view_TrainTipperQueue_DF.Any(a => a.TrainNumber == input))
                            {
                                MessageBoxEx.Show("�ڶ�����δ�ҵ��˳�������������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            return true;
                        });

                        if (frmInput.ShowDialog() == DialogResult.OK)
                        {
                            Log4Neter.Info(this.trainTipper.EquipmentName + " - �û����룺" + frmInput.Input);
                            inTrainCarriagePass.TrainNumber = frmInput.Input;
                        }
                        else
                        {
                            Log4Neter.Info(this.trainTipper.EquipmentName + " - �û��ر����봰��");

                            inTrainCarriagePass.DataFlag = 1;
                            Dbers.GetInstance().SelfDber.Update(inTrainCarriagePass);

                            autoResetEvent.Set();
                        };
                    }

                    #endregion

                    Log4Neter.Info(this.trainTipper.EquipmentName + " - ��ǰ���ţ�" + inTrainCarriagePass.TrainNumber);
                    commonDAO.SetSignalDataValue(this.trainTipper.EquipmentCode, eSignalDataName.��ǰ����.ToString(), inTrainCarriagePass.TrainNumber);

                    View_TrainTipperQueue selfView_TrainTipperQueue = this.view_TrainTipperQueue_All.FirstOrDefault(a => a.TrainNumber == inTrainCarriagePass.TrainNumber);
                    if (selfView_TrainTipperQueue != null)
                    {
                        commonDAO.SetSignalDataValue(this.trainTipper.EquipmentCode, eSignalDataName.��ǰ��Id.ToString(), selfView_TrainTipperQueue.TransportId);

                        if (selfView_TrainTipperQueue.SampleType != eSamplingType.Ƥ������.ToString())
                        {
                            // ��������������Ϊ�ǻ�Ƥ��

                            Log4Neter.Info(this.trainTipper.EquipmentName + " - ��������������Ϊ��Ƥ������,SampleType=" + selfView_TrainTipperQueue.SampleType);

                            DialogResult dialogResult = MessageBoxEx2Show("<font size='+2'>����: <font color='red'>" + selfView_TrainTipperQueue.TrainNumber + "</font><br/><br/>�ó�Ϊ" + selfView_TrainTipperQueue.SampleType + "<br/><br/>���<font color='red'>[��]</font>����֪ͨƤ��������ֹͣ����<br/>Ȼ���ڲ������ɹ�ֹͣ��ʼ����<br/><br/>���<font color='red'>[��]</font>ֱ�ӿ�ʼ����<br/><br/>���<font color='red'>[ȡ��]</font>�����κδ���</font>", "��ʾ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                            if (dialogResult == DialogResult.Yes)
                            {
                                if (SendSamplerStopCmd(selfView_TrainTipperQueue))
                                    // ���Ϊ�Ѵ��� 
                                    ToHandled(inTrainCarriagePass.Id, selfView_TrainTipperQueue.Id);
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                // ���Ϊ�Ѵ��� 
                                ToHandled(inTrainCarriagePass.Id, selfView_TrainTipperQueue.Id);
                            }
                        }
                        else
                        {
                            if (MessageBoxEx2Show("<font size='+2'>����: <font color='red'>" + selfView_TrainTipperQueue.TrainNumber + "</font><br/><br/>���<font color='red'>[ȷ��]</font>����֪ͨƤ����������ʼ����<br/>ȷ�ϲ����������ɹ����ٿ�ʼ����<br/><br/>���<font color='red'>[ȡ��]</font>�����κδ���</font>", "��ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                            {
                                if (SendSamplerStartCmd(selfView_TrainTipperQueue))
                                    // ���Ϊ�Ѵ��� 
                                    ToHandled(inTrainCarriagePass.Id, selfView_TrainTipperQueue.Id);
                            }
                        }
                    }
                    else
                    {
                        // δ�ҵ��˳�

                        commonDAO.SetSignalDataValue(this.trainTipper.EquipmentCode, eSignalDataName.��ǰ��Id.ToString(), string.Empty);

                        if (MessageBoxEx2Show("δ�ҵ�����[" + inTrainCarriagePass.TrainNumber + "]���Ƿ���ԣ�", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            // ��ǳ���ʶ���¼Ϊ�Ѵ���
                            carriageRecognitionerDAO.ChangeTrainCarriagePassToHandled(inTrainCarriagePass.Id);
                    }
                }

                autoResetEvent.Set();

            }, trainCarriagePass);
            task.Start();
        }

        #endregion

        #region ��������

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
        /// MessageBoxEx��װ�������Ի���֮ǰ�л���ѡ�
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        private DialogResult MessageBoxEx2Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Form1.superTabControlManager.ChangeToTab(this.trainTipper.EquipmentCode);
            return MessageBoxEx.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// ����ToolTip��ʾ
        /// </summary>
        private void SetSystemStatusToolTip(Control control)
        {
            this.toolTip1.SetToolTip(control, "<��ɫ> ��������\r\n<��ɫ> ��������\r\n<��ɫ> ��������");
        }

        /// <summary>
        /// ���¶�Ӧ��Ƥ��������ϵͳ״̬
        /// </summary>
        private void UpdateLinkBeltSamplerStatus()
        {
            this.trainBeltSampler = TrainInFactoryDAO.GetInstance().GetTrainTipperLinkBeltSampler(this.trainTipper.EquipmentCode);
            if (trainBeltSampler != null)
            {
                lblTrainBeltSampler.Text = this.trainBeltSampler.EquipmentName;

                string systemStatus = commonDAO.GetSignalDataValue(this.trainBeltSampler.EquipmentCode, eSignalDataName.ϵͳ.ToString());
                if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString())
                    uCtrlSignalLight_TrainBeltSampler.LightColor = EquipmentStatusColors.BeReady;
                else if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString() || systemStatus == eEquInfSamplerSystemStatus.����ж��.ToString())
                    uCtrlSignalLight_TrainBeltSampler.LightColor = EquipmentStatusColors.Working;
                else if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString())
                    uCtrlSignalLight_TrainBeltSampler.LightColor = EquipmentStatusColors.Breakdown;

                // ��⼯�����Ƿ����
                int barrelCount = commonDAO.SelfDber.Count<InfEquInfSampleBarrel>("where MachineCode=:MachineCode and BarrelStatus!=:BarrelStatus and BarrelType=:BarrelType", new { MachineCode = this.trainBeltSampler.EquipmentCode, BarrelStatus = eSampleBarrelStatus.����.ToString(), BarrelType = "��жʽ" });
                lblBarrelStatus.Text = barrelCount >= 1 ? "(��Ͱ����)" : "(��Ͱ����)";
                lblBarrelStatus.Location = new Point(lblTrainBeltSampler.Location.X + lblTrainBeltSampler.Size.Width, lblBarrelStatus.Location.Y);
            }
            else
            {
                lblTrainBeltSampler.Text = "δ����";

                uCtrlSignalLight_TrainBeltSampler.LightColor = EquipmentStatusColors.Forbidden;
            }
        }

        /// <summary>
        /// �����ǰ������Ϣ
        /// </summary>
        /// <param name="output"></param>
        private void OutputRunInfo(string output)
        {
            this.InvokeEx(() =>
            {
                lblRunInfo.Text = "> " + output;

                Log4Neter.Info(this.trainTipper.EquipmentName + " - ���������" + output);
            });
        }

        /// <summary>
        /// ���Ľ���ؼ��Ŀ�������
        /// </summary>
        /// <param name="enabled"></param>
        private void ChangeUIEnabled(bool enabled)
        {
            this.InvokeEx(() =>
            {
                btnAddTrainCarriagePass.Enabled = enabled;
                btnStopTurnover.Enabled = enabled;
            });
        }

        /// <summary>
        /// ���Ϳ�ʼ��������
        /// </summary>
        /// <param name="view_TrainTipperQueue"></param>
        /// <returns></returns>
        private bool SendSamplerStartCmd(View_TrainTipperQueue view_TrainTipperQueue)
        {
            this.lastView_TrainTipperQueue = view_TrainTipperQueue;

            bool res = false;

            // ��������ϵͳ��״̬
            string samplerSystemStatue = commonDAO.GetSignalDataValue(this.trainBeltSampler.EquipmentCode, eSignalDataName.ϵͳ.ToString());
            if (samplerSystemStatue == eEquInfSamplerSystemStatus.��������.ToString() || samplerSystemStatue == eEquInfSamplerSystemStatus.��������.ToString())
            {
                Log4Neter.Info(this.trainTipper.EquipmentName + " - ��[" + this.trainBeltSampler.EquipmentCode + "]����[��ʼ����]��������룺" + view_TrainTipperQueue.YuSampleCode);

                // ���Ͳ����ƻ�
                if (SendSamplePlan(view_TrainTipperQueue))
                {
                    string cmdId;
                    bool sendSuccess = beltSamplerDAO.SendSampleCmd(this.trainBeltSampler.EquipmentCode, eEquInfSamplerCmd.��ʼ����, view_TrainTipperQueue.YuSampleCode, out cmdId);

                    ChangeUIEnabled(false);
                    OutputRunInfo("�ȴ�Ƥ����������������(��ʼ����)ִ�н��");

                    eEquInfCmdResultCode equInfCmdResultCode;
                    do
                    {
                        Thread.Sleep(10000);

                        equInfCmdResultCode = beltSamplerDAO.GetSampleCmdResult(cmdId);
                    }
                    while (sendSuccess && equInfCmdResultCode == eEquInfCmdResultCode.Ĭ��);

                    res = equInfCmdResultCode == eEquInfCmdResultCode.�ɹ�;

                    ChangeUIEnabled(true);
                    OutputRunInfo("Ƥ��������ִ������(��ʼ����)" + (res ? "�ɹ�" : "ʧ��"));
                }
            }
            else
            {
                Log4Neter.Info(this.trainTipper.EquipmentName + " - " + this.trainBeltSampler.EquipmentCode + "δ����" + samplerSystemStatue + "״̬����ֹ���Ϳ�ʼ��������");
            }

            return res;
        }

        /// <summary>
        /// ���ͽ�����������
        /// </summary>
        /// <param name="view_TrainTipperQueue"></param> 
        private bool SendSamplerStopCmd(View_TrainTipperQueue view_TrainTipperQueue)
        {
            this.lastView_TrainTipperQueue = null;

            bool res = false;

            // ��������ϵͳ��״̬
            string samplerSystemStatue = commonDAO.GetSignalDataValue(this.trainBeltSampler.EquipmentCode, eSignalDataName.ϵͳ.ToString());
            if (samplerSystemStatue == eEquInfSamplerSystemStatus.��������.ToString())
            {
                string sampleCode = view_TrainTipperQueue != null ? view_TrainTipperQueue.YuSampleCode : string.Empty;

                Log4Neter.Info(this.trainTipper.EquipmentName + " - ��[" + this.trainBeltSampler.EquipmentCode + "]����[��������]��������룺" + sampleCode);

                string cmdId;
                bool sendSuccess = beltSamplerDAO.SendSampleCmd(this.trainBeltSampler.EquipmentCode, eEquInfSamplerCmd.��������, sampleCode, out cmdId);

                ChangeUIEnabled(false);
                OutputRunInfo("�ȴ�Ƥ����������������(��������)ִ�н��");

                eEquInfCmdResultCode equInfCmdResultCode;
                do
                {
                    Thread.Sleep(10000);

                    equInfCmdResultCode = beltSamplerDAO.GetSampleCmdResult(cmdId);
                }
                while (sendSuccess && equInfCmdResultCode == eEquInfCmdResultCode.Ĭ��);

                res = equInfCmdResultCode == eEquInfCmdResultCode.�ɹ�;

                ChangeUIEnabled(true);
                OutputRunInfo("Ƥ��������ִ������(��������)" + (res ? "�ɹ�" : "ʧ��"));

                return res;
            }
            else
            {
                OutputRunInfo(this.trainBeltSampler.EquipmentCode + "δ������������״̬�����뷢�ͽ�����������");
                Log4Neter.Info(this.trainTipper.EquipmentName + " - " + this.trainBeltSampler.EquipmentCode + "δ������������״̬�����뷢�ͽ�����������");

                return true;
            }
        }

        /// <summary>
        /// ���Ͳ����ƻ��������������
        /// </summary>
        /// <param name="view_TrainTipperQueue"></param>  
        /// <returns></returns>
        public bool SendSamplePlan(View_TrainTipperQueue view_TrainTipperQueue)
        {
            CmcsInFactoryBatch inFactoryBatch = Dbers.GetInstance().SelfDber.Get<CmcsInFactoryBatch>(view_TrainTipperQueue.InFactoryBatchId);
            if (inFactoryBatch == null) throw new ArgumentNullException("inFactoryBatchId", "δ�ҵ�Id=" + view_TrainTipperQueue.InFactoryBatchId + "������");

            CmcsFuelKind fuelKind = Dbers.GetInstance().SelfDber.Get<CmcsFuelKind>(inFactoryBatch.FuelKindId);
            List<View_TrainTipperQueue> list = trainTipperDAO.GetView_TrainTipperQueueBy(view_TrainTipperQueue.TrainSampleSchemeId);

            InfBeltSamplePlan oldBeltSamplePlan = Dbers.GetInstance().SelfDber.Entity<InfBeltSamplePlan>("where InFactoryBatchId=:InFactoryBatchId and SampleCode=:SampleCode", new { InFactoryBatchId = view_TrainTipperQueue.InFactoryBatchId, SampleCode = view_TrainTipperQueue.YuSampleCode });
            if (oldBeltSamplePlan == null)
            {
                Log4Neter.Info(this.trainTipper.EquipmentName + " - ��[" + this.trainBeltSampler.EquipmentCode + "]���Ͳ����ƻ��������룺" + view_TrainTipperQueue.YuSampleCode);

                return Dbers.GetInstance().SelfDber.Insert<InfBeltSamplePlan>(new InfBeltSamplePlan
                {
                    DataFlag = 0,
                    InterfaceType = this.trainBeltSampler.InterfaceType,
                    InFactoryBatchId = view_TrainTipperQueue.InFactoryBatchId,
                    SampleCode = view_TrainTipperQueue.YuSampleCode,
                    FuelKindName = fuelKind != null ? fuelKind.FuelName : string.Empty,
                    CarCount = list.Count,
                    Mt = 0,
                    TicketWeight = list.Sum(a => a.TicketWeight),
                    SampleType = eEquInfSampleType.��������.ToString(),
                    GatherType = commonDAO.GetCommonAppletConfigString(this.trainBeltSampler.EquipmentCode + "������ʽ")
                }) > 0;
            }
            else
            {
                Log4Neter.Info(this.trainTipper.EquipmentName + " - ��[" + this.trainBeltSampler.EquipmentCode + "]���²����ƻ��������룺" + view_TrainTipperQueue.YuSampleCode);

                oldBeltSamplePlan.DataFlag = 0;
                oldBeltSamplePlan.FuelKindName = fuelKind != null ? fuelKind.FuelName : string.Empty;
                oldBeltSamplePlan.CarCount = list.Count;
                oldBeltSamplePlan.Mt = 0;
                oldBeltSamplePlan.TicketWeight = list.Sum(a => a.TicketWeight);
                oldBeltSamplePlan.SampleType = eEquInfSampleType.��������.ToString();
                oldBeltSamplePlan.GatherType = commonDAO.GetCommonAppletConfigString(this.trainBeltSampler.EquipmentCode + "������ʽ");

                return Dbers.GetInstance().SelfDber.Update(oldBeltSamplePlan) > 0;
            }
        }

        /// <summary>
        /// ������ʶ��ͶԵ���¼����Ϊ�Ѵ����ѷ�����ˢ�½����б�
        /// </summary>
        /// <param name="inTrainCarriagePass">����ʶ��Id</param>
        /// <param name="trainWeightRecordId">���볧��¼Id</param>
        private void ToHandled(string inTrainCarriagePass, string trainWeightRecordId)
        {
            // ��ǳ���ʶ���¼Ϊ�Ѵ���
            carriageRecognitionerDAO.ChangeTrainCarriagePassToHandled(inTrainCarriagePass);
            // ��ǶԵ����м�¼Ϊ�ѷ�
            TrainTipperDAO.GetInstance().ChangeTrainWeightRecordIsTurnover(trainWeightRecordId, eTrainTipperTurnoverStatus.�ѷ�);
            // ���ط������Ե�����������ͼ
            LoadView_TrainTipperQueue();
        }

        /// <summary>
        /// ���ط������Ե�����������ͼ
        /// </summary>
        private void LoadView_TrainTipperQueue()
        {
            this.view_TrainTipperQueue_All = TrainTipperDAO.GetInstance().GetView_TrainTipperQueue(this.trainTipper.EquipmentCode, dtiptStartArriveTime.Value, dtiptEndArriveTime.Value);

            this.view_TrainTipperQueue_DF = this.view_TrainTipperQueue_All.Where(a => a.IsTurnover == eTrainTipperTurnoverStatus.����.ToString()).OrderBy(a => a.OrderNumber).ToList();
            superGridControl_DF.PrimaryGrid.DataSource = this.view_TrainTipperQueue_DF;

            this.view_TrainTipperQueue_YF = this.view_TrainTipperQueue_All.Where(a => a.IsTurnover == eTrainTipperTurnoverStatus.�ѷ�.ToString()).OrderByDescending(a => a.OrderNumber).ToList();
            superGridControl_YF.PrimaryGrid.DataSource = this.view_TrainTipperQueue_YF;

            AllotCellColor();
        }

        /// <summary>
        /// ���䵥Ԫ����ɫ
        /// </summary>
        private void AllotCellColor()
        {
            dicCellColors.Clear();

            foreach (View_TrainTipperQueue view_TrainTipperQueue in this.view_TrainTipperQueue_All)
            {
                string key = view_TrainTipperQueue.InFactoryBatchId + "-" + view_TrainTipperQueue.TrainSampleSchemeId;

                if (!dicCellColors.ContainsKey(key) && dicCellColors.Count < CellColors.Length) dicCellColors.Add(key, CellColors[dicCellColors.Count]);
            }
        }

        #endregion

        #region �ؼ��¼�

        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartTurnover_Click(object sender, EventArgs e)
        {
            Log4Neter.Info(this.trainTipper.EquipmentName + " - ��ʼ����");

            if (this.view_TrainTipperQueue_DF.Count == 0)
            {
                MessageBoxEx2Show("��������Ϊ�գ����ѯ�����к�������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnStartTurnover.Enabled = false;
            btnAddTrainCarriagePass.Enabled = true;
            dtiptStartArriveTime.Enabled = false;
            dtiptEndArriveTime.Enabled = false;

            CreateMainTask();
        }

        /// <summary>
        /// ֹͣ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopTurnover_Click(object sender, EventArgs e)
        {
            Log4Neter.Info(this.trainTipper.EquipmentName + " - ֹͣ����");

            // ȡ������
            this.taskSimpleScheduler.Cancal();

            if (MessageBoxEx2Show("�Ƿ�����֪ͨƤ��������ֹͣ������", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                btnStartTurnover.Enabled = false;
                btnAddTrainCarriagePass.Enabled = false;
                dtiptStartArriveTime.Enabled = false;
                dtiptEndArriveTime.Enabled = false;

                Task task = new Task(() =>
                {
                    // ֹͣ������
                    SendSamplerStopCmd(this.lastView_TrainTipperQueue);

                    this.InvokeEx(() =>
                    {
                        btnStartTurnover.Enabled = true;
                        btnAddTrainCarriagePass.Enabled = false;
                        dtiptStartArriveTime.Enabled = true;
                        dtiptEndArriveTime.Enabled = true;
                    });
                });
                task.Start();
            }
            else
            {
                btnStartTurnover.Enabled = true;
                btnAddTrainCarriagePass.Enabled = false;
                dtiptStartArriveTime.Enabled = true;
                dtiptEndArriveTime.Enabled = true;
            }
        }

        /// <summary>
        /// �볧ʱ��ı��¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtiptStartArriveTime_ValueObjectChanged(object sender, EventArgs e)
        {
            LoadView_TrainTipperQueue();
        }

        /// <summary>
        /// �볧ʱ��ı��¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtiptEndArriveTime_ValueObjectChanged(object sender, EventArgs e)
        {
            LoadView_TrainTipperQueue();
        }

        /// <summary>
        /// ����Ϊ��ǰ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddTrainCarriagePass_Click(object sender, EventArgs e)
        {
            GridRow gridRow = superGridControl_DF.PrimaryGrid.SelectedRows.OfType<GridRow>().FirstOrDefault();
            if (gridRow == null) return;

            View_TrainTipperQueue view_TrainTipperQueue = gridRow.DataItem as View_TrainTipperQueue;
            if (view_TrainTipperQueue == null) return;

            if (this.CurrentTrainCarriagePass == null
                || (this.CurrentTrainCarriagePass != null && (view_TrainTipperQueue.TrainNumber != this.CurrentTrainCarriagePass.TrainNumber
                || (view_TrainTipperQueue.TrainNumber == this.CurrentTrainCarriagePass.TrainNumber
                && MessageBoxEx.Show("�˳��Ѿ����ڷ����У�ȷ��Ҫ�������ã�", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))))
            {
                // ������������ʱ������ʾ
                if (view_TrainTipperQueue != this.view_TrainTipperQueue_DF.FirstOrDefault() && MessageBoxEx.Show("����[" + view_TrainTipperQueue.TrainNumber + "]���ǵ�ǰ�����е�һ�ڳ��ᣬȷ��Ҫ���ã�", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                // ���복��ʶ��ͨ����¼
                if (carriageRecognitionerDAO.SaveTrainCarriagePass(this.carriageRecognitionerMachineCode, view_TrainTipperQueue.TrainNumber, DateTime.Now, eTrainPassDirection.����))
                {
                    Log4Neter.Info(this.trainTipper.EquipmentName + " - ���ó���[" + view_TrainTipperQueue.TrainNumber + "]Ϊ��ǰ����");
                    OutputRunInfo("���ó���[" + view_TrainTipperQueue.TrainNumber + "]Ϊ��ǰ����");
                }
                else
                    MessageBoxEx.Show("����ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region SuperGridControl

        /// <summary>
        /// ��ʽ����Ԫ����ɫ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl_FormatCellColor_GetCellStyle(object sender, GridGetCellStyleEventArgs e)
        {
            if (e.GridCell.GridColumn.DataPropertyName == "TrainNumber")
            {
                View_TrainTipperQueue view_TrainTipperQueue = e.GridCell.GridRow.DataItem as View_TrainTipperQueue;
                if (view_TrainTipperQueue != null) e.Style.Background.Color1 = this.dicCellColors[view_TrainTipperQueue.InFactoryBatchId + "-" + view_TrainTipperQueue.TrainSampleSchemeId];
            }
        }

        private void superGridControl_BeginEdit(object sender, GridEditEventArgs e)
        {
            // ȡ���༭
            e.Cancel = true;
        }

        /// <summary>
        /// �����к�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl_GetRowHeaderText(object sender, GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.RowIndex + 1).ToString();
        }

        #endregion
    }
}
