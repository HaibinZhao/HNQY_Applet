using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using BasisPlatform;
using CMCS.Common;
using CMCS.DotNetBar.Utilities;
using CMCS.WeighCheck.SampleCheck.Frms.Sys;

namespace CMCS.WeighCheck.SampleCheck
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

            Application.Run(new FrmLogin());
        }
    }
}
