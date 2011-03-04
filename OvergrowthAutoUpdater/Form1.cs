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

///<summary>   </summary>
namespace OvergrowthAutoUpdater
{
    public partial class frmMain : Form
    {

        /// <summary>The base URL for the update file. Does not include the "a###.zip" part. </summary>
        public static string updateURL = "http://cdn.wolfire.com/alpha/diffs/";
        ///<summary>The .txt file where all of the config options are stored.</summary>
        public static string configPath = Directory.GetCurrentDirectory() + "config.txt";
        ///<summary>An object holding all of the option values.</summary>
        public ConfigAtrributes attributes;
        ///<summary>The file names for the update zip files.</summary>
        public string[] updateZips;
        ///<summary>True if it is the first time the form is loaded. Used so message boxes don't pop up.</summary>
        public bool firstTime = true;



        public frmMain()
        {
            InitializeComponent();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            //I'll delete this later
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadConfigFile(configPath);
            txtUpdateDir.Text = attributes.updateDirectory;
            firstTime = false;
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
                        case "updatePath":
                            if( data[1] != "") //the default value is the current directory\Updates
                                attributes.updateDirectory = data[1];
                            break;
                        case "downloadOption":
                            attributes.downloadOption = data[1];
                            break;
                        default:
                            break;
                    } //end switch
                } //end while
                
            }
            catch (Exception ex)
            { 
                //TODO: Catch some shit.
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
            }
            
        }


        private void rbtnDownload_CheckedChanged(object sender, EventArgs e)
        {
            attributes.downloadOption = "Download";
        }

        private void rbtnUpdate_CheckedChanged(object sender, EventArgs e)
        {
            attributes.downloadOption = "Update";
        }

        private void rbtnDownloadAndUpdate_CheckedChanged(object sender, EventArgs e)
        {
            attributes.downloadOption = "DownloadAndUpdate";
        }


        private void txtUpdateDir_TextChanged(object sender, EventArgs e)
        {

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
        } //end txtUpdateDir_Leave

    } //end partial class
} //end namespace
