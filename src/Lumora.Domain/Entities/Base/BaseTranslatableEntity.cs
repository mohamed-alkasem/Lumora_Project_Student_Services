using Lumora.Domain.Constants;

namespace Lumora.Domain.Entities.Base;

public abstract class BaseTranslatableEntity<TTranslation> : BaseEntity
    where TTranslation : BaseTranslation
{
    public ICollection<TTranslation> Translations { get; set; } = new List<TTranslation>();

    public TTranslation? GetTranslation(string languageCode) =>
        Translations.FirstOrDefault(t => t.LanguageCode == languageCode);

    public TTranslation? GetTranslationWithFallback(
        string languageCode,
        string fallback = LanguageConstants.Default) =>
        GetTranslation(languageCode) ?? GetTranslation(fallback);
}
