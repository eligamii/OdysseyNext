﻿using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.Resources;

namespace Odyssey.Helpers
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ResourceString : MarkupExtension
    {
        private static ResourceLoader resourceLoader;

        public string Name { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;

        protected override object ProvideValue()
        {
            resourceLoader = new(Filename);
            return resourceLoader.GetString(Name);
         
        }
    }
}
