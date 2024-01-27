using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.FourGet.Objects
{
    public class Spelling
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "using")]
        public string Using { get; set; }

        [JsonProperty(PropertyName = "correction")]
        public string Correction { get; set; }
    }
}
