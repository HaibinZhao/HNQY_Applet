using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using RfidApiLib;

namespace RW.UHFApiV10
{
    public class UHFApiV10Rwer
    {
        RfidApi rfidApi = null;

        public UHFApiV10Rwer()
        {
            rfidApi = new RfidApi();
        }

        /// <summary>
        /// 读取区域 销毁密码和访问密码区为0，EPC编码区为1，USER区为3区
        /// </summary>
        private byte MemBank = 1;
        /// <summary>
        /// 起始地址
        /// </summary>
        private int WordPtr = 2;
        /// <summary>
        /// 安全读取数据的长度
        /// </summary>
        private byte WordCnt = 6;



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


        private List<string> tags = new List<string>();
        /// <summary>
        /// 当前读取到的标签集
        /// </summary>
        public List<string> Tags
        {
            get { return tags; }
        }

        public delegate void ScanErrorEventHandler(Exception error);
        public event ScanErrorEventHandler OnScanError;

        public delegate void StatusChangeHandler(bool status);
        public event StatusChangeHandler OnStatusChange;

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="com">端口号</param> 
        /// <param name="bandrate">波特率</param>
        /// <returns></returns>
        public bool OpenCom(string com)
        {
            if (!com.ToUpper().Contains("COM"))
                com = "COM" + com;

            try
            {
                if (rfidApi.OpenCommPort(com) == 0)
                {
                    SetStatus(true);
                }
                else
                {
                    this.status = false;
                    if (this.OnStatusChange != null) this.OnStatusChange(this.status);
                }

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
                rfidApi.CloseCommPort();//释放端口连接

                SetStatus(false);

            }
            catch (Exception ex)
            {
                if (OnScanError != null) OnScanError(ex);
            }
        }


        /// <summary>
        /// 修改标签号
        /// </summary>
        /// <param name="epc">新标签号</param>
        /// <returns></returns>
        public bool WriteEPC(string epc)
        {
            ushort[] value = new ushort[16];

            byte membank;
            byte wordptr;
            byte wordcnt;
            int status;
            int count = 0;

            membank = (byte)(this.MemBank);
            wordptr = (byte)(this.WordPtr);
            wordcnt = (byte)(this.WordCnt);

            for (int h = 0; h < epc.Length / 2 / 2; h++)
            {
                value[h] = (ushort)Convert.ToInt32(epc.Substring(h * 4, 4), 16);
            }

            try
            {
                for (byte j = 0; j < wordcnt; j++)
                {
                    for (int nRetry = 0; nRetry < 25; nRetry++)
                    {
                        status = rfidApi.EpcWrite(membank, (byte)(wordptr + j), value[j]);

                        if (status == 0)
                        {
                            count++;
                            break;
                        }
                    }
                }

                if (count == wordcnt)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public List<string> ScaleTags()
        {
            List<string> tags = new List<string>();

            string tag_temp = "";
            string tag = "";

            try
            {
                int status;
                int i, j;
                byte[,] IsoBuf = new byte[100, 12];
                byte tag_cnt = 0;
                byte tag_flag = 0;

                this.rfidApi.ClearIdBuf();

                status = rfidApi.EpcMultiTagIdentify(ref IsoBuf, ref tag_cnt, ref tag_flag);

                if (tag_cnt > 0)
                {
                    for (i = 0; i < tag_cnt; i++)
                    {

                        for (j = 0; j < Convert.ToInt16(this.WordCnt) * 2; j++)
                        {
                            tag_temp = string.Format("{0:X2}", IsoBuf[i, j]);
                            tag += tag_temp;
                        }
                        tags.Add(tag);

                    }
                }

            }
            catch (Exception ex)
            {
                if (OnScanError != null) OnScanError(ex);

                SetStatus(false);
            }

            return tags;
        }

    }
}
