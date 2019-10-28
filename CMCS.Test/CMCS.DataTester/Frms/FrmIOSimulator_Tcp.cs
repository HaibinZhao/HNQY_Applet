using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMCS.DataTester.DAO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace CMCS.DataTester.Frms
{
    public partial class FrmIOSimulator_Tcp : Form
    {
        private UdpClient udpClient;
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        UdpState udpReceiveState = null;
        Thread t = null;

        DataTesterDAO dataTesterDAO = DataTesterDAO.GetInstance();

        public FrmIOSimulator_Tcp()
        {
            InitializeComponent();
        }

        private void FrmIOSimulator_Load(object sender, EventArgs e)
        {

        }

        private void FrmIOSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnColseCom_Click(null, null);
        }

        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            try
            {
                udpClient = new UdpClient(int.Parse(txt_Port.Text));

                timer1.Start();

                btnOpenCom.Enabled = false;
                btnCloseCom.Enabled = true;

                udpReceiveState = new UdpState();
                udpReceiveState.udpClient = udpClient;
                udpReceiveState.OnReceive = Receive;
                //udpReceiveState.ipEndPoint = remotePoint;

                t = new Thread(new ThreadStart(ReceiveMsg));
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnColseCom_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();

                udpClient.Close();
                t.Abort();
                btnOpenCom.Enabled = true;
                btnCloseCom.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Invoke封装
        /// </summary>
        /// <param name="action"></param>
        public void InvokeEx(Action action)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            this.Invoke(action);
        }

        /// <summary>
        /// 临时数据
        /// </summary>
        private List<byte> ReceiveList = new List<byte>();

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (udpClient == null) return;

            byte[] datas = new byte[12];
            int i = 0;
            foreach (Button btn in flpanInput.Controls.OfType<Button>())
            {
                datas[i] = ((btn.Tag != null && btn.Tag.ToString() == "1") ? (byte)0x01 : (byte)0x00);
                i++;
            }

            this.udpClient.Send(datas, datas.Length);//将数据发送到远程端点 
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Tag == null || btn.Tag.ToString() == "1")
            {
                btn.Tag = "0";
                btn.BackColor = Color.Red;
            }
            else
            {
                btn.Tag = "1";
                btn.BackColor = Color.Green;
            }
        }

        private void Receive(int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Button btn = flpanOutput.Controls.OfType<Button>().FirstOrDefault(a => a.Tag.ToString() == (i + 1).ToString());
                if (btn != null) btn.BackColor = (data[i] == 1) ? Color.Green : Color.Red;
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public void ReceiveMsg()
        {
            while (true)
            {
                receiveDone.Reset();
                // 调用接收回调函数
                IAsyncResult iar = this.udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpReceiveState);
                receiveDone.WaitOne();
                //Thread.Sleep(100);
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

                    receiveString = receiveString.Replace("123456:(", "").Replace(")", "");

                    int[] returnDataArr = new int[8];
                    if (receiveString.Length == 8)
                    {
                        for (var j = 0; j < receiveString.Length; j++)
                        {
                            returnDataArr[j] = int.Parse(receiveString.Substring(j, 1));
                        }
                    }
                    udpReceiveState.OnReceive(returnDataArr);

                    receiveDone.Set();
                }
            }
            catch { }
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
