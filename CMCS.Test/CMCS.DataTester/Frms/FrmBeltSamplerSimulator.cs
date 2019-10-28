using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.DumblyConcealer.Tasks.BeltSampler.Entities;
using CMCS.DumblyConcealer;
using CMCS.DapperDber.Dbs.SqlServerDb;
using CMCS.DapperDber.Util;
using CMCS.Common.Utilities;
using System.Threading;
using System.Threading.Tasks;
using CMCS.Common.Enums;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities.BaseInfo;

namespace CMCS.DataTester.Frms
{
    public partial class FrmBeltSamplerSimulator : Form
    {
        SqlServerDapperDber dber = DcDbers.GetInstance().BeltSampler_Dber;

        bool isStartSimulator = false;
        /// <summary>
        /// 是否开始模拟
        /// </summary>
        public bool IsStartSimulator
        {
            get { return isStartSimulator; }
            set
            {
                isStartSimulator = value;

                btnStart.Text = value ? "停止模拟" : "开始模拟";
            }
        }

        public FrmBeltSamplerSimulator()
        {
            InitializeComponent();
        }

        private void FrmBeltSamplerSimulator_Load(object sender, EventArgs e)
        {
            // 加载皮带采样机
            foreach (CmcsCMEquipment cMEquipment in CommonDAO.GetInstance().GetChildrenMachinesByCode("皮带采样机"))
            {
                cmbBeltSampler.Items.Add(cMEquipment.EquipmentCode);
            }
            if (cmbBeltSampler.Items.Count > 0) cmbBeltSampler.SelectedIndex = 0;

            CreateMainTask();
        }

        /// <summary>
        /// 开始模拟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            this.IsStartSimulator = !IsStartSimulator;
        }

        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

        System.Threading.AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        private void CreateMainTask()
        {
            taskSimpleScheduler = new TaskSimpleScheduler();

            autoResetEvent.Reset();

            taskSimpleScheduler.StartNewTask("模拟业务", () =>
            {
                if (!this.IsStartSimulator) return;

                // 心跳
                dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = GlobalVars.EquHeartbeatName, TagValue = DateTime.Now.ToString() });
                // 更新采样计划
                dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJPlan>() + " set DataFlag=1 where DataFlag=0");

                // 控制命令
                EquPDCYJCmd pDCYJCmd = dber.Entity<EquPDCYJCmd>("where DataFlag=0 order by CreateDate desc");
                if (pDCYJCmd != null)
                {
                    CmdHandle(pDCYJCmd);

                    autoResetEvent.WaitOne();
                }

