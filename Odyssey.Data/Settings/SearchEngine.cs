using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace Odyssey.Data.Settings
{

    public class SearchEngine
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Prefix { get; private set; }
        public string SearchUrl { get { return Url + Prefix; } }


        public static SearchEngine ToSearchEngineObject(SearchEngines? searchEngineEnum)
        {
            switch (searchEngineEnum)
            {
                case SearchEngines.DuckDuckGo: return SearchEngine.DuckDuckGo;
                case SearchEngines.Startpage: return SearchEngine.Startpage;
                case SearchEngines.YouDotCom: return SearchEngine.YouDotCom;
                case SearchEngines.Google: return SearchEngine.Google;
                case SearchEngines.Qwant: return SearchEngine.Qwant;
                case SearchEngines.Yahoo: return SearchEngine.Yahoo;
                case SearchEngines.Bing: return SearchEngine.Bing;
                default: return null;
            }
        }

        public static SearchEngines ToSearchEngineEnum(SearchEngine obj)
        {
            if (obj == SearchEngine.Google) return SearchEngines.Google;
            else if (obj == SearchEngine.Startpage) return SearchEngines.Startpage;
            else if (obj == SearchEngine.YouDotCom) return SearchEngines.YouDotCom;
            else if (obj == SearchEngine.DuckDuckGo) return SearchEngines.DuckDuckGo;
            else if (obj == SearchEngine.Qwant) return SearchEngines.Qwant;
            else if (obj == SearchEngine.Yahoo) return SearchEngines.Yahoo;
            else return SearchEngines.Bing;
        }




        public static SearchEngine Google
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "Google",
                    Url = "https://www.google.com/",
                    Prefix = "search?q="
                };
            }
        }

        public static SearchEngine Bing
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "Bing",
                    Url = "https://www.bing.com/",
                    Prefix = "search?q="
                };
            }
        }

        public static SearchEngine Yahoo
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "yahoo",
                    Url = "https://www.search.yahoo.com/",
                    Prefix = "search?p="

                };
            }
        }

        public static SearchEngine DuckDuckGo
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "DuckDuckGo",
                    Url = "https://duckduckgo.com/",
                    Prefix = "?q="
                };
            }
        }

        public static SearchEngine Qwant
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "Qwant",
                    Url = "https://www.qwant.com/",
                    Prefix = "?q="
                };
            }
        }

        public static SearchEngine YouDotCom
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "You.com",
                    Url = "https://you.com/",
                    Prefix = "search?q="
                };
            }
        }

        public static SearchEngine Startpage
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "Startpage",
                    Url = "https://www.startpage.com/",
                    Prefix = "do/search?query="
                };
            }
        }
    }


}
