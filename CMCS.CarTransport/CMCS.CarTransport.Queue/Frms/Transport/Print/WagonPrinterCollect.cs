using System;
using System.Collections.Generic;
//
using System.Drawing;
using System.Drawing.Printing;
using DevComponents.DotNetBar;
using System.Windows.Forms;
using DevComponents.DotNetBar.Metro;
using CMCS.Common.Entities.CarTransport;

namespace CMCS.CarTransport.Queue.Frms.Transport.Print
{
    /// <summary>
    /// 报表打印
    /// </summary>
    class WagonPrinterCollect : MetroAppForm
    {
        Font TitleFont = new Font("宋体", 24, FontStyle.Bold, GraphicsUnit.Pixel);
        Font ContentFont = new Font("宋体", 15, FontStyle.Regular, GraphicsUnit.Pixel);
        PrintDocument _PrintDocument = null;
        List<CmcsBuyFuelTransport> _Transport = null;
        DateTime StartTime = new DateTime();
        DateTime EndTime = new DateTime();
        private int count;
        int LineHeight = 25;//行高
        int paddingleft = 10;//矩形距离左边的距离
        int paddingtopAll = 60;//整体距离上部的距离


        public WagonPrinterCollect(PrintDocument printDoc)
        {
            this._PrintDocument = printDoc;
            this._PrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Custum", 1200, LineHeight * (count + 1) + paddingtopAll + 200);
            this._PrintDocument.OriginAtMargins = true;
            this._PrintDocument.DefaultPageSettings.Margins.Left = 10;
            this._PrintDocument.DefaultPageSettings.Margins.Right = 0;
            this._PrintDocument.DefaultPageSettings.Margins.Top = 0;
            this._PrintDocument.DefaultPageSettings.Margins.Bottom = 0;
            this._PrintDocument.PrintController = new StandardPrintController();
            this._PrintDocument.PrintPage += _PrintDocument_PrintPage;
        }

