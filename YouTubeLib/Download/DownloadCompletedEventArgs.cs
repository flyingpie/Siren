using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouTubeLib.Download
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public string FileName { get; set; }
    }
}
