using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.TrainTipper.Utilities;
using CMCS.Common.Entities;
using DevComponents.DotNetBar;
using CMCS.Common.DAO;
using CMCS.Common.Enums;
using CMCS.Common;
using CMCS.TrainTipper.DAO;
using CMCS.TrainTipper.Frms;
using CMCS.Common.Entities.BaseInfo; 

namespace CMCS.TrainTipper
{
    public partial class Form1 : DevComponents.DotNetBar.Metro.MetroForm
    {
        public static SuperTabControlManager superTabControlManager;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "版本：" + new AU.Updater().Version;

            superTabControlManager = new SuperTabControlManager(superTabControl1);

            CreateTrainTipperTab(); 

            timer1.Enabled = true;
        }

        /// <summary>
        /// 创建翻车机选项卡
        /// </summary>
        private void CreateTrainTipperTab()
        {
            superTabControl1.SuspendLayout();

            // 获取翻车机编码
            List<CmcsCMEquipment> trainTippers = TrainTipperDAO.GetInstance().GetTrainTippers();
            // 车号识别设备编码，跟翻车机一一对应
            string[] carriageRecognitionerMachineCodes = CommonDAO.GetInstance().GetAppletConfigString("翻车机车号识别编码").Split('|');
            for (int i = 0; i < trainTippers.Count; i++)
            {
                CmcsCMEquipment cMEquipment = trainTippers[i];
                superTabControlManager.CreateTab(cMEquipment.EquipmentName, cMEquipment.EquipmentCode, new Frms.FrmTrainTipper(cMEquipment, carriageRecognitionerMachineCodes[i]), false);
            }

            superTabControl1.ResumeLayout();

            // 选中第一个选项卡
            if (superTabControl1.Tabs.Count > 0)
                superTabControl1.SelectedTabIndex = 0;
            else
            {
                MessageBoxEx.Show("翻车机参数未设置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        } 

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBoxEx.Show("确认退出系统？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
