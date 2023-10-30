using Odyssey.Data.Main;
using Odyssey.Shared.ViewModels.Data;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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
            string optionSeparatorRegex = @"([-a-zA-Z0-9()@%_\+.~#?&/\\=;]|""[-a-zA-Z0-9()@:%_\+.~#?&/\\=; ]*"")*";

            string optionName = Regex.Match(option, optionSeparatorRegex).Value;
            string optionValue = Regex.Matches(option, optionSeparatorRegex).Select(p => p.Value).ElementAt(2); // every two value is empty

            if (optionValue.StartsWith("\""))
                optionValue = optionValue.Remove(0, 1).Remove(optionValue.Length - 2, 1);

            switch (optionName)
            {
                case "url": url = optionValue; break;
            }
        }
    }
}
