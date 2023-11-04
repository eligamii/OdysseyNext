using Microsoft.Web.WebView2.Core;
using Odyssey.Data.Settings;
using System.Collections.Generic;

namespace Odyssey.QuickActions
{
    public class Variables
    {
        public static CoreWebView2ContextMenuRequestedEventArgs ContextMenuArgs { internal get; set; } // for the <linkurl>, <selectionurl>,... variables
        public static string AskText { get; set; }
        internal static string QAFlyoutUrl { get; set; } = string.Empty;
        internal static List<KeyValuePair<string, string>> UserVariables { get; set; } = new();
        internal static List<KeyValuePair<string, string>> SessionUserVariables { get; set; } = new();


        internal static string VariablesToValues(string command) // ex: $command option:<pos> => $command option:"172;537"
        {
            string resultCommand = string.Empty;

            resultCommand = command.Replace("<pointerpos>", PointerPos)
                                   .Replace("<linkurl", LinkUrl)
                                   .Replace("<searchurl>", SearchUrl)
                                   .Replace("<selectiontext>", SelectionText)
                                   .Replace("<ask>", AskText)
                                   .Replace("<flyouturl>", QAFlyoutUrl)
                                   ;

            foreach (var variable in UserVariables)
            {
                resultCommand = resultCommand.Replace($"<{variable.Key}>", variable.Value);
            }

            foreach (var variable in SessionUserVariables)
            {
                resultCommand = resultCommand.Replace($"<{variable.Key}>", variable.Value);
            }

            return resultCommand;
        }


        //************* QACommands **************
        private static string PointerPos
        {
            get { if (ContextMenuArgs != null) return $"{ContextMenuArgs.Location.X};{ContextMenuArgs.Location.Y}"; else return "0;0"; }
        }

        private static string SearchUrl { get { return SearchEngine.ToSearchEngineObject((SearchEngines)Settings.SelectedSearchEngine).SearchUrl; } }



        //************* WebView **************

        // The right-clicked webview weblink (if one)
        private static string LinkUrl
        {
            get
            {
                if (ContextMenuArgs != null)
                {
                    if (ContextMenuArgs.ContextMenuTarget.HasLinkUri)
                    {
                        return ContextMenuArgs.ContextMenuTarget.LinkUri;
                    }
                }

                return string.Empty;
            }
        }

        private static string SelectionText
        {
            get
            {
                if (ContextMenuArgs != null)
                {
                    if (ContextMenuArgs.ContextMenuTarget.HasSelection)
                    {
                        return ContextMenuArgs.ContextMenuTarget.SelectionText;
                    }
                }

                return string.Empty;
            }
        }
    }
}
