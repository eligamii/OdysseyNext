using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.FourGet.Objects
{
    public class Result
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "spelling")]
        public Spelling Spelling { get; set; }

        [JsonProperty(PropertyName = "npt")]
        public string Nextpage { get; set; }

        [JsonProperty(PropertyName = "answer")]
        public List<Answer> Answer { get; set; }

        [JsonProperty(PropertyName = "web")]
        public List<SearchResult> WebSearchResults { get; set; }

        [JsonProperty(PropertyName = "video")]
        public List<SearchResult> VideoSearchResults { get; set; }

        [JsonProperty(PropertyName = "news")]
        public List<SearchResult> NewsSearchResults { get; set; }

        [JsonProperty(PropertyName = "related")]
        public List<string> RelatedSearchResults { get; set; }
    }
}
