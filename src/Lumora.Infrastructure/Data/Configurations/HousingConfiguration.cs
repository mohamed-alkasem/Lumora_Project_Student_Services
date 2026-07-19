using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class HousingConfiguration : IEntityTypeConfiguration<Housing>
{
    public void Configure(EntityTypeBuilder<Housing> builder)
    {
        builder.ToTable("Housings");

        builder.Property(h => h.PricePerMonthUsd).HasColumnType("decimal(10,2)");
        builder.Property(h => h.ImageUrl).HasMaxLength(500);

        builder.HasMany(h => h.Translations)
            .WithOne(t => t.Housing)
            .HasForeignKey(t => t.HousingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.City)
            .WithMany(c => c.Housings)
            .HasForeignKey(h => h.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class HousingTranslationConfiguration : IEntityTypeConfiguration<HousingTranslation>
{
    public void Configure(EntityTypeBuilder<HousingTranslation> builder)
    {
        builder.ToTable("HousingTranslations");

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(2000);
        builder.Property(t => t.Address).IsRequired().HasMaxLength(300);

        builder.HasIndex(t => new { t.HousingId, t.LanguageCode }).IsUnique();
    }
}
