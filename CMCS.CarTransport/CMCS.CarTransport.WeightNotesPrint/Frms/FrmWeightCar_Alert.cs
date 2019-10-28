using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using DevComponents.DotNetBar;
using CMCS.Common;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.Common.Entities;
using CMCS.CarTransport.DAO;
using CMCS.Common.DAO;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.BaseInfo;
using DevComponents.DotNetBar.Controls;

namespace CMCS.CarTransport.WeightNotesPrint.Frms
{
    public partial class FrmWeightCar_Alert : DevComponents.DotNetBar.Metro.MetroForm
    {

        String id = String.Empty;
        bool edit = false;

        CommonDAO commonDAO = CommonDAO.GetInstance();

        public FrmWeightCar_Alert()
        {
            InitializeComponent();
        }
        public FrmWeightCar_Alert(String pId, bool pEdit)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
        }

        /// <summary>
        /// 加载煤种
        /// </summary>
        void LoadFuelkind(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.DisplayMember = "FuelName";
            comboBoxEx.ValueMember = "Id";
            comboBoxEx.DataSource = Dbers.GetInstance().SelfDber.Entities<CmcsFuelKind>("where  IsUse='1' and ParentId is not null");
        }


        private CmcsSupplier selectedSupplier_BuyFuel;
        /// <summary>
        /// 选择的供煤单位
        /// </summary>
        public CmcsSupplier SelectedSupplier_BuyFuel
        {
            get { return selectedSupplier_BuyFuel; }
            set
            {
                selectedSupplier_BuyFuel = value;

                if (value != null)
                {
                    txtSupplierName_BuyFuel.Text = value.Name;
                }
                else
                {
                    txtSupplierName_BuyFuel.ResetText();
                }
            }
        }

        private CmcsTransportCompany selectedTransportCompany_BuyFuel;
        /// <summary>
        /// 选择的运输单位
        /// </summary>
        public CmcsTransportCompany SelectedTransportCompany_BuyFuel
        {
            get { return selectedTransportCompany_BuyFuel; }
            set
            {
                selectedTransportCompany_BuyFuel = value;

                if (value != null)
                {
                    txtTransportCompanyName_BuyFuel.Text = value.Name;
                }
                else
                {
                    txtTransportCompanyName_BuyFuel.ResetText();
                }
            }
        }

        private CmcsMine selectedMine_BuyFuel;
        /// <summary>
        /// 选择的矿点
        /// </summary>
        public CmcsMine SelectedMine_BuyFuel
        {
            get { return selectedMine_BuyFuel; }
            set
            {
                selectedMine_BuyFuel = value;

                if (value != null)
                {
                    txtMineName_BuyFuel.Text = value.Name;
                }
                else
                {
                    txtMineName_BuyFuel.ResetText();
                }

            }
        }

        private CmcsFuelKind selectedFuelKind_BuyFuel;
        /// <summary>
        /// 选择的煤种
        /// </summary>
        public CmcsFuelKind SelectedFuelKind_BuyFuel
        {
            get { return selectedFuelKind_BuyFuel; }
            set
            {
                if (value != null)
                {
                    selectedFuelKind_BuyFuel = value;
                    cmbFuelName_BuyFuel.Text = value.FuelName;
                }
                else
                {
                    cmbFuelName_BuyFuel.SelectedIndex = 0;
                }
            }
        }


        /// <summary>
        /// 选择供煤单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectSupplier_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmSupplier_Select frm = new FrmSupplier_Select("where IsUse='1' order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedSupplier_BuyFuel = frm.Output;
            }
        }

        /// <summary>
        /// 选择矿点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectMine_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmMine_Select frm = new FrmMine_Select("where IsUse='1' order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedMine_BuyFuel = frm.Output;
            }
        }

        /// <summary>
        /// 选择运输单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectTransportCompany_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmTransportCompany_Select frm = new FrmTransportCompany_Select("where IsUse=1 order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.SelectedTransportCompany_BuyFuel = frm.Output;
            }
        }

        private void FrmWeightCar_Alert_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;

            LoadFuelkind(cmbFuelName_BuyFuel);

            if (!string.IsNullOrEmpty(id))
            {
                CmcsBuyFuelTransport entity = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(id);

                if (entity != null)
                {
                    this.txtCarNumber.Text = entity.CarNumber;
                    this.SelectedSupplier_BuyFuel = commonDAO.SelfDber.Get<CmcsSupplier>(entity.SupplierId);
                    this.SelectedTransportCompany_BuyFuel = commonDAO.SelfDber.Get<CmcsTransportCompany>(entity.TransportCompanyId);
                    this.SelectedMine_BuyFuel = commonDAO.SelfDber.Get<CmcsMine>(entity.MineId);
                    this.SelectedFuelKind_BuyFuel = commonDAO.SelfDber.Get<CmcsFuelKind>(entity.FuelKindId);
                    this.txtTicketWeight_BuyFuel.Text = entity.TicketWeight.ToString();
                    this.txtTareWeight_BuyFuel.Text = entity.TareWeight.ToString();
                    this.txtGrossWeight_BuyFuel.Text = entity.GrossWeight.ToString();
                }
            }
        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (this.SelectedFuelKind_BuyFuel == null)
            {
                MessageBoxEx.Show("请选择煤种", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (this.SelectedMine_BuyFuel == null)
            {
                MessageBoxEx.Show("请选择矿点", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (this.SelectedSupplier_BuyFuel == null)
            {
                MessageBoxEx.Show("请选择供煤单位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (this.SelectedTransportCompany_BuyFuel == null)
            {
                MessageBoxEx.Show("请选择运输单位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtTicketWeight_BuyFuel.Value <= 0)
            {
                MessageBoxEx.Show("请输入有效的矿发量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (txtGrossWeight_BuyFuel.Value <= 0)
            {
                MessageBoxEx.Show("请输入有效的毛重", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtTareWeight_BuyFuel.Value <= 0)
            {
                MessageBoxEx.Show("请输入有效的皮重", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                CmcsBuyFuelTransport entity = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(id);

                if (entity != null)
                {

                    entity.SupplierId = SelectedSupplier_BuyFuel.Id;
                    entity.SupplierName = SelectedSupplier_BuyFuel.Name;
                    entity.MineId = SelectedMine_BuyFuel.Id;
                    entity.MineName = SelectedMine_BuyFuel.Name;
                    entity.TransportCompanyId = SelectedTransportCompany_BuyFuel.Id;
                    entity.TransportCompanyName = SelectedTransportCompany_BuyFuel.Name;
                    entity.FuelKindId = this.cmbFuelName_BuyFuel.SelectedValue.ToString();
                    entity.FuelKindName = this.cmbFuelName_BuyFuel.Text;
                    entity.TicketWeight = decimal.Parse(this.txtTicketWeight_BuyFuel.Text);
                    entity.GrossWeight = decimal.Parse(this.txtGrossWeight_BuyFuel.Text);
                    entity.TareWeight = decimal.Parse(this.txtTareWeight_BuyFuel.Text);
                    entity.SuttleWeight = entity.GrossWeight - entity.TareWeight;

                    if (commonDAO.SelfDber.Update(entity) > 0)
                    {
                        //更新运输记录到批次
                        commonDAO.InsertWaitForHandleEvent("汽车智能化_同步入厂煤运输记录到批次", entity.Id);

                        MessageBoxEx.Show("修改成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("保存失败\r\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }
    }
}