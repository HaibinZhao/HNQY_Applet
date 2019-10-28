using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CMCS.CarTransport.DAO;
using CMCS.CarTransport.Views;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.Common.Entities.CarTransport;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.SuperGrid;

namespace CMCS.CarTransport.BeltSampler.Frms
{
    public partial class FrmUnFinishTransport_Select : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// ѡ�е�ʵ��
        /// </summary>
        public View_UnFinishTransport Output;

        /// <summary>
        /// �������
        /// </summary>
        string sqlWhere;

        public FrmUnFinishTransport_Select(string sqlWhere)
        {
            InitializeComponent();

            this.sqlWhere = sqlWhere;
        }

        private void FrmUnFinishTransport_Select_Load(object sender, EventArgs e)
        {
            Search(string.Empty);
        }

        private void FrmUnFinishTransport_Select_KeyUp(object sender, KeyEventArgs e)
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
            else
            {
                Search(txtInput.Text.Trim());
            }
        }

        void Search(string input)
        {
            List<View_UnFinishTransport> list = new List<View_UnFinishTransport>();
            if (!string.IsNullOrEmpty(input))
                list = CarTransportDAO.GetInstance().GetUnFinishTransportByCarNumber(input.Trim(), this.sqlWhere);
            else
                list = CommonDAO.GetInstance().SelfDber.Entities<View_UnFinishTransport>(this.sqlWhere);

            superGridControl1.PrimaryGrid.DataSource = list;
        }

        void Return()
        {
            GridRow gridRow = superGridControl1.PrimaryGrid.ActiveRow as GridRow;
            if (gridRow == null) return;

            this.Output = (gridRow.DataItem as View_UnFinishTransport);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void superGridControl1_BeginEdit(object sender, DevComponents.DotNetBar.SuperGrid.GridEditEventArgs e)
        {
            // ȡ���༭ģʽ
            e.Cancel = true;
        }

        /// <summary>
        /// �����к�
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