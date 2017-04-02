using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using YouTubeLib.Download;

namespace YouTubeLib
{
    public class YouTubeAPI
    {
        private const string YouTubeBaseUrl = "http://www.youtube.com";

        private string executablePath;

        public List<DownloadTask> Downloads { get; set; }

        public YouTubeAPI()
        {
            var sirenExePath = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            var sirenExeDir = Path.GetDirectoryName(sirenExePath);

            executablePath = Path.Combine(sirenExeDir, @"Resources\youtube-dl.exe");
            executablePath = @"Resources\youtube-dl.exe";
            
            Downloads = new List<DownloadTask>();
        }

        public IEnumerable<Video> Search(string term, int pages)
        {
            var result = new List<Video>();

            Query.Search(term, pages, result);

            return result;
        }

        public DownloadTask CreateDownload(string relativeUrl, string outputFolder)
        {
            var processStartInfo = CreateProcess(relativeUrl, outputFolder);
            var download = new DownloadTask(processStartInfo);
            Downloads.Add(download);
            return download;
        }

        internal ProcessStartInfo CreateProcess(string relativeUrl, string outputFolder)
        {
            var guid = Guid.NewGuid();
            
            var arg = "--extract-audio --audio-format mp3 --audio-quality 0 --output \"{0}\" {1}";

            var output = Path.Combine(outputFolder, "%(title)s.mp3");

            arg = string.Format(arg, output, YouTubeBaseUrl + relativeUrl);

            var oInfo = new ProcessStartInfo(executablePath, arg);
            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            return oInfo;
        }
    }
}