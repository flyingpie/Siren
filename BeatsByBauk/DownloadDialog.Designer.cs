namespace B3
{
    partial class DownloadDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadDialog));
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // prgProgress
            // 
            this.prgProgress.Location = new System.Drawing.Point(12, 77);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(560, 23);
            this.prgProgress.Step = 1;
            this.prgProgress.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(145)))), ((int)(((byte)(148)))));
            this.lblStatus.Location = new System.Drawing.Point(15, 42);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(6);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(554, 26);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Starting...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(145)))), ((int)(((byte)(148)))));
            this.lblInfo.Location = new System.Drawing.Point(15, 109);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(6);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(554, 26);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "...";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(145)))), ((int)(((byte)(148)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 15);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(554, 26);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DownloadDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(584, 162);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.prgProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DownloadDialog";
            this.Text = "Download";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgProgress;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblTitle;
    }
}