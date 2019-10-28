using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using System.IO.Ports;
using CMCS.Common.DAO;
using CMCS.Common;
using CMCS.DapperDber.Dbs.OracleDb;
using CMCS.Common.Utilities;
using CMCS.CarTransport.Queue.Core;

namespace CMCS.CarTransport.Queue.Frms
{
    public partial class FrmSetting : DevComponents.DotNetBar.Metro.MetroForm
    {
        CommonDAO commonDAO = CommonDAO.GetInstance();

        CommonAppConfig commonAppConfig = CommonAppConfig.GetInstance();

        public FrmSetting()
        {
            InitializeComponent();
        }

        void InitForm()
        {
            InitComPortComboBoxs(cmbRwer2Com);
            InitNumberAscComboBoxs(1, 15, cmbInductorCoil1Port, cmbInductorCoil2Port, cmbInductorCoil3Port, cmbInductorCoil4Port, cmbGate1UpPort, cmbGate1DownPort, cmbGate2UpPort, cmbGate2DownPort, cmbSignalLight1Port, cmbSignalLight2Port);
            InitNumberAscComboBox(1, 30, cmbRwerPower);
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {

        }

        private void FrmSetting_Shown(object sender, EventArgs e)
        {
            InitForm();

            LoadAppConfig();
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <returns></returns>
        private bool TestDBConnect()
        {
            if (string.IsNullOrEmpty(txtSelfConnStr.Text.Trim()))
            {
                MessageBoxEx.Show("请先输入数据库连接字符串", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            try
            {
                OracleDapperDber dber = new OracleDapperDber(txtSelfConnStr.Text.Trim());
                using (System.Data.OracleClient.OracleConnection conn = dber.CreateConnection() as System.Data.OracleClient.OracleConnection)
                {
                    conn.Open();
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("数据库连接失败，" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        void LoadAppConfig()
        {
            txtAppIdentifier.Text = commonAppConfig.AppIdentifier;
            txtSelfConnStr.Text = commonAppConfig.SelfConnStr;
            chbStartup.Checked = (commonDAO.GetAppletConfigString("开机启动") == "1");
            dbtxtProfinWeight.Value = commonDAO.GetAppletConfigDouble("盈异常吨数");
            dbtxtProfinWeight2.Value = commonDAO.GetAppletConfigDouble("亏异常吨数");
            // IO控制器
            iptxtIOIP.Value = commonDAO.GetAppletConfigString("IO控制器_IP地址");
            txtIOPort.Text = commonDAO.GetAppletConfigString("IO控制器_端口");
            SelectedComboBoxItem(cmbInductorCoil1Port, commonDAO.GetAppletConfigString("IO控制器_地感1端口"));
            SelectedComboBoxItem(cmbInductorCoil2Port, commonDAO.GetAppletConfigString("IO控制器_地感2端口"));
            SelectedComboBoxItem(cmbInductorCoil3Port, commonDAO.GetAppletConfigString("IO控制器_地感3端口"));
            SelectedComboBoxItem(cmbInductorCoil4Port, commonDAO.GetAppletConfigString("IO控制器_地感4端口"));
            SelectedComboBoxItem(cmbGate1UpPort, commonDAO.GetAppletConfigString("IO控制器_道闸1升杆端口"));
            SelectedComboBoxItem(cmbGate1DownPort, commonDAO.GetAppletConfigString("IO控制器_道闸1降杆端口"));
            SelectedComboBoxItem(cmbGate2UpPort, commonDAO.GetAppletConfigString("IO控制器_道闸2升杆端口"));
            SelectedComboBoxItem(cmbGate2DownPort, commonDAO.GetAppletConfigString("IO控制器_道闸2降杆端口"));
            SelectedComboBoxItem(cmbSignalLight1Port, commonDAO.GetAppletConfigString("IO控制器_信号灯1端口"));
            SelectedComboBoxItem(cmbSignalLight2Port, commonDAO.GetAppletConfigString("IO控制器_信号灯2端口"));

            // LED显示屏
            iptxtLED1IP.Value = commonDAO.GetAppletConfigString("LED显示屏1_IP地址");

            // 读卡器网口
            ipAddressInput1.Value = commonDAO.GetAppletConfigString("读卡器1IP地址");
            ipAddressPort1.Text = commonDAO.GetAppletConfigString("读卡器1端口");
            txtRwerTagStartWith.Text = commonDAO.GetAppletConfigString("读卡器_标签过滤");
            SelectedComboBoxItem(cmbRwerPower, commonDAO.GetAppletConfigString("读卡器1功率"));

            // 发卡器
            SelectedComboBoxItem(cmbRwer2Com, commonDAO.GetAppletConfigString("发卡器_串口"));

        }

        /// <summary>
        /// 保存配置
        /// </summary>
        bool SaveAppConfig()
        {
            if (!TestDBConnect()) return false;

            commonAppConfig.AppIdentifier = txtAppIdentifier.Text.Trim();
            commonAppConfig.SelfConnStr = txtSelfConnStr.Text;
            commonAppConfig.Save();
            commonDAO.SetAppletConfig("开机启动", Convert.ToInt16(chbStartup.Checked).ToString());
            commonDAO.SetAppletConfig("盈异常吨数", dbtxtProfinWeight.Value.ToString());
            commonDAO.SetAppletConfig("亏异常吨数", dbtxtProfinWeight2.Value.ToString());
            try
            {
#if DEBUG

#else
                // 添加、取消开机启动
                if (chbStartup.Checked)
                    StartUpUtil.InsertStartUp(Application.ProductName, Application.ExecutablePath);
                else
                    StartUpUtil.DeleteStartUp(Application.ProductName);
#endif
            }
            catch { }

            // IO控制器
            commonDAO.SetAppletConfig("IO控制器_IP地址", iptxtIOIP.Value);
            commonDAO.SetAppletConfig("IO控制器_端口", txtIOPort.Text);
            commonDAO.SetAppletConfig("IO控制器_地感1端口", (cmbInductorCoil1Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_地感2端口", (cmbInductorCoil2Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_地感3端口", (cmbInductorCoil3Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_地感4端口", (cmbInductorCoil4Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_道闸1升杆端口", (cmbGate1UpPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_道闸1降杆端口", (cmbGate1DownPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_道闸2升杆端口", (cmbGate2UpPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_道闸2降杆端口", (cmbGate2DownPort.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_信号灯1端口", (cmbSignalLight1Port.SelectedItem as DataItem).Value);
            commonDAO.SetAppletConfig("IO控制器_信号灯2端口", (cmbSignalLight2Port.SelectedItem as DataItem).Value);

            // LED显示屏
            commonDAO.SetAppletConfig("LED显示屏1_IP地址", iptxtLED1IP.Value);

            // 读卡器网口
            commonDAO.SetAppletConfig("读卡器1IP地址", ipAddressInput1.Value);
            commonDAO.SetAppletConfig("读卡器1端口", ipAddressPort1.Text);
            commonDAO.SetAppletConfig("读卡器_标签过滤", txtRwerTagStartWith.Text);
            commonDAO.SetAppletConfig("读卡器1功率", (cmbRwerPower.SelectedItem as DataItem).Value);

            // 发卡器
            commonDAO.SetAppletConfig("发卡器_串口", (cmbRwer2Com.SelectedItem as DataItem).Value);


            return true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!ValidateInputEmpty(new List<string> { "程序唯一标识符", "数据库连接字符串" }, new List<Control> { txtAppIdentifier, txtSelfConnStr })) return;

            if (!SaveAppConfig()) return;

            if (MessageBoxEx.Show("更改的配置需要重启程序才能生效，是否立刻重启？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Restart();
            else
                this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 其他函数

        /// <summary>
        /// 验证批量控件为空，并提示
        /// </summary>
        /// <param name="tipsNames"></param>
        /// <param name="controls"></param>
        /// <returns></returns>
        public static bool ValidateInputEmpty(List<string> tipsNames, List<Control> controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                Control control = controls[i];

                if (control is TextBoxX && string.IsNullOrEmpty(((TextBoxX)control).Text))
                {
                    control.Focus();
                    MessageBoxEx.Show("请输入" + tipsNames[i] + "！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 选中下拉框选项
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="text"></param>
        private void SelectedComboBoxItem(ComboBoxEx cmb, string value)
        {
            foreach (DataItem dataItem in cmb.Items)
            {
                if (dataItem.Value == value) cmb.SelectedItem = dataItem;
            }
        }

        /// <summary>
        /// 初始化串口下拉框
        /// </summary>
        /// <param name="cmb"></param>
        void InitComPortComboBox(ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            for (int i = 1; i < 20; i++)
            {
                cmb.Items.Add(new DataItem("COM" + i.ToString(), i.ToString()));
            }

            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化串口下拉框
        /// </summary>
        /// <param name="cmbs"></param>
        void InitComPortComboBoxs(params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitComPortComboBox(cmb);
            }
        }

        /// <summary>
        /// 初始化波特率下拉框
        /// </summary>
        /// <param name="cmb"></param>
        private void InitBandrateComboBox(ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            cmb.Items.Add(new DataItem("1200"));
            cmb.Items.Add(new DataItem("4800"));
            cmb.Items.Add(new DataItem("9600"));
            cmb.Items.Add(new DataItem("14400"));
            cmb.Items.Add(new DataItem("19200"));
            cmb.Items.Add(new DataItem("38400"));
            cmb.Items.Add(new DataItem("56000"));
            cmb.Items.Add(new DataItem("57600"));
            cmb.Items.Add(new DataItem("115200"));

            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化波特率下拉框
        /// </summary>
        /// <param name="cmbs"></param>
        void InitBandrateComboBoxs(params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitBandrateComboBox(cmb);
            }
        }

        /// <summary>
        /// 初始化数字下拉框
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void InitNumberAscComboBox(int start, int end, ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            for (int i = start; i <= end; i++)
            {
                cmb.Items.Add(new DataItem(i.ToString()));
            }

            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化数字下拉框
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void InitNumberAscComboBoxs(int start, int end, params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitNumberAscComboBox(start, end, cmb);
            }
        }

        /// <summary>
        /// 初始化停止位下拉框
        /// </summary>
        /// <param name="cmb"></param>
        void InitStopBitsComboBox(ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            cmb.Items.Add(new DataItem(StopBits.None.ToString(), ((int)StopBits.None).ToString()));
            cmb.Items.Add(new DataItem(StopBits.One.ToString(), ((int)StopBits.One).ToString()));
            cmb.Items.Add(new DataItem(StopBits.OnePointFive.ToString(), ((int)StopBits.OnePointFive).ToString()));
            cmb.Items.Add(new DataItem(StopBits.Two.ToString(), ((int)StopBits.Two).ToString()));

            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化停止位下拉框
        /// </summary>
        /// <param name="cmbs"></param>
        void InitStopBitsComboBoxs(params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitStopBitsComboBox(cmb);
            }
        }

        /// <summary>
        /// 初始化校验位下拉框
        /// </summary>
        /// <param name="cmb"></param>
        void InitParityComboBox(ComboBoxEx cmb)
        {
            cmb.Items.Clear();

            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            cmb.Items.Add(new DataItem(Parity.None.ToString(), ((int)Parity.None).ToString()));
            cmb.Items.Add(new DataItem(Parity.Odd.ToString(), ((int)Parity.Odd).ToString()));
            cmb.Items.Add(new DataItem(Parity.Even.ToString(), ((int)Parity.Even).ToString()));
            cmb.Items.Add(new DataItem(Parity.Mark.ToString(), ((int)Parity.Mark).ToString()));
            cmb.Items.Add(new DataItem(Parity.Space.ToString(), ((int)Parity.Space).ToString()));

            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化校验位下拉框
        /// </summary>
        /// <param name="cmbs"></param>
        void InitParityComboBoxs(params ComboBoxEx[] cmbs)
        {
            foreach (ComboBoxEx cmb in cmbs)
            {
                InitParityComboBox(cmb);
            }
        }

        #endregion
    }
}