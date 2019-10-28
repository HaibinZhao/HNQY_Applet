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
        /// 窗体唯一标识符
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
        /// 语音播报
        /// </summary>
        VoiceSpeaker voiceSpeaker = new VoiceSpeaker();

        eFlowFlag currentFlowFlag = eFlowFlag.发送计划;
        /// <summary>
        /// 当前业务流程标识
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
        /// 采样机编码 默认#1采样机
        /// </summary>
        string[] sampleMachineCodes = new string[] { GlobalVars.MachineCode_PDCYJ_1 };

        /// <summary>
        /// 当前选中的皮带采样机设备
        /// </summary>
        CmcsCMEquipment currentSampleMachine;
        /// <summary>
        /// 当前选择的采样机设备
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
        /// 当前入炉指令
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
        /// 当前采样命令
        /// </summary>
        public InfBeltSampleCmd CurrentSampleCMD
        {
            get { return currentSampleCMD; }
            set { currentSampleCMD = value; }
        }

        eEquInfGatherType currentGatherType = eEquInfGatherType.底卸式;
        /// <summary>
        /// 当前卸样方式 默认底卸式
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

        eEquInfCmdResultCode currentCmdResultCode = eEquInfCmdResultCode.默认;
        /// <summary>
        /// 当前命令执行结果 
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

        eEquInfSamplerSystemStatus currentSystemStatus = eEquInfSamplerSystemStatus.就绪待机;
        /// <summary>
        /// 当前采样机系统状态
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
        /// 窗体初始化
        /// </summary>
        private void InitForm()
        {
            superGridControl1.PrimaryGrid.AutoGenerateColumns = false;
            superGridControl2.PrimaryGrid.AutoGenerateColumns = false;
            //绑定SuperGridControl事件 gclmSetSampler
            GridButtonXEditControl btnSetSampler = superGridControl2.PrimaryGrid.Columns["gclmSetSampler"].EditControl as GridButtonXEditControl;
            btnSetSampler.Click += btnSetSampler_Click;

            // 采样机设备编码，跟采样程序一一对应
            sampleMachineCodes = CommonDAO.GetInstance().GetAppletConfigString("采样机设备编码").Split('|');

            // 重置程序远程控制命令
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
            // 触发第一个按钮
            if (lypanSamplerButton.Controls.Count > 0) (lypanSamplerButton.Controls[0] as RadioButton).Checked = true;

        }

        private void FrmCarSampler_FormClosing(object sender, FormClosingEventArgs e)
        {

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
            timer1.Interval = 2000;

            try
            {
                switch (this.CurrentFlowFlag)
                {
                    case eFlowFlag.等待执行:
                        CurrentCmdResultCode = beltSamplerDAO.GetSampleCmdResult(CurrentSampleCMD.Id);
                        if (CurrentCmdResultCode == eEquInfCmdResultCode.成功)
                            this.CurrentFlowFlag = eFlowFlag.执行完毕;
                        break;
                    case eFlowFlag.执行完毕:
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
            // 2秒执行一次
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

        #region 操作
        /// <summary>
        /// 保存入厂煤运输记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendSamplingPlan_Click(object sender, EventArgs e)
        {
            if (CurrentRCSampling == null) { MessageBoxEx.Show("请先设置当前采样单"); return; }

            if (this.CurrentSystemStatus != eEquInfSamplerSystemStatus.就绪待机) { MessageBoxEx.Show(this.CurrentSampleMachine.EquipmentName + "未就绪", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!SendSamplingPlan()) { MessageBoxEx.Show("采样计划发送失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!SendSamplingCMD(true)) { MessageBoxEx.Show("采样命令发送失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MessageBoxEx.Show("发送成功，等待采样");
            timer1.Enabled = true;
            this.CurrentFlowFlag = eFlowFlag.等待执行;
        }

        /// <summary>
        /// 开始采样
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartSampler_Click(object sender, EventArgs e)
        {
            if (CurrentRCSampling == null) { MessageBoxEx.Show("请先设置当前采样单"); return; }

            if (!SendSamplingCMD(true)) { MessageBoxEx.Show("采样命令发送失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MessageBoxEx.Show("命令发送成功，等待执行");
            timer1.Enabled = true;
            this.CurrentFlowFlag = eFlowFlag.等待执行;
        }

        /// <summary>
        /// 停止采样
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopSampler_Click(object sender, EventArgs e)
        {
            if (CurrentRCSampling == null) { MessageBoxEx.Show("请先设置当前采样单"); return; }

            if (!SendSamplingCMD(false)) { MessageBoxEx.Show("采样命令发送失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MessageBoxEx.Show("命令发送成功，等待执行");
            timer1.Enabled = true;
            this.CurrentFlowFlag = eFlowFlag.等待执行;
        }

        /// <summary>
        /// 设置当前采样单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetSampler_Click(object sender, EventArgs e)
        {
            GridRow gridRow = (superGridControl2.PrimaryGrid.ActiveRow as GridRow);
            if (gridRow == null) return;

            if (MessageBoxEx.Show("是否设置该记录为当前采样单", "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                CurrentRCSampling = gridRow.DataItem as View_RLSampling;
        }
        #endregion

        #region 入厂煤采样业务
        /// <summary>
        /// 保存入厂煤运输记录
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
                    SampleType = "皮带采样"
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
        /// 发送开始采样命令
        /// </summary>
        /// <returns></returns>
        bool SendSamplingCMD(bool isStart)
        {
            CurrentSampleCMD = new InfBeltSampleCmd
           {
               DataFlag = 0,
               InterfaceType = this.CurrentSampleMachine.InterfaceType,
               MachineCode = this.CurrentSampleMachine.EquipmentCode,
               ResultCode = eEquInfCmdResultCode.默认.ToString(),
               SampleCode = this.CurrentRCSampling.SampleCode,
               CmdCode = (isStart == true ? eEquInfSamplerCmd.开始采样.ToString() : eEquInfSamplerCmd.结束采样.ToString())
           };
            return Dbers.GetInstance().SelfDber.Insert<InfBeltSampleCmd>(CurrentSampleCMD) > 0;
        }

        /// <summary>
        /// 重置入厂煤运输记录
        /// </summary>
        void ResetBuyFuel()
        {
            this.CurrentFlowFlag = eFlowFlag.选择计划;
            this.CurrentCmdResultCode = eEquInfCmdResultCode.默认;
            this.CurrentSampleCMD = null;
            this.CurrentRCSampling = null;
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

        #endregion

        #region 信号业务
        /// <summary>
        /// 创建皮带采样机、全自动制样机状态
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
                MessageBoxEx.Show("皮带采样机或制样机参数未设置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 更新皮带采样机状态
        /// </summary>
        private void RefreshEquStatus()
        {
            foreach (UCtrlSignalLight uCtrlSignalLight in flpanEquState.Controls.OfType<UCtrlSignalLight>())
            {
                if (uCtrlSignalLight.Tag == null) continue;

                string machineCode = uCtrlSignalLight.Tag.ToString();
                if (string.IsNullOrEmpty(machineCode)) continue;

                string systemStatus = CommonDAO.GetInstance().GetSignalDataValue(machineCode, eSignalDataName.系统.ToString());
                if (systemStatus == eEquInfSamplerSystemStatus.就绪待机.ToString())
                    uCtrlSignalLight.LightColor = EquipmentStatusColors.BeReady;
                else if (systemStatus == eEquInfSamplerSystemStatus.正在运行.ToString() || systemStatus == eEquInfSamplerSystemStatus.正在卸样.ToString())
                    uCtrlSignalLight.LightColor = EquipmentStatusColors.Working;
                else if (systemStatus == eEquInfSamplerSystemStatus.发生故障.ToString())
                    uCtrlSignalLight.LightColor = EquipmentStatusColors.Breakdown;

                eEquInfSamplerSystemStatus status;
                //当前选择的采样机状态
                if (machineCode == CurrentSampleMachine.EquipmentCode)
                    if (Enum.TryParse(systemStatus, out status))
                        CurrentSystemStatus = status;
            }
        }

        /// <summary>
        /// 设置ToolTip提示
        /// </summary>
        private void SetSystemStatusToolTip(Control control)
        {
            this.toolTip1.SetToolTip(control, "<绿色> 就绪待机\r\n<红色> 正在运行\r\n<黄色> 发生故障");
        }

        private void SetGatherType()
        {
            eEquInfGatherType GatherType;
            if (currentSampleMachine.EquipmentCode == "#1皮带采样机")
            {
                if (Enum.TryParse(CommonDAO.GetInstance().GetAppletConfigString("公共配置", "#1皮带采样机集样方式"), out GatherType))
                    CurrentGatherType = GatherType;
            }
            else
                if (Enum.TryParse(CommonDAO.GetInstance().GetAppletConfigString("公共配置", "#2皮带采样机集样方式"), out GatherType))
                    CurrentGatherType = GatherType;
        }
        #endregion

        #region 其他

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

        /// <summary>
        /// 生成采样机选项
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
        /// 绑定集样罐信息
        /// </summary>
        /// <param name="superGridControl"></param>
        /// <param name="machineCode">设备编码</param>
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
