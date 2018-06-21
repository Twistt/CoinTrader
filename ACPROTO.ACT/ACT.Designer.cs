namespace ACPROTO.ACT
{
    partial class ACT
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
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalPortfolioValueUSD = new System.Windows.Forms.Label();
            this.lblTotalPortfolioValueBTC = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.flpAlerts = new System.Windows.Forms.FlowLayoutPanel();
            this.flpChat = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.flpOrders = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPositions = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabWatches = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.flpAlerts.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPositions.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.lblTotalPortfolioValueUSD);
            this.flowLayoutPanel2.Controls.Add(this.lblTotalPortfolioValueBTC);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 316);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(783, 23);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // lblTotalPortfolioValueUSD
            // 
            this.lblTotalPortfolioValueUSD.AutoSize = true;
            this.lblTotalPortfolioValueUSD.Location = new System.Drawing.Point(745, 0);
            this.lblTotalPortfolioValueUSD.Name = "lblTotalPortfolioValueUSD";
            this.lblTotalPortfolioValueUSD.Size = new System.Drawing.Size(35, 13);
            this.lblTotalPortfolioValueUSD.TabIndex = 0;
            this.lblTotalPortfolioValueUSD.Text = "label1";
            // 
            // lblTotalPortfolioValueBTC
            // 
            this.lblTotalPortfolioValueBTC.AutoSize = true;
            this.lblTotalPortfolioValueBTC.Location = new System.Drawing.Point(704, 0);
            this.lblTotalPortfolioValueBTC.Name = "lblTotalPortfolioValueBTC";
            this.lblTotalPortfolioValueBTC.Size = new System.Drawing.Size(35, 13);
            this.lblTotalPortfolioValueBTC.TabIndex = 1;
            this.lblTotalPortfolioValueBTC.Text = "label1";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(552, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(228, 310);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.flpAlerts);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(220, 284);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Alerts";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // flpAlerts
            // 
            this.flpAlerts.Controls.Add(this.flpChat);
            this.flpAlerts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpAlerts.Location = new System.Drawing.Point(3, 3);
            this.flpAlerts.Name = "flpAlerts";
            this.flpAlerts.Size = new System.Drawing.Size(214, 278);
            this.flpAlerts.TabIndex = 0;
            // 
            // flpChat
            // 
            this.flpChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpChat.Location = new System.Drawing.Point(3, 3);
            this.flpChat.Name = "flpChat";
            this.flpChat.Size = new System.Drawing.Size(154, 0);
            this.flpChat.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.flpOrders);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(220, 284);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Orders";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // flpOrders
            // 
            this.flpOrders.AutoScroll = true;
            this.flpOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpOrders.Location = new System.Drawing.Point(3, 3);
            this.flpOrders.Name = "flpOrders";
            this.flpOrders.Size = new System.Drawing.Size(214, 278);
            this.flpOrders.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(220, 284);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Discussion";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPositions);
            this.tabControl2.Controls.Add(this.tabWatches);
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(556, 310);
            this.tabControl2.TabIndex = 3;
            // 
            // tabPositions
            // 
            this.tabPositions.Controls.Add(this.flowLayoutPanel1);
            this.tabPositions.Location = new System.Drawing.Point(4, 22);
            this.tabPositions.Name = "tabPositions";
            this.tabPositions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPositions.Size = new System.Drawing.Size(548, 284);
            this.tabPositions.TabIndex = 0;
            this.tabPositions.Text = "CryptoPositions";
            this.tabPositions.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(542, 278);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // tabWatches
            // 
            this.tabWatches.Location = new System.Drawing.Point(4, 22);
            this.tabWatches.Name = "tabWatches";
            this.tabWatches.Padding = new System.Windows.Forms.Padding(3);
            this.tabWatches.Size = new System.Drawing.Size(548, 284);
            this.tabWatches.TabIndex = 1;
            this.tabWatches.Text = "Watch List";
            this.tabWatches.UseVisualStyleBackColor = true;
            // 
            // ACT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 339);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Name = "ACT";
            this.Text = "ACProto Crypto Trader";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.flpAlerts.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPositions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lblTotalPortfolioValueUSD;
        private System.Windows.Forms.Label lblTotalPortfolioValueBTC;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.FlowLayoutPanel flpAlerts;
        private System.Windows.Forms.FlowLayoutPanel flpOrders;
        private System.Windows.Forms.FlowLayoutPanel flpChat;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPositions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TabPage tabWatches;
    }
}

