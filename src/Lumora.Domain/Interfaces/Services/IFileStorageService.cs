namespace Lumora.Domain.Interfaces.Services;

/// Abstraction over uploaded-file storage so callers never touch paths directly.
/// Current implementation stores under wwwroot/uploads (Infrastructure LocalFileStorageService);
/// swapping to cloud storage later only requires a new implementation of this interface.
public interface IFileStorageService
{
    /// Saves the content and returns the public relative URL (e.g. "/uploads/universities/abc123.png").
    Task<string> SaveAsync(Stream content, string originalFileName, string subfolder);

    /// Deletes a previously saved file by the relative URL SaveAsync returned. Missing files are ignored.
    Task DeleteAsync(string? relativeUrl);
}
