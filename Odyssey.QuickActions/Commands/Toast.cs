using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Odyssey.QuickActions.Objects;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal class Toast
    {
        private static string title = "Toast";
        private static string content = string.Empty;
        private static AppNotificationDuration duration = AppNotificationDuration.Default;
        internal static Res Exec(string[] options)
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
                    content = string.Empty;
                    return new Res(true);
                }
            }

            return new Res(false, null, "This command requires at least 1 option: content");
        }

        private static void SetOptions(string option)
        {
            if (Option.IsAValidOptionString(option))
            {
                Option opt = new(option);

                switch (opt.Name)
                {
                    case "title": title = opt.Value; break;
                    case "content": content = opt.Value; break;
                    case "duration":
                        if (opt.Value == "long") { duration = AppNotificationDuration.Long; }
                        else { duration = AppNotificationDuration.Default; }
                        break;
                }
            }
        }
    }
}
