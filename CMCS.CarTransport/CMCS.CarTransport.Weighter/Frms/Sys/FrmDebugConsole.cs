using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar; 
using CMCS.CarTransport.Weighter.Core;
using CMCS.CarTransport.Weighter.Enums;
using CMCS.CarTransport.Weighter.Frms;

namespace CMCS.CarTransport.Weighter.Frms.Sys
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
            cmbPassWay.Items.Add(new DataItem("����һ", "����һ", eDirection.Way1));
            cmbPassWay.Items.Add(new DataItem("�����", "�����", eDirection.Way2));
            cmbPassWay.SelectedIndex = 0;
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

            FrmWeighter.passCarQueuer.Enqueue((eDirection)(cmbPassWay.SelectedItem as DataItem).Data, txtVoucher.Text.Trim());

            Output("ģ��������" + txtVoucher.Text.Trim() + "  " + cmbPassWay.Text);
        }
    }
}