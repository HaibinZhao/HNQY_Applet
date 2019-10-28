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
using CMCS.DumblyConcealer.Tasks.AssayDevice;
using CMCS.DumblyConcealer.Enums;

namespace CMCS.DumblyConcealer.Win.DumblyTasks
{
    public partial class FrmAssayDevice : TaskForm
    {

        RTxtOutputer rTxtOutputer;
        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

        public FrmAssayDevice()
        {
            InitializeComponent();
        }

        private void FrmAssayDevice_Load(object sender, EventArgs e)
        {
            this.Text = "化验设备数据同步业务";

            this.rTxtOutputer = new RTxtOutputer(rtxtOutput);

            ExecuteAllTask();
        }

        /// <summary>
        /// 执行所有任务
        /// </summary>
        void ExecuteAllTask()
        {
            //EquAssayDeviceDAO assayDevice_DAO = EquAssayDeviceDAO.GetInstance();
            HNQYEquAssayDeviceDAO assayDevice_DAO = HNQYEquAssayDeviceDAO.GetInstance();

            taskSimpleScheduler.StartNewTask("生成标准测硫仪数据", () =>
            {
                assayDevice_DAO.SaveToSulfurStdAssay(this.rTxtOutputer.Output);

            }, 30000, OutputError);

            taskSimpleScheduler.StartNewTask("生成标准量热仪数据", () =>
            {
                assayDevice_DAO.SaveToHeatStdAssay(this.rTxtOutputer.Output);

            }, 30000, OutputError);

            taskSimpleScheduler.StartNewTask("生成标准水分仪数据", () =>
            {
                assayDevice_DAO.SaveToMoistureStdAssay(this.rTxtOutputer.Output);

            }, 30000, OutputError);

            taskSimpleScheduler.StartNewTask("生成标准工分仪数据", () =>
            {
                assayDevice_DAO.SaveToProximateStdAssay(this.rTxtOutputer.Output);

            }, 30000, OutputError);


            taskSimpleScheduler.StartNewTask("生成元素分析仪数据", () =>
            {
                assayDevice_DAO.SaveToElementStdAssay(this.rTxtOutputer.Output);

            }, 30000, OutputError);

            taskSimpleScheduler.StartNewTask("生成灰融仪数据", () =>
            {
                assayDevice_DAO.SaveToFusionPointStdAssay(this.rTxtOutputer.Output);

            }, 30000, OutputError);

            taskSimpleScheduler.StartNewTask("保存化验室网络信号点", () =>
            {
                assayDevice_DAO.SaveHYSWLSingalData(this.rTxtOutputer.Output);

            }, 20000, OutputError);

        }

        /// <summary>
        /// 输出异常信息
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        void OutputError(string text, Exception ex)
        {
            this.rTxtOutputer.Output(text + Environment.NewLine + ex.Message, eOutputType.Error);
        }

        /// <summary>
        /// 窗体关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmAssayDevice_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 注意：必须取消任务
            this.taskSimpleScheduler.Cancal();
        }
    }
}
