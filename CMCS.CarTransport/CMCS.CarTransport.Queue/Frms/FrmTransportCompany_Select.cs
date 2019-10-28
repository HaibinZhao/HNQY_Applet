using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.Common;
using DevComponents.DotNetBar.SuperGrid;
using CMCS.Common.Entities;
using CMCS.CarTransport.DAO;
using CMCS.Common.DAO;
using CMCS.Common.Entities.BaseInfo;

namespace CMCS.CarTransport.Queue.Frms
{
    public partial class FrmTransportCompany_Select : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// 选中的实体
        /// </summary>
        public CmcsTransportCompany Output;

        /// <summary>
        /// 条件语句
        /// </summary>
        string sqlWhere;

        public FrmTransportCompany_Select()
        {
            InitializeComponent();
        }

        public FrmTransportCompany_Select(string sqlWhere)
        {
            InitializeComponent();

            this.sqlWhere = sqlWhere;
        }

        private void FrmTransportCompany_Select_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Output = null;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (superGridControl1.PrimaryGrid.Rows.Count > 0) superGridControl1.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                Return();
            }
            else if (!string.IsNullOrEmpty(txtInput.Text.Trim()))
            {
                Search(txtInput.Text.Trim());
            }
            else if (string.IsNullOrEmpty(txtInput.Text.Trim()))
            {
                superGridControl1.PrimaryGrid.DataSource = new List<CmcsTransportCompany>();
            }
        }

        void Search(string input)
        {
            List<CmcsTransportCompany> list = CommonDAO.GetInstance().GetTransportCompanyByNameOrChs(input.Trim(), sqlWhere);
            superGridControl1.PrimaryGrid.DataSource = list;
        }

        void Return()
        {
            GridRow gridRow = superGridControl1.PrimaryGrid.ActiveRow as GridRow;
            if (gridRow == null) return;

            this.Output = (gridRow.DataItem as CmcsTransportCompany);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void superGridControl1_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // 取消编辑模式
            e.Cancel = true;
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

        private void superGridControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Return();
        }  

        private void superGridControl1_CellDoubleClick(object sender, GridCellDoubleClickEventArgs e)
        {
            Return();
        }
    }
}