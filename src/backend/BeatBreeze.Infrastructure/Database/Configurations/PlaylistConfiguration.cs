using BeatBreeze.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeatBreeze.Infrastructure.Database.Configurations;

internal sealed class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder
            .HasIndex(p => new { p.UserId, p.Name })
            .IsUnique();
    }
}
