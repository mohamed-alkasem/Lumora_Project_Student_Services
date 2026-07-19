using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("Universities");

        builder.Property(u => u.Rating).HasColumnType("decimal(3,2)");
        builder.Property(u => u.AnnualTuitionUsd).HasColumnType("decimal(10,2)");
        builder.Property(u => u.LogoUrl).HasMaxLength(500);

        builder.HasMany(u => u.Translations)
            .WithOne(t => t.University)
            .HasForeignKey(t => t.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict, not cascade: a city with universities must be reassigned before it can be removed.
        builder.HasOne(u => u.City)
            .WithMany(c => c.Universities)
            .HasForeignKey(u => u.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Programs)
            .WithMany(p => p.Universities)
            .UsingEntity(j => j.ToTable("UniversityAcademicPrograms"));
    }
}

public class UniversityTranslationConfiguration : IEntityTypeConfiguration<UniversityTranslation>
{
    public void Configure(EntityTypeBuilder<UniversityTranslation> builder)
    {
        builder.ToTable("UniversityTranslations");

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.ShortDescription).IsRequired().HasMaxLength(1000);

        builder.HasIndex(t => new { t.UniversityId, t.LanguageCode }).IsUnique();
    }
}
