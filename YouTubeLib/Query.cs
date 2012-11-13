using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace YouTubeLib
{
    public static class Query
    {
        private const string SearchUrl = "http://www.youtube.com/results?search_query={0}";

        public static List<Video> Search(String term)
        {
            var query = String.Format(SearchUrl, term);

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
                        Title = x.InnerText,
                        Link = x.GetAttributeValue("href", "")
                    }).ToList();

                return links;
            }
        }
    }
}
