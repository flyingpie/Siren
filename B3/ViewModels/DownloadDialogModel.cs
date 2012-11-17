using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VideoEncoder;
//using YouTubeDownloader;
using YouTubeLib;

namespace B3.ViewModels
{
    public class DownloadDialogModel : ViewModel
    {
        public enum ProcessState
        {
            Downloading,
            Encoding
        }

        private const String TitleDownloading = "Downloading...";
        private const String TitleEncoding = "Encoding...";

        private Download downloader;
        private Encoder encoder;

        private String title;
        private int progress;
        private int speed;
        private long eta;
        private bool active;
        private Video video;
        private ProcessState state;

        public DownloadDialogModel(Download downloader, Encoder encoder, Video video)
        {
            this.downloader = downloader;
            this.downloader.ProgressChanged += DownloaderProgressChanged;
            this.downloader.RunWorkerCompleted += DownloaderRunWorkerCompleted;

            this.encoder = encoder;
            this.encoder.OnEncodeProgress += EncoderOnEncodeProgress;
            this.encoder.OnEncodeFinished += EncoderOnEncodeFinished;

            this.video = video;

            title = TitleDownloading;
            active = true;
            state = ProcessState.Downloading;
        }
        
        public String Title { get { return title; } }

        public int Progress { get { return progress; } }

        public int Speed { get { return speed; } }

        public long Eta { get { return eta; } }

        public bool Active { get { return active; } }

        public Video Video { get { return video; } }

        public ProcessState State { get { return state; } }

        private void DownloaderProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Download progress should be from 0 to 50 percent
            progress = e.ProgressPercentage / 2;
            speed = downloader.DownloadSpeed;
            eta = downloader.ETA;

            SetChanged();
        }

        private void DownloaderRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            title = TitleEncoding;
            state = ProcessState.Encoding;

            SetChanged();
        }

        private void EncoderOnEncodeProgress(object sender, EncodeProgressEventArgs e)
        {
            // Encode progress should be from 50 to 100 percent
            progress = 50 + (int)((float)e.Percentage / 2f);
            speed = e.BytesPerSecond;
            eta = e.Eta;

            SetChanged();
        }

        private void EncoderOnEncodeFinished(object sender, EncodeFinishedEventArgs e)
        {
            active = false;

            SetChanged();
        }
    }
}
