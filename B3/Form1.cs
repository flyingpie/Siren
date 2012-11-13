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
using Google.GData.Client;
using Google.GData.YouTube;
using Google.YouTube;
using VideoEncoder;

namespace B3
{
    public partial class Form1 : Form
    {
        private YouTubeRequestSettings settings;
        private Splash splash;

        public Form1()
        {
            InitializeComponent();

            settings = new YouTubeRequestSettings("Flying Pie Entertainment", "AI39si4kCLYAsKD2056-Q3Op9g1r_o0T5TcVF66j3r6tJXumg02A91QL727AOUPMzsMBCDj6adLY8OxkNTOomUP-D23b-C4qAg");
            settings.AutoPaging = true;
            settings.PageSize = 40;

            txtBrowse.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            txtSearch.KeyDown += txtSearch_KeyDown;

            new Thread(new ThreadStart(InitializeLoaderThread)).Start();

            ShowSplash();
        }

        private delegate void SplashDelegate();

        private void ShowSplash()
        {
            splash = new Splash();
            splash.ShowDialog();
        }

        private void HideSplash()
        {
            if (splash.InvokeRequired)
            {
                splash.Invoke(new SplashDelegate(HideSplash));
            }
            else
            {
                splash.Close();
            }
        }

        private void InitializeLoaderThread()
        {
            YouTubeQuery q = new YouTubeQuery(YouTubeQuery.DefaultVideoUri);
            var v = new YouTubeRequest(settings).Get<Video>(q).Entries.First();

            HideSplash();
        }

        void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                button1_Click(null, null);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            YouTubeRequest request = new YouTubeRequest(settings);
            request.Proxy = null;

            YouTubeQuery query = new YouTubeQuery(YouTubeQuery.DefaultVideoUri);
            query.Query = txtSearch.Text;
            query.SafeSearch = YouTubeQuery.SafeSearchValues.None;
            
            Feed<Video> videos = request.Get<Video>(query);

            int i = 0;

            listView1.Items.Clear();
            foreach (Video video in videos.Entries)
            {
                listView1.Items.Add(new ListViewItem(new string[] {

                    video.Title,
                    video.Description,
                    video.ViewCount.ToString()
                    
                }) { Tag = video });

                i++;

                if (i >= 40) break;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Video video = listView1.SelectedItems[0].Tag as Video;

                var urls = YouTubeDownloader.YouTubeDownloader.GetYouTubeVideoUrls(new string[] { video.WatchPage.AbsoluteUri });

                //var dialog = new SaveFileDialog();
                //dialog.ShowDialog();
                //output = dialog.FileName;

                Directory.CreateDirectory("output");
                output = Path.Combine("output", urls[0].VideoTitle);
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
            encoder.FFmpegPath = "ffmpeg.exe";

            VideoFile file = new VideoFile(output);

            //var dialog = new SaveFileDialog();
            //dialog.DefaultExt = "mp3";

            //dialog.ShowDialog();

            YouTubeDownloader.frmFileDownloader downloader = (YouTubeDownloader.frmFileDownloader)sender;

            var outputFile = Path.Combine(txtBrowse.Text, downloader.Video.VideoTitle + ".mp3"); //dialog.FileName;

            //if (Path.GetExtension(outputFile) != ".mp3") outputFile += ".mp3";

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
    }
}
