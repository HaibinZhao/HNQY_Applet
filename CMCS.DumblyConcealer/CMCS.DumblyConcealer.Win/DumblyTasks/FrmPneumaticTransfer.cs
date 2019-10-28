using System;
using System.Windows.Forms;
using CMCS.DumblyConcealer.Win.Core;
using CMCS.Common.Utilities;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS;

namespace CMCS.DumblyConcealer.Win.DumblyTasks
{
    public partial class FrmPneumaticTransfer : TaskForm
    {
        RTxtOutputer rTxtOutputer;
        RTxtOutputer rTxtOutputer2;
        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

        /// <summary>
        /// 最后一次心跳值
        /// </summary>
        bool lastHeartbeat;

        public FrmPneumaticTransfer()
        {
            InitializeComponent();
        }

        private void FrmAutoCupboard_NCGM_Load(object sender, EventArgs e)
        {
            this.Text = "气动传输调度接口业务";

            this.rTxtOutputer = new RTxtOutputer(rtxtOutput);
            this.rTxtOutputer2 = new RTxtOutputer(rtxtOutput2);

            ExecuteAllTask();
        }

        /// <summary>
        /// 执行所有任务
        /// </summary>
        void ExecuteAllTask()
        {
            PneumaticTransfer_XMJS_DAO pneumaticTransfer_XMJS_DAO = PneumaticTransfer_XMJS_DAO.GetInstance();

            taskSimpleScheduler.StartNewTask("气动站点状态检测", () =>
            {
                pneumaticTransfer_XMJS_DAO.CheckSampleState(this.rTxtOutputer.Output);
                pneumaticTransfer_XMJS_DAO.SyncPnumResultToCYGCmd(this.rTxtOutputer.Output);
            }, 3000, OutputError);

            this.taskSimpleScheduler.StartNewTask("取样、弃样状态检测", () =>
            {
                pneumaticTransfer_XMJS_DAO.CheckCYGCmd(this.rTxtOutputer2.Output);
            }, 3000, OutputError);


            this.taskSimpleScheduler.StartNewTask("气动传输同步", () =>
            {
                //pneumaticTransfer_XMJS_DAO.CheckCYGCmd(this.rTxtOutputer.Output);

                pneumaticTransfer_XMJS_DAO.SyncError(this.rTxtOutputer.Output);
                pneumaticTransfer_XMJS_DAO.SyncSignal(this.rTxtOutputer.Output);
                pneumaticTransfer_XMJS_DAO.SyncTransmissionRecord (this.rTxtOutputer.Output);
                //pneumaticTransfer_XMJS_DAO.SyncTransmissionInterface(this.rTxtOutputer.Output);
            }, 30000, OutputError);
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
        /// 输出异常信息
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        void OutputError2(string text, Exception ex)
        {
            this.rTxtOutputer2.Output(text + Environment.NewLine + ex.Message, eOutputType.Error);

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
