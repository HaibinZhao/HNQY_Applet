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
using CMCS.Common;
using CMCS.Common.DAO;

namespace CMCS.DumblyConcealer.Win.DumblyTasks
{
    public partial class FrmAutoMaker : TaskForm
    {
        RTxtOutputer rTxtOutputer;

        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

        public FrmAutoMaker()
        {
            InitializeComponent();
        }

        private void FrmAutoMaker_NCGM_Load(object sender, EventArgs e)
        {
            this.Text = "全自动制样机接口业务";

            this.rTxtOutputer = new RTxtOutputer(rtxtOutput);

            ExecuteAllTask();

        }

        /// <summary>
        /// 执行所有任务
        /// </summary>
        void ExecuteAllTask()
        {
            #region #1全自动制样机

            EquAutoMakerDAO autoMakerDAO1 = new EquAutoMakerDAO(GlobalVars.MachineCode_QZDZYJ_1, new DapperDber.Dbs.SqlServerDb.SqlServerDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#1全自动制样机接口连接字符串")));
            taskSimpleScheduler.StartNewTask("#1全自动制样机-快速同步", () =>
            {
                autoMakerDAO1.SyncPlan(this.rTxtOutputer.Output);
                autoMakerDAO1.SyncCmd(this.rTxtOutputer.Output);
                autoMakerDAO1.SyncError(this.rTxtOutputer.Output);
                autoMakerDAO1.SyncSignal(this.rTxtOutputer.Output);
                autoMakerDAO1.SyncMakeDetail(this.rTxtOutputer.Output);

            }, 2000, OutputError);

            this.taskSimpleScheduler.StartNewTask("#1全自动制样机-同步上位机心跳", () =>
            {
                autoMakerDAO1.SyncHeartbeatSignal();
            }, 30000, OutputError);

            #endregion

            #region #2全自动制样机

            EquAutoMakerDAO autoMakerDAO2 = new EquAutoMakerDAO(GlobalVars.MachineCode_QZDZYJ_2, new DapperDber.Dbs.SqlServerDb.SqlServerDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#2全自动制样机接口连接字符串")));
            taskSimpleScheduler.StartNewTask("#2全自动制样机-快速同步", () =>
            {
                autoMakerDAO2.SyncPlan(this.rTxtOutputer.Output);
                autoMakerDAO2.SyncCmd(this.rTxtOutputer.Output);
                autoMakerDAO2.SyncError(this.rTxtOutputer.Output);
                autoMakerDAO2.SyncSignal(this.rTxtOutputer.Output);
                autoMakerDAO2.SyncMakeDetail(this.rTxtOutputer.Output);

            }, 2000, OutputError);

            this.taskSimpleScheduler.StartNewTask("#2全自动制样机-同步上位机心跳", () =>
            {
                autoMakerDAO2.SyncHeartbeatSignal();
            }, 30000, OutputError);

            #endregion


            #region 人工制样芯片写桶
            EquAutoMakerDAO RGMakerDAO1 = new EquAutoMakerDAO(GlobalVars.MachineCode_QZDZYJ_1, new DapperDber.Dbs.SqlServerDb.SqlServerDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#1人工制样芯片写码数据库")));
            taskSimpleScheduler.StartNewTask("#1人工制样-快速同步", () =>
            {
                RGMakerDAO1.SyncMakeDetail(this.rTxtOutputer.Output);

            }, 2000, OutputError);

            EquAutoMakerDAO RGMakerDAO2 = new EquAutoMakerDAO(GlobalVars.MachineCode_QZDZYJ_1, new DapperDber.Dbs.SqlServerDb.SqlServerDapperDber(CommonDAO.GetInstance().GetCommonAppletConfigString("#2人工制样芯片写码数据库")));
            taskSimpleScheduler.StartNewTask("#2人工制样-快速同步", () =>
            {
                RGMakerDAO2.SyncMakeDetail(this.rTxtOutputer.Output);

            }, 2000, OutputError);
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
        /// 窗体关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmAutoMaker_NCGM_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 注意：必须取消任务
            this.taskSimpleScheduler.Cancal();
        }
    }
}
