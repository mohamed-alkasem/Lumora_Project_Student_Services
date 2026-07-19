using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class ScholarshipConfiguration : IEntityTypeConfiguration<Scholarship>
{
    public void Configure(EntityTypeBuilder<Scholarship> builder)
    {
        builder.ToTable("Scholarships");

        builder.Property(s => s.OriginalPriceUsd).HasColumnType("decimal(10,2)");
        builder.Property(s => s.DiscountedPriceUsd).HasColumnType("decimal(10,2)");
        builder.Property(s => s.ImageUrl).HasMaxLength(500);

        builder.HasMany(s => s.Translations)
            .WithOne(t => t.Scholarship)
            .HasForeignKey(t => t.ScholarshipId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.University)
            .WithMany()
            .HasForeignKey(s => s.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ScholarshipTranslationConfiguration : IEntityTypeConfiguration<ScholarshipTranslation>
{
    public void Configure(EntityTypeBuilder<ScholarshipTranslation> builder)
    {
        builder.ToTable("ScholarshipTranslations");

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(2000);

        builder.HasIndex(t => new { t.ScholarshipId, t.LanguageCode }).IsUnique();
    }
}
