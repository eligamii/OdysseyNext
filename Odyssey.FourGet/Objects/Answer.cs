using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.FourGet.Objects
{
    public class Answer
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public List<AnswerDescription> Description { get; set; }

        [JsonProperty(PropertyName = "thumb")]
        public Thumbnail Thumbnail { get; set; }

        [JsonProperty(PropertyName = "sublink")]
        public object Sublink { get; set; }
    }

    public class Sublink
    {
        public string Website { get; set; }
    }

    public class AnswerDescription
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }


    }
}
