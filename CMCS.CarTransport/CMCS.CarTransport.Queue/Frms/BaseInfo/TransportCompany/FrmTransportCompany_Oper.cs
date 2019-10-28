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

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.TransportCompany
{
    public partial class FrmTransportCompany_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsTransportCompany cmcsTransportCompany;
        public FrmTransportCompany_Oper()
        {
            InitializeComponent();
        }
        public FrmTransportCompany_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }
        private void FrmTransportCompany_Oper_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsTransportCompany = Dbers.GetInstance().SelfDber.Get<CmcsTransportCompany>(this.id);
                txt_Code.Text = cmcsTransportCompany.Code;
                txt_Name.Text = cmcsTransportCompany.Name;
                chb_IsUse.Checked = (cmcsTransportCompany.IsUse == 1);
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
                MessageBoxEx.Show("该单位名称不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (txt_Code.Text.Length == 0)
            {
                MessageBoxEx.Show("该单位编号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if ((cmcsTransportCompany == null || cmcsTransportCompany.Name != txt_Name.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsTransportCompany>(" where Name=:Name", new { Name = txt_Name.Text }).Count > 0)
            {
                MessageBoxEx.Show("该单位名称不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if ((cmcsTransportCompany == null || cmcsTransportCompany.Code != txt_Code.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsTransportCompany>(" where Code=:Code", new { Code = txt_Code.Text }).Count > 0)
            {
                MessageBoxEx.Show("该单位编号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmcsTransportCompany != null)
            {
                cmcsTransportCompany.Name = txt_Name.Text;
                cmcsTransportCompany.Code = txt_Code.Text;
                cmcsTransportCompany.IsUse = (chb_IsUse.Checked ? 1 : 0);
                Dbers.GetInstance().SelfDber.Update(cmcsTransportCompany);
            }
            else
            {
                cmcsTransportCompany = new CmcsTransportCompany();
                cmcsTransportCompany.Name = txt_Name.Text;
                cmcsTransportCompany.Code = txt_Code.Text;
                cmcsTransportCompany.IsUse = (chb_IsUse.Checked ? 1 : 0);
                Dbers.GetInstance().SelfDber.Insert(cmcsTransportCompany);
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