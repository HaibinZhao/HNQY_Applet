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
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Utilities;
using CMCS.CarTransport.DAO;
using DevComponents.DotNetBar.Controls;
using CMCS.Common.DAO;
using CMCS.Common.Enums;
using CMCS.CarTransport.Queue.Core;

namespace CMCS.CarTransport.Queue.Frms.Transport.BuyFuelTransport
{
    public partial class FrmBuyFuelTransport_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        #region Vars
        String id = String.Empty;
        bool edit = false;
        CmcsBuyFuelTransport cmcsBuyFuelTransport;
        CmcsTransportCompany cmcsTransportCompany; /// <summary>
        /// 当前运输记录
        /// </summary>
        private CmcsTransportCompany CmcsTransportCompany
        {
            get { return cmcsTransportCompany; }
            set
            {
                cmcsTransportCompany = value;
                if (value != null)
                    txt_TransportCompanyName.Text = value.Name;
            }
        }

        CmcsMine cmcsMine;
        /// <summary>
        /// 当前矿点
        /// </summary>
        private CmcsMine CmcsMine
        {
            get { return cmcsMine; }
            set
            {
                cmcsMine = value;
                if (value != null)
                    txt_MineName.Text = value.Name;
            }
        }

        CmcsSupplier cmcsSupplier;
        /// <summary>
        /// 当前供应商
        /// </summary>
        private CmcsSupplier CmcsSupplier
        {
            get { return cmcsSupplier; }
            set
            {
                cmcsSupplier = value;
                if (value != null)
                    txt_SupplierName.Text = value.Name;
            }
        }

        CmcsFuelKind cmcsFuelKind;

        CmcsAutotruck cmcsAutotruck;
        /// <summary>
        /// 当前车辆
        /// </summary>
        private CmcsAutotruck CmcsAutotruck
        {
            get { return cmcsAutotruck; }
            set
            {
                cmcsAutotruck = value;
                if (value != null)
                {
                    this.txt_CarNumber.Text = value.CarNumber;
                    this.cmcsBuyFuelTransport.AutotruckId = value.Id;
                }
            }
        }

        List<CmcsBuyFuelTransportDeduct> cmcsbuyfueltransportdeducts;

        bool hasDeductManagePower = false;
        /// <summary>
        /// 对否有扣吨维护权限
        /// </summary>
        public bool HasDeductManagePower
        {
            get
            {
                return hasDeductManagePower;
            }
            set
            {
                hasDeductManagePower = value;
                btn_AddDeduct.Enabled = value;
                superGridControl1.PrimaryGrid.Columns["clmEdit"].Visible = value;
                superGridControl1.PrimaryGrid.Columns["clmDelete"].Visible = value;
            }
        }

        bool hasManagePower = false;
        /// <summary>
        /// 对否有扣吨维护权限
        /// </summary>
        public bool HasManagePower
        {
            get
            {
                return hasManagePower;
            }
            set
            {
                hasManagePower = value;
                date_InFactoryTime.Enabled = value;
                date_GrossTime.Enabled = value;
                date_TareTime.Enabled = value;
                date_OutFactoryTime.Enabled = value;
                dbi_GrossWeight.IsInputReadOnly = !value;
                dbi_TareWeight.IsInputReadOnly = !value;
            }
        }
        #endregion

        public FrmBuyFuelTransport_Oper()
        {
            InitializeComponent();
        }
        public FrmBuyFuelTransport_Oper(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
            //HasManagePower = CommonDAO.GetInstance().HasResourcePowerByResCode(SelfVars.LoginUser.UserAccount, eUserRoleCodes.汽车智能化信息维护.ToString());
            //HasDeductManagePower = CommonDAO.GetInstance().HasResourcePowerByResCode(SelfVars.LoginUser.UserAccount, eUserRoleCodes.汽车智能化扣吨信息维护.ToString());
        }

