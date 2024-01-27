using Newtonsoft.Json;
using Odyssey.FourGet.Helpers;
using Odyssey.FourGet.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Odyssey.FourGet.Helpers.Api;




namespace Odyssey.FourGet
{
    public class FourGet
    {
        public static async Task<Result> SearchAsync(string query)
        {
            string json = await GET("https://4get.lunar.icu/api/v1/web", new P("s", query));
            return JsonConvert.DeserializeObject<Result>(json);
        }
    }
}
