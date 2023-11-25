using Odyssey.Data.Main;
using Odyssey.QuickActions.Objects;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal class New
    {
        private static string url = string.Empty;
        internal static bool Exec(string[] options)
        {
            if (options.Count() == 2)
            {
                SetOptions(options[1]);

                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    switch (options[0])
                    {
                        case "tab": NewTab(); break;
                        case "pin": NewPin(); break;
                    }

                    return true;
                }
            }

            return false;
        }

        private static void NewPin()
        {
            Pin pin = new()
            {
                Url = url,
                ToolTip = url,
                Title = url
            };

            Pins.Items.Add(pin);
        }

        private static void NewTab()
        {
            Tab tab = new()
            {
                Url = url,
                ToolTip = url,
                Title = url
            };

            Tabs.Items.Add(tab);
        }


        private static void SetOptions(string option)
        {
            if (Option.IsAValidOptionString(option))
            {
                Option opt = new(option);

                switch (opt.Name)
                {
                    case "url": url = opt.Value; break;
                }
            }
        }
    }
}
