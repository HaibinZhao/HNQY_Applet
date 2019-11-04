namespace CMCS.CommonADGS.Win
{
    partial class ConfigSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigSetting));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.txtPort = new System.Windows.Forms.NumericUpDown();
			this.txtGrabInterval = new System.Windows.Forms.NumericUpDown();
			this.ddlDbType = new System.Windows.Forms.ComboBox();
			this.ckVerifyBeforeClose = new System.Windows.Forms.CheckBox();
			this.ckIsSeccetRunning = new System.Windows.Forms.CheckBox();
			this.ckStartup = new System.Windows.Forms.CheckBox();
			this.txtIP = new System.Windows.Forms.TextBox();
			this.txtUpLoadIdentifier = new System.Windows.Forms.TextBox();
			this.txtAppIdentifier = new System.Windows.Forms.TextBox();
			this.txtSQL = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtConnStr = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.txtProcessName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtPort)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtGrabInterval)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBox1.Controls.Add(this.txtProcessName);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.btnSave);
			this.groupBox1.Controls.Add(this.txtPort);
			this.groupBox1.Controls.Add(this.txtGrabInterval);
			this.groupBox1.Controls.Add(this.ddlDbType);
			this.groupBox1.Controls.Add(this.ckVerifyBeforeClose);
			this.groupBox1.Controls.Add(this.ckIsSeccetRunning);
			this.groupBox1.Controls.Add(this.ckStartup);
			this.groupBox1.Controls.Add(this.txtIP);
			this.groupBox1.Controls.Add(this.txtUpLoadIdentifier);
			this.groupBox1.Controls.Add(this.txtAppIdentifier);
			this.groupBox1.Controls.Add(this.txtSQL);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.txtConnStr);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Location = new System.Drawing.Point(15, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(915, 477);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "基础配置";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(743, 416);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(119, 32);
			this.btnSave.TabIndex = 33;
			this.btnSave.Text = "保    存";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnBase_Click);
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(177, 122);
			this.txtPort.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(120, 21);
			this.txtPort.TabIndex = 29;
			this.txtPort.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
			// 
			// txtGrabInterval
			// 
			this.txtGrabInterval.Location = new System.Drawing.Point(177, 386);
			this.txtGrabInterval.Name = "txtGrabInterval";
			this.txtGrabInterval.Size = new System.Drawing.Size(120, 21);
			this.txtGrabInterval.TabIndex = 29;
			this.txtGrabInterval.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// ddlDbType
			// 
			this.ddlDbType.FormattingEnabled = true;
			this.ddlDbType.Items.AddRange(new object[] {
            "Access",
            "SqlServer",
            "SQLite"});
			this.ddlDbType.Location = new System.Drawing.Point(177, 155);
			this.ddlDbType.Name = "ddlDbType";
			this.ddlDbType.Size = new System.Drawing.Size(225, 20);
			this.ddlDbType.TabIndex = 28;
			// 
			// ckVerifyBeforeClose
			// 
			this.ckVerifyBeforeClose.AutoSize = true;
			this.ckVerifyBeforeClose.Checked = true;
			this.ckVerifyBeforeClose.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckVerifyBeforeClose.Location = new System.Drawing.Point(351, 421);
			this.ckVerifyBeforeClose.Name = "ckVerifyBeforeClose";
			this.ckVerifyBeforeClose.Size = new System.Drawing.Size(72, 16);
			this.ckVerifyBeforeClose.TabIndex = 1;
			this.ckVerifyBeforeClose.Text = "关闭验证";
			this.ckVerifyBeforeClose.UseVisualStyleBackColor = true;
			// 
			// ckIsSeccetRunning
			// 
			this.ckIsSeccetRunning.AutoSize = true;
			this.ckIsSeccetRunning.Checked = true;
			this.ckIsSeccetRunning.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckIsSeccetRunning.Location = new System.Drawing.Point(262, 421);
			this.ckIsSeccetRunning.Name = "ckIsSeccetRunning";
			this.ckIsSeccetRunning.Size = new System.Drawing.Size(84, 16);
			this.ckIsSeccetRunning.TabIndex = 1;
			this.ckIsSeccetRunning.Text = "最小化运行";
			this.ckIsSeccetRunning.UseVisualStyleBackColor = true;
			// 
			// ckStartup
			// 
			this.ckStartup.AutoSize = true;
			this.ckStartup.Checked = true;
			this.ckStartup.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckStartup.Location = new System.Drawing.Point(177, 421);
			this.ckStartup.Name = "ckStartup";
			this.ckStartup.Size = new System.Drawing.Size(72, 16);
			this.ckStartup.TabIndex = 1;
			this.ckStartup.Text = "开机启动";
			this.ckStartup.UseVisualStyleBackColor = true;
			// 
			// txtIP
			// 
			this.txtIP.Location = new System.Drawing.Point(177, 89);
			this.txtIP.Name = "txtIP";
			this.txtIP.Size = new System.Drawing.Size(225, 21);
			this.txtIP.TabIndex = 5;
			// 
			// txtUpLoadIdentifier
			// 
			this.txtUpLoadIdentifier.Location = new System.Drawing.Point(177, 60);
			this.txtUpLoadIdentifier.Name = "txtUpLoadIdentifier";
			this.txtUpLoadIdentifier.Size = new System.Drawing.Size(691, 21);
			this.txtUpLoadIdentifier.TabIndex = 5;
			// 
			// txtAppIdentifier
			// 
			this.txtAppIdentifier.Location = new System.Drawing.Point(177, 29);
			this.txtAppIdentifier.Name = "txtAppIdentifier";
			this.txtAppIdentifier.Size = new System.Drawing.Size(691, 21);
			this.txtAppIdentifier.TabIndex = 5;
			// 
			// txtSQL
			// 
			this.txtSQL.Location = new System.Drawing.Point(177, 244);
			this.txtSQL.Multiline = true;
			this.txtSQL.Name = "txtSQL";
			this.txtSQL.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtSQL.Size = new System.Drawing.Size(685, 108);
			this.txtSQL.TabIndex = 22;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(111, 92);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(53, 12);
			this.label6.TabIndex = 0;
			this.label6.Text = "服务器IP";
			// 
			// txtConnStr
			// 
			this.txtConnStr.Location = new System.Drawing.Point(177, 186);
			this.txtConnStr.Multiline = true;
			this.txtConnStr.Name = "txtConnStr";
			this.txtConnStr.Size = new System.Drawing.Size(685, 50);
			this.txtConnStr.TabIndex = 22;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(99, 124);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(65, 12);
			this.label7.TabIndex = 2;
			this.label7.Text = "服务器端口";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(111, 63);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "上传标识";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(45, 390);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(119, 12);
			this.label3.TabIndex = 2;
			this.label3.Text = "提取间隔 单位：分钟";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(85, 247);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(77, 12);
			this.label5.TabIndex = 14;
			this.label5.Text = "数据查询语句";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(87, 33);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "程序唯一标识";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(37, 191);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(125, 12);
			this.label10.TabIndex = 14;
			this.label10.Text = "设备数据库连接字符串";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(51, 159);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(113, 12);
			this.label11.TabIndex = 15;
			this.label11.Text = "化验设备数据库类型";
			// 
			// txtProcessName
			// 
			this.txtProcessName.Location = new System.Drawing.Point(177, 359);
			this.txtProcessName.Name = "txtProcessName";
			this.txtProcessName.Size = new System.Drawing.Size(225, 21);
			this.txtProcessName.TabIndex = 35;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(109, 364);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 12);
			this.label4.TabIndex = 34;
			this.label4.Text = "进程名称";
			// 
			// ConfigSetting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(949, 501);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ConfigSetting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "煤质化验设备数据提取工具-配置程序";
			this.Load += new System.EventHandler(this.ConfigSetting_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtPort)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtGrabInterval)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckStartup;
        private System.Windows.Forms.TextBox txtAppIdentifier;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox ddlDbType;
        private System.Windows.Forms.TextBox txtConnStr;
        private System.Windows.Forms.NumericUpDown txtGrabInterval;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox ckVerifyBeforeClose;
        private System.Windows.Forms.CheckBox ckIsSeccetRunning;
        private System.Windows.Forms.TextBox txtUpLoadIdentifier;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown txtPort;
        private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtProcessName;
		private System.Windows.Forms.Label label4;
	}
}