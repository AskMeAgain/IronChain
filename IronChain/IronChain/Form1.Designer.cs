namespace IronChain {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMiningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopMiningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendIronToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ironChainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyseFromGenesisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startFromGenesisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteIronChainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllAccountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBox2.Location = new System.Drawing.Point(12, 149);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(272, 326);
            this.textBox2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(155, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 31);
            this.label3.TabIndex = 18;
            this.label3.Text = "0 Iron";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(500, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 29);
            this.label5.TabIndex = 27;
            this.label5.Text = "Block 0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(28, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 20);
            this.label7.TabIndex = 30;
            this.label7.Text = "Your Account";
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.Control;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(20, 64);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 32;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.onAccountChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.miningToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.ironChainToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1022, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // miningToolStripMenuItem
            // 
            this.miningToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startMiningToolStripMenuItem,
            this.stopMiningToolStripMenuItem});
            this.miningToolStripMenuItem.Name = "miningToolStripMenuItem";
            this.miningToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.miningToolStripMenuItem.Text = "Mining";
            // 
            // startMiningToolStripMenuItem
            // 
            this.startMiningToolStripMenuItem.Name = "startMiningToolStripMenuItem";
            this.startMiningToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.startMiningToolStripMenuItem.Text = "Start Mining";
            this.startMiningToolStripMenuItem.Click += new System.EventHandler(this.onClickStartMining);
            // 
            // stopMiningToolStripMenuItem
            // 
            this.stopMiningToolStripMenuItem.Name = "stopMiningToolStripMenuItem";
            this.stopMiningToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.stopMiningToolStripMenuItem.Text = "Stop Mining";
            this.stopMiningToolStripMenuItem.Click += new System.EventHandler(this.onClickStopMining);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendIronToolStripMenuItem,
            this.addAccountToolStripMenuItem,
            this.importAccountToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionToolStripMenuItem.Text = "Action";
            // 
            // sendIronToolStripMenuItem
            // 
            this.sendIronToolStripMenuItem.Name = "sendIronToolStripMenuItem";
            this.sendIronToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.sendIronToolStripMenuItem.Text = "Send Iron";
            this.sendIronToolStripMenuItem.Click += new System.EventHandler(this.onClickSendIron);
            // 
            // addAccountToolStripMenuItem
            // 
            this.addAccountToolStripMenuItem.Name = "addAccountToolStripMenuItem";
            this.addAccountToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.addAccountToolStripMenuItem.Text = "Create Account";
            this.addAccountToolStripMenuItem.Click += new System.EventHandler(this.onClickAddAccount);
            // 
            // importAccountToolStripMenuItem
            // 
            this.importAccountToolStripMenuItem.Name = "importAccountToolStripMenuItem";
            this.importAccountToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.importAccountToolStripMenuItem.Text = "Import Account";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Settings";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.onClickOpenSettings);
            // 
            // ironChainToolStripMenuItem
            // 
            this.ironChainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analyseFromGenesisToolStripMenuItem,
            this.startFromGenesisToolStripMenuItem,
            this.deleteIronChainToolStripMenuItem,
            this.deleteAllAccountsToolStripMenuItem});
            this.ironChainToolStripMenuItem.Name = "ironChainToolStripMenuItem";
            this.ironChainToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.ironChainToolStripMenuItem.Text = "IronChain";
            // 
            // analyseFromGenesisToolStripMenuItem
            // 
            this.analyseFromGenesisToolStripMenuItem.Name = "analyseFromGenesisToolStripMenuItem";
            this.analyseFromGenesisToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.analyseFromGenesisToolStripMenuItem.Text = "Analyse from Genesis";
            // 
            // startFromGenesisToolStripMenuItem
            // 
            this.startFromGenesisToolStripMenuItem.Name = "startFromGenesisToolStripMenuItem";
            this.startFromGenesisToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.startFromGenesisToolStripMenuItem.Text = "Start From Genesis";
            // 
            // deleteIronChainToolStripMenuItem
            // 
            this.deleteIronChainToolStripMenuItem.Name = "deleteIronChainToolStripMenuItem";
            this.deleteIronChainToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.deleteIronChainToolStripMenuItem.Text = "Delete IronChain";
            this.deleteIronChainToolStripMenuItem.Click += new System.EventHandler(this.onClickDeleteIronChain);
            // 
            // deleteAllAccountsToolStripMenuItem
            // 
            this.deleteAllAccountsToolStripMenuItem.Name = "deleteAllAccountsToolStripMenuItem";
            this.deleteAllAccountsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.deleteAllAccountsToolStripMenuItem.Text = "Delete all Accounts";
            this.deleteAllAccountsToolStripMenuItem.Click += new System.EventHandler(this.onClickDeleteAllAccounts);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1125, 72);
            this.panel1.TabIndex = 38;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(235, 31);
            this.label4.TabIndex = 39;
            this.label4.Text = "Your Transactions";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(290, 237);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 23);
            this.button1.TabIndex = 40;
            this.button1.Text = "Start Listening ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.onClickCreateServerListener);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(290, 290);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(163, 23);
            this.button2.TabIndex = 41;
            this.button2.Text = "Connect as client";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.onClickConnectClient);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(290, 346);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 23);
            this.button3.TabIndex = 42;
            this.button3.Text = "RequestFile";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.onClickRequestFile);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(847, 266);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(163, 162);
            this.textBox1.TabIndex = 44;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(983, 117);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(27, 20);
            this.textBox3.TabIndex = 45;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(847, 208);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(170, 23);
            this.button4.TabIndex = 46;
            this.button4.Text = "Print ChainPath";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(935, 179);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 47;
            this.button5.Text = "TestResize";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(847, 237);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(175, 23);
            this.button6.TabIndex = 48;
            this.button6.Text = "Set changepath to desktop";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(844, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 15);
            this.label1.TabIndex = 49;
            this.label1.Text = "Not listening for connections";
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(290, 320);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(126, 20);
            this.textBox4.TabIndex = 50;
            this.textBox4.Text = "localhost";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(422, 319);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(31, 20);
            this.textBox5.TabIndex = 51;
            this.textBox5.Text = "4712";
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(561, 237);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(81, 23);
            this.button7.TabIndex = 52;
            this.button7.Text = "Push Block";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.onClickPushBlock);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(558, 218);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 53;
            this.label2.Text = "Not working ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 578);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startMiningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopMiningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendIronToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ironChainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analyseFromGenesisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAccountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startFromGenesisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteIronChainToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem deleteAllAccountsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importAccountToolStripMenuItem;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label2;
    }
}

