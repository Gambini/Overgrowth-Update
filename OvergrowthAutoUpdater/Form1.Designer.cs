﻿namespace OvergrowthAutoUpdater
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblDirExe = new System.Windows.Forms.Label();
            this.txtExeDir = new System.Windows.Forms.TextBox();
            this.btnExeBrowse = new System.Windows.Forms.Button();
            this.lblUpdatePath = new System.Windows.Forms.Label();
            this.txtUpdateDir = new System.Windows.Forms.TextBox();
            this.Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.cboxHaveUpdate = new System.Windows.Forms.CheckBox();
            this.btnUpdateDir = new System.Windows.Forms.Button();
            this.sstripInfo = new System.Windows.Forms.StatusStrip();
            this.sstriplblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbarDownload = new System.Windows.Forms.ProgressBar();
            this.lblDownloadProgress = new System.Windows.Forms.Label();
            this.dnu_lblDownloadProgress = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUpdatesFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertGameVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadSequentiallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rbtnDownload = new System.Windows.Forms.RadioButton();
            this.rbtnUpdate = new System.Windows.Forms.RadioButton();
            this.rbtnDownloadAndUpdate = new System.Windows.Forms.RadioButton();
            this.grpDownloadOptions = new System.Windows.Forms.GroupBox();
            this.btnDoUpdate = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lstUpdates = new System.Windows.Forms.ListBox();
            this.opnFileDialogExe = new System.Windows.Forms.OpenFileDialog();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.foldBrowserDialogUpdate = new System.Windows.Forms.FolderBrowserDialog();
            this.lblUpdatesDownloaded = new System.Windows.Forms.Label();
            this.lstDownloadProgress = new System.Windows.Forms.ListBox();
            this.lblIndividualDownloadProgress = new System.Windows.Forms.Label();
            this.bwUpdateFiles = new System.ComponentModel.BackgroundWorker();
            this.bwDeleteFiles = new System.ComponentModel.BackgroundWorker();
            this.updateAvailableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateThisProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bwUpdateTool = new System.ComponentModel.BackgroundWorker();
            this.sstripInfo.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.grpDownloadOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDirExe
            // 
            this.lblDirExe.AutoSize = true;
            this.lblDirExe.Location = new System.Drawing.Point(12, 30);
            this.lblDirExe.Name = "lblDirExe";
            this.lblDirExe.Size = new System.Drawing.Size(138, 13);
            this.lblDirExe.TabIndex = 0;
            this.lblDirExe.Text = "The path to overgrowth.exe";
            this.Tooltip.SetToolTip(this.lblDirExe, "Make sure the last part points to a .exe file in your Overgrowth folder.");
            // 
            // txtExeDir
            // 
            this.txtExeDir.Location = new System.Drawing.Point(13, 47);
            this.txtExeDir.Name = "txtExeDir";
            this.txtExeDir.Size = new System.Drawing.Size(353, 20);
            this.txtExeDir.TabIndex = 0;
            this.txtExeDir.Text = "Example: C:\\Program Files\\Overgrowth\\Overgrowth.exe";
            this.Tooltip.SetToolTip(this.txtExeDir, "Make sure the last part points to a .exe file in your Overgrowth folder.");
            // 
            // btnExeBrowse
            // 
            this.btnExeBrowse.Location = new System.Drawing.Point(372, 47);
            this.btnExeBrowse.Name = "btnExeBrowse";
            this.btnExeBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnExeBrowse.TabIndex = 1;
            this.btnExeBrowse.Text = "Browse...";
            this.btnExeBrowse.UseVisualStyleBackColor = true;
            this.btnExeBrowse.Click += new System.EventHandler(this.btnExeBrowse_Click);
            // 
            // lblUpdatePath
            // 
            this.lblUpdatePath.AutoSize = true;
            this.lblUpdatePath.Location = new System.Drawing.Point(13, 96);
            this.lblUpdatePath.Name = "lblUpdatePath";
            this.lblUpdatePath.Size = new System.Drawing.Size(154, 13);
            this.lblUpdatePath.TabIndex = 8;
            this.lblUpdatePath.Text = "Where to download the update";
            this.Tooltip.SetToolTip(this.lblUpdatePath, "This is the folder that has all of the .zip update files. Do not unzip them, othe" +
                    "rwise the update might not work. Do not have a any slash at the end, or it will " +
                    "not work.");
            // 
            // txtUpdateDir
            // 
            this.txtUpdateDir.Location = new System.Drawing.Point(13, 113);
            this.txtUpdateDir.Name = "txtUpdateDir";
            this.txtUpdateDir.Size = new System.Drawing.Size(353, 20);
            this.txtUpdateDir.TabIndex = 2;
            this.txtUpdateDir.Text = "currentdirectory\\Updates\\";
            this.Tooltip.SetToolTip(this.txtUpdateDir, "This is the folder that has all of the .zip update files. Do not unzip them, othe" +
                    "rwise the update might not work. Do not have a any slash at the end, or it will " +
                    "not work.");
            this.txtUpdateDir.Leave += new System.EventHandler(this.txtUpdateDir_Leave);
            // 
            // cboxHaveUpdate
            // 
            this.cboxHaveUpdate.AutoSize = true;
            this.cboxHaveUpdate.Location = new System.Drawing.Point(19, 140);
            this.cboxHaveUpdate.Name = "cboxHaveUpdate";
            this.cboxHaveUpdate.Size = new System.Drawing.Size(390, 17);
            this.cboxHaveUpdate.TabIndex = 11;
            this.cboxHaveUpdate.Text = "I already have downloaded the update(s). (Make sure it is in the above folder)";
            this.Tooltip.SetToolTip(this.cboxHaveUpdate, "Check this box if you have already downloaded the update(s). Do not rename the .z" +
                    "ip file, or the update won\'t work properly");
            this.cboxHaveUpdate.UseVisualStyleBackColor = true;
            this.cboxHaveUpdate.CheckStateChanged += new System.EventHandler(this.cboxHaveUpdate_CheckStateChanged);
            // 
            // btnUpdateDir
            // 
            this.btnUpdateDir.Location = new System.Drawing.Point(373, 113);
            this.btnUpdateDir.Name = "btnUpdateDir";
            this.btnUpdateDir.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateDir.TabIndex = 3;
            this.btnUpdateDir.Text = "Browse...";
            this.btnUpdateDir.UseVisualStyleBackColor = true;
            this.btnUpdateDir.Click += new System.EventHandler(this.btnUpdateDir_Click);
            // 
            // sstripInfo
            // 
            this.sstripInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sstriplblStatus});
            this.sstripInfo.Location = new System.Drawing.Point(0, 420);
            this.sstripInfo.Name = "sstripInfo";
            this.sstripInfo.Size = new System.Drawing.Size(600, 22);
            this.sstripInfo.SizingGrip = false;
            this.sstripInfo.TabIndex = 12;
            this.sstripInfo.Text = "statusStrip1";
            // 
            // sstriplblStatus
            // 
            this.sstriplblStatus.Name = "sstriplblStatus";
            this.sstriplblStatus.Size = new System.Drawing.Size(26, 17);
            this.sstriplblStatus.Text = "Idle";
            // 
            // pbarDownload
            // 
            this.pbarDownload.Location = new System.Drawing.Point(12, 321);
            this.pbarDownload.Name = "pbarDownload";
            this.pbarDownload.Size = new System.Drawing.Size(477, 23);
            this.pbarDownload.TabIndex = 13;
            // 
            // lblDownloadProgress
            // 
            this.lblDownloadProgress.AutoSize = true;
            this.lblDownloadProgress.Location = new System.Drawing.Point(495, 331);
            this.lblDownloadProgress.Name = "lblDownloadProgress";
            this.lblDownloadProgress.Size = new System.Drawing.Size(89, 13);
            this.lblDownloadProgress.TabIndex = 14;
            this.lblDownloadProgress.Text = "Not Downloading";
            // 
            // dnu_lblDownloadProgress
            // 
            this.dnu_lblDownloadProgress.AutoSize = true;
            this.dnu_lblDownloadProgress.Location = new System.Drawing.Point(19, 305);
            this.dnu_lblDownloadProgress.Name = "dnu_lblDownloadProgress";
            this.dnu_lblDownloadProgress.Size = new System.Drawing.Size(102, 13);
            this.dnu_lblDownloadProgress.TabIndex = 15;
            this.dnu_lblDownloadProgress.Text = "Download Progress:";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.updateAvailableToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(600, 24);
            this.menuStrip.TabIndex = 16;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cleanUpdatesFolderToolStripMenuItem,
            this.revertGameVersionToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // cleanUpdatesFolderToolStripMenuItem
            // 
            this.cleanUpdatesFolderToolStripMenuItem.Name = "cleanUpdatesFolderToolStripMenuItem";
            this.cleanUpdatesFolderToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.cleanUpdatesFolderToolStripMenuItem.Text = "Clean Updates folder";
            this.cleanUpdatesFolderToolStripMenuItem.ToolTipText = "Deletes all of the files from the update directory";
            this.cleanUpdatesFolderToolStripMenuItem.Click += new System.EventHandler(this.cleanUpdatesFolderToolStripMenuItem_Click);
            // 
            // revertGameVersionToolStripMenuItem
            // 
            this.revertGameVersionToolStripMenuItem.Name = "revertGameVersionToolStripMenuItem";
            this.revertGameVersionToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.revertGameVersionToolStripMenuItem.Text = "Revert game version...";
            this.revertGameVersionToolStripMenuItem.ToolTipText = "Changes your version to a previous version. Useful if you want to make sure all o" +
                "f the previous updates installed correctly.";
            this.revertGameVersionToolStripMenuItem.Click += new System.EventHandler(this.revertGameVersionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createBackupToolStripMenuItem,
            this.loggingToolStripMenuItem,
            this.downloadSequentiallyToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // createBackupToolStripMenuItem
            // 
            this.createBackupToolStripMenuItem.Checked = true;
            this.createBackupToolStripMenuItem.CheckOnClick = true;
            this.createBackupToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.createBackupToolStripMenuItem.Name = "createBackupToolStripMenuItem";
            this.createBackupToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.createBackupToolStripMenuItem.Text = "Create Backup";
            this.createBackupToolStripMenuItem.ToolTipText = "Creates a backup of every file that will be replaced in the update process.";
            this.createBackupToolStripMenuItem.CheckedChanged += new System.EventHandler(this.createBackupToolStripMenuItem_CheckedChanged);
            // 
            // loggingToolStripMenuItem
            // 
            this.loggingToolStripMenuItem.CheckOnClick = true;
            this.loggingToolStripMenuItem.Name = "loggingToolStripMenuItem";
            this.loggingToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.loggingToolStripMenuItem.Text = "Logging";
            this.loggingToolStripMenuItem.ToolTipText = "Will write which files were added/modified to a log.txt file";
            this.loggingToolStripMenuItem.CheckedChanged += new System.EventHandler(this.loggingToolStripMenuItem_CheckedChanged);
            // 
            // downloadSequentiallyToolStripMenuItem
            // 
            this.downloadSequentiallyToolStripMenuItem.CheckOnClick = true;
            this.downloadSequentiallyToolStripMenuItem.Enabled = false;
            this.downloadSequentiallyToolStripMenuItem.Name = "downloadSequentiallyToolStripMenuItem";
            this.downloadSequentiallyToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.downloadSequentiallyToolStripMenuItem.Text = "Download sequentially";
            this.downloadSequentiallyToolStripMenuItem.ToolTipText = "(Not Implemented)Downloads the update files one at a time and in order, rather th" +
                "an all at once.";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // rbtnDownload
            // 
            this.rbtnDownload.AutoSize = true;
            this.rbtnDownload.Location = new System.Drawing.Point(12, 19);
            this.rbtnDownload.Name = "rbtnDownload";
            this.rbtnDownload.Size = new System.Drawing.Size(73, 17);
            this.rbtnDownload.TabIndex = 0;
            this.rbtnDownload.TabStop = true;
            this.rbtnDownload.Text = "Download";
            this.rbtnDownload.UseVisualStyleBackColor = true;
            this.rbtnDownload.CheckedChanged += new System.EventHandler(this.rbtnDownload_CheckedChanged);
            // 
            // rbtnUpdate
            // 
            this.rbtnUpdate.AutoSize = true;
            this.rbtnUpdate.Enabled = false;
            this.rbtnUpdate.Location = new System.Drawing.Point(92, 19);
            this.rbtnUpdate.Name = "rbtnUpdate";
            this.rbtnUpdate.Size = new System.Drawing.Size(60, 17);
            this.rbtnUpdate.TabIndex = 1;
            this.rbtnUpdate.TabStop = true;
            this.rbtnUpdate.Text = "Update";
            this.rbtnUpdate.UseVisualStyleBackColor = true;
            this.rbtnUpdate.CheckedChanged += new System.EventHandler(this.rbtnUpdate_CheckedChanged);
            // 
            // rbtnDownloadAndUpdate
            // 
            this.rbtnDownloadAndUpdate.AutoSize = true;
            this.rbtnDownloadAndUpdate.Location = new System.Drawing.Point(159, 19);
            this.rbtnDownloadAndUpdate.Name = "rbtnDownloadAndUpdate";
            this.rbtnDownloadAndUpdate.Size = new System.Drawing.Size(132, 17);
            this.rbtnDownloadAndUpdate.TabIndex = 2;
            this.rbtnDownloadAndUpdate.TabStop = true;
            this.rbtnDownloadAndUpdate.Text = "Download and Update";
            this.rbtnDownloadAndUpdate.UseVisualStyleBackColor = true;
            this.rbtnDownloadAndUpdate.CheckedChanged += new System.EventHandler(this.rbtnDownloadAndUpdate_CheckedChanged);
            // 
            // grpDownloadOptions
            // 
            this.grpDownloadOptions.Controls.Add(this.rbtnUpdate);
            this.grpDownloadOptions.Controls.Add(this.rbtnDownloadAndUpdate);
            this.grpDownloadOptions.Controls.Add(this.rbtnDownload);
            this.grpDownloadOptions.Location = new System.Drawing.Point(12, 371);
            this.grpDownloadOptions.Name = "grpDownloadOptions";
            this.grpDownloadOptions.Size = new System.Drawing.Size(295, 41);
            this.grpDownloadOptions.TabIndex = 6;
            this.grpDownloadOptions.TabStop = false;
            this.grpDownloadOptions.Text = "Download Options";
            // 
            // btnDoUpdate
            // 
            this.btnDoUpdate.Location = new System.Drawing.Point(424, 389);
            this.btnDoUpdate.Name = "btnDoUpdate";
            this.btnDoUpdate.Size = new System.Drawing.Size(164, 23);
            this.btnDoUpdate.TabIndex = 8;
            this.btnDoUpdate.Text = "Download and Update";
            this.btnDoUpdate.UseVisualStyleBackColor = true;
            this.btnDoUpdate.Click += new System.EventHandler(this.btnDoUpdate_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(343, 389);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lstUpdates
            // 
            this.lstUpdates.FormattingEnabled = true;
            this.lstUpdates.Location = new System.Drawing.Point(12, 194);
            this.lstUpdates.Name = "lstUpdates";
            this.lstUpdates.Size = new System.Drawing.Size(202, 108);
            this.lstUpdates.Sorted = true;
            this.lstUpdates.TabIndex = 4;
            // 
            // opnFileDialogExe
            // 
            this.opnFileDialogExe.Filter = "Exe files|*.exe";
            this.opnFileDialogExe.Title = "Choose Overgrowth.exe";
            this.opnFileDialogExe.FileOk += new System.ComponentModel.CancelEventHandler(this.opnFileDialogExe_FileOk);
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Location = new System.Drawing.Point(178, 28);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(223, 13);
            this.lblCurrentVersion.TabIndex = 24;
            this.lblCurrentVersion.Text = "Current Overgrowth version: (no exe selected)";
            // 
            // foldBrowserDialogUpdate
            // 
            this.foldBrowserDialogUpdate.RootFolder = System.Environment.SpecialFolder.ProgramFiles;
            // 
            // lblUpdatesDownloaded
            // 
            this.lblUpdatesDownloaded.AutoSize = true;
            this.lblUpdatesDownloaded.Location = new System.Drawing.Point(13, 175);
            this.lblUpdatesDownloaded.Name = "lblUpdatesDownloaded";
            this.lblUpdatesDownloaded.Size = new System.Drawing.Size(108, 13);
            this.lblUpdatesDownloaded.TabIndex = 25;
            this.lblUpdatesDownloaded.Text = "Updates downloaded";
            // 
            // lstDownloadProgress
            // 
            this.lstDownloadProgress.Enabled = false;
            this.lstDownloadProgress.FormattingEnabled = true;
            this.lstDownloadProgress.Location = new System.Drawing.Point(244, 194);
            this.lstDownloadProgress.Name = "lstDownloadProgress";
            this.lstDownloadProgress.Size = new System.Drawing.Size(204, 108);
            this.lstDownloadProgress.TabIndex = 5;
            // 
            // lblIndividualDownloadProgress
            // 
            this.lblIndividualDownloadProgress.AutoSize = true;
            this.lblIndividualDownloadProgress.Enabled = false;
            this.lblIndividualDownloadProgress.Location = new System.Drawing.Point(244, 175);
            this.lblIndividualDownloadProgress.Name = "lblIndividualDownloadProgress";
            this.lblIndividualDownloadProgress.Size = new System.Drawing.Size(98, 13);
            this.lblIndividualDownloadProgress.TabIndex = 27;
            this.lblIndividualDownloadProgress.Text = "Download progress";
            // 
            // bwUpdateFiles
            // 
            this.bwUpdateFiles.WorkerReportsProgress = true;
            this.bwUpdateFiles.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwUpdateFiles_DoWork);
            this.bwUpdateFiles.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwUpdateFiles_ProgressChanged);
            this.bwUpdateFiles.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwUpdateFiles_RunWorkerCompleted);
            // 
            // bwDeleteFiles
            // 
            this.bwDeleteFiles.WorkerReportsProgress = true;
            this.bwDeleteFiles.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwDeleteFiles_DoWork);
            this.bwDeleteFiles.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwDeleteFiles_ProgressChanged);
            this.bwDeleteFiles.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwDeleteFiles_RunWorkerCompleted);
            // 
            // updateAvailableToolStripMenuItem
            // 
            this.updateAvailableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateThisProgramToolStripMenuItem});
            this.updateAvailableToolStripMenuItem.Enabled = false;
            this.updateAvailableToolStripMenuItem.Name = "updateAvailableToolStripMenuItem";
            this.updateAvailableToolStripMenuItem.Size = new System.Drawing.Size(118, 20);
            this.updateAvailableToolStripMenuItem.Text = "*Update Available*";
            this.updateAvailableToolStripMenuItem.ToolTipText = "Click this dropdown menu and select \"Update this program...\" to update. A restart" +
                " of this program will be required.";
            this.updateAvailableToolStripMenuItem.Visible = false;
            // 
            // updateThisProgramToolStripMenuItem
            // 
            this.updateThisProgramToolStripMenuItem.Name = "updateThisProgramToolStripMenuItem";
            this.updateThisProgramToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.updateThisProgramToolStripMenuItem.Text = "Update this program...";
            this.updateThisProgramToolStripMenuItem.ToolTipText = "Downloads the file to be updated to.";
            this.updateThisProgramToolStripMenuItem.Click += new System.EventHandler(this.updateThisProgramToolStripMenuItem_Click);
            // 
            // bwUpdateTool
            // 
            this.bwUpdateTool.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwUpdateTool_DoWork);
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnDoUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(600, 442);
            this.Controls.Add(this.lblIndividualDownloadProgress);
            this.Controls.Add(this.lstDownloadProgress);
            this.Controls.Add(this.lblUpdatesDownloaded);
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.lstUpdates);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnDoUpdate);
            this.Controls.Add(this.grpDownloadOptions);
            this.Controls.Add(this.dnu_lblDownloadProgress);
            this.Controls.Add(this.lblDownloadProgress);
            this.Controls.Add(this.pbarDownload);
            this.Controls.Add(this.sstripInfo);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.cboxHaveUpdate);
            this.Controls.Add(this.btnUpdateDir);
            this.Controls.Add(this.txtUpdateDir);
            this.Controls.Add(this.lblUpdatePath);
            this.Controls.Add(this.btnExeBrowse);
            this.Controls.Add(this.txtExeDir);
            this.Controls.Add(this.lblDirExe);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Overgrowth Updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.sstripInfo.ResumeLayout(false);
            this.sstripInfo.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.grpDownloadOptions.ResumeLayout(false);
            this.grpDownloadOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDirExe;
        private System.Windows.Forms.TextBox txtExeDir;
        private System.Windows.Forms.Button btnExeBrowse;
        private System.Windows.Forms.Label lblUpdatePath;
        private System.Windows.Forms.TextBox txtUpdateDir;
        private System.Windows.Forms.ToolTip Tooltip;
        private System.Windows.Forms.Button btnUpdateDir;
        private System.Windows.Forms.CheckBox cboxHaveUpdate;
        private System.Windows.Forms.StatusStrip sstripInfo;
        private System.Windows.Forms.ToolStripStatusLabel sstriplblStatus;
        private System.Windows.Forms.ProgressBar pbarDownload;
        private System.Windows.Forms.Label lblDownloadProgress;
        private System.Windows.Forms.Label dnu_lblDownloadProgress;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.RadioButton rbtnDownload;
        private System.Windows.Forms.RadioButton rbtnUpdate;
        private System.Windows.Forms.RadioButton rbtnDownloadAndUpdate;
        private System.Windows.Forms.GroupBox grpDownloadOptions;
        private System.Windows.Forms.Button btnDoUpdate;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ListBox lstUpdates;
        private System.Windows.Forms.OpenFileDialog opnFileDialogExe;
        private System.Windows.Forms.ToolStripMenuItem createBackupToolStripMenuItem;
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.ToolStripMenuItem downloadSequentiallyToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog foldBrowserDialogUpdate;
        private System.Windows.Forms.Label lblUpdatesDownloaded;
        private System.Windows.Forms.ListBox lstDownloadProgress;
        private System.Windows.Forms.Label lblIndividualDownloadProgress;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cleanUpdatesFolderToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bwUpdateFiles;
        private System.Windows.Forms.ToolStripMenuItem loggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem revertGameVersionToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bwDeleteFiles;
        private System.Windows.Forms.ToolStripMenuItem updateAvailableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateThisProgramToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bwUpdateTool;
    }
}

