namespace Lumora.Domain.Entities.Base;

public abstract class BaseTranslation
{
    public int Id { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
}
