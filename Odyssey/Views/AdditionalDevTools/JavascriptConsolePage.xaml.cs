using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Monaco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey.Views.AdditionalDevTools
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JavascriptConsolePage : Page
    {
        public JavascriptConsolePage()
        {
            this.InitializeComponent();
            // Need to make a custom MonacoEditor control
            // With as js code:

            // => CodeEditor.Text
            // editor.getValue() to get text
            // editor.SetValue() to set text

            // => CodeEditor.Language, with Language an enum
            // var model = editor.getModel(); 
            // monaco.editor.setModelLanguage(model, "html/javascript") 
            // to set language

            // => CodeEditor.Theme, with theme a ElementTheme and if ElementTheme is default, the theme will auto change
            // monaco.editor.setTheme('vs') or monaco.editor.setTheme('vs-dark') to set theme

            // => CodeEditor.Reload() = webview.reload

            // => CodeEditor.WriteToFileAsync(path) = get the text the save it to path

            // => CodeEditor.OpenFileAsync(path, language) = open the file, set the text then set the language (default plaintext)

            // => CodeEditor.KeyDown (event) = see FWebView keydown helpers

            // => CodeEditor.ExecuteWithResultAsync(js) = WebView2.ExecuteScriptAsync()

            // => CodeEditor.GetAvailableLanguagesAsync() with result an array of languages enums

            // => CodeEditor.GetAvailableLanguagesAsStringsAsync() with result an array of monaco languages strings

            // => CodeEditor.SetLanguageStringAsync(string) = set the language

            // Block every DevTools, Error pages, ContextMenus, Local scripts, default dialogs, key accelerators, etc

        }


        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
