using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace OvergrowthAutoUpdater
{
    struct Download
    {
        /// <summary>The total amount of bytes we expect to download for this file</summary>
        public long totalSize;
        /// <summary>The total amount of bytes we have downloaded for this file</summary>
        public long completed;
        /// <summary>The WebClient responsible for downloading the file</summary>
        public WebClient wclient;
        /// <summary>Used to make the total update size for all of the downloads accurate</summary>
        public bool accountedFor;
        /// <summary> The version that it is downloading, minus the 'a' </summary>
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
}
