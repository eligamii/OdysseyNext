﻿using System.Reflection.Metadata.Ecma335;
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
                case SearchEngines.DuckDuckGo: return DuckDuckGo;
                case SearchEngines.Startpage: return Startpage;
                case SearchEngines.YouDotCom: return YouDotCom;
                case SearchEngines.Google: return Google;
                case SearchEngines.Qwant: return Qwant;
                case SearchEngines.Yahoo: return Yahoo;
                case SearchEngines.Bing: return Bing;
                case SearchEngines.PerplexityAI: return PerplexityAI;
                case SearchEngines.Kagi: return Kagi;
                default: return null;
            }
        }

        
        public static SearchEngine SelectedSearchEngine
        {
            get => ToSearchEngineObject((SearchEngines)Settings.SelectedSearchEngine);
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

        public static SearchEngine PerplexityAI
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "Perplexity AI",
                    Url = "https://www.perplexity.ai/",
                    Prefix = "search?focus=internet&q="
                };
            }
        }

        public static SearchEngine Kagi
        {
            get
            {
                return new SearchEngine()
                {
                    Name = "Kagi",
                    Url = "https://kagi.com/",
                    Prefix = "search?q="
                };
            }
        }
    }


}
