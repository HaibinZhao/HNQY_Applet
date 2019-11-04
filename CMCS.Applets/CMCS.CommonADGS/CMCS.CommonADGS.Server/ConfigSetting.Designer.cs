namespace CMCS.CommonADGS.Server
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigSetting));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.Delete = new System.Windows.Forms.DataGridViewLinkColumn();
			this.UpLoadIdentifier = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DataTableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PrimarKeys = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.txtPort = new System.Windows.Forms.NumericUpDown();
			this.ckVerifyBeforeClose = new System.Windows.Forms.CheckBox();
			this.ckIsSeccetRunning = new System.Windows.Forms.CheckBox();
			this.ckStartup = new System.Windows.Forms.CheckBox();
			this.txtSelfConnStr = new System.Windows.Forms.TextBox();
			this.txtOracleKeywords = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtAppIdentifier = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPort)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBox1.Controls.Add(this.dataGridView1);
			this.groupBox1.Controls.Add(this.btnAdd);
			this.groupBox1.Controls.Add(this.btnSave);
			this.groupBox1.Controls.Add(this.txtPort);
			this.groupBox1.Controls.Add(this.ckVerifyBeforeClose);
			this.groupBox1.Controls.Add(this.ckIsSeccetRunning);
			this.groupBox1.Controls.Add(this.ckStartup);
			this.groupBox1.Controls.Add(this.txtSelfConnStr);
			this.groupBox1.Controls.Add(this.txtOracleKeywords);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.txtAppIdentifier);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1059, 608);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "基础配置";
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Delete,
            this.UpLoadIdentifier,
            this.DataTableName,
            this.PrimarKeys});
			this.dataGridView1.Location = new System.Drawing.Point(88, 235);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 23;
			this.dataGridView1.Size = new System.Drawing.Size(881, 292);
			this.dataGridView1.TabIndex = 34;
			this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
			// 
			// Delete
			// 
			dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Delete.DefaultCellStyle = dataGridViewCellStyle1;
			this.Delete.HeaderText = "操作";
			this.Delete.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.Delete.Name = "Delete";
			this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.Delete.Text = "删除";
			this.Delete.UseColumnTextForLinkValue = true;
			// 
			// UpLoadIdentifier
			// 
			this.UpLoadIdentifier.DataPropertyName = "UpLoadIdentifer";
			this.UpLoadIdentifier.HeaderText = "上传标识";
			this.UpLoadIdentifier.Name = "UpLoadIdentifier";
			this.UpLoadIdentifier.Width = 150;
			// 
			// DataTableName
			// 
			this.DataTableName.DataPropertyName = "DataTableName";
			this.DataTableName.HeaderText = "表名称";
			this.DataTableName.Name = "DataTableName";
			this.DataTableName.Width = 150;
			// 
			// PrimarKeys
			// 
			this.PrimarKeys.DataPropertyName = "PrimarKeys";
			this.PrimarKeys.HeaderText = "主键(多个以\"|\"分开)";
			this.PrimarKeys.Name = "PrimarKeys";
			this.PrimarKeys.Width = 150;
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(88, 206);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(79, 23);
			this.btnAdd.TabIndex = 33;
			this.btnAdd.Text = "增  加";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(850, 548);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(119, 32);
			this.btnSave.TabIndex = 33;
			this.btnSave.Text = "保    存";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(278, 142);
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
			// ckVerifyBeforeClose
			// 
			this.ckVerifyBeforeClose.AutoSize = true;
			this.ckVerifyBeforeClose.Checked = true;
			this.ckVerifyBeforeClose.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckVerifyBeforeClose.Location = new System.Drawing.Point(452, 179);
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
			this.ckIsSeccetRunning.Location = new System.Drawing.Point(363, 179);
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
			this.ckStartup.Location = new System.Drawing.Point(278, 179);
			this.ckStartup.Name = "ckStartup";
			this.ckStartup.Size = new System.Drawing.Size(72, 16);
			this.ckStartup.TabIndex = 1;
			this.ckStartup.Text = "开机启动";
			this.ckStartup.UseVisualStyleBackColor = true;
			// 
			// txtSelfConnStr
			// 
			this.txtSelfConnStr.Location = new System.Drawing.Point(278, 84);
			this.txtSelfConnStr.Multiline = true;
			this.txtSelfConnStr.Name = "txtSelfConnStr";
			this.txtSelfConnStr.Size = new System.Drawing.Size(691, 52);
			this.txtSelfConnStr.TabIndex = 5;
			// 
			// txtOracleKeywords
			// 
			this.txtOracleKeywords.Location = new System.Drawing.Point(278, 55);
			this.txtOracleKeywords.Name = "txtOracleKeywords";
			this.txtOracleKeywords.Size = new System.Drawing.Size(691, 21);
			this.txtOracleKeywords.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(164, 93);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(101, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "数据库连接字符串";
			// 
			// txtAppIdentifier
			// 
			this.txtAppIdentifier.Location = new System.Drawing.Point(278, 27);
			this.txtAppIdentifier.Name = "txtAppIdentifier";
			this.txtAppIdentifier.Size = new System.Drawing.Size(691, 21);
			this.txtAppIdentifier.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(86, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(179, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "Oracle关键字(多个以“|”分开)";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(188, 146);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(77, 12);
			this.label7.TabIndex = 2;
			this.label7.Text = "数据接收端口";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(188, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "程序唯一标识";
			// 
			// ConfigSetting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1090, 649);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ConfigSetting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "煤质化验设备数据接收工具-配置程序";
			this.Load += new System.EventHandler(this.ConfigSetting_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPort)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckStartup;
        private System.Windows.Forms.TextBox txtAppIdentifier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox ckVerifyBeforeClose;
        private System.Windows.Forms.CheckBox ckIsSeccetRunning;
        private System.Windows.Forms.TextBox txtOracleKeywords;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txtPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtSelfConnStr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridViewLinkColumn Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpLoadIdentifier;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataTableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrimarKeys;
    }
}