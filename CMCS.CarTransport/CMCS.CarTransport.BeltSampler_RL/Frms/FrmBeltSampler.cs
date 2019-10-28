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
using CMCS.CarTransport.BeltSampler.Core;
using CMCS.CarTransport.BeltSampler.Enums;
using CMCS.CarTransport.BeltSampler.Frms.Sys;
using CMCS.CarTransport.DAO;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common.Entities.AutoMaker;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.BeltSampler;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Entities.Inf;
using CMCS.Common.Entities.Sys;
using CMCS.Common.Enums;
using CMCS.Common.Utilities;
using CMCS.Common.Views;
using CMCS.Forms.UserControls;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.SuperGrid;
using LED.YB14;

namespace CMCS.CarTransport.BeltSampler.Frms
{
    public partial class FrmBeltSampler : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// ����Ψһ��ʶ��
        /// </summary>
        public static string UniqueKey = "FrmBeltSampler";

        public FrmBeltSampler()
        {
            InitializeComponent();
        }

        #region Vars

        CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();
        BeltSamplerDAO beltSamplerDAO = BeltSamplerDAO.GetInstance();
        CommonDAO commonDAO = CommonDAO.GetInstance();

        /// <summary>
        /// ��������
        /// </summary>
        VoiceSpeaker voiceSpeaker = new VoiceSpeaker();

        eFlowFlag currentFlowFlag = eFlowFlag.���ͼƻ�;
        /// <summary>
        /// ��ǰҵ�����̱�ʶ
        /// </summary>
        public eFlowFlag CurrentFlowFlag
        {
            get { return currentFlowFlag; }
            set
            {
                currentFlowFlag = value;
                panCurrentCarNumber.Text = value.ToString();
            }
        }

        /// <summary>
        /// ���������� Ĭ��#1������
        /// </summary>
        string[] sampleMachineCodes = new string[] { GlobalVars.MachineCode_PDCYJ_1 };

        /// <summary>
        /// ��ǰѡ�е�Ƥ���������豸
        /// </summary>
        CmcsCMEquipment currentSampleMachine;
        /// <summary>
        /// ��ǰѡ��Ĳ������豸
        /// </summary>
        public CmcsCMEquipment CurrentSampleMachine
        {
            get { return currentSampleMachine; }
            set
            {
                currentSampleMachine = value;
                if (value != null)
                {
                    lblCurrSamplerName.Text = value.EquipmentName;
                }
            }
        }

        View_RLSampling currentRCSampling;
        /// <summary>
        /// ��ǰ��¯ָ��
        /// </summary>
        public View_RLSampling CurrentRCSampling
        {
            get { return currentRCSampling; }
            set
            {
                currentRCSampling = value;
                if (value != null)
                {
                    lblBatch.Text = value.ClassCyc;
                    labZC.Text = value.DutyCyc;
                    lblFactarriveDate.Text = value.RecordDate.ToString("yyyy-MM-dd");
                    lblSupplierName.Text = value.CoalpotName;
                }
                else
                {
                    lblBatch.Text = "####";
                    lblFactarriveDate.Text = "####";
                    lblSupplierName.Text = "####";
                    labZC.Text = "####";
                }
            }
        }

        InfBeltSampleCmd currentSampleCMD;
        /// <summary>
        /// ��ǰ��������
        /// </summary>
        public InfBeltSampleCmd CurrentSampleCMD
        {
            get { return currentSampleCMD; }
            set { currentSampleCMD = value; }
        }

        eEquInfGatherType currentGatherType = eEquInfGatherType.��жʽ;
        /// <summary>
        /// ��ǰж����ʽ Ĭ�ϵ�жʽ
        /// </summary>
        public eEquInfGatherType CurrentGatherType
        {
            get { return currentGatherType; }
            set
            {
                currentGatherType = value;
                lblGatherType.Text = value.ToString();
            }
        }

        eEquInfCmdResultCode currentCmdResultCode = eEquInfCmdResultCode.Ĭ��;
        /// <summary>
        /// ��ǰ����ִ�н�� 
        /// </summary>
        public eEquInfCmdResultCode CurrentCmdResultCode
        {
            get { return currentCmdResultCode; }
            set
            {
                currentCmdResultCode = value;

                lblResult.Text = currentCmdResultCode.ToString();
            }
        }

        eEquInfSamplerSystemStatus currentSystemStatus = eEquInfSamplerSystemStatus.��������;
        /// <summary>
        /// ��ǰ������ϵͳ״̬
        /// </summary>
        public eEquInfSamplerSystemStatus CurrentSystemStatus
        {
            get { return currentSystemStatus; }
            set
            {
                currentSystemStatus = value;
                lblSampleState.Text = value.ToString();
            }
        }
        #endregion

        /// <summary>
        /// �����ʼ��
        /// </summary>
        private void InitForm()
        {
            superGridControl1.PrimaryGrid.AutoGenerateColumns = false;
            superGridControl2.PrimaryGrid.AutoGenerateColumns = false;
            //��SuperGridControl�¼� gclmSetSampler
            GridButtonXEditControl btnSetSampler = superGridControl2.PrimaryGrid.Columns["gclmSetSampler"].EditControl as GridButtonXEditControl;
            btnSetSampler.Click += btnSetSampler_Click;

            // �������豸���룬����������һһ��Ӧ
            sampleMachineCodes = CommonDAO.GetInstance().GetAppletConfigString("�������豸����").Split('|');

            // ���ó���Զ�̿�������
            commonDAO.ResetAppRemoteControlCmd(CommonAppConfig.GetInstance().AppIdentifier);
        }

        private void FrmCarSampler_Load(object sender, EventArgs e)
        {

        }

        private void FrmCarSampler_Shown(object sender, EventArgs e)
        {
            InitForm();

            CreateSamplerButton();
            CreateEquStatus();
            BindRCSampling(superGridControl2);
            // ������һ����ť
            if (lypanSamplerButton.Controls.Count > 0) (lypanSamplerButton.Controls[0] as RadioButton).Checked = true;

        }

