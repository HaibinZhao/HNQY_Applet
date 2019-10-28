using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Metro;
using CMCS.Common;
using CMCS.Common.Entities.CarTransport;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.Common.Entities;
using CMCS.CarTransport.Queue.Frms.Transport.TransportPicture;
using CMCS.CarTransport.Queue.Frms.Transport.BuyFuelTransport;
using CMCS.CarTransport.Queue.Core;
using CMCS.CarTransport.DAO;
using CMCS.Common.DAO;

namespace CMCS.CarTransport.Queue.Frms.Transport.GoodsTransport
{
    public partial class FrmGoodsTransport_List : MetroAppForm
    {
        /// <summary>
        /// ����Ψһ��ʶ��
        /// </summary>
        public static string UniqueKey = "FrmGoodsTransport_List";

        CommonDAO commonDAO = CommonDAO.GetInstance();
        /// <summary>
        /// ÿҳ��ʾ����
        /// </summary>
        int PageSize = 35;

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

                superGridControl1.PrimaryGrid.Columns["clmDelete"].Visible = value;
            }
        }

        public FrmGoodsTransport_List()
        {
            InitializeComponent();
        }

        private void FrmGoodsTransport_List_Load(object sender, EventArgs e)
        {
            //01�鿴 02���� 03�޸� 04ɾ��
            GridColumn clmEdit = superGridControl1.PrimaryGrid.Columns["clmEdit"];
            clmEdit.Visible = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "01", SelfVars.LoginUser);
            GridColumn clmDelete = superGridControl1.PrimaryGrid.Columns["clmDelete"];
            clmDelete.Visible = QueuerDAO.GetInstance().CheckPower(this.GetType().ToString(), "02", SelfVars.LoginUser);


            superGridControl1.PrimaryGrid.AutoGenerateColumns = false;

            dtpStartTime.Value = DateTime.Now;
            dtpEndTime.Value = DateTime.Now;
            btnSearch_Click(null, null);
        }

        public void BindData()
        {
            string tempSqlWhere = this.SqlWhere;
            commonDAO.SetSignalDataValue("�����������ʲ�ѯ1", Guid.NewGuid().ToString(), tempSqlWhere);
            List<CmcsGoodsTransport> list = Dbers.GetInstance().SelfDber.ExecutePager<CmcsGoodsTransport>(PageSize, CurrentIndex, tempSqlWhere + " order by SerialNumber desc");
            commonDAO.SetSignalDataValue("�����������ʲ�ѯ2", Guid.NewGuid().ToString(), list.Count.ToString());
            //List<CmcsGoodsTransport> list = Dbers.GetInstance().SelfDber.Entities<CmcsGoodsTransport>(tempSqlWhere + " order by SerialNumber desc");
            superGridControl1.PrimaryGrid.DataSource = list;

            GetTotalCount(tempSqlWhere);
            PagerControlStatue();

            lblPagerInfo.Text = string.Format("�� {0} ����¼��ÿҳ {1} ������ {2} ҳ����ǰ�� {3} ҳ", TotalCount, PageSize, PageCount, CurrentIndex + 1);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SqlWhere = " where 1=1";

            if (!string.IsNullOrEmpty(txtCarNumber_Ser.Text)) this.SqlWhere += " and CarNumber like '%" + txtCarNumber_Ser.Text + "%'";
            if (dtpStartTime.Value.Year > 2000) this.SqlWhere += " and InFactoryTime>='" + dtpStartTime.Value.Date + "'";
            if (dtpEndTime.Value.Year > 2000) this.SqlWhere += " and InFactoryTime<'" + dtpEndTime.Value.AddDays(1).Date + "'";
            if (!string.IsNullOrEmpty(txtSupplyUnitName_Goods.Text)) this.SqlWhere += " and SupplyUnitName = '" + txtSupplyUnitName_Goods.Text + "'";
            if (!string.IsNullOrEmpty(txtGoodsTypeName_Goods.Text)) this.SqlWhere += " and GoodsTypeName = '" + txtGoodsTypeName_Goods.Text + "'";
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
            FrmGoodsTransport_Oper frm = new FrmGoodsTransport_Oper();
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
            TotalCount = Dbers.GetInstance().SelfDber.Count<CmcsGoodsTransport>(sqlWhere);
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

            CmcsGoodsTransport entity = Dbers.GetInstance().SelfDber.Get<CmcsGoodsTransport>(superGridControl1.PrimaryGrid.GetCell(e.ColumnIndex, superGridControl1.PrimaryGrid.Columns["clmId"].ColumnIndex).Value.ToString());
            if (entity == null)
                return;

            switch (superGridControl1.PrimaryGrid.Columns[e.ColumnIndex].Name)
            {
                case "clmDelete":
                    // ��ѯ����ʹ�øü�¼�ĳ��� 
                    if (MessageBoxEx.Show("ȷ��Ҫɾ���ü�¼��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Dbers.GetInstance().SelfDber.Delete<CmcsGoodsTransport>(entity.Id);

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

            //CmcsGoodsTransport entity = superGridControl1.PrimaryGrid.Rows[e.RowIndex] as CmcsGoodsTransport;

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
            CmcsGoodsTransport entity = Dbers.GetInstance().SelfDber.Get<CmcsGoodsTransport>(superGridControl1.PrimaryGrid.GetCell(e.GridCell.GridRow.Index, superGridControl1.PrimaryGrid.Columns["clmId"].ColumnIndex).Value.ToString());
            switch (superGridControl1.PrimaryGrid.Columns[e.GridCell.ColumnIndex].Name)
            {
                case "clmPrint":
                    if (entity.SuttleWeight <= 0)
                    {
                        MessageBoxEx.Show("�����쳣����ֹ��ӡ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                    FrmPrint_Oper frmPrint = new FrmPrint_Oper(entity.Id, false, "��������");
                    if (frmPrint.ShowDialog() == DialogResult.OK)
                    {
                        BindData();
                    }
                    break;
                case "clmShow":
                    FrmGoodsTransport_Oper frmShow = new FrmGoodsTransport_Oper(entity.Id, false);
                    if (frmShow.ShowDialog() == DialogResult.OK)
                    {
                        BindData();
                    }
                    break;
                case "clmEdit":
                    FrmGoodsTransport_Oper frmEdit = new FrmGoodsTransport_Oper(entity.Id, true);
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
                            Dbers.GetInstance().SelfDber.Delete<CmcsGoodsTransport>(entity.Id);
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

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            FrmGoodsTransport_Oper frmEdit = new FrmGoodsTransport_Oper(String.Empty, true);
            if (frmEdit.ShowDialog() == DialogResult.OK)
            {
                BindData();
            }
        }

        #region DataGridView
        private void superGridControl1_DataBindingComplete(object sender, DevComponents.DotNetBar.SuperGrid.GridDataBindingCompleteEventArgs e)
        {

            foreach (GridRow gridRow in e.GridPanel.Rows)
            {
                CmcsGoodsTransport entity = gridRow.DataItem as CmcsGoodsTransport;
                if (entity == null) return;

                // �����Ч״̬
                gridRow.Cells["clmIsUse"].Value = (entity.IsUse == 1 ? "��" : "��");


                // ����Ƿ��Ѵ�ӡ״̬
                gridRow.Cells["clmIsPrint"].Value = (entity.IsPrint == 1 ? "�Ѵ�ӡ" : "");
            }
        }
        #endregion


        /// <summary>
        /// ѡ�񹩻���λ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnbtnSelectSupply_Goods_Click(object sender, EventArgs e)
        {
            FrmSupplyReceive_Select frm = new FrmSupplyReceive_Select("where IsValid=1 order by UnitName asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.txtSupplyUnitName_Goods.Text = frm.Output.UnitName;
            }
        }

        /// <summary>
        /// ѡ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectGoodsType_Goods_Click(object sender, EventArgs e)
        {
            FrmGoodsType_Select frm = new FrmGoodsType_Select();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.txtGoodsTypeName_Goods.Text = frm.Output.GoodsName;
            }
        }

    }
}
