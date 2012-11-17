using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using B3.ViewModels;
using FFMpegLib;
using VideoEncoder;
//using YouTubeDownloader;
using YouTubeLib;

namespace B3
{
    public partial class Search : Form
    {
        private FFMpegInstance ffmpeg;

        public Search()
        {
            InitializeComponent();

            txtBrowse.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            txtSearch.KeyDown += txtSearch_KeyDown;

            FormClosed += Search_FormClosed;

            Stream stream = GetType().Assembly.GetManifestResourceStream("B3.Resources.ffmpeg.exe");
            ffmpeg = new FFMpegInstance(stream);
        }

        void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.KeyCode == Keys.Return)
            {
                button1_Click(null, null);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var videos = Query.Search(txtSearch.Text);

            listView1.Items.Clear();
            foreach (Video video in videos)
            {
                listView1.Items.Add(new ListViewItem(new string[] {

                    video.Title
                    
                }) { Tag = video });
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Video video = listView1.SelectedItems[0].Tag as Video;
                
                //var urls = YouTubeDownloader.YouTubeDownloader.GetYouTubeVideoUrls(new string[] { String.Format("http://www.youtube.com{0}", video.Link) });
                var qualityEntry = video.GetBestQuality();

                var formattedTitle = Download.ConvertTitleToFilename(video.Title);
                var output = Path.Combine(ffmpeg.OutputPath, formattedTitle);

                // Downloader
                var downloader = new Download(qualityEntry.DownloadUrl, ffmpeg.OutputPath, formattedTitle);

                // Encoder
                var encoder = new Encoder();
                encoder.FFmpegPath = ffmpeg.ExecutablePath;

                downloader.RunWorkerCompleted += (object s, RunWorkerCompletedEventArgs ev) =>
                {
                    encoder.EncodeVideoAsync(new VideoFile(output), "-ab 192000", Path.Combine(txtBrowse.Text, formattedTitle + ".mp3"), this, 1);
                };

                var viewModel = new DownloadDialogModel(downloader, encoder, video);
                var view = new DownloadDialog(viewModel);
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

        void Search_FormClosed(object sender, FormClosedEventArgs e)
        {
            ffmpeg.Dispose();
        }
    }
}
