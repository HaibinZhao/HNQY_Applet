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
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.Fuel;

namespace CMCS.CarTransport.Queue.Frms
{
    public partial class FrmBuyFuelForecast_Select : DevComponents.DotNetBar.Metro.MetroForm
    {
        /// <summary>
        /// ѡ�е�ʵ��
        /// </summary>
        public CmcsLMYB Output;

        private DateTime dtCurrent;
        /// <summary>
        /// Ԥ�Ƶ���ʱ��
        /// </summary>
        public DateTime DtCurrent
        {
            get { return dtCurrent; }
            set
            {
                dtCurrent = value;

                Search(value);
            }
        }

        public FrmBuyFuelForecast_Select()
        {
            InitializeComponent();
        }

        public FrmBuyFuelForecast_Select(string sqlWhere)
        {
            InitializeComponent();
        }

        private void FrmBuyFuelForecast_Select_Shown(object sender, EventArgs e)
        {
            this.DtCurrent = DateTime.Now;
        }

        private void FrmBuyFuelForecast_Select_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Output = null;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        void Search(DateTime inFactoryTime)
        {
            List<CmcsLMYB> list = CommonDAO.GetInstance().SelfDber.Entities<CmcsLMYB>("where to_char(InFactoryTime,'yyyymmdd')=to_char(:InFactoryTime,'yyyymmdd') order by InFactoryTime desc", new { InFactoryTime = inFactoryTime });
            superGridControl1.PrimaryGrid.DataSource = list;
        }

        void Return()
        {
            GridRow gridRow = superGridControl1.PrimaryGrid.ActiveRow as GridRow;
            if (gridRow == null) return;

            this.Output = (gridRow.DataItem as CmcsLMYB);
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

        /// <summary>
        /// ��һ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrevDay_Click(object sender, EventArgs e)
        {
            this.DtCurrent = this.DtCurrent.AddDays(-1);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToday_Click(object sender, EventArgs e)
        {
            this.DtCurrent = DateTime.Now;
        }

        /// <summary>
        /// ��һ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextDay_Click(object sender, EventArgs e)
        {
            this.DtCurrent = this.DtCurrent.AddDays(1);
        }
    }
}