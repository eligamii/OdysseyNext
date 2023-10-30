namespace Odyssey.Shared.ViewModels.Data
{
    public class HistoryItem
    {
        public string Description { get; set; }
        public long Timestamp { get; set; }
        public string ImageSource { get { return $"https://muddy-jade-bear.faviconkit.com/{new System.Uri(Url).Host}/21"; } }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Date { get; set; }
        public string Subtitle
        {
            get
            {
                if (Url.Length < 40)
                    return Url;
                else return Url.Substring(0, 40) + "..."; // To avoid too long strings

            }
        }
    }

}
