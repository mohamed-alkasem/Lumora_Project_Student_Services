using Lumora.Application.Interfaces.Services;
using Lumora.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lumora.Web.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IAcademicProgramService, AcademicProgramService>();
        services.AddScoped<IUniversityService, UniversityService>();
        services.AddScoped<IHousingService, HousingService>();
        services.AddScoped<IFaqService, FaqService>();
        services.AddScoped<ITestimonialService, TestimonialService>();
        services.AddScoped<IConsultationService, ConsultationService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IScholarshipService, ScholarshipService>();
        services.AddScoped<IArticleService, ArticleService>();

        return services;
    }
}
