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
using CMCS.CarTransport.Queue.Core;
using CMCS.Common.DAO;

namespace CMCS.CarTransport.Queue.Frms.Transport.BuyFuelTransport
{
    public partial class FrmBuyFuelTransportDeduct_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        DateTime dtnow = DateTime.Now;
        String id = String.Empty;
        String TransportId = string.Empty;
        bool edit = false;
        public CmcsBuyFuelTransportDeduct cmcsBuyFuelTransportDeduct;
        List<CmcsBuyFuelTransportDeduct> cmcsbuyfueltransportdeducts;
        public FrmBuyFuelTransportDeduct_Oper()
        {
            InitializeComponent();
        }
        public FrmBuyFuelTransportDeduct_Oper(String pId, String transportId, bool pEdit, List<CmcsBuyFuelTransportDeduct> pCmcsBuyFuelTransportDeducts)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
            TransportId = transportId;
            cmcsbuyfueltransportdeducts = pCmcsBuyFuelTransportDeducts;
        }
        private void FrmBuyFuelTransportDeduct_Oper_Load(object sender, EventArgs e)
        {
            cmb_DeductType.Items.Add("扣矸");
            cmb_DeductType.Items.Add("扣水");
            cmb_DeductType.Items.Add("其他");
            cmb_DeductType.SelectedIndex = 0;
            if (!String.IsNullOrEmpty(id))
            {
                this.cmcsBuyFuelTransportDeduct = cmcsbuyfueltransportdeducts.Where(a => a.Id == id).First();
                dbi_DeductWeight.Value = (double)cmcsBuyFuelTransportDeduct.DeductWeight;
                txt_OperUser.Text = cmcsBuyFuelTransportDeduct.OperUser;
                cmb_DeductType.SelectedItem = cmcsBuyFuelTransportDeduct.DeductType;
                txt_OperDate.Text = cmcsBuyFuelTransportDeduct.OperDate.ToString();
            }
            else
            {
                txt_OperDate.Text = dtnow.ToString();
                txt_OperUser.Text = SelfVars.LoginUser.UserName;
            }
            if (!edit)
            {
                btnSubmit.Enabled = false;
            }
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dbi_DeductWeight.Value == 0)
            {
                MessageBoxEx.Show("扣重不能为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmcsBuyFuelTransportDeduct != null)
            {
                cmcsBuyFuelTransportDeduct.DeductWeight = (decimal)dbi_DeductWeight.Value;
                cmcsBuyFuelTransportDeduct.DeductType = (string)cmb_DeductType.SelectedItem;
                cmcsBuyFuelTransportDeduct.OperDate = DateTime.Now;
                cmcsBuyFuelTransportDeduct.OperUser = SelfVars.LoginUser.UserName;
                CommonDAO.GetInstance().SelfDber.Update(cmcsBuyFuelTransportDeduct);
            }
            else
            {
                cmcsBuyFuelTransportDeduct = new CmcsBuyFuelTransportDeduct();
                cmcsBuyFuelTransportDeduct.DeductWeight = (decimal)dbi_DeductWeight.Value;
                cmcsBuyFuelTransportDeduct.DeductType = (string)cmb_DeductType.SelectedItem;
                cmcsBuyFuelTransportDeduct.OperDate = dtnow;
                cmcsBuyFuelTransportDeduct.OperUser = SelfVars.LoginUser.UserName;
                cmcsBuyFuelTransportDeduct.TransportId = this.TransportId;
                CommonDAO.GetInstance().SelfDber.Insert(cmcsBuyFuelTransportDeduct);
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