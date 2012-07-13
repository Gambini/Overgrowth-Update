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
        private void DownloadUpdateFiles()
        {
            //get a just the number of the current version
            currentVersion = GetCurrentVersion();
            int current = int.Parse(currentVersion.Remove(0, 1)); //TryParse maybe?
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
                    dl.wclient.BaseAddress = updateURL + dl.alphaFileName + ".zip";
                    //So we don't make the user re-download every update.
                    if (File.Exists(@attributes.updateDirectory + "\\" + dl.alphaFileName + ".zip"))
                    {
                        //'trick' it to think that the download is completed. This will make the items be out of order, more than likely
                        DownloadFileCompleted(dl.wclient, new AsyncCompletedEventArgs(new Exception(), false, new object()));
                    }
                    else
                    {
                        //because DlFileAsync combines BaseAddress with the adress given, I can create an empty uri
                        dl.wclient.DownloadFileAsync(new Uri("", UriKind.Relative), @attributes.updateDirectory + "\\" + dl.alphaFileName + ".zip");
                        lstDownloadProgress.Items.Add(dl);
                    }
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
            string latestURL = ""; //this will be the full url, and naming scheme for the .zip file

            //check for a httprequest from an incremental number starting from the current version
            try
            {
                latestURL = updateURL + ComposeAlphaFileName(latest) + ".zip";
                HttpWebRequest req  = (HttpWebRequest)WebRequest.Create(latestURL);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                while (res.StatusCode == HttpStatusCode.OK)
                {
                    req.Abort(); //so we start with new streams
                    res.Close();
                    latest += 1; //set up checking for the next version up
                    latestURL = updateURL + ComposeAlphaFileName(latest) + ".zip";
                    req = (HttpWebRequest)WebRequest.Create(latestURL);
                    res = (HttpWebResponse)req.GetResponse();
                }
                return latest -= 1;
            }
            catch (Exception ex) //comes here when the file doesn't exist on the site.
            {
                //do some extra error checking. for now, I'm going to assume that everything worked fine
                sstriplblStatus.Text = "Idle";
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
            lstUpdates.Items.Add(dl.alphaFileName);

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
    }
}
