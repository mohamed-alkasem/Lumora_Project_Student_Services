using Lumora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.ToTable("Testimonials");

        builder.Property(t => t.PhotoUrl).HasMaxLength(500);

        builder.HasMany(t => t.Translations)
            .WithOne(t => t.Testimonial)
            .HasForeignKey(t => t.TestimonialId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TestimonialTranslationConfiguration : IEntityTypeConfiguration<TestimonialTranslation>
{
    public void Configure(EntityTypeBuilder<TestimonialTranslation> builder)
    {
        builder.ToTable("TestimonialTranslations");

        builder.Property(t => t.StudentName).IsRequired().HasMaxLength(150);
        builder.Property(t => t.StudentTitle).IsRequired().HasMaxLength(200);
        builder.Property(t => t.QuoteText).IsRequired().HasMaxLength(1000);

        builder.HasIndex(t => new { t.TestimonialId, t.LanguageCode }).IsUnique();
    }
}
