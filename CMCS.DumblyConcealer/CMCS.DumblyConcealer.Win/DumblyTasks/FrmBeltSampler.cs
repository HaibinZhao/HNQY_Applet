using System;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.Utilities;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.BeltSampler;
using CMCS.DumblyConcealer.Win.Core;

namespace CMCS.DumblyConcealer.Win.DumblyTasks
{
	public partial class FrmBeltSampler : TaskForm
	{
		RTxtOutputer rTxtOutputer;
		TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

		public FrmBeltSampler()
		{
			InitializeComponent();
		}

		private void FrmBeltSampler_NCGM_Load(object sender, EventArgs e)
		{
			this.Text = "皮带采样机接口业务";

			this.rTxtOutputer = new RTxtOutputer(rtxtOutput);

			ExecuteAllTask();
		}

		/// <summary>
		/// 执行所有任务
		/// </summary>
		void ExecuteAllTask()
		{
			#region #1皮带采样机
			taskSimpleScheduler.StartNewTask("快速同步", () =>
				{
					EquBeltSamplerDAO beltSamplerDAO = new EquBeltSamplerDAO(GlobalVars.MachineCode_PDCYJ_1, DcDbers.GetInstance().BeltSampler_Dber1);

					beltSamplerDAO.SyncSignal(this.rTxtOutputer.Output);
					beltSamplerDAO.SyncError(this.rTxtOutputer.Output);
					beltSamplerDAO.SyncBarrel(this.rTxtOutputer.Output);
					beltSamplerDAO.SyncPlan(this.rTxtOutputer.Output);
					beltSamplerDAO.SyncUnloadCmd(this.rTxtOutputer.Output);
					beltSamplerDAO.SyncCmd(this.rTxtOutputer.Output);
					beltSamplerDAO.SyncUnloadResult(this.rTxtOutputer.Output);

				}, 3000, OutputError);

			this.taskSimpleScheduler.StartNewTask("同步上位机运行状态-心跳", () =>
			{
				EquBeltSamplerDAO beltSamplerDAO = new EquBeltSamplerDAO(GlobalVars.MachineCode_PDCYJ_1, DcDbers.GetInstance().BeltSampler_Dber1);

				beltSamplerDAO.SyncHeartbeatSignal();

			}, 30000, OutputError);
			#endregion

			#region #2皮带采样机
			taskSimpleScheduler.StartNewTask("快速同步", () =>
			{
				EquBeltSamplerDAO beltSamplerDAO = new EquBeltSamplerDAO(GlobalVars.MachineCode_PDCYJ_2, DcDbers.GetInstance().BeltSampler_Dber2);

				beltSamplerDAO.SyncSignal(this.rTxtOutputer.Output);
				beltSamplerDAO.SyncError(this.rTxtOutputer.Output);
				beltSamplerDAO.SyncBarrel(this.rTxtOutputer.Output);
				beltSamplerDAO.SyncPlan(this.rTxtOutputer.Output);
				beltSamplerDAO.SyncUnloadCmd(this.rTxtOutputer.Output);
				beltSamplerDAO.SyncCmd(this.rTxtOutputer.Output);
				beltSamplerDAO.SyncUnloadResult(this.rTxtOutputer.Output);

			}, 3000, OutputError);

			this.taskSimpleScheduler.StartNewTask("同步上位机运行状态-心跳", () =>
			{
				EquBeltSamplerDAO beltSamplerDAO = new EquBeltSamplerDAO(GlobalVars.MachineCode_PDCYJ_2, DcDbers.GetInstance().BeltSampler_Dber2);

				beltSamplerDAO.SyncHeartbeatSignal();

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
		/// 窗体关闭后
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FrmBeltSampler_NCGM_FormClosed(object sender, FormClosedEventArgs e)
		{
			// 注意：必须取消任务
			this.taskSimpleScheduler.Cancal();
		}
	}
}
