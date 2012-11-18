using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTubeLib
{
    public enum DownloadStatus { None, Downloading, Paused, Success, Failed, Canceled }
    /// <summary>
    /// Downloads and resumes files from HTTP, FTP, and File (file://) URLS
    /// </summary>
    public class Download : AbortableBackgroundWorker
    {
        private static String InvalidCharacters = new String(Path.GetInvalidFileNameChars()) + new String(Path.GetInvalidPathChars());

        // Block size to download is by default 1K.
        public static int DownloadBlockSize = 1024 * 200;
        private string downloadingTo;
        public string FileUrl { get; private set; }
        public string DestFolder { get; private set; }
        public string DestFileName { get; private set; }
        /// <summary>
        /// Gets the current DownloadStatus
        /// </summary>
        public DownloadStatus DownloadStatus { get; private set; }
        /// <summary>
        /// Gets the current DownloadData
        /// </summary>
        public DownloadData DownloadData { get; private set; }
        /// <summary>
        /// Gets the current DownloadSpeed
        /// </summary>
        public int DownloadSpeed { get; private set; }
        /// <summary>
        /// Gets the estimate time to finish downloading, the time is in seconds
        /// </summary>
        public long ETA
        {
            get
            {
                if (DownloadData == null || DownloadSpeed == 0) return 0;
                long remainBytes = DownloadData.FileSize - totalDownloaded;
                return remainBytes / DownloadSpeed;
            }
        }
        public Download(string FileUrl, string DestFolder, string DestFileName)
        {
            this.FileUrl = FileUrl;
            this.DestFolder = DestFolder;
            this.DestFileName = DestFileName;
            DoWork += StartDownload;
        }

        public int Progress { get; private set; }
        /// <summary>
        /// Make the download to Pause
        /// </summary>
        public void Pause()
        {
            _pause = true;
        }
        /// <summary>
        /// Make the download to resume
        /// </summary>
        public void Resume()
        {
            _pause = false;
        }
        private bool _pause;
        static long SecondTicks = TimeSpan.FromSeconds(1).Ticks;
        FileStream fileStream;
        long totalDownloaded;
        /// <summary>
        /// Begin downloading the file at the specified url, and save it to the given folder.
        /// </summary>
        private void StartDownload(object sender, DoWorkEventArgs e)
        {
            _pause = false;
            DownloadStatus = DownloadStatus.Downloading;
            OnProgressChanged(new ProgressChangedEventArgs(Progress, null));
            DownloadData = DownloadData.Create(FileUrl, DestFolder, this.DestFileName);
            if (string.IsNullOrEmpty(DestFileName))
                Path.GetFileName(DownloadData.Response.ResponseUri.ToString());
            this.downloadingTo = Path.Combine(DestFolder, DestFileName);
            FileMode mode = DownloadData.StartPoint > 0 ? FileMode.Append : FileMode.OpenOrCreate;
            fileStream = File.Open(downloadingTo, mode, FileAccess.Write);
            byte[] buffer = new byte[DownloadBlockSize];
            totalDownloaded = DownloadData.StartPoint;
            double totalDownloadedInTime = 0; long totalDownloadedTime = 0;
            OnProgressChanged(new ProgressChangedEventArgs(Progress, null));
            bool callProgess = true;
            while (true)
            {
                callProgess = true;
                if (CancellationPending)
                { DownloadSpeed = Progress = 0; e.Cancel = true; break; }
                if (_pause) { DownloadSpeed = 0; DownloadStatus = DownloadStatus.Paused; System.Threading.Thread.Sleep(500); }
                else
                {
                    DownloadStatus = DownloadStatus.Downloading;
                    long startTime = DateTime.Now.Ticks;
                    int readCount = DownloadData.DownloadStream.Read(buffer, 0, DownloadBlockSize);
                    if (readCount == 0) break;
                    totalDownloadedInTime += readCount;
                    totalDownloadedTime += DateTime.Now.Ticks - startTime;
                    if (callProgess = totalDownloadedTime >= SecondTicks)
                    {
                        DownloadSpeed = (int)(totalDownloadedInTime / TimeSpan.FromTicks(totalDownloadedTime).TotalSeconds);
                        totalDownloadedInTime = 0; totalDownloadedTime = 0;
                    }
                    totalDownloaded += readCount;
                    fileStream.Write(buffer, 0, readCount);
                    fileStream.Flush();
                }
                Progress = (int)(100.0 * totalDownloaded / DownloadData.FileSize);
                if (callProgess && DownloadData.IsProgressKnown)
                    ReportProgress(Progress);
            }
            ReportProgress(Progress);
        }
        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            try { if (DownloadData != null) DownloadData.Close(); }
            catch { }
            try { if (fileStream != null) fileStream.Close(); }
            catch { }
            if (e.Cancelled)
                DownloadStatus = DownloadStatus.Canceled;
            else if (e.Error != null) DownloadStatus = DownloadStatus.Failed;
            else DownloadStatus = DownloadStatus.Success;
            DownloadSpeed = 0;
            base.OnRunWorkerCompleted(e);
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
