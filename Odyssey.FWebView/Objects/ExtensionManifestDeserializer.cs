﻿using System.Collections.Generic;

namespace Odyssey.FWebView.Objects
{
    internal class ExtensionManifestDeserializer
    {
        public BrowserAction browser_action { get; set; }

        public class BrowserAction
        {
            public Dictionary<string, string> default_icon { get; set; }
            public string default_popup { get; set; }
            public string default_title { get; set; }
        }
    }
}
