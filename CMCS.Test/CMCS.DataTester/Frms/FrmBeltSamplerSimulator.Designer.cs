namespace CMCS.DataTester.Frms
{
    partial class FrmBeltSamplerSimulator
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
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbBeltSampler = new System.Windows.Forms.ComboBox();
            this.btnSystemStatus_FSGZ = new System.Windows.Forms.Button();
            this.btnSystemStatus_JXDJ = new System.Windows.Forms.Button();
            this.btnSystemStatus_ZZXY = new System.Windows.Forms.Button();
            this.btnSystemStatus_ZZYX = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtxtOutput = new System.Windows.Forms.RichTextBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStart.Location = new System.Drawing.Point(25, 27);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(93, 27);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "开始模拟";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnReset);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.btnStart);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(5, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(690, 100);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " 操作 ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbBeltSampler);
            this.groupBox3.Controls.Add(this.btnSystemStatus_FSGZ);
            this.groupBox3.Controls.Add(this.btnSystemStatus_JXDJ);
            this.groupBox3.Controls.Add(this.btnSystemStatus_ZZXY);
            this.groupBox3.Controls.Add(this.btnSystemStatus_ZZYX);
            this.groupBox3.Location = new System.Drawing.Point(200, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(479, 53);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " 设置系统状态 ";
            // 
            // cmbBeltSampler
            // 
            this.cmbBeltSampler.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBeltSampler.FormattingEnabled = true;
            this.cmbBeltSampler.Location = new System.Drawing.Point(16, 23);
            this.cmbBeltSampler.Name = "cmbBeltSampler";
            this.cmbBeltSampler.Size = new System.Drawing.Size(126, 20);
            this.cmbBeltSampler.TabIndex = 8;
            // 
            // btnSystemStatus_FSGZ
            // 
            this.btnSystemStatus_FSGZ.Location = new System.Drawing.Point(391, 20);
            this.btnSystemStatus_FSGZ.Name = "btnSystemStatus_FSGZ";
            this.btnSystemStatus_FSGZ.Size = new System.Drawing.Size(78, 23);
            this.btnSystemStatus_FSGZ.TabIndex = 7;
            this.btnSystemStatus_FSGZ.Text = "发生故障";
            this.btnSystemStatus_FSGZ.UseVisualStyleBackColor = true;
            this.btnSystemStatus_FSGZ.Click += new System.EventHandler(this.btnSystemStatus_FSGZ_Click);
            // 
            // btnSystemStatus_JXDJ
            // 
            this.btnSystemStatus_JXDJ.Location = new System.Drawing.Point(148, 20);
            this.btnSystemStatus_JXDJ.Name = "btnSystemStatus_JXDJ";
            this.btnSystemStatus_JXDJ.Size = new System.Drawing.Size(78, 23);
            this.btnSystemStatus_JXDJ.TabIndex = 4;
            this.btnSystemStatus_JXDJ.Text = "就绪待机";
            this.btnSystemStatus_JXDJ.UseVisualStyleBackColor = true;
            this.btnSystemStatus_JXDJ.Click += new System.EventHandler(this.btnSystemStatus_JXDJ_Click);
            // 
            // btnSystemStatus_ZZXY
            // 
            this.btnSystemStatus_ZZXY.Location = new System.Drawing.Point(310, 20);
            this.btnSystemStatus_ZZXY.Name = "btnSystemStatus_ZZXY";
            this.btnSystemStatus_ZZXY.Size = new System.Drawing.Size(78, 23);
            this.btnSystemStatus_ZZXY.TabIndex = 6;
            this.btnSystemStatus_ZZXY.Text = "正在卸样";
            this.btnSystemStatus_ZZXY.UseVisualStyleBackColor = true;
            this.btnSystemStatus_ZZXY.Click += new System.EventHandler(this.btnSystemStatus_ZZXY_Click);
            // 
            // btnSystemStatus_ZZYX
            // 
            this.btnSystemStatus_ZZYX.Location = new System.Drawing.Point(229, 20);
            this.btnSystemStatus_ZZYX.Name = "btnSystemStatus_ZZYX";
            this.btnSystemStatus_ZZYX.Size = new System.Drawing.Size(78, 23);
            this.btnSystemStatus_ZZYX.TabIndex = 5;
            this.btnSystemStatus_ZZYX.Text = "正在运行";
            this.btnSystemStatus_ZZYX.UseVisualStyleBackColor = true;
            this.btnSystemStatus_ZZYX.Click += new System.EventHandler(this.btnSystemStatus_ZZYX_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtxtOutput);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(5, 105);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(690, 307);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " 输出 ";
            // 
            // rtxtOutput
            // 
            this.rtxtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtOutput.Location = new System.Drawing.Point(3, 17);
            this.rtxtOutput.Name = "rtxtOutput";
            this.rtxtOutput.Size = new System.Drawing.Size(684, 287);
            this.rtxtOutput.TabIndex = 2;
            this.rtxtOutput.Text = "";
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReset.Location = new System.Drawing.Point(25, 60);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(93, 27);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "重置数据";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // FrmBeltSamplerSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 417);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "FrmBeltSamplerSimulator";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "皮带采样机模拟";
            this.Load += new System.EventHandler(this.FrmBeltSamplerSimulator_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtxtOutput;
        private System.Windows.Forms.Button btnSystemStatus_ZZXY;
        private System.Windows.Forms.Button btnSystemStatus_ZZYX;
        private System.Windows.Forms.Button btnSystemStatus_JXDJ;
        private System.Windows.Forms.Button btnSystemStatus_FSGZ;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbBeltSampler;
        private System.Windows.Forms.Button btnReset;
    }
}