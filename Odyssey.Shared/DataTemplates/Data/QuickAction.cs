using Microsoft.Web.WebView2.Core;
using Odyssey.Shared.Enums;

namespace Odyssey.Shared.DataTemplates.Data
{
    public class QuickAction
    {
        public string Label { get; set; }
        public int Icon { get; set; }
        public string Command { get; set; }
        public QuickActionItemShowOptions ShowOptions { get; set; }



        private bool CondiditonsAreMet(CoreWebView2ContextMenuRequestedEventArgs args)
        {
            switch (ShowOptions.ShowCondition)
            {
                case QuickActionShowCondition.None: return true;

                case QuickActionShowCondition.HasLinkText:
                    return args.ContextMenuTarget.HasLinkText;

                case QuickActionShowCondition.HasLinkUri:
                    return args.ContextMenuTarget.HasLinkUri;

                case QuickActionShowCondition.HasSourceUri:
                    return args.ContextMenuTarget.HasSourceUri;

                case QuickActionShowCondition.HasSelection:
                    return args.ContextMenuTarget.HasSelection;

                case QuickActionShowCondition.IsEditable:
                    return args.ContextMenuTarget.IsEditable;

                default: return false;
            }
        }

        public class QuickActionItemShowOptions
        {
            public QuickActionShowType Type { get; set; } = QuickActionShowType.ContextMenuItem;
            public QuickActionShowPosition Position { get; set; } = QuickActionShowPosition.Top;
            public QuickActionShowCondition ShowCondition { get; set; } = QuickActionShowCondition.None;
            public string UrlRegex { get; set; }
        }
    }
}
