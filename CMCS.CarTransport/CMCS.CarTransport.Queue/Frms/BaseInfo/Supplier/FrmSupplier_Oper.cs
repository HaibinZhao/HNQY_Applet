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
using CMCS.Common.Entities;
using CMCS.Common.Entities.BaseInfo;

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.Supplier
{
    public partial class FrmSupplier_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsSupplier cmcsSupplier;
        public FrmSupplier_Oper()
        {
            InitializeComponent();
        }
        public FrmSupplier_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }
        private void FrmSupplier_Oper_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsSupplier = Dbers.GetInstance().SelfDber.Get<CmcsSupplier>(this.id);
                txt_Code.Text = cmcsSupplier.Code;
                txt_Name.Text = cmcsSupplier.Name;
                chb_IsUse.Checked = (cmcsSupplier.IsUse == "1");
            }
            if (!edit)
            {
                btnSubmit.Enabled = false;
                CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(panelEx2);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_Name.Text.Length == 0)
            {
                MessageBoxEx.Show("该供应商名称不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if ((cmcsSupplier == null || cmcsSupplier.Name != txt_Name.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsSupplier>(" where Name=:Name", new { Name = txt_Name.Text }).Count > 0)
            {
                MessageBoxEx.Show("该供应商名称不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txt_Code.Text.Length == 0)
            {
                MessageBoxEx.Show("该供应商编号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if ((cmcsSupplier == null || cmcsSupplier.Code != txt_Code.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsSupplier>(" where Code=:Code", new { Code = txt_Code.Text }).Count > 0)
            {
                MessageBoxEx.Show("该供应商编号不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmcsSupplier != null)
            {
                cmcsSupplier.Name = txt_Name.Text;
                cmcsSupplier.Code = txt_Code.Text;
                cmcsSupplier.IsUse = (chb_IsUse.Checked ? "1" : "0");
                Dbers.GetInstance().SelfDber.Update(cmcsSupplier);
            }
            else
            {
                cmcsSupplier = new CmcsSupplier();
                cmcsSupplier.Name = txt_Name.Text;
                cmcsSupplier.Code = txt_Code.Text;
                cmcsSupplier.IsUse = (chb_IsUse.Checked ? "1" : "0");
                Dbers.GetInstance().SelfDber.Insert(cmcsSupplier);
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