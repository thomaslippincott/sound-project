namespace Synthie.Effects
{
    partial class ReverbInputs
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
            this.lbldelay = new System.Windows.Forms.Label();
            this.txtDelay = new System.Windows.Forms.TextBox();
            this.lblTaps = new System.Windows.Forms.Label();
            this.txtTaps = new System.Windows.Forms.TextBox();
            this.cbPiano = new System.Windows.Forms.CheckBox();
            this.cborgan = new System.Windows.Forms.CheckBox();
            this.cbdrums = new System.Windows.Forms.CheckBox();
            this.cbadditive = new System.Windows.Forms.CheckBox();
            this.lblwetper = new System.Windows.Forms.Label();
            this.txtwetper = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbldelay
            // 
            this.lbldelay.AutoSize = true;
            this.lbldelay.Location = new System.Drawing.Point(13, 13);
            this.lbldelay.Name = "lbldelay";
            this.lbldelay.Size = new System.Drawing.Size(43, 13);
            this.lbldelay.TabIndex = 0;
            this.lbldelay.Text = "Delay : ";
            // 
            // txtDelay
            // 
            this.txtDelay.Location = new System.Drawing.Point(62, 6);
            this.txtDelay.Name = "txtDelay";
            this.txtDelay.Size = new System.Drawing.Size(100, 20);
            this.txtDelay.TabIndex = 1;
            this.txtDelay.Text = "0.2";
            // 
            // lblTaps
            // 
            this.lblTaps.AutoSize = true;
            this.lblTaps.Location = new System.Drawing.Point(13, 42);
            this.lblTaps.Name = "lblTaps";
            this.lblTaps.Size = new System.Drawing.Size(40, 13);
            this.lblTaps.TabIndex = 2;
            this.lblTaps.Text = "Taps : ";
            // 
            // txtTaps
            // 
            this.txtTaps.Location = new System.Drawing.Point(62, 42);
            this.txtTaps.Name = "txtTaps";
            this.txtTaps.Size = new System.Drawing.Size(100, 20);
            this.txtTaps.TabIndex = 3;
            this.txtTaps.Text = "2";
            // 
            // cbPiano
            // 
            this.cbPiano.AutoSize = true;
            this.cbPiano.Location = new System.Drawing.Point(16, 92);
            this.cbPiano.Name = "cbPiano";
            this.cbPiano.Size = new System.Drawing.Size(53, 17);
            this.cbPiano.TabIndex = 4;
            this.cbPiano.Text = "Piano";
            this.cbPiano.UseVisualStyleBackColor = true;
            // 
            // cborgan
            // 
            this.cborgan.AutoSize = true;
            this.cborgan.Location = new System.Drawing.Point(98, 92);
            this.cborgan.Name = "cborgan";
            this.cborgan.Size = new System.Drawing.Size(55, 17);
            this.cborgan.TabIndex = 5;
            this.cborgan.Text = "Organ";
            this.cborgan.UseVisualStyleBackColor = true;
            // 
            // cbdrums
            // 
            this.cbdrums.AutoSize = true;
            this.cbdrums.Location = new System.Drawing.Point(16, 116);
            this.cbdrums.Name = "cbdrums";
            this.cbdrums.Size = new System.Drawing.Size(56, 17);
            this.cbdrums.TabIndex = 6;
            this.cbdrums.Text = "Drums";
            this.cbdrums.UseVisualStyleBackColor = true;
            // 
            // cbadditive
            // 
            this.cbadditive.AutoSize = true;
            this.cbadditive.Location = new System.Drawing.Point(98, 116);
            this.cbadditive.Name = "cbadditive";
            this.cbadditive.Size = new System.Drawing.Size(64, 17);
            this.cbadditive.TabIndex = 7;
            this.cbadditive.Text = "Additive";
            this.cbadditive.UseVisualStyleBackColor = true;
            // 
            // lblwetper
            // 
            this.lblwetper.AutoSize = true;
            this.lblwetper.Location = new System.Drawing.Point(16, 73);
            this.lblwetper.Name = "lblwetper";
            this.lblwetper.Size = new System.Drawing.Size(47, 13);
            this.lblwetper.TabIndex = 8;
            this.lblwetper.Text = "Wet % : ";
            // 
            // txtwetper
            // 
            this.txtwetper.Location = new System.Drawing.Point(62, 70);
            this.txtwetper.Name = "txtwetper";
            this.txtwetper.Size = new System.Drawing.Size(100, 20);
            this.txtwetper.TabIndex = 9;
            this.txtwetper.Text = "0.5";
            // 
            // ReverbInputs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(187, 155);
            this.Controls.Add(this.txtwetper);
            this.Controls.Add(this.lblwetper);
            this.Controls.Add(this.cbadditive);
            this.Controls.Add(this.cbdrums);
            this.Controls.Add(this.cborgan);
            this.Controls.Add(this.cbPiano);
            this.Controls.Add(this.txtTaps);
            this.Controls.Add(this.lblTaps);
            this.Controls.Add(this.txtDelay);
            this.Controls.Add(this.lbldelay);
            this.Name = "ReverbInputs";
            this.Text = "ReverbInputs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbldelay;
        private System.Windows.Forms.TextBox txtDelay;
        private System.Windows.Forms.Label lblTaps;
        private System.Windows.Forms.TextBox txtTaps;
        private System.Windows.Forms.CheckBox cbPiano;
        private System.Windows.Forms.CheckBox cborgan;
        private System.Windows.Forms.CheckBox cbdrums;
        private System.Windows.Forms.CheckBox cbadditive;
        private System.Windows.Forms.Label lblwetper;
        private System.Windows.Forms.TextBox txtwetper;
    }
}