namespace Lumora.Web.Areas.Admin.Models;

public static class AdminUpload
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".webp", ".svg" };

    public const long MaxBytes = 2 * 1024 * 1024;

    /// Returns null when valid, otherwise the resource key of the error message.
    public static string? Validate(IFormFile file)
    {
        if (file.Length == 0 || file.Length > MaxBytes)
        {
            return "ImageTooLarge";
        }

        var extension = Path.GetExtension(file.FileName);
        return AllowedExtensions.Contains(extension) ? null : "ImageTypeNotAllowed";
    }
}
