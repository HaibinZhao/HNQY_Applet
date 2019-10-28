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

namespace CMCS.CarTransport.Queue.Frms.BaseInfo.EPCCard
{
    public partial class FrmEPCCard_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        bool edit = false;
        CmcsEPCCard cmcsepccard;
        public FrmEPCCard_Oper()
        {
            InitializeComponent();
        }
        public FrmEPCCard_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }
        private void FrmEPCCard_Oper_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsepccard = Dbers.GetInstance().SelfDber.Get<CmcsEPCCard>(this.id);
                txt_CardNumber.Text = cmcsepccard.CardNumber;
                txt_TagId.Text = cmcsepccard.TagId;
            }
            if (!edit)
            {
                btnSubmit.Enabled = false;
                CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(panelEx2);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_CardNumber.Text.Length == 0)
            {
                MessageBoxEx.Show("该标卡号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (txt_TagId.Text.Length == 0)
            {
                MessageBoxEx.Show("该标签号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if ((cmcsepccard == null || cmcsepccard.CardNumber != txt_CardNumber.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsEPCCard>(" where CardNumber=:CardNumber", new { CardNumber = txt_CardNumber.Text }).Count > 0)
            {
                MessageBoxEx.Show("该标卡号不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if ((cmcsepccard == null || cmcsepccard.TagId != txt_TagId.Text) && Dbers.GetInstance().SelfDber.Entities<CmcsEPCCard>(" where TagId=:TagId", new { TagId = txt_TagId.Text }).Count > 0)
            {
                MessageBoxEx.Show("该标签号不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmcsepccard != null)
            {
                cmcsepccard.CardNumber = txt_CardNumber.Text;
                cmcsepccard.TagId = txt_TagId.Text;
                Dbers.GetInstance().SelfDber.Update(cmcsepccard);
            }
            else
            {
                cmcsepccard = new CmcsEPCCard();
                cmcsepccard.CardNumber = txt_CardNumber.Text;
                cmcsepccard.TagId = txt_TagId.Text;
                Dbers.GetInstance().SelfDber.Insert(cmcsepccard);
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