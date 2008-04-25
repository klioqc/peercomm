namespace cir.PeerComm.ClientExample
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
            this._MakeCert = new System.Windows.Forms.Button();
            this._MakeSatAss = new System.Windows.Forms.Button();
            this._CompanyName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _MakeCert
            // 
            this._MakeCert.Location = new System.Drawing.Point(13, 13);
            this._MakeCert.Name = "_MakeCert";
            this._MakeCert.Size = new System.Drawing.Size(75, 23);
            this._MakeCert.TabIndex = 0;
            this._MakeCert.Text = "Make Cert";
            this._MakeCert.UseVisualStyleBackColor = true;
            this._MakeCert.Click += new System.EventHandler(this._MakeCert_Click);
            // 
            // _MakeSatAss
            // 
            this._MakeSatAss.Location = new System.Drawing.Point(159, 12);
            this._MakeSatAss.Name = "_MakeSatAss";
            this._MakeSatAss.Size = new System.Drawing.Size(121, 23);
            this._MakeSatAss.TabIndex = 1;
            this._MakeSatAss.Text = "Make Assembly";
            this._MakeSatAss.UseVisualStyleBackColor = true;
            this._MakeSatAss.Click += new System.EventHandler(this._MakeSatAss_Click);
            // 
            // _CompanyName
            // 
            this._CompanyName.Location = new System.Drawing.Point(13, 43);
            this._CompanyName.Name = "_CompanyName";
            this._CompanyName.Size = new System.Drawing.Size(100, 20);
            this._CompanyName.TabIndex = 2;
            this._CompanyName.Text = "FiveInchFish";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this._CompanyName);
            this.Controls.Add(this._MakeSatAss);
            this.Controls.Add(this._MakeCert);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _MakeCert;
        private System.Windows.Forms.Button _MakeSatAss;
        private System.Windows.Forms.TextBox _CompanyName;
    }
}

