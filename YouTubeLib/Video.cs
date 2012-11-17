using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeLib
{
    public class Video
    {
        private List<VideoQuality> qualityEntries;

        public Video()
        {
            
        }

        public String Title { get; set; }

        public String Link { get; set; }

        public int ViewCount { get; set; }

        public List<VideoQuality> QualityEntries { get { return qualityEntries; } }

        public void LoadQualityEntires()
        {
            qualityEntries = new List<VideoQuality>();
            qualityEntries = Query.GetYouTubeVideoUrls(new string[] { String.Format("http://www.youtube.com{0}", Link) });
        }

        public VideoQuality GetBestQuality()
        {
            if (qualityEntries == null)
            {
                LoadQualityEntires();
            }

            return qualityEntries.OrderByDescending(e => e.Dimension.Width * e.Dimension.Height).FirstOrDefault();
        }
    }
}
