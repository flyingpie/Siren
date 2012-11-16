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
using FFMpegLib;
using VideoEncoder;
using YouTubeLib;

namespace B3
{
    public partial class Search : Form
    {
        private FFMpegInstance ffmpeg;
        private EncodingProgress encodingProgressDialog;

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

                var urls = YouTubeDownloader.YouTubeDownloader.GetYouTubeVideoUrls(new string[] { String.Format("http://www.youtube.com{0}", video.Link) });

                //Directory.CreateDirectory("output");
                output = Path.Combine(ffmpeg.OutputPath, urls[0].VideoTitle);
                var downloader = new YouTubeDownloader.frmFileDownloader(urls[0].DownloadUrl, output);
                downloader.Video = urls[0];
                downloader.FormClosed += downloader_FormClosed;
                downloader.Show();
            }
        }

        private String output = String.Empty;

        void downloader_FormClosed(object sender, FormClosedEventArgs e)
        {
            Encoder encoder = new Encoder();
            encoder.FFmpegPath = ffmpeg.ExecutablePath; //"ffmpeg.exe";

            VideoFile file = new VideoFile(output);

            YouTubeDownloader.frmFileDownloader downloader = (YouTubeDownloader.frmFileDownloader)sender;

            var outputFile = Path.Combine(txtBrowse.Text, downloader.Video.VideoTitle + ".mp3");

            encodingProgressDialog = new EncodingProgress();
            encodingProgressDialog.Show();

            encoder.EncodeVideoAsync(file, "-ab 192000", outputFile, this, 1);

            encoder.OnEncodeProgress += encoder_OnEncodeProgress;
            encoder.OnEncodeFinished += encoder_OnEncodeFinished;
        }

        void encoder_OnEncodeProgress(object sender, EncodeProgressEventArgs e)
        {
            Console.WriteLine("Encode progress: " + Math.Round(((float)e.CurrentFrame / (float)e.TotalFrames) * 100, 2));
        }

        void encoder_OnEncodeFinished(object sender, EncodeFinishedEventArgs e)
        {
            Console.WriteLine("Encode finished");

            encodingProgressDialog.Close();
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
            /*
            var output = Directory.GetFiles();

            foreach (var file in output)
            {
                File.Delete(file);
            }*/

            ffmpeg.Dispose();
        }
    }
}
