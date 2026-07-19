using Lumora.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Lumora.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private const string UploadsRoot = "uploads";

    /// Anything larger than this gets downscaled server-side; phone photos of 4000+px
    /// become web-friendly without the admin having to resize anything.
    private const int MaxDimension = 1920;

    private static readonly HashSet<string> RasterExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".webp" };

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
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

        // Buffer the upload so we can fall back to a raw copy if image processing fails.
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer);
        buffer.Position = 0;

        if (RasterExtensions.Contains(extension))
        {
            try
            {
                using var image = await Image.LoadAsync(buffer);
                image.Mutate(x => x.AutoOrient()); // respect phone EXIF rotation

                if (image.Width > MaxDimension || image.Height > MaxDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxDimension, MaxDimension)
                    }));
                }

                await using var target = File.Create(fullPath);
                switch (extension)
                {
                    case ".png":
                        await image.SaveAsPngAsync(target); // keeps transparency (logos)
                        break;
                    case ".webp":
                        await image.SaveAsWebpAsync(target, new WebpEncoder { Quality = 80 });
                        break;
                    default:
                        await image.SaveAsJpegAsync(target, new JpegEncoder { Quality = 82 });
                        break;
                }

                return $"/{UploadsRoot}/{safeSubfolder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Image processing failed for {File}; storing the original bytes instead.", originalFileName);
                buffer.Position = 0;
            }
        }

        await using (var raw = File.Create(fullPath))
        {
            await buffer.CopyToAsync(raw);
        }

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
