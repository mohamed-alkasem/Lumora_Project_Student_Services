using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");

        builder.Property(a => a.CoverImageUrl).HasMaxLength(500);

        builder.HasMany(a => a.Translations)
            .WithOne(t => t.Article)
            .HasForeignKey(t => t.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ArticleTranslationConfiguration : IEntityTypeConfiguration<ArticleTranslation>
{
    public void Configure(EntityTypeBuilder<ArticleTranslation> builder)
    {
        builder.ToTable("ArticleTranslations");

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Summary).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Body).IsRequired(); // nvarchar(max) for long-form content

        builder.HasIndex(t => new { t.ArticleId, t.LanguageCode }).IsUnique();
    }
}
