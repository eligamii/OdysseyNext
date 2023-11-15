using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Monaco
{
    public sealed partial class Editor : UserControl
    {
        private string ToCSharpString(string str)
        {
            str = str.Replace(@"\\", "\\")
                     .Replace(@"\n", "\n")
                     .Replace(@"\r", "\r")
                     .Replace(@"\t", "\t")
                     .Replace(@"\b", "\b")
                     .Replace(@"\f", "\f");

            str = str.Substring(1);
            str = str.Remove(str.Length - 1);
            return str;
        }

        public delegate void LoadedEventHandler(object sender, object args);

        public new event LoadedEventHandler Loaded;

        public delegate void TextChangedEventHandler(object sender, string args);

        public event TextChangedEventHandler TextChanged;


        public Editor()
        {
            this.InitializeComponent();
            string path = "file:///" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonacoEditorSource\\index.html").Replace("\\", "/");
            webView.Source = new Uri(path);

            this.ActualThemeChanged += Editor_ActualThemeChanged;


            webView.CoreWebView2Initialized += (s, a) =>
            {
                var c = s.CoreWebView2.Settings;
                c.AreDefaultContextMenusEnabled = false;
                c.AreDevToolsEnabled = false;
                c.AreBrowserAcceleratorKeysEnabled = false;
                c.IsBuiltInErrorPageEnabled = false;
                c.IsStatusBarEnabled = false;
                c.IsWebMessageEnabled = false;

                s.CoreWebView2.NavigationCompleted += async (s, a) =>
                {
                    await s.ExecuteScriptAsync("editor.getModel().onDidChangeContent((event) => {\r\n  console.log(\"TEXTCHANGED\")\r\n});");
                    RefreshTheme();
                    Loaded(this, null);
                };

                s.CoreWebView2.WebMessageReceived += async (s, a) =>
                {
                    if (a.WebMessageAsJson == @"""TEXTCHANGED""")
                    {
                        string text = await GetTextAsync();
                        TextChanged(this, text);
                    }
                };
            };


            Loaded += (s, a) => { };
            TextChanged += (s, a) => { };
        }

        private void Editor_ActualThemeChanged(FrameworkElement sender, object args)
        {
            RefreshTheme();
        }

        private async void RefreshTheme()
        {
            string themeStr = this.ActualTheme == ElementTheme.Light ? "vs" : "vs-dark";
            await webView.ExecuteScriptAsync($"monaco.editor.setTheme(\"{themeStr}\")");
        }

        public async void SetLanguage(Language language)
        {
            string id = LanguageCoverter.LanguageEnumToString(language);
            await webView.ExecuteScriptAsync("var model = editor.getModel();" +
                                             $"monaco.editor.setModelLanguage(model, \"{id}\")");
        }

        /// <summary>
        /// monaco.editor.getValue()
        /// </summary>
        public async Task<string> GetTextAsync()
        {
            string str = await webView.CoreWebView2.ExecuteScriptAsync("editor.getValue()");
            return ToCSharpString(str);
        }
        /// <summary>
        /// monaco.editor.setValue(str)
        /// </summary>
        public async void SetText(string str)
        {
            await webView.CoreWebView2.ExecuteScriptAsync($"editor.setValue({str})");
        }

        public async Task OpenFileAsync(string path)
        {
            if (File.Exists(path))
            {
                string fileString = File.ReadAllText(path);
                await webView.CoreWebView2.ExecuteScriptAsync($"editor.setValue({fileString})");
            }
            else throw new FileNotFoundException();
        }

        public async Task SaveToFileAsync(string path)
        {
            string text = await GetTextAsync();
            File.WriteAllText(path, text);
        }

    }
}
