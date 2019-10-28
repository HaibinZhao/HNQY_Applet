using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace RW.LZR12
{
    /// <summary>
    /// LZR12 读卡器
    /// </summary>
    public class Lzr12Rwer_Com
    {
        public Lzr12Rwer_Com()
        {
            timer1 = new System.Timers.Timer(this.scanInterval)
            {
                AutoReset = true
            };
            timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
        }

        RfidApiLib.RfidApi Api = new RfidApiLib.RfidApi();

        /// <summary>
        /// 串口
        /// </summary>
        private int Com = 1;

        private string errorMessage = string.Empty;
        /// <summary>
        /// 当前读卡类错误信息
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        private bool status = false;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Status
        {
            get { return status; }
        }

        /// <summary>
        /// 设置连接状态
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(bool status)
        {
            if (this.status != status && this.OnStatusChange != null) this.OnStatusChange(status);
            this.status = status;
        }

        private string startWith = string.Empty;
        /// <summary>
        /// 标签号筛选过滤
        /// </summary>
        public string StartWith
        {
            get { return startWith; }
            set { startWith = value; }
        }

        private System.Timers.Timer timer1;

        private double scanInterval = 200;
        /// <summary>
        /// 扫描间隔 单位：毫秒  默认值：200
        /// </summary>
        public double ScanInterval
        {
            get { return scanInterval; }
            set
            {
                if (scanInterval > 0)
                {
                    scanInterval = value;
                    timer1.Interval = scanInterval;
                }
            }
        }
        public delegate void ScanErrorEventHandler(Exception ex);
        public event ScanErrorEventHandler OnScanError;

        public delegate void StatusChangeHandler(bool status);
        public event StatusChangeHandler OnStatusChange;

        public delegate void ScanSuccessEventHandler(List<string> tags);
        public event ScanSuccessEventHandler OnScanSuccess;

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="com">端口号</param>
        /// <param name="power">功率值，取值为0~30，对应0~30dBm.</param>
        /// <param name="freq_type">频率类型 取0时为国标（920M~925M），取1时为美标（902M~928M） </param>
        /// <returns></returns>
        public bool OpenCom(int com, byte power = 30, byte freq_type = 0)
        {
            try
            {
                this.Com = com;
                if (this.Api.OpenCommPort("COM" + com.ToString()) == 0)
                {
                    Api.SetRf(power, freq_type);
                    SetStatus(true);
                    return true;
                }

                SetStatus(false);
                if (this.OnStatusChange != null) this.OnStatusChange(this.status);
            }
            catch (Exception ex)
            {
                if (OnScanError != null) OnScanError(ex);

                this.status = false;
                if (this.OnStatusChange != null) this.OnStatusChange(status);
            }

            return this.status;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public void CloseCom()
        {
            try
            {
                SetStatus(false);

                Api.CloseCommPort();

            }
            catch (Exception ex)
            {
                if (OnScanError != null) OnScanError(ex);
            }
        }

        /// <summary>
        /// 扫描标签卡
        /// </summary>
        /// <returns></returns>
        public List<string> ScanTags()
        {
            List<string> tags = new List<string>();

            byte tagCount = 0, tagFlag = 0;
            byte[,] tagBuffer = new byte[100, 12];

            try
            {
                this.Api.ClearIdBuf();

                if (this.Api.EpcMultiTagIdentify(ref tagBuffer, ref tagCount, ref tagFlag) == 0)
                {
                    if (tagCount >= 100) tagCount = 100;

                    if (tagCount > 0)
                    {
                        SetStatus(true);

                        for (int i = 0; i < tagCount; i++)
                        {
                            string tag = string.Empty;
                            for (int j = 0; j < 12; j++)
                            {
                                tag += string.Format("{0:X2}", tagBuffer[i, j]);
                            }

                            if (string.IsNullOrEmpty(this.startWith) || (!string.IsNullOrEmpty(this.startWith) && tag.StartsWith(this.startWith)))
                            {
                                tags.Add(tag);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Api.ClearIdBuf();

                if (OnScanError != null) OnScanError(ex);

                SetStatus(false);
            }
            finally
            {

            }

            return tags;
        }

        /// <summary>
        /// 修改标签号
        /// </summary>
        /// <param name="epc">新标签号</param>
        /// <returns></returns>
        public bool WriteEPC(string epc)
        {
            //0:RESERVE  1:EPC  2:TID  3:USER  读取区域，销毁密码和访问密码区为0，EPC编码区为1，USER区为3区
            byte membank = (byte)(1);
            //起始地址 2
            byte wordptr = (byte)(2);
            //子长度
            ushort[] value = HexStringToUshortArray(epc);
            int status = 0;

            for (byte j = 0; j < 6; j++)
            {
                for (int nRetry = 0; nRetry < 5; nRetry++)
                {
                    status = Api.EpcWrite(membank, (byte)(wordptr + j), value[j]);
                    if (status == 0)
                        break;
                }

                if (status != 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化标签卡长度
        /// </summary>
        /// <param name="bit_cnt"></param>
        /// <returns></returns>
        public bool InitEPC(byte bit_cnt = 96)
        {
            return Api.EpcInitEpc(bit_cnt) == 0;
        }

        void timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            //timer1.Stop();
            try
            {
                List<string> tags = ScanTags();

                if (OnScanSuccess != null) OnScanSuccess(tags);
            }
            catch (Exception ex)
            {
                if (OnScanError != null) OnScanError(ex);

                SetStatus(false);
            }
            finally
            {
                //timer1.Start();
            }
        }

        /// <summary>
        /// 开始读卡
        /// </summary>
        public void StartRead()
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        /// <summary>
        /// 停止读卡
        /// </summary>
        public void StopRead()
        {
            timer1.Enabled = false;
            timer1.Stop();
        }

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private ushort[] HexStringToUshortArray(string str)
        {
            str = str.Replace(" ", "");
            ushort[] buffer = new ushort[str.Length / 4];
            for (int i = 0; i < str.Length; i += 4)
                buffer[i / 4] = (ushort)Convert.ToInt32(str.Substring(i, 4), 16);
            return buffer;
        }
    }
}
