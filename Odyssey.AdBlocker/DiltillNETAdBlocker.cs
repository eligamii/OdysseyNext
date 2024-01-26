using DistillNET;
using Microsoft.Web.WebView2.Core;
using Odyssey.Data.Settings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;


namespace Odyssey.AdBlocker
{
    public class DiltillNETAdBlocker // take 50 microseconds per requests and compatible with AdBlockPlus format 
    {
        private static FilterDbCollection _filterCollection;
        public static async void Init()
        {
            string dbCollectionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rules.db");
            _filterCollection = new FilterDbCollection(dbCollectionPath, true, true);

            // Create the db collection only if non already existant 
            if (!File.Exists(dbCollectionPath))
            {
                string assetsFile = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Odyssey.AdBlocker", "Assets");
                string easylistPath = Path.Combine(assetsFile, "easylist.txt");
                var easylistFileStream = File.OpenRead(easylistPath);

                _filterCollection.ParseStoreRulesFromStream(easylistFileStream, 2);
                _filterCollection.FinalizeForRead();
            }
        }

        public static void AttachCoreWebView2(CoreWebView2 coreWebView2)
        {
            coreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            coreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        }

        private static void CoreWebView2_WebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            if (Settings.IsAdBlockerEnabled)
            {
                Uri url = new Uri(args.Request.Uri);
                var headers = new NameValueCollection(StringComparer.OrdinalIgnoreCase)
                {
                    { "X-Requested-With", args.ResourceContext.ToString() },
                };

                foreach(var header in args.Request.Headers)
                {
                    var nvcol = new NameValueCollection(StringComparer.OrdinalIgnoreCase) { { header.Key, header.Value } };
                    headers.Add(nvcol);
                }

                var filters = _filterCollection.GetFiltersForDomain(url.Host);
                var whitelist = _filterCollection.GetWhitelistFiltersForDomain(url.Host);

                bool matches = filters.Any(p => p.IsMatch(url, headers)) && !whitelist.Any(p => p.IsMatch(url, headers));

                if(matches)
                {
                    sender.Stop();
                    args.Response = sender.Environment.CreateWebResourceResponse(null, 503, "Service Unavailable", "");
                    sender.Resume();
                }
            }
        }
    }
}
