namespace lab1_ex7
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
            this.label1 = new System.Windows.Forms.Label();
            this.ax_tb = new System.Windows.Forms.TextBox();
            this.ay_tb = new System.Windows.Forms.TextBox();
            this.az_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.state_tb = new System.Windows.Forms.TextBox();
            this.history_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.process_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ax";
            // 
            // ax_tb
            // 
            this.ax_tb.Location = new System.Drawing.Point(37, 6);
            this.ax_tb.Name = "ax_tb";
            this.ax_tb.Size = new System.Drawing.Size(100, 20);
            this.ax_tb.TabIndex = 1;
            // 
            // ay_tb
            // 
            this.ay_tb.Location = new System.Drawing.Point(172, 6);
            this.ay_tb.Name = "ay_tb";
            this.ay_tb.Size = new System.Drawing.Size(100, 20);
            this.ay_tb.TabIndex = 2;
            // 
            // az_tb
            // 
            this.az_tb.Location = new System.Drawing.Point(314, 6);
            this.az_tb.Name = "az_tb";
            this.az_tb.Size = new System.Drawing.Size(100, 20);
            this.az_tb.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Ay";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(289, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Az";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(236, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Current State";
            // 
            // state_tb
            // 
            this.state_tb.Location = new System.Drawing.Point(308, 79);
            this.state_tb.Name = "state_tb";
            this.state_tb.Size = new System.Drawing.Size(100, 20);
            this.state_tb.TabIndex = 7;
            // 
            // history_tb
            // 
            this.history_tb.Location = new System.Drawing.Point(12, 139);
            this.history_tb.Multiline = true;
            this.history_tb.Name = "history_tb";
            this.history_tb.Size = new System.Drawing.Size(396, 285);
            this.history_tb.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Data History";
            // 
            // process_btn
            // 
            this.process_btn.Location = new System.Drawing.Point(12, 46);
            this.process_btn.Name = "process_btn";
            this.process_btn.Size = new System.Drawing.Size(402, 23);
            this.process_btn.TabIndex = 10;
            this.process_btn.Text = "Process New Data Point";
            this.process_btn.UseVisualStyleBackColor = true;
            this.process_btn.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 438);
            this.Controls.Add(this.process_btn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.history_tb);
            this.Controls.Add(this.state_tb);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.az_tb);
            this.Controls.Add(this.ay_tb);
            this.Controls.Add(this.ax_tb);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ax_tb;
        private System.Windows.Forms.TextBox ay_tb;
        private System.Windows.Forms.TextBox az_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox state_tb;
        private System.Windows.Forms.TextBox history_tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button process_btn;
    }
}

