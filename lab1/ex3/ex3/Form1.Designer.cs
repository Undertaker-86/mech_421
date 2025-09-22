namespace ex2
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
            this.components = new System.ComponentModel.Container();
            this.button_enqueue = new System.Windows.Forms.Button();
            this.buttonDequeue = new System.Windows.Forms.Button();
            this.txtEnqueue = new System.Windows.Forms.TextBox();
            this.txtDequeue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtQueueAmount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDequeAverage = new System.Windows.Forms.TextBox();
            this.textAvgResult = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.buttonAverage = new System.Windows.Forms.Button();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // button_enqueue
            // 
            this.button_enqueue.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button_enqueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_enqueue.Location = new System.Drawing.Point(25, 33);
            this.button_enqueue.Name = "button_enqueue";
            this.button_enqueue.Size = new System.Drawing.Size(215, 46);
            this.button_enqueue.TabIndex = 0;
            this.button_enqueue.Text = "Enqueue";
            this.button_enqueue.UseVisualStyleBackColor = false;
            this.button_enqueue.Click += new System.EventHandler(this.button_enqueue_Click);
            // 
            // buttonDequeue
            // 
            this.buttonDequeue.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonDequeue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDequeue.Location = new System.Drawing.Point(25, 104);
            this.buttonDequeue.Name = "buttonDequeue";
            this.buttonDequeue.Size = new System.Drawing.Size(215, 46);
            this.buttonDequeue.TabIndex = 1;
            this.buttonDequeue.Text = "Dequeue";
            this.buttonDequeue.UseVisualStyleBackColor = false;
            this.buttonDequeue.Click += new System.EventHandler(this.buttonDequeue_Click);
            // 
            // txtEnqueue
            // 
            this.txtEnqueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEnqueue.Location = new System.Drawing.Point(254, 33);
            this.txtEnqueue.Name = "txtEnqueue";
            this.txtEnqueue.Size = new System.Drawing.Size(357, 44);
            this.txtEnqueue.TabIndex = 2;
            this.txtEnqueue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEnqueue_KeyDown);
            // 
            // txtDequeue
            // 
            this.txtDequeue.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.txtDequeue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDequeue.Location = new System.Drawing.Point(254, 104);
            this.txtDequeue.Name = "txtDequeue";
            this.txtDequeue.ReadOnly = true;
            this.txtDequeue.Size = new System.Drawing.Size(357, 44);
            this.txtDequeue.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 181);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 37);
            this.label1.TabIndex = 4;
            this.label1.Text = "Items in Queue";
            // 
            // txtQueueAmount
            // 
            this.txtQueueAmount.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.txtQueueAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQueueAmount.Location = new System.Drawing.Point(254, 178);
            this.txtQueueAmount.Name = "txtQueueAmount";
            this.txtQueueAmount.ReadOnly = true;
            this.txtQueueAmount.Size = new System.Drawing.Size(357, 44);
            this.txtQueueAmount.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 300);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 37);
            this.label2.TabIndex = 6;
            this.label2.Text = "N:";
            // 
            // txtDequeAverage
            // 
            this.txtDequeAverage.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtDequeAverage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDequeAverage.Location = new System.Drawing.Point(70, 297);
            this.txtDequeAverage.Name = "txtDequeAverage";
            this.txtDequeAverage.Size = new System.Drawing.Size(170, 44);
            this.txtDequeAverage.TabIndex = 7;
            // 
            // textAvgResult
            // 
            this.textAvgResult.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.textAvgResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textAvgResult.Location = new System.Drawing.Point(433, 295);
            this.textAvgResult.Name = "textAvgResult";
            this.textAvgResult.ReadOnly = true;
            this.textAvgResult.Size = new System.Drawing.Size(178, 44);
            this.textAvgResult.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(283, 300);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 37);
            this.label3.TabIndex = 8;
            this.label3.Text = "Average:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(41, 362);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(249, 37);
            this.label4.TabIndex = 10;
            this.label4.Text = "Queue Contents";
            // 
            // txtContent
            // 
            this.txtContent.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.txtContent.Location = new System.Drawing.Point(34, 403);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ReadOnly = true;
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtContent.Size = new System.Drawing.Size(579, 503);
            this.txtContent.TabIndex = 11;
            // 
            // buttonAverage
            // 
            this.buttonAverage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonAverage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAverage.Location = new System.Drawing.Point(48, 235);
            this.buttonAverage.Name = "buttonAverage";
            this.buttonAverage.Size = new System.Drawing.Size(563, 46);
            this.buttonAverage.TabIndex = 12;
            this.buttonAverage.Text = "Dequeue and Average First N Data";
            this.buttonAverage.UseVisualStyleBackColor = false;
            this.buttonAverage.Click += new System.EventHandler(this.buttonAverage_Click);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 918);
            this.Controls.Add(this.buttonAverage);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textAvgResult);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDequeAverage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtQueueAmount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDequeue);
            this.Controls.Add(this.txtEnqueue);
            this.Controls.Add(this.buttonDequeue);
            this.Controls.Add(this.button_enqueue);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_enqueue;
        private System.Windows.Forms.Button buttonDequeue;
        private System.Windows.Forms.TextBox txtEnqueue;
        private System.Windows.Forms.TextBox txtDequeue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtQueueAmount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDequeAverage;
        private System.Windows.Forms.TextBox textAvgResult;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Button buttonAverage;
        private System.Windows.Forms.Timer updateTimer;
    }
}

