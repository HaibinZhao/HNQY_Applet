using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Metro;
using CMCS.Common;
using CMCS.Common.Entities.CarTransport;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.Common.Entities;
using CMCS.CarTransport.Queue.Frms.Transport.TransportPicture;
using CMCS.Common.Entities.Fuel;
using DevComponents.DotNetBar.Controls;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.DAO;
using CMCS.Common.Enums;
using CMCS.CarTransport.Queue.Core;
using CMCS.CarTransport.DAO;
using CMCS.Common.Utilities;

namespace CMCS.CarTransport.Queue.Frms.Transport.BuyFuelTransport
{
    public partial class FrmBuyFuelTransport_List : MetroAppForm
    {
        /// <summary>
        /// ����Ψһ��ʶ��
        /// </summary>
        public static string UniqueKey = "FrmBuyFuelTransport_List";


        /// <summary>
        /// ÿҳ��ʾ����
        /// </summary>
        int PageSize = 18;

        /// <summary>
        /// ��ҳ��
        /// </summary>
        int PageCount = 0;

        /// <summary>
        /// �ܼ�¼��
        /// </summary>
        int TotalCount = 0;

        /// <summary>
        /// ��ǰҳ����
        /// </summary>
        int CurrentIndex = 0;

        string SqlWhere = string.Empty;

        bool hasManagePower = false;
        /// <summary>
        /// �Է���ά��Ȩ��
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
                //superGridControl1.PrimaryGrid.Columns["clmEdit"].Visible = value;
                superGridControl1.PrimaryGrid.Columns["clmDelete"].Visible = value;
            }
        }

        /// <summary>
        /// �Ƿ��в鿴��Ӧ�̼�ú��Ȩ��
        /// </summary>
        public bool HasShowSupplier = false;

        public FrmBuyFuelTransport_List()
        {
            InitializeComponent();
        }

        private void FrmBuyFuelTransport_List_Load(object sender, EventArgs e)
        {
            //01�鿴 02���� 03�޸� 04ɾ��
            GridColumn clmEdit = superGridControl1.PrimaryGrid.Columns["clmEdit"];
            clmEdit.Visible = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "01", SelfVars.LoginUser);
            GridColumn clmDelete = superGridControl1.PrimaryGrid.Columns["clmDelete"];
            clmDelete.Visible = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "02", SelfVars.LoginUser);
            HasShowSupplier = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "03", SelfVars.LoginUser);

            HasManagePower = CommonDAO.GetInstance().HasResourcePowerByResCode(SelfVars.LoginUser.UserAccount, eUserRoleCodes.�������ܻ���Ϣά��.ToString());
            LoadFuelkind(cmbFuelName_BuyFuel);
            superGridControl1.PrimaryGrid.AutoGenerateColumns = false;
            dtpStartTime.Value = DateTime.Now;
            dtpEndTime.Value = DateTime.Now;
            btnSearch_Click(null, null);
        }

        /// <summary>
        /// ����ú��
        /// </summary>
        void LoadFuelkind(ComboBoxEx comboBoxEx)
        {
            comboBoxEx.Items.Add(new ComboBoxItem("", ""));
            IList<CmcsFuelKind> data = Dbers.GetInstance().SelfDber.Entities<CmcsFuelKind>("where  IsUse='1' and ParentId is not null");
            foreach (CmcsFuelKind item in data)
            {
                comboBoxEx.Items.Add(new ComboBoxItem(item.FuelName, item.FuelName));
            }
        }

        public void BindData()
        {
            string tempSqlWhere = this.SqlWhere;
            //List<CmcsBuyFuelTransport> list = Dbers.GetInstance().SelfDber.ExecutePager<CmcsBuyFuelTransport>(PageSize, CurrentIndex, tempSqlWhere + " order by SerialNumber desc");

            string sql = "select t.* from cmcstbbuyfueltransport t left join  fultbinfactorybatch a on t.infactorybatchid=a.id ";

            DataTable tb = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql+ tempSqlWhere+ " order by t.SerialNumber desc");
            IList<CmcsBuyFuelTransport> list = ConvertHelper<CmcsBuyFuelTransport>.ConvertToList(tb);
            //List<CmcsBuyFuelTransport> list = Dbers.GetInstance().SelfDber.Entities<CmcsBuyFuelTransport>(tempSqlWhere + " order by SerialNumber desc");
            CmcsBuyFuelTransport total = new CmcsBuyFuelTransport();
            total.SerialNumber = "�ϼ�";
            total.GrossWeight = list.Sum(a => a.GrossWeight);
            total.TareWeight = list.Sum(a => a.TareWeight);
            total.SuttleWeight = list.Sum(a => a.SuttleWeight);
            total.DeductWeight = list.Sum(a => a.DeductWeight);
            total.TicketWeight = list.Sum(a => a.TicketWeight);
            list.Add(total);
            superGridControl1.PrimaryGrid.DataSource = list;
            
            //GetTotalCount(tempSqlWhere);
            //PagerControlStatue();

            //lblPagerInfo.Text = string.Format("�� {0} ����¼��ÿҳ {1} ������ {2} ҳ����ǰ�� {3} ҳ", TotalCount, PageSize, PageCount, CurrentIndex + 1);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SqlWhere = " where 1=1";

            if (!string.IsNullOrEmpty(txtSupplierName_BuyFuel.Text)) this.SqlWhere += " and t.SupplierName like '%" + txtSupplierName_BuyFuel.Text + "%'";
            if (!string.IsNullOrEmpty(txtMineName_BuyFuel.Text)) this.SqlWhere += " and t.MineName like '%" + txtMineName_BuyFuel.Text + "%'";
            if (!string.IsNullOrEmpty(cmbFuelName_BuyFuel.Text)) this.SqlWhere += " and t.FuelKindName like '%" + cmbFuelName_BuyFuel.Text + "%'";
            if (dtpStartTime.Value.Year > 2000) this.SqlWhere += " and t.InFactoryTime >= '" + dtpStartTime.Value.Date + "'";
            if (dtpEndTime.Value.Year > 2000) this.SqlWhere += " and t.InFactoryTime < '" + dtpEndTime.Value.AddDays(1).Date + "'";
            if (!string.IsNullOrEmpty(txtCarNumber_Ser.Text)) this.SqlWhere += " and t.CarNumber like '%" + txtCarNumber_Ser.Text + "%'";
            if (!string.IsNullOrEmpty(txt_BatchNo.Text)) this.SqlWhere += " and a.batch like '%" + txt_BatchNo.Text + "%'";
            CurrentIndex = 0;
            BindData();
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            this.SqlWhere = string.Empty;
            txtCarNumber_Ser.Text = string.Empty;

            CurrentIndex = 0;
            BindData();
        }

        private void btnInStore_Click(object sender, EventArgs e)
        {
            FrmBuyFuelTransport_Oper frm = new FrmBuyFuelTransport_Oper();
            frm.ShowDialog();

            BindData();
        }

        #region Pager

        private void btnPagerCommand_Click(object sender, EventArgs e)
        {
            ButtonX btn = sender as ButtonX;
            switch (btn.CommandParameter.ToString())
            {
                case "First":
                    CurrentIndex = 0;
                    break;
                case "Previous":
                    CurrentIndex = CurrentIndex - 1;
                    break;
                case "Next":
                    CurrentIndex = CurrentIndex + 1;
                    break;
                case "Last":
                    CurrentIndex = PageCount - 1;
                    break;
            }

            BindData();
        }

        public void PagerControlStatue()
        {
            if (PageCount <= 1)
            {
                btnFirst.Enabled = false;
                btnPrevious.Enabled = false;
                btnLast.Enabled = false;
                btnNext.Enabled = false;

                return;
            }

            if (CurrentIndex == 0)
            {
                // ��ҳ
                btnFirst.Enabled = false;
                btnPrevious.Enabled = false;
                btnLast.Enabled = true;
                btnNext.Enabled = true;
            }

            if (CurrentIndex > 0 && CurrentIndex < PageCount - 1)
            {
                // ��һҳ/��һҳ
                btnFirst.Enabled = true;
                btnPrevious.Enabled = true;
                btnLast.Enabled = true;
                btnNext.Enabled = true;
            }

            if (CurrentIndex == PageCount - 1)
            {
                // ĩҳ
                btnFirst.Enabled = true;
                btnPrevious.Enabled = true;
                btnLast.Enabled = false;
                btnNext.Enabled = false;
            }
        }

        private void GetTotalCount(string sqlWhere)
        {
            TotalCount = Dbers.GetInstance().SelfDber.Count<CmcsBuyFuelTransport>(sqlWhere);
            if (TotalCount % PageSize != 0)
                PageCount = TotalCount / PageSize + 1;
            else
                PageCount = TotalCount / PageSize;
        }
        #endregion

        #region DataGridView

        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            CmcsBuyFuelTransport entity = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(superGridControl1.PrimaryGrid.GetCell(e.ColumnIndex, superGridControl1.PrimaryGrid.Columns["clmId"].ColumnIndex).Value.ToString());
            if (entity == null)
                return;

            switch (superGridControl1.PrimaryGrid.Columns[e.ColumnIndex].Name)
            {
                case "clmDelete":
                    // ��ѯ����ʹ�øü�¼�ĳ��� 
                    if (MessageBoxEx.Show("ȷ��Ҫɾ���ü�¼��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Dbers.GetInstance().SelfDber.Delete<CmcsBuyFuelTransport>(entity.Id);

                        BindData();
                    }
                    else
                        MessageBoxEx.Show("�ü�¼����ʹ���У���ֹɾ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        private void dataGridViewX1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
        }

        private void dataGridViewX1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            //CmcsBuyFuelTransport entity = superGridControl1.PrimaryGrid.Rows[e.RowIndex] as CmcsBuyFuelTransport;

            switch (superGridControl1.PrimaryGrid.Columns[e.ColumnIndex].Name)
            {
                case "clmDelete":
                    break;
            }
        }

        #endregion

        private void superGridControl1_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // ȡ���༭
            e.Cancel = true;
        }

        private void superGridControl1_CellMouseDown(object sender, DevComponents.DotNetBar.SuperGrid.GridCellMouseEventArgs e)
        {
            CmcsBuyFuelTransport entity = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(superGridControl1.PrimaryGrid.GetCell(e.GridCell.GridRow.Index, superGridControl1.PrimaryGrid.Columns["clmId"].ColumnIndex).Value.ToString());
            if (entity == null || entity.SerialNumber == "�ϼ�") return;
            switch (superGridControl1.PrimaryGrid.Columns[e.GridCell.ColumnIndex].Name)
            {
                case "clmPrint":
                    if (entity.SuttleWeight <= 0)
                    {
                        MessageBoxEx.Show("�����쳣����ֹ��ӡ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                    if (entity.ProfitWeight > 0 && entity.ProfitWeight >= Convert.ToDecimal(CommonDAO.GetInstance().GetAppletConfigDouble("ӯ�쳣����")))
                    {
                        MessageBoxEx.Show("ӯ���쳣", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    if (entity.ProfitWeight < 0 && entity.ProfitWeight >= Convert.ToDecimal(CommonDAO.GetInstance().GetAppletConfigDouble("���쳣����")))
                    {
                        MessageBoxEx.Show("�����쳣", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    FrmPrint_Oper frmPrint = new FrmPrint_Oper(entity.Id, false);
                    if (frmPrint.ShowDialog() == DialogResult.OK)
                    {
                        BindData();
                    }
                    break;
                case "clmShow":
                    FrmBuyFuelTransport_Oper frmShow = new FrmBuyFuelTransport_Oper(entity.Id, false);
                    if (frmShow.ShowDialog() == DialogResult.OK)
                    {
                        BindData();
                    }
                    break;
                case "clmEdit":
                    FrmBuyFuelTransport_Oper frmEdit = new FrmBuyFuelTransport_Oper(entity.Id, true);
                    if (frmEdit.ShowDialog() == DialogResult.OK)
                    {
                        BindData();
                    }
                    break;
                case "clmDelete":
                    // ��ѯ����ʹ�øü�¼�ĳ��� 
                    if (MessageBoxEx.Show("ȷ��Ҫɾ���ü�¼��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            CommonDAO.GetInstance().SaveAppletLog(eAppletLogLevel.Warn, "ɾ���볧ú�����¼", string.Format("����:{0};ë��:{1};Ƥ��:{2};���:{3};������:{4}", entity.CarNumber, entity.GrossWeight, entity.TareWeight, entity.MineName, SelfVars.LoginUser.UserName));
                            CommonDAO.GetInstance().RemoveDeduct(entity.Id);
                            Dbers.GetInstance().SelfDber.Delete<CmcsBuyFuelTransport>(entity.Id);
                            CommonDAO.GetInstance().RemoveUnFinishTransport(entity.Id);
                        }
                        catch (Exception)
                        {
                            MessageBoxEx.Show("�ü�¼����ʹ���У���ֹɾ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        BindData();
                    }
                    break;
                case "clmPic":

                    if (Dbers.GetInstance().SelfDber.Entities<CmcsTransportPicture>(String.Format(" where TransportId='{0}'", entity.Id)).Count > 0)
                    {
                        FrmTransportPicture frmPic = new FrmTransportPicture(entity.Id, entity.CarNumber);
                        if (frmPic.ShowDialog() == DialogResult.OK)
                        {
                            BindData();
                        }
                    }
                    else
                    {
                        MessageBoxEx.Show("����ץ��ͼƬ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    break;
            }
        }

        private void superGridControl1_DataBindingComplete(object sender, DevComponents.DotNetBar.SuperGrid.GridDataBindingCompleteEventArgs e)
        {

            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsBuyFuelTransport entity = gridRow.DataItem as CmcsBuyFuelTransport;
                if (entity == null) return;

                // �����Ч״̬
                gridRow.Cells["clmIsUse"].Value = (entity.IsUse == 1 ? "��" : "��");
                // ����Ƿ��Ѵ�ӡ״̬
                gridRow.Cells["clmIsPrint"].Value = (entity.IsPrint == 1 ? "�Ѵ�ӡ" : "");
                CmcsInFactoryBatch cmcsinfactorybatch = Dbers.GetInstance().SelfDber.Get<CmcsInFactoryBatch>(entity.InFactoryBatchId);
                if (cmcsinfactorybatch != null)
                {
                    gridRow.Cells["clmInFactoryBatchNumber"].Value = cmcsinfactorybatch.Batch;
                }
                if (!HasShowSupplier)
                {
                    gridRow.Cells["clmSupplierName"].Value = "****";
                    gridRow.Cells["clmFuelKind"].Value = "****";
                    gridRow.Cells["clmMineName"].Value = "****";
                }
                if (entity.SerialNumber == "�ϼ�")
                {
                    gridRow.Cells["clmPrint"].Value = string.Empty;
                    gridRow.Cells["clmShow"].Value = string.Empty;
                    gridRow.Cells["clmEdit"].Value = string.Empty;
                    gridRow.Cells["clmDelete"].Value = string.Empty;
                    gridRow.Cells["clmPic"].Value = string.Empty;

                    gridRow.Cells["clmInFactoryTime"].Visible = false;
                    gridRow.Cells["clmIsUse"].Visible = false;
                }

                //List<CmcsTransportPicture> cmcstrainwatchs = Dbers.GetInstance().SelfDber.Entities<CmcsTransportPicture>(String.Format(" where TransportId='{0}'", gridRow.Cells["clmId"].Value));
                //if (cmcstrainwatchs.Count == 0)
                //{
                //    //gridRow.Cells["clmPic"].Value = "";
                //}

            }
        }

        private void btnSelectSupplier_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmSupplier_Select frm = new FrmSupplier_Select("where IsUse='1' order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.txtSupplierName_BuyFuel.Text = frm.Output.Name;
            }
        }

        private void btnSelectMine_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmMine_Select frm = new FrmMine_Select("where IsUse='1' order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.txtMineName_BuyFuel.Text = frm.Output.Name;
            }
        }

        private void superGridControl1_GetRowHeaderText(object sender, GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.RowIndex + 1).ToString();
        }
    }
}
