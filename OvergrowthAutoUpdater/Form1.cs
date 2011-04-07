using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Net;
using Ionic.Zip;


namespace OvergrowthAutoUpdater
{
    /// <summary>So I can pass more than one number to the BackgroundWorker</summary>
    public struct UpdateData
    {
        /// <summary>The alpha to be updated to</summary>
        public int next;
        /// <summary>How many times we are going to update. Obtained from updateZips.Length</summary>
        public int iterations;
        public UpdateData(int Next, int Iterations) { next = Next; iterations = Iterations; }
    }

    public partial class frmMain : Form
    {

        /// <summary>The base URL for the update file. Does not include the "a###.zip" part. </summary>
        public static string updateURL = "http://cdn.wolfire.com/alpha/diffs/";
        ///<summary>The .txt file where all of the config options are stored.</summary>
        public static string configPath = Directory.GetCurrentDirectory() + "\\config.txt";
        ///<summary>An object holding all of the option values.</summary>
        public ConfigAtrributes attributes;
        ///<summary>The file names for the update zip files.</summary>
        public string[] updateZips;
        ///<summary>True if it is the first time the form is loaded. Used so message boxes don't pop up.</summary>
        public bool firstTime = true;
        ///<summary>The window that pops open when the user clicks btnExeBrowse</summary>
        public OpenFileDialog exeFileDialog;
        ///<summary>The window that pops up when the user clicks btnUpdateDir</summary>
        public FolderBrowserDialog fldrUpdateDialog;
        ///<summary>The current version of the .exe file. Not persistent. Format a###,</summary>
        public string currentVersion;
        ///<summary>The latest version available to download.</summary>
        public int latestVersion;
        ////<summary>A variable used if the sequential download option is true</summary>
        //private bool canDownloadNextFile = true;
        /// <summary>value is of "Download" type</summary> 
        public ArrayList clients;
        /// <summary> The total amount of bytes we expect to download from every file </summary>
        public long totalUpdateSize;
        /// <summary>The amount of bytes we have recieved from the downloaded(ing) files </summary>
        public long totalUpdateRecieved;
        /// <summary>true if the program is allowed to apply the update</summary>
        public bool canApplyUpdate = false;
        /// <summary>I need this in the global scope, it contains what update to apply next</summary>
        public UpdateData udata; 

        public frmMain()
        {
            InitializeComponent();
            
            firstTime = false;
            totalUpdateSize = 0;
            totalUpdateRecieved = 0;
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            ReadConfigFile(configPath); //gets the persistant data

            //Setting the options to what the user had before. 
            txtUpdateDir.Text = attributes.updateDirectory;
            if (attributes.exeDirectory != "")
            {
                currentVersion = GetCurrentVersion();
                lblCurrentVersion.Text = "Current Overgrowth version: " + currentVersion;
                txtExeDir.Text = attributes.exeDirectory + "Overgrowth.exe";
                latestVersion = FindLatestVersion(int.Parse(currentVersion.Remove(0, 1))); //might make startup long on slow Internet computers
            }
            cboxHaveUpdate.Checked = attributes.hasUpdateFiles;
            createBackupToolStripMenuItem.Checked = attributes.createBackup;
            downloadSequentiallyToolStripMenuItem.Checked = attributes.sequentialDownload;
            switch (attributes.downloadOption)
            {
                case "Download": rbtnDownload.Checked = true; break;
                case "Update": rbtnUpdate.Checked = true; break;
                case "Download and Update": rbtnDownloadAndUpdate.Checked = true; break;
                default: break;
            }
            loggingToolStripMenuItem.Checked = attributes.logging;
        }


        //cancels any async operations before the form closes
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            attributes.WriteToFile(configPath);

            //if it is downloading an update
            if (totalUpdateSize != 0 && totalUpdateRecieved != totalUpdateSize)
            {
                foreach (Download dl in clients)
                    dl.wclient.CancelAsync();
            }

            //if we are updating files
            if (bwUpdateFiles.IsBusy)
            {
                bwUpdateFiles.CancelAsync();
            }
        }


        ///<summary>Changes the download options that are available if the user supplies its own update files.</summary>
        private void cboxHaveUpdate_CheckStateChanged(object sender, EventArgs e)
        {
            if (cboxHaveUpdate.CheckState == CheckState.Checked)
            {
                rbtnDownload.Enabled = false;
                rbtnDownloadAndUpdate.Enabled = false;
                rbtnUpdate.Enabled = true;
                rbtnUpdate.Checked = true;
                canApplyUpdate = true;
                UpdateListBox();
            }
            else
            {
                rbtnDownload.Enabled = true;
                rbtnDownloadAndUpdate.Enabled = true;
                rbtnDownloadAndUpdate.Checked = true;
                rbtnUpdate.Enabled = false;
                canApplyUpdate = false; // could be bad if the user checks it during download
                lstUpdates.Items.Clear(); //empty the list box so that the user doesn't think the option is checked
            }
        }


