namespace Lumora.Domain.Constants;

public static class LanguageConstants
{
    public const string English  = "en";
    public const string Arabic   = "ar";
    public const string Turkish  = "tr";
    public const string Turkmen  = "tk";
    public const string Persian  = "fa";

    public const string Default  = English;

    public static readonly string[] All = [English, Arabic, Turkish, Turkmen, Persian];

    // RTL writing-direction languages
    private static readonly HashSet<string> RtlLanguages = [Arabic, Persian];

    public static bool IsRtl(string languageCode) =>
        RtlLanguages.Contains(languageCode);

    public static string DirectionOf(string languageCode) =>
        IsRtl(languageCode) ? "rtl" : "ltr";

    /// Curated display names for the language switcher — CultureInfo.NativeName gives
    /// inconsistent casing/wording (e.g. lowercase "türkmen dili").
    public static string NativeNameOf(string languageCode) => languageCode switch
    {
        English => "English",
        Arabic  => "العربية",
        Turkish => "Türkçe",
        Turkmen => "Türkmençe",
        Persian => "فارسی",
        _ => languageCode
    };
}