        private void cmbFuelName_BuyFuel_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cmcsFuelKind = cmbFuelName_BuyFuel.SelectedItem as CmcsFuelKind;
        }

        private void FrmBuyFuelTransport_Oper_Load(object sender, EventArgs e)
        {
            try
            {
                LoadSampleType(cmb_SampingType);
                cmb_SampingType.SelectedIndex = 0;
                LoadSample(cmb_Samping);
                cmb_Samping.SelectedIndex = 0;
                LoadStepName(cmb_STEPNAME);
                cmb_STEPNAME.SelectedIndex = 0;
                cmbFuelName_BuyFuel.DisplayMember = "FuelName";
                cmbFuelName_BuyFuel.ValueMember = "Id";
                cmbFuelName_BuyFuel.DataSource = Dbers.GetInstance().SelfDber.Entities<CmcsFuelKind>("where isuse=1 and ParentId is not null");
                cmbFuelName_BuyFuel.SelectedIndex = 0;
                if (!String.IsNullOrEmpty(id))
                {
                    this.cmcsBuyFuelTransport = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(this.id);
                    txt_SerialNumber.Text = cmcsBuyFuelTransport.SerialNumber;
                    txt_CarNumber.Text = cmcsBuyFuelTransport.CarNumber;
                    CmcsInFactoryBatch cmcsinfactorybatch = Dbers.GetInstance().SelfDber.Get<CmcsInFactoryBatch>(cmcsBuyFuelTransport.InFactoryBatchId);
                    if (cmcsinfactorybatch != null)
                    {
                        txt_InFactoryBatchNumber.Text = cmcsinfactorybatch.Batch;
                    }
                    txt_SupplierName.Text = cmcsBuyFuelTransport.SupplierName;
                    txt_TransportCompanyName.Text = cmcsBuyFuelTransport.TransportCompanyName;
                    txt_MineName.Text = cmcsBuyFuelTransport.MineName;
                    cmbFuelName_BuyFuel.Text = cmcsBuyFuelTransport.FuelKindName;
                    cmb_SampingType.Text = cmcsBuyFuelTransport.SamplingType;
                    cmb_Samping.Text = cmcsBuyFuelTransport.FPSamplePlace;
                    cmb_STEPNAME.Text = cmcsBuyFuelTransport.StepName;
                    dbi_TicketWeight.Value = (double)cmcsBuyFuelTransport.TicketWeight;
                    dbi_GrossWeight.Value = (double)cmcsBuyFuelTransport.GrossWeight;
                    dbi_TareWeight.Value = (double)cmcsBuyFuelTransport.TareWeight;
                    dbi_DeductWeight.Value = (double)cmcsBuyFuelTransport.DeductWeight;
                    dbi_SuttleWeight.Value = (double)cmcsBuyFuelTransport.SuttleWeight;
                    txt_UnloadArea.Text = cmcsBuyFuelTransport.UnLoadArea;
                    date_InFactoryTime.Value = cmcsBuyFuelTransport.InFactoryTime;
                    txt_SamplingTime.Text = cmcsBuyFuelTransport.SamplingTime.Year == 1 ? "" : cmcsBuyFuelTransport.SamplingTime.ToString();
                    date_GrossTime.Value = cmcsBuyFuelTransport.GrossTime;
                    txt_UploadTime.Text = cmcsBuyFuelTransport.UploadTime.Year == 1 ? "" : cmcsBuyFuelTransport.UploadTime.ToString();
                    date_TareTime.Value = cmcsBuyFuelTransport.TareTime;
                    date_OutFactoryTime.Value = cmcsBuyFuelTransport.OutFactoryTime;
                    txt_Remark.Text = cmcsBuyFuelTransport.Remark;
                    chb_IsFinish.Checked = (cmcsBuyFuelTransport.IsFinish == 1);
                    chb_IsUse.Checked = (cmcsBuyFuelTransport.IsUse == 1);
                    chbIsAutoDeduct.Checked = (cmcsBuyFuelTransport.IsAutoDeduct == 1);

                    ShowDeduct(this.cmcsBuyFuelTransport.Id);
                }
                if (!edit)
                {
                    btnSubmit.Enabled = false;
                    CMCS.CarTransport.Queue.Utilities.Helper.ControlReadOnly(panelEx2);
                }
            }
            catch (Exception ex)
            {
                Log4Neter.Error("加载事件", ex);
            }
        }

        /// <summary>
        /// 加载采样方式
        /// </summary>
        void LoadSampleType(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "Content";
            comboBoxEx.ValueMember = "Code";
            comboBoxEx.DataSource = CommonDAO.GetInstance().GetCodeContentByKind("采样方式");
        }

        /// <summary>
        /// 加载采样机
        /// </summary>
        void LoadSample(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "Content";
            comboBoxEx.ValueMember = "Code";
            comboBoxEx.DataSource = CommonDAO.GetInstance().GetCodeContentByKind("汽车机械采样机");

            //comboBoxEx.Text = eSamplingType.机械采样.ToString();
        }


        /// <summary>
        /// 加载流程状态
        /// </summary>
        void LoadStepName(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "Content";
            comboBoxEx.ValueMember = "Code";
            comboBoxEx.DataSource = CommonDAO.GetInstance().GetCodeContentByKind("流程状态");
        }

        public void ShowDeduct(String newId)
        {
            cmcsbuyfueltransportdeducts = Dbers.GetInstance().SelfDber.Entities<CmcsBuyFuelTransportDeduct>(" where TransportId=:TransportId", new { TransportId = newId });
            superGridControl1.PrimaryGrid.DataSource = cmcsbuyfueltransportdeducts;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string content_old = string.Format("修改前:车号:{0};供应商:{1};矿点:{2};煤种:{3};运输单位:{4};矿发量:{5};毛重:{6};皮重:{7};毛重时间:{8};皮重时间:{9};", cmcsBuyFuelTransport.CarNumber, cmcsBuyFuelTransport.SupplierName, cmcsBuyFuelTransport.MineName, cmcsBuyFuelTransport.FuelKindName, cmcsBuyFuelTransport.TransportCompanyName, cmcsBuyFuelTransport.TicketWeight, cmcsBuyFuelTransport.GrossWeight, cmcsBuyFuelTransport.TareWeight, cmcsBuyFuelTransport.GrossTime, cmcsBuyFuelTransport.TareTime);
            if (txt_SerialNumber.Text.Length == 0)
            {
                MessageBoxEx.Show("该车牌号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if ((cmcsBuyFuelTransport == null || cmcsBuyFuelTransport.CarNumber != txt_SerialNumber.Text))
            {
                if (Dbers.GetInstance().SelfDber.Entities<CmcsBuyFuelTransport>(" where CarNumber=:CarNumber and IsFinish=0", new { CarNumber = txt_SerialNumber.Text }).Count > 0)
                {
                    MessageBoxEx.Show("该车牌号不可重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (cmcsBuyFuelTransport != null)
            {
                cmcsBuyFuelTransport.SerialNumber = txt_SerialNumber.Text;
                cmcsBuyFuelTransport.CarNumber = txt_CarNumber.Text;
                if (cmcsSupplier != null)
                {
                    cmcsBuyFuelTransport.SupplierId = cmcsSupplier.Id;
                    cmcsBuyFuelTransport.SupplierName = cmcsSupplier.Name;
                }
                if (cmcsTransportCompany != null)
                {
                    cmcsBuyFuelTransport.TransportCompanyId = cmcsTransportCompany.Id;
                    cmcsBuyFuelTransport.TransportCompanyName = cmcsTransportCompany.Name;
                }
                if (cmcsMine != null)
                {
                    cmcsBuyFuelTransport.MineId = cmcsMine.Id;
                    cmcsBuyFuelTransport.MineName = cmcsMine.Name;
                }
                if (cmcsFuelKind != null)
                {
                    cmcsBuyFuelTransport.FuelKindId = cmcsFuelKind.Id;
                    cmcsBuyFuelTransport.FuelKindName = cmcsFuelKind.FuelName;
                }
                cmcsBuyFuelTransport.SamplingType = cmb_SampingType.Text;
                cmcsBuyFuelTransport.FPSamplePlace = cmb_Samping.Text;
                cmcsBuyFuelTransport.StepName = cmb_STEPNAME.Text;
                cmcsBuyFuelTransport.TicketWeight = (decimal)dbi_TicketWeight.Value;
                cmcsBuyFuelTransport.GrossWeight = (decimal)dbi_GrossWeight.Value;
                cmcsBuyFuelTransport.DeductWeight = (decimal)dbi_DeductWeight.Value;
                cmcsBuyFuelTransport.TareWeight = (decimal)dbi_TareWeight.Value;
                cmcsBuyFuelTransport.SuttleWeight = (decimal)dbi_SuttleWeight.Value;
                txt_Remark.Text = cmcsBuyFuelTransport.Remark;
                cmcsBuyFuelTransport.IsFinish = (chb_IsFinish.Checked ? 1 : 0);
                cmcsBuyFuelTransport.IsUse = (chb_IsUse.Checked ? 1 : 0);
                cmcsBuyFuelTransport.IsAutoDeduct = chbIsAutoDeduct.Checked ? 1 : 0;
                cmcsBuyFuelTransport.InFactoryTime = date_InFactoryTime.Value;
                cmcsBuyFuelTransport.GrossTime = date_GrossTime.Value;
                cmcsBuyFuelTransport.TareTime = date_TareTime.Value;
                cmcsBuyFuelTransport.OutFactoryTime = date_OutFactoryTime.Value;
                cmcsBuyFuelTransport.CheckWeight = cmcsBuyFuelTransport.SuttleWeight - cmcsBuyFuelTransport.DeductWeight;

                // 生成批次以及采制化三级编码数据 
                CmcsInFactoryBatch inFactoryBatch = CarTransportDAO.GetInstance().GCQCInFactoryBatchByBuyFuelTransport(cmcsBuyFuelTransport);
                CarTransportDAO.GetInstance().SaveBuyFuelTransport(cmcsBuyFuelTransport);
                if (cmcsBuyFuelTransport.IsFinish == 0)
                {
                    CarTransportDAO.GetInstance().SaveUnFinishTransport(cmcsBuyFuelTransport.Id, eCarType.入厂煤, cmcsBuyFuelTransport.AutotruckId);
                }
            }
            else
            {
                cmcsBuyFuelTransport = new CmcsBuyFuelTransport();
                cmcsBuyFuelTransport.SerialNumber = txt_SerialNumber.Text;
                cmcsBuyFuelTransport.CarNumber = txt_CarNumber.Text;
                if (cmcsSupplier != null)
                {
                    cmcsBuyFuelTransport.SupplierId = cmcsSupplier.Id;
                    cmcsBuyFuelTransport.SupplierName = cmcsSupplier.Name;
                }
                if (cmcsTransportCompany != null)
                {
                    cmcsBuyFuelTransport.TransportCompanyId = cmcsTransportCompany.Id;
                    cmcsBuyFuelTransport.TransportCompanyName = cmcsTransportCompany.Name;
                }
                if (cmcsMine != null)
                {
                    cmcsBuyFuelTransport.MineId = cmcsMine.Id;
                    cmcsBuyFuelTransport.MineName = cmcsMine.Name;
                }
                if (cmcsFuelKind != null)
                {
                    cmcsBuyFuelTransport.FuelKindId = cmcsFuelKind.Id;
                    cmcsBuyFuelTransport.FuelKindName = cmcsFuelKind.FuelName;
                }
                cmcsBuyFuelTransport.SamplingType = (string)cmb_SampingType.SelectedItem;
                cmcsBuyFuelTransport.FPSamplePlace = (string)cmb_Samping.SelectedItem;
                cmcsBuyFuelTransport.StepName = (string)cmb_STEPNAME.SelectedItem;
                cmcsBuyFuelTransport.TicketWeight = (decimal)dbi_TicketWeight.Value;
                cmcsBuyFuelTransport.GrossWeight = (decimal)dbi_GrossWeight.Value;
                cmcsBuyFuelTransport.DeductWeight = (decimal)dbi_DeductWeight.Value;
                cmcsBuyFuelTransport.TareWeight = (decimal)dbi_TareWeight.Value;
                cmcsBuyFuelTransport.SuttleWeight = (decimal)dbi_SuttleWeight.Value;
                txt_Remark.Text = cmcsBuyFuelTransport.Remark;
                cmcsBuyFuelTransport.IsFinish = (chb_IsFinish.Checked ? 1 : 0);
                cmcsBuyFuelTransport.IsUse = (chb_IsUse.Checked ? 1 : 0);
                Dbers.GetInstance().SelfDber.Insert(cmcsBuyFuelTransport);
                //SaveAndUpdate(cmcsBuyFuelTransport, cmcsbuyfueltransportdeducts);
            }
            string content_new = string.Format("修改后:车号:{0};供应商:{1};矿点:{2};煤种:{3};运输单位:{4};矿发量:{5};毛重:{6};皮重:{7};毛重时间:{8};皮重时间:{9};", cmcsBuyFuelTransport.CarNumber, cmcsBuyFuelTransport.SupplierName, cmcsBuyFuelTransport.MineName, cmcsBuyFuelTransport.FuelKindName, cmcsBuyFuelTransport.TransportCompanyName, cmcsBuyFuelTransport.TicketWeight, cmcsBuyFuelTransport.GrossWeight, cmcsBuyFuelTransport.TareWeight, cmcsBuyFuelTransport.GrossTime, cmcsBuyFuelTransport.TareTime);
            CommonDAO.GetInstance().SaveAppletLog(eAppletLogLevel.Warn, "修改入厂煤运输记录", content_old + "  " + content_new + "  " + SelfVars.LoginUser.UserName);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            FrmBuyFuelTransportDeduct_Oper frmEdit = new FrmBuyFuelTransportDeduct_Oper(String.Empty, cmcsBuyFuelTransport.Id, true, cmcsbuyfueltransportdeducts);
            if (frmEdit.ShowDialog() == DialogResult.OK)
            {
                cmcsbuyfueltransportdeducts = superGridControl1.PrimaryGrid.DataSource as List<CmcsBuyFuelTransportDeduct>;
                cmcsbuyfueltransportdeducts = cmcsbuyfueltransportdeducts.Where(a => a.Id != frmEdit.cmcsBuyFuelTransportDeduct.Id).ToList();
                cmcsbuyfueltransportdeducts.Add(frmEdit.cmcsBuyFuelTransportDeduct);
                superGridControl1.PrimaryGrid.DataSource = cmcsbuyfueltransportdeducts;
                dbi_DeductWeight.Value = (double)cmcsbuyfueltransportdeducts.Select(a => a.DeductWeight).Sum();
            }
        }

        void SaveAndUpdate(CmcsBuyFuelTransport item, List<CmcsBuyFuelTransportDeduct> details)
        {
            List<CmcsBuyFuelTransportDeduct> olds = Dbers.GetInstance().SelfDber.Entities<CmcsBuyFuelTransportDeduct>(" where TransportId=:TransportId", new { TransportId = item.Id });
            foreach (CmcsBuyFuelTransportDeduct old in olds)
            {
                CmcsBuyFuelTransportDeduct del = details.Where(a => a.Id == old.Id).FirstOrDefault();
                if (del == null)
                {
                    Dbers.GetInstance().SelfDber.Delete<CmcsBuyFuelTransportDeduct>(old.Id);
                }
            }
            foreach (var detail in details)
            {
                detail.TransportId = item.Id;
                CmcsBuyFuelTransportDeduct insertorupdate = olds.Where(a => a.Id == detail.Id).FirstOrDefault();
                if (insertorupdate == null)
                {
                    Dbers.GetInstance().SelfDber.Insert(detail);
                }
                else
                {
                    Dbers.GetInstance().SelfDber.Update(detail);
                }
            }
            List<CmcsBuyFuelTransportDeduct> news = Dbers.GetInstance().SelfDber.Entities<CmcsBuyFuelTransportDeduct>(" where TransportId=:TransportId", new { TransportId = item.Id });
            if (news != null && news.Count > 0)
            {
                item.DeductWeight = news.Sum(a => a.DeductWeight);
                //Dbers.GetInstance().SelfDber.Update(item);
            }
        }

        private void superGridControl1_CellMouseDown(object sender, DevComponents.DotNetBar.SuperGrid.GridCellMouseEventArgs e)
        {
            CmcsBuyFuelTransportDeduct entity = cmcsbuyfueltransportdeducts.Where(a => a.Id == superGridControl1.PrimaryGrid.GetCell(e.GridCell.GridRow.Index, superGridControl1.PrimaryGrid.Columns["clmId"].ColumnIndex).Value.ToString()).FirstOrDefault();
            switch (superGridControl1.PrimaryGrid.Columns[e.GridCell.ColumnIndex].Name)
            {
                case "clmShow":
                    FrmBuyFuelTransportDeduct_Oper frmShow = new FrmBuyFuelTransportDeduct_Oper(entity.Id, cmcsBuyFuelTransport.Id, false, cmcsbuyfueltransportdeducts);
                    if (frmShow.ShowDialog() == DialogResult.OK)
                    {
                        superGridControl1.PrimaryGrid.DataSource = cmcsbuyfueltransportdeducts;
                        dbi_DeductWeight.Value = (double)cmcsbuyfueltransportdeducts.Select(a => a.DeductWeight).Sum();
                    }
                    break;
                case "clmEdit":
                    FrmBuyFuelTransportDeduct_Oper frmEdit = new FrmBuyFuelTransportDeduct_Oper(entity.Id, cmcsBuyFuelTransport.Id, true, cmcsbuyfueltransportdeducts);
                    if (frmEdit.ShowDialog() == DialogResult.OK)
                    {
                        cmcsbuyfueltransportdeducts = cmcsbuyfueltransportdeducts.Where(a => a.Id != frmEdit.cmcsBuyFuelTransportDeduct.Id).ToList();
                        cmcsbuyfueltransportdeducts.Add(frmEdit.cmcsBuyFuelTransportDeduct);
                        superGridControl1.PrimaryGrid.DataSource = cmcsbuyfueltransportdeducts;
                        dbi_DeductWeight.Value = (double)cmcsbuyfueltransportdeducts.Select(a => a.DeductWeight).Sum();
                    }
                    break;
                case "clmDelete":

                    cmcsbuyfueltransportdeducts = cmcsbuyfueltransportdeducts.Where(a => a.Id != entity.Id).ToList();
                    superGridControl1.PrimaryGrid.DataSource = cmcsbuyfueltransportdeducts;
                    dbi_DeductWeight.Value = (double)cmcsbuyfueltransportdeducts.Select(a => a.DeductWeight).Sum();
                    CommonDAO.GetInstance().SelfDber.Delete<CmcsBuyFuelTransportDeduct>(entity.Id);
                    break;
            }
        }


        private void superGridControl1_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // 取消编辑
            e.Cancel = true;
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            FrmSupplier_Select Frm = new FrmSupplier_Select();
            Frm.ShowDialog();
            if (Frm.DialogResult == DialogResult.OK)
            {
                CmcsSupplier = Frm.Output;
            }
        }

        private void btnTransportCompany_Click(object sender, EventArgs e)
        {
            FrmTransportCompany_Select Frm = new FrmTransportCompany_Select();
            Frm.ShowDialog();
            if (Frm.DialogResult == DialogResult.OK)
            {
                CmcsTransportCompany = Frm.Output;
            }
        }

        private void BtnMine_Click(object sender, EventArgs e)
        {
            FrmMine_Select Frm = new FrmMine_Select();
            Frm.ShowDialog();
            if (Frm.DialogResult == DialogResult.OK)
            {
                CmcsMine = Frm.Output;
            }
        }

        private void btnFuelKind_Click(object sender, EventArgs e)
        {
            //FrmFuelKind_Select Frm = new FrmFuelKind_Select();
            //Frm.ShowDialog();
            //if (Frm.DialogResult == DialogResult.OK)
            //{
            //    cmcsMine = Frm.Output;
            //}
        }

        private void btnCarNumber_Click(object sender, EventArgs e)
        {
            FrmAutotruck_Select frm = new FrmAutotruck_Select("and CarType='" + eCarType.入厂煤.ToString() + "' and IsUse=1 order by CarNumber asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.CmcsAutotruck = frm.Output;
            }
        }
    }
}