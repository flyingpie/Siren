using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouTubeDL
{
    public class DownloadTask
    {
        private const string YouTubeBaseUrl = "http://www.youtube.com";

        private const string LINE_DESTINATION = "Destination:";
        private const string LINE_ETA = "ETA";

        public delegate void DownloadStartedEventHandler(object sender, DownloadStartedEventArgs args);
        public delegate void DownloadProgressChangedEventHandler(object sender, DownloadProgressChangedEventArgs args);
        public delegate void DownloadCompletedEventHandler(object sender, DownloadCompletedEventArgs args);

        public event EventHandler<DownloadStartedEventArgs> DownloadStarted = delegate { };
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged = delegate { };
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted = delegate { };

        private ProcessStartInfo processStartInfo;

        public DownloadTask(ProcessStartInfo process)
        {
            this.processStartInfo = process;
        }

        public void StartAsync(Control caller)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Process proc = Process.Start(processStartInfo);

                    string line;
                    string filePath = null;
                    while ((line = proc.StandardOutput.ReadLine()) != null)
                    {
                        if (line.Contains(LINE_DESTINATION))
                        {
                            filePath = line.Substring(line.IndexOf(LINE_DESTINATION) + LINE_DESTINATION.Length).Trim();
                            Console.WriteLine("FilePath: " + filePath);
                        }

                        if (line.Contains(LINE_ETA))
                        {
                            try
                            {
                                var split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                var percentage = float.Parse(split[1].Trim().Replace("%", "").Trim(), CultureInfo.InvariantCulture);

                                OnDownloadProgressChanged(caller, percentage);

                                Console.WriteLine("Percentage: " + percentage);
                            }
                            catch (Exception)
                            {
                                // Parsing of percentage failed
                            }
                        }
                    }

                    //Wait for exit
                    proc.WaitForExit();

                    //Release resources
                    proc.Close();

                    OnDownloadCompleted(caller, filePath);
                }
                catch (Exception ex)
                {

                }
            });
        }

        private void OnDownloadProgressChanged(Control caller, float percentage)
        {
            var arg = new DownloadProgressChangedEventArgs() { Percentage = percentage };

            if (caller.InvokeRequired)
            {
                caller.BeginInvoke(new DownloadProgressChangedEventHandler(DownloadProgressChanged), caller, arg);
            }
            else
            {
                DoDownloadProgressChanged(arg);
            }
        }

        private void DoDownloadProgressChanged(DownloadProgressChangedEventArgs arg)
        {
            DownloadProgressChanged(this, arg);
        }

        private void OnDownloadCompleted(Control caller, string fileName)
        {
            var arg = new DownloadCompletedEventArgs() { FileName = fileName };

            if (caller.InvokeRequired)
            {
                caller.BeginInvoke(new DownloadCompletedEventHandler(DownloadCompleted), caller, arg);
            }
            else
            {
                DoDownloadCompleted(arg);
            }
        }

        private void DoDownloadCompleted(DownloadCompletedEventArgs arg)
        {
            DownloadCompleted(this, arg);
        }
    }
}
