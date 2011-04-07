namespace OvergrowthAutoUpdater
{
    partial class RevertVersion
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.cboxVersions = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblRevert = new System.Windows.Forms.Label();
            this.lblUpdate = new System.Windows.Forms.Label();
            this.cboxUpdate = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(148, 13);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Placeholder, text in the .cs file";
            // 
            // cboxVersions
            // 
            this.cboxVersions.FormattingEnabled = true;
            this.cboxVersions.Location = new System.Drawing.Point(12, 127);
            this.cboxVersions.Name = "cboxVersions";
            this.cboxVersions.Size = new System.Drawing.Size(121, 21);
            this.cboxVersions.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(168, 165);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(249, 165);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblRevert
            // 
            this.lblRevert.AutoSize = true;
            this.lblRevert.Location = new System.Drawing.Point(15, 108);
            this.lblRevert.Name = "lblRevert";
            this.lblRevert.Size = new System.Drawing.Size(99, 13);
            this.lblRevert.TabIndex = 4;
            this.lblRevert.Text = "Version to revert to:";
            // 
            // lblUpdate
            // 
            this.lblUpdate.AutoSize = true;
            this.lblUpdate.Location = new System.Drawing.Point(12, 151);
            this.lblUpdate.Name = "lblUpdate";
            this.lblUpdate.Size = new System.Drawing.Size(150, 13);
            this.lblUpdate.TabIndex = 6;
            this.lblUpdate.Text = "(Optional)Version to update to:";
            // 
            // cboxUpdate
            // 
            this.cboxUpdate.FormattingEnabled = true;
            this.cboxUpdate.Location = new System.Drawing.Point(12, 167);
            this.cboxUpdate.Name = "cboxUpdate";
            this.cboxUpdate.Size = new System.Drawing.Size(121, 21);
            this.cboxUpdate.TabIndex = 5;
            // 
            // RevertVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 200);
            this.Controls.Add(this.lblUpdate);
            this.Controls.Add(this.cboxUpdate);
            this.Controls.Add(this.lblRevert);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cboxVersions);
            this.Controls.Add(this.lblInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "RevertVersion";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Revert Game Version";
            this.Load += new System.EventHandler(this.RevertVersion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.ComboBox cboxVersions;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblRevert;
        private System.Windows.Forms.Label lblUpdate;
        private System.Windows.Forms.ComboBox cboxUpdate;
    }
}