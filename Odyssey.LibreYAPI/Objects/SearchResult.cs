using System;
using System.Net;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.LibreYAPI.Objects
{

    public class SearchResult
    {
        public string title { get; set; }
        public string url { get; set; }
        public string base_url { get; set; }
        public string description { get; set; }

        public string source { get; set; }


        public string decodedTitle => WebUtility.HtmlDecode(title);
        public string decodedDescription => WebUtility.HtmlDecode(description);
        public string subtitle
        {
            get
            {
                Uri url = new Uri(this.url);
                return url.Host.Replace("www.", "");
            }
        }

        public string iconSource => $"http://muddy-jade-bear.faviconkit.com/{new Uri(url).Host}/21";

        public SpecialResponse special_response { get; set; }

        public static implicit operator SearchResult(string str)
        {
            return new SearchResult() { source = str };
        }
    }

    public class SpecialResponse
    {
        public string response { get; set; }
        public string source { get; set; }
        public string image { get; set; }
    }
}
