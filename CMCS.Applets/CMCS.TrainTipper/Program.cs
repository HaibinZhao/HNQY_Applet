﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//
using System.Threading;
using BasisPlatform;
using CMCS.Common;
using System.Diagnostics;
using CMCS.DotNetBar.Utilities;
using CMCS.TrainTipper.Frms.Sys;

namespace CMCS.TrainTipper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        { 
            // 检测更新
            AU.Updater updater = new AU.Updater();
            if (updater.NeedUpdate())
            {
                Process.Start("AutoUpdater.exe");
                Environment.Exit(0);
            }

            // BasisPlatform:应用程序初始化
            Basiser basiser = Basiser.GetInstance();
            basiser.EnabledEbiaSupport = true;
            basiser.InitBasisPlatform(CommonAppConfig.GetInstance().AppIdentifier, PlatformType.Winform);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DotNetBarUtil.InitLocalization();

            bool notRunning;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out notRunning))
            {
                if (notRunning) Application.Run(new FrmLogin());
            }
        }
    }
}
