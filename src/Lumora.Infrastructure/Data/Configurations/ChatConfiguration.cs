using Lumora.Domain.Entities;
using Lumora.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumora.Infrastructure.Data.Configurations;

public class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
{
    public void Configure(EntityTypeBuilder<ChatConversation> builder)
    {
        builder.ToTable("ChatConversations");

        builder.HasIndex(c => c.StudentUserId).IsUnique();

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Conversations survive account issues; deactivate the user instead of deleting.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.StudentUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");

        builder.Property(m => m.Text).IsRequired().HasMaxLength(2000);

        builder.HasIndex(m => new { m.ConversationId, m.CreatedAt });
    }
}
