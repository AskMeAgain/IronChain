namespace IronChain {
    partial class TestWindow {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestWindow));
            this.button15 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(302, 12);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(176, 23);
            this.button15.TabIndex = 82;
            this.button15.Text = "Change path to test folder";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.onClickChangeGlobalPath);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(12, 296);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(176, 23);
            this.button14.TabIndex = 81;
            this.button14.Text = "Download blocks from Server";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.onClickRequestBlock);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(12, 267);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(176, 23);
            this.button12.TabIndex = 79;
            this.button12.Text = "Push new block height to Server";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.onClickPushBlock);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(302, 133);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(176, 23);
            this.button9.TabIndex = 77;
            this.button9.Text = "Connect to Server with port 3000";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.onClickConnectTo3001);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(302, 162);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(176, 23);
            this.button8.TabIndex = 76;
            this.button8.Text = "Connect to Server with port 3001";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.onClickConnectTo3000);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(302, 94);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(176, 23);
            this.button7.TabIndex = 75;
            this.button7.Text = "Host Server on Port 3001";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.onClickHostServer3001);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(302, 64);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(176, 23);
            this.button6.TabIndex = 74;
            this.button6.Text = "Host Server on  Port 3000";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.onClickHostServer3000);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 130);
            this.label1.TabIndex = 83;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 25);
            this.label2.TabIndex = 84;
            this.label2.Text = "Test Suite";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 238);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(176, 23);
            this.button1.TabIndex = 85;
            this.button1.Text = "disable automatic block download";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.disableEnableAutomaticBlockDownload);
            // 
            // TestWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 366);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Name = "TestWindow";
            this.Text = "TestWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}