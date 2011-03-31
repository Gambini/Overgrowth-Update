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
    public partial class frmMain
    {
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
                    switch (data[0])
                    {
                        case "updateDirectory":
                            if (data[1] != "") //the default value is the current directory\Updates
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
                        case "logging":
                            attributes.logging = bool.Parse(data[1]);
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


        private void DoUpdateFiles()
        {
            if (!canApplyUpdate)
                return;

            sstriplblStatus.Text = "Applying Updates";
            int next = 0; //the alpha version we are looking to update to
            currentVersion = GetCurrentVersion(); //so we have the most up to date version
            int current = int.Parse(currentVersion.Remove(0, 1));

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
                //will create a log file regardless of if the user selected logging
                StreamWriter sw = new StreamWriter("log.txt", true);
                if (attributes.logging)  //will only ever write to the log if the user selected logging
                    sw.WriteLine("\n\nLog " + DateTime.Now.ToLongDateString() + " from " + GetCurrentVersion() + " to the next.");

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
                        {
                            commonDirectories.Add(updateDirectories[u].Remove(0, path.Length));//does not end with \\
                            writeLog("Updated Directory " + updateDirectories[u].Remove(0, path.Length), sw);
                        }
                        else
                        {
                            if (!Directory.Exists(attributes.exeDirectory + updateDirectories[u].Remove(0, path.Length)))
                            {
                                Directory.CreateDirectory(attributes.exeDirectory + updateDirectories[u].Remove(0, path.Length));
                                commonDirectories.Add(updateDirectories[u].Remove(0, path.Length));
                                writeLog("New Directory " + updateDirectories[u].Remove(0, path.Length), sw);
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
                                writeLog("Copy file " + updateFiles[u] + " to " + currentFiles[c], sw);
                            }
                            else //it is a new file
                            {
                                //we need this check because it will keep on iterating through the non-common files
                                if (!File.Exists(attributes.exeDirectory + dir + "\\" + updateFiles[u].Remove(0, path.Length + dir.Length)))
                                {
                                    //                                                              The name of the file
                                    File.Copy(updateFiles[u], attributes.exeDirectory + dir + "\\" + updateFiles[u].Remove(0, path.Length + dir.Length));
                                    writeLog("New file " + updateFiles[u] + " to " + attributes.exeDirectory + dir + "\\" + updateFiles[u].Remove(0, path.Length + dir.Length), sw);
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
                        //string a = updateFiles[u].Remove(0, path.Length);
                        //string b = currentFiles[c].Remove(0, attributes.exeDirectory.Length);
                        //if the files are the same
                        if (updateFiles[u].Remove(0, path.Length) == currentFiles[c].Remove(0, attributes.exeDirectory.Length))
                        {
                            if (attributes.createBackup)
                                backup.AddFile(currentFiles[c]);

                            File.Delete(currentFiles[c]); //delete the file, so we don't get errors from the next line
                            File.Copy(updateFiles[u], currentFiles[c]);
                            writeLog("Copy file " + updateFiles[u] + " to " + currentFiles[c], sw);
                        }
                        else //it is a new file
                        {
                            if (!File.Exists(attributes.exeDirectory + updateFiles[u].Remove(0, path.Length)))
                            {
                                //                                                              The name of the file
                                File.Copy(updateFiles[u], attributes.exeDirectory + updateFiles[u].Remove(0, path.Length));
                                writeLog("New file " + updateFiles[u] + " to " + attributes.exeDirectory + updateFiles[u].Remove(0, path.Length), sw);
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
                            writeLog("Copy file " + updateFiles[u] + " to " + currentFiles[c], sw);
                        }
                        else //it is a new file
                        {
                            if (!File.Exists(attributes.exeDirectory + updateFiles[u].Remove(0, path.Length)))
                            {
                                //                                                              The name of the file
                                File.Copy(updateFiles[u], attributes.exeDirectory + updateFiles[u].Remove(0, path.Length));
                                writeLog("New file " + updateFiles[u] + " to " + attributes.exeDirectory + updateFiles[u].Remove(0, path.Length), sw);
                            }
                        }
                    }
                } //end outer 'for'

                if (attributes.createBackup)
                {
                    bwUpdateFiles.ReportProgress(5);
                    backup.Save();
                }

                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("something happened in the ReplaceFiles function\n" + ex.Message);
            }
        } //end ReplaceFiles


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


        private void writeLog(string line, StreamWriter sw)
        {
            if (attributes.logging)
            {
                sw.WriteLine(line);
            }

        }
    }//end partial class
}//end namespace
