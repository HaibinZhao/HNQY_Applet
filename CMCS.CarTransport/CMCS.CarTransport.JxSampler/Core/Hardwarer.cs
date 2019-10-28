
namespace CMCS.CarTransport.JxSampler.Core
{
    /// <summary>
    /// 硬件设备类
    /// </summary>
    public class Hardwarer
    {
        static IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer iocer = new IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer();
        /// <summary>
        /// IO控制器
        /// </summary>
        public static IOC.JMDMYTWI8DOMR.JMDMYTWI8DOMRIocer Iocer
        {
            get { return iocer; }
        }

        static RW.LZR12.Lzr12Rwer rwer1 = new RW.LZR12.Lzr12Rwer();
        /// <summary>
        /// 读卡器1
        /// </summary>
        public static RW.LZR12.Lzr12Rwer Rwer1
        {
            get { return rwer1; }
        }
    }
}
