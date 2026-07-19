using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class AcademicProgramConfiguration : IEntityTypeConfiguration<AcademicProgram>
{
    public void Configure(EntityTypeBuilder<AcademicProgram> builder)
    {
        builder.ToTable("AcademicPrograms");

        builder.HasMany(p => p.Translations)
            .WithOne(t => t.AcademicProgram)
            .HasForeignKey(t => t.AcademicProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AcademicProgramTranslationConfiguration : IEntityTypeConfiguration<AcademicProgramTranslation>
{
    public void Configure(EntityTypeBuilder<AcademicProgramTranslation> builder)
    {
        builder.ToTable("AcademicProgramTranslations");

        builder.Property(t => t.Name).IsRequired().HasMaxLength(150);

        builder.HasIndex(t => new { t.AcademicProgramId, t.LanguageCode }).IsUnique();
    }
}
