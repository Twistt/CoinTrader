namespace BinanceAutoTrader
{
    partial class Form1
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalPortfolioValueUSD = new System.Windows.Forms.Label();
            this.lblTotalPortfolioValueBTC = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // lblTotalPortfolioValueUSD
            // 
            this.lblTotalPortfolioValueUSD.AutoSize = true;
            this.lblTotalPortfolioValueUSD.Location = new System.Drawing.Point(762, 0);
            this.lblTotalPortfolioValueUSD.Name = "lblTotalPortfolioValueUSD";
            this.lblTotalPortfolioValueUSD.Size = new System.Drawing.Size(35, 13);
            this.lblTotalPortfolioValueUSD.TabIndex = 0;
            this.lblTotalPortfolioValueUSD.Text = "label1";
            // 
            // lblTotalPortfolioValueBTC
            // 
            this.lblTotalPortfolioValueBTC.AutoSize = true;
            this.lblTotalPortfolioValueBTC.Location = new System.Drawing.Point(721, 0);
            this.lblTotalPortfolioValueBTC.Name = "lblTotalPortfolioValueBTC";
            this.lblTotalPortfolioValueBTC.Size = new System.Drawing.Size(35, 13);
            this.lblTotalPortfolioValueBTC.TabIndex = 1;
            this.lblTotalPortfolioValueBTC.Text = "label1";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.lblTotalPortfolioValueUSD);
            this.flowLayoutPanel2.Controls.Add(this.lblTotalPortfolioValueBTC);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 427);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(800, 23);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblTotalPortfolioValueUSD;
        private System.Windows.Forms.Label lblTotalPortfolioValueBTC;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}

