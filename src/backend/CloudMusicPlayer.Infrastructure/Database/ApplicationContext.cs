using System.Reflection;
using CloudMusicPlayer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Database;

internal sealed class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; init; } = null!;
    public DbSet<PlaylistItem> PlaylistItems { get; init; } = null!;
    public DbSet<DataProvider> DataProviders { get; init; } = null!;
    public DbSet<Playlist> Playlists { get; init; } = null!;
    public DbSet<MusicFile> MusicFiles { get; init; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
