using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using DevComponents.DotNetBar;
using CMCS.Common;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities;
using CMCS.Common.Entities.BaseInfo;
using CMCS.CarTransport.Queue.Utilities;
using System.Drawing.Printing;
using System.IO;
using CMCS.Common.Entities.iEAA;
using CMCS.CarTransport.Queue.Core;

namespace CMCS.CarTransport.Queue.Frms.Transport.BuyFuelTransport
{
    public partial class FrmPrint_Oper : DevComponents.DotNetBar.Metro.MetroForm
    {
        //记录ID
        String id = String.Empty;
        bool edit = false;
        /// <summary>
        /// 打印类型（入厂煤或者其他物资）
        /// </summary>
        String printType = String.Empty;
        //入厂煤运输记录
        CmcsBuyFuelTransport buyFuelTransport;
        //其他物资运输记录
        CmcsGoodsTransport goodsTransport;

        /// <summary>
        /// 是否有查看供应商及煤种权限
        /// </summary>
        public bool HasShowSupplier = false;

        PrintAppConfig instance = PrintAppConfig.GetInstance();
        public FrmPrint_Oper()
        {
            InitializeComponent();
        }
        public FrmPrint_Oper(String pId, bool pEdit, String type = "入厂煤", bool isShowSupplier = false)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
            printType = type;
            HasShowSupplier = isShowSupplier;
        }
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmSupplier_Oper_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            if (!String.IsNullOrEmpty(id))
            {
                try
                {
                    if (printType == "入厂煤")
                    {
                        this.buyFuelTransport = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(this.id);
                    }
                    else if (printType == "其他物资")
                    {
                        this.goodsTransport = Dbers.GetInstance().SelfDber.Get<CmcsGoodsTransport>(this.id);
                    }
                    makeImage(null, null);
                }
                catch (Exception ex)
                {
                    MessageBoxEx.Show("打印失败，请联系系统管理员！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        //打印事件
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (PrintCode())
            {
                if (printType == "入厂煤")
                    UpdateBuyFuelTransports();
                else if (printType == "其他物资")
                    UpdateGoodsTransports();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #region 磅单打印完成后更新业务流程
        /// <summary>
        /// 更新流程
        /// </summary>
        private void UpdateBuyFuelTransports()
        {
            if (buyFuelTransport != null)
            {
                //完成流程
                buyFuelTransport.IsFinish = 1;
                buyFuelTransport.IsPrint = 1;
                if (Dbers.GetInstance().SelfDber.Update(buyFuelTransport) > 0)
                {
                    //打印磅单时删除临时运输记录
                    CmcsUnFinishTransport unFinishTransport = Dbers.GetInstance().SelfDber.Entity<CmcsUnFinishTransport>("where TransportId=:TransportId", new { TransportId = this.id });
                    if (unFinishTransport != null)
                        Dbers.GetInstance().SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                }
            }
        }

        /// <summary>
        /// 更新流程
        /// </summary>
        private void UpdateGoodsTransports()
        {
            if (goodsTransport != null)
            {
                //完成流程
                goodsTransport.IsFinish = 1;
                goodsTransport.IsPrint = 1;
                if (Dbers.GetInstance().SelfDber.Update(goodsTransport) > 0)
                {
                    //打印磅单时删除临时运输记录
                    CmcsUnFinishTransport unFinishTransport = Dbers.GetInstance().SelfDber.Entity<CmcsUnFinishTransport>("where TransportId=:TransportId", new { TransportId = this.id });
                    if (unFinishTransport != null)
                        Dbers.GetInstance().SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                }
            }
        }

        #endregion

        #region 打印方法
        private bool PrintCode()
        {
            try
            {
                PrintDocument prtdoc = this.printDocument1;
                prtdoc.DefaultPageSettings.PaperSize = new PaperSize("Custum", 310, 650);
                prtdoc.OriginAtMargins = true;
                prtdoc.DefaultPageSettings.Margins.Left = 3;
                prtdoc.DefaultPageSettings.Margins.Right = 3;
                prtdoc.DefaultPageSettings.Margins.Top = 15;
                prtdoc.DefaultPageSettings.Margins.Bottom = 5;
                prtdoc.PrintController = new StandardPrintController();
                prtdoc.PrintPage += new PrintPageEventHandler(makeImage);

                for (int i = 0; i < instance.PrintNums; i++)
                {
                    try
                    {
                        prtdoc.Print();
                    }
                    catch
                    {

                    }
                }
                return true;
            }
            catch (Exception)
            {
                MessageBoxEx.Show("打印机出现异常，请检查！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 生成磅单预览图
        private void makeImage(object sender, PrintPageEventArgs e)
        {
            Font fontTitle = new Font("黑体", 14, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontTitle1 = new Font("黑体", 12, FontStyle.Bold, GraphicsUnit.Pixel);

            Font fontContent = new Font("黑体", instance.FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontPrintTime = new Font("黑体", instance.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontSupplier = new Font("黑体", instance.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            float leftPadding = instance.LeftPadding;
            int PageIndex = 1;

            Graphics g;
            Bitmap result = new Bitmap(600, 600);
            if (e == null)
            {
                g = Graphics.FromImage(result);
                g.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, result.Width, result.Height);
            }
            else
                g = e.Graphics;

            // 行间距 30 
            float TopValue = 53;
            Image img;
            String path = Application.StartupPath + "\\logos.png";
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                img = Image.FromStream(fs);
            }

            string printValue = "";

            g.DrawImage(img, leftPadding + 10, 25);
            g.DrawString("国家电投河南公司沁阳发电分公司", fontTitle1, Brushes.Black, leftPadding + 40, 40);
            TopValue += 15;

            g.DrawString("过  磅  单", new Font("黑体", 18, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, leftPadding + 85, TopValue);
            TopValue += 34;

            g.DrawLine(new Pen(Color.Black, 2), leftPadding + 0, TopValue, 300 - 10, TopValue);
            TopValue += 15;

            g.DrawString("打印时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), fontPrintTime, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            if (printType == "入厂煤")
            {
                #region 入厂煤数据打印
                CmcsBuyFuelTransport entity = this.buyFuelTransport;
                List<CmcsBuyFuelTransportDeduct> deduct = Dbers.GetInstance().SelfDber.Entities<CmcsBuyFuelTransportDeduct>("where TransportId=:TransportId", new { TransportId = entity.Id });
                if (deduct != null && deduct.Count > 0)
                {
                    entity.DeductWeight = deduct.Sum(a => a.DeductWeight);
                }
                g.DrawString("流 水 号：" + entity.SerialNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("车 牌 号：" + entity.CarNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                string FuelKindName = HasShowSupplier ? entity.FuelKindName : "****";
                g.DrawString("煤    种：" + FuelKindName, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                string mineName = HasShowSupplier ? entity.MineName : "****";
                g.DrawString("矿    点：" + mineName, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                printValue = entity.TheInFactoryBatch != null ? entity.TheInFactoryBatch.Batch : string.Empty;
                g.DrawString("批 次 号：" + printValue, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("收货单位：", fontContent, Brushes.Black, leftPadding + 0, TopValue);
                printValue = "国家电投河南公司沁阳发电分公司";

                SizeF SizeTitle = g.MeasureString("收货单位：", fontContent);

                TopValue = DrawContent(fontSupplier, leftPadding + SizeTitle.Width, g, TopValue, printValue);

                g.DrawString("运输单位：", fontContent, Brushes.Black, leftPadding, TopValue);
                printValue = entity.TransportCompanyName != null ? entity.TransportCompanyName : string.Empty;

                SizeTitle = g.MeasureString("运输单位：", fontContent);

                TopValue = DrawContent(fontSupplier, leftPadding + SizeTitle.Width, g, TopValue, printValue);

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

                g.DrawString(string.Format("盈 亏 量：{0} 吨", Math.Round((entity.ProfitWeight), 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("净    重：{0} 吨", Math.Round(entity.SuttleWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("验 收 量：{0} 吨", Math.Round(entity.CheckWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;
                #endregion
            }
            else if (printType == "其他物资")
            {
                #region 其他物资数据打印
                CmcsGoodsTransport entity = this.goodsTransport;
                g.DrawString("流 水 号：" + entity.SerialNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("车 牌 号：" + entity.CarNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("供货单位：", fontContent, Brushes.Black, leftPadding + 0, TopValue);
                printValue = entity.SupplyUnitName != null ? entity.SupplyUnitName : string.Empty;
                SizeF SizeTitle = g.MeasureString("供货单位：", fontContent);
                TopValue = DrawContent(fontSupplier, leftPadding + SizeTitle.Width, g, TopValue, printValue);

                g.DrawString("收货单位：", fontContent, Brushes.Black, leftPadding + 0, TopValue);
                //printValue = "国家电投河南公司沁阳发电分公司";
                printValue = entity.ReceiveUnitName != null ? entity.ReceiveUnitName : "国家电投河南公司沁阳发电分公司";
                SizeTitle = g.MeasureString("收货单位：", fontContent);
                TopValue = DrawContent(fontSupplier, leftPadding + SizeTitle.Width, g, TopValue, printValue);

                g.DrawString("物资类型：" + entity.GoodsTypeName, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("第一次称重：", fontPrintTime, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("称重时间：" + DisposeTime(entity.FirstTime.ToString(), "yyyy-MM-dd HH:mm"), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("重    量：{0} 吨", Math.Round(entity.FirstWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("第二次称重：", fontPrintTime, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("称重时间：" + DisposeTime(entity.SecondTime.ToString(), "yyyy-MM-dd HH:mm"), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("重    量：{0} 吨", Math.Round(entity.SecondWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("净    重：{0} 吨", Math.Round(entity.SuttleWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;
                #endregion
            }

            g.DrawString(string.Format("操 作 员：{0}", SelfVars.LoginUser.UserName), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(PageIndex.ToString() + "联", fontTitle, Brushes.Black, leftPadding + 110, TopValue);
            TopValue += 20;

            if (e == null)
            {
                this.pictureBox1.Image = result;
                g.Dispose();
            }
        }

        #endregion


        #region 辅助方法
        private float DrawContent(Font fontSupplier, float leftPadding, Graphics g, float TopValue, string printValue)
        {
            if (!String.IsNullOrEmpty(printValue) && printValue.Length > instance.RowMaxChaNums)
            {
                int rows = printValue.Length % instance.RowMaxChaNums == 0 ? printValue.Length / instance.RowMaxChaNums : ((printValue.Length / instance.RowMaxChaNums) + 1);
                for (int i = 0; i < rows; i++)
                {
                    if (i == rows - 1)
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, printValue.Length - i * instance.RowMaxChaNums), fontSupplier, Brushes.Black, leftPadding, TopValue);
                        TopValue += 20;
                        break;
                    }
                    else
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, instance.RowMaxChaNums), fontSupplier, Brushes.Black, leftPadding, TopValue);
                        TopValue += 20;
                    }
                }
            }
            else
            {
                g.DrawString(printValue, fontSupplier, Brushes.Black, leftPadding, TopValue);
                TopValue += 20;
            }
            return TopValue;
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

        #endregion
    }
}