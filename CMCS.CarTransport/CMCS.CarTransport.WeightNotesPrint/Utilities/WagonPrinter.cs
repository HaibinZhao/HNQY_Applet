using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Drawing;
using System.Drawing.Printing;
using DevComponents.DotNetBar;
using System.Windows.Forms;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.iEAA;
using CMCS.Common;
using System.IO;

namespace CMCS.CarTransport.WeightNotesPrint.Utilities
{
    class WagonPrinter
    {
        PrintAppConfig instance = PrintAppConfig.GetInstance();
        PrintDocument prtdoc = null;
        int PageIndex = 1;
        CmcsBuyFuelTransport entity = null;

        public WagonPrinter(PrintDocument prt_doc)
        {
            this.prtdoc = prt_doc;
            this.prtdoc.DefaultPageSettings.PaperSize = new PaperSize("Custum", 310, 650);
            this.prtdoc.OriginAtMargins = true;
            this.prtdoc.DefaultPageSettings.Margins.Left = 3;
            this.prtdoc.DefaultPageSettings.Margins.Right = 3;
            this.prtdoc.DefaultPageSettings.Margins.Top = 15;
            this.prtdoc.DefaultPageSettings.Margins.Bottom = 5;
            this.prtdoc.PrintController = new StandardPrintController();
            this.prtdoc.PrintPage += new PrintPageEventHandler(prtdoc_PrintPage);
        }

        public void Print(CmcsBuyFuelTransport _entity, int count)
        {
            if (_entity != null)
            {
                PageIndex = 1;

                entity = _entity;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        this.prtdoc.Print();
                    }
                    catch
                    {
                        MessageBoxEx.Show("打印机出现异常，请检查！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    PageIndex++;
                }

                entity = null;
            }
        }

        void prtdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // 行间距 30 
            float TopValue = 53;
            float leftPadding = instance.LeftPadding;
            Image img;
            String path = Application.StartupPath + "\\logos.png";
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                img = Image.FromStream(fs);
            }

