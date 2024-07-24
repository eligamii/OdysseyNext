﻿using Odyssey.QuickActions.Objects;

namespace Odyssey.QuickActions.Commands
{
    internal class Refresh
    {
        internal static Res Exec(string[] options)
        {
            if (Variables.SelectedWebView != null)
            {
                Variables.SelectedWebView.Reload();
                return new Res(true);
            }
            return new Res(false, null, "no tab is currently selected");
        }
    }
}
