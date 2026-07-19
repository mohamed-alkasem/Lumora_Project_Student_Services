namespace Lumora.Application.DTOs.Base;

/// One gallery photo row as shown in the admin edit screens.
public class GalleryImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
}
