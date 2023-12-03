using Monaco.Helpers;
using Odyssey.FWebView;
using Odyssey.QuickActions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Js
    {
        internal static async Task<Res> Exec(string[] options)
        {
            string js = string.Empty;
            foreach (var option in options) js += option + " ";

            string res = await WebView.SelectedWebView.ExecuteScriptAsync(js);
            res = JavascriptHelpers.ToCSharpString(res);

            return new Res(true, res);
        }


    }
}
