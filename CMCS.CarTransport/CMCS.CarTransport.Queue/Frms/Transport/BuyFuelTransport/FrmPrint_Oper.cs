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
        //��¼ID
        String id = String.Empty;
        bool edit = false;
        /// <summary>
        /// ��ӡ���ͣ��볧ú�����������ʣ�
        /// </summary>
        String printType = String.Empty;
        //�볧ú�����¼
        CmcsBuyFuelTransport buyFuelTransport;
        //�������������¼
        CmcsGoodsTransport goodsTransport;

        /// <summary>
        /// �Ƿ��в鿴��Ӧ�̼�ú��Ȩ��
        /// </summary>
        public bool HasShowSupplier = false;

        PrintAppConfig instance = PrintAppConfig.GetInstance();
        public FrmPrint_Oper()
        {
            InitializeComponent();
        }
        public FrmPrint_Oper(String pId, bool pEdit, String type = "�볧ú", bool isShowSupplier = false)
        {
            InitializeComponent();
            id = pId;
            edit = pEdit;
            printType = type;
            HasShowSupplier = isShowSupplier;
        }
        /// <summary>
        /// ��������¼�
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
                    if (printType == "�볧ú")
                    {
                        this.buyFuelTransport = Dbers.GetInstance().SelfDber.Get<CmcsBuyFuelTransport>(this.id);
                    }
                    else if (printType == "��������")
                    {
                        this.goodsTransport = Dbers.GetInstance().SelfDber.Get<CmcsGoodsTransport>(this.id);
                    }
                    makeImage(null, null);
                }
                catch (Exception ex)
                {
                    MessageBoxEx.Show("��ӡʧ�ܣ�����ϵϵͳ����Ա��", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        //��ӡ�¼�
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (PrintCode())
            {
                if (printType == "�볧ú")
                    UpdateBuyFuelTransports();
                else if (printType == "��������")
                    UpdateGoodsTransports();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #region ������ӡ��ɺ����ҵ������
        /// <summary>
        /// ��������
        /// </summary>
        private void UpdateBuyFuelTransports()
        {
            if (buyFuelTransport != null)
            {
                //�������
                buyFuelTransport.IsFinish = 1;
                buyFuelTransport.IsPrint = 1;
                if (Dbers.GetInstance().SelfDber.Update(buyFuelTransport) > 0)
                {
                    //��ӡ����ʱɾ����ʱ�����¼
                    CmcsUnFinishTransport unFinishTransport = Dbers.GetInstance().SelfDber.Entity<CmcsUnFinishTransport>("where TransportId=:TransportId", new { TransportId = this.id });
                    if (unFinishTransport != null)
                        Dbers.GetInstance().SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        private void UpdateGoodsTransports()
        {
            if (goodsTransport != null)
            {
                //�������
                goodsTransport.IsFinish = 1;
                goodsTransport.IsPrint = 1;
                if (Dbers.GetInstance().SelfDber.Update(goodsTransport) > 0)
                {
                    //��ӡ����ʱɾ����ʱ�����¼
                    CmcsUnFinishTransport unFinishTransport = Dbers.GetInstance().SelfDber.Entity<CmcsUnFinishTransport>("where TransportId=:TransportId", new { TransportId = this.id });
                    if (unFinishTransport != null)
                        Dbers.GetInstance().SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                }
            }
        }

        #endregion

        #region ��ӡ����
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
                MessageBoxEx.Show("��ӡ�������쳣�����飡", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region ���ɰ���Ԥ��ͼ
        private void makeImage(object sender, PrintPageEventArgs e)
        {
            Font fontTitle = new Font("����", 14, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontTitle1 = new Font("����", 12, FontStyle.Bold, GraphicsUnit.Pixel);

            Font fontContent = new Font("����", instance.FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontPrintTime = new Font("����", instance.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontSupplier = new Font("����", instance.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
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

            // �м�� 30 
            float TopValue = 53;
            Image img;
            String path = Application.StartupPath + "\\logos.png";
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                img = Image.FromStream(fs);
            }

            string printValue = "";

            g.DrawImage(img, leftPadding + 10, 25);
            g.DrawString("���ҵ�Ͷ���Ϲ�˾��������ֹ�˾", fontTitle1, Brushes.Black, leftPadding + 40, 40);
            TopValue += 15;

            g.DrawString("��  ��  ��", new Font("����", 18, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, leftPadding + 85, TopValue);
            TopValue += 34;

            g.DrawLine(new Pen(Color.Black, 2), leftPadding + 0, TopValue, 300 - 10, TopValue);
            TopValue += 15;

            g.DrawString("��ӡʱ�䣺" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), fontPrintTime, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            if (printType == "�볧ú")
            {
                #region �볧ú���ݴ�ӡ
                CmcsBuyFuelTransport entity = this.buyFuelTransport;
                List<CmcsBuyFuelTransportDeduct> deduct = Dbers.GetInstance().SelfDber.Entities<CmcsBuyFuelTransportDeduct>("where TransportId=:TransportId", new { TransportId = entity.Id });
                if (deduct != null && deduct.Count > 0)
                {
                    entity.DeductWeight = deduct.Sum(a => a.DeductWeight);
                }
                g.DrawString("�� ˮ �ţ�" + entity.SerialNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("�� �� �ţ�" + entity.CarNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                string FuelKindName = HasShowSupplier ? entity.FuelKindName : "****";
                g.DrawString("ú    �֣�" + FuelKindName, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                string mineName = HasShowSupplier ? entity.MineName : "****";
                g.DrawString("��    �㣺" + mineName, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                printValue = entity.TheInFactoryBatch != null ? entity.TheInFactoryBatch.Batch : string.Empty;
                g.DrawString("�� �� �ţ�" + printValue, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("�ջ���λ��", fontContent, Brushes.Black, leftPadding + 0, TopValue);
                printValue = "���ҵ�Ͷ���Ϲ�˾��������ֹ�˾";

                SizeF SizeTitle = g.MeasureString("�ջ���λ��", fontContent);

                TopValue = DrawContent(fontSupplier, leftPadding + SizeTitle.Width, g, TopValue, printValue);

                g.DrawString("���䵥λ��", fontContent, Brushes.Black, leftPadding, TopValue);
                printValue = entity.TransportCompanyName != null ? entity.TransportCompanyName : string.Empty;

                SizeTitle = g.MeasureString("���䵥λ��", fontContent);

                TopValue = DrawContent(fontSupplier, leftPadding + SizeTitle.Width, g, TopValue, printValue);

                g.DrawString("����ʱ�䣺" + DisposeTime(entity.GrossTime.ToString(), "yyyy-MM-dd HH:mm"), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("��Ƥʱ�䣺" + DisposeTime(entity.TareTime.ToString(), "yyyy-MM-dd HH:mm"), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("��������{0} ��", Math.Round(entity.TicketWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("ë    �أ�{0} ��", Math.Round(entity.GrossWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("Ƥ    �أ�{0} ��", Math.Round(entity.TareWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("��    �֣�{0} ��", Math.Round(entity.DeductWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("ӯ �� ����{0} ��", Math.Round((entity.ProfitWeight), 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("��    �أ�{0} ��", Math.Round(entity.SuttleWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("�� �� ����{0} ��", Math.Round(entity.CheckWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;
                #endregion
            }
            else if (printType == "��������")
            {
                #region �����������ݴ�ӡ
                CmcsGoodsTransport entity = this.goodsTransport;
                g.DrawString("�� ˮ �ţ�" + entity.SerialNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("�� �� �ţ�" + entity.CarNumber, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("������λ��", fontContent, Brushes.Black, leftPadding + 0, TopValue);
                printValue = entity.SupplyUnitName != null ? entity.SupplyUnitName : string.Empty;
                SizeF SizeTitle = g.MeasureString("������λ��", fontContent);
                TopValue = DrawContent(fontSupplier, leftPadding + SizeTitle.Width, g, TopValue, printValue);

                g.DrawString("�ջ���λ��", fontContent, Brushes.Black, leftPadding + 0, TopValue);
                //printValue = "���ҵ�Ͷ���Ϲ�˾��������ֹ�˾";
                printValue = entity.ReceiveUnitName != null ? entity.ReceiveUnitName : "���ҵ�Ͷ���Ϲ�˾��������ֹ�˾";
                SizeTitle = g.MeasureString("�ջ���λ��", fontContent);
                TopValue = DrawContent(fontSupplier, leftPadding + SizeTitle.Width, g, TopValue, printValue);

                g.DrawString("�������ͣ�" + entity.GoodsTypeName, fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("��һ�γ��أ�", fontPrintTime, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("����ʱ�䣺" + DisposeTime(entity.FirstTime.ToString(), "yyyy-MM-dd HH:mm"), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("��    ����{0} ��", Math.Round(entity.FirstWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("�ڶ��γ��أ�", fontPrintTime, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString("����ʱ�䣺" + DisposeTime(entity.SecondTime.ToString(), "yyyy-MM-dd HH:mm"), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("��    ����{0} ��", Math.Round(entity.SecondWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;

                g.DrawString(string.Format("��    �أ�{0} ��", Math.Round(entity.SuttleWeight, 2).ToString("F2").PadLeft(6, ' ')), fontContent, Brushes.Black, leftPadding + 0, TopValue);
                TopValue += 20;
                #endregion
            }

            g.DrawString(string.Format("�� �� Ա��{0}", SelfVars.LoginUser.UserName), fontContent, Brushes.Black, leftPadding + 0, TopValue);
            TopValue += 20;

            g.DrawString(PageIndex.ToString() + "��", fontTitle, Brushes.Black, leftPadding + 110, TopValue);
            TopValue += 20;

            if (e == null)
            {
                this.pictureBox1.Image = result;
                g.Dispose();
            }
        }

        #endregion


        #region ��������
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