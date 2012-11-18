using System;
using System.Collections.Generic;
using System.Linq;

namespace YouTubeLib
{
    /// <summary>
    /// Represents a single item in a results page
    /// </summary>
    public class Video
    {
        private List<VideoQuality> qualityEntries;

        public Video()
        {
            
        }

        public String Title { get; set; }

        public String Link { get; set; }

        public List<VideoQuality> QualityEntries { get { return qualityEntries; } }

        /// <summary>
        /// Loads information about in which formats the video is available
        /// </summary>
        public void LoadQualityEntires()
        {
            qualityEntries = new List<VideoQuality>();
            qualityEntries = Query.GetYouTubeVideoUrls(String.Format("{0}{1}", Query.YouTubeBaseUrl, Link));
        }

        /// <summary>
        /// Returns the best available format
        /// </summary>
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
