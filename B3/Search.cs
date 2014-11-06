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

namespace B3
{
    public partial class Search : Form
    {
        private FFMpegInstance ffmpeg;
        private List<DownloadDialogModel> downloads;

        private string youtubeDlPath;

        public Search()
        {
            InitializeComponent();

            txtBrowse.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            txtSearch.KeyDown += Search_KeyDown;

            FormClosed += Search_FormClosed;

            Stream stream = GetType().Assembly.GetManifestResourceStream("B3.Resources.ffmpeg.exe");
            ffmpeg = new FFMpegInstance(stream);

            youtubeDlPath = UnpackYoutubeDl();

            downloads = new List<DownloadDialogModel>();
        }

        public string UnpackYoutubeDl()
        {
            // youtube-dl
            var stream = GetType().Assembly.GetManifestResourceStream("B3.Resources.youtube-dl.exe");
            var tempPath = Path.Combine(Path.GetTempPath(), "B3");

            var executablePath = Path.Combine(tempPath, "youtube-dl.exe");
            var outputPath = Path.Combine(tempPath, "output");

            Directory.CreateDirectory(outputPath);

            if (!File.Exists(executablePath))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                File.WriteAllBytes(executablePath, bytes);
            }

            return executablePath;
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

            ResultsList.Bind(videos);
            ResultsList.ViewFunc = (v) => new string[] { v.Title };

            Task.Factory.StartNew(() => Query.Search(txtSearch.Text, 2, videos));

            btnSearch.Enabled = true;
            txtSearch.Enabled = true;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (lstResults.SelectedItems.Count == 1)
            {
                var view = new DownloadDialog(null);

                Video video = lstResults.SelectedItems[0].Tag as Video;

                //var urls = YouTubeDownloader.YouTubeDownloader.GetYouTubeVideoUrls(new string[] { String.Format("http://www.youtube.com{0}", video.Link) });
                //var qualityEntry = video.GetBestQuality();

                var formattedTitle = Download.ConvertTitleToFilename(video.Title);
                var output = Path.Combine(ffmpeg.OutputPath, formattedTitle);

                // Downloader
                //var downloader = new Download(qualityEntry.DownloadUrl, ffmpeg.OutputPath, formattedTitle);
                //var videoInfos = DownloadUrlResolver.GetDownloadUrls(video.Link);
                //var videoInfo = videoInfos.OrderBy(vi => vi.AudioBitrate).First();

                //if (videoInfo.RequiresDecryption)
                //{
                //    DownloadUrlResolver.DecryptDownloadUrl(videoInfo);
                //}

                //var videoDownloader = new VideoDownloader(videoInfo, output);
                //videoDownloader.DownloadProgressChanged += (object vSender, ProgressEventArgs vE) =>
                //{
                //    view.TaskText = "Downloading...";
                //    int averageProgress = (int)((float)downloads.Sum(d => d.Progress) / (float)downloads.Count);
                //    view.ProgressBar = averageProgress;

                //    if (TaskbarManager.IsPlatformSupported)
                //    {
                //        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                //        TaskbarManager.Instance.SetProgressValue(averageProgress, 100);
                //    }
                //};


                // Encoder
                var encoder = new Encoder();
                encoder.FFmpegPath = ffmpeg.ExecutablePath;
                encoder.OnEncodeProgress += (object fSender, EncodeProgressEventArgs fE) =>
                {
                    view.PStyle = ProgressBarStyle.Continuous;
                    view.TaskText = "Encoding...";
                    view.ProgressBar = fE.Percentage;
                };
                encoder.OnEncodeFinished += (object ffSender, EncodeFinishedEventArgs ffE) =>
                {
                    File.Delete(output);
                    view.Close();
                };

                view.Show();

                Task.WaitAll(Task.Factory.StartNew(() =>
                {
                    //view.TaskText = "Downloading...";
                    //view.Pbar.Style = ProgressBarStyle.Continuous;

                    var arg = "\"" + Query.YouTubeBaseUrl + video.Link + "\" -o \"" + ffmpeg.OutputPath + @"\" + formattedTitle + ".%(ext)s\"";
                    ProcessStartInfo oInfo = new ProcessStartInfo(youtubeDlPath, arg);
                    oInfo.UseShellExecute = false;
                    oInfo.CreateNoWindow = true;
                    oInfo.RedirectStandardOutput = true;
                    oInfo.RedirectStandardError = true;

                    //Create the output
                    string output1 = null;

                    //try the process
                    try
                    {
                        //run the process
                        Process proc = System.Diagnostics.Process.Start(oInfo);

                        //now put it in a string
                        //This needs to be before WaitForExit() to prevent deadlock, for details: http://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput%28v=VS.80%29.aspx
                        output1 = proc.StandardError.ReadToEnd();
                        var o2 = proc.StandardOutput.ReadToEnd();
                        //Wait for exit
                        proc.WaitForExit();

                        //Release resources
                        proc.Close();
                    }
                    catch (Exception ex)
                    {
                        output = string.Empty;
                    }

                    output = Path.Combine(output, Directory.GetFiles(ffmpeg.OutputPath).First());
                }));

                encoder.EncodeVideoAsync(new VideoFile(output), "-ab 192000", Path.Combine(txtBrowse.Text, formattedTitle + ".mp3"), this, 1);


                //Task.Run(() =>
                //{
                //    videoDownloader.Execute();
                //    encoder.EncodeVideoAsync(new VideoFile(output), "-ab 192000", Path.Combine(txtBrowse.Text, formattedTitle + ".mp3"), this, 1);
                //});

                //var viewModel = new DownloadDialogModel(downloader, encoder, video);
                //var view = new DownloadDialog(viewModel);

                //downloads.Add(viewModel);

                //downloader.ProgressChanged += (object s, ProgressChangedEventArgs ev) =>
                //{
                //    int averageProgress = (int)((float)downloads.Sum(d => d.Progress) / (float)downloads.Count);

                //    if (TaskbarManager.IsPlatformSupported)
                //    {
                //        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                //        TaskbarManager.Instance.SetProgressValue(averageProgress, 100);
                //    }
                //};

                //downloader.RunWorkerCompleted += (object s, RunWorkerCompletedEventArgs ev) =>
                //{
                //    encoder.EncodeVideoAsync(new VideoFile(output), "-ab 192000", Path.Combine(txtBrowse.Text, formattedTitle + ".mp3"), this, 1);

                //    downloads.Remove(viewModel);
                //};

                //view.Show();

                //downloader.RunWorkerAsync();
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
            try
            {
                File.Delete(youtubeDlPath);
            }
            catch { }
        }
    }
}
