using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using B3.ViewModels;
using VideoEncoder;
using YouTubeDownloader;

namespace B3
{
    public partial class DownloadDialog : Form
    {
        private delegate void InvokeChange(object sender, EventArgs e);

        private DownloadDialogModel viewModel;

        public DownloadDialog(DownloadDialogModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            this.viewModel.Changed += ViewModelChanged;
        }

        private void ViewModelChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new InvokeChange(ViewModelChanged), sender, e);
            }
            else
            {
                Text = viewModel.Title;
                lblTitle.Text = viewModel.Video.Title;
                lblStatus.Text = viewModel.Title;
                prgProgress.Value = viewModel.Progress;
                lblInfo.Text = viewModel.Speed + " " + viewModel.Eta;
                lblInfo.Text = viewModel.Eta + "s remaining";
                lblInfo.Text += " (" + viewModel.Speed + "b/s)";

                if (!viewModel.Active)
                {
                    Close();
                }
            }
        }
    }
}
