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
            string uncompressedDirectory = path.Remove(path.IndexOf(".zip"));
            string salpha = path.Remove(0, path.IndexOf(".zip") - 4); //only used on the next 2 lines, gets a###.zip
            salpha = salpha.Remove(salpha.IndexOf(".zip")); //gets a###
            int alpha = int.Parse(salpha.Remove(0,1)); //this should get the number

            using (ZipFile zip = ZipFile.Read(path))
            {
                bwUpdateFiles.ReportProgress(1);
                if (Directory.Exists(uncompressedDirectory))
                    CleanUpdatesFolder(uncompressedDirectory, true);
                zip.ExtractAll(uncompressedDirectory);
                zip.Dispose();
            }

            path = path.Remove(path.IndexOf(".zip")) + "\\update\\";

            try
            {
                //will create a log file regardless of if the user selected logging
                StreamWriter sw = new StreamWriter("log.txt", true);
                if (attributes.logging)  //will only ever write to the log if the user selected logging
                    sw.WriteLine("\n\nLog " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLocalTime() + " from " + GetCurrentVersion() + " to a" + alpha);

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
                {
                    string backupPath = attributes.updateDirectory + "\\backupfiles" + alpha + ".zip";
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                    backup = new ZipFile(backupPath);
                } 
                else backup = new ZipFile();//not going to use it

                bwUpdateFiles.ReportProgress(3);
                foreach (string dir in commonDirectories)
                {
                    //no more clouding up the install folder with useless folders
                    if (dir == "Mac" || dir == "Windows") continue;

                    if (attributes.createBackup)
                        backup.AddDirectoryByName(dir);

                    updateFiles = Directory.GetFiles(path + dir + "\\");
                    currentFiles = Directory.GetFiles(attributes.exeDirectory + dir + "\\");

                    //go through all of the files
                    for (int u = 0; u < updateFiles.Length; u++)
                    {
                        for (int c = 0; c < currentFiles.Length; c++)
                        {
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

                        if (currentFiles.Length == 0) //It is an empty folder, so the inner 'for' loop will not run
                        {
                            File.Copy(updateFiles[u], attributes.exeDirectory + dir + "\\" + updateFiles[u].Remove(0, path.Length + dir.Length));
                            writeLog("New file " + updateFiles[u] + " to " + attributes.exeDirectory + dir + "\\" + updateFiles[u].Remove(0, path.Length + dir.Length), sw);
                        }
                    } //end outer 'for'
                }//end foreach

                bwUpdateFiles.ReportProgress(4);
                //copying over the windows only stuff
                //I'm just going to copy all of the files over, just in case the user likes to debug stuff
                path = path + "Windows\\";
                updateFiles = Directory.GetFiles(path);
                currentFiles = Directory.GetFiles(attributes.exeDirectory); //this should never be empty
                //go through all of the files
                for (int u = 0; u < updateFiles.Length; u++)
                {
                    for (int c = 0; c < currentFiles.Length; c++)
                    {
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
                CleanUpdatesFolder(uncompressedDirectory, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("something happened in the ReplaceFiles function\n" + ex.Message);
            }
        } //end ReplaceFiles


        ///<summary>Deletes everything from the given directory.</summary>
        ///<param name="directory"> The directory to delete from.</param>
        ///<param name="deleteParameterDirectory"> If we should delete the directory that we passed as well. </param>
        private void CleanUpdatesFolder(string directory, bool deleteParameterDirectory)
        {
            bwDeleteFiles.ReportProgress(0);
            try
            {
                string[] files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                string[] dirs = Directory.GetDirectories(directory);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    Directory.Delete(dir, true);
                }

                //An example of when we don't want to delete it is if we pass the update directory.
                if (deleteParameterDirectory)
                {
                    Directory.Delete(directory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the CleanUpdatesFolder function.\n" + ex.Message);
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
                    throw new FileNotFoundException("version.xml does not exist");

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


        //this is poorly done and based on the version.xml not changing. However, I cannot think of a better
        //way to write just a single word in to the middle of an xml file.
        private bool ChangeCurrentVersion(int version)
        {
            sstriplblStatus.Text = "Changing version";
            if (latestVersion != 0 && version > latestVersion)
                MessageBox.Show("Cannot go past the lastest version available, which is" + latestVersion);

            string line;
            string[] data;
            char[] delim = { '<', '>' };
            bool ret = false;
            try
            {
                if (!File.Exists(attributes.exeDirectory + @"Data\version.xml"))
                    throw new FileNotFoundException("version.xml does not exist");

                StringBuilder sbuild = new StringBuilder();
                using (StreamReader sread = new StreamReader(attributes.exeDirectory + @"Data\version.xml"))
                {
                    while (!sread.EndOfStream)
                    {
                        line = sread.ReadLine();
                        data = line.Split(delim);

                        if (data.Length > 0 && data[1] == "shortname") //GetCurrentVersion only looks at the shortname property
                        {
                            sbuild.AppendLine(data[0] + "<" + data[1] + ">a" + version + "<" + data[3] + ">");
                            ret = true;
                        }
                        else
                            sbuild.AppendLine(line);
                    }
                }

                using (StreamWriter swrite = new StreamWriter(attributes.exeDirectory + @"Data\version.xml", false))
                {
                    swrite.Write(sbuild.ToString());
                }

                return ret;
            }
            catch (FileNotFoundException fnfex)
            {
                MessageBox.Show("Error in the ChangeCurrentVersion function.\n" +
                    "The version.xml file could not be found, so the rest of the program will likely not work. " +
                    "Please make sure that folder where Overgrowth.exe is in has a /Data/version.xml file.\n " +
                    ".Net error message: " + fnfex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something happened in the ChangeCurrentVersion function.\n" + ex.Message);
                return false;
            }
        }


        private void writeLog(string line, StreamWriter sw)
        {
            if (attributes.logging)
            {
                sw.WriteLine(line);
            }

        }
    }//end partial class
}//end namespace
