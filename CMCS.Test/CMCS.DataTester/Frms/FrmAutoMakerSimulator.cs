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
using CMCS.DumblyConcealer.Tasks.AutoMaker.Entities;
using CMCS.Common.Entities.iEAA;

namespace CMCS.DataTester.Frms
{
    public partial class FrmAutoMakerSimulator : Form
    {
        SqlServerDapperDber dber1 = new SqlServerDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#1全自动制样机接口连接字符串"));
        SqlServerDapperDber dber2 = new SqlServerDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#2全自动制样机接口连接字符串"));

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

        public FrmAutoMakerSimulator()
        {
            InitializeComponent();
        }

        private void FrmBeltSamplerSimulator_Load(object sender, EventArgs e)
        {
            CreateMainTask();
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
                this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = GlobalVars.EquHeartbeatName, TagValue = DateTime.Now.ToString() });
                // 更新采样计划
                this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJPlan>() + " set DataFlag=1 where DataFlag=0");

                // 控制命令
                EquQZDZYJCmd pDCYJCmd = this.EquDber.Entity<EquQZDZYJCmd>("where DataFlag=0 order by CreateDate desc");
                if (pDCYJCmd != null)
                {
                    CmdHandle(pDCYJCmd);

                    autoResetEvent.WaitOne();
                }

            }, 3000);
        }

        private void CmdHandle(EquQZDZYJCmd input)
        {
            Task task = new Task((state) =>
            {
                EquQZDZYJCmd pDCYJCmd = state as EquQZDZYJCmd;
                OutputRunInfo(rtxtOutput, "处理命令，命令代码：" + pDCYJCmd.CmdCode + "  制样码：" + pDCYJCmd.MakeCode);

                if (pDCYJCmd.CmdCode == eEquInfMakerCmd.开始制样.ToString())
                {
                    EquQZDZYJPlan pDCYJPlan = this.EquDber.Entity<EquQZDZYJPlan>("where MakeCode=@MakeCode", new { MakeCode = pDCYJCmd.MakeCode });
                    if (pDCYJPlan != null)
                    {
                        Thread.Sleep(3000);
                        OutputRunInfo(rtxtOutput, "启动制样机");

                        // 更新系统状态为正在运行
                        this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在运行.ToString() });

                        // 生成出样记录
                        foreach (CodeContent codeContent in CommonDAO.GetInstance().GetCodeContentByKind("样品类型"))
                        {
                            EquQZDZYJDetail detail = new EquQZDZYJDetail()
                              {
                                  BarrelCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                                  DataFlag = 0,
                                  StartTime = DateTime.Now.AddSeconds(3),
                                  EndTime = DateTime.Now,
                                  MakeCode = pDCYJCmd.MakeCode,
                                  MakeUser = "自动",
                                  YPType = codeContent.Code,
                                  YPWeight = 500
                              };
                            if (this.EquDber.Insert(detail) > 0)
                                OutputRunInfo(rtxtOutput, "制出样品：" + codeContent.Code);
                            Thread.Sleep(2000);

                        }

                        // 更新命令
                        pDCYJCmd.ResultCode = (int)eEquInfCmdResultCode.成功;
                        pDCYJCmd.DataFlag = 2;
                        this.EquDber.Update(pDCYJCmd);

                        // 更新系统状态为就绪待机
                        this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.就绪待机.ToString() });
                    }
                    else
                    {
                        OutputRunInfo(rtxtOutput, "未找到制样计划，制样码：" + pDCYJCmd.MakeCode);

                        pDCYJCmd.ResultCode = (int)eEquInfCmdResultCode.失败;
                        pDCYJCmd.DataFlag = 1;
                        this.EquDber.Update(pDCYJCmd);
                    }
                }
                else if (pDCYJCmd.CmdCode == eEquInfMakerCmd.停止制样.ToString())
                {
                    Thread.Sleep(3000);
                    OutputRunInfo(rtxtOutput, "停止制样机");

                    // 更新系统状态为就绪待机
                    this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.就绪待机.ToString() });

                    // 更新命令
                    pDCYJCmd.ResultCode = (int)eEquInfCmdResultCode.成功;
                    pDCYJCmd.DataFlag = 2;
                    this.EquDber.Update(pDCYJCmd);
                }

                autoResetEvent.Set();

            }, input);
            task.Start();
        }

        /// <summary>
        /// 重置所有接口表
        /// </summary>
        void ResetAll()
        {
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJCmd>() + " set DataFlag=3");
        }

        #region 改变系统状态

        private void btnSystemStatus_JXDJ_Click(object sender, EventArgs e)
        {
            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.就绪待机.ToString());
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.就绪待机.ToString() });
        }

        private void btnSystemStatus_ZZYX_Click(object sender, EventArgs e)
        {
            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.正在运行.ToString());
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.正在运行.ToString() });
        }

        private void btnSystemStatus_FSGZ_Click(object sender, EventArgs e)
        {
            OutputRunInfo(rtxtOutput, "系统状态更改为" + eEquInfSamplerSystemStatus.发生故障.ToString());
            this.EquDber.Execute("update " + EntityReflectionUtil.GetTableName<EquQZDZYJSignal>() + " set TagValue=@TagValue where TagName=@TagName", new { TagName = eSignalDataName.系统.ToString(), TagValue = eEquInfSamplerSystemStatus.发生故障.ToString() });
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
