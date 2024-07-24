using Microsoft.UI.Xaml.Controls.Primitives;
using Odyssey.QuickActions.Controls;
using Odyssey.QuickActions.Objects;
using System;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal static class Flyout
    {
        private static string content = "about:blank"; // should be an url
        private static string pos = "0;0";
        private static string buttoncommand = string.Empty;
        internal static Res Exec(string[] options)
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
                flyoutOptions.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;

                flyout.webView.Source = new Uri(content);
                flyout.Command = buttoncommand;

                flyout.ShowAt(QACommands.Frame, flyoutOptions);


                return new Res(true);
            }
            else return new Res(false, null, "This command requires at least one option: content");
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
