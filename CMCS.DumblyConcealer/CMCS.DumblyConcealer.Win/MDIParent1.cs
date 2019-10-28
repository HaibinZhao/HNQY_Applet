using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.DumblyConcealer.Win.DumblyTasks;
using CMCS.DumblyConcealer.Win.Core;
using BasisPlatform.Util;

namespace CMCS.DumblyConcealer.Win
{
    public partial class MDIParent1 : Form
    {
        public MDIParent1()
        {
            InitializeComponent();
        }

        #region 窗口

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        #endregion

        private void MDIParent1_Load(object sender, EventArgs e)
        {

        }

        private void MDIParent1_Shown(object sender, EventArgs e)
        {
            BasisPlatformUtil.StartNewTask("开机延迟启动", () =>
            {
                int minute = 5, surplus = minute;

                while (minute > 0)
                {
                    double d = minute - Environment.TickCount / 1000 / 60;
                    if (Environment.TickCount < 0 || d <= 0) break;

                    System.Threading.Thread.Sleep(60000);

                    surplus--;
                }
#if DEBUG

#else
                this.InvokeEx(() => { timer1.Enabled = true; });
#endif
            });
        }

        private void MDIParent1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("确认退出系统？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                }
                else
                {
                    e.Cancel = true;
                }
            }
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

        /// <summary>
        /// 任务索引
        /// </summary>
        int taskFormIndex = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (taskFormIndex)
            {
                case 0:
                    tsmiOpenFrmWeightBridger_Click(null, null);
                    break;
                case 1:
                    tsmiOpenFrmBeltSampler_NCGM_Click(null, null);
                    break;
                case 2:
                    tsmiOpenFrmAutoMaker_NCGM_Click(null, null);
                    break;
                case 3:
                    tsmiOpenFrmAutoCupboard_NCGM_Click(null, null);
                    break;
                case 5:
                    tsmiOpenFrmAssayDevice_Click(null, null);
                    break;
                case 6:
                    tsmiOpenFrmTrainWeight_Click(null, null);
                    break;
                case 7:
                    tsmiOpenFrmCarSampler_CSKY_Click(null, null);
                    break;
                case 8:
                    tsmiOpenFrmCarSampler_CSKY2_Click(null, null);
                    break;
                case 9:
                    tsmiOpenFrmCarSynchronous_Click(null, null);
                    break;
                case 10:
                    tsmiOpenFrmPnem_Click(null, null);
                    break;
            }

            if (taskFormIndex == 10)
            {
                TileHorizontalToolStripMenuItem_Click(null, null);
                timer1.Stop();
            }

            taskFormIndex++;
        }

        /// <summary>
        /// 01.同步轨道衡数据、入厂车号识别数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmWeightBridger_Click(object sender, EventArgs e)
        {
            new FrmWeightBridger
            {
                MdiParent = this
            }.Show();
        }

        /// <summary>
        /// 02.皮带采样机接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmBeltSampler_NCGM_Click(object sender, EventArgs e)
        {
            new FrmBeltSampler
            {
                MdiParent = this
            }.Show();
        }

        /// <summary>
        /// 03.全自动制样机接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmAutoMaker_NCGM_Click(object sender, EventArgs e)
        {
            new FrmAutoMaker
            {
                MdiParent = this
            }.Show();
        }

        /// <summary>
        /// 04.智能存样柜接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmAutoCupboard_NCGM_Click(object sender, EventArgs e)
        {
            new FrmAutoCupboard_NCGM
            {
                MdiParent = this
            }.Show();
        }

        /// <summary>
        /// 05.化验设备数据读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmAssayDevice_Click(object sender, EventArgs e)
        {
            new FrmAssayDevice
            {
                MdiParent = this
            }.Show();
        }

        /// <summary>
        /// 06.车号识别报文TCP/IP同步业务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmTrainWeight_Click(object sender, EventArgs e)
        {
            new FrmTrainDiscriminator
            {
                MdiParent = this
            }.Show();
        }

        /// <summary>
        /// 07.1,2汽车采样机接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmCarSampler_CSKY_Click(object sender, EventArgs e)
        {
            new FrmCarSampler
            {
                MdiParent = this
            }.Show();
        }

        /// <summary>
        /// 08.3,4汽车采样机接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmCarSampler_CSKY2_Click(object sender, EventArgs e)
        {
            new FrmCarSamplerTwo
            {
                MdiParent = this
            }.Show();
        }

        /// <summary>
        /// 09.综合事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpenFrmCarSynchronous_Click(object sender, EventArgs e)
        {
            new FrmDataHandler
            {
                MdiParent = this
            }.Show();
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FrmAutoCupBoard_NCGM_Test
            {
                MdiParent = this
            }.Show();
        }

        private void tsmiOpenFrmPnem_Click(object sender, EventArgs e)
        {
            new FrmPneumaticTransfer
            {
                MdiParent = this
            }.Show();
        }

      
    }
}
