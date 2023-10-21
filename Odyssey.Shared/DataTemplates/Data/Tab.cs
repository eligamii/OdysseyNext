using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.Shared.DataTemplates.Data
{
    public class Tab
    {
        public string Title { get; set; }
        public BitmapImage ImageSource { get; set; }
        public string ToolTip { get; set; }
        public object MainWebView { get; set; }
        public object SplitViewWebView { get; set; }
    }
}
