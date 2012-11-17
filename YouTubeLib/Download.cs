using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTubeLib
{
    public class Download
    {
        public enum DownloadState
        {
            None,
            Downloading,
            Paused,
            Success,
            Failed,
            Canceled
        }

        private static String InvalidCharacters = new String(Path.GetInvalidFileNameChars()) + new String(Path.GetInvalidPathChars());

        public event EventHandler<DownloadEventArgs> DownloadStarted = delegate { };
        public event EventHandler<DownloadEventArgs> DownloadProgress = delegate { };
        public event EventHandler<DownloadEventArgs> DownloadCompleted = delegate { };
        public event EventHandler<DownloadEventArgs> DownloadFailed = delegate { };

        private Video video;
        private String filename;

        public Download(Video video)
        {
            this.video = video;
            this.filename = ConvertTitleToFilename(video.Title);
        }

        public Video Video { get { return video; } }
        public String Filename { get { return filename; } }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Pause()
        {

        }

        public static String ConvertTitleToFilename(String title)
        {
            foreach (char invalidChar in InvalidCharacters)
            {
                title = title.Replace(invalidChar, ' ');
            }

            // Strip any double spaces
            title = Regex.Replace(title, @"\s+", " ");

            return title;
        }
    }
}