                // 卸样命令
                EquPDCYJUnloadCmd pDCYJUnloadCmd = dber.Entity<EquPDCYJUnloadCmd>("where DataFlag=0 order by CreateDate desc");
                if (pDCYJUnloadCmd != null)
                {
                    CmdHandle(pDCYJUnloadCmd);

                    autoResetEvent.WaitOne();
                }

            }, 3000);
        }

        private void CmdHandle(EquPDCYJCmd input)
        {
            Task task = new Task((state) =>
            {
                EquPDCYJCmd pDCYJCmd = state as EquPDCYJCmd;
                OutputRunInfo(rtxtOutput, "处理命令，命令代码：" + pDCYJCmd.CmdCode + "  采样码：" + pDCYJCmd.SampleCode);

                if (pDCYJCmd.CmdCode == eEquInfSamplerCmd.开始采样.ToString())
                {
                    EquPDCYJPlan pDCYJPlan = dber.Entity<EquPDCYJPlan>("where SampleCode=@SampleCode", new { SampleCode = pDCYJCmd.SampleCode });
                    if (pDCYJPlan != null)
                    {
                        Thread.Sleep(3000);
                        OutputRunInfo(rtxtOutput, "启动采样机");
                        // 更新系统状态为正在运行
                        dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJSignal>() + " set TagValue=@TagValue where MachineCode=@MachineCode and TagName=@TagName", new { MachineCode = pDCYJCmd.MachineCode, TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在运行.ToString() });

                        // 更新集样罐
                        dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJBarrel>() + " set IsCurrent=0 where MachineCode=@MachineCode and BarrelType=@BarrelType", new { MachineCode = pDCYJCmd.MachineCode, BarrelType = pDCYJPlan.GatherType });
                        EquPDCYJBarrel pDCYJBarrel = dber.Entity<EquPDCYJBarrel>("where MachineCode=@MachineCode and SampleCode=@SampleCode and BarrelType=@BarrelType and SampleCount<3", new { MachineCode = pDCYJCmd.MachineCode, SampleCode = pDCYJCmd.SampleCode, BarrelType = pDCYJPlan.GatherType });
                        if (pDCYJBarrel == null)
                        {
                            pDCYJBarrel = dber.Entity<EquPDCYJBarrel>("where MachineCode=@MachineCode and SampleCode='' and BarrelType=@BarrelType", new { MachineCode = pDCYJCmd.MachineCode, BarrelType = pDCYJPlan.GatherType });
                            if (pDCYJBarrel != null)
                            {
                                OutputRunInfo(rtxtOutput, "分配集样罐，罐号：" + pDCYJBarrel.BarrelNumber + "  " + pDCYJBarrel.BarrelType);

                                pDCYJBarrel.SampleCount = 1;
                                pDCYJBarrel.SampleCode = pDCYJCmd.SampleCode;
                                pDCYJBarrel.InFactoryBatchId = pDCYJPlan.InFactoryBatchId;
                                pDCYJBarrel.IsCurrent = 1;
                                pDCYJBarrel.BarrelStatus = eSampleBarrelStatus.未满.ToString();
                                pDCYJBarrel.UpdateTime = DateTime.Now;
                                pDCYJBarrel.DataFlag = 0;
                                dber.Update(pDCYJBarrel);
                            }
                        }
                        else
                        {
                            OutputRunInfo(rtxtOutput, "分配集样罐，罐号：" + pDCYJBarrel.BarrelNumber + " " + pDCYJBarrel.BarrelType);

                            pDCYJBarrel.SampleCount++;
                            pDCYJBarrel.IsCurrent = 1;
                            pDCYJBarrel.BarrelStatus = eSampleBarrelStatus.未满.ToString();
                            pDCYJBarrel.UpdateTime = DateTime.Now;
                            pDCYJBarrel.DataFlag = 0;
                            dber.Update(pDCYJBarrel);
                        }

                        // 更新计划
                        pDCYJPlan.StartTime = DateTime.Now;
                        dber.Update(pDCYJPlan);

                        // 更新命令
                        pDCYJCmd.ResultCode = eEquInfCmdResultCode.成功.ToString();
                        pDCYJCmd.DataFlag = 2;
                        dber.Update(pDCYJCmd);
                    }
                    else
                    {
                        OutputRunInfo(rtxtOutput, "未找到采样计划，采样码：" + pDCYJCmd.SampleCode);

                        pDCYJCmd.ResultCode = eEquInfCmdResultCode.失败.ToString();
                        pDCYJCmd.DataFlag = 1;
                        dber.Update(pDCYJCmd);
                    }
                }
                else if (pDCYJCmd.CmdCode == eEquInfSamplerCmd.结束采样.ToString())
                {
                    Thread.Sleep(3000);
                    OutputRunInfo(rtxtOutput, "停止采样机");

                    // 更新系统状态为就绪待机
                    dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJSignal>() + " set TagValue=@TagValue where MachineCode=@MachineCode and TagName=@TagName", new { MachineCode = pDCYJCmd.MachineCode, TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.就绪待机.ToString() });

                    EquPDCYJPlan pDCYJPlan = dber.Entity<EquPDCYJPlan>("where SampleCode=@SampleCode", new { SampleCode = pDCYJCmd.SampleCode });
                    if (pDCYJPlan != null)
                    {
                        // 更新计划
                        pDCYJPlan.SampleUser = "模拟采样员";
                        pDCYJPlan.StartTime = DateTime.Now.AddSeconds(10);
                        pDCYJPlan.EndTime = DateTime.Now;
                        dber.Update(pDCYJPlan);
                    }

                    // 更新命令
                    pDCYJCmd.ResultCode = eEquInfCmdResultCode.成功.ToString();
                    pDCYJCmd.DataFlag = 2;
                    dber.Update(pDCYJCmd);
                }

                autoResetEvent.Set();

            }, input);
            task.Start();
        }

        private void CmdHandle(EquPDCYJUnloadCmd input)
        {
            Task task = new Task((state) =>
            {
                EquPDCYJUnloadCmd pDCYJUnloadCmd = state as EquPDCYJUnloadCmd;
                OutputRunInfo(rtxtOutput, "处理卸样命令，采样码：" + pDCYJUnloadCmd.SampleCode);

                // 更新系统状态为正在卸样
                dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJSignal>() + " set TagValue=@TagValue where MachineCode=@MachineCode and TagName=@TagName", new { MachineCode = pDCYJUnloadCmd.MachineCode, TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在卸样.ToString() });
                Thread.Sleep(3000);

                foreach (EquPDCYJBarrel pDCYJBarrel in dber.Entities<EquPDCYJBarrel>("where SampleCode=@SampleCode", new { SampleCode = pDCYJUnloadCmd.SampleCode }))
                {
                    // 生成卸样结果
                    dber.Insert(new EquPDCYJUnloadResult
                    {
                        DataFlag = 0,
                        SampleCode = pDCYJUnloadCmd.SampleCode,
                        UnloadTime = DateTime.Now,
                        BarrelNumber = pDCYJBarrel.BarrelNumber,
                        BarrelCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        SamplingId = pDCYJUnloadCmd.SamplingId,
                        MachineCode = pDCYJBarrel.MachineCode
                    });

                    pDCYJBarrel.SampleCount = 0;
                    pDCYJBarrel.InFactoryBatchId = string.Empty;
                    pDCYJBarrel.SampleCode = string.Empty;
                    pDCYJBarrel.BarrelStatus = eSampleBarrelStatus.空桶.ToString();
                    pDCYJBarrel.UpdateTime = DateTime.Now;
                    pDCYJBarrel.DataFlag = 0;

                    if (dber.Update(pDCYJBarrel) > 0) OutputRunInfo(rtxtOutput, "卸样完成，罐号：" + pDCYJBarrel.BarrelNumber.ToString() + "  " + pDCYJBarrel.BarrelType.ToString());

                    Thread.Sleep(1500);
                }

                // 更新命令
                pDCYJUnloadCmd.ResultCode = eEquInfCmdResultCode.成功.ToString();
                pDCYJUnloadCmd.DataFlag = 2;
                dber.Update(pDCYJUnloadCmd);

                OutputRunInfo(rtxtOutput, "卸样完成");

                autoResetEvent.Set();

            }, input);
            task.Start();
        }

        /// <summary>
        /// 重置所有接口表
        /// </summary>
        void ResetAll()
        {
            dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJPlan>() + " set DataFlag=3");
            dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJCmd>() + " set DataFlag=3");
            dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJUnloadCmd>() + " set DataFlag=3");
            dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJBarrel>() + " set SampleCode='',InFactoryBatchId='',SampleCount=0,BarrelStatus='" + eSampleBarrelStatus.空桶.ToString() + "'");
        }

        #region 改变系统状态

        private void btnSystemStatus_JXDJ_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbBeltSampler.Text)) return;

            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.就绪待机.ToString());
            dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJSignal>() + " set TagValue=@TagValue where MachineCode=@MachineCode and TagName=@TagName", new { MachineCode = (cmbBeltSampler.SelectedIndex + 1).ToString(), TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.就绪待机.ToString() });
        }

        private void btnSystemStatus_ZZYX_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbBeltSampler.Text)) return;

            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.正在运行.ToString());
            dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJSignal>() + " set TagValue=@TagValue where MachineCode=@MachineCode and TagName=@TagName", new { MachineCode = (cmbBeltSampler.SelectedIndex + 1).ToString(), TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在运行.ToString() });
        }

        private void btnSystemStatus_ZZXY_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbBeltSampler.Text)) return;

            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.正在卸样.ToString());
            dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJSignal>() + " set TagValue=@TagValue where MachineCode=@MachineCode and TagName=@TagName", new { MachineCode = (cmbBeltSampler.SelectedIndex + 1).ToString(), TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在卸样.ToString() });
        }

        private void btnSystemStatus_FSGZ_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbBeltSampler.Text)) return;

            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.发生故障.ToString());
            dber.Execute("update " + EntityReflectionUtil.GetTableName<EquPDCYJSignal>() + " set TagValue=@TagValue where MachineCode=@MachineCode and TagName=@TagName", new { MachineCode = (cmbBeltSampler.SelectedIndex + 1).ToString(), TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.发生故障.ToString() });
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetAll();
        }

        #endregion

        #region Util

        /// <summary>
        /// 输出信息类型
        /// </summary>
        public enum eOutputType
        {
            /// <summary>
            /// 普通
            /// </summary>
            [Description("#FFFFFF")]
            Normal,
            /// <summary>
            /// 错误
            /// </summary>
            [Description("#DB2606")]
            Error
        }

        /// <summary>
        /// 输出运行信息
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="text"></param>
        /// <param name="outputType"></param>
        private void OutputRunInfo(RichTextBox richTextBox, string text, eOutputType outputType = eOutputType.Normal)
        {
            this.InvokeEx(() =>
            {
                if (richTextBox.TextLength > 100000) richTextBox.Clear();

                text = string.Format(" # {0} - {1}", DateTime.Now.ToString("HH:mm:ss"), text);

                richTextBox.SelectionStart = richTextBox.TextLength;

                switch (outputType)
                {
                    case eOutputType.Normal:
                        richTextBox.SelectionColor = Color.Black;
                        break;
                    case eOutputType.Error:
                        richTextBox.SelectionColor = ColorTranslator.FromHtml("#DB2606");
                        break;
                    default:
                        richTextBox.SelectionColor = Color.White;
                        break;
                }

                richTextBox.AppendText(string.Format("{0}\r", text));

                richTextBox.ScrollToCaret();
            });
        }

        /// <summary>
        /// 输出异常信息
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        private void OutputErrorInfo(string text, Exception ex)
        {
            this.InvokeEx(() =>
            {
                text = string.Format("# {0} - {1}\r\n{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text, ex.Message);

                OutputRunInfo(rtxtOutput, text + "", eOutputType.Error);
            });
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
    }
}
