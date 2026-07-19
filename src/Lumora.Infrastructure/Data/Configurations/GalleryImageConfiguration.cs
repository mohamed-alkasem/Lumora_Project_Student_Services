using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class UniversityImageConfiguration : IEntityTypeConfiguration<UniversityImage>
{
    public void Configure(EntityTypeBuilder<UniversityImage> builder)
    {
        builder.ToTable("UniversityImages");

        builder.Property(i => i.Url).IsRequired().HasMaxLength(500);

        builder.HasOne(i => i.University)
            .WithMany(u => u.Images)
            .HasForeignKey(i => i.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.UniversityId, i.DisplayOrder });
    }
}

public class HousingImageConfiguration : IEntityTypeConfiguration<HousingImage>
{
    public void Configure(EntityTypeBuilder<HousingImage> builder)
    {
        builder.ToTable("HousingImages");

        builder.Property(i => i.Url).IsRequired().HasMaxLength(500);

        builder.HasOne(i => i.Housing)
            .WithMany(h => h.Images)
            .HasForeignKey(i => i.HousingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.HousingId, i.DisplayOrder });
    }
}
