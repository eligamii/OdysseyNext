using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.Resources;

// This entire code is from https://github.com/files-community/Files/blob/main/src/Files.App/Helpers/ResourceHelpers.cs

namespace Odyssey.Helpers
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ResourceString : MarkupExtension
    {
        private static readonly ResourceLoader resourceLoader = new();

        public string Name { get; set; } = string.Empty;

        protected override object ProvideValue() => resourceLoader.GetString(Name);
    }
}
