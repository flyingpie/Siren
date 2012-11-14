using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
    }
}
