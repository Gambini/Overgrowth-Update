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
        //default constructor
        //public Download() { totalSize = 0; completed = 0; }
        public Download(WebClient wc, int a)
        { wclient = wc; totalSize = 0; completed = 0; accountedFor = false; alpha = a; }
        public override string ToString()
        {
            int percent;
            if (totalSize == 0) percent = 0;
            else percent = (int)((double)(completed / totalSize)*100);
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
        ///<summary>A variable used if the sequential download option is true</summary>
        private bool canDownloadNextFile = true;
        /// <summary> Holds a list of keys that have been taken into account for the total size
        public ArrayList keys;
        /// <summary>Key is the version of the alpha minus 'a', value is of "Download" type</summary> 
        //public SortedList clients;
        public ArrayList clients;
        public long totalUpdateSize;
        public long totalUpdateRecieved;


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
            if (attributes.exeDirectory != "") txtExeDir.Text = attributes.exeDirectory + "Overgrowth.exe";
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
                MessageBox.Show(ex.Message);
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
                UpdateListBox();
            }
            else
            {
                rbtnDownload.Enabled = true;
                rbtnDownloadAndUpdate.Enabled = true;
                rbtnDownloadAndUpdate.Checked = true;
                rbtnUpdate.Enabled = false;
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
                    lstUpdates.Items.AddRange(updateZips); //I hope this is in order.
                }
            }
            catch (Exception ex)
            {
                //TODO: Catch lots of shit
                MessageBox.Show("Something happened in the updateListBox function");
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
        }


        ///<summary>This is the "Do everything with given info" button.</summary>
        private void btnDoUpdate_Click(object sender, EventArgs e)
        {
            //so the user doesn't change the options in the middle of the process
            rbtnDownload.Enabled = false;
            rbtnDownloadAndUpdate.Enabled = false;
            rbtnUpdate.Enabled = false;

            if (rbtnDownload.Checked || rbtnDownloadAndUpdate.Checked)
                DownloadUpdateFiles();
            if (rbtnUpdate.Checked || rbtnDownloadAndUpdate.Checked)
                DoUpdateFiles();

        }


        private void DoUpdateFiles()
        {
            //hfpaeifa
        }


        private void DownloadUpdateFiles()
        {
            //get a just the number of the current version
            //temporary for now. Make this not have to be done in two places
            currentVersion = GetCurrentVersion();
            string tempCurrent = currentVersion;
            tempCurrent = tempCurrent.Remove(0, 1); //shoud just remove the'a'
            int current = int.Parse(tempCurrent); //TryParse maybe?
            latestVersion = FindLatestVersion(current);
            if (current == latestVersion)
            {
                MessageBox.Show("You are already at the newest version.");
                return;
            }

            //clients = new SortedList();
            clients = new ArrayList();
            try
            {
                for (int i = ++current; i <= latestVersion; i++)
                {
                    WebClient wclient = new WebClient();
                    wclient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                    wclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);

                    
                    //clients.Add(i, new Download(wclient, i));
                    clients.Add(new Download(wclient, i));
                }

                lstDownloadProgress.Enabled = true;
                lblIndividualDownloadProgress.Enabled = true;
                sstriplblStatus.Text = "Downloading updates";
                foreach (Download dl in clients)
                {
                    //Download dl = (Download)de.Value;
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
            int latest = 115;//current;

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
                return latest -= 1; //I think it is -=1. 
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
            //Download dld = (Download)clients.GetByIndex(index); //is this making a copy of the object?
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
            pbarDownload.Increment((int)((double)(totalUpdateRecieved / totalUpdateSize)*100));
            lblDownloadProgress.Text = "" + (int)((double)(totalUpdateRecieved / totalUpdateSize)*100) + "%";
            //lstDownloadProgress.Refresh(); //so we get updated percentages in real time
        }


        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

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
                    data = line.Split( delim );

                    if (data[1] == "shortname")
                        return data[2];
                }
                sread.Close();

                return null;                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something happened in the GetCurrentVersion function"); 
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
