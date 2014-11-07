using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
 
namespace YouTubeLib
{
    public static class Helper
    {
        private static String InvalidCharacters = new String(Path.GetInvalidFileNameChars()) + new String(Path.GetInvalidPathChars());

        /// <summary>
        /// Decode a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(string str)
        {
            return System.Web.HttpUtility.UrlDecode(str);
        }

        public static bool IsValidUrl(string url)
        {
            string pattern = @"^(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?$";
            Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.IsMatch(url);
        }

        /// <summary>
        /// Gets the txt that lies between these two strings
        /// </summary>
        public static string GetTxtBtwn(string input, string start, string end, int startIndex)
        {
            return GetTxtBtwn(input, start, end, startIndex,false);
        }

        /// <summary>
        /// Gets the txt that lies between these two strings
        /// </summary>
        public static string GetLastTxtBtwn(string input, string start, string end, int startIndex)
        {
            return GetTxtBtwn(input, start, end, startIndex,true);
        }

        /// <summary>
        /// Gets the txt that lies between these two strings
        /// </summary>
        private static string GetTxtBtwn(string input, string start, string end,int startIndex,bool UseLastIndexOf)
        {
            int index1 = UseLastIndexOf ? input.LastIndexOf(start, startIndex) :
                                          input.IndexOf(start, startIndex);
            if (index1 == -1) return "";
            index1 += start.Length;
            int index2 = input.IndexOf(end, index1);
            if (index2 == -1) return input.Substring(index1);
            return input.Substring(index1, index2 - index1);
        }

        /// <summary>
        /// Split the input text for this pattren
        /// </summary>
        public static string[] Split(string input, string pattren)
        {
            return Regex.Split(input, pattren);
        }
    
        /// <summary>
        /// Returns the content of a given web adress as string.
        /// </summary>
        /// <param name="Url">URL of the webpage</param>
        /// <returns>Website content</returns>
        public static string DownloadWebPage(string Url)
        {
            return DownloadWebPage(Url, null);
        }

        private static string DownloadWebPage(string Url, string stopLine)
        {
            // Open a connection
            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(Url);

            //WebRequestObject.Proxy = InitialProxy();
            WebRequestObject.Proxy = null;
            // You can also specify additional header values like 
            // the user agent or the referer:
            WebRequestObject.UserAgent = ".NET Framework/2.0";

            // Request response:
            WebResponse Response = WebRequestObject.GetResponse();

            // Open data stream:
            Stream WebStream = Response.GetResponseStream();

            // Create reader object:
            StreamReader Reader = new StreamReader(WebStream);
            string PageContent = "", line;
            if (stopLine == null)
                PageContent = Reader.ReadToEnd();
            else while (!Reader.EndOfStream)
                {
                    line = Reader.ReadLine();
                    PageContent += line + Environment.NewLine;
                    if (line.Contains(stopLine)) break;
                }
            // Cleanup
            Reader.Close();
            WebStream.Close();
            Response.Close();

            return PageContent;
        }
        /// <summary>
        /// Get the ID of a youtube video from its URL
        /// </summary>
        public static string GetVideoIDFromUrl(string url)
        {
            url = url.Substring(url.IndexOf("?") + 1);
            string[] props = url.Split('&');

            string videoid = "";
            foreach (string prop in props)
            {
                if (prop.StartsWith("v="))
                    videoid = prop.Substring(prop.IndexOf("v=") + 2);
            }

            return videoid;
        }

        public static String ConvertTitleToFilename(String title)
        {
            foreach (char invalidChar in InvalidCharacters)
            {
                title = title.Replace(invalidChar, ' ');
            }

            // Strip any double spaces
            title = Regex.Replace(title, @"\s+", " ");

            return title;
        }
    }
}
