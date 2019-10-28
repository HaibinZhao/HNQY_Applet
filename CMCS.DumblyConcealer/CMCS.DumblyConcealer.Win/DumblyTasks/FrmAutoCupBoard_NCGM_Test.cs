using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CMCS.DumblyConcealer.Win.Core;
using CMCS.Common.Utilities;
using CMCS.Common.Entities;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Entities;
using CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Enums;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Enums;
using CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities;
using CMCS.Common;
using CMCS.Common.Entities.AutoCupboard;
using CMCS.Common.DAO;
using CMCS.Common.Enums.AutoCupboard;
using DevComponents.DotNetBar;

namespace CMCS.DumblyConcealer.Win.DumblyTasks
{
    public partial class FrmAutoCupBoard_NCGM_Test : TaskForm
    {
        RTxtOutputer rTxtOutputer;
        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();
        PneumaticTransfer_XMJS_DAO pneumaticTransfer_XMJS_DAO = PneumaticTransfer_XMJS_DAO.GetInstance();
        public FrmAutoCupBoard_NCGM_Test()
        {
            InitializeComponent();

            this.Text = "南昌光明存样柜接口业务测试";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void FrmAutoCupBoard_NCGM_Test_Load(object sender, EventArgs e)
        {
            this.rTxtOutputer = new RTxtOutputer(rtxtOutput);

            //加载操作票类型
            cmbCZPLX.DataSource = Enum.GetNames(typeof(eCZPLX));

            //加载存样柜
            cmbCYG.DataSource = new string[] { GlobalVars.MachineCode_CYG1, GlobalVars.MachineCode_CYG2 };
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

        /// <summary>
        /// 发送存样柜命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendgCYGCmd_Click(object sender, EventArgs e)
        {
            eCZPLX cZType;
            Enum.TryParse<eCZPLX>(cmbCZPLX.Text, out cZType);
            AutoCupboardDAO.GetInstance().SaveAutoCupboardCmd(txtMakeCode.Text, cmbCYG.Text, cZType, "测试");
        }

        /// <summary>
        /// 发送气动命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendPunmCmd_Click(object sender, EventArgs e)
        {
            pneumaticTransfer_XMJS_DAO.SendQDCmd(pneumaticTransfer_XMJS_DAO.GetLisIndexByDeviceName(cmbStartPlace_Punm.Text), pneumaticTransfer_XMJS_DAO.GetLisIndexByDeviceName(cmbEndPlace_Punm.Text), txt_MakeCode_Pnum.Text);
        }
    }
}