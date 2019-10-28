using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common.Enums;
using CMCS.WeighCheck.DAO;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.Metro;
using CMCS.WeighCheck.MakeWeight.Enums;
using CMCS.Common.Utilities;
using CMCS.WeighCheck.MakeWeight.Frms;
using CMCS.Forms.UserControls;
using CMCS.Common.Entities.Fuel;
using DevComponents.DotNetBar.SuperGrid;

namespace CMCS.WeighCheck.MakeWeight.Frms.SampleWeigth
{
    public partial class FrmMakeWeight : MetroForm
    {
        public FrmMakeWeight()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体唯一标识符
        /// </summary>
        public static string UniqueKey = "FrmMakeWeight";

        #region Vars

        CommonDAO commonDAO = CommonDAO.GetInstance();
        CZYHandlerDAO czyHandlerDAO = CZYHandlerDAO.GetInstance();
        CodePrinter _CodePrinter = null;

        eFlowFlag currentFlowFlag = eFlowFlag.等待扫码;
        /// <summary>
        /// 当前流程标识
        /// </summary>
        public eFlowFlag CurrentFlowFlag
        {
            get { return currentFlowFlag; }
            set
            {
                currentFlowFlag = value;
                lblCurrentFlowFlag.Text = value.ToString();
            }
        }

        string resMessage = string.Empty;

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public void InitFrom()
        {
            this.IsUseWeight = Convert.ToBoolean(commonDAO.GetAppletConfigInt32("启用称重"));
            this._CodePrinter = new CodePrinter(printDocument1);

            // 生成编码按钮
            GridButtonXEditControl btnNewCode = superGridControl1.PrimaryGrid.Columns["gclmNewCode"].EditControl as GridButtonXEditControl;
            btnNewCode.ColorTable = eButtonColor.BlueWithBackground;
            btnNewCode.Click += new EventHandler(btnNewCode_Click);
            // 打印编码按钮
            GridButtonXEditControl btnPrintCode = superGridControl1.PrimaryGrid.Columns["gclmPrintCode"].EditControl as GridButtonXEditControl;
            btnNewCode.ColorTable = eButtonColor.BlueWithBackground;
            btnPrintCode.Click += new EventHandler(btnPrintCode_Click);
        }

        private void FrmMakeWeight_Load(object sender, EventArgs e)
        {
            // 初始化
            InitFrom();
            // 初始化设备
            InitHardware();
        }

        private void FrmMakeWeight_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnloadHardware();
        }

        #region 电子秤

        /// <summary>
        /// 电子秤
        /// </summary>
        WB.TOLEDO.IND231.TOLEDO_IND231Wber wber = new WB.TOLEDO.IND231.TOLEDO_IND231Wber(3);

        bool isUseWeight = true;
        /// <summary>
        /// 启用电子秤
        /// </summary>
        public bool IsUseWeight
        {
            get { return isUseWeight; }
            set
            {
                isUseWeight = value;

                lblWber.Visible = value;
                slightWber.Visible = value;
            }
        }

        bool wbSteady = false;
        /// <summary>
        /// 电子秤仪表稳定状态
        /// </summary>
        public bool WbSteady
        {
            get { return wbSteady; }
            set
            {
                wbSteady = value;
            }
        }

        double wbMinWeight = 0;
        /// <summary>
        /// 电子秤仪表最小称重 单位：吨
        /// </summary>
        public double WbMinWeight
        {
            get { return wbMinWeight; }
            set
            {
                wbMinWeight = value;
            }
        }

        /// <summary>
        /// 重量稳定事件
        /// </summary>
        /// <param name="steady"></param>
        void Wber_OnSteadyChange(bool steady)
        {
            // 仪表稳定状态 
            InvokeEx(() =>
            {
                this.WbSteady = steady;
            });
        }

        /// <summary>
        /// 电子秤状态变化
        /// </summary>
        /// <param name="status"></param>
        void Wber_OnStatusChange(bool status)
        {
            // 接收设备状态 
            InvokeEx(() =>
            {
                slightWber.LightColor = (status ? Color.Green : Color.Red);
            });
        }

        #endregion

        #region 设备初始化与卸载

        /// <summary>
        /// 初始化外接设备
        /// </summary>
        private void InitHardware()
        {
            try
            {
                bool success = false;

                // 初始化-电子秤
                if (IsUseWeight)
                {
                    this.WbMinWeight = commonDAO.GetAppletConfigDouble("电子秤最小重量");

                    wber.OnStatusChange += new WB.TOLEDO.IND231.TOLEDO_IND231Wber.StatusChangeHandler(Wber_OnStatusChange);
                    wber.OnSteadyChange += new WB.TOLEDO.IND231.TOLEDO_IND231Wber.SteadyChangeEventHandler(Wber_OnSteadyChange);
                    success = wber.OpenCom(commonDAO.GetAppletConfigInt32("电子秤串口"), commonDAO.GetAppletConfigInt32("电子秤波特率"), commonDAO.GetAppletConfigInt32("电子秤数据位"), commonDAO.GetAppletConfigInt32("电子秤停止位"));
                }
            }
            catch (Exception ex)
            {
                Log4Neter.Error("设备初始化", ex);
            }
        }

        /// <summary>
        /// 卸载设备
        /// </summary>
        private void UnloadHardware()
        {
            // 注意此段代码
            Application.DoEvents();

            try
            {
                wber.CloseCom();
            }
            catch { }
        }
        #endregion

        #region 业务

        /// <summary>
        ///  重置流程信息
        /// </summary>
        private void Restet()
        {
            this.CurrentFlowFlag = eFlowFlag.等待扫码;

            txtInputMakeCode.ResetText();
            rtxtMakeWeightInfo.ResetText();

            // 方便客户快速使用，获取焦点
            txtInputMakeCode.Focus();
        }

        #endregion

        #region 操作

        /// <summary>
        /// 键入Enter检测有效性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInputMakeCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(txtInputMakeCode.Text.Trim()))
                {
                    LoadRCMakeDetail();
                }
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMakeWeightCode_ButtonCustomClick(object sender, EventArgs e)
        {
            Restet();
        }

        /// <summary>
        /// 加载制样明细记录
        /// </summary>
        private void LoadRCMakeDetail()
        {
            List<CmcsRCMakeDetail> rCMakeDetails = czyHandlerDAO.GetRCMakeDetails(txtInputMakeCode.Text.Trim(), out resMessage);
            if (rCMakeDetails.Count == 0)
            {
                ShowMessage(resMessage, eOutputType.Error);

                txtInputMakeCode.ResetText();
            }
            else
            {
                this.CurrentFlowFlag = eFlowFlag.样品登记;
            }

            superGridControl1.PrimaryGrid.DataSource = rCMakeDetails;
        }

        /// <summary>
        /// 生成编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNewCode_Click(object sender, EventArgs e)
        {
            GridButtonXEditControl btn = sender as GridButtonXEditControl;
            if (btn == null) return;

            CmcsRCMakeDetail rCMakeDetail = btn.EditorCell.GridRow.DataItem as CmcsRCMakeDetail;
            if (rCMakeDetail == null) return;

            if (!string.IsNullOrEmpty(rCMakeDetail.BarrelCode) && MessageBoxEx.Show("样罐编码已存在，确定要重新生成？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // 生成随机样罐编码
            string newBarrelCode = commonDAO.CreateNewMakeBarrelCode(DateTime.Now);
            // 称重校验
            #region 原称重代码，取电子秤数据
            //if (IsUseWeight)
            //{
            //    if (wber.Status && wber.Weight > 0 && wber.Weight > WbMinWeight)
            //    {
            //        rCMakeDetail.BarrelCode = newBarrelCode;
            //        rCMakeDetail.Weight = wber.Weight;

            //        czyHandlerDAO.UpdateMakeDetailWeightAndBarrelCode(rCMakeDetail.Id, wber.Weight, newBarrelCode);
            //    }
            //    else
            //        MessageBoxEx.Show("未检测到重量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //// 不称重校验
            //else
            //{
            //    rCMakeDetail.BarrelCode = newBarrelCode;

            //    czyHandlerDAO.UpdateMakeDetailWeightAndBarrelCode(rCMakeDetail.Id, 0, newBarrelCode);
            //}
            #endregion

            #region 临时的手动称重，接入电子秤后可还原
            if (IsUseWeight)
            {
                czyHandlerDAO.UpdateMakeDetailWeightAndBarrelCode(rCMakeDetail.Id, rCMakeDetail.Weight, newBarrelCode);
            }
            // 不称重校验
            else
            {
                rCMakeDetail.BarrelCode = newBarrelCode;
                czyHandlerDAO.UpdateMakeDetailWeightAndBarrelCode(rCMakeDetail.Id, 0, newBarrelCode);
            }
            //rCMakeDetail.TheRCMake
            List<CmcsRCMakeDetail> rCMakeDetails = commonDAO.SelfDber.Entities<CmcsRCMakeDetail>(" where MakeId=:MakeId order by CreateDate asc", new { MakeId = rCMakeDetail.TheRCMake.Id });
            superGridControl1.PrimaryGrid.DataSource = rCMakeDetails;
            #endregion
        }

        /// <summary>
        /// 打印编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnPrintCode_Click(object sender, EventArgs e)
        {
            GridButtonXEditControl btn = sender as GridButtonXEditControl;
            if (btn == null) return;

            CmcsRCMakeDetail rCMakeDetail = btn.EditorCell.GridRow.DataItem as CmcsRCMakeDetail;
            if (rCMakeDetail == null) return;

            if (!string.IsNullOrEmpty(rCMakeDetail.BarrelCode))
            //_CodePrinter.Print(rCMakeDetail.BarrelCode);
            {

                FrmMakeWeight_Print print = new FrmMakeWeight_Print(rCMakeDetail.BarrelCode);
                print.ShowDialog();
            }
            else
                MessageBoxEx.Show("请先点击[生成]按钮生成样罐编码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void superGridControl1_BeginEdit(object sender, GridEditEventArgs e)
        {
            // 取消编辑
            e.Cancel = false;
        }

        #endregion

        #region 其他

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
        /// 输出信息类型
        /// </summary>
        public enum eOutputType
        {
            /// <summary>
            /// 普通
            /// </summary>
            [Description("#BD86FA")]
            Normal,
            /// <summary>
            /// 重要
            /// </summary>
            [Description("#A50081")]
            Important,
            /// <summary>
            /// 警告
            /// </summary>
            [Description("#F9C916")]
            Warn,
            /// <summary>
            /// 错误
            /// </summary>
            [Description("#DB2606")]
            Error
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
