namespace CMCS.DumblyConcealer.Win.DumblyTasks
{
    partial class FrmAutoCupBoard_NCGM_Test
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
            this.components = new System.ComponentModel.Container();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSendPunmCmd = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbEndPlace_Punm = new System.Windows.Forms.ComboBox();
            this.cmbStartPlace_Punm = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSendgCYGCmd = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMakeCode = new System.Windows.Forms.TextBox();
            this.cmbCYG = new System.Windows.Forms.ComboBox();
            this.cmbCZPLX = new System.Windows.Forms.ComboBox();
            this.rtxtOutput = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_MakeCode_Pnum = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Metro;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(163)))), ((int)(((byte)(26))))));
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(690, 534);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(682, 508);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "气动传输";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txt_MakeCode_Pnum);
            this.groupBox2.Controls.Add(this.btnSendPunmCmd);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.cmbEndPlace_Punm);
            this.groupBox2.Controls.Add(this.cmbStartPlace_Punm);
            this.groupBox2.Location = new System.Drawing.Point(8, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(404, 203);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "气动命令";
            // 
            // btnSendPunmCmd
            // 
            this.btnSendPunmCmd.Location = new System.Drawing.Point(91, 108);
            this.btnSendPunmCmd.Name = "btnSendPunmCmd";
            this.btnSendPunmCmd.Size = new System.Drawing.Size(107, 23);
            this.btnSendPunmCmd.TabIndex = 15;
            this.btnSendPunmCmd.Text = "发送气动命令";
            this.btnSendPunmCmd.UseVisualStyleBackColor = true;
            this.btnSendPunmCmd.Click += new System.EventHandler(this.btnSendPunmCmd_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "终点";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(31, 57);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "起点";
            // 
            // cmbEndPlace_Punm
            // 
            this.cmbEndPlace_Punm.FormattingEnabled = true;
            this.cmbEndPlace_Punm.Items.AddRange(new object[] {
            "存查柜1(BB)",
            "存查柜2(BC)",
            "取样站(B0)",
            "弃样站(B1)",
            "单发单收站1(AB)",
            "单发单收站2(AC)",
            "存样站2(A1)",
            "存样站1(A0)"});
            this.cmbEndPlace_Punm.Location = new System.Drawing.Point(91, 81);
            this.cmbEndPlace_Punm.Name = "cmbEndPlace_Punm";
            this.cmbEndPlace_Punm.Size = new System.Drawing.Size(107, 21);
            this.cmbEndPlace_Punm.TabIndex = 9;
            // 
            // cmbStartPlace_Punm
            // 
            this.cmbStartPlace_Punm.FormattingEnabled = true;
            this.cmbStartPlace_Punm.Items.AddRange(new object[] {
            "存查柜1(BB)",
            "存查柜2(BC)",
            "取样站(B0)",
            "弃样站(B1)",
            "单发单收站1(AB)",
            "单发单收站2(AC)",
            "存样站2(A1)",
            "存样站1(A0)"});
            this.cmbStartPlace_Punm.Location = new System.Drawing.Point(91, 54);
            this.cmbStartPlace_Punm.Name = "cmbStartPlace_Punm";
            this.cmbStartPlace_Punm.Size = new System.Drawing.Size(107, 21);
            this.cmbStartPlace_Punm.TabIndex = 10;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.rtxtOutput);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(682, 508);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "存样柜";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(676, 502);
            this.panel1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSendgCYGCmd);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtMakeCode);
            this.groupBox1.Controls.Add(this.cmbCYG);
            this.groupBox1.Controls.Add(this.cmbCZPLX);
            this.groupBox1.Location = new System.Drawing.Point(21, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(472, 148);
            this.groupBox1.TabIndex = 48;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "存样柜命令测试";
            // 
            // btnSendgCYGCmd
            // 
            this.btnSendgCYGCmd.Location = new System.Drawing.Point(79, 102);
            this.btnSendgCYGCmd.Name = "btnSendgCYGCmd";
            this.btnSendgCYGCmd.Size = new System.Drawing.Size(107, 23);
            this.btnSendgCYGCmd.TabIndex = 8;
            this.btnSendgCYGCmd.Text = "发送存样柜命令";
            this.btnSendgCYGCmd.UseVisualStyleBackColor = true;
            this.btnSendgCYGCmd.Click += new System.EventHandler(this.btnSendgCYGCmd_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "存样柜";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "操作类型";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "制样码";
            // 
            // txtMakeCode
            // 
            this.txtMakeCode.Location = new System.Drawing.Point(79, 20);
            this.txtMakeCode.Name = "txtMakeCode";
            this.txtMakeCode.Size = new System.Drawing.Size(107, 22);
            this.txtMakeCode.TabIndex = 5;
            // 
            // cmbCYG
            // 
            this.cmbCYG.FormattingEnabled = true;
            this.cmbCYG.Location = new System.Drawing.Point(79, 75);
            this.cmbCYG.Name = "cmbCYG";
            this.cmbCYG.Size = new System.Drawing.Size(107, 21);
            this.cmbCYG.TabIndex = 4;
            // 
            // cmbCZPLX
            // 
            this.cmbCZPLX.FormattingEnabled = true;
            this.cmbCZPLX.Location = new System.Drawing.Point(79, 48);
            this.cmbCZPLX.Name = "cmbCZPLX";
            this.cmbCZPLX.Size = new System.Drawing.Size(107, 21);
            this.cmbCZPLX.TabIndex = 4;
            // 
            // rtxtOutput
            // 
            this.rtxtOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rtxtOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtOutput.ForeColor = System.Drawing.Color.White;
            this.rtxtOutput.Location = new System.Drawing.Point(3, 3);
            this.rtxtOutput.Name = "rtxtOutput";
            this.rtxtOutput.Size = new System.Drawing.Size(676, 502);
            this.rtxtOutput.TabIndex = 2;
            this.rtxtOutput.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "制样码";
            // 
            // txt_MakeCode_Pnum
            // 
            this.txt_MakeCode_Pnum.Location = new System.Drawing.Point(91, 21);
            this.txt_MakeCode_Pnum.Name = "txt_MakeCode_Pnum";
            this.txt_MakeCode_Pnum.Size = new System.Drawing.Size(107, 22);
            this.txt_MakeCode_Pnum.TabIndex = 16;
            // 
            // FrmAutoCupBoard_NCGM_Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 534);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmAutoCupBoard_NCGM_Test";
            this.Text = "DotNetBar Metro App Form";
            this.Load += new System.EventHandler(this.FrmAutoCupBoard_NCGM_Test_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox rtxtOutput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSendgCYGCmd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMakeCode;
        private System.Windows.Forms.ComboBox cmbCZPLX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbCYG;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSendPunmCmd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbEndPlace_Punm;
        private System.Windows.Forms.ComboBox cmbStartPlace_Punm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_MakeCode_Pnum;

    }
}

