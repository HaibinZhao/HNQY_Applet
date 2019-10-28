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
using CMCS.Common;
using CMCS.CarTransport.Queue.Core;

namespace CMCS.CarTransport.Queue.Frms.Transport.Print
{
    /// <summary>
    /// 磅单打印
    /// </summary>
    class WagonPrinter : MetroForm
    {
        Font TitleFont = new Font("宋体", 24, FontStyle.Bold, GraphicsUnit.Pixel);
        Font ContentFont = new Font("宋体", 14, FontStyle.Regular, GraphicsUnit.Pixel);
        PrintDocument _PrintDocument = null;
        CmcsBuyFuelTransport _BuyFuelTransport = null;
        CmcsGoodsTransport _GoodsTransport = null;
        int PageIndex = 1;
        Graphics gs = null;

        public WagonPrinter(PrintDocument printDoc)
        {
            this._PrintDocument = printDoc;
            this._PrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Custum", 850, 368);
            this._PrintDocument.OriginAtMargins = true;
            this._PrintDocument.DefaultPageSettings.Margins.Left = 10;
            this._PrintDocument.DefaultPageSettings.Margins.Right = 0;
            this._PrintDocument.DefaultPageSettings.Margins.Top = 0;
            this._PrintDocument.DefaultPageSettings.Margins.Bottom = 0;
            this._PrintDocument.PrintController = new StandardPrintController();
            this._PrintDocument.PrintPage += _PrintDocument_PrintPage;
        }
        public void Print(CmcsBuyFuelTransport buyfueltransport, CmcsGoodsTransport goodstransport)
        {
            if (buyfueltransport == null && goodstransport == null) return;
            _BuyFuelTransport = buyfueltransport;
            _GoodsTransport = goodstransport;
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

            Graphics g = e.Graphics;
            if (this.gs != null)
                g = this.gs;
          
            string SerialNumber = string.Empty,
                    CarNumber = string.Empty,
                    MineName = string.Empty,
                    SupplierName = string.Empty,
                    GrossTime = string.Empty,
                    TareTime = string.Empty,
                    TicketWeight = string.Empty,
                    GrossWeight = string.Empty,
                    TareWeight = string.Empty,
                    ProfitAndLossWeight = string.Empty,
                    CheckWeight = string.Empty,
                    UserName = string.Empty;
            if (this._BuyFuelTransport != null)
            {
                SerialNumber = this._BuyFuelTransport.SerialNumber;
                CarNumber = this._BuyFuelTransport.CarNumber;
                MineName = this._BuyFuelTransport.MineName;
                SupplierName = this._BuyFuelTransport.SupplierName;
                GrossTime = DisposeTime(this._BuyFuelTransport.GrossTime.ToString(), "yyyy-MM-dd HH:mm");
                TareTime = DisposeTime(this._BuyFuelTransport.TareTime.ToString(), "yyyy-MM-dd HH:mm");
                TicketWeight = this._BuyFuelTransport.TicketWeight.ToString("F2").PadLeft(6, ' ');
                GrossWeight = this._BuyFuelTransport.GrossWeight.ToString("F2").PadLeft(6, ' ');
                TareWeight = this._BuyFuelTransport.TareWeight.ToString("F2").PadLeft(6, ' ');
                CheckWeight = this._BuyFuelTransport.CheckWeight.ToString("F2").PadLeft(6, ' ');
                TicketWeight = this._BuyFuelTransport.TicketWeight.ToString("F2").PadLeft(6, ' ');
                #region 入厂煤
                // 行间距 24 
                float TopValue = 53;
                string printValue = "";
                g.DrawString("沁阳电厂汽车过磅单", new Font("黑体", 20, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, 30, TopValue);
                TopValue += 34;

                g.DrawString("打印时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawLine(new Pen(Color.Black, 2), 0, TopValue, 300 - 10, TopValue);
                TopValue += 15;

                g.DrawString("流 水 号：" + SerialNumber, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString("车 牌 号：" + CarNumber, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString("矿    点：" + MineName, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString("供货单位：", ContentFont, Brushes.Black, 30, TopValue);
                printValue = SupplierName != null ? SupplierName : string.Empty;

                if (printValue.Length > 12)
                {
                    g.DrawString(printValue.Substring(0, 12), ContentFont, Brushes.Black, 105, TopValue);
                    TopValue += 24;
                    g.DrawString(printValue.Substring(12, printValue.Length - 12), ContentFont, Brushes.Black, 105, TopValue);
                    TopValue += 24;
                }
                else
                {
                    g.DrawString(printValue, ContentFont, Brushes.Black, 102, TopValue);
                    TopValue += 24;
                }

                g.DrawString("毛重时间：" + GrossTime, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString("皮重时间：" + TareTime, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("矿 发 量：{0} 吨", TicketWeight), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("毛    重：{0} 吨", GrossWeight), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("皮    重：{0} 吨", TareWeight), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("盈 亏 量：{0} 吨", ProfitAndLossWeight), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("验 收 量：{0} 吨", CheckWeight), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("操 作 员：{0}", SelfVars.LoginUser.UserName), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 34;
                g.DrawString(PageIndex.ToString() + "联", ContentFont, Brushes.Black, 110, TopValue);
                TopValue += 24;
                #endregion
            }
            else
            {
                SerialNumber = this._GoodsTransport.SerialNumber;
                CarNumber = this._GoodsTransport.CarNumber;
                MineName = this._GoodsTransport.ReceiveUnitName;
                SupplierName = this._GoodsTransport.SupplyUnitName;
                GrossTime = DisposeTime(this._GoodsTransport.FirstTime.ToString(), "yyyy-MM-dd HH:mm");
                TareTime = DisposeTime(this._GoodsTransport.SecondTime.ToString(), "yyyy-MM-dd HH:mm");
                GrossWeight = this._GoodsTransport.FirstTime.ToString("F2").PadLeft(6, ' ');
                TareWeight = this._GoodsTransport.SecondWeight.ToString("F2").PadLeft(6, ' ');
                #region 其它物资
                // 行间距 24 
                float TopValue = 53;
                string printValue = "";
                g.DrawString("豫能鹤淇电厂汽车过磅单", new Font("黑体", 20, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, 30, TopValue);
                TopValue += 34;

                g.DrawString("打印时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawLine(new Pen(Color.Black, 2), 0, TopValue, 300 - 10, TopValue);
                TopValue += 15;

                g.DrawString("流 水 号：" + SerialNumber, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString("车 牌 号：" + CarNumber, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString("矿    点：" + MineName, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString("供货单位：", ContentFont, Brushes.Black, 30, TopValue);
                printValue = SupplierName != null ? SupplierName : string.Empty;

                if (printValue.Length > 12)
                {
                    g.DrawString(printValue.Substring(0, 12), ContentFont, Brushes.Black, 105, TopValue);
                    TopValue += 24;
                    g.DrawString(printValue.Substring(12, printValue.Length - 12), ContentFont, Brushes.Black, 105, TopValue);
                    TopValue += 24;
                }
                else
                {
                    g.DrawString(printValue, ContentFont, Brushes.Black, 102, TopValue);
                    TopValue += 24;
                }

                g.DrawString("毛重时间：" + GrossTime, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString("皮重时间：" + TareTime, ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("毛    重：{0} 吨", GrossWeight), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("皮    重：{0} 吨", TareWeight), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 24;

                g.DrawString(string.Format("操 作 员：{0}", SelfVars.LoginUser.UserName), ContentFont, Brushes.Black, 30, TopValue);
                TopValue += 34;
                g.DrawString(PageIndex.ToString() + "联", ContentFont, Brushes.Black, 110, TopValue);
                TopValue += 24;
                #endregion
            }
        }

        public static string DisposeTime(string dt, string format)
        {
            if (!string.IsNullOrEmpty(dt))
            {
                DateTime dti = DateTime.Parse(dt);
                if (dti > new DateTime(2000, 1, 1))
                    return dti.ToString(format);
            }
            return string.Empty;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WagonPrinter
            // 
            this.ClientSize = new System.Drawing.Size(362, 227);
            this.DoubleBuffered = true;
            this.Name = "WagonPrinter";
            this.ResumeLayout(false);

        }
    }
}
