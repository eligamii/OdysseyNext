using Monaco.Helpers;
using Odyssey.QuickActions.Objects;
using System;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Js
    {
        internal static async Task<Res> Exec(string[] options)
        {
            if (Variables.SelectedWebView != null)
            {
                string js = string.Empty;
                foreach (var option in options) js += option + " ";

                string res = await Variables.SelectedWebView.ExecuteScriptAsync(js);
                res = JavascriptHelpers.ToCSharpString(res);

                return new Res(true, res);
            }

            return new Res(false, null, "No tab is currently selected");
        }


    }
}
