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
using CMCS.Common.Entities.Fuel;
using CMCS.CarTransport.Queue.Frms.Transport.Print;
using CMCS.Common.Enums;
using System.Linq;

namespace CMCS.CarTransport.Queue.Frms.Transport.BuyFuelTransport
{
    public partial class FrmBuyFuelTransport_Collect : MetroAppForm
    {
        /// <summary>
        /// 窗体唯一标识符
        /// </summary>
        public static string UniqueKey = "FrmBuyFuelTransport_Collect";

        WagonPrinterCollect wagonPrinter = null;
        List<CmcsBuyFuelTransport> listCount = new List<CmcsBuyFuelTransport>();

        string SqlWhere = string.Empty;

        public FrmBuyFuelTransport_Collect()
        {
            InitializeComponent();
        }

        private void FrmBuyFuelTransport_List_Load(object sender, EventArgs e)
        {
            superGridControl1.PrimaryGrid.AutoGenerateColumns = false;

            dtpStartTime.Value = DateTime.Now;
            dtpEndTime.Value = DateTime.Now;

            this.wagonPrinter = new WagonPrinterCollect(printDocument1);

            btnSearch_Click(null, null);
        }

        public void BindData()
        {
            listCount.Clear();
            string tempSqlWhere = this.SqlWhere;
            List<CmcsBuyFuelTransport> list = Dbers.GetInstance().SelfDber.Entities<CmcsBuyFuelTransport>(tempSqlWhere + " order by SerialNumber desc");
            var minename = from p in list group p by new { p.MineName } into g select new { MineName = g.Key.MineName };

            foreach (var item in minename)
            {
                List<CmcsBuyFuelTransport> listone = list.Where(a => a.MineName == item.MineName).ToList();
                if (listone != null && listone.Count > 0)
                {
                    CmcsBuyFuelTransport entity = new CmcsBuyFuelTransport();
                    entity.MineName = listone[0].MineName;
                    entity.TicketWeight = listone.Sum(a => a.TicketWeight);
                    entity.GrossWeight = listone.Sum(a => a.GrossWeight);
                    entity.TareWeight = listone.Sum(a => a.TareWeight);
                    entity.SuttleWeight = listone.Sum(a => a.GrossWeight - a.TareWeight);
                    entity.CheckWeight = listone.Sum(a => a.CheckWeight);
                    entity.DeductWeight = listone.Sum(a => a.DeductWeight);
                    entity.IsFinish = listone.Count;//把车数放到完成状态，用来过渡数据
                    listCount.Add(entity);
                }
            }

            listCount.OrderBy(a => a.MineName);
            CmcsBuyFuelTransport listTotal1 = new CmcsBuyFuelTransport();
            listTotal1.MineName = "合计";
            listTotal1.TicketWeight = list.Sum(a => a.TicketWeight);
            listTotal1.GrossWeight = list.Sum(a => a.GrossWeight);
            listTotal1.TareWeight = list.Sum(a => a.TareWeight);
            listTotal1.SuttleWeight = list.Sum(a => a.GrossWeight - a.TareWeight);
            listTotal1.CheckWeight = list.Sum(a => a.CheckWeight);
            listTotal1.DeductWeight = list.Sum(a => a.DeductWeight);
            listTotal1.IsFinish = list.Count;//车数
            listCount.Add(listTotal1);

            superGridControl1.PrimaryGrid.DataSource = listCount;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SqlWhere = " where TareWeight>0";
            if (dtpStartTime.Value != DateTime.MinValue) this.SqlWhere += " and trunc(InFactoryTime) >= '" + dtpStartTime.Value.ToString("yyyy-MM-dd") + "'";
            if (dtpEndTime.Value != DateTime.MinValue) this.SqlWhere += " and trunc(InFactoryTime) < '" + dtpEndTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "'";
            if (!string.IsNullOrEmpty(txtMineName_BuyFuel.Text)) this.SqlWhere += " and MineName = '" + txtMineName_BuyFuel.Text + "'";

            BindData();
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            this.SqlWhere = string.Empty;
            txtMineName_BuyFuel.Text = string.Empty;

            BindData();
        }

        private void btnInStore_Click(object sender, EventArgs e)
        {
            FrmBuyFuelTransport_Oper frm = new FrmBuyFuelTransport_Oper();
            frm.ShowDialog();

            BindData();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            FrmBuyFuelTransport_Oper frmEdit = new FrmBuyFuelTransport_Oper(String.Empty, true);
            if (frmEdit.ShowDialog() == DialogResult.OK)
            {
                BindData();
            }
        }

        /// <summary>
        /// 打印磅单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiPrint_Click(object sender, EventArgs e)
        {
            this.wagonPrinter.Print(this.listCount, null, dtpStartTime.Value, dtpEndTime.Value);
        }

        private void btnSelectMine_BuyFuel_Click(object sender, EventArgs e)
        {
            FrmMine_Select frm = new FrmMine_Select("where IsUse='1' order by Name asc");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.txtMineName_BuyFuel.Text = frm.Output.Name;
            }
        }

        /// <summary>
        /// 设置行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl1_GetRowHeaderText(object sender, GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.RowIndex + 1).ToString();
        }
    }
}
