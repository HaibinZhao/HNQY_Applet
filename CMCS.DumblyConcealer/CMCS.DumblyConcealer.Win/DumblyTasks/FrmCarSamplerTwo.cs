using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CMCS.Common.Utilities;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.AutoMaker;
using CMCS.DumblyConcealer.Win.Core;
using CMCS.DumblyConcealer.Tasks.CarJXSampler;
using CMCS.Common;
using CMCS.Common.DAO;

namespace CMCS.DumblyConcealer.Win.DumblyTasks
{
    public partial class FrmCarSamplerTwo : TaskForm
    {
        RTxtOutputer rTxtOutputer;
        RTxtOutputer rTxtOutResultputer;
        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

        public FrmCarSamplerTwo()
        {
            InitializeComponent();
        }

        private void FrmCarSampler_CSKY_Load(object sender, EventArgs e)
        {
            this.Text = "3,4号汽车机械采样机接口业务";

            this.rTxtOutputer = new RTxtOutputer(rtxtOutput);

            ExecuteAllTask();

        }

        /// <summary>
        /// 执行所有任务
        /// </summary>
        void ExecuteAllTask()
        {
            #region #3汽车机械采样机

            EquCarJXSamplerDAO carJXSamplerDAO3 = new EquCarJXSamplerDAO(GlobalVars.MachineCode_QCJXCYJ_3, new DapperDber.Dbs.OracleDb.OracleDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#3汽车机械采样机接口连接字符串")));

            taskSimpleScheduler.StartNewTask("#3汽车机械采样机-快速同步", () =>
            {
                carJXSamplerDAO3.SyncBarrel(this.rTxtOutputer.Output);
                carJXSamplerDAO3.SyncSampleCmd(this.rTxtOutputer.Output);
                carJXSamplerDAO3.SyncJXCYControlUnloadCMD(this.rTxtOutputer.Output);
                carJXSamplerDAO3.SyncUnloadResult(this.rTxtOutputer.Output);
                carJXSamplerDAO3.SyncQCJXCYJError(this.rTxtOutputer.Output);
                carJXSamplerDAO3.SyncSignal(this.rTxtOutputer.Output);
 
            }, 2000, OutputError);

            this.taskSimpleScheduler.StartNewTask("#3汽车机械采样机-上位机心跳", () =>
            {
                carJXSamplerDAO3.SyncHeartbeatSignal();
            }, 30000, OutputError);

            #endregion
            #region #4汽车机械采样机

            EquCarJXSamplerDAO carJXSamplerDAO4 = new EquCarJXSamplerDAO(GlobalVars.MachineCode_QCJXCYJ_4, new DapperDber.Dbs.OracleDb.OracleDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#4汽车机械采样机接口连接字符串")));

            taskSimpleScheduler.StartNewTask("#4汽车机械采样机-快速同步", () =>
            {
                carJXSamplerDAO4.SyncBarrel(this.rTxtOutputer.Output);
                carJXSamplerDAO4.SyncSampleCmd(this.rTxtOutputer.Output);
                carJXSamplerDAO4.SyncJXCYControlUnloadCMD(this.rTxtOutputer.Output);
                carJXSamplerDAO4.SyncUnloadResult(this.rTxtOutputer.Output);
                carJXSamplerDAO4.SyncQCJXCYJError(this.rTxtOutputer.Output);
                carJXSamplerDAO4.SyncSignal(this.rTxtOutputer.Output);

            }, 2000, OutputError);

            this.taskSimpleScheduler.StartNewTask("#4汽车机械采样机-上位机心跳", () =>
            {
                carJXSamplerDAO4.SyncHeartbeatSignal();
            }, 30000, OutputError);

            #endregion
        }

        /// <summary>
        /// 输出异常信息
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        void OutputError(string text, Exception ex)
        {
            this.rTxtOutputer.Output(text + Environment.NewLine + ex.Message, eOutputType.Error);

            Log4Neter.Error(text, ex);
        }

        /// <summary>
        /// 输出异常信息（结果）
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        void OutputResultError(string text, Exception ex)
        {
            this.rTxtOutResultputer.Output(text + Environment.NewLine + ex.Message, eOutputType.Error);

            Log4Neter.Error(text, ex);
        }

        /// <summary>
        /// 窗体关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCarSampler_CSKY_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 注意：必须取消任务
            this.taskSimpleScheduler.Cancal();
        }
    }
}
