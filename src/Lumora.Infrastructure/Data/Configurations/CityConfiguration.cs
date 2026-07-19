using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");

        builder.HasMany(c => c.Translations)
            .WithOne(t => t.City)
            .HasForeignKey(t => t.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CityTranslationConfiguration : IEntityTypeConfiguration<CityTranslation>
{
    public void Configure(EntityTypeBuilder<CityTranslation> builder)
    {
        builder.ToTable("CityTranslations");

        builder.Property(t => t.Name).IsRequired().HasMaxLength(150);

        builder.HasIndex(t => new { t.CityId, t.LanguageCode }).IsUnique();
    }
}
