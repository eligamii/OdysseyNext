using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.FourGet.Helpers
{
    internal class Api
    {
        internal class P
        {
            internal P(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; set; }
            public string Value { get; set; }
        }

        private static HttpClient client = new();
        internal static async Task<string> GET(string endpoint, params P[] parameters)
        {
            string parametersString = string.Empty;
            if (parameters.Count() > 0) parametersString = "?";

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i != 0) parametersString += "&";
                parametersString += $"{parameters[i].Key}={parameters[i].Value}";
            }

            string url = endpoint + parametersString;

            try
            {
                return await client.GetStringAsync(url);
            }
            catch (HttpRequestException) { return string.Empty; }

        }
    }
}
