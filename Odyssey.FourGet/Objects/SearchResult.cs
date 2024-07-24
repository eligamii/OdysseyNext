using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.FourGet.Objects
{
    public class SearchResult
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "date")]
        public int? Date { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int? Duration { get; set; }

        [JsonProperty(PropertyName = "views")]
        public int? Views { get; set; }

        [JsonProperty(PropertyName = "thumb")]
        public Thumbnail Thumbnail { get; set; }

        [JsonProperty(PropertyName = "sublink")]
        public List<SearchResult> SubLink { get; set; }
    }
}
