using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.Common;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities;
using CMCS.CarTransport.DAO;
using CMCS.Common.Enums;

namespace CMCS.CarTransport.Queue.Frms.Transport.GoodsTransport
{
    public partial class FrmGoodsTransport_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsGoodsTransport cmcsGoodsTransport;
        CmcsSupplyReceive supplyUnit;
        /// <summary>
        /// 当前供应商
        /// </summary>
        private CmcsSupplyReceive SupplyUnit
        {
            get { return supplyUnit; }
            set
            {
                supplyUnit = value;
                if (value != null)
                    txt_SupplyUnitName.Text = value.UnitName;
            }
        }
        CmcsSupplyReceive receiveUnit;
        private CmcsSupplyReceive ReceiveUnit
        {
            get { return receiveUnit; }
            set
            {
                receiveUnit = value;
                if (value != null)
                    txt_ReceiveUnitName.Text = value.UnitName;
            }
        }
        CmcsGoodsType cmcsGoodsType;

        public FrmGoodsTransport_Oper()
        {
            InitializeComponent();
        }
        public FrmGoodsTransport_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }


        private void FrmGoodsTransport_Oper_Load(object sender, EventArgs e)
        {
            cmb_GoodsTypeName.DataSource = Dbers.GetInstance().SelfDber.Entities<CmcsGoodsType>(" where ParentId is not null order by OrderNumber");
            cmb_GoodsTypeName.DisplayMember = "GoodsName";
            cmb_GoodsTypeName.ValueMember = "Id";
            cmb_GoodsTypeName.SelectedIndex = 0;

            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsGoodsTransport = Dbers.GetInstance().SelfDber.Get<CmcsGoodsTransport>(this.id);
                txt_SerialNumber.Text = cmcsGoodsTransport.SerialNumber;
                txt_CarNumber.Text = cmcsGoodsTransport.CarNumber;
                txt_SupplyUnitName.Text = cmcsGoodsTransport.SupplyUnitName;
                txt_ReceiveUnitName.Text = cmcsGoodsTransport.ReceiveUnitName;
                dbi_FirstWeight.Value = (double)cmcsGoodsTransport.FirstWeight;
                dbi_SecondWeight.Value = (double)cmcsGoodsTransport.SecondWeight;
                cmb_GoodsTypeName.Text = cmcsGoodsTransport.GoodsTypeName;
                dbi_SuttleWeight.Value = (double)cmcsGoodsTransport.SuttleWeight;
                txt_InFactoryTime.Text = cmcsGoodsTransport.InFactoryTime.Year == 1 ? "" : cmcsGoodsTransport.InFactoryTime.ToString();
                txt_OutFactoryTime.Text = cmcsGoodsTransport.OutFactoryTime.Year == 1 ? "" : cmcsGoodsTransport.OutFactoryTime.ToString();
                txt_FirstTime.Text = cmcsGoodsTransport.FirstTime.Year == 1 ? "" : cmcsGoodsTransport.FirstTime.ToString();
                txt_SecondTime.Text = cmcsGoodsTransport.SecondTime.Year == 1 ? "" : cmcsGoodsTransport.SecondTime.ToString();
                txt_Remark.Text = cmcsGoodsTransport.Remark;
                chb_IsFinish.Checked = (cmcsGoodsTransport.IsFinish == 1);
                chb_IsUse.Checked = (cmcsGoodsTransport.IsUse == 1);
            }
            if (!edit)
            {
                btnSubmit.Enabled = false;
                CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(panelEx2);
            }
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_SerialNumber.Text.Length == 0)
            {
                MessageBoxEx.Show("该标车牌号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if ((cmcsGoodsTransport == null || cmcsGoodsTransport.CarNumber != txt_SerialNumber.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsGoodsTransport>(" where CarNumber=:CarNumber", new { CarNumber = txt_SerialNumber.Text }).Count > 0)
            {
                MessageBoxEx.Show("该标车牌号不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmcsGoodsTransport != null)
            {
                cmcsGoodsTransport.FirstWeight = (decimal)dbi_FirstWeight.Value;
                cmcsGoodsTransport.SecondWeight = (decimal)dbi_SecondWeight.Value;
                cmcsGoodsTransport.SuttleWeight =Math.Abs(cmcsGoodsTransport.FirstWeight - cmcsGoodsTransport.SecondWeight);
                if (receiveUnit != null)
                {
                    cmcsGoodsTransport.ReceiveUnitId = receiveUnit.Id;
                    cmcsGoodsTransport.ReceiveUnitName = receiveUnit.UnitName;
                }
                if (supplyUnit != null)
                {
                    cmcsGoodsTransport.SupplyUnitId = supplyUnit.Id;
                    cmcsGoodsTransport.SupplyUnitName = supplyUnit.UnitName;
                }
                if (cmcsGoodsType != null)
                {
                    cmcsGoodsTransport.GoodsTypeId = cmcsGoodsType.Id;
                    cmcsGoodsTransport.GoodsTypeName = cmcsGoodsType.GoodsName;
                }
                txt_Remark.Text = cmcsGoodsTransport.Remark;
                cmcsGoodsTransport.IsFinish = (chb_IsFinish.Checked ? 1 : 0);
                cmcsGoodsTransport.IsUse = (chb_IsUse.Checked ? 1 : 0);

                CarTransportDAO.GetInstance().SaveGoodsTransport(cmcsGoodsTransport);
                if (cmcsGoodsTransport.IsFinish == 0)
                {
                    CarTransportDAO.GetInstance().SaveUnFinishTransport(cmcsGoodsTransport.Id, eCarType.其他物资, cmcsGoodsTransport.AutotruckId);
                }
            }
            else
            {
                cmcsGoodsTransport = new CmcsGoodsTransport();
                cmcsGoodsTransport.FirstWeight = (decimal)dbi_FirstWeight.Value;
                cmcsGoodsTransport.SecondWeight = (decimal)dbi_SecondWeight.Value;
                cmcsGoodsTransport.SuttleWeight = (decimal)dbi_SuttleWeight.Value;
                if (receiveUnit != null)
                {
                    cmcsGoodsTransport.ReceiveUnitId = receiveUnit.Id;
                    cmcsGoodsTransport.ReceiveUnitName = receiveUnit.UnitName;
                }
                if (supplyUnit != null)
                {
                    cmcsGoodsTransport.SupplyUnitId = supplyUnit.Id;
                    cmcsGoodsTransport.SupplyUnitName = supplyUnit.UnitName;
                }
                if (cmcsGoodsType != null)
                {
                    cmcsGoodsTransport.GoodsTypeId = cmcsGoodsType.Id;
                    cmcsGoodsTransport.GoodsTypeName = cmcsGoodsType.GoodsName;
                }
                txt_Remark.Text = cmcsGoodsTransport.Remark;
                cmcsGoodsTransport.IsFinish = (chb_IsFinish.Checked ? 1 : 0);
                cmcsGoodsTransport.IsUse = (chb_IsUse.Checked ? 1 : 0);
                Dbers.GetInstance().SelfDber.Insert(cmcsGoodsTransport);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {


        }




        private void superGridControl1_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // 取消编辑
            e.Cancel = true;
        }



        private void btnReceiveUnit_Click(object sender, EventArgs e)
        {

            FrmSupplyReceive_Select Frm = new FrmSupplyReceive_Select();
            Frm.ShowDialog();
            if (Frm.DialogResult == DialogResult.OK)
            {
                ReceiveUnit = Frm.Output;
            }
        }

        private void btnSupplyUnit_Click(object sender, EventArgs e)
        {

            FrmSupplyReceive_Select Frm = new FrmSupplyReceive_Select();
            Frm.ShowDialog();
            if (Frm.DialogResult == DialogResult.OK)
            {
                SupplyUnit = Frm.Output;
            }
        }

        private void cmb_GoodsTypeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cmcsGoodsType = cmb_GoodsTypeName.SelectedItem as CmcsGoodsType;
        }

    }
}