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
using System.IO;
using NPOI.HSSF.UserModel;

namespace CMCS.CarTransport.Queue.Frms.Transport.GoodsTransport
{
    public partial class FrmGoodsTransport_Detail : MetroAppForm
    {
        /// <summary>
        /// 窗体唯一标识符
        /// </summary>
        public static string UniqueKey = "FrmGoodsTransport_Detail";

        WagonPrinterDetail wagonPrinter = null;
        List<CmcsGoodsTransport> listCount = new List<CmcsGoodsTransport>();

        string SqlWhere = string.Empty;

        public FrmGoodsTransport_Detail()
        {
            InitializeComponent();
        }

        private void FrmBuyFuelTransport_List_Load(object sender, EventArgs e)
        {
            superGridControl1.PrimaryGrid.AutoGenerateColumns = false;

            dtpStartTime.Value = DateTime.Now;
            dtpEndTime.Value = DateTime.Now;

            this.wagonPrinter = new WagonPrinterDetail(printDocument1);

            btnSearch_Click(null, null);
        }

        public void BindData()
        {
            listCount.Clear();
            string tempSqlWhere = this.SqlWhere;
            listCount = Dbers.GetInstance().SelfDber.Entities<CmcsGoodsTransport>(tempSqlWhere + " order by SerialNumber desc");

            labNumber_BuyFuel.Text = string.Format("已登记：{0}  已称重：{1}  已回皮：{2}  未回皮：{3}", listCount.Count, listCount.Where(a => a.FirstWeight > 0).Count(), listCount.Where(a => a.SecondWeight > 0).Count(), listCount.Where(a => a.SuttleWeight == 0).Count());
            listCount.OrderBy(a => a.SupplyUnitName);
            CmcsGoodsTransport listTotal1 = new CmcsGoodsTransport();
            listTotal1.CarNumber = "合计";
            listTotal1.FirstWeight = listCount.Sum(a => a.FirstWeight);
            listTotal1.SecondWeight = listCount.Sum(a => a.SecondWeight);
            listTotal1.SuttleWeight = listCount.Sum(a => a.SuttleWeight);
            listTotal1.SupplyUnitName = listCount.Count.ToString() + "车";//车数
            listCount.Add(listTotal1);

            superGridControl1.PrimaryGrid.DataSource = listCount;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SqlWhere = " where 1=1";
            if (dtpStartTime.Value != DateTime.MinValue) this.SqlWhere += " and trunc(InFactoryTime) >= '" + dtpStartTime.Value.ToString("yyyy-MM-dd") + "'";
            if (dtpEndTime.Value != DateTime.MinValue) this.SqlWhere += " and trunc(InFactoryTime) < '" + dtpEndTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "'";
            if (!string.IsNullOrEmpty(txtCarNumber.Text)) this.SqlWhere += " and CarNumber like '%" + txtCarNumber.Text + "%'";
            if (!string.IsNullOrEmpty(txtGoodsType.Text)) this.SqlWhere += " and GoodsTypeName = '" + txtGoodsType.Text + "'";
            BindData();
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            this.SqlWhere = string.Empty;
            txtCarNumber.Text = string.Empty;

            BindData();
        }

        /// <summary>
        /// 选择物资类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectGoodsType_Goods_Click(object sender, EventArgs e)
        {
            FrmGoodsType_Select frm = new FrmGoodsType_Select();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.txtGoodsType.Text = frm.Output.GoodsName;
            }
        }

        #region DataGridView
        /// <summary>
        /// 设置行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superGridControl1_GetRowHeaderText(object sender, GridGetRowHeaderTextEventArgs e)
        {
            e.Text = (e.GridRow.RowIndex + 1).ToString();
        }
        #endregion

        #region 导出Excel

        private void btnXExport_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream file = new FileStream("车辆出入厂.xls", FileMode.Open, FileAccess.Read);
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                HSSFSheet sheetl = (HSSFSheet)hssfworkbook.GetSheet("sheet1");

                if (this.listCount.Count == 0)
                {
                    MessageBox.Show("请先查询数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                    return;
                for (int i = 0; i < listCount.Count; i++)
                {
                    CmcsGoodsTransport entity = listCount[i];
                    CmcsAutotruck autoTruck = Dbers.GetInstance().SelfDber.Get<CmcsAutotruck>(entity.AutotruckId);
                    if (entity.CarNumber == "合计")
                        continue;
                    Mysheet1(sheetl, i + 2, 0, entity.CarNumber);
                    Mysheet1(sheetl, i + 2, 1, entity.InFactoryTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    Mysheet1(sheetl, i + 2, 2, entity.SecondTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    Mysheet1(sheetl, i + 2, 3, autoTruck != null ? autoTruck.EmissionStandard : "");
                    sheetl.GetRow(i + 2).Height = sheetl.GetRow(1).Height;

                    sheetl.GetRow(i + 3).GetCell(0).CellStyle = sheetl.GetRow(2).GetCell(0).CellStyle;
                    sheetl.GetRow(i + 3).GetCell(1).CellStyle = sheetl.GetRow(2).GetCell(1).CellStyle;
                    sheetl.GetRow(i + 3).GetCell(2).CellStyle = sheetl.GetRow(2).GetCell(2).CellStyle;
                    sheetl.GetRow(i + 3).GetCell(3).CellStyle = sheetl.GetRow(2).GetCell(3).CellStyle;
                    sheetl.GetRow(i + 3).GetCell(4).CellStyle = sheetl.GetRow(2).GetCell(4).CellStyle;
                    sheetl.GetRow(i + 3).GetCell(5).CellStyle = sheetl.GetRow(2).GetCell(5).CellStyle;
                    sheetl.GetRow(i + 3).GetCell(6).CellStyle = sheetl.GetRow(2).GetCell(6).CellStyle;
                    sheetl.GetRow(i + 3).GetCell(7).CellStyle = sheetl.GetRow(2).GetCell(7).CellStyle;
                    sheetl.GetRow(i + 3).GetCell(8).CellStyle = sheetl.GetRow(2).GetCell(8).CellStyle;
                }

                sheetl.ForceFormulaRecalculation = true;
                string fileName = "其他物资车辆出入厂记录_" + dtpStartTime.Value.ToString("yyyy-MM-dd") + ".xls";
                GC.Collect();

                FileStream fs = File.OpenWrite(folderBrowserDialog1.SelectedPath + "\\" + fileName);
                hssfworkbook.Write(fs);   //向打开的这个xls文件中写入表并保存。  
                fs.Close();
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Mysheet1(HSSFSheet sheet1, int x, int y, String Value)
        {
            if (sheet1.GetRow(x) == null)
            {
                sheet1.CreateRow(x);
            }
            if (sheet1.GetRow(x).GetCell(y) == null)
            {
                sheet1.GetRow(x).CreateCell(y);
            }
            sheet1.GetRow(x).GetCell(y).SetCellValue(Value);

        }

        #endregion

    }
}