        public void Print(List<CmcsBuyFuelTransport> transport, Graphics gs, DateTime start, DateTime end)
        {
            if (transport == null) return;
            _Transport = transport;
            StartTime = start;
            EndTime = end;
            count = transport.Count;
            this._PrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Custum", 1200, LineHeight * (count + 1) + paddingtopAll + 200);
            this.ClientSize = new System.Drawing.Size(1200, LineHeight * (count + 1) + paddingtopAll + 200);
            try
            {
                this._PrintDocument.Print();
            }
            catch
            {
                MessageBoxEx.Show("打印异常，请检查打印机！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _Transport = null;
        }

        private void _PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {

            Graphics gs = e.Graphics;

            #region 绘制磅单

            int paddingtop = paddingtopAll + 80;//矩形距离上部的距离
            float cloum1 = 170;//供应商的宽
            float cloum2 = 40;//车数的宽
            float cloum3 = 71;//重量的宽

            int RowWeight = Convert.ToInt32(cloum1 + cloum2 + cloum3 * 8);//列表宽
            int Height = LineHeight * (_Transport.Count + 1);//矩形的高
            int top = 0;
            Rectangle r = new Rectangle(paddingleft, paddingtop, RowWeight, Height);//矩形的位置和大小
            Pen pen = new Pen(Color.Black, 1);//画笔
            gs.DrawRectangle(pen, r);

            string title = "企业过磅日报表";
            Font titlefont = new Font("宋体", 20, FontStyle.Bold, GraphicsUnit.Pixel);

            gs.DrawString(title, titlefont, Brushes.Black, (RowWeight - gs.MeasureString(title, titlefont).Width) / 2, paddingtopAll);
            gs.DrawString("统计时间：" + StartTime.ToString("yyyy-MM-dd") + "-" + EndTime.ToString("yyyy-MM-dd") + "                                   指标日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "    单位：吨", ContentFont, Brushes.Black, paddingleft + 10, paddingtopAll + 55);
            int seralinumber = 1;//序号
            //列标题

            //供应商
            string SupplierNameTitle = "发货单位";
            SizeF SupplierNameSizeTitle = gs.MeasureString(SupplierNameTitle, ContentFont);
            gs.DrawString(SupplierNameTitle, ContentFont, Brushes.Black, paddingleft + (cloum1 - SupplierNameSizeTitle.Width) / 2, paddingtop + (LineHeight - SupplierNameSizeTitle.Height) / 2);
            //车数
            string MineNameTitle = "车数";
            SizeF MineNameSizeTitle = gs.MeasureString(MineNameTitle, ContentFont);
            gs.DrawString(MineNameTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + (cloum2 - MineNameSizeTitle.Width) / 2, paddingtop + (LineHeight - MineNameSizeTitle.Height) / 2);
            //毛重
            string GrossWeightTitle = "毛重(T)";
            SizeF GrossWeightSizeTitle = gs.MeasureString(GrossWeightTitle, ContentFont);
            gs.DrawString(GrossWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + (cloum3 - GrossWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - GrossWeightSizeTitle.Height) / 2);
            //皮重
            string TareWeightTitle = "皮重(T)";
            SizeF TareWeightSizeTitle = gs.MeasureString(TareWeightTitle, ContentFont);
            gs.DrawString(TareWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 + (cloum3 - TareWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - TareWeightSizeTitle.Height) / 2);
            //扣水
            string KsWeightTitle = "扣水(T)";
            SizeF KsWeightSizeTitle = gs.MeasureString(KsWeightTitle, ContentFont);
            gs.DrawString(KsWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 2 + (cloum3 - KsWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - KsWeightSizeTitle.Height) / 2);
            //扣矸
            string KgWeightTitle = "扣矸(T)";
            SizeF KgWeightSizeTitle = gs.MeasureString(KgWeightTitle, ContentFont);
            gs.DrawString(KgWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 3 + (cloum3 - KgWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - KgWeightSizeTitle.Height) / 2);
            //净重
            string SuttleWeightTitle = "净重(T)";
            SizeF SuttleWeightSizeTitle = gs.MeasureString(SuttleWeightTitle, ContentFont);
            gs.DrawString(SuttleWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 4 + (cloum3 - SuttleWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - SuttleWeightSizeTitle.Height) / 2);

            //验收量
            string CheckWeightTitle = "验收量(T)";
            SizeF CheckWeightSizeTitle = gs.MeasureString(CheckWeightTitle, ContentFont);
            gs.DrawString(CheckWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 5 + (cloum3 - CheckWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - CheckWeightSizeTitle.Height) / 2);

            //矿发量
            string TicketWeightTitle = "矿发量(T)";
            SizeF TicketWeightSizeTitle = gs.MeasureString(TicketWeightTitle, ContentFont);
            gs.DrawString(TicketWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 6 + (cloum3 - TicketWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - TicketWeightSizeTitle.Height) / 2);
            //盈亏
            string ProfitAndLossWeightTitle = "盈亏(T)";
            SizeF ProfitAndLossWeightSizeTitle = gs.MeasureString(ProfitAndLossWeightTitle, ContentFont);
            gs.DrawString(ProfitAndLossWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 7 + (cloum3 - ProfitAndLossWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - ProfitAndLossWeightSizeTitle.Height) / 2);

            gs.DrawLine(pen, paddingleft, paddingtop + LineHeight, paddingleft + RowWeight, paddingtop + LineHeight);

            foreach (CmcsBuyFuelTransport item in _Transport)
            {
                top = LineHeight * seralinumber;

                //供应商
                string SupplierName = item.MineName;
                SizeF SupplierNameSize = gs.MeasureString(SupplierName, ContentFont);
                if (SupplierName == "合计")
                    gs.DrawString(SupplierName, ContentFont, Brushes.Black, paddingleft + (cloum1 - SupplierNameSize.Width) / 2, paddingtop + top + (LineHeight - SupplierNameSize.Height) / 2);
                else
                    gs.DrawString(SupplierName, ContentFont, Brushes.Black, paddingleft + (cloum1 - SupplierNameSize.Width) / 2, paddingtop + top + (LineHeight - SupplierNameSize.Height) / 2);
                //车数
                string MineName = item.IsFinish.ToString();
                SizeF MineNameSize = gs.MeasureString(MineName, ContentFont);
                gs.DrawString(MineName, ContentFont, Brushes.Black, paddingleft + cloum1 + (cloum2 - MineNameSize.Width) / 2, paddingtop + top + (LineHeight - MineNameSize.Height) / 2);
                //毛重
                string GrossWeight = item.GrossWeight.ToString("F2");
                SizeF GrossWeightSize = gs.MeasureString(GrossWeight, ContentFont);
                gs.DrawString(GrossWeight, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + (cloum3 - GrossWeightSize.Width) / 2, paddingtop + top + (LineHeight - GrossWeightSize.Height) / 2);
                //皮重
                string TareWeight = item.TareWeight.ToString("F2");
                SizeF TareWeightSize = gs.MeasureString(TareWeight, ContentFont);
                gs.DrawString(TareWeight, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 + (cloum3 - TareWeightSize.Width) / 2, paddingtop + top + (LineHeight - TareWeightSize.Height) / 2);
                //净重
                string SuttleWeight = item.SuttleWeight.ToString("F2");
                SizeF SuttleWeightSize = gs.MeasureString(SuttleWeight, ContentFont);
                gs.DrawString(SuttleWeight, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 4 + (cloum3 - SuttleWeightSize.Width) / 2, paddingtop + top + (LineHeight - SuttleWeightSize.Height) / 2);
                //验收量
                string CheckWeight = item.CheckWeight.ToString("F2");
                SizeF CheckWeightSize = gs.MeasureString(CheckWeight, ContentFont);
                gs.DrawString(CheckWeight, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 5 + (cloum3 - CheckWeightSize.Width) / 2, paddingtop + top + (LineHeight - CheckWeightSize.Height) / 2);
                //矿发量
                string TicketWeight = item.TicketWeight.ToString("F2");
                SizeF TicketWeightSize = gs.MeasureString(TicketWeight, ContentFont);
                gs.DrawString(TicketWeight, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 * 6 + (cloum3 - TicketWeightSize.Width) / 2, paddingtop + top + (LineHeight - TicketWeightSize.Height) / 2);

                gs.DrawLine(pen, paddingleft, paddingtop + LineHeight * seralinumber, paddingleft + RowWeight, paddingtop + LineHeight * seralinumber);
                seralinumber++;
            }

            gs.DrawLine(pen, paddingleft + cloum1, paddingtop, paddingleft + cloum1, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2, paddingtop, paddingleft + cloum1 + cloum2, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3, paddingtop, paddingleft + cloum1 + cloum2 + cloum3, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 * 2, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 * 2, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 * 3, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 * 3, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 * 4, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 * 4, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 * 5, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 * 5, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 * 6, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 * 6, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 * 7, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 * 7, paddingtop + Height);
            gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 * 8, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 * 8, paddingtop + Height);
            //gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 * 9, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 * 9, paddingtop + Height);
            #endregion
        }

        public static string DisposeTime(string dt, string format)
        {
            if (!string.IsNullOrEmpty(dt))
            {
                DateTime dti = DateTime.Parse(dt);
                if (dti != DateTime.MinValue)
                    return dti.ToString(format);
            }
            return string.Empty;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new System.Drawing.Size(1200, 2000);
            this.Name = "WagonPrinterCollect";
            this.ResumeLayout(false);

        }
    }
}
