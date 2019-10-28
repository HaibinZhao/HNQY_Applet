using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.CarTransport.Weighter.Utilities;
using CMCS.Common;
using CMCS.Common.Entities;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Entities.iEAA;
using DevComponents.DotNetBar;
using ThoughtWorks.QRCode.Codec;

namespace CMCS.CarTransport.Weighter.Frms
{
    public partial class FrmWeighter_Print : DevComponents.DotNetBar.Metro.MetroForm
    {
        String id = String.Empty;
        CmcsBuyFuelTransport cmcsSupplier;
        PrintAppConfig instance = PrintAppConfig.GetInstance();
        public FrmWeighter_Print()
        {
            InitializeComponent();
        }
        public FrmWeighter_Print(String pId)
        {
            InitializeComponent();
            id = pId;
        }
        private void FrmSupplier_Oper_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            if (!String.IsNullOrEmpty(id))
            {
                try
                {
                    this.cmcsSupplier = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(this.id);
                    makeImage(null,null);
                }
                catch (Exception ex)
                {
                    MessageBoxEx.Show("打印失败，请联系系统管理员！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (PrintCode())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 生成预览图
        private void makeImage(object sender, PrintPageEventArgs e)
        {
            Font fontTitle = new Font("黑体", instance.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            CmcsBuyFuelTransport entity = this.cmcsSupplier;
            String sampleCode = "";
            String BatchCode = "";
            String weight = "";
            String carNumber = "";

            CmcsInFactoryBatch batch = Dbers.GetInstance().SelfDber.Get<CmcsInFactoryBatch>(entity.InFactoryBatchId);
            BatchCode = batch == null ? "" : batch.Batch;
            CmcsRCSampling sampling = Dbers.GetInstance().SelfDber.Entities<CmcsRCSampling>(" where InFactoryBatchId=:InFactoryBatchId", new { InFactoryBatchId = entity.InFactoryBatchId }).FirstOrDefault();
            sampleCode = sampling == null ? "" : sampling.SampleCode;

            DataTable lmyb = Dbers.GetInstance().SelfDber.ExecuteDataTable(String.Format("select a.CoalNumber,a.TRANSFERNUMBER from fultbtransfer a where id=(select t.lmybid from fultbinfactorybatch t where t.id='{0}')", entity.InFactoryBatchId));
            if (lmyb != null && lmyb.Rows.Count > 0)
            {
                weight = instance.ParseDecimal(lmyb.Rows[0][0]).ToString("F2");
                carNumber = instance.ParseDecimal(lmyb.Rows[0][1]).ToString("F0");
            }
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
            float TopValue = 0;
            Bitmap bmp = null;

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = instance.ImgSize;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            bmp = qrCodeEncoder.Encode(sampleCode);

            string printValue = "";

            g.DrawImage(bmp, instance.ImgLeftPadding, instance.ImgTopPadding);
            //TopValue = instance.ImgTopPadding + bmp.Height + instance.CharToImg;
            TopValue = instance.CharToImg;
            float leftValue = instance.ImgLeftPadding + bmp.Width + instance.CharLeftPadding;

            printValue = "车    号：" + entity.CarNumber;
            TopValue = DrawContent(fontTitle, g, TopValue, printValue, leftValue);

            //printValue = "批次编号：" + BatchCode;
            //TopValue = DrawContent(fontTitle, g, instance.ImgTopPadding + bmp.Height + 5, printValue, instance.ImgLeftPadding + 5);

            //printValue = "采样编码：" + sampleCode;
            //TopValue = DrawContent(fontTitle, g, TopValue, printValue);
            printValue = "计划车数：" + (String.IsNullOrEmpty(carNumber) ? "0  " : carNumber) + " 车";
            TopValue = DrawContent(fontTitle, g, TopValue, printValue, leftValue);


            printValue = "计划煤量：" + (String.IsNullOrEmpty(weight) ? "0.00" : weight) + " 吨";
            TopValue = DrawContent(fontTitle, g, TopValue, printValue, leftValue);


            printValue = "批次编号：" + BatchCode;
            TopValue = DrawContent(fontTitle, g, instance.ImgTopPadding + bmp.Height + 5, printValue, instance.ImgLeftPadding + 5);

            if (e == null)
            {
                this.pictureBox1.Image = result;
                g.Dispose();
            }
        }

        private float DrawContent(Font fontTitle, Graphics g, float TopValue, string printValue,float leftValue)
        {
            if (!String.IsNullOrEmpty(printValue) && printValue.Length > instance.RowMaxChaNums)
            {
                int rows = printValue.Length % instance.RowMaxChaNums == 0 ? printValue.Length / instance.RowMaxChaNums : ((printValue.Length / instance.RowMaxChaNums) + 1);
                for (int i = 0; i < rows; i++)
                {
                    if (i == rows - 1)
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, printValue.Length - i * instance.RowMaxChaNums), fontTitle, Brushes.Black, leftValue, TopValue);
                        TopValue += instance.CharLineSpacing;
                        break;
                    }
                    else
                    {
                        g.DrawString(printValue.Substring(i * instance.RowMaxChaNums, instance.RowMaxChaNums), fontTitle, Brushes.Black, leftValue, TopValue);
                        TopValue += instance.CharLineSpacing;
                    }
                }
            }
            else
            {
                g.DrawString(printValue, fontTitle, Brushes.Black, leftValue, TopValue);
                TopValue += instance.CharLineSpacing;
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