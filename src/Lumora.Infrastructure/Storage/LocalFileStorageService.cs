using Lumora.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace Lumora.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private const string UploadsRoot = "uploads";
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(Stream content, string originalFileName, string subfolder)
    {
        // Never trust the original name: keep only a whitelisted extension, generate the rest.
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        var safeSubfolder = Path.GetFileName(subfolder); // strips any path segments

        var directory = Path.Combine(_environment.WebRootPath, UploadsRoot, safeSubfolder);
        Directory.CreateDirectory(directory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(directory, fileName);

        await using var target = File.Create(fullPath);
        await content.CopyToAsync(target);

        return $"/{UploadsRoot}/{safeSubfolder}/{fileName}";
    }

    public Task DeleteAsync(string? relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl))
        {
            return Task.CompletedTask;
        }

        // Only ever delete inside wwwroot/uploads, regardless of what the URL claims.
        var relative = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, relative));
        var uploadsPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, UploadsRoot));

        if (fullPath.StartsWith(uploadsPath, StringComparison.OrdinalIgnoreCase) && File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
