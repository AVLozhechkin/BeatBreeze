using BeatBreeze.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeatBreeze.Infrastructure.Database.Configurations;

internal sealed class PlaylistItemConfiguration : IEntityTypeConfiguration<PlaylistItem>
{
    public void Configure(EntityTypeBuilder<PlaylistItem> builder)
    {
        builder
            .HasIndex(pi => new { ProviderId = pi.MusicFileId, pi.PlaylistId })
            .IsUnique();
    }
}
