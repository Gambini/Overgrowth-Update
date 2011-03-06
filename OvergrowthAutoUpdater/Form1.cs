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

///<summary>   </summary>
namespace OvergrowthAutoUpdater
{
    struct Download
    {
        public long totalSize;
        public long completed;
        public WebClient wclient;
        public bool accountedFor;
        public int alpha;

        public Download(WebClient wc, int a)
        { wclient = wc; totalSize = 0; completed = 0; accountedFor = false; alpha = a; }

        public override string ToString()
        {
            int percent;
            if (totalSize == 0) percent = 0;
            else percent = (int)(((double)completed / (double)totalSize) * 100);
            return "a" + alpha + ".zip.... " + percent + "%";
        }
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
        public string currentVersion; // = "a118";
        ///<summary>The latest version available to download.</summary>
        public int latestVersion;
        ////<summary>A variable used if the sequential download option is true</summary>
        //private bool canDownloadNextFile = true;
        /// <summary>Key is the version of the alpha minus 'a', value is of "Download" type</summary> 
        //public SortedList clients;
        public ArrayList clients;
        public long totalUpdateSize;
        public long totalUpdateRecieved;
        /// <summary>true if the program is allowed to apply the update</summary>
        public bool canApplyUpdate = false;


        public frmMain()
        {
            InitializeComponent();
            
            firstTime = false;
            //updateSize = new SortedList();
            totalUpdateSize = 0;
            totalUpdateRecieved = 0;
        }


        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            //I'll delete this later
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
            }
            cboxHaveUpdate.Checked = attributes.hasUpdateFiles;
            createBackupToolStripMenuItem.Checked = attributes.createBackup;
            downloadSequentiallyToolStripMenuItem.Checked = attributes.sequentialDownload;
            

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
                //TODO: Catch some shit.
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
                //TODO: Catch lots of shit
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


        private void txtUpdateDir_TextChanged(object sender, EventArgs e)
        {
            //Delete this later
        }


        //I think I want this instead of "TextChanged"
        private void txtUpdateDir_Leave(object sender, EventArgs e)
        {
            if (firstTime) return; //Don't do anything if it is the initial start up

            
            if (!Directory.Exists(txtUpdateDir.Text))
            {
                DialogResult result = MessageBox.Show("The directory " + attributes.updateDirectory + " does not currently exist. Would you like to make it?",
                    "Directory does not exist", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
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
                txtExeDir.Text = "not an exe file";
        }


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
                //Do I want to do this?
                //txtUpdateDir_Leave(sender, e); //I hope it works passing the same parameters as this function recieved.
                
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
            toggleDownloadOptions(false);
            cboxHaveUpdate.Enabled = false;

            latestVersion = FindLatestVersion(int.Parse(GetCurrentVersion().Remove(0,1)));

            if (rbtnDownload.Checked || rbtnDownloadAndUpdate.Checked)
                DownloadUpdateFiles();
            if (rbtnUpdate.Checked || rbtnDownloadAndUpdate.Checked)
                DoUpdateFiles();


            lblCurrentVersion.Text = "Current version: " + GetCurrentVersion();
            toggleDownloadOptions(true);
            cboxHaveUpdate.Enabled = true;

        }

        private void toggleDownloadOptions(bool enabled)
        {
            //so the user doesn't change the options in the middle of the process
            rbtnDownload.Enabled = enabled;
            rbtnDownloadAndUpdate.Enabled = enabled;
            rbtnUpdate.Enabled = enabled;
        }


        private void DoUpdateFiles()
        {
            if (!canApplyUpdate)
                return;
            int next = 0; //the alpha version we are looking to update to
            currentVersion = GetCurrentVersion(); //so we have the most up to date version
            int current = int.Parse(currentVersion.Remove(0,1));

            updateZips = Directory.GetFiles(attributes.updateDirectory + "\\", "a*.zip");
            if (updateZips.Length == 0) return; //don't put anything in the list box if no update files are there
            for (int i = 0; i < updateZips.Length; i++)
            {
                next = current += 1;
                if (File.Exists(attributes.updateDirectory + "\\a" + next + ".zip"))
                    if (next <= latestVersion)
                        ReplaceFiles(attributes.updateDirectory + "\\a" + next + ".zip");
                    else
                        return;
                else
                {
                    MessageBox.Show("Error in DoUpdateFiles function.\n" +
                        "Could not find update file " + attributes.updateDirectory + "a" + next + ".zip\n" +
                        "You will be at version a" + (next - 1) + " until the update file for a" + next + " is found.");
                    return;
                }
            }
        }


        /// <summary>Does the actual file operations to apply the update</summary>
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
                //go through each of the directories and compare them. Mightily ineffiecient, but effective
                for (int u = 0; u < updateDirectories.Length; u++)
                {
                    for (int c = 0; c < currentDirectories.Length; c++)
                    {
                        string b = updateDirectories[u].Remove(0, path.Length);
                        string d = currentDirectories[c].Remove(0, attributes.exeDirectory.Length);
                        if (updateDirectories[u].Remove(0, path.Length) == currentDirectories[c].Remove(0, attributes.exeDirectory.Length))
                            commonDirectories.Add(updateDirectories[u].Remove(0, path.Length));//does not end with \\
                    }
                }

                if (attributes.createBackup)
                    backup = new ZipFile(path + "backupfiles.zip");
                else backup = new ZipFile();//not going to use it

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
                            string a = updateFiles[u].Remove(0, path.Length + dir.Length);
                            string b = currentFiles[c].Remove(0, attributes.exeDirectory.Length + dir.Length);
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
                                if(!File.Exists(updateFiles[u]))
                                {
                                    //                                                              The name of the file
                                    File.Copy(updateFiles[u], attributes.exeDirectory + dir + "\\" + updateFiles[u].Remove(0, path.Length + dir.Length));
                                }
                            }
                        }
                    } //end outer 'for'
                }//end foreach

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
                            if (!File.Exists(updateFiles[u]))
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
                            if (!File.Exists(updateFiles[u]))
                            {
                                //                                                              The name of the file
                                File.Copy(updateFiles[u], attributes.exeDirectory + updateFiles[u].Remove(0, path.Length));
                            }
                        }
                    }
                } //end outer 'for'

                if (attributes.createBackup)
                    backup.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("something happened in the ReplaceFiles function\n" + ex.Message);
            }
        } //end ReplaceFiles


        private void DownloadUpdateFiles()
        {
            //get a just the number of the current version
            //temporary for now. Make this not have to be done in two places
            currentVersion = GetCurrentVersion();
            string tempCurrent = currentVersion;
            tempCurrent = tempCurrent.Remove(0, 1); //shoud just remove the'a'
            int current = int.Parse(tempCurrent); //TryParse maybe?
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
                    req.Abort();
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
            clients[index] = dld; // if it is making a copy, then put the copy into the array

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
                toggleDownloadOptions(true);
                cboxHaveUpdate.Enabled = true;
                canApplyUpdate = true;
                if( attributes.downloadOption == "Download and Update" )
                    DoUpdateFiles();
            }

        }


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


        ///<summary>Returns an index obtained from the clients list</summary>
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
    } //end partial class
} //end namespace
