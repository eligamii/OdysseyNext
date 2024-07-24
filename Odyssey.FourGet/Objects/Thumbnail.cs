using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.FourGet.Objects
{
    public class Thumbnail
    {
        public  Thumbnail(string url)
        {
            Url = url;
        }
        public Thumbnail() { }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        public static implicit operator Thumbnail(string input)
        {
            return new Thumbnail(input);
        }
    }
}
