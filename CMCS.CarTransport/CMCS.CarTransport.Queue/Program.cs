﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BasisPlatform;
using System.Diagnostics;
using CMCS.Common;
using CMCS.CarTransport.Queue.Frms.Sys;
using CMCS.DotNetBar.Utilities;
using CMCS.Common.Enums;
using CMCS.CarTransport.Queue.Core;
using System.Threading;

namespace CMCS.CarTransport.Queue
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
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            DotNetBarUtil.InitLocalization();

            CMCS.Common.DAO.CommonDAO.GetInstance().SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.系统.ToString(), "1");
            bool notRunning;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out notRunning))
            {
                if (notRunning) Application.Run(new FrmLogin());
            }
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            CMCS.Common.DAO.CommonDAO.GetInstance().SetSignalDataValue(CommonAppConfig.GetInstance().AppIdentifier, eSignalDataName.系统.ToString(), "0");
        }
    }
}
