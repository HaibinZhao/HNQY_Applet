using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.CarTransport.WeightNotesPrint.Utilities;

namespace CMCS.CarTransport.WeightNotesPrint.Frms
{
    public partial class FrmWeightCar_Goods_List : DevComponents.DotNetBar.Metro.MetroAppForm
    {
        /// <summary>
        /// 窗体唯一标识符
        /// </summary>
        public static string UniqueKey = "FrmWeightNotesPrint_Goods_List";


        /// <summary>
        /// 每页显示行数
        /// </summary>
        int PageSize = 18;

        /// <summary>
        /// 总页数
        /// </summary>
        int PageCount = 0;

        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalCount = 0;

        /// <summary>
        /// 当前页索引
        /// </summary>
        int CurrentIndex = 0;

        string SqlWhere = string.Empty;

        bool hasManagePower = false;
        /// <summary>
        /// 对否有维护权限
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

        public FrmWeightCar_Goods_List()
        {
            InitializeComponent();
        }

        private void FrmSupplier_List_Load(object sender, EventArgs e)
        {
            superGridControl1.PrimaryGrid.AutoGenerateColumns = false;
            dtiptStartArriveTime.Value = DateTime.Now.Date;
            dtiptEndArriveTime.Value = dtiptStartArriveTime.Value.AddDays(1).AddMilliseconds(-1);

            btnSearch_Click(null, null);
        }

        public void BindData()
        {
            string tempSqlWhere = this.SqlWhere;
            List<CmcsGoodsTransport> list = Dbers.GetInstance().SelfDber.ExecutePager<CmcsGoodsTransport>(PageSize, CurrentIndex, tempSqlWhere + " order by InFactoryTime desc");
            superGridControl1.PrimaryGrid.DataSource = list;

            GetTotalCount(tempSqlWhere);
            PagerControlStatue();

            lblPagerInfo.Text = string.Format("共 {0} 条记录，每页 {1} 条，共 {2} 页，当前第 {3} 页", TotalCount, PageSize, PageCount, CurrentIndex + 1);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            this.SqlWhere = " where 1=1 ";

            if (!string.IsNullOrEmpty(txtName_Ser.Text.Trim())) this.SqlWhere += " and SupplyUnitName like '% " + txtName_Ser.Text.Trim() + "%'";
            this.SqlWhere += String.Format(" and InFactoryTime >= '{0}' ", dtiptStartArriveTime.Value.ToString("yyyy-MM-dd HH:mm:00"));
            this.SqlWhere += String.Format(" and InFactoryTime <= '{0}' ", dtiptEndArriveTime.Value.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:00"));

            CurrentIndex = 0;
            BindData();
        }

        /// <summary>
        /// 入厂时间改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtiptStartArriveTime_ValueObjectChanged(object sender, EventArgs e)
        {
            btnSearch_Click(null, null);
        }

        /// <summary>
        /// 入厂时间改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtiptEndArriveTime_ValueObjectChanged(object sender, EventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            this.SqlWhere = string.Empty;
            //txtName_Ser.Text = string.Empty;

            CurrentIndex = 0;
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
                // 首页
                btnFirst.Enabled = false;
                btnPrevious.Enabled = false;
                btnLast.Enabled = true;
                btnNext.Enabled = true;
            }

            if (CurrentIndex > 0 && CurrentIndex < PageCount - 1)
            {
                // 上一页/下一页
                btnFirst.Enabled = true;
                btnPrevious.Enabled = true;
                btnLast.Enabled = true;
                btnNext.Enabled = true;
            }

            if (CurrentIndex == PageCount - 1)
            {
                // 末页
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

            CmcsGoodsTransport entity = Dbers.GetInstance().SelfDber.Get<CmcsGoodsTransport>(superGridControl1.PrimaryGrid.GetCell(e.ColumnIndex, superGridControl1.PrimaryGrid.Columns["clmId"].ColumnIndex).Value.ToString());
            if (entity == null)
                return;

            switch (superGridControl1.PrimaryGrid.Columns[e.ColumnIndex].Name)
            {
                case "clmShow":
                    FrmWeightCar_Oper frmShow = new FrmWeightCar_Oper(entity.Id, false, "其他物资");
                    if (frmShow.ShowDialog() == DialogResult.OK)
                    {
                        BindData();
                    }
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
            // 取消编辑
            e.Cancel = true;
        }

        private void superGridControl1_CellMouseDown(object sender, DevComponents.DotNetBar.SuperGrid.GridCellMouseEventArgs e)
        {
            CmcsGoodsTransport entity = Dbers.GetInstance().SelfDber.Get<CmcsGoodsTransport>(superGridControl1.PrimaryGrid.GetCell(e.GridCell.GridRow.Index, superGridControl1.PrimaryGrid.Columns["clmId"].ColumnIndex).Value.ToString());
            switch (superGridControl1.PrimaryGrid.Columns[e.GridCell.ColumnIndex].Name)
            {

                case "clmShow":
                    FrmWeightCar_Oper frmShow = new FrmWeightCar_Oper(entity.Id, false, "其他物资");
                    if (frmShow.ShowDialog() == DialogResult.OK)
                    {
                        BindData();
                    }
                    break;
            }
        }

        private void superGridControl1_DataBindingComplete(object sender, DevComponents.DotNetBar.SuperGrid.GridDataBindingCompleteEventArgs e)
        {

        }
        /// <summary>
        /// 设置行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl_GetRowHeaderText(object sender, DevComponents.DotNetBar.SuperGrid.GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.RowIndex + 1).ToString();

        }

        /// <summary>
        /// 已打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYesPrint_Click(object sender, EventArgs e)
        {
            this.SqlWhere = " where 1=1 ";

            if (!string.IsNullOrEmpty(txtName_Ser.Text.Trim())) this.SqlWhere += " and SupplyUnitName like '% " + txtName_Ser.Text.Trim() + "%'";
            this.SqlWhere += String.Format(" and InFactoryTime >= '{0}' ", dtiptStartArriveTime.Value.ToString("yyyy-MM-dd HH:mm:00"));
            this.SqlWhere += String.Format(" and InFactoryTime <= '{0}' ", dtiptEndArriveTime.Value.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:00"));

            this.SqlWhere += " and FirstWeight!=0 and SecondWeight!=0 ";

            this.SqlWhere += " and isfinish!=0";

            CurrentIndex = 0;
            BindData();
        }

        /// <summary>
        /// 未打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNoPrint_Click(object sender, EventArgs e)
        {
            this.SqlWhere = " where 1=1 ";

            if (!string.IsNullOrEmpty(txtName_Ser.Text.Trim())) this.SqlWhere += " and SupplyUnitName like '% " + txtName_Ser.Text.Trim() + "%'";
            this.SqlWhere += String.Format(" and InFactoryTime >= '{0}' ", dtiptStartArriveTime.Value.ToString("yyyy-MM-dd HH:mm:00"));
            this.SqlWhere += String.Format(" and InFactoryTime <= '{0}' ", dtiptEndArriveTime.Value.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:00"));

            this.SqlWhere += " and FirstWeight!=0 and SecondWeight!=0 ";

            this.SqlWhere += " and isfinish=0";

            CurrentIndex = 0;

            BindData();
        }
    }
}