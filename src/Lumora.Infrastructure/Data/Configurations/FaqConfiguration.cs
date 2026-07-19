using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> builder)
    {
        builder.ToTable("Faqs");

        builder.HasMany(f => f.Translations)
            .WithOne(t => t.Faq)
            .HasForeignKey(t => t.FaqId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FaqTranslationConfiguration : IEntityTypeConfiguration<FaqTranslation>
{
    public void Configure(EntityTypeBuilder<FaqTranslation> builder)
    {
        builder.ToTable("FaqTranslations");

        builder.Property(t => t.Question).IsRequired().HasMaxLength(300);
        builder.Property(t => t.Answer).IsRequired().HasMaxLength(2000);

        builder.HasIndex(t => new { t.FaqId, t.LanguageCode }).IsUnique();
    }
}
