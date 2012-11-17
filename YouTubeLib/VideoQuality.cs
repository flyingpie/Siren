using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeLib
{
    public class VideoQuality
    {
        public VideoQuality()
        {

        }

        public String VideoTitle { get; set; }

        public String Extension { get; set; }

        public String DownloadUrl { get; set; }

        public String VideoUrl { get; set; }

        public Size Dimension { get; set; }

        public void SetQuality(String extension, Size dimension)
        {
            Extension = extension;
            Dimension = dimension;
        }
    }
}
