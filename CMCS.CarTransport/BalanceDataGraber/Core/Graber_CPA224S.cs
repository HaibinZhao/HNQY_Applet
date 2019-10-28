using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.IO.Ports;
using System.Windows.Forms;

namespace BalanceDataGraber.Core
{
    /// <summary>
    /// 赛多利斯天平取数 CPA224S
    /// </summary>
    class Graber_CPA224S
    {
        public delegate void OutputInvokeEventHandler(double value);
        public event OutputInvokeEventHandler OnOutputInvoke;

        private SerialPort _SerialPort = new SerialPort();

        /// <summary>
        /// 临时数据集
        /// </summary>
        List<byte> TempReceiveData = new List<byte>();

        /// <summary>
        /// 是否已打开
        /// </summary>
        public bool IsOpen
        {
            get { return this._SerialPort.IsOpen; }
        }

        public Graber_CPA224S()
        {

        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="com">COM 端口</param>
        /// <param name="bandrate">波特率</param>
        /// <param name="receivedBytesThreshold">内部输入缓冲区字节数</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">校验协议</param>
        /// <returns></returns>
        public bool Open(int com, int bandrate, int receivedBytesThreshold = 1, int dataBits = 8, StopBits stopBits = StopBits.One, Parity parity = Parity.None)
        {
            this._SerialPort.PortName = "COM" + com.ToString();
            this._SerialPort.BaudRate = bandrate;
            this._SerialPort.DataBits = dataBits;
            this._SerialPort.StopBits = stopBits;
            this._SerialPort.Parity = parity;
            this._SerialPort.DataReceived += new SerialDataReceivedEventHandler(_SerialPort_DataReceived);
            this._SerialPort.ReceivedBytesThreshold = receivedBytesThreshold;
            this._SerialPort.RtsEnable = true; 
            this._SerialPort.DtrEnable = true; 

            try
            {
                this._SerialPort.Open();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("设备连接失败!" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            try
            {
                this._SerialPort.Close();
            }
            catch
            {

            }
        }

        void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!this._SerialPort.IsOpen) return;

            int bytesToRead = this._SerialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            this._SerialPort.Read(buffer, 0, bytesToRead);

            

            foreach (byte b in buffer)
            {
                if (b == 0xCE) this.TempReceiveData.Clear();

                this.TempReceiveData.Add(b);

                if (b == 0x8A && this.TempReceiveData.Count == 22)
                {
                    double value = 0;

                    try
                    {
                        string strTemp = string.Empty;

                        for (int i = 7; i <= 16; i++)
                        {
                            if (i == 11) continue;

                            strTemp += this.TempReceiveData[i].ToString("X2").Substring(1, 1);
                        }

                        double.TryParse(strTemp, out value);

                        if (this.TempReceiveData[6] == 0xAD) value = value * -1;
                    }
                    catch { }

                    if (OnOutputInvoke != null) OnOutputInvoke(value / 100000d);
                }
            }

        }
    }
}
