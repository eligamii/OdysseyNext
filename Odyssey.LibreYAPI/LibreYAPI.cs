using Newtonsoft.Json;
using Odyssey.LibreYAPI.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Odyssey.FourGet.Helpers.Api;

namespace Odyssey.LibreYAPI
{
    public class LibreYAPI
    {
        public static async Task<Dictionary<string, SearchResult>> SearchAsync(string query, int page)
        {
            string json = await GET("https://glass.prpl.wtf/api.php", new P("q", WebUtility.UrlEncode(query)), new P("p", page.ToString()), new P("t", "0"));
            return JsonConvert.DeserializeObject<Dictionary<string, SearchResult>>(json);
        }
    }
}
