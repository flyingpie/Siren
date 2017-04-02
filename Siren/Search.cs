using Siren.Controls;
using Siren.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using YouTubeLib;
using YouTubeLib.Download;

namespace Siren
{
    public partial class Search : Form
    {
        private YouTubeAPI youtubedl;

        private List<DownloadDialogModel> downloads;

        public Search()
        {
            InitializeComponent();

            txtBrowse.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            txtSearch.KeyDown += Search_KeyDown;

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

                var downloader = youtubedl.CreateDownload(video.Link, txtBrowse.Text);

                downloader.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs args) =>
                {
                    view.TaskText = "Downloading...";
                    view.ProgressBar = (int)Math.Round(args.Percentage);
                };

                downloader.DownloadCompleted += (object sender, DownloadCompletedEventArgs args) =>
                {
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
    }
}