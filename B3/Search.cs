using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using B3.Controls;
using B3.ViewModels;
using FFMpegLib;
using Microsoft.WindowsAPICodePack.Taskbar;
using VideoEncoder;
using YouTubeLib;
using System.Diagnostics;
using YouTubeLib;
using YouTubeLib.Download;

namespace B3
{
    public partial class Search : Form
    {
        private FFMpegInstance ffmpeg;
        private YouTubeAPI youtubedl;

        private List<DownloadDialogModel> downloads;

        public Search()
        {
            InitializeComponent();

            txtBrowse.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            txtSearch.KeyDown += Search_KeyDown;
            FormClosed += Search_FormClosed;

            ffmpeg = new FFMpegInstance();
            youtubedl = new YouTubeAPI();

            downloads = new List<DownloadDialogModel>();
        }

        public DataListView<Video> ResultsList
        {
            get { return lstResults as DataListView<Video>; }
        }

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Search_Click(null, null);

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void Search_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            txtSearch.Enabled = false;

            lstResults.Items.Clear();

            var videos = new DataList<Video>();

            ResultsList.Bind(videos);
            ResultsList.ViewFunc = (v) => new string[] { v.Title };

            Task.Factory.StartNew(() =>
                {
                    //Query.Search(txtSearch.Text, 2, videos)

                    foreach (var video in youtubedl.Search(txtSearch.Text, 2))
                    {
                        videos.Add(video);
                    }
                });

            btnSearch.Enabled = true;
            txtSearch.Enabled = true;
        }

        private void listView1_DoubleClick(object s, EventArgs e)
        {
            if (lstResults.SelectedItems.Count == 1)
            {
                Video video = lstResults.SelectedItems[0].Tag as Video;

                var view = new DownloadDialog(null, video.Title);

                string tempFile = null;

                var encoder = ffmpeg.CreateEncoder();
                var downloader = youtubedl.CreateDownload(video.Link);

                downloader.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs args) =>
                {
                    view.TaskText = "Downloading...";
                    view.ProgressBar = (int)Math.Round(args.Percentage);
                };
                downloader.DownloadCompleted += (object sender, DownloadCompletedEventArgs args) =>
                {
                    tempFile = args.FileName;
                    var outputFile = Path.Combine(txtBrowse.Text, video.FormattedTitle + ".mp3");
                    encoder.EncodeVideoAsync(args.FileName, outputFile, OutputMode.Mp3, this);
                };
                encoder.OnEncodeProgress += (object fSender, EncodeProgressEventArgs fE) =>
                {
                    view.PStyle = ProgressBarStyle.Continuous;
                    view.TaskText = "Encoding...";
                    view.ProgressBar = fE.Percentage;
                };
                encoder.OnEncodeFinished += (object ffSender, EncodeFinishedEventArgs ffE) =>
                {
                    File.Delete(tempFile);
                    view.Close();
                };
                
                downloader.StartAsync(this);
                
                view.Show();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            dialog.ShowDialog();

            if (Directory.Exists(dialog.SelectedPath))
            {
                txtBrowse.Text = dialog.SelectedPath;
            }
        }

        private void Search_FormClosed(object sender, FormClosedEventArgs e)
        {
            ffmpeg.Dispose();
            youtubedl.Dispose();
        }
    }
}