            Font fontTitle = new Font("黑体", 14, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontTitle1 = new Font("黑体", 12, FontStyle.Bold, GraphicsUnit.Pixel);

            Font fontContent = new Font("黑体", instance.FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontPrintTime = new Font("黑体", instance.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontSupplier = new Font("黑体", instance.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);

            string printValue = "";
            //画logo
            g.DrawImage(img, leftPadding + 10, 25);

            g.DrawString("国家电投河南公司沁阳发电分公司", fontTitle1, Brushes.Black, leftPadding + 40, 40);
            TopValue += 15;

            g.DrawString("过  磅  单", new Font("黑体", 18, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, leftPadding + 85, TopValue);
            TopValue += 34;

            g.DrawLine(new Pen(Color.Black, 2), leftPadding + 0, TopValue, 300 - 10, TopValue);
            TopValue += 15;

            g.DrawString("打印时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), fontPrintTime, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString("流 水 号：" + entity.SerialNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString("车 牌 号：" + entity.CarNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString("煤    种：" + entity.FuelKindName, fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString("矿    点：" + entity.MineName, fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString("供货单位：", fontContent, Brushes.Black, leftPadding + 0, TopValue);
            printValue = entity.SupplierName != null ? entity.SupplierName : string.Empty;

            if (!String.IsNullOrEmpty(printValue) && printValue.Length > instance.RowMaxChaNums)
            {
                int rows = printValue.Length % instance.RowMaxChaNums == 0 ? printValue.Length / instance.RowMaxChaNums : ((printValue.Length / instance.RowMaxChaNums) + 1);
                for (int i = 0; i < rows; i++)
                {
                    if (i == rows - 1)
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, printValue.Length - i * instance.RowMaxChaNums), fontSupplier, Brushes.Black, leftPadding + 102, TopValue);
                        TopValue += 20;
                        break;
                    }
                    else
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, instance.RowMaxChaNums), fontSupplier, Brushes.Black, leftPadding + 102, TopValue);
                        TopValue += 20;
                    }
                }
            }
            else
            {
                g.DrawString(printValue, fontSupplier, Brushes.Black, leftPadding + 62, TopValue);
                TopValue += 20;
            }

            g.DrawString("收货单位：", fontContent, Brushes.Black, leftPadding + 0, TopValue);
            printValue = "国家电投河南公司沁阳发电分公司";

            if (!String.IsNullOrEmpty(printValue) && printValue.Length > instance.RowMaxChaNums)
            {
                int rows = printValue.Length % instance.RowMaxChaNums == 0 ? printValue.Length / instance.RowMaxChaNums : ((printValue.Length / instance.RowMaxChaNums) + 1);
                for (int i = 0; i < rows; i++)
                {
                    if (i == rows - 1)
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, printValue.Length - i * instance.RowMaxChaNums), fontSupplier, Brushes.Black, leftPadding + 100, TopValue);
                        TopValue += 20;
                        break;
                    }
                    else
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, instance.RowMaxChaNums), fontSupplier, Brushes.Black, leftPadding + 100, TopValue);
                        TopValue += 20;
                    }
                }
            }
            else
            {
                g.DrawString(printValue, fontSupplier, Brushes.Black, leftPadding + 62, TopValue);
                TopValue += 20;
            }

            g.DrawString("运输单位：", fontContent, Brushes.Black, leftPadding + 0, TopValue);
            printValue = entity.TransportCompanyName != null ? entity.TransportCompanyName : string.Empty;

            if (!String.IsNullOrEmpty(printValue) && printValue.Length > instance.RowMaxChaNums)
            {
                int rows = printValue.Length % instance.RowMaxChaNums == 0 ? printValue.Length / instance.RowMaxChaNums : ((printValue.Length / instance.RowMaxChaNums) + 1);
                for (int i = 0; i < rows; i++)
                {
                    if (i == rows - 1)
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, printValue.Length - i * instance.RowMaxChaNums), fontSupplier, Brushes.Black, leftPadding + 102, TopValue);
                        TopValue += 20;
                        break;
                    }
                    else
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, instance.RowMaxChaNums), fontSupplier, Brushes.Black, leftPadding + 102, TopValue);
                        TopValue += 20;
                    }
                }
            }
            else
            {
                g.DrawString(printValue, fontSupplier, Brushes.Black, leftPadding + 62, TopValue);
                TopValue += 20;
            }

            g.DrawString("称重时间：" + DisposeTime(entity.GrossTime.ToString(), "yyyy-MM-dd HH:mm"), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString("回皮时间：" + DisposeTime(entity.TareTime.ToString(), "yyyy-MM-dd HH:mm"), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(string.Format("矿发重量：{0} 吨", Math.Round(entity.TicketWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(string.Format("毛    重：{0} 吨", Math.Round(entity.GrossWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(string.Format("皮    重：{0} 吨", Math.Round(entity.TareWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(string.Format("扣    吨：{0} 吨", Math.Round(entity.DeductWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(string.Format("盈 亏 量：{0} 吨", Math.Round((entity.SuttleWeight - entity.TicketWeight), 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(string.Format("净    重：{0} 吨", Math.Round(entity.SuttleWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(string.Format("验 收 量：{0} 吨", Math.Round(entity.CheckWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            //User user = Dbers.GetInstance().SelfDber.Entity<User>(String.Format(" where  UserAccount='{0}'", entity.CreateUser));
            g.DrawString(string.Format("操 作 员：{0}", SelfVars.LoginUser.UserName), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(PageIndex.ToString() + "联", fontTitle, Brushes.Black, leftPadding + 110, TopValue);
            TopValue += 20;

            g.DrawString("", fontTitle, Brushes.Black, leftPadding + 110, TopValue);
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
    }
}
