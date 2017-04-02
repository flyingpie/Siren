using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
//using YouTubeDownloader;
using YouTubeLib;

namespace Siren.ViewModels
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
        
        private String title;
        private int progress;
        private int speed;
        private long eta;
        private bool active;
        private Video video;
        private ProcessState state;

        public DownloadDialogModel(Video video)
        {
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
            progress = e.ProgressPercentage;
            //speed = downloader.DownloadSpeed;
            //eta = downloader.ETA;

            SetChanged();
        }

        private void DownloaderRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            title = TitleEncoding;
            state = ProcessState.Encoding;

            SetChanged();

            active = false;
        }
    }
}
