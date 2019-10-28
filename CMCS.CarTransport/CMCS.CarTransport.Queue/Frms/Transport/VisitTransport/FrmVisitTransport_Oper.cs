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

namespace CMCS.CarTransport.Queue.Frms.Transport.VisitTransport
{
    public partial class FrmVisitTransport_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsVisitTransport cmcsVisitTransport;
        CmcsSupplyReceive supplyUnit;
        CmcsSupplyReceive receiveUnit;
        CmcsGoodsType cmcsGoodsType;

        public FrmVisitTransport_Oper()
        {
            InitializeComponent();
        }
        public FrmVisitTransport_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }


        private void FrmVisitTransport_Oper_Load(object sender, EventArgs e)
        {

            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsVisitTransport = Dbers.GetInstance().SelfDber.Get<CmcsVisitTransport>(this.id);
                txt_SerialNumber.Text = cmcsVisitTransport.SerialNumber;
                txt_CarNumber.Text = cmcsVisitTransport.CarNumber;
                txt_InFactoryTime.Text = cmcsVisitTransport.InFactoryTime.Year == 1 ? "" : cmcsVisitTransport.InFactoryTime.ToString();
                txt_OutFactoryTime.Text = cmcsVisitTransport.OutFactoryTime.Year == 1 ? "" : cmcsVisitTransport.OutFactoryTime.ToString();
                txt_Remark.Text = cmcsVisitTransport.Remark;
                chb_IsFinish.Checked = (cmcsVisitTransport.IsFinish == 1);
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
            if ((cmcsVisitTransport == null || cmcsVisitTransport.CarNumber != txt_SerialNumber.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsVisitTransport>(" where CarNumber=:CarNumber", new { CarNumber = txt_SerialNumber.Text }).Count > 0)
            {
                MessageBoxEx.Show("该标车牌号不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmcsVisitTransport != null)
            {
                cmcsVisitTransport.Remark = txt_Remark.Text;
                cmcsVisitTransport.IsFinish = (chb_IsFinish.Checked ? 1 : 0);
                Dbers.GetInstance().SelfDber.Update(cmcsVisitTransport);
            }
            else
            {
                cmcsVisitTransport = new CmcsVisitTransport();
                cmcsVisitTransport.Remark = txt_Remark.Text;
                cmcsVisitTransport.IsFinish = (chb_IsFinish.Checked ? 1 : 0);
                Dbers.GetInstance().SelfDber.Insert(cmcsVisitTransport);
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
                receiveUnit = Frm.Output;
            }
        }

        private void btnSupplyUnit_Click(object sender, EventArgs e)
        {

            FrmSupplyReceive_Select Frm = new FrmSupplyReceive_Select();
            Frm.ShowDialog();
            if (Frm.DialogResult == DialogResult.OK)
            {
                supplyUnit = Frm.Output;
            }
        }

        private void cmb_GoodsTypeName_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

    }
}