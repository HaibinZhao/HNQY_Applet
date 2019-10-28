using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BalanceDataGraber.Core;

namespace BalanceDataGraber
{
    public partial class Form1 : Form
    {
        Graber_BS224S _Graber_BS224S_A = new Graber_BS224S();
        Graber_BS224S _Graber_BS224S_B = new Graber_BS224S();

        Graber_TE6100L _Graber_TE6100L = new Graber_TE6100L();

        Graber_Std _Graber_Std = new Graber_Std();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "版本：" + new AU.Updater().Version;

            cmbCOM1.SelectedIndex = Config.GetInstance().ComIndex1;
            //cmbCOM2.SelectedIndex = Config.GetInstance().ComIndex2;
            //cmbCOM3.SelectedIndex = Config.GetInstance().ComIndex3;
            cmbCOM4.SelectedIndex = Config.GetInstance().ComIndex4;

            //this._Graber_BS224S_A.OnOutputInvoke += new Graber_BS224S.OutputInvokeEventHandler(_Graber_BS224S_A_OnOutputInvoke);
            this._Graber_BS224S_B.OnOutputInvoke += new Graber_BS224S.OutputInvokeEventHandler(_Graber_BS224S_B_OnOutputInvoke);
            //this._Graber_TE6100L.OnOutputInvoke += new Graber_TE6100L.OutputInvokeEventHandler(_Graber_TE6100L_OnOutputInvoke);
            this._Graber_Std.OnOutputInvoke += new Graber_Std.OutputInvokeEventHandler(_Graber_Std_OnOutputInvoke);

            btnOpen1_Click(null, null);
            //btnOpen2_Click(null, null);
            //btnOpen3_Click(null, null);
            btnOpen4_Click(null, null);

            this.WindowState = FormWindowState.Minimized;//使当前窗体最小化
            notifyIcon1.Visible = true;//使最下滑的图标可见
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._Graber_BS224S_A.IsOpen) this._Graber_BS224S_A.Close();
            if (this._Graber_BS224S_B.IsOpen) this._Graber_BS224S_B.Close();
            if (this._Graber_TE6100L.IsOpen) this._Graber_TE6100L.Close();
        }

        //void _Graber_BS224S_A_OnOutputInvoke(double value)
        //{
        //    this.Invoke((Action)(() =>
        //    {
        //        lblValue1.Text = value.ToString() + " g";
        //    }));

        //    //SendKeys.SendWait(value.ToString() + "\r");

        //    SendKeys.SendWait(" ");
        //    System.Threading.Thread.Sleep(20);
        //    SendKeys.SendWait(value.ToString());
        //    System.Threading.Thread.Sleep(20);
        //    SendKeys.SendWait("\r");
        //}

        void _Graber_BS224S_B_OnOutputInvoke(double value)
        {
            this.Invoke((Action)(() =>
            {
                lblValue2.Text = value.ToString() + " mg";
            }));


            SendKeys.SendWait(" ");
            System.Threading.Thread.Sleep(20);
            SendKeys.SendWait(value.ToString());
            System.Threading.Thread.Sleep(20);
            SendKeys.SendWait("\r");
        }

        //void _Graber_TE6100L_OnOutputInvoke(double value)
        //{
        //    this.Invoke((Action)(() =>
        //    {
        //        lblValue3.Text = value.ToString() + " g";
        //    }));

 

        //    SendKeys.SendWait(" ");
        //    System.Threading.Thread.Sleep(20);
        //    SendKeys.SendWait(value.ToString());
        //    System.Threading.Thread.Sleep(20);
        //    SendKeys.SendWait("\r");
        //}

        void _Graber_Std_OnOutputInvoke(double value)
        {
            this.Invoke((Action)(() =>
            {
                lblValue4.Text = value.ToString() + " g";
            }));

            //SendKeys.SendWait(value.ToString() + "\r");

            SendKeys.SendWait(" ");
            System.Threading.Thread.Sleep(20);
            SendKeys.SendWait(value.ToString());
            System.Threading.Thread.Sleep(20);
            SendKeys.SendWait("\r");
        }

        private void btnOpen1_Click(object sender, EventArgs e)
        {
            if (!_Graber_BS224S_B.IsOpen)
            {
                if (this._Graber_BS224S_B.Open(cmbCOM1.SelectedIndex + 1, 9600))
                {
                    btnOpen1.Text = "关 闭";
                    btnOpen1.BackColor = Color.Green;

                    Config.GetInstance().ComIndex1 = cmbCOM1.SelectedIndex;
                    Config.GetInstance().Save();
                }
            }
            else if (_Graber_BS224S_B.IsOpen)
            {
                this._Graber_BS224S_B.Close();
                btnOpen1.Text = "打 开";
                btnOpen1.BackColor = Color.Red;
            }
        }

        //private void btnOpen2_Click(object sender, EventArgs e)
        //{
        //    if (!_Graber_BS224S_A.IsOpen)
        //    {
        //        if (this._Graber_BS224S_A.Open(cmbCOM2.SelectedIndex + 1, 9600))
        //        {
        //            btnOpen2.Text = "关 闭";
        //            btnOpen2.BackColor = Color.Green;

        //            Config.GetInstance().ComIndex2 = cmbCOM2.SelectedIndex;
        //            Config.GetInstance().Save();
        //        }
        //    }
        //    else if (_Graber_BS224S_A.IsOpen)
        //    {
        //        this._Graber_BS224S_A.Close();
        //        btnOpen2.Text = "打 开";
        //        btnOpen2.BackColor = Color.Red;
        //    }
        //}

        //private void btnOpen3_Click(object sender, EventArgs e)
        //{
        //    if (!_Graber_TE6100L.IsOpen)
        //    {
        //        if (this._Graber_TE6100L.Open(cmbCOM3.SelectedIndex + 1, 1200))
        //        {
        //            btnOpen3.Text = "关 闭";
        //            btnOpen3.BackColor = Color.Green;

        //            Config.GetInstance().ComIndex3 = cmbCOM3.SelectedIndex;
        //            Config.GetInstance().Save();
        //        }
        //    }
        //    else if (_Graber_TE6100L.IsOpen)
        //    {
        //        this._Graber_TE6100L.Close();
        //        btnOpen3.Text = "打 开";
        //        btnOpen3.BackColor = Color.Red;
        //    }
        //}

        private void btnOpen4_Click(object sender, EventArgs e)
        {
            if (!_Graber_Std.IsOpen)
            {
                if (this._Graber_Std.Open(cmbCOM4.SelectedIndex + 1, 9600))
                {
                    btnOpen4.Text = "关 闭";
                    btnOpen4.BackColor = Color.Green;

                    Config.GetInstance().ComIndex4 = cmbCOM4.SelectedIndex;
                    Config.GetInstance().Save();
                }
            }
            else if (_Graber_Std.IsOpen)
            {
                this._Graber_Std.Close();
                btnOpen4.Text = "打 开";
                btnOpen4.BackColor = Color.Red;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) //判断当前窗体的状态是否为最小化
            {
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;//将当前窗体状态恢复为正常
                notifyIcon1.Visible = false;//将notifyIcon图标隐藏
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //如果当前状态的状态为最小化，则显示状态栏的程序托盘  
            if (this.WindowState == FormWindowState.Minimized)
            {
                //不在Window任务栏中显示  
                this.ShowInTaskbar = false;
                //使图标在状态栏中显示  
                this.notifyIcon1.Visible = true;
            }
        }

    }
}
