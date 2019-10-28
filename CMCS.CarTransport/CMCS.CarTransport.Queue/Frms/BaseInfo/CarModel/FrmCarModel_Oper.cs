using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.Common;
using CMCS.Common.Entities.CarTransport;
using DevComponents.Editors;

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.CarModel
{
    public partial class FrmCarModel_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsCarModel cmcsCarModel;
        public FrmCarModel_Oper()
        {
            InitializeComponent();
        }
        public FrmCarModel_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }
        private void FrmCarModel_Oper_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsCarModel = Dbers.GetInstance().SelfDber.Get<CmcsCarModel>(this.id);
                txt_ModelName.Text = cmcsCarModel.ModelName;
                dbi_LeftObstacle1.Value = cmcsCarModel.LeftObstacle1;
                dbi_LeftObstacle2.Value = cmcsCarModel.LeftObstacle2;
                dbi_LeftObstacle3.Value = cmcsCarModel.LeftObstacle3;
                dbi_LeftObstacle4.Value = cmcsCarModel.LeftObstacle4;
                dbi_LeftObstacle5.Value = cmcsCarModel.LeftObstacle5;
                dbi_LeftObstacle6.Value = cmcsCarModel.LeftObstacle6;
                dbi_RightObstacle1.Value = cmcsCarModel.RightObstacle1;
                dbi_RightObstacle2.Value = cmcsCarModel.RightObstacle2;
                dbi_RightObstacle3.Value = cmcsCarModel.RightObstacle3;
                dbi_RightObstacle4.Value = cmcsCarModel.RightObstacle4;
                dbi_RightObstacle5.Value = cmcsCarModel.RightObstacle5;
                dbi_RightObstacle6.Value = cmcsCarModel.RightObstacle6;
                dbi_CarriageLength.Value = cmcsCarModel.CarriageLength;
                dbi_CarriageWidth.Value = cmcsCarModel.CarriageWidth;
                dbi_CarriageBottomToFloor.Value = cmcsCarModel.CarriageBottomToFloor;
                txt_ReMark.Text = cmcsCarModel.ReMark;
            }
            if (!edit)
            {
                btnSubmit.Enabled = false;
                CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(panelEx2);
            }
            label_warn.ForeColor = Color.Red;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_ModelName.Text.Length == 0)
            {
                MessageBoxEx.Show("该标车型不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if ((cmcsCarModel == null || cmcsCarModel.ModelName != txt_ModelName.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsCarModel>(" where ModelName=:ModelName", new { ModelName = txt_ModelName.Text }).Count > 0)
            {
                MessageBoxEx.Show("该标车型不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmcsCarModel != null)
            {
                if (dbi_CarriageLength.Value <= 0)
                {
                    MessageBoxEx.Show("该车型长不能为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dbi_CarriageWidth.Value <= 0)
                {
                    MessageBoxEx.Show("该车型宽不能为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dbi_CarriageBottomToFloor.Value <= 0)
                {
                    MessageBoxEx.Show("该车型车厢底部到地面高不能为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle6.Value > 0 || dbi_RightObstacle6.Value > 0) && (dbi_LeftObstacle5.Value <= 0 && dbi_RightObstacle5.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋六信息必须有拉筋五信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle5.Value > 0 || dbi_RightObstacle5.Value > 0) && (dbi_LeftObstacle4.Value <= 0 && dbi_RightObstacle4.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋五信息必须有拉筋四信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle4.Value > 0 || dbi_RightObstacle4.Value > 0) && (dbi_LeftObstacle3.Value <= 0 && dbi_RightObstacle3.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋四信息必须有拉筋三信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle3.Value > 0 || dbi_RightObstacle3.Value > 0) && (dbi_LeftObstacle2.Value <= 0 && dbi_RightObstacle2.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋三信息必须有拉筋二信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle2.Value > 0 || dbi_RightObstacle2.Value > 0) && (dbi_LeftObstacle1.Value <= 0 && dbi_RightObstacle1.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋二信息必须有拉筋一信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                cmcsCarModel.ModelName = txt_ModelName.Text;
                cmcsCarModel.LeftObstacle1 = (int)dbi_LeftObstacle1.Value;
                cmcsCarModel.LeftObstacle2 = (int)dbi_LeftObstacle2.Value;
                cmcsCarModel.LeftObstacle3 = (int)dbi_LeftObstacle3.Value;
                cmcsCarModel.LeftObstacle4 = (int)dbi_LeftObstacle4.Value;
                cmcsCarModel.LeftObstacle5 = (int)dbi_LeftObstacle5.Value;
                cmcsCarModel.LeftObstacle6 = (int)dbi_LeftObstacle6.Value;
                cmcsCarModel.RightObstacle1 = (int)dbi_RightObstacle1.Value;
                cmcsCarModel.RightObstacle2 = (int)dbi_RightObstacle2.Value;
                cmcsCarModel.RightObstacle3 = (int)dbi_RightObstacle3.Value;
                cmcsCarModel.RightObstacle4 = (int)dbi_RightObstacle4.Value;
                cmcsCarModel.RightObstacle5 = (int)dbi_RightObstacle5.Value;
                cmcsCarModel.RightObstacle6 = (int)dbi_RightObstacle6.Value;
                cmcsCarModel.CarriageLength = (int)dbi_CarriageLength.Value;
                cmcsCarModel.CarriageWidth = (int)dbi_CarriageWidth.Value;
                cmcsCarModel.CarriageBottomToFloor = (int)dbi_CarriageBottomToFloor.Value;
                cmcsCarModel.ReMark = txt_ReMark.Text;
                Dbers.GetInstance().SelfDber.Update(cmcsCarModel);
            }
            else
            {
                cmcsCarModel = new CmcsCarModel();
                cmcsCarModel.ModelName = txt_ModelName.Text;
                cmcsCarModel.LeftObstacle1 = (int)dbi_LeftObstacle1.Value;
                cmcsCarModel.LeftObstacle2 = (int)dbi_LeftObstacle2.Value;
                cmcsCarModel.LeftObstacle3 = (int)dbi_LeftObstacle3.Value;
                cmcsCarModel.LeftObstacle4 = (int)dbi_LeftObstacle4.Value;
                cmcsCarModel.LeftObstacle5 = (int)dbi_LeftObstacle5.Value;
                cmcsCarModel.LeftObstacle6 = (int)dbi_LeftObstacle6.Value;
                cmcsCarModel.RightObstacle1 = (int)dbi_RightObstacle1.Value;
                cmcsCarModel.RightObstacle2 = (int)dbi_RightObstacle2.Value;
                cmcsCarModel.RightObstacle3 = (int)dbi_RightObstacle3.Value;
                cmcsCarModel.RightObstacle4 = (int)dbi_RightObstacle4.Value;
                cmcsCarModel.RightObstacle5 = (int)dbi_RightObstacle5.Value;
                cmcsCarModel.RightObstacle6 = (int)dbi_RightObstacle6.Value;
                cmcsCarModel.CarriageLength = (int)dbi_CarriageLength.Value;
                cmcsCarModel.CarriageWidth = (int)dbi_CarriageWidth.Value;
                cmcsCarModel.CarriageBottomToFloor = (int)dbi_CarriageBottomToFloor.Value;
                cmcsCarModel.ReMark = txt_ReMark.Text;
                Dbers.GetInstance().SelfDber.Insert(cmcsCarModel);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 给右拉近赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Obstacle_ValueChange(object sender, EventArgs e)
        {
            IntegerInput txt = (IntegerInput)sender;
            switch (txt.Name)
            {
                case "dbi_LeftObstacle1":
                    dbi_RightObstacle1.Value = txt.Value;
                    break;
                case "dbi_LeftObstacle2":
                    dbi_RightObstacle2.Value = txt.Value;
                    break;
                case "dbi_LeftObstacle3":
                    dbi_RightObstacle3.Value = txt.Value;
                    break;
                case "dbi_LeftObstacle4":
                    dbi_RightObstacle4.Value = txt.Value;
                    break;
                case "dbi_LeftObstacle5":
                    dbi_RightObstacle5.Value = txt.Value;
                    break;
                case "dbi_LeftObstacle6":
                    dbi_RightObstacle6.Value = txt.Value;
                    break;
                default:
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}