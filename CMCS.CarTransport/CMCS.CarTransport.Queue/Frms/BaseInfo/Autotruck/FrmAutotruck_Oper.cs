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
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors;
using CMCS.CarTransport.Queue.Utilities;
using CMCS.CarTransport.DAO;
using CMCS.Common.DAO;

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.Autotruck
{
    public partial class FrmAutotruck_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsAutotruck cmcsAutotruck;
        CmcsEPCCard cmcsEPCCard;

        CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();
        CommonDAO commonDAO = CommonDAO.GetInstance();

        public FrmAutotruck_Oper()
        {
            InitializeComponent();
            edit = true;
        }
        public FrmAutotruck_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }


        private void FrmAutotruck_Oper_Load(object sender, EventArgs e)
        {
            cmb_CarType.Items.Add("入厂煤");
            cmb_CarType.Items.Add("其他物资");
            cmb_CarType.Items.Add("来访车辆");
            cmb_CarType.SelectedIndex = 0;

            cmbEmissionStandard.Items.Add("国III");
            cmbEmissionStandard.Items.Add("国IV");
            cmbEmissionStandard.Items.Add("国V");
            cmbEmissionStandard.Items.Add("国VI");
            cmbEmissionStandard.Items.Add("新能源");
            cmbEmissionStandard.SelectedIndex = 0;
            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsAutotruck = Dbers.GetInstance().SelfDber.Get<CmcsAutotruck>(this.id);
                txt_CarNumber.Text = cmcsAutotruck.CarNumber;
                cmb_CarType.SelectedItem = cmcsAutotruck.CarType;
                cmbEmissionStandard.SelectedItem = cmcsAutotruck.EmissionStandard;
                txt_Driver.Text = cmcsAutotruck.Driver;
                txt_CellPhoneNumber.Text = cmcsAutotruck.CellPhoneNumber;
                chb_IsUse.Checked = (cmcsAutotruck.IsUse == 1);
                dbi_LeftObstacle1.Value = cmcsAutotruck.LeftObstacle1;
                dbi_LeftObstacle2.Value = cmcsAutotruck.LeftObstacle2;
                dbi_LeftObstacle3.Value = cmcsAutotruck.LeftObstacle3;
                dbi_LeftObstacle4.Value = cmcsAutotruck.LeftObstacle4;
                dbi_LeftObstacle5.Value = cmcsAutotruck.LeftObstacle5;
                dbi_LeftObstacle6.Value = cmcsAutotruck.LeftObstacle6;
                dbi_RightObstacle1.Value = cmcsAutotruck.RightObstacle1;
                dbi_RightObstacle2.Value = cmcsAutotruck.RightObstacle2;
                dbi_RightObstacle3.Value = cmcsAutotruck.RightObstacle3;
                dbi_RightObstacle4.Value = cmcsAutotruck.RightObstacle4;
                dbi_RightObstacle5.Value = cmcsAutotruck.RightObstacle5;
                dbi_RightObstacle6.Value = cmcsAutotruck.RightObstacle6;
                dbi_CarriageLength.Value = cmcsAutotruck.CarriageLength;
                dbi_CarriageWidth.Value = cmcsAutotruck.CarriageWidth;
                dbi_CarriageBottomToFloor.Value = cmcsAutotruck.CarriageBottomToFloor;
                dbi_CarriageLength2.Value = cmcsAutotruck.CarriageLength2;
                dbi_CarriageBottomToFloor2.Value = cmcsAutotruck.CarriageBottomToFloor2;
                txt_ReMark.Text = cmcsAutotruck.ReMark;
                cmcsEPCCard = Dbers.GetInstance().SelfDber.Get<CmcsEPCCard>(cmcsAutotruck.EPCCardId);
                txt_EPCCardNumber.Text = cmcsEPCCard.CardNumber;
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
            txt_CarNumber.Text = txt_CarNumber.Text.ToUpper();
            if (txt_CarNumber.Text.Length == 0)
            {
                MessageBoxEx.Show("该车牌号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if ((cmcsAutotruck == null || cmcsAutotruck.CarNumber != txt_CarNumber.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsAutotruck>(" where CarNumber=:CarNumber", new { CarNumber = txt_CarNumber.Text }).Count > 0)
            {
                MessageBoxEx.Show("该车牌号不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txt_EPCCardNumber.Text.Length == 0)
            {
                MessageBoxEx.Show("标签号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (String.IsNullOrEmpty(id)) { id = Guid.NewGuid().ToString(); }
            if (Dbers.GetInstance().SelfDber.Entities<CmcsAutotruck>(" where EPCCardId=:EPCCardId and Id!=:Id", new { EPCCardId = cmcsEPCCard.Id, Id = id }).Count > 0)
            {
                MessageBoxEx.Show("标签号不能相同！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(cmbEmissionStandard.Text))
            {
                MessageBoxEx.Show("请选择排放标准！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmb_CarType.SelectedItem.ToString() == "入厂煤")
            {
                #region 入厂煤车辆长宽信息
                if (dbi_CarriageLength.Value <= 0)
                {
                    MessageBoxEx.Show("该长不能为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dbi_CarriageWidth.Value <= 0)
                {
                    MessageBoxEx.Show("该宽不能为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dbi_CarriageBottomToFloor.Value <= 0)
                {
                    MessageBoxEx.Show("该车厢底部到地面高不能为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dbi_CarriageLength.Value > 15000)
                {
                    MessageBoxEx.Show("车长不能超过15000mm！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dbi_CarriageWidth.Value > 3000)
                {
                    MessageBoxEx.Show("车宽不能超过3000mm！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dbi_CarriageBottomToFloor.Value > 2000)
                {
                    MessageBoxEx.Show("车宽不能超过2000mm！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle6.Value > 0 || dbi_RightObstacle6.Value > 0) && (dbi_LeftObstacle5.Value <= 0 && dbi_RightObstacle5.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋F信息必须有拉筋E信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle5.Value > 0 || dbi_RightObstacle5.Value > 0) && (dbi_LeftObstacle4.Value <= 0 && dbi_RightObstacle4.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋E信息必须有拉筋D信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle4.Value > 0 || dbi_RightObstacle4.Value > 0) && (dbi_LeftObstacle3.Value <= 0 && dbi_RightObstacle3.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋D信息必须有拉筋C信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle3.Value > 0 || dbi_RightObstacle3.Value > 0) && (dbi_LeftObstacle2.Value <= 0 && dbi_RightObstacle2.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋C信息必须有拉筋B信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((dbi_LeftObstacle2.Value > 0 || dbi_RightObstacle2.Value > 0) && (dbi_LeftObstacle1.Value <= 0 && dbi_RightObstacle1.Value <= 0))
                {
                    MessageBoxEx.Show("已有拉筋B信息必须有拉筋A信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                #endregion
            }
            if (cmcsAutotruck != null)
            {
                cmcsAutotruck.CarNumber = txt_CarNumber.Text;
                cmcsAutotruck.CarType = cmb_CarType.SelectedItem.ToString();
                cmcsAutotruck.EmissionStandard = cmbEmissionStandard.SelectedItem.ToString();
                cmcsAutotruck.Driver = txt_Driver.Text;
                cmcsAutotruck.CellPhoneNumber = txt_CellPhoneNumber.Text;
                cmcsAutotruck.IsUse = (chb_IsUse.Checked ? 1 : 0);
                cmcsAutotruck.LeftObstacle1 = (int)dbi_LeftObstacle1.Value;
                cmcsAutotruck.LeftObstacle2 = (int)dbi_LeftObstacle2.Value;
                cmcsAutotruck.LeftObstacle3 = (int)dbi_LeftObstacle3.Value;
                cmcsAutotruck.LeftObstacle4 = (int)dbi_LeftObstacle4.Value;
                cmcsAutotruck.LeftObstacle5 = (int)dbi_LeftObstacle5.Value;
                cmcsAutotruck.LeftObstacle6 = (int)dbi_LeftObstacle6.Value;
                cmcsAutotruck.RightObstacle1 = (int)dbi_RightObstacle1.Value;
                cmcsAutotruck.RightObstacle2 = (int)dbi_RightObstacle2.Value;
                cmcsAutotruck.RightObstacle3 = (int)dbi_RightObstacle3.Value;
                cmcsAutotruck.RightObstacle4 = (int)dbi_RightObstacle4.Value;
                cmcsAutotruck.RightObstacle5 = (int)dbi_RightObstacle5.Value;
                cmcsAutotruck.RightObstacle6 = (int)dbi_RightObstacle6.Value;
                cmcsAutotruck.CarriageLength = (int)dbi_CarriageLength.Value;
                cmcsAutotruck.CarriageWidth = (int)dbi_CarriageWidth.Value;
                cmcsAutotruck.CarriageBottomToFloor = (int)dbi_CarriageBottomToFloor.Value;
                cmcsAutotruck.CarriageLength2 = (int)dbi_CarriageLength2.Value;
                cmcsAutotruck.CarriageBottomToFloor2 = (int)dbi_CarriageBottomToFloor2.Value;
                cmcsAutotruck.ReMark = txt_ReMark.Text;
                cmcsAutotruck.EPCCardId = cmcsEPCCard.Id;
                Dbers.GetInstance().SelfDber.Update(cmcsAutotruck);

                CmcsBuyFuelTransport _CmcsBuyFuelTransport = carTransportDAO.GetBuyFuelTransportByAutoTruckId(cmcsAutotruck.Id);

                if (_CmcsBuyFuelTransport != null)
                {
                    _CmcsBuyFuelTransport.CarNumber = cmcsAutotruck.CarNumber;

                    if (Dbers.GetInstance().SelfDber.Update(_CmcsBuyFuelTransport) > 0)
                    {
                        //更新运输记录到批次
                        commonDAO.InsertWaitForHandleEvent("汽车智能化_同步入厂煤运输记录到批次", _CmcsBuyFuelTransport.Id);
                    }
                }
            }
            else
            {
                cmcsAutotruck = new CmcsAutotruck();
                cmcsAutotruck.CarNumber = txt_CarNumber.Text;
                cmcsAutotruck.CarType = cmb_CarType.SelectedItem.ToString();
                cmcsAutotruck.EmissionStandard = cmbEmissionStandard.SelectedItem.ToString();
                cmcsAutotruck.Driver = txt_Driver.Text;
                cmcsAutotruck.CellPhoneNumber = txt_CellPhoneNumber.Text;
                cmcsAutotruck.IsUse = (chb_IsUse.Checked ? 1 : 0);
                cmcsAutotruck.LeftObstacle1 = (int)dbi_LeftObstacle1.Value;
                cmcsAutotruck.LeftObstacle2 = (int)dbi_LeftObstacle2.Value;
                cmcsAutotruck.LeftObstacle3 = (int)dbi_LeftObstacle3.Value;
                cmcsAutotruck.LeftObstacle4 = (int)dbi_LeftObstacle4.Value;
                cmcsAutotruck.LeftObstacle5 = (int)dbi_LeftObstacle5.Value;
                cmcsAutotruck.LeftObstacle6 = (int)dbi_LeftObstacle6.Value;
                cmcsAutotruck.RightObstacle1 = (int)dbi_RightObstacle1.Value;
                cmcsAutotruck.RightObstacle2 = (int)dbi_RightObstacle2.Value;
                cmcsAutotruck.RightObstacle3 = (int)dbi_RightObstacle3.Value;
                cmcsAutotruck.RightObstacle4 = (int)dbi_RightObstacle4.Value;
                cmcsAutotruck.RightObstacle5 = (int)dbi_RightObstacle5.Value;
                cmcsAutotruck.RightObstacle6 = (int)dbi_RightObstacle6.Value;
                cmcsAutotruck.CarriageLength = (int)dbi_CarriageLength.Value;
                cmcsAutotruck.CarriageWidth = (int)dbi_CarriageWidth.Value;
                cmcsAutotruck.CarriageBottomToFloor = (int)dbi_CarriageBottomToFloor.Value;
                cmcsAutotruck.CarriageLength2 = (int)dbi_CarriageLength2.Value;
                cmcsAutotruck.CarriageBottomToFloor2 = (int)dbi_CarriageBottomToFloor2.Value;
                cmcsAutotruck.ReMark = txt_ReMark.Text;
                cmcsAutotruck.EPCCardId = cmcsEPCCard.Id;
                Dbers.GetInstance().SelfDber.Insert(cmcsAutotruck);
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

        private void btnSelectedCarModel_Click(object sender, EventArgs e)
        {
            FrmCarModel_Select Frm = new FrmCarModel_Select();
            Frm.ShowDialog();
            if (Frm.DialogResult == DialogResult.OK)
            {
                CmcsCarModel item = Frm.Output;
                dbi_LeftObstacle1.Value = item.LeftObstacle1;
                dbi_LeftObstacle2.Value = item.LeftObstacle2;
                dbi_LeftObstacle3.Value = item.LeftObstacle3;
                dbi_LeftObstacle4.Value = item.LeftObstacle4;
                dbi_LeftObstacle5.Value = item.LeftObstacle5;
                dbi_LeftObstacle6.Value = item.LeftObstacle6;
                dbi_RightObstacle1.Value = item.RightObstacle1;
                dbi_RightObstacle2.Value = item.RightObstacle2;
                dbi_RightObstacle3.Value = item.RightObstacle3;
                dbi_RightObstacle4.Value = item.RightObstacle4;
                dbi_RightObstacle5.Value = item.RightObstacle5;
                dbi_RightObstacle6.Value = item.RightObstacle6;
                dbi_CarriageLength.Value = item.CarriageLength;
                dbi_CarriageWidth.Value = item.CarriageWidth;
                dbi_CarriageBottomToFloor.Value = item.CarriageBottomToFloor;
            }
        }

        private void btnSelectEPCCardNumber_Click(object sender, EventArgs e)
        {
            FrmEPCCard_Select Frm = new FrmEPCCard_Select();
            Frm.ShowDialog();
            if (Frm.DialogResult == DialogResult.OK)
            {
                cmcsEPCCard = Frm.Output;
                txt_EPCCardNumber.Text = cmcsEPCCard.CardNumber;
            }
        }

        private void txt_CarNumber_TextChanged(object sender, EventArgs e)
        {
        }

        private void txt_CarNumber_Leave(object sender, EventArgs e)
        {

            ((TextBoxX)sender).Text = ((TextBoxX)sender).Text.ToUpper();
        }

        /// <summary>
        /// 创建省份简称按钮
        /// </summary>
        private void CreateProvinceAbbreviationButton()
        {
            flpanProvinceAbbreviation.Controls.Clear();

            foreach (CmcsProvinceAbbreviation provinceAbbreviation in CarTransportDAO.GetInstance().GetProvinceAbbreviationsInOrder())
            {
                ButtonX btnProvinceAbbreviation = new ButtonX();
                btnProvinceAbbreviation.Text = provinceAbbreviation.PaName;
                btnProvinceAbbreviation.Style = eDotNetBarStyle.Metro;
                btnProvinceAbbreviation.Font = new Font("微软雅黑", 10.8f, FontStyle.Bold);
                btnProvinceAbbreviation.Size = new Size(26, 26);
                btnProvinceAbbreviation.Margin = new System.Windows.Forms.Padding(3);
                btnProvinceAbbreviation.Click += BtnProvinceAbbreviation_Click;

                flpanProvinceAbbreviation.Controls.Add(btnProvinceAbbreviation);
            }
        }

        /// <summary>
        /// 点击省份简称按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProvinceAbbreviation_Click(object sender, EventArgs e)
        {
            ButtonX btnProvinceAbbreviation = sender as ButtonX;
            if (btnProvinceAbbreviation != null) CarTransportDAO.GetInstance().AddProvinceAbbreviationUseCount(btnProvinceAbbreviation.Text);

            txt_CarNumber.Text = txt_CarNumber.Text.Insert(0, btnProvinceAbbreviation.Text);
            txt_CarNumber.CloseDropDown();

            txt_CarNumber.Focus();
            txt_CarNumber.Select(txt_CarNumber.Text.Length, 0);
        }

        private void txt_CarNumber_ButtonDropDownClick(object sender, CancelEventArgs e)
        {
            CreateProvinceAbbreviationButton();
        }
    }
}