using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CMCS.DotNetBar.Utilities;
using BasisPlatform;
using CMCS.Common;

namespace CMCS.DataTester
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
            basiser.EnabledEbiaSupport = false;
            basiser.InitBasisPlatform(CommonAppConfig.GetInstance().AppIdentifier, PlatformType.Winform);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DotNetBarUtil.InitLocalization();

            Application.Run(new MDIParent1());
        }
    }
}