        ///<summary>When the user checks the box for them already having update files, this function
        /// lists all of the update files in the folder. Might highlight which ones will be used?</summary>
        private void UpdateListBox()
        {
            try //it only makes sense, because we are working with files
            {
                updateZips = Directory.GetFiles(attributes.updateDirectory, "a*.zip");
                if (updateZips.Length == 0) return; //don't put anything in the list box if no update files are there
                for (int i = 0; i < updateZips.Length; i++)
                {
                    //the '-4' at the end makes it so it doesn't remove the 'a###' part. If alpha versions go into the 1000's
                    //then this value will have to be changed
                    lstUpdates.Items.Add(updateZips[i].Remove(0, updateZips[i].IndexOf(".zip")-4)); //I hope this is in order.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something happened in the updateListBox function\n" + ex.Message);
            }
            
        }

        /********************************
        The three radio buttons
        ********************************/
        private void rbtnDownload_CheckedChanged(object sender, EventArgs e)
        {
            attributes.downloadOption = "Download";
            btnDoUpdate.Text = attributes.downloadOption;
        }

        private void rbtnUpdate_CheckedChanged(object sender, EventArgs e)
        {
            attributes.downloadOption = "Update";
            btnDoUpdate.Text = attributes.downloadOption;
        }

        private void rbtnDownloadAndUpdate_CheckedChanged(object sender, EventArgs e)
        {
            attributes.downloadOption = "Download and Update";
            btnDoUpdate.Text = attributes.downloadOption;
        }


        //I think I want this instead of "TextChanged"
        private void txtUpdateDir_Leave(object sender, EventArgs e)
        {
            if (firstTime)
            {
                //remove trailing slashes
                if (txtUpdateDir.Text[txtUpdateDir.Text.Length - 1] == '\\')
                    txtUpdateDir.Text.Remove(txtUpdateDir.Text.Length - 1);
                return; //Don't do anything if it is the initial start up
            }

            if (!Directory.Exists(txtUpdateDir.Text))
            {
                DialogResult result = MessageBox.Show("The directory " + attributes.updateDirectory + " does not currently exist. Would you like to make it?",
                    "Directory does not exist", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    //remove trailing slashes
                    if (txtUpdateDir.Text[txtUpdateDir.Text.Length - 1] == '\\')
                        txtUpdateDir.Text.Remove(txtUpdateDir.Text.Length - 1);
                    attributes.updateDirectory = txtUpdateDir.Text;
                    Directory.CreateDirectory(attributes.updateDirectory);
                }
                else
                {
                    txtUpdateDir.Text = attributes.updateDirectory; //put it back to what it was before
                }
            } //end if
        }//end txtUpdateDir_Leave


        ///<summary>Make sure the user points to Overgrowth.exe so we can orient ourselves.</summary>
        private void opnFileDialogExe_FileOk(object sender, CancelEventArgs e)
        {
            if (!File.Exists(exeFileDialog.FileName))
                txtExeDir.Text = "The file you gave does not exist";

            if (exeFileDialog.FileName.Contains("Overgrowth.exe") && File.Exists(exeFileDialog.FileName))
            {
                txtExeDir.Text = exeFileDialog.FileName;
                attributes.exeDirectory = exeFileDialog.FileName;
                attributes.exeDirectory = attributes.exeDirectory.Remove(attributes.exeDirectory.IndexOf("Overgrowth.exe"));
                currentVersion = GetCurrentVersion();
                lblCurrentVersion.Text = "Current Overgrowth version: " + currentVersion;
            }
            else
                //show error
                txtExeDir.Text = "You did not choose Overgrowth.exe";
        }


        /**************************************
        Buttons
        **************************************/


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnExeBrowse_Click(object sender, EventArgs e)
        {
            exeFileDialog = opnFileDialogExe;
            exeFileDialog.ShowDialog();
        }

        /// <summary>When the user clicks the button, it opens a FolderBrowser dialog.
        /// When the user clicks "Ok", it makes sure that the folder exists and uses that folder as the update directory </summary>
        private void btnUpdateDir_Click(object sender, EventArgs e)
        {
            fldrUpdateDialog = new FolderBrowserDialog();
            DialogResult dresult = fldrUpdateDialog.ShowDialog();

            if (dresult == DialogResult.OK)
            {
                if (!Directory.Exists(fldrUpdateDialog.SelectedPath))
                {
                    //show message that the folder isn't good
                    return;
                }

                txtUpdateDir.Text = fldrUpdateDialog.SelectedPath;
                attributes.updateDirectory = fldrUpdateDialog.SelectedPath;

                //make sure the list box updates when the folder is changed
                if (cboxHaveUpdate.Checked)
                    UpdateListBox();
            }
        }


        ///<summary>This is the "Do everything with given info" button.</summary>
        private void btnDoUpdate_Click(object sender, EventArgs e)
        {
            //Make sure we  have all the info we need
            if (!File.Exists(attributes.exeDirectory + "Overgrowth.exe"))
            {
                btnExeBrowse_Click(this, e);
                return;
            }
            if (!Directory.Exists(attributes.updateDirectory))
            {
                txtUpdateDir_Leave(this, e);
                return;
            }

            toggleDownloadOptions(false);
            cboxHaveUpdate.Enabled = false;

            if (rbtnDownload.Checked || rbtnDownloadAndUpdate.Checked)
                DownloadUpdateFiles();
            if (rbtnUpdate.Checked)
            {
                DoUpdateFiles();
            }
        }


        /// <summary>Changes the options for all of the radio buttons in the group box for download options. Also the btnDoUpdate.</summary>
        /// <param name="enabled">What to set the radiobuttons.Enabled to</param>
        private void toggleDownloadOptions(bool enabled)
        {
            //so the user doesn't change the options in the middle of the process
            rbtnDownload.Enabled = enabled;
            rbtnDownloadAndUpdate.Enabled = enabled;
            rbtnUpdate.Enabled = enabled;
            btnDoUpdate.Enabled = enabled;
        }


        /************************************
        Toolstrip Items
        ************************************/

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form abt = new About();
            abt.Show(this);            
        }

