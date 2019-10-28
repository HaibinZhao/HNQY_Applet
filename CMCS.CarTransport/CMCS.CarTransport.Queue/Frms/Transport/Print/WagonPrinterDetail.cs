using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// 磅单打印
    /// </summary>
    class WagonPrinterDetail : MetroAppForm
    {
        Font TitleFont = new Font("宋体", 24, FontStyle.Regular, GraphicsUnit.Pixel);
        Font ContentFont = new Font("宋体", 14, FontStyle.Regular, GraphicsUnit.Pixel);

        PrintDocument _PrintDocument = null;
        List<CmcsBuyFuelTransport> _BuyFuelTransport = null;
        List<CmcsGoodsTransport> _GoodsTransport = null;
        Graphics gs = null;
        DateTime StartTime = new DateTime();
        DateTime EndTime = new DateTime();


        private int count;

        private int LineHeight = 25;//行高
        public WagonPrinterDetail(PrintDocument printDoc)
        {
            this._PrintDocument = printDoc;
            this._PrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Custum", 1000, LineHeight * (count + 1) + 120);
            this._PrintDocument.OriginAtMargins = true;
            this._PrintDocument.DefaultPageSettings.Margins.Left = 10;
            this._PrintDocument.DefaultPageSettings.Margins.Right = 0;
            this._PrintDocument.DefaultPageSettings.Margins.Top = 0;
            this._PrintDocument.DefaultPageSettings.Margins.Bottom = 0;
            this._PrintDocument.PrintController = new StandardPrintController();
            this._PrintDocument.PrintPage += _PrintDocument_PrintPage;
        }

        public void Print(List<CmcsBuyFuelTransport> buyfueltransport, List<CmcsGoodsTransport> goodstransport, DateTime start, DateTime end)
        {
            if (buyfueltransport == null && goodstransport == null) return;
            _BuyFuelTransport = buyfueltransport;
            _GoodsTransport = goodstransport;
            StartTime = start;
            EndTime = end;
            this._PrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Custum", 1000, LineHeight * (count + 1) + 120);
            try
            {
                this._PrintDocument.Print();
            }
            catch
            {
                MessageBoxEx.Show("打印异常，请检查打印机！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _BuyFuelTransport = null;
            _GoodsTransport = null;
        }

        private void _PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {

            Graphics gs = e.Graphics;
            if (this.gs != null)
                gs = this.gs;
            else
            {
                #region 绘制磅单
                int paddingleft = 90;//矩形距离左边的距离
                int paddingtop = 80;//矩形距离上部的距离

                int RowWeight = 700;//列表宽
                int Height = LineHeight * (_BuyFuelTransport.Count + 1);//矩形的高
                int top = 0;
                Rectangle r = new Rectangle(paddingleft, paddingtop, RowWeight, Height);//矩形的位置和大小
                Pen pen = new Pen(Color.Black, 1);//画笔
                gs.DrawRectangle(pen, r);

                float cloum1 = 40;//序号的宽
                float cloum2 = 230;//供应商的宽
                float cloum3 = 55;//煤种的宽
                float cloum4 = 80;//车号的宽
                float cloum5 = 70;//重量的宽
                gs.DrawString("新乡中益发电有限公司入厂煤计量明细表（汽运）", new Font("宋体", 20, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, paddingleft + 130, 30);
                gs.DrawString("起始时间：" + StartTime.ToString("yyyy-MM-dd 03:00:00") + "   终止时间：" + EndTime.AddDays(1).ToString("yyyy-MM-dd 03:00:00") + "    单位：t", ContentFont, Brushes.Black, paddingleft + 10, 65);
                int seralinumber = 1;//序号
                //列标题
                //序号
                string seralinumberTitle = "序号";
                SizeF seralinumberSizeTitle = gs.MeasureString(seralinumberTitle, ContentFont);
                gs.DrawString(seralinumberTitle.ToString(), ContentFont, Brushes.Black, paddingleft + (cloum1 - seralinumberSizeTitle.Width) / 2, paddingtop + (LineHeight - seralinumberSizeTitle.Height) / 2);
                //供应商
                string SupplierNameTitle = "供货单位";
                SizeF SupplierNameSizeTitle = gs.MeasureString(SupplierNameTitle, ContentFont);
                gs.DrawString(SupplierNameTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + (cloum2 - SupplierNameSizeTitle.Width) / 2, paddingtop + (LineHeight - SupplierNameSizeTitle.Height) / 2);
                //煤种
                string KindNameTitle = "煤种";
                SizeF KindNameSizeTitle = gs.MeasureString(KindNameTitle, ContentFont);
                gs.DrawString(KindNameTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + (cloum3 - KindNameSizeTitle.Width) / 2, paddingtop + (LineHeight - KindNameSizeTitle.Height) / 2);

                //车牌号
                string CarNumberTitle = "车牌号";
                SizeF CarNumberSizeTitle = gs.MeasureString(CarNumberTitle, ContentFont);
                gs.DrawString(CarNumberTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 + (cloum4 - CarNumberSizeTitle.Width) / 2, paddingtop + (LineHeight - CarNumberSizeTitle.Height) / 2);

                //矿发量
                string TicketWeightTitle = "矿发量";
                SizeF TicketWeightSizeTitle = gs.MeasureString(TicketWeightTitle, ContentFont);
                gs.DrawString(TicketWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 + cloum4 + (cloum5 - TicketWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - TicketWeightSizeTitle.Height) / 2);

                //验收量
                string CheckWeightTitle = "验收量";
                SizeF CheckWeightSizeTitle = gs.MeasureString(CheckWeightTitle, ContentFont);
                gs.DrawString(CheckWeightTitle, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 + cloum4 + cloum5 * 2 + (cloum5 - CheckWeightSizeTitle.Width) / 2, paddingtop + (LineHeight - CheckWeightSizeTitle.Height) / 2);

                foreach (CmcsBuyFuelTransport item in _BuyFuelTransport)
                {
                    top = LineHeight * seralinumber;
                    //序号
                    SizeF seralinumberSize = gs.MeasureString(seralinumber.ToString(), ContentFont);
                    gs.DrawString(seralinumber.ToString(), ContentFont, Brushes.Black, paddingleft + (cloum1 - seralinumberSize.Width) / 2, paddingtop + top + (LineHeight - seralinumberSize.Height) / 2);

                    //供应商
                    string SupplierName = item.SupplierName;
                    SizeF SupplierNameSize = gs.MeasureString(SupplierName, ContentFont);
                    if (item.Id == "合计")
                        gs.DrawString(SupplierName, ContentFont, Brushes.Black, paddingleft + (cloum1 + cloum2 + cloum3 + cloum4 - SupplierNameSize.Width) / 2, paddingtop + top + (LineHeight - SupplierNameSize.Height) / 2);
                    else
                        gs.DrawString(SupplierName, ContentFont, Brushes.Black, paddingleft + cloum1 + (cloum2 - SupplierNameSize.Width) / 2, paddingtop + top + (LineHeight - SupplierNameSize.Height) / 2);
                    //煤种
                    string KindName = item.FuelKindName;
                    SizeF KindNameSize = gs.MeasureString(KindName, ContentFont);
                    gs.DrawString(KindName, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + (cloum3 - KindNameSize.Width) / 2, paddingtop + top + (LineHeight - KindNameSize.Height) / 2);

                    //车牌号
                    string CarNumber = item.CarNumber;
                    SizeF CarNumberSize = gs.MeasureString(CarNumber, ContentFont);
                    gs.DrawString(CarNumber, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 + (cloum4 - CarNumberSize.Width) / 2, paddingtop + top + (LineHeight - CarNumberSize.Height) / 2);

                    //矿发量
                    string TicketWeight = item.TicketWeight.ToString("F2");
                    SizeF TicketWeightSize = gs.MeasureString(TicketWeight, ContentFont);
                    gs.DrawString(TicketWeight, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 + cloum4 + (cloum5 - TicketWeightSize.Width) / 2, paddingtop + top + (LineHeight - TicketWeightSize.Height) / 2);

                    //验收量
                    string CheckWeight = item.CheckWeight.ToString("F2");
                    SizeF CheckWeightSize = gs.MeasureString(CheckWeight, ContentFont);
                    gs.DrawString(CheckWeight, ContentFont, Brushes.Black, paddingleft + cloum1 + cloum2 + cloum3 + cloum4 + cloum5 * 2 + (cloum5 - CheckWeightSize.Width) / 2, paddingtop + top + (LineHeight - CheckWeightSize.Height) / 2);

                    gs.DrawLine(pen, paddingleft, paddingtop + LineHeight * seralinumber, paddingleft + RowWeight, paddingtop + LineHeight * seralinumber);
                    seralinumber++;
                }
                gs.DrawString("计量站：", ContentFont, Brushes.Black, paddingleft, paddingtop + Height + 10);
                gs.DrawString("审核人：", ContentFont, Brushes.Black, paddingleft + 200, paddingtop + Height + 10);
                gs.DrawString("打印日期：" + DateTime.Now.ToString("yyyy年MM月dd日"), ContentFont, Brushes.Black, paddingleft + 400, paddingtop + Height + 10);

                gs.DrawLine(pen, paddingleft + cloum1, paddingtop, paddingleft + cloum1, paddingtop + Height);
                gs.DrawLine(pen, paddingleft + cloum1 + cloum2, paddingtop, paddingleft + cloum1 + cloum2, paddingtop + Height - LineHeight);
                gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3, paddingtop, paddingleft + cloum1 + cloum2 + cloum3, paddingtop + Height - LineHeight);
                gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 + cloum4, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 + cloum4, paddingtop + Height);
                gs.DrawLine(pen, paddingleft + cloum1 + cloum2 + cloum3 + cloum4 + cloum5, paddingtop, paddingleft + cloum1 + cloum2 + cloum3 + cloum4 + cloum5, paddingtop + Height);

                #endregion
            }
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
            
            this.ClientSize = new System.Drawing.Size(362, 227);
            this.Name = "WagonPrinterList";
            this.ResumeLayout(false);

        }
    }
}
