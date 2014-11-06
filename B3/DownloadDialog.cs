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

namespace B3
{
    public partial class DownloadDialog : Form
    {
        private delegate void InvokeChange(object sender, EventArgs e);

        private DownloadDialogModel viewModel;

        public string TaskText
        {
            get { return lblStatus.Text; }
            set { lblStatus.Text = value; }
        }

        public int ProgressBar
        {
            get { return prgProgress.Value; }
            set { prgProgress.Value = value; }
        }

        public ProgressBar Pbar
        {
            get { return prgProgress; }
        }

        public ProgressBarStyle PStyle
        {
            get { return prgProgress.Style; }
            set { prgProgress.Style = value; }
        }

        public DownloadDialog(DownloadDialogModel viewModel)
        {
            InitializeComponent();

            this.FormClosed += DownloadDialog_FormClosed;

            prgProgress.Style = ProgressBarStyle.Marquee;
            lblTitle.Text = "Downloading...";
            lblStatus.Text = "Sorry I can't show you any progress yet :'(";

            //this.viewModel = viewModel;
            //this.viewModel.Changed += ViewModelChanged;
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
                prgProgress.Style = ProgressBarStyle.Continuous;
                prgProgress.Value = viewModel.Progress;
                lblInfo.Text = Formatting.FormatSeconds(viewModel.Eta) + " remaining";
                lblInfo.Text += " (" + Formatting.FormatBytes(viewModel.Speed) + "/s)";

                if (!viewModel.Active)
                {
                    Close();
                }
            }
        }

        private void DownloadDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
