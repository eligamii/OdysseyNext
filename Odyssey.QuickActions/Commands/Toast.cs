using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System.Linq;
using System.Text.RegularExpressions;

namespace Odyssey.QuickActions.Commands
{
    internal class Toast
    {
        private static string title = "Toast";
        private static string content = string.Empty;
        private static AppNotificationDuration duration = AppNotificationDuration.Default;
        internal static bool Exec(string[] options)
        {
            if (options.Count() >= 1)
            {
                foreach (string option in options) SetOptions(option);

                if (content != string.Empty)
                {
                    // Send toast notification
                    var toast = new AppNotificationBuilder()
                    .AddText(title)
                    .AddText(content)
                    .SetDuration(duration)
                    .BuildNotification();

                    AppNotificationManager.Default.Show(toast);

                    title = "Toast"; // set default title
                    return true;
                }
            }

            return false; // This command requires at least 1 option: content
        }

        private static void SetOptions(string option)
        {
            string optionSeparatorRegex = @"([-a-zA-Z0-9()@%_\+.~#?&/\\=;]|"".*"")*";

            string optionName = Regex.Match(option, optionSeparatorRegex).Value;
            string optionValue = Regex.Matches(option, optionSeparatorRegex).Select(p => p.Value).ElementAt(2); // every two value is empty

            if (optionValue.StartsWith("\""))
                optionValue = optionValue.Remove(0, 1).Remove(optionValue.Length - 2, 1);


            switch (optionName)
            {
                case "title": title = optionValue; break;
                case "content": content = optionValue; break;
                case "duration":
                    if (optionValue == "long") { duration = AppNotificationDuration.Long; }
                    else { duration = AppNotificationDuration.Default; }
                    break;
            }
        }
    }
}
