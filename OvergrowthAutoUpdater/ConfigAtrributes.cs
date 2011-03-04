using System;
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
            downloadOption = "DownloadAndUpdate";
        }

        /// <summary> The directory that holds Overgrowth.exe. </summary>
        public string exeDirectory;
        ///<summary>The directory where all of the update .zip files are stored. </summary>
        public string updateDirectory;
        ///<summary>Possible values: Download, Update, DownloadAndUpdate, corresponds to the download options on the form.</summary>
        public string downloadOption;

    }
}
