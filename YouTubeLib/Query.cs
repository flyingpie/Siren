using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace YouTubeLib
{
    public static class Query
    {
        private const string SearchUrl = "http://www.youtube.com/results?search_type=videos&search_query={0}";

        /// <summary>
        /// Searches YouTube for the specified term and returns a list of resulting video links
        /// </summary>
        /// <param name="term">The term to search for</param>
        /// <returns>A list of video links</returns>
        public static List<Video> Search(String term)
        {
            var query = String.Format(SearchUrl, term);

            // Create a web request with a proxy null value to speed up the request action
            WebRequest request = WebRequest.Create(query);
            request.Proxy = null;

            WebResponse response = request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(result);

                var links = doc.DocumentNode.ChildNodes
                    .Descendants()
                    .Where(x => x.GetAttributeValue("class", "").Contains("yt-uix-tile-link"))
                    .Select(x => new Video()
                    {
                        Title = HttpUtility.HtmlDecode(x.InnerText),
                        Link = HttpUtility.HtmlDecode(x.GetAttributeValue("href", ""))
                    }).ToList();

                return links;
            }
        }

        public static List<VideoQuality> GetYouTubeVideoUrls(params string[] VideoUrls)
        {
            List<VideoQuality> urls = new List<VideoQuality>();
            foreach (var VideoUrl in VideoUrls)
            {
                string html = Helper.DownloadWebPage(VideoUrl);
                string title = GetTitle(html);
                foreach (var videoLink in ExtractUrls(html))
                {
                    VideoQuality q = new VideoQuality();
                    q.VideoUrl = VideoUrl;
                    q.VideoTitle = title;
                    q.DownloadUrl = videoLink + "&title=" + title;
                    if (getQuality(q))
                        urls.Add(q);
                }
            }
            return urls;
        }

        private static string GetTitle(string RssDoc)
        {
            string str14 = Helper.GetTxtBtwn(RssDoc, "'VIDEO_TITLE': '", "'", 0);
            if (str14 == "") str14 = Helper.GetTxtBtwn(RssDoc, "\"title\" content=\"", "\"", 0);
            if (str14 == "") str14 = Helper.GetTxtBtwn(RssDoc, "&title=", "&", 0);
            str14 = str14.Replace(@"\", "").Replace("'", "&#39;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("+", " ");
            return str14;
        }


        private static List<string> ExtractUrls(string html)
        {
            html = Uri.UnescapeDataString(Regex.Match(html, "url_encoded_fmt_stream_map=(.+?)&", RegexOptions.Singleline).Groups[1].ToString());
            MatchCollection matchs = Regex.Matches(html, "url=(.+?)&quality=(.+?)&fallback_host=(.+?)&type=(.+?)&itag=(.+?),", RegexOptions.Singleline);
            bool firstTry = matchs.Count > 0;
            if (!firstTry)
                matchs = Regex.Matches(html, "itag=(.+?)&url=(.+?)&type=(.+?)&fallback_host=(.+?)&sig=(.+?)&quality=(.+?),{0,1}", RegexOptions.Singleline);
            List<string> urls = new List<string>();
            foreach (Match match in matchs)
            {
                if (firstTry)
                    urls.Add(Uri.UnescapeDataString(match.Groups[1] + ""));
                else urls.Add(Uri.UnescapeDataString(match.Groups[2] + "") + "&signature=" + match.Groups[5]);
            }
            return urls;
        }

        private static bool getQuality(VideoQuality q)
        {
            if (q.DownloadUrl.Contains("itag=5"))
                q.SetQuality("flv", new Size(320, 240));
            else if (q.DownloadUrl.Contains("itag=34"))
                q.SetQuality("flv", new Size(400, 226));
            else if (q.DownloadUrl.Contains("itag=6"))
                q.SetQuality("flv", new Size(480, 360));
            else if (q.DownloadUrl.Contains("itag=35"))
                q.SetQuality("flv", new Size(640, 380));
            else if (q.DownloadUrl.Contains("itag=18"))
                q.SetQuality("mp4", new Size(480, 360));
            else if (q.DownloadUrl.Contains("itag=22"))
                q.SetQuality("mp4", new Size(1280, 720));
            else if (q.DownloadUrl.Contains("itag=37"))
                q.SetQuality("mp4", new Size(1920, 1280));
            else if (q.DownloadUrl.Contains("itag=38"))
                q.SetQuality("mp4", new Size(4096, 72304));
            else return false;
            return true;
        }
    }
}
