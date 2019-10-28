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
using CMCS.DumblyConcealer.Tasks.CarJXSampler.Entities;

namespace CMCS.DataTester.Frms
{
    public partial class FrmCarJxSamplerSimulator : Form
    {
        SqlServerDapperDber dber1 = new SqlServerDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#1汽车机械采样机接口连接字符串"));
        SqlServerDapperDber dber2 = new SqlServerDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#2汽车机械采样机接口连接字符串"));

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

                rbtnMachineCode1.Enabled = !isStartSimulator;
                rbtnMachineCode2.Enabled = !isStartSimulator;

                btnStart.Text = value ? "停止模拟" : "开始模拟";
            }
        }

        SqlServerDapperDber EquDber
        {
            get
            {
                if (rbtnMachineCode1.Checked)
                    return dber1;
                else
                    return dber2;
            }
        }

        public FrmCarJxSamplerSimulator()
        {
            InitializeComponent();
        }

        private void FrmCarJxSamplerSimulator_Load(object sender, EventArgs e)
        {
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
                this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = GlobalVars.EquHeartbeatName, TagValue = DateTime.Now.ToString() });

                // 控制命令
                EquQCJXCYJSampleCmd qCJXCYJSampleCmd = this.EquDber.Entity<EquQCJXCYJSampleCmd>("where DataFlag=0 order by CreateDate desc");
                if (qCJXCYJSampleCmd != null)
                {
                    CmdHandle(qCJXCYJSampleCmd);

                    autoResetEvent.WaitOne();
                }

                // 卸样命令
                EquQCJXCYJUnloadCmd qCJXCYJUnloadCmd = this.EquDber.Entity<EquQCJXCYJUnloadCmd>("where DataFlag=0 order by CreateDate desc");
                if (qCJXCYJUnloadCmd != null)
                {
                    CmdHandle(qCJXCYJUnloadCmd);

                    autoResetEvent.WaitOne();
                }

            }, 3000);
        }

        private void CmdHandle(EquQCJXCYJSampleCmd input)
        {
            Task task = new Task((state) =>
            {
                EquQCJXCYJSampleCmd qCJXCYJSampleCmd = state as EquQCJXCYJSampleCmd;
                OutputRunInfo(rtxtOutput, "处理采样命令，采样码：" + qCJXCYJSampleCmd.SampleCode);

                // 更新系统状态为正在运行
                this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在运行.ToString() });
                Thread.Sleep(3000);

                OutputRunInfo(rtxtOutput, "启动采样机");
                qCJXCYJSampleCmd.StartTime = DateTime.Now;

                // 更新集样罐
                this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJBarrel>() + " set IsCurrent=0");
                EquQCJXCYJBarrel qCJXCYJBarrel = this.EquDber.Entity<EquQCJXCYJBarrel>("where SampleCode=@SampleCode and SampleCount<3", new { SampleCode = qCJXCYJSampleCmd.SampleCode });
                if (qCJXCYJBarrel == null)
                {
                    qCJXCYJBarrel = this.EquDber.Entity<EquQCJXCYJBarrel>("where SampleCode=''");
                    if (qCJXCYJBarrel != null)
                    {
                        OutputRunInfo(rtxtOutput, "分配集样罐，罐号：" + qCJXCYJBarrel.BarrelNumber + "  " + qCJXCYJBarrel.BarrelType);

                        qCJXCYJBarrel.SampleCount = 1;
                        qCJXCYJBarrel.SampleCode = qCJXCYJSampleCmd.SampleCode;
                        qCJXCYJBarrel.InFactoryBatchId = qCJXCYJSampleCmd.InFactoryBatchId;
                        qCJXCYJBarrel.IsCurrent = 1;
                        qCJXCYJBarrel.BarrelStatus = eSampleBarrelStatus.未满.ToString();
                        qCJXCYJBarrel.UpdateTime = DateTime.Now;
                        qCJXCYJBarrel.BarrelType = eEquInfGatherType.底卸式.ToString();
                        qCJXCYJBarrel.DataFlag = 0;
                        this.EquDber.Update(qCJXCYJBarrel);
                    }
                }
                else
                {
                    OutputRunInfo(rtxtOutput, "分配集样罐，罐号：" + qCJXCYJBarrel.BarrelNumber + " " + qCJXCYJBarrel.BarrelType);

                    qCJXCYJBarrel.SampleCount++;
                    qCJXCYJBarrel.IsCurrent = 1;
                    qCJXCYJBarrel.BarrelStatus = eSampleBarrelStatus.未满.ToString();
                    qCJXCYJBarrel.UpdateTime = DateTime.Now;
                    qCJXCYJBarrel.BarrelType = eEquInfGatherType.底卸式.ToString();
                    qCJXCYJBarrel.DataFlag = 0;
                    this.EquDber.Update(qCJXCYJBarrel);
                }

                // 更新命令
                qCJXCYJSampleCmd.ResultCode = eEquInfCmdResultCode.成功.ToString();
                qCJXCYJSampleCmd.DataFlag = 2;
                this.EquDber.Update(qCJXCYJSampleCmd);

                // 更新系统状态为就绪待机
                this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSignal>() + " set TagValue=@TagValue where  TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.就绪待机.ToString() });

                Thread.Sleep(2000);

                // 更新命令
                qCJXCYJSampleCmd.SampleUser = "自动";
                qCJXCYJSampleCmd.EndTime = DateTime.Now;
                qCJXCYJSampleCmd.ResultCode = eEquInfCmdResultCode.成功.ToString();
                qCJXCYJSampleCmd.DataFlag = 2;
                this.EquDber.Update(qCJXCYJSampleCmd);

                autoResetEvent.Set();

            }, input);

            task.Start();
        }

        private void CmdHandle(EquQCJXCYJUnloadCmd input)
        {
            Task task = new Task((state) =>
            {
                EquQCJXCYJUnloadCmd qCJXCYJUnloadCmd = state as EquQCJXCYJUnloadCmd;
                OutputRunInfo(rtxtOutput, "处理卸样命令，采样码：" + qCJXCYJUnloadCmd.SampleCode);

                // 更新系统状态为正在卸样
                this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在卸样.ToString() });
                Thread.Sleep(3000);

                foreach (EquQCJXCYJBarrel pDCYJBarrel in this.EquDber.Entities<EquQCJXCYJBarrel>("where SampleCode=@SampleCode", new { SampleCode = qCJXCYJUnloadCmd.SampleCode }))
                {
                    // 生成卸样结果
                    this.EquDber.Insert(new EquQCJXCYJUnloadResult
                    {
                        DataFlag = 0,
                        SampleCode = qCJXCYJUnloadCmd.SampleCode,
                        UnloadTime = DateTime.Now,
                        BarrelNumber = pDCYJBarrel.BarrelNumber,
                        BarrelCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        SamplingId = qCJXCYJUnloadCmd.SamplingId,
                    });

                    pDCYJBarrel.SampleCount = 0;
                    pDCYJBarrel.InFactoryBatchId = string.Empty;
                    pDCYJBarrel.SampleCode = string.Empty;
                    pDCYJBarrel.BarrelStatus = eSampleBarrelStatus.空桶.ToString();
                    pDCYJBarrel.UpdateTime = DateTime.Now;
                    pDCYJBarrel.DataFlag = 0;
                 

                    if (this.EquDber.Update(pDCYJBarrel) > 0) OutputRunInfo(rtxtOutput, "卸样完成，罐号：" + pDCYJBarrel.BarrelNumber.ToString() + "  " + pDCYJBarrel.BarrelType.ToString());

                    Thread.Sleep(2000);
                }

                // 更新命令
                qCJXCYJUnloadCmd.ResultCode = eEquInfCmdResultCode.成功.ToString();
                qCJXCYJUnloadCmd.DataFlag = 2;
                this.EquDber.Update(qCJXCYJUnloadCmd);

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
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSampleCmd>() + " set DataFlag=3");
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJUnloadCmd>() + " set DataFlag=3");
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJBarrel>() + " set SampleCode='',InFactoryBatchId='',SampleCount=0,BarrelStatus='" + eSampleBarrelStatus.空桶.ToString() + "'");
        }

        #region 改变系统状态

        private void btnSystemStatus_JXDJ_Click(object sender, EventArgs e)
        {
            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.就绪待机.ToString());
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSignal>() + " set TagValue=@TagValue where  TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.就绪待机.ToString() });
        }

        private void btnSystemStatus_ZZYX_Click(object sender, EventArgs e)
        {
            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.正在运行.ToString());
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSignal>() + " set TagValue=@TagValue where   TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在运行.ToString() });
        }

        private void btnSystemStatus_ZZXY_Click(object sender, EventArgs e)
        {
            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.正在卸样.ToString());
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSignal>() + " set TagValue=@TagValue where  TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在卸样.ToString() });
        }

        private void btnSystemStatus_FSGZ_Click(object sender, EventArgs e)
        {
            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.发生故障.ToString());
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQCJXCYJSignal>() + " set TagValue=@TagValue where  TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.发生故障.ToString() });
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
