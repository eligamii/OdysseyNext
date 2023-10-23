using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Odyssey.FWebView.Classes;
using Odyssey.FWebView.Controls;
using Odyssey.Shared.DataTemplates.Data;
using Odyssey.WebSearch.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static Odyssey.WebSearch.Helpers.WebUrlHelpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.FWebView
{
    public sealed partial class WebView : WebView2
    {
        public bool IsSelected { get; set; } = true;
        public Tab LinkedTab { get; set; }
        public Tab ParentTab { get; set; } = null; // used when a webView is assiociated to another (login, etc)
        public Favorite LinkedFavorite { get; set; } = null;
        public static new XamlRoot XamlRoot { get; set; }
        public static WebView New(string url = "about:blank")
        {
            WebView newWebView = new();
            newWebView.Source = new Uri(url);

            return newWebView;
        }

        public WebView()
        {
            this.InitializeComponent();

        }

        private void WebView2_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            sender.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;

            sender.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
        }

        private void CoreWebView2_NavigationStarting(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            if(GetStringKind(args.Uri) == StringKind.ExternalAppUri)
            {
                args.Cancel = true;
                AppUriLaunch.Launch(new Uri(args.Uri));
            }
        }

        private void CoreWebView2_ContextMenuRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs args)
        {
            FWebViewContextMenu fWebViewContextMenu = new();
            fWebViewContextMenu.Show(this, args);
        }

        private void CoreWebView2_DocumentTitleChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, object args)
        {
            LinkedTab.Title = sender.DocumentTitle;
        }
    }
}
