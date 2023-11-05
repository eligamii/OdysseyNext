using Microsoft.UI.Xaml.Controls.Primitives;
using Odyssey.QuickActions.Controls;
using Odyssey.QuickActions.Objects;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Odyssey.QuickActions.Commands
{
    internal static class Flyout
    {
        private static string content = "about:blank"; // should be an url
        private static string pos = "0;0";
        private static string buttoncommand = string.Empty;
        internal static bool Exec(string[] options)
        {
            if (options.Count() >= 1) // option requires at least 1 option: content
            {
                foreach (string option in options) { SetOptions(option); }

                string[] position = pos.Split(";");
                double x = double.Parse(position[0]);
                double y = double.Parse(position[1]);

                QAFlyout flyout = new();

                FlyoutShowOptions flyoutOptions = new();
                flyoutOptions.Position = new Windows.Foundation.Point(x, y);

                flyout.webView.Source = new Uri(content);
                flyout.Command = buttoncommand;

                flyout.ShowAt(QACommands.Frame, flyoutOptions);


                return true;
            }
            else return false;
        }

        private static void SetOptions(string option)
        {
            if (Option.IsAValidOptionString(option))
            {
                Option opt = new(option);

                switch (opt.Name)
                {
                    case "content": content = opt.Value; break;
                    case "pos": pos = opt.Value; break;
                    case "buttoncommand": buttoncommand = opt.Value; break;
                }
            }
        }

    }
}
