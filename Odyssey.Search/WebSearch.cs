using Microsoft.UI.Xaml;
using Odyssey.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.Search
{
    public static class WebSearch
    {
        public static string Search(string query, FWebView initializedWebView)
        {
            if (FWebView.CurrentlySelected == null)
            {
                string s = "t";
                return s;
            }
            return "t";
        }
    }
}
