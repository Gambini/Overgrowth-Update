﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OvergrowthAutoUpdater
{
    ///<summary>Kinda like a struct for config options.</summary>
    public class ConfigAtrributes
    {
        public ConfigAtrributes()
        {
            updateDirectory = Directory.GetCurrentDirectory() + "Updates\\";
            exeDirectory = "";
            downloadOption = "Download and Update";
            hasUpdateFiles = false;
            createBackup = true;
            sequentialDownload = false;
        }

        /// <summary> The directory that holds Overgrowth.exe. Not the .exe itself. </summary>
        public string exeDirectory;
        ///<summary>The directory where all of the update .zip files are stored. </summary>
        public string updateDirectory;
        ///<summary>Possible values: Download, Update, Download and Update, corresponds to the download options on the form.</summary>
        public string downloadOption;
        ///<summary>If the check box for having update files already downloaded is/was checked.</summary>
        public bool hasUpdateFiles;
        ///<summary> If the user would like to create backup copies of the files to be updated. In the options dropdown menu.</summary>
        public bool createBackup;
        ///<summary>If the user wants us to download everything one at a time and in a row. In the options dropdown menu.</summary>
        public bool sequentialDownload;


        ///<summary>Writes the member data of the object to the config file. Uses same name as the member data as an identifier.</summary>
        ///<returns>Returns true if the write was sucessful, otherwise returns false</returns>
        public bool WriteToFile(string configPath)
        {
            try
            {
                if (!File.Exists(configPath))
                {
                    return false;
                }

                //do I really have to do all of this manually?
                StreamWriter swrite = new StreamWriter(configPath,false); //overwrite the entire file
                swrite.WriteLine("exeDirectory=" + exeDirectory);
                swrite.WriteLine("updateDirectory=" + updateDirectory);
                swrite.WriteLine("downloadOption=" + downloadOption);
                swrite.WriteLine("hasUpdateFiles=" + hasUpdateFiles.ToString());
                swrite.WriteLine("createBackup=" + createBackup.ToString());
                swrite.WriteLine("sequentialDownload=" + sequentialDownload.ToString());
                swrite.Close();
                return true;
            }
            catch( Exception ex )
            {
                return false;
                
            }
        } //end WriteToFile
    }
}
