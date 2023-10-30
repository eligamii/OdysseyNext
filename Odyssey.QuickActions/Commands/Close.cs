using Microsoft.UI.Windowing;
using Odyssey.Data.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Devices;

namespace Odyssey.QuickActions.Commands
{
    internal static class Close
    {
        internal static bool Exec(string[] options)
        {
            if (options.Count() == 0 || options[0] == "this")
            {
                // close the window
                QACommands.MainWindow.Close();
                return true;
            }
            else if (options[0] == "tabs")
            {
                // close every tab
                foreach(var item in Data.Main.Tabs.Items)
                {
                    // Close every tab's webview
                    if(item.MainWebView != null) { item.MainWebView.Close(); }                    
                }

                Tabs.Items.Clear();
                return true;
            }
            else
            {
                return false; // incorrect command
            }
        }

    }
}
