﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using BasisPlatform;
using CMCS.CommonADGS.Configurations;

namespace CMCS.CommonADGS.Server
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // BasisPlatform:应用程序初始化
            Basiser basiser = Basiser.GetInstance();
            //basiser.Init(ServerConfiguration.Instance.AppIdentifier, PlatformType.Winform, IPAddress.Parse("127.0.0.1"), 0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool notRun;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out notRun))
            {
                if (notRun)
                    Application.Run(new FrmServer());
                else
                    MessageBox.Show("程序正在运行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
