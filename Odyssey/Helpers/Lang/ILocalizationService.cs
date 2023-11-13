using Odyssey.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odyssey.Helpers
{
    public interface ILocalizationService
    {
        List<LanguageItem> Languages { get; }

        LanguageItem GetCurrentLanguageItem();
        Task InitializeAsync();
        Task SetLanguageAsync(LanguageItem languageItem);
    }
}