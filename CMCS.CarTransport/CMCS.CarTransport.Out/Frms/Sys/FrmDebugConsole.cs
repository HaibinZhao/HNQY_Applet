using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CMCS.CarTransport.Out.Core;
using CMCS.CarTransport.Out.Enums;

namespace CMCS.CarTransport.Out.Frms.Sys
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
            cmbPassWay.Items.Add(new DataItem("��·һ", "��·һ", ePassWay.Way1));
            cmbPassWay.Items.Add(new DataItem("��·��", "��·��", ePassWay.Way2));
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

            FrmOuter.passCarQueuer.Enqueue((ePassWay)(cmbPassWay.SelectedItem as DataItem).Data, txtVoucher.Text.Trim(), true);

            Output("ģ��������" + txtVoucher.Text.Trim() + "  " + cmbPassWay.Text);
        }
    }
}