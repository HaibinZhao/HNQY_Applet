using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//
using System.Threading;
using BasisPlatform;
using CMCS.Common;

namespace CMCS.DumblyConcealer.Win
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
            basiser.EnabledEbiaSupport = true;
            basiser.InitBasisPlatform(CommonAppConfig.GetInstance().AppIdentifier, PlatformType.Winform);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool notRunning;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out notRunning))
            {
                if (notRunning) Application.Run(new MDIParent1());
            }
        }
    }
}
