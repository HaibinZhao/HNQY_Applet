using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
//
using CMCS.Common.DAO;
using CMCS.WeighCheck.DAO;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.Metro;
using CMCS.Common.Utilities;
using CMCS.Common.Entities.Fuel;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.WeighCheck.MakeChange.Enums;

using ThoughtWorks.QRCode.Codec;
using CMCS.Common.Entities.AutoCupboard;
using CMCS.WeighCheck.MakeChange.Utilities;
using System.Threading;
using CMCS.Common.Enums;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common;
using CMCS.Common.Enums.AutoCupboard;

namespace CMCS.WeighCheck.MakeChange.Frms
{
    public partial class FrmAutoCupboard : MetroForm
    {
        public FrmAutoCupboard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体唯一标识符
        /// </summary>
        public static string UniqueKey = "FrmAutoCupboard";

        #region 业务处理类

        CommonDAO commonDAO = CommonDAO.GetInstance();
        CZYHandlerDAO czyHandlerDAO = CZYHandlerDAO.GetInstance();
        AutoCupboardDAO autoCupboardDAO = AutoCupboardDAO.GetInstance();

        TaskSimpleScheduler taskSimpleScheduler = new TaskSimpleScheduler();

        System.Threading.AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        #endregion

        #region Vars
        bool IsWorking;

        string resMessage = string.Empty;
        string sqlWhere = "where 1=1 ";
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public void InitFrom()
        {
            // 生成取样按钮
            GridButtonXEditControl btnNewCode = superGridControl1.PrimaryGrid.Columns["gclmTakeOut"].EditControl as GridButtonXEditControl;
            btnNewCode.ColorTable = eButtonColor.BlueWithBackground;
            btnNewCode.Click += new EventHandler(btnTake);
            // 生成弃样按钮
            GridButtonXEditControl btnPrintCode = superGridControl1.PrimaryGrid.Columns["gclmPutAway"].EditControl as GridButtonXEditControl;
            btnNewCode.ColorTable = eButtonColor.BlueWithBackground;
            btnPrintCode.Click += new EventHandler(btnPut);
        }

        private void FrmMakeWeight_Load(object sender, EventArgs e)
        {
            // 初始化
            InitFrom();
            dtpStartTime.Value = DateTime.Now.AddDays(-3);
            dtpEndTime.Value = DateTime.Now;
            btnSearch_Click(null, null);
            timer1_Tick(null, null);
        }

        private void FrmMakeWeight_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private void BindData()
        {
            IList<InfCYGSam> cyg = commonDAO.SelfDber.Entities<InfCYGSam>(sqlWhere + " and Code is not null order by MachineCode,CellIndex,ColumnIndex ");
            superGridControl1.PrimaryGrid.DataSource = cyg;
        }

