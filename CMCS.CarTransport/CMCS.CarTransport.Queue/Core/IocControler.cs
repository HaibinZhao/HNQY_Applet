﻿//
using System.Threading;
using CMCS.Common.DAO;
using CMCS.Common;
using CMCS.CarTransport.Queue.Frms.Sys;
using IOC.JMDMYTWI8DOMR;

namespace CMCS.CarTransport.Queue.Core
{
    /// <summary>
    /// IO控制器设备控制
    /// </summary>
    public class IocControler
    {
        JMDMYTWI8DOMRIocer Iocer;
        CommonAppConfig commonAppConfig = CommonAppConfig.GetInstance();

        public IocControler(JMDMYTWI8DOMRIocer iocer)
        {
            this.Iocer = iocer;
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();

        /// <summary>
        /// 道闸1升杆
        /// </summary>
        public void Gate1Up()
        {
#if DEBUG
            FrmDebugConsole.GetInstance().Output("道闸1升杆");
#endif

            int port = commonDAO.GetAppletConfigInt32("IO控制器_道闸1升杆端口");

            this.Iocer.Output(port, true);
            Thread.Sleep(100);
            this.Iocer.Output(port, false);
            Thread.Sleep(500);

            commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, "道闸1升杆", "1");
        }

        /// <summary>
        /// 道闸1降杆
        /// </summary>
        public void Gate1Down()
        {
#if DEBUG
            FrmDebugConsole.GetInstance().Output("道闸1降杆");
#endif
            int port = commonDAO.GetAppletConfigInt32("IO控制器_道闸1降杆端口");

            this.Iocer.Output(port, true);
            Thread.Sleep(100);
            this.Iocer.Output(port, false);
            Thread.Sleep(500);

            commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, "道闸1升杆", "0");
        }

        /// <summary>
        /// 道闸2升杆
        /// </summary>
        public void Gate2Up()
        {
#if DEBUG
            FrmDebugConsole.GetInstance().Output("道闸2升杆");
#endif
            int port = commonDAO.GetAppletConfigInt32("IO控制器_道闸2升杆端口");

            this.Iocer.Output(port, true);
            Thread.Sleep(100);
            this.Iocer.Output(port, false);
            Thread.Sleep(500);

            commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, "道闸2升杆", "1");
        }

        /// <summary>
        /// 道闸2降杆
        /// </summary>
        public void Gate2Down()
        {
#if DEBUG
            FrmDebugConsole.GetInstance().Output("道闸2降杆");
#endif
            int port = commonDAO.GetAppletConfigInt32("IO控制器_道闸2降杆端口");

            this.Iocer.Output(port, true);
            Thread.Sleep(100);
            this.Iocer.Output(port, false);
            Thread.Sleep(500);

            commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, "道闸2升杆", "0");
        }

        /// <summary>
        /// 信号灯1红灯
        /// </summary>
        public void RedLight1()
        {
#if DEBUG
            FrmDebugConsole.GetInstance().Output("信号灯1红灯");
#endif
            this.Iocer.Output(commonDAO.GetAppletConfigInt32("IO控制器_信号灯1端口"), false);
            Thread.Sleep(500);

            commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, "信号灯1", "1");
        }

        /// <summary>
        /// 信号灯1绿灯
        /// </summary>
        public void GreenLight1()
        {
#if DEBUG
            FrmDebugConsole.GetInstance().Output("信号灯1绿灯");
#endif
            this.Iocer.Output(commonDAO.GetAppletConfigInt32("IO控制器_信号灯1端口"), true);
            Thread.Sleep(500);

            commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, "信号灯1", "0");
        }

        /// <summary>
        /// 信号灯2红灯
        /// </summary>
        public void RedLight2()
        {
#if DEBUG
            FrmDebugConsole.GetInstance().Output("信号灯2红灯");
#endif
            this.Iocer.Output(commonDAO.GetAppletConfigInt32("IO控制器_信号灯2端口"), false);
            Thread.Sleep(500);

            commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, "信号灯2", "1");
        }

        /// <summary>
        /// 信号灯2绿灯
        /// </summary>
        public void GreenLight2()
        {
#if DEBUG
            FrmDebugConsole.GetInstance().Output("信号灯2绿灯");
#endif
            this.Iocer.Output(commonDAO.GetAppletConfigInt32("IO控制器_信号灯2端口"), true);
            Thread.Sleep(500);

            commonDAO.SetSignalDataValue(commonAppConfig.AppIdentifier, "信号灯2", "0");
        }
    }
}
