using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Odyssey.WebSearch.Helpers.Answers
{
    public class Answer
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageSource { get; set; } = null;
        public string Source { get; set; }
    }

    public class SearchAnswerHelper // works mostly thanks to the DuckDuckGo Instant Answer API, NCalc
    {
        private static HttpClient _httpClient = new();

        public static async Task<Answer> Answer(string query)
        {
            Answer answer = new();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:122.0) Gecko/20100101 Firefox/122.0");
            var json = await _httpClient.GetStringAsync("http://duckduckgo.com/?format=json&no_html=true&q=" + WebUtility.UrlEncode(query));

            var rawAnswer = JsonConvert.DeserializeObject<AnswerDeserializer>(json);

            answer.Title = rawAnswer.Heading;
            answer.Description = rawAnswer.Abstract != null ? rawAnswer.Abstract : rawAnswer.AbstractSource;

            return answer;
        }
    }
}
