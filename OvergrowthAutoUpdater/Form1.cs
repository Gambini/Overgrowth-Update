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
                latestVersion = FindLatestVersion(int.Parse(currentVersion.Remove(0, 1)));
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
            
            

        }


        ///<summary>Reads and/or Creates the config.txt file, and modifies the 'attributes' variable.</summary>
        private void ReadConfigFile(string configPath)
        {
            attributes = new ConfigAtrributes();
            string line;
            string[] data;

            try //because we are messing with files
            {
                FileStream ifs = File.Open(configPath, FileMode.OpenOrCreate); //self-documenting
                StreamReader sread = new StreamReader(ifs);

                while (!sread.EndOfStream)
                {
                    line = sread.ReadLine();
                    data = line.Split('='); //should end up with the data name in [0] and the value in [1]

                    //Incoming mess. This assigns the correct value to the correct data.
                    switch( data[0] )
                    {
                        case "updateDirectory":
                            if( data[1] != "") //the default value is the current directory\Updates
                                attributes.updateDirectory = data[1];
                            break;
                        case "exeDirectory":
                            if (data[1] != "")
                                attributes.exeDirectory = data[1];
                            break;
                        case "downloadOption":
                            attributes.downloadOption = data[1];
                            break;
                        case "hasUpdateFiles":
                            attributes.hasUpdateFiles = bool.Parse(data[1]);
                            break;
                        case "createBackup":
                            attributes.createBackup = bool.Parse(data[1]);
                            break;
                        case "sequentialDownload":
                            attributes.sequentialDownload = bool.Parse(data[1]);
                            break;
                        default:
                            break;
                    } //end switch
                } //end while
                sread.Close();
                ifs.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something happened in the ReadConfigFile function" + ex.Message);
            }
        }//end ReadConfigFile


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


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            attributes.WriteToFile(configPath);

            //if it is downloading an update
            if (totalUpdateSize != 0 && totalUpdateRecieved != totalUpdateSize)
            {
                foreach (Download dl in clients)
                    dl.wclient.CancelAsync();
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

            latestVersion = FindLatestVersion(int.Parse(GetCurrentVersion().Remove(0,1)));

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


        private void DoUpdateFiles()
        {
            if (!canApplyUpdate)
                return;

            sstriplblStatus.Text = "Applying Updates";
            int next = 0; //the alpha version we are looking to update to
            currentVersion = GetCurrentVersion(); //so we have the most up to date version
            int current = int.Parse(currentVersion.Remove(0,1));

            updateZips = Directory.GetFiles(attributes.updateDirectory + "\\", "a*.zip");
            if (updateZips.Length == 0)
            {
                MessageBox.Show("You have no updates in the update folder.");
                return; //don't put anything in the list box if no update files are there
            }

            next = current += 1;
            if (next <= latestVersion)
                if (File.Exists(attributes.updateDirectory + "\\a" + next + ".zip"))
                {
                    udata = new UpdateData(next, updateZips.Length);
                    bwUpdateFiles.RunWorkerAsync();
                    //ReplaceFiles(attributes.updateDirectory + "\\a" + next + ".zip");
                }
                
                else
                {
                    MessageBox.Show("Error in DoUpdateFiles function.\n" +
                        "Could not find update file " + attributes.updateDirectory + "\\a" + next + ".zip\n" +
                        "You will be at version a" + (next - 1) + " until the update file for a" + next + " is found.");
                    return;
                }
            else
            {
                MessageBox.Show("You are at the latest version.");
                return;
            }
        }


        /// <summary>Does the actual file operations to apply the update. It is a mess.</summary>
        /// <param name="path">The path of the .zip file, including the .zip</param>
        private void ReplaceFiles(string path)
        {
            string[] updateFiles; //The file names and paths taken from the .zip folder
            string[] currentFiles; //The file names of the currently installed version
            string[] updateDirectories;
            string[] currentDirectories;
            ArrayList commonDirectories = new ArrayList();
            ArrayList newFiles = new ArrayList();
            ZipFile backup;

            using (ZipFile zip = ZipFile.Read(path))
            {
                bwUpdateFiles.ReportProgress(1);
                zip.ExtractAll(path.Remove(path.IndexOf(".zip")));
                zip.Dispose();
            }

            path = path.Remove(path.IndexOf(".zip")) + "\\update\\";

            try
            {
                //get all of the directories from each one.
                updateDirectories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                currentDirectories = Directory.GetDirectories(attributes.exeDirectory, "*", SearchOption.AllDirectories);

                if (updateDirectories.Length == 0 || currentDirectories.Length == 0) return;
                bwUpdateFiles.ReportProgress(2);
                //go through each of the directories and compare them. Mightily ineffiecient, but effective
                for (int u = 0; u < updateDirectories.Length; u++)
                {
                    for (int c = 0; c < currentDirectories.Length; c++)
                    {
                        //string b = updateDirectories[u].Remove(0, path.Length); //for debug
                        //string d = currentDirectories[c].Remove(0, attributes.exeDirectory.Length);
                        if (updateDirectories[u].Remove(0, path.Length) == currentDirectories[c].Remove(0, attributes.exeDirectory.Length))
                            commonDirectories.Add(updateDirectories[u].Remove(0, path.Length));//does not end with \\
                        else
                        {
                            if (!Directory.Exists(attributes.exeDirectory + updateDirectories[u].Remove(0, path.Length)))
                            {
                                Directory.CreateDirectory(attributes.exeDirectory + updateDirectories[u].Remove(0, path.Length));
                                commonDirectories.Add(updateDirectories[u].Remove(0, path.Length));
                            }
                        }
                    }
                }

                if (attributes.createBackup)
                    backup = new ZipFile(path + "backupfiles.zip");
                else backup = new ZipFile();//not going to use it

                bwUpdateFiles.ReportProgress(3);
                foreach (string dir in commonDirectories)
                {
                    if (attributes.createBackup)
                        backup.AddDirectoryByName(dir);

                    updateFiles = Directory.GetFiles(path + dir + "\\");
                    currentFiles = Directory.GetFiles(attributes.exeDirectory + dir + "\\");
                    if (updateFiles.Length == 0 || currentFiles.Length == 0) continue;

                    //go through all of the files
                    for (int u = 0; u < updateFiles.Length; u++)
                    {
                        for (int c = 0; c < currentFiles.Length; c++)
                        {
                            //string a = updateFiles[u].Remove(0, path.Length + dir.Length); //for debug
                            //string b = currentFiles[c].Remove(0, attributes.exeDirectory.Length + dir.Length);
                            //if the files are the same
                            if (updateFiles[u].Remove(0, path.Length + dir.Length) == currentFiles[c].Remove(0, attributes.exeDirectory.Length + dir.Length))
                            {
                                if (attributes.createBackup)
                                    backup.AddFile(currentFiles[c], dir);

                                File.Delete(currentFiles[c]); //delete the file, so we don't get errors from the next line
                                File.Copy(updateFiles[u], currentFiles[c]);
                            }
                            else //it is a new file
                            {  
                                //we need this check because it will keep on iterating through the non-common files
                                if (!File.Exists(attributes.exeDirectory + dir + "\\" + updateFiles[u].Remove(0, path.Length + dir.Length)))
                                {
                                    //                                                              The name of the file
                                    File.Copy(updateFiles[u], attributes.exeDirectory + dir + "\\" + updateFiles[u].Remove(0, path.Length + dir.Length));
                                }
                            }
                        }
                    } //end outer 'for'
                }//end foreach

                bwUpdateFiles.ReportProgress(4);
                //copying over the windows only stuff
                //because we can only do one pattern at a time, going to have some copypasta code
                path = path + "Windows\\";
                updateFiles = Directory.GetFiles(path, "*.exe");
                currentFiles = Directory.GetFiles(attributes.exeDirectory, "*.exe");
                //go through all of the files
                for (int u = 0; u < updateFiles.Length; u++)
                {
                    for (int c = 0; c < currentFiles.Length; c++)
                    {
                        string a = updateFiles[u].Remove(0, path.Length);
                        string b = currentFiles[c].Remove(0, attributes.exeDirectory.Length);
                        //if the files are the same
                        if (updateFiles[u].Remove(0, path.Length) == currentFiles[c].Remove(0, attributes.exeDirectory.Length))
                        {
                            if (attributes.createBackup)
                                backup.AddFile(currentFiles[c]);

                            File.Delete(currentFiles[c]); //delete the file, so we don't get errors from the next line
                            File.Copy(updateFiles[u], currentFiles[c]);
                        }
                        else //it is a new file
                        {
                            if (!File.Exists(attributes.exeDirectory + updateFiles[u].Remove(0, path.Length)))
                            {
                                //                                                              The name of the file
                                File.Copy(updateFiles[u], attributes.exeDirectory + updateFiles[u].Remove(0, path.Length));
                            }
                        }
                    }
                } //end outer 'for'

                //I feel really bad for copypasting a huge block. 
                updateFiles = Directory.GetFiles(path, "*.dll");
                currentFiles = Directory.GetFiles(attributes.exeDirectory, "*.dll");
                //go through all of the files
                for (int u = 0; u < updateFiles.Length; u++)
                {
                    for (int c = 0; c < currentFiles.Length; c++)
                    {
                        //string a = updateFiles[u].Remove(0, path.Length); //for debug
                        //string b = currentFiles[c].Remove(0, attributes.exeDirectory.Length);
                        //if the files are the same
                        if (updateFiles[u].Remove(0, path.Length) == currentFiles[c].Remove(0, attributes.exeDirectory.Length))
                        {
                            if (attributes.createBackup)
                                backup.AddFile(currentFiles[c]);

                            File.Delete(currentFiles[c]); //delete the file, so we don't get errors from the next line
                            File.Copy(updateFiles[u], currentFiles[c]);
                        }
                        else //it is a new file
                        {
                            if (!File.Exists(attributes.exeDirectory + updateFiles[u].Remove(0, path.Length)))
                            {
                                //                                                              The name of the file
                                File.Copy(updateFiles[u], attributes.exeDirectory + updateFiles[u].Remove(0, path.Length));
                            }
                        }
                    }
                } //end outer 'for'

                if (attributes.createBackup)
                {
                    bwUpdateFiles.ReportProgress(5);
                    backup.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("something happened in the ReplaceFiles function\n" + ex.Message);
            }
        } //end ReplaceFiles


        private void DownloadUpdateFiles()
        {
            //get a just the number of the current version
            currentVersion = GetCurrentVersion();
            int current = int.Parse(currentVersion.Remove(0,1)); //TryParse maybe?
            if (current == latestVersion)
            {
                MessageBox.Show("You are already at the newest version.");
                toggleDownloadOptions(true);
                cboxHaveUpdate.Enabled = true;
                return;
            }

            clients = new ArrayList();
            try
            {
                for (int i = ++current; i <= latestVersion; i++)
                {
                    WebClient wclient = new WebClient();
                    wclient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                    wclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);

                    clients.Add(new Download(wclient, i));
                }

                lstDownloadProgress.Enabled = true;
                lblIndividualDownloadProgress.Enabled = true;
                sstriplblStatus.Text = "Downloading updates";
                foreach (Download dl in clients)
                {
                    dl.wclient.BaseAddress = updateURL + "a" + dl.alpha + ".zip";
                    //because DlFileAsync combines BaseAddress with the adress given, I can create an empty uri
                    dl.wclient.DownloadFileAsync(new Uri("", UriKind.Relative), @attributes.updateDirectory + "\\a" + dl.alpha + ".zip");
                    lstDownloadProgress.Items.Add(dl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something happened in the DownloadUpdateFiles function.\n" +
                    ex.Message);
            }
        } //end DownloadUpdateFiles


        ///<summary>Helper function for DownloadUpdateFiles. It gets the latest alpha version minus the 'a' and '.zip'</summary>
        private int FindLatestVersion(int current) 
        {
            int latest = current;

            //check for a httprequest from an incremental number starting from the current version
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(updateURL + "a" + latest + ".zip"); //I'm going to assume that the current version is there
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                while (res.StatusCode == HttpStatusCode.OK)
                {
                    req.Abort(); //so we start with new streams
                    res.Close();
                    req = (HttpWebRequest)WebRequest.Create(updateURL + "a" + (++latest) + ".zip");
                    sstripInfo.Text = "Requesting status of " + updateURL + "a" + latest + ".zip";
                    res = (HttpWebResponse)req.GetResponse();
                }
                return latest -= 1;
            }
            catch (Exception ex) //comes here when the file doesn't exist on the site.
            {
                //do some extra error checking. for now, I'm going to assume that everything worked fine
                sstripInfo.Text = "Idle";
                return latest -= 1;

            }
        }


        /// <summary>Updates the download visuals (progress bar, text %, etc.)</summary>
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int index = GetDownloadFromWebClient((WebClient)sender);
            Download dld = (Download)clients[index]; //is this creating a copy?
            if (!dld.accountedFor)
            {
                dld.totalSize = e.TotalBytesToReceive;
                totalUpdateSize += e.TotalBytesToReceive;
                dld.accountedFor = true;
            }
            dld.completed = e.BytesReceived;
            clients[index] = dld; // put the copy into the array

            long tempTotal = 0;
            lstDownloadProgress.Items.Clear();
            //Should I make this next part thread safe? It isn't really an exact science, so I don't think it matters
            foreach (Download dl in clients)
            {
                tempTotal += dl.completed;
                lstDownloadProgress.Items.Add(dl);
            }

            totalUpdateRecieved = tempTotal;
            pbarDownload.Value = (int)(((double)totalUpdateRecieved / (double)totalUpdateSize) * 100);
            lblDownloadProgress.Text = "" + (int)(((double)totalUpdateRecieved / (double)totalUpdateSize) * 100) + "%";
        }


        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled) //don't do anything if the operation was cancelled
                return;

            //visual stuff
            int index = GetDownloadFromWebClient((WebClient)sender);
            Download dl = (Download)clients[index];            
            lstUpdates.Items.Add("a" + dl.alpha + ".zip");
            
            if (lstUpdates.Items.Count == clients.Count) //if all of the files have finished downloading
            {
                canApplyUpdate = true;
                if (attributes.downloadOption == "Download")
                {
                    toggleDownloadOptions(true);
                    cboxHaveUpdate.Enabled = true;
                }
                if (attributes.downloadOption == "Download and Update")
                {
                    DoUpdateFiles();
                }
            }

        }

        /// <summary> Reads the version.xml file to determine the alpha version</summary>
        /// <returns>A string value in the format of a###</returns>
        private string GetCurrentVersion()
        {
            string line;
            string[] data;
            char[] delim = { '<', '>' };
            try
            {
                if (!File.Exists(attributes.exeDirectory + @"Data\version.xml"))
                    return null;

                StreamReader sread = new StreamReader(attributes.exeDirectory + @"Data\version.xml");
                while (!sread.EndOfStream)
                {
                    line = sread.ReadLine();
                    data = line.Split(delim);

                    if (data[1] == "shortname")
                    {
                        sread.Close();
                        return data[2];
                    }
                }
                sread.Close();

                return null;
            }
            catch (FileNotFoundException fnfex)
            {
                MessageBox.Show("Error in the GetCurrentVersion function.\n" +
                    "The version.xml file could not be found, so the rest of the program will likely not work. " +
                    "Please make sure that folder where Overgrowth.exe is in has a /Data/version.xml file.\n " +
                    ".Net error message: " + fnfex.Message);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something happened in the GetCurrentVersion function.\n" + ex.Message);
                return null;
                //TODO: Catch some shit
            }
        } //end GetCurrentVersion


        ///<summary>Returns an index obtained from the clients list by comparing base addresses</summary>
        private int GetDownloadFromWebClient(WebClient wclient)
        {
            if (clients.Count == 0)
                throw new IndexOutOfRangeException("There are no elements in the client list");
            else if (clients.Count == 1)
                return 0;
            else
            {
                Download dl;
                for (int i = 0; i < clients.Count; i++)
                {
                    dl = (Download)clients[i];
                    if (dl.wclient.BaseAddress == wclient.BaseAddress)
                        return i;
                }
            }
            throw new KeyNotFoundException("The Web Client given wasn't in the clients list");
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form abt = new About();
            abt.Show(this);            
        }

        private void createBackupToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            attributes.createBackup = createBackupToolStripMenuItem.Checked;
        }


        //Deletes the files, then the directories. You can not delete non-empty directories.
        private void cleanUpdatesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string[] files = Directory.GetFiles(attributes.updateDirectory, "*", SearchOption.AllDirectories);
                string[] dirs = Directory.GetDirectories(attributes.updateDirectory);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    Directory.Delete(dir, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the CleanUpdatesFolder function.\n" + ex.Message +
                    "\n The function might have properly run. You can check your update folder to see if it did.");
            }
        }


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
    } //end partial class
} //end namespace
