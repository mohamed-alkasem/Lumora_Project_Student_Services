using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;
using Lumora.Domain.Enums;

namespace Lumora.Application.DTOs.Consultations;

/// Submitted by a logged-in student from the public site.
public class ConsultationCreateDto
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Message { get; set; }

    public int? UniversityId { get; set; }
}

/// Row in the admin inbox.
public class ConsultationAdminDto : BaseDto
{
    public int StudentUserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? UniversityName { get; set; }
    public ConsultationStatus Status { get; set; }
}

public class ConsultationFilterDto
{
    public ConsultationStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
