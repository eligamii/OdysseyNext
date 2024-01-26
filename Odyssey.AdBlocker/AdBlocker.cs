using Microsoft.Web.WebView2.Core;
using Odyssey.Data.Settings;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// Every host blocklists can be found here : https://firebog.net/
// Credit to the Firebog, Easylist and Spam404 lists mainteners

namespace Odyssey.AdBlocker
{
    public class AdBlocker // Instalnt and only compatible with host-based lists
    {
        private static string[] easylist;
        private static string[] easyprivacy;
        private static string[] malwaresList; // Spam404's main host blocklist
        private static string[] whitelist; // By me

        public static void Init()
        {
            if (easylist == null)
            {
                string assetsFile = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Odyssey.AdBlocker", "Assets");

                string easylistPath = Path.Combine(assetsFile, "Easylist.txt");
                string easyPrivacyPath = Path.Combine(assetsFile, "Easyprivacy.txt");
                string malwaresListPath = Path.Combine(assetsFile, "MalwaresList.txt");
                string whitelistListPath = Path.Combine(assetsFile, "Whitelist.txt");

                easylist = File.ReadAllLines(easylistPath);
                easyprivacy = File.ReadAllLines(easyPrivacyPath);
                malwaresList = File.ReadAllLines(malwaresListPath);
                whitelist = File.ReadAllLines(whitelistListPath);
            }

            if (EasyListAdBlocker.AdBlockList.Count() == 0 && Settings.AdBlockerType == 1)
            {
                EasyListAdBlocker.CreateRegexBasedFilterList();
            }
        }


        public AdBlocker(CoreWebView2 coreWebView)
        {
            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

            coreWebView.WebResourceRequested += CoreWebView_WebResourceRequested;
        }

        private async void CoreWebView_WebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            if (Settings.IsAdBlockerEnabled)
            {
                if (Settings.AdBlockerType == 0) // host-based
                {
                    string host = new Uri(args.Request.Uri).Host;

                    if (ShouldBlockHost(host, sender))
                    {
                        sender.Stop(); // to have the time to block the request
                        args.Response = sender.Environment.CreateWebResourceResponse(null, 503, "Service Unavailable", "");
                        sender.Resume();
                    }
                }
                else
                {
                    if (await ShouldBlock(args.Request.Uri, args.ResourceContext, new Uri(sender.Source).Host))
                    {
                        sender.Stop(); // to have the time to block the request
                        args.Response = sender.Environment.CreateWebResourceResponse(null, 503, "Service Unavailable", "");
                        sender.Resume();
                    }
                }
            }
        }

        private async Task<bool> ShouldBlock(string request, CoreWebView2WebResourceContext context, string host)
        {

            return await EasyListAdBlocker.ShouldBlock(request, host, context);
        }

        private bool ShouldBlockHost(string host, CoreWebView2 sender)
        {
            if (!whitelist.Any(p => p == new Uri(sender.Source).Host))
            {
                if (easylist.Any(p => p == host)) return true;

                if (easyprivacy.Any(p => p == host)) return true;

                if (malwaresList.Any(p => p == host)) return true;
            }


            return false;
        }
    }
}