        /// <summary>
        /// 取样
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTake(object sender, EventArgs e)
        {
            GridButtonXEditControl btn = sender as GridButtonXEditControl;
            if (btn == null) return;
            if (MessageBoxEx.Show("确定发送取样命令？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                InfCYGSam entity = btn.EditorCell.GridRow.DataItem as InfCYGSam;
                if (entity != null)
                {
                    if (entity.MachineCode == GlobalVars.MachineCode_CYG1 && slightCYG.LightColor != EquipmentStatusColors.BeReady)
                    {
                        MessageBoxEx.Show(string.Format("{0}未就绪!", entity.MachineCode), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (entity.MachineCode == GlobalVars.MachineCode_CYG2 && slightCYG2.LightColor != EquipmentStatusColors.BeReady)
                    {
                        MessageBoxEx.Show(string.Format("{0}未就绪!", entity.MachineCode), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    SendCYGCmd(entity.Code, entity.MachineCode, eCZPLX.取样_气动口, SelfVars.LoginUser.UserName);
                }
            }
        }

        /// <summary>
        /// 弃样
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPut(object sender, EventArgs e)
        {
            GridButtonXEditControl btn = sender as GridButtonXEditControl;
            if (btn == null) return;
            if (MessageBoxEx.Show("确定发送弃样命令？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                InfCYGSam entity = btn.EditorCell.GridRow.DataItem as InfCYGSam;
                if (entity != null)
                {
                    SendCYGCmd(entity.Code, entity.MachineCode, eCZPLX.弃样, SelfVars.LoginUser.UserName);
                }
            }
        }


        /// <summary>
        /// 发送存样柜命令并监听执行结果
        /// </summary> 
        /// <returns></returns>
        private void SendCYGCmd(string MakeCode, string machineCode, eCZPLX OperType, string OperUser)
        {
            taskSimpleScheduler = new TaskSimpleScheduler();

            autoResetEvent.Reset();

            taskSimpleScheduler.StartNewTask("存样柜命令", () =>
            {
                this.IsWorking = true;

                // 发送取样命令
                if (autoCupboardDAO.SaveAutoCupboardCmd(MakeCode, machineCode, OperType, OperUser))
                {
                    ShowMessage(string.Format("{0}命令发送成功,等待存样柜执行", OperType), eOutputType.Normal);

                    int waitCount = 0;
                    eEquInfCYGCmdResultCode equInfCmdResultCode = eEquInfCYGCmdResultCode.默认;
                    do
                    {
                        Thread.Sleep(10000);
                        if (waitCount % 5 == 0) ShowMessage("等待执行...执行结果:" + equInfCmdResultCode.ToString().Replace("默认", "执行中"), eOutputType.Normal);
                        waitCount++;

                        // 获取卸样命令的执行结果
                        equInfCmdResultCode = autoCupboardDAO.GetAutoCupboardResult(MakeCode);
                    }
                    while (equInfCmdResultCode == eEquInfCYGCmdResultCode.气动执行成功);
                    ShowMessage(equInfCmdResultCode.ToString(), eOutputType.Important);
                }
                else
                {
                    ShowMessage("存样柜命令发送失败", eOutputType.Error);
                }

                autoResetEvent.Set();
            });
        }

        private void ShowMessage(string info, eOutputType outputType)
        {
            OutputRunInfo(rtxtMakeWeightInfo, info, outputType);
        }

        /// <summary>
        /// 输出运行信息
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="text"></param>
        /// <param name="outputType"></param>
        private void OutputRunInfo(RichTextBoxEx richTextBox, string text, eOutputType outputType = eOutputType.Normal)
        {
            this.Invoke((EventHandler)(delegate
            {
                if (richTextBox.TextLength > 100000) richTextBox.Clear();

                text = string.Format("{0}  {1}", DateTime.Now.ToString("HH:mm:ss"), text);

                richTextBox.SelectionStart = richTextBox.TextLength;

                switch (outputType)
                {
                    case eOutputType.Normal:
                        richTextBox.SelectionColor = ColorTranslator.FromHtml("#BD86FA");
                        break;
                    case eOutputType.Important:
                        richTextBox.SelectionColor = ColorTranslator.FromHtml("#A50081");
                        break;
                    case eOutputType.Warn:
                        richTextBox.SelectionColor = ColorTranslator.FromHtml("#F9C916");
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

            }));
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMakeCode.Text))
                sqlWhere += " and Code like '%" + txtMakeCode.Text + "%'";
            if (dtpStartTime.Value.Year > 2000)
                sqlWhere += " and UpdateTime >= '" + dtpStartTime.Value + "'";
            if (dtpEndTime.Value.Year > 2000)
                sqlWhere += " and UpdateTime < '" + dtpEndTime.Value.AddDays(1) + "'";
            BindData();
        }

        /// <summary>
        /// 全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAll_Click(object sender, EventArgs e)
        {
            sqlWhere = "where 1=1 ";
            txtMakeCode.ResetText();
            BindData();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btnSearch_Click(null, null);
            RefreshEquStatus();
        }


        /// <summary>
        /// 更新设备状态
        /// </summary>
        private void RefreshEquStatus()
        {
            string systemStatus = commonDAO.GetSignalDataValue(GlobalVars.MachineCode_CYG1, eSignalDataName.总体状态.ToString());

            if (systemStatus == ((int)eEquInfSystemStatus.就绪待机).ToString())
            {
                slightCYG.LightColor = EquipmentStatusColors.BeReady;
            }
            else if (systemStatus == ((int)eEquInfSystemStatus.正在运行).ToString())
            {
                slightCYG.LightColor = EquipmentStatusColors.Working;
            }
            else if (systemStatus == ((int)eEquInfSystemStatus.发生故障).ToString())
            {
                slightCYG.LightColor = EquipmentStatusColors.Breakdown;
            }

            systemStatus = commonDAO.GetSignalDataValue(GlobalVars.MachineCode_CYG2, eSignalDataName.总体状态.ToString());

            if (systemStatus == ((int)eEquInfSystemStatus.就绪待机).ToString())
            {
                slightCYG2.LightColor = EquipmentStatusColors.BeReady;
            }
            else if (systemStatus == ((int)eEquInfSystemStatus.正在运行).ToString())
            {
                slightCYG.LightColor = EquipmentStatusColors.Working;
            }
            else if (systemStatus == ((int)eEquInfSystemStatus.发生故障).ToString())
            {
                slightCYG.LightColor = EquipmentStatusColors.Breakdown;
            }
        }

        #region DataGridView
        private void superGridControl1_GetRowHeaderText(object sender, GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.Index + 1).ToString();
        }

        private void superGridControl1_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                InfCYGSam entity = gridRow.DataItem as InfCYGSam;
            }
        }
        #endregion
    }
}
