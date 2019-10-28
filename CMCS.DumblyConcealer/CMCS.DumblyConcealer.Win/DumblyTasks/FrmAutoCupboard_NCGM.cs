using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.DumblyConcealer.Win.Core;
using CMCS.Common.Utilities;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Enums;
using CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Entities;
using CMCS.Common.Entities;
using CMCS.Common;
using CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Enums;

namespace CMCS.DumblyConcealer.Win.DumblyTasks
{
    public partial class FrmAutoCupboard_NCGM : TaskForm
    {
        RTxtOutputer rTxtOutputer;
        RTxtOutputer rTxtOutputer2;
        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

        /// <summary>
        /// 最后一次心跳值
        /// </summary>
        string lastHeartbeat;

        public FrmAutoCupboard_NCGM()
        {
            InitializeComponent();
            this.Text = "南昌光明存样柜接口业务";
        }

        private void FrmAutoCupboard_NCGM_Load(object sender, EventArgs e)
        {
            this.rTxtOutputer = new RTxtOutputer(rtxtOutput);
            this.rTxtOutputer2 = new RTxtOutputer(rtxtOutput2);

            ExecuteAllTask();
        }

        String beginname = "存样柜操作";

        /// <summary>
        /// 执行所有任务
        /// </summary>
        void ExecuteAllTask()
        {
            EquAutoCupboard_NCGM_DAO autoCupboard_NCGM_DAO = new EquAutoCupboard_NCGM_DAO(GlobalVars.MachineCode_CYG1, DcDbers.GetInstance().AutoCupboard_Dber);

            this.taskSimpleScheduler.StartNewTask("#1存样柜-快速同步", () =>
            {
                autoCupboard_NCGM_DAO.SyncSignal(this.rTxtOutputer.Output);
                autoCupboard_NCGM_DAO.SyncCYGCmd(this.rTxtOutputer.Output);
                autoCupboard_NCGM_DAO.SyncCYGRecord(this.rTxtOutputer.Output);
                autoCupboard_NCGM_DAO.SyncCYGError(this.rTxtOutputer.Output);
                autoCupboard_NCGM_DAO.SyncMakeDetail(this.rTxtOutputer.Output);//人工制样数据同步,万通在存样柜数据库里面新增了一个制样明细表,他们人工制样的样桶明细写在这个表
            }, 2000, OutputError);

            this.taskSimpleScheduler.StartNewTask("#1存样柜-上位机心跳", () =>
            {
                autoCupboard_NCGM_DAO.SyncHeartbeatSignal();
                autoCupboard_NCGM_DAO.SynCYGInfo(this.rTxtOutputer.Output);
            }, 30000, OutputError);

            EquAutoCupboard_NCGM_DAO autoCupboard_NCGM_DAO2 = new EquAutoCupboard_NCGM_DAO(GlobalVars.MachineCode_CYG2, DcDbers.GetInstance().AutoCupboard_Dber2);

            this.taskSimpleScheduler.StartNewTask("#2存样柜-快速同步", () =>
            {
                autoCupboard_NCGM_DAO2.SyncSignal(this.rTxtOutputer.Output);
                autoCupboard_NCGM_DAO2.SyncCYGCmd(this.rTxtOutputer.Output);
                autoCupboard_NCGM_DAO2.SyncCYGRecord(this.rTxtOutputer.Output);
                autoCupboard_NCGM_DAO2.SyncCYGError(this.rTxtOutputer.Output);
                autoCupboard_NCGM_DAO2.SyncMakeDetail(this.rTxtOutputer.Output);
            }, 2000, OutputError);

            this.taskSimpleScheduler.StartNewTask("#2存样柜-上位机心跳", () =>
            {
                autoCupboard_NCGM_DAO2.SyncHeartbeatSignal();
                autoCupboard_NCGM_DAO2.SynCYGInfo(this.rTxtOutputer.Output);
            }, 30000, OutputError);

            this.taskSimpleScheduler.StartNewTask("弃样业务处理   ", () =>
            {
                if (DateTime.Now.Hour == 5)
                    autoCupboard_NCGM_DAO.SynCYGPut(this.rTxtOutputer2.Output);
            }, 3600000, OutputError);
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
        private void FrmAutoCupboard_NCGM_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 注意：必须取消任务
            this.taskSimpleScheduler.Cancal();
        }

    }
}
