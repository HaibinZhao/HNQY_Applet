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

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.SupplyReceive
{
    public partial class FrmSupplyReceive_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsSupplyReceive cmcsSupplyReceive;
        public FrmSupplyReceive_Oper()
        {
            InitializeComponent();
        }
        public FrmSupplyReceive_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }
        private void FrmSupplyReceive_Oper_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsSupplyReceive = Dbers.GetInstance().SelfDber.Get<CmcsSupplyReceive>(this.id);
                txt_UnitName.Text = cmcsSupplyReceive.UnitName;
                chb_IsUse.Checked = (cmcsSupplyReceive.IsValid == 1);
                txt_ReMark.Text = cmcsSupplyReceive.Remark;
            }
            if (!edit)
            {
                btnSubmit.Enabled = false;
                CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(panelEx2);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_UnitName.Text.Length == 0)
            {
                MessageBoxEx.Show("该标单位名称不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if ((cmcsSupplyReceive == null || cmcsSupplyReceive.UnitName != txt_UnitName.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsSupplyReceive>(" where CardNumber=:CardNumber", new { CardNumber = txt_UnitName.Text }).Count > 0)
            {
                MessageBoxEx.Show("该标单位名称不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmcsSupplyReceive != null)
            {
                cmcsSupplyReceive.UnitName = txt_UnitName.Text;
                cmcsSupplyReceive.IsValid = (chb_IsUse.Checked ? 1 : 0);
                cmcsSupplyReceive.Remark = txt_ReMark.Text;
                Dbers.GetInstance().SelfDber.Update(cmcsSupplyReceive);
            }
            else
            {
                cmcsSupplyReceive = new CmcsSupplyReceive();
                cmcsSupplyReceive.UnitName = txt_UnitName.Text;
                cmcsSupplyReceive.IsValid = (chb_IsUse.Checked ? 1 : 0);
                cmcsSupplyReceive.Remark = txt_ReMark.Text;
                Dbers.GetInstance().SelfDber.Insert(cmcsSupplyReceive);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}