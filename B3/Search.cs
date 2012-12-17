using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using B3.Controls;
using B3.ViewModels;
using FFMpegLib;
using Microsoft.WindowsAPICodePack.Taskbar;
using VideoEncoder;
using YouTubeLib;

namespace B3
{
    public partial class Search : Form
    {
        private FFMpegInstance ffmpeg;
        private List<DownloadDialogModel> downloads;

        public Search()
        {
            InitializeComponent();

            txtBrowse.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            txtSearch.KeyDown += Search_KeyDown;

            FormClosed += Search_FormClosed;

            Stream stream = GetType().Assembly.GetManifestResourceStream("B3.Resources.ffmpeg.exe");
            ffmpeg = new FFMpegInstance(stream);

            downloads = new List<DownloadDialogModel>();
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

        private async void Search_Click(object sender, EventArgs e)
        {
            /*
            var videos = await Query.Search(txtSearch.Text);

            lstResults.Items.Clear();
            foreach (Video video in videos)
            {
                lstResults.Items.Add(new ListViewItem(new string[] {

                    video.Title
                    
                }) { Tag = video });
            }
            */
            btnSearch.Enabled = false;
            txtSearch.Enabled = false;

            lstResults.Items.Clear();

            var videos = new DataList<Video>();

            lstResults.Bind(videos);
            lstResults.ViewFunc = (v) => new string[] { v.Title };

            await Task.Run(() => Query.Search(txtSearch.Text, 2, videos));

            btnSearch.Enabled = true;
            txtSearch.Enabled = true;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (lstResults.SelectedItems.Count == 1)
            {
                Video video = lstResults.SelectedItems[0].Tag as Video;
                
                //var urls = YouTubeDownloader.YouTubeDownloader.GetYouTubeVideoUrls(new string[] { String.Format("http://www.youtube.com{0}", video.Link) });
                var qualityEntry = video.GetBestQuality();

                var formattedTitle = Download.ConvertTitleToFilename(video.Title);
                var output = Path.Combine(ffmpeg.OutputPath, formattedTitle);

                // Downloader
                var downloader = new Download(qualityEntry.DownloadUrl, ffmpeg.OutputPath, formattedTitle);

                // Encoder
                var encoder = new Encoder();
                encoder.FFmpegPath = ffmpeg.ExecutablePath;

                var viewModel = new DownloadDialogModel(downloader, encoder, video);
                var view = new DownloadDialog(viewModel);

                downloads.Add(viewModel);

                downloader.ProgressChanged += (object s, ProgressChangedEventArgs ev) =>
                {
                    int averageProgress = (int)((float)downloads.Sum(d => d.Progress) / (float)downloads.Count);

                    if (TaskbarManager.IsPlatformSupported)
                    {
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                        TaskbarManager.Instance.SetProgressValue(averageProgress, 100);
                    }
                };

                downloader.RunWorkerCompleted += (object s, RunWorkerCompletedEventArgs ev) =>
                {
                    encoder.EncodeVideoAsync(new VideoFile(output), "-ab 192000", Path.Combine(txtBrowse.Text, formattedTitle + ".mp3"), this, 1);

                    downloads.Remove(viewModel);
                };

                view.Show();

                downloader.RunWorkerAsync();
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
        }
    }
}
