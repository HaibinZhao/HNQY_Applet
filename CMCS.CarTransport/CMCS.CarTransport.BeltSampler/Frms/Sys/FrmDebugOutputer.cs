using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace CMCS.CarTransport.BeltSampler.Frms.Sys
{
    /// <summary>
    /// 调试输出控制台
    /// </summary>
    public partial class FrmDebugOutputer : DevComponents.DotNetBar.Metro.MetroForm
    {
        private static FrmDebugOutputer instance;

        public static FrmDebugOutputer GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmDebugOutputer();
                instance.Show();
            }

            return instance;
        }

        private FrmDebugOutputer()
        {
            InitializeComponent();
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
    }
}