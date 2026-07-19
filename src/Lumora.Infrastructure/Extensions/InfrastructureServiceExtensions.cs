using Lumora.Domain.Interfaces.Repositories;
using Lumora.Infrastructure.Data;
using Lumora.Infrastructure.Identity;
using Lumora.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lumora.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(
                    typeof(ApplicationDbContext).Assembly.FullName)
            ));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<Lumora.Domain.Interfaces.Services.IFileStorageService, Storage.LocalFileStorageService>();
        services.Configure<Email.SmtpOptions>(configuration.GetSection("Smtp"));
        services.AddSingleton<Lumora.Domain.Interfaces.Services.IEmailSender, Email.SmtpEmailSender>();
        services.AddScoped<IUniversityRepository, UniversityRepository>();
        services.AddScoped<IHousingRepository, HousingRepository>();
        services.AddScoped<IConsultationRepository, ConsultationRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();

        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
