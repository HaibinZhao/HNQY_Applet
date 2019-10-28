using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace BalanceDataGraber
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool notRun;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out notRun))
            {
                if (notRun)
                {
                    // 检测更新
                    AU.Updater updater = new AU.Updater();
                    if (updater.NeedUpdate())
                    {
                        Process.Start("AutoUpdater.exe");
                        Environment.Exit(0);
                    }

                    Application.Run(new Form1());
                }
                else
                    MessageBox.Show("程序正在运行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
