using Odyssey.FWebView;
using Odyssey.QuickActions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Js
    {
        internal static Res Exec(string[] options)
        {
            string js = string.Empty;
            foreach (var option in options) js += option + " ";

            WebView.SelectedWebView.ExecuteScriptAsync(js);

            return new Res(true);
        }


    }
}
