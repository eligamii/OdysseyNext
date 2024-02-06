using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
