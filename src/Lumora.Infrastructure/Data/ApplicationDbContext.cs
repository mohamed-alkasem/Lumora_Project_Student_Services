using Lumora.Domain.Entities;
using Lumora.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<City> Cities => Set<City>();
    public DbSet<CityTranslation> CityTranslations => Set<CityTranslation>();

    public DbSet<University> Universities => Set<University>();
    public DbSet<UniversityTranslation> UniversityTranslations => Set<UniversityTranslation>();

    public DbSet<AcademicProgram> AcademicPrograms => Set<AcademicProgram>();
    public DbSet<AcademicProgramTranslation> AcademicProgramTranslations => Set<AcademicProgramTranslation>();

    public DbSet<Housing> Housings => Set<Housing>();
    public DbSet<HousingTranslation> HousingTranslations => Set<HousingTranslation>();

    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<FaqTranslation> FaqTranslations => Set<FaqTranslation>();

    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<TestimonialTranslation> TestimonialTranslations => Set<TestimonialTranslation>();

    public DbSet<ConsultationRequest> ConsultationRequests => Set<ConsultationRequest>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    public DbSet<UniversityImage> UniversityImages => Set<UniversityImage>();
    public DbSet<HousingImage> HousingImages => Set<HousingImage>();

    public DbSet<ChatConversation> ChatConversations => Set<ChatConversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
