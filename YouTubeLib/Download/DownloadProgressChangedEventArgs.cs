using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouTubeLib.Download
{
    public class DownloadProgressChangedEventArgs : EventArgs
    {
        public float Percentage { get; set; }
    }
}