        private void FrmCarSampler_FormClosing(object sender, FormClosingEventArgs e)
        {

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
            timer1.Interval = 2000;

            try
            {
                switch (this.CurrentFlowFlag)
                {
                    case eFlowFlag.�ȴ�ִ��:
                        CurrentCmdResultCode = beltSamplerDAO.GetSampleCmdResult(CurrentSampleCMD.Id);
                        if (CurrentCmdResultCode == eEquInfCmdResultCode.�ɹ�)
                            this.CurrentFlowFlag = eFlowFlag.ִ�����;
                        break;
                    case eFlowFlag.ִ�����:
                        ResetBuyFuel();
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

            timer1.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            // 2��ִ��һ��
            timer2.Interval = 2000;

            try
            {
                RefreshEquStatus();
                SetGatherType();
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

        #endregion

        #region ����
        /// <summary>
        /// �����볧ú�����¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendSamplingPlan_Click(object sender, EventArgs e)
        {
            if (CurrentRCSampling == null) { MessageBoxEx.Show("�������õ�ǰ������"); return; }

            if (this.CurrentSystemStatus != eEquInfSamplerSystemStatus.��������) { MessageBoxEx.Show(this.CurrentSampleMachine.EquipmentName + "δ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!SendSamplingPlan()) { MessageBoxEx.Show("�����ƻ�����ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!SendSamplingCMD(true)) { MessageBoxEx.Show("���������ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MessageBoxEx.Show("���ͳɹ����ȴ�����");
            timer1.Enabled = true;
            this.CurrentFlowFlag = eFlowFlag.�ȴ�ִ��;
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartSampler_Click(object sender, EventArgs e)
        {
            if (CurrentRCSampling == null) { MessageBoxEx.Show("�������õ�ǰ������"); return; }

            if (!SendSamplingCMD(true)) { MessageBoxEx.Show("���������ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MessageBoxEx.Show("����ͳɹ����ȴ�ִ��");
            timer1.Enabled = true;
            this.CurrentFlowFlag = eFlowFlag.�ȴ�ִ��;
        }

        /// <summary>
        /// ֹͣ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopSampler_Click(object sender, EventArgs e)
        {
            if (CurrentRCSampling == null) { MessageBoxEx.Show("�������õ�ǰ������"); return; }

            if (!SendSamplingCMD(false)) { MessageBoxEx.Show("���������ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MessageBoxEx.Show("����ͳɹ����ȴ�ִ��");
            timer1.Enabled = true;
            this.CurrentFlowFlag = eFlowFlag.�ȴ�ִ��;
        }

        /// <summary>
        /// ���õ�ǰ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetSampler_Click(object sender, EventArgs e)
        {
            GridRow gridRow = (superGridControl2.PrimaryGrid.ActiveRow as GridRow);
            if (gridRow == null) return;

            if (MessageBoxEx.Show("�Ƿ����øü�¼Ϊ��ǰ������", "������ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                CurrentRCSampling = gridRow.DataItem as View_RLSampling;
        }
        #endregion

        #region �볧ú����ҵ��
        /// <summary>
        /// �����볧ú�����¼
        /// </summary>
        /// <returns></returns>
        bool SendSamplingPlan()
        {
            InfBeltSamplePlan oldBeltSamplePlan = Dbers.GetInstance().SelfDber.Entity<InfBeltSamplePlan>("where InFactoryBatchId=:InFactoryBatchId and SampleCode=:SampleCode", new { InFactoryBatchId = this.CurrentRCSampling.InfuranceId, SampleCode = this.CurrentRCSampling.SampleCode });
            if (oldBeltSamplePlan == null)
            {
                return Dbers.GetInstance().SelfDber.Insert<InfBeltSamplePlan>(new InfBeltSamplePlan
                {
                    DataFlag = 0,
                    InterfaceType = this.CurrentSampleMachine.InterfaceType,
                    InFactoryBatchId = this.CurrentRCSampling.InfuranceId,
                    SampleCode = this.CurrentRCSampling.SampleCode,
                    //FuelKindName = this.CurrentRCSampling.CoalpotName,
                    CarCount = 0,
                    Mt = 0,
                    TicketWeight = 0,
                    GatherType = CurrentGatherType.ToString(),
                    SampleType = "Ƥ������"
                }) > 0;
            }
            else
            {
                oldBeltSamplePlan.DataFlag = 0;
                //oldBeltSamplePlan.FuelKindName = this.CurrentRCSampling.FuelName;
                oldBeltSamplePlan.CarCount = 0;
                oldBeltSamplePlan.Mt = 0;
                oldBeltSamplePlan.TicketWeight = 0;
                oldBeltSamplePlan.GatherType = CurrentGatherType.ToString();

                return Dbers.GetInstance().SelfDber.Update(oldBeltSamplePlan) > 0;
            }
        }

        /// <summary>
        /// ���Ϳ�ʼ��������
        /// </summary>
        /// <returns></returns>
        bool SendSamplingCMD(bool isStart)
        {
            CurrentSampleCMD = new InfBeltSampleCmd
           {
               DataFlag = 0,
               InterfaceType = this.CurrentSampleMachine.InterfaceType,
               MachineCode = this.CurrentSampleMachine.EquipmentCode,
               ResultCode = eEquInfCmdResultCode.Ĭ��.ToString(),
               SampleCode = this.CurrentRCSampling.SampleCode,
               CmdCode = (isStart == true ? eEquInfSamplerCmd.��ʼ����.ToString() : eEquInfSamplerCmd.��������.ToString())
           };
            return Dbers.GetInstance().SelfDber.Insert<InfBeltSampleCmd>(CurrentSampleCMD) > 0;
        }

        /// <summary>
        /// �����볧ú�����¼
        /// </summary>
        void ResetBuyFuel()
        {
            this.CurrentFlowFlag = eFlowFlag.ѡ��ƻ�;
            this.CurrentCmdResultCode = eEquInfCmdResultCode.Ĭ��;
            this.CurrentSampleCMD = null;
            this.CurrentRCSampling = null;
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

        #endregion

        #region �ź�ҵ��
        /// <summary>
        /// ����Ƥ����������ȫ�Զ�������״̬
        /// </summary>
        private void CreateEquStatus()
        {
            flpanEquState.SuspendLayout();

            foreach (string cMEquipmentCode in sampleMachineCodes)
            {
                UCtrlSignalLight uCtrlSignalLight = new UCtrlSignalLight()
                {
                    Anchor = AnchorStyles.Left,
                    Tag = cMEquipmentCode,
                    Size = new System.Drawing.Size(20, 20),
                    Padding = new System.Windows.Forms.Padding(10, 0, 0, 0)
                };
                SetSystemStatusToolTip(uCtrlSignalLight);

                flpanEquState.Controls.Add(uCtrlSignalLight);

                LabelX lblMachineName = new LabelX()
                {
                    Text = cMEquipmentCode,
                    Tag = cMEquipmentCode,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Segoe UI", 14.25f, FontStyle.Bold)
                };

                flpanEquState.Controls.Add(lblMachineName);
            }

            flpanEquState.ResumeLayout();

            if (this.flpanEquState.Controls.Count == 0)
                MessageBoxEx.Show("Ƥ��������������������δ���ã�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// ����Ƥ��������״̬
        /// </summary>
        private void RefreshEquStatus()
        {
            foreach (UCtrlSignalLight uCtrlSignalLight in flpanEquState.Controls.OfType<UCtrlSignalLight>())
            {
                if (uCtrlSignalLight.Tag == null) continue;

                string machineCode = uCtrlSignalLight.Tag.ToString();
                if (string.IsNullOrEmpty(machineCode)) continue;

                string systemStatus = CommonDAO.GetInstance().GetSignalDataValue(machineCode, eSignalDataName.ϵͳ.ToString());
                if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString())
                    uCtrlSignalLight.LightColor = EquipmentStatusColors.BeReady;
                else if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString() || systemStatus == eEquInfSamplerSystemStatus.����ж��.ToString())
                    uCtrlSignalLight.LightColor = EquipmentStatusColors.Working;
                else if (systemStatus == eEquInfSamplerSystemStatus.��������.ToString())
                    uCtrlSignalLight.LightColor = EquipmentStatusColors.Breakdown;

                eEquInfSamplerSystemStatus status;
                //��ǰѡ��Ĳ�����״̬
                if (machineCode == CurrentSampleMachine.EquipmentCode)
                    if (Enum.TryParse(systemStatus, out status))
                        CurrentSystemStatus = status;
            }
        }

        /// <summary>
        /// ����ToolTip��ʾ
        /// </summary>
        private void SetSystemStatusToolTip(Control control)
        {
            this.toolTip1.SetToolTip(control, "<��ɫ> ��������\r\n<��ɫ> ��������\r\n<��ɫ> ��������");
        }

        private void SetGatherType()
        {
            eEquInfGatherType GatherType;
            if (currentSampleMachine.EquipmentCode == "#1Ƥ��������")
            {
                if (Enum.TryParse(CommonDAO.GetInstance().GetAppletConfigString("��������", "#1Ƥ��������������ʽ"), out GatherType))
                    CurrentGatherType = GatherType;
            }
            else
                if (Enum.TryParse(CommonDAO.GetInstance().GetAppletConfigString("��������", "#2Ƥ��������������ʽ"), out GatherType))
                    CurrentGatherType = GatherType;
        }
        #endregion

        #region ����

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

        /// <summary>
        /// ���ɲ�����ѡ��
        /// </summary>
        private void CreateSamplerButton()
        {
            foreach (string machineCode in sampleMachineCodes)
            {
                CmcsCMEquipment Equipment = CommonDAO.GetInstance().GetCMEquipmentByMachineCode(machineCode);
                RadioButton rbtnSampler = new RadioButton();
                rbtnSampler.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
                rbtnSampler.Text = Equipment.EquipmentName;
                rbtnSampler.Tag = Equipment;
                rbtnSampler.AutoSize = true;
                rbtnSampler.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
                rbtnSampler.CheckedChanged += new EventHandler(rbtnSampler_CheckedChanged);

                lypanSamplerButton.Controls.Add(rbtnSampler);
            }
        }

        void rbtnSampler_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbtnSampler = sender as RadioButton;
            this.CurrentSampleMachine = rbtnSampler.Tag as CmcsCMEquipment;
            BindBeltSampleBarrel(superGridControl1, CurrentSampleMachine.EquipmentCode);
        }

        #endregion

        /// <summary>
        /// �󶨼�������Ϣ
        /// </summary>
        /// <param name="superGridControl"></param>
        /// <param name="machineCode">�豸����</param>
        private void BindBeltSampleBarrel(SuperGridControl superGridControl, string machineCode)
        {
            List<InfEquInfSampleBarrel> list = CommonDAO.GetInstance().GetEquInfSampleBarrels(machineCode);
            superGridControl.PrimaryGrid.DataSource = list;
        }

        private void BindRCSampling(SuperGridControl superGridControl)
        {
            List<View_RLSampling> list = commonDAO.SelfDber.Entities<View_RLSampling>("where RecordDate >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
            superGridControl.PrimaryGrid.DataSource = list;
        }
    }
}
