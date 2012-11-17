using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeLib
{
    public class DownloadEventArgs : EventArgs
    {
        private Download download;

        public DownloadEventArgs(Download download)
        {
            this.download = download;
        }

        public Download Download { get { return download; } }
    }
}
