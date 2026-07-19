using Lumora.Application.DTOs.Base;
using Lumora.Domain.Constants;

namespace Lumora.Web.Areas.Admin.Models;

public static class TranslationForm
{
    /// Admin forms always show one tab per supported language, in a fixed order —
    /// this pads/reorders whatever the service returned so form indices line up.
    public static List<T> Normalize<T>(IEnumerable<T>? existing) where T : BaseTranslationDto, new()
    {
        var list = existing?.ToList() ?? [];
        return LanguageConstants.All
            .Select(code =>
            {
                var translation = list.FirstOrDefault(t => t.LanguageCode == code) ?? new T();
                translation.LanguageCode = code;
                return translation;
            })
            .ToList();
    }
}
