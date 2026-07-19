using Lumora.Domain.Entities;
using Lumora.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class ConsultationRequestConfiguration : IEntityTypeConfiguration<ConsultationRequest>
{
    public void Configure(EntityTypeBuilder<ConsultationRequest> builder)
    {
        builder.ToTable("ConsultationRequests");

        builder.Property(c => c.FullName).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Phone).IsRequired().HasMaxLength(30);
        builder.Property(c => c.Message).HasMaxLength(2000);

        builder.HasOne(c => c.University)
            .WithMany()
            .HasForeignKey(c => c.UniversityId)
            .OnDelete(DeleteBehavior.SetNull);

        // Restrict, not cascade: consultation requests are business leads and must survive
        // even if a student account is later removed (deactivate via ApplicationUser.IsActive instead).
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.StudentUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
