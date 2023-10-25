using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

// Every host blocklists can be found here : https://firebog.net/
// Credit to the Firebog, Easylist and Spam404 lists mainteners

namespace Odyssey.AdBlocker
{
    public class AdBlocker
    {
        private static string[] easylist;
        private static string[] easyprivacy;
        private static string[] malwaresList; // Spam404's main host blocklist
        private static string[] whitelist; // By me

        public static void Init()
        {
            if(easylist == null)
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
        }


        public AdBlocker(CoreWebView2 coreWebView) 
        {
            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.XmlHttpRequest);
            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Image);
            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Media);
            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Script);
            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Other);

            coreWebView.WebResourceRequested += CoreWebView_WebResourceRequested;
        }

        private void CoreWebView_WebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            string host = new Uri(args.Request.Uri).Host;

            if(ShouldBlock(host, sender))
            {
                sender.Stop(); // to have the time to block the request
                args.Response = sender.Environment.CreateWebResourceResponse(null, 503, "Service Unavailable", "");
                sender.Resume();
            }
        }

        private bool ShouldBlock(string host, CoreWebView2 sender)
        {
            if(!whitelist.Any(p => p == new Uri(sender.Source).Host))
            {
                if (easylist.Any(p => p == host)) return true;

                if (easyprivacy.Any(p => p == host)) return true;

                if (malwaresList.Any(p => p == host)) return true;
            }


            return false;
        }
    }
}