        private void createBackupToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            attributes.createBackup = createBackupToolStripMenuItem.Checked;
        }

        private void loggingToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            attributes.logging = loggingToolStripMenuItem.Checked;
        }


        //Deletes the files, then the directories. You can not delete non-empty directories.
        private void cleanUpdatesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bwDeleteFiles.RunWorkerAsync();
        }


        //Opens a form for the user to select which version to go to.
        private void revertGameVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int version = 1;
            int toVersion = 1;

            //so the form doesn't get disposed before we get the data we need
            using (RevertVersion rev = new RevertVersion(this))
            {
                DialogResult res = rev.ShowDialog();
                if (res == DialogResult.OK)
                {
                    version = rev.retVersion;
                    toVersion = rev.retToVersion;
                }
                else //the user closed the form without wanting to change
                    return;
            }

            if (version != 0)
            {
                if (ChangeCurrentVersion(version))
                {
                    sstriplblStatus.Text = "Version change success. Click 'Download and Update' to update to latest version.";
                    lblCurrentVersion.Text = "Current Overgrowth version: "+ GetCurrentVersion();
                } 
                else
                    sstriplblStatus.Text = "Version change failed.";
            }
            else if (version == 0)
                sstriplblStatus.Text = "Version change failed. You didn't select a valid version";
            else if (version == 1 || toVersion == 1) //we only get here when bad things happen
                sstriplblStatus.Text = "Version change failed. The variable(s) never got set";

            if (toVersion != 0) //if we specified a version we want to update to
            {
                if (toVersion < version) 
                    sstriplblStatus.Text = "Will not update to the version specified, because it is less than the reverted version.";
                else
                    latestVersion = toVersion;
            }

        }//end revertGameVersionToolStripMenuItem_Click


        /*****************************************
        BackgroundWorker to do the UpdateFiles
        *****************************************/

        /// <summary>So the UI thread isn't busied up for the duration of the file portion of the update</summary>
        private void bwUpdateFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            ReplaceFiles(attributes.updateDirectory + "\\a" + udata.next + ".zip");
        }

        private void bwUpdateFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1:
                    sstriplblStatus.Text = "Unpackaging the .zip update file.";
                    break;
                case 2:
                    sstriplblStatus.Text = "Finding common directories.";
                    break;
                case 3:
                    sstriplblStatus.Text = "Replacing files.";
                    break;
                case 4:
                    sstriplblStatus.Text = "Replacing Windows only files";
                    break;
                case 5:
                    sstriplblStatus.Text = "Compressing the backup files.";
                    break;
                default:
                    sstriplblStatus.Text = "Idle";
                    break;
            }

        }

        private void bwUpdateFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            udata.next += 1;
            udata.iterations -= 1;

            //failure conditions. If we have updated once for every zip in the array, or the next zip puts us above the latest
            if (udata.iterations < 1 || udata.next > latestVersion)
            {
                lblCurrentVersion.Text = "Current version: " + GetCurrentVersion();
                sstriplblStatus.Text = "Done";
                toggleDownloadOptions(true);
                cboxHaveUpdate.Enabled = true;
                return; // is this correct?
            }

            bwUpdateFiles.RunWorkerAsync(); //I hope this doesn't go all recursive-crazy on me
        }


        /*****************************************
        BackgroundWorker to delete files
        *****************************************/
        private void bwDeleteFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            CleanUpdatesFolder(attributes.updateDirectory, false);
        }

        private void bwDeleteFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toggleDownloadOptions(true);
            sstriplblStatus.Text = "Finished deleting files from the update directory.";
        }

        private void bwDeleteFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                toggleDownloadOptions(false); //so we don't attempt to download stuff in the middle of deleting
                sstriplblStatus.Text = "Deleting files from the update directory.";
            }
        }
    } //end partial class
} //end namespace
