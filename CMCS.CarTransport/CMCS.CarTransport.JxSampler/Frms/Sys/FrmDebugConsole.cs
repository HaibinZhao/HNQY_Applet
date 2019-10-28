using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.CarTransport.JxSampler.Frms;
using CMCS.CarTransport.JxSampler.Core;

namespace CMCS.CarTransport.JxSampler.Frms.Sys
{
    /// <summary>
    /// �����������̨
    /// </summary>
    public partial class FrmDebugConsole : DevComponents.DotNetBar.Metro.MetroForm
    {
        private static FrmDebugConsole instance;

        public static FrmDebugConsole GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmDebugConsole();
                instance.Show();
            }

            return instance;
        }

        private FrmDebugConsole()
        {
            InitializeComponent();
        }

        private void FrmDebugConsole_Load(object sender, EventArgs e)
        {

        }

        public void Output(string message)
        {
            try
            {
                rtxtOutput.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message + Environment.NewLine);
                rtxtOutput.ScrollToCaret();
            }
            catch { }
        }

        /// <summary>
        /// ģ��ˢ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtVoucher.Text.Trim()))
            {
                MessageBoxEx.Show("�����복�ƺ�\\��ǩ�ţ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FrmJxSampler.passCarQueuer.Enqueue(txtVoucher.Text.Trim());

            Output("ģ��������" + txtVoucher.Text.Trim());
        }
    }
}