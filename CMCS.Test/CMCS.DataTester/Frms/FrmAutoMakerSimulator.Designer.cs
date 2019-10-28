namespace CMCS.DataTester.Frms
{
    partial class FrmAutoMakerSimulator
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
            this.rbtnMachineCode2 = new System.Windows.Forms.RadioButton();
            this.rbtnMachineCode1 = new System.Windows.Forms.RadioButton();
            this.btnReset = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSystemStatus_FSGZ = new System.Windows.Forms.Button();
            this.btnSystemStatus_JXDJ = new System.Windows.Forms.Button();
            this.btnSystemStatus_ZZYX = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtxtOutput = new System.Windows.Forms.RichTextBox();
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
            this.groupBox2.Controls.Add(this.rbtnMachineCode2);
            this.groupBox2.Controls.Add(this.rbtnMachineCode1);
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
            // rbtnMachineCode2
            // 
            this.rbtnMachineCode2.AutoSize = true;
            this.rbtnMachineCode2.Location = new System.Drawing.Point(139, 66);
            this.rbtnMachineCode2.Name = "rbtnMachineCode2";
            this.rbtnMachineCode2.Size = new System.Drawing.Size(107, 16);
            this.rbtnMachineCode2.TabIndex = 11;
            this.rbtnMachineCode2.Text = "#2全自动制样机";
            this.rbtnMachineCode2.UseVisualStyleBackColor = true;
            // 
            // rbtnMachineCode1
            // 
            this.rbtnMachineCode1.AutoSize = true;
            this.rbtnMachineCode1.Checked = true;
            this.rbtnMachineCode1.Location = new System.Drawing.Point(139, 32);
            this.rbtnMachineCode1.Name = "rbtnMachineCode1";
            this.rbtnMachineCode1.Size = new System.Drawing.Size(107, 16);
            this.rbtnMachineCode1.TabIndex = 10;
            this.rbtnMachineCode1.TabStop = true;
            this.rbtnMachineCode1.Text = "#1全自动制样机";
            this.rbtnMachineCode1.UseVisualStyleBackColor = true;
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSystemStatus_FSGZ);
            this.groupBox3.Controls.Add(this.btnSystemStatus_JXDJ);
            this.groupBox3.Controls.Add(this.btnSystemStatus_ZZYX);
            this.groupBox3.Location = new System.Drawing.Point(411, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(268, 53);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " 设置系统状态 ";
            // 
            // btnSystemStatus_FSGZ
            // 
            this.btnSystemStatus_FSGZ.Location = new System.Drawing.Point(179, 20);
            this.btnSystemStatus_FSGZ.Name = "btnSystemStatus_FSGZ";
            this.btnSystemStatus_FSGZ.Size = new System.Drawing.Size(78, 23);
            this.btnSystemStatus_FSGZ.TabIndex = 7;
            this.btnSystemStatus_FSGZ.Text = "发生故障";
            this.btnSystemStatus_FSGZ.UseVisualStyleBackColor = true;
            this.btnSystemStatus_FSGZ.Click += new System.EventHandler(this.btnSystemStatus_FSGZ_Click);
            // 
            // btnSystemStatus_JXDJ
            // 
            this.btnSystemStatus_JXDJ.Location = new System.Drawing.Point(17, 20);
            this.btnSystemStatus_JXDJ.Name = "btnSystemStatus_JXDJ";
            this.btnSystemStatus_JXDJ.Size = new System.Drawing.Size(78, 23);
            this.btnSystemStatus_JXDJ.TabIndex = 4;
            this.btnSystemStatus_JXDJ.Text = "就绪待机";
            this.btnSystemStatus_JXDJ.UseVisualStyleBackColor = true;
            this.btnSystemStatus_JXDJ.Click += new System.EventHandler(this.btnSystemStatus_JXDJ_Click);
            // 
            // btnSystemStatus_ZZYX
            // 
            this.btnSystemStatus_ZZYX.Location = new System.Drawing.Point(98, 20);
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
            // FrmAutoMakerSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 417);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "FrmAutoMakerSimulator";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "全自动制样机模拟";
            this.Load += new System.EventHandler(this.FrmBeltSamplerSimulator_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtxtOutput;
        private System.Windows.Forms.Button btnSystemStatus_ZZYX;
        private System.Windows.Forms.Button btnSystemStatus_JXDJ;
        private System.Windows.Forms.Button btnSystemStatus_FSGZ;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.RadioButton rbtnMachineCode2;
        private System.Windows.Forms.RadioButton rbtnMachineCode1;
    }
}