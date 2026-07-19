using Lumora.Domain.Entities;
using Lumora.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites");

        builder.HasOne(f => f.University)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK-only relationship to ApplicationUser: Favorite (Domain) has no navigation property
        // to it since Domain must not depend on Infrastructure's Identity types.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(f => f.StudentUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.StudentUserId, f.UniversityId }).IsUnique();
    }
}
