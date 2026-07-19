using Lumora.Application.DTOs.Articles;
using Lumora.Application.DTOs.Cities;
using Lumora.Application.DTOs.Consultations;
using Lumora.Application.DTOs.Faqs;
using Lumora.Application.DTOs.Housings;
using Lumora.Application.DTOs.Programs;
using Lumora.Application.DTOs.Scholarships;
using Lumora.Application.DTOs.Testimonials;
using Lumora.Application.DTOs.Universities;
using Lumora.Domain.Common;
using Lumora.Domain.Enums;

namespace Lumora.Application.Interfaces.Services;

public interface ICityService
    : ITranslatableService<CityDto, CityCreateDto, CityUpdateDto>
{
}

public interface IAcademicProgramService
    : ITranslatableService<AcademicProgramDto, AcademicProgramCreateDto, AcademicProgramUpdateDto>
{
}

public interface IFaqService
    : ITranslatableService<FaqDto, FaqCreateDto, FaqUpdateDto>
{
    /// Public page: ordered by DisplayOrder.
    Task<IReadOnlyList<FaqDto>> GetOrderedAsync(string languageCode);
}

public interface ITestimonialService
    : ITranslatableService<TestimonialDto, TestimonialCreateDto, TestimonialUpdateDto>
{
    /// Public "About us" section: published only, ordered by DisplayOrder.
    Task<IReadOnlyList<TestimonialDto>> GetPublishedAsync(string languageCode);
}

public interface IUniversityService
    : ITranslatableService<UniversityDetailsDto, UniversityCreateDto, UniversityUpdateDto>
{
    Task<PagedResult<UniversityCardDto>> SearchAsync(UniversityFilterDto filter, string languageCode);

    Task<UniversityTypeCountsDto> GetTypeCountsAsync();

    // Gallery management (admin)
    Task<IReadOnlyList<Lumora.Application.DTOs.Base.GalleryImageDto>> GetImagesAsync(int universityId);
    Task AddImageAsync(int universityId, string url);
    /// Returns the removed image's url (for physical file cleanup), or null when not found.
    Task<string?> RemoveImageAsync(int imageId);
}

public interface IHousingService
    : ITranslatableService<HousingDetailsDto, HousingCreateDto, HousingUpdateDto>
{
    Task<PagedResult<HousingCardDto>> SearchAsync(HousingFilterDto filter, string languageCode);

    // Gallery management (admin)
    Task<IReadOnlyList<Lumora.Application.DTOs.Base.GalleryImageDto>> GetImagesAsync(int housingId);
    Task AddImageAsync(int housingId, string url);
    /// Returns the removed image's url (for physical file cleanup), or null when not found.
    Task<string?> RemoveImageAsync(int imageId);
}

public interface IScholarshipService
    : ITranslatableService<ScholarshipCardDto, ScholarshipCreateDto, ScholarshipUpdateDto>
{
    /// Public page: active offers only, ordered by DisplayOrder then newest.
    Task<IReadOnlyList<ScholarshipCardDto>> GetActiveAsync(string languageCode);
}

public interface IArticleService
    : ITranslatableService<ArticleDetailsDto, ArticleCreateDto, ArticleUpdateDto>
{
    /// Public list: published only, newest first.
    Task<IReadOnlyList<ArticleCardDto>> GetPublishedAsync(string languageCode);
}

public interface IConsultationService
{
    Task<int> CreateAsync(int studentUserId, ConsultationCreateDto dto);
    Task<PagedResult<ConsultationAdminDto>> GetPagedForAdminAsync(ConsultationFilterDto filter, string languageCode);
    Task<bool> UpdateStatusAsync(int id, ConsultationStatus status);
}

public interface IChatService
{
    /// Returns the student's single conversation id, creating it on first use.
    Task<int> GetOrCreateConversationIdAsync(int studentUserId);

    /// Null when the conversation does not exist.
    Task<int?> GetConversationStudentIdAsync(int conversationId);

    Task<IReadOnlyList<Lumora.Application.DTOs.Chat.ChatMessageDto>> GetMessagesAsync(int conversationId);

    Task<Lumora.Application.DTOs.Chat.ChatMessageDto> SendAsync(
        int conversationId, int senderUserId, bool isFromAdmin, string text);

    /// Admin inbox: newest activity first, with unread-from-student counts.
    Task<IReadOnlyList<Lumora.Application.DTOs.Chat.ChatConversationDto>> GetConversationsForAdminAsync();

    /// Marks the messages written by the OTHER party as read.
    Task MarkReadAsync(int conversationId, bool readerIsAdmin);
}

public interface IFavoriteService
{
    /// Returns true when the university is now a favorite, false when it was removed.
    Task<bool> ToggleAsync(int studentUserId, int universityId);

    Task<IReadOnlyList<UniversityCardDto>> GetMyFavoritesAsync(int studentUserId, string languageCode);

    /// Ids the public directory uses to paint the heart icons for the current student.
    Task<IReadOnlySet<int>> GetMyFavoriteIdsAsync(int studentUserId);
}
