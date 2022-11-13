namespace Synthie.Effects
{
    partial class RingModInputs
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
            this.lblfreq = new System.Windows.Forms.Label();
            this.txtfreq = new System.Windows.Forms.TextBox();
            this.lblamp = new System.Windows.Forms.Label();
            this.txtamp = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblfreq
            // 
            this.lblfreq.AutoSize = true;
            this.lblfreq.Location = new System.Drawing.Point(13, 13);
            this.lblfreq.Name = "lblfreq";
            this.lblfreq.Size = new System.Drawing.Size(63, 13);
            this.lblfreq.TabIndex = 0;
            this.lblfreq.Text = "Frequency :";
            // 
            // txtfreq
            // 
            this.txtfreq.Location = new System.Drawing.Point(82, 10);
            this.txtfreq.Name = "txtfreq";
            this.txtfreq.Size = new System.Drawing.Size(69, 20);
            this.txtfreq.TabIndex = 1;
            this.txtfreq.Text = "5";
            // 
            // lblamp
            // 
            this.lblamp.AutoSize = true;
            this.lblamp.Location = new System.Drawing.Point(16, 43);
            this.lblamp.Name = "lblamp";
            this.lblamp.Size = new System.Drawing.Size(62, 13);
            this.lblamp.TabIndex = 2;
            this.lblamp.Text = "Amplitude : ";
            // 
            // txtamp
            // 
            this.txtamp.Location = new System.Drawing.Point(82, 43);
            this.txtamp.Name = "txtamp";
            this.txtamp.Size = new System.Drawing.Size(69, 20);
            this.txtamp.TabIndex = 3;
            this.txtamp.Text = "0.1";
            // 
            // RingModInputs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 86);
            this.Controls.Add(this.txtamp);
            this.Controls.Add(this.lblamp);
            this.Controls.Add(this.txtfreq);
            this.Controls.Add(this.lblfreq);
            this.Name = "RingModInputs";
            this.Text = "RingModInputs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblfreq;
        private System.Windows.Forms.TextBox txtfreq;
        private System.Windows.Forms.Label lblamp;
        private System.Windows.Forms.TextBox txtamp;
    }
}