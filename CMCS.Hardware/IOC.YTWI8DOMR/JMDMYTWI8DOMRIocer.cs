using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace IOC.JMDMYTWI8DOMR
{
    public class JMDMYTWI8DOMRIocer
    {
        public JMDMYTWI8DOMRIocer()
        {
            timer1 = new System.Timers.Timer(3000)
            {
                AutoReset = true
            };
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Elapsed);

            timer2 = new System.Timers.Timer(1000)
            {
                AutoReset = true,
            };
            timer2.Elapsed += new System.Timers.ElapsedEventHandler(timer2_Elapsed);
        }

        public delegate void ReceivedEventHandler(int[] receiveValue);
        public event ReceivedEventHandler OnReceived;

        public delegate void StatusChangeHandler(bool status);
        public event StatusChangeHandler OnStatusChange;

        /// <summary>
        /// 设置连接状态
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(bool status)
        {
            if (this.Status != status && this.OnStatusChange != null) this.OnStatusChange(status);
            this.Status = status;
        }

        /// <summary>
        /// 接收数据次数
        /// </summary>
        private int IOStateCount = 0;

        /// <summary>
        /// 接收到的数据
        /// </summary>
        public int[] ReceiveValue = new int[20];

        private System.Timers.Timer timer1;
        private System.Timers.Timer timer2;

        private UdpClient udpClient;

        public UdpClient UdpClient
        {
            get { return udpClient; }
            set { udpClient = value; }
        }
        // 异步状态同步
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        //发送/接收状态
        UdpState udpReceiveState = null;
        UdpState udpSendState = null;

        Thread t = null;

        private bool status;

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 远程端点
        /// </summary>
        private IPEndPoint remotePoint = null;

        /// <summary>
        /// 打开UDP端口
        /// </summary>
        /// <param name="tcp"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public void OpenUDP(string ip, int port)
        {
            try
            {
                IPAddress remoteIP = IPAddress.Parse(ip);

                remotePoint = new IPEndPoint(remoteIP, port);//实例化一个远程端点 

                UdpClient = new UdpClient();

                UdpClient.Connect(remotePoint);

                SetStatus(true);

                timer1.Enabled = true;
                timer2.Enabled = true;
                // 分别实例化udpSendState、udpReceiveState
                udpReceiveState = new UdpState();
                udpReceiveState.udpClient = UdpClient;
                udpReceiveState.ipEndPoint = remotePoint;
                udpReceiveState.OnReceive = OnReceive;

                t = new Thread(new ThreadStart(ReceiveMsg));
                t.Start();
            }
            catch (Exception)
            {
                SetStatus(false);
                if (this.OnStatusChange != null) this.OnStatusChange(status);
            }
        }

        /// <summary>
        /// 关闭UDP端口
        /// </summary>
        /// <param name="tcp"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public void ClostUDP()
        {
            t.Abort();
            receiveDone.Reset();
            SetStatus(false);
            timer1.Enabled = false;
            timer2.Enabled = false;
            this.UdpClient.Close();
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public void ReceiveData()
        {
            if (this.status)
            {
                try
                {
                    Output("123456:I");

                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    Byte[] receiveBytes = this.UdpClient.Receive(ref RemoteIpEndPoint);
                    this.IOStateCount = receiveBytes.Length;
                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    returnData = returnData.Replace("I=", "");

                    string[] returnDataArr = new string[12];

                    for (var j = 0; j < returnData.Length; j++)
                    {
                        returnDataArr[j] = returnData.Substring(j, 1);
                    }

                    for (int i = 0; i < returnDataArr.Length; i++)
                    {
                        this.ReceiveValue[i] = int.Parse(returnDataArr[i]);
                    }

                    if (OnReceived != null) OnReceived(this.ReceiveValue);
                }
                catch { }
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="receiveData"></param>
        public void OnReceive(int[] receiveData)
        {
            this.IOStateCount++;
            this.ReceiveValue = receiveData;
            if (OnReceived != null) OnReceived(this.ReceiveValue);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public void ReceiveMsg()
        {
            while (true)
            {
                try
                {
                    receiveDone.Reset();
                    // 调用接收回调函数
                    IAsyncResult iar = this.UdpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpReceiveState);
                    receiveDone.WaitOne();
                    //Thread.Sleep(100);
                }
                catch
                {
                }
            }
        }

        // 接收回调函数
        private void ReceiveCallback(IAsyncResult iar)
        {
            try
            {
                UdpState udpReceiveState = iar.AsyncState as UdpState;
                if (iar.IsCompleted)
                {
                    Byte[] receiveBytes = udpReceiveState.udpClient.EndReceive(iar, ref udpReceiveState.ipEndPoint);
                    string receiveString = Encoding.ASCII.GetString(receiveBytes);

                    receiveString = receiveString.Replace("I=", "");

                    int[] returnDataArr = new int[12];
                    if (receiveString.Length == 12)
                    {
                        for (var j = 0; j < receiveString.Length; j++)
                        {
                            returnDataArr[j] = int.Parse(receiveString.Substring(j, 1));
                        }
                        if (udpReceiveState.OnReceive != null) udpReceiveState.OnReceive(returnDataArr);
                    }
                    receiveDone.Set();
                }
            }
            catch { }
        }

        /// <summary>
        /// 间隔事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer1.Stop();
            try
            {
                if (IOStateCount > 0)
                    SetStatus(true);
                else
                    SetStatus(false);

                IOStateCount = 0;
            }
            catch
            {
            }
            finally
            {
                timer1.Start();
            }
        }

        /// <summary>
        /// 发送取数指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer2.Stop();
            try
            {
                Output("123456:I");
            }
            catch
            {
            }
            finally
            {
                timer2.Start();
            }

            //ReceiveData();
        }

        /// <summary>
        /// 输入
        /// </summary>
        /// <param name="pnum">通道号</param>
        /// <param name="type"></param>
        public void Output(int pnum, bool type)
        {
            if (this.status)
            {
                string str = "";

                for (int i = 1; i <= 8; i++)
                {
                    if (i == pnum)
                    {
                        str += type == true ? "1" : "0";
                    }
                    else
                    {
                        str += "2";
                    }
                }

                string sendCmd = string.Format("123456:({0})", str);

                byte[] buffer = System.Text.Encoding.Default.GetBytes(sendCmd);

                this.UdpClient.Send(buffer, buffer.Length);//将数据发送到远程端点 
            }
        }

        /// <summary>
        /// 输入命令
        /// </summary>
        /// <param name="sendCmd"></param>
        public void Output(string sendCmd)
        {
            if (this.UdpClient != null)
            {
                byte[] buffer = System.Text.Encoding.Default.GetBytes(sendCmd);

                this.UdpClient.Send(buffer, buffer.Length);//将数据发送到远程端点 
            }
        }
    }

    // 定义 UdpState类
    public class UdpState
    {
        public UdpClient udpClient = null;
        public IPEndPoint ipEndPoint = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public int counter = 0;

        public Action<int[]> OnReceive;
    }
}
