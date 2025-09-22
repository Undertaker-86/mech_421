namespace lab1
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
            this.xLabel = new System.Windows.Forms.Label();
            this.txtMouseX = new System.Windows.Forms.TextBox();
            this.txtMouseY = new System.Windows.Forms.TextBox();
            this.yLable = new System.Windows.Forms.Label();
            this.picCanvas = new System.Windows.Forms.PictureBox();
            this.clickLabel = new System.Windows.Forms.Label();
            this.txtClicks = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picCanvas)).BeginInit();
            this.SuspendLayout();
            // 
            // xLabel
            // 
            this.xLabel.AutoSize = true;
            this.xLabel.Location = new System.Drawing.Point(12, 18);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(32, 25);
            this.xLabel.TabIndex = 0;
            this.xLabel.Text = "X:";
            this.xLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtMouseX
            // 
            this.txtMouseX.Location = new System.Drawing.Point(59, 13);
            this.txtMouseX.Name = "txtMouseX";
            this.txtMouseX.Size = new System.Drawing.Size(100, 31);
            this.txtMouseX.TabIndex = 1;
            this.txtMouseX.TextChanged += new System.EventHandler(this.txtMouseX_TextChanged);
            // 
            // txtMouseY
            // 
            this.txtMouseY.Location = new System.Drawing.Point(59, 51);
            this.txtMouseY.Name = "txtMouseY";
            this.txtMouseY.Size = new System.Drawing.Size(100, 31);
            this.txtMouseY.TabIndex = 2;
            // 
            // yLable
            // 
            this.yLable.AutoSize = true;
            this.yLable.Location = new System.Drawing.Point(12, 57);
            this.yLable.Name = "yLable";
            this.yLable.Size = new System.Drawing.Size(33, 25);
            this.yLable.TabIndex = 3;
            this.yLable.Text = "Y:";
            // 
            // picCanvas
            // 
            this.picCanvas.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.picCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picCanvas.Location = new System.Drawing.Point(220, 12);
            this.picCanvas.Name = "picCanvas";
            this.picCanvas.Size = new System.Drawing.Size(1318, 815);
            this.picCanvas.TabIndex = 4;
            this.picCanvas.TabStop = false;
            this.picCanvas.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picCanvas_MouseClick);
            this.picCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picCanvas_MouseMove);
            // 
            // clickLabel
            // 
            this.clickLabel.AutoSize = true;
            this.clickLabel.Location = new System.Drawing.Point(8, 121);
            this.clickLabel.Name = "clickLabel";
            this.clickLabel.Size = new System.Drawing.Size(169, 25);
            this.clickLabel.TabIndex = 5;
            this.clickLabel.Text = "Recorded Clicks";
            // 
            // txtClicks
            // 
            this.txtClicks.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtClicks.Location = new System.Drawing.Point(10, 169);
            this.txtClicks.Multiline = true;
            this.txtClicks.Name = "txtClicks";
            this.txtClicks.ReadOnly = true;
            this.txtClicks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtClicks.Size = new System.Drawing.Size(204, 657);
            this.txtClicks.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1550, 839);
            this.Controls.Add(this.txtClicks);
            this.Controls.Add(this.clickLabel);
            this.Controls.Add(this.picCanvas);
            this.Controls.Add(this.yLable);
            this.Controls.Add(this.txtMouseY);
            this.Controls.Add(this.txtMouseX);
            this.Controls.Add(this.xLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picCanvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label xLabel;
        private System.Windows.Forms.TextBox txtMouseX;
        private System.Windows.Forms.TextBox txtMouseY;
        private System.Windows.Forms.Label yLable;
        private System.Windows.Forms.PictureBox picCanvas;
        private System.Windows.Forms.Label clickLabel;
        private System.Windows.Forms.TextBox txtClicks;
    }
}

