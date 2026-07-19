using Lumora.Application.DTOs.Consultations;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Common;
using Lumora.Domain.Entities;
using Lumora.Domain.Enums;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class ConsultationService : IConsultationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConsultationRepository _consultations;

    public ConsultationService(IUnitOfWork unitOfWork, IConsultationRepository consultations)
    {
        _unitOfWork = unitOfWork;
        _consultations = consultations;
    }

    public async Task<int> CreateAsync(int studentUserId, ConsultationCreateDto dto)
    {
        var request = new ConsultationRequest
        {
            StudentUserId = studentUserId,
            FullName = dto.FullName.Trim(),
            Phone = dto.Phone.Trim(),
            Message = dto.Message?.Trim(),
            UniversityId = dto.UniversityId,
            Status = ConsultationStatus.New
        };

        await _consultations.AddAsync(request);
        await _unitOfWork.CommitAsync();
        return request.Id;
    }

    public async Task<PagedResult<ConsultationAdminDto>> GetPagedForAdminAsync(
        ConsultationFilterDto filter, string languageCode)
    {
        var result = await _consultations.GetPagedAsync(new ConsultationQueryParameters
        {
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        });

        return result.Map(c => new ConsultationAdminDto
        {
            Id = c.Id,
            CreatedAt = c.CreatedAt,
            StudentUserId = c.StudentUserId,
            FullName = c.FullName,
            Phone = c.Phone,
            Message = c.Message,
            UniversityName = c.University?.GetTranslationWithFallback(languageCode)?.Name,
            Status = c.Status
        });
    }

    public async Task<bool> UpdateStatusAsync(int id, ConsultationStatus status)
    {
        var request = await _consultations.GetByIdAsync(id);
        if (request is null)
        {
            return false;
        }

        request.Status = status;
        _consultations.Update(request);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
