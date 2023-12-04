using CloudMusicPlayer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Database;

internal sealed class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; init; } = null!;
    public DbSet<PlaylistItem> PlaylistItems { get; init; } = null!;
    public DbSet<DataProvider> DataProviders { get; init; } = null!;
    public DbSet<Playlist> Playlists { get; init; } = null!;
    public DbSet<SongFile> SongFiles { get; init; } = null!;
    public DbSet<History> Histories { get; init; } = null!;
    public DbSet<HistoryItem> HistoryItems { get; init; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TODO Move configurations into separate files
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // DataProvider setup
        modelBuilder.Entity<DataProvider>()
            .Property(dp => dp.ProviderType)
            .HasConversion<string>();

        modelBuilder.Entity<DataProvider>()
            .HasIndex(dp => new { dp.ProviderType, dp.UserId, dp.Name }).IsUnique();

        // PlaylistItem setup
        modelBuilder.Entity<PlaylistItem>()
            .HasIndex(pi => new { ProviderId = pi.SongFileId, pi.PlaylistId }).IsUnique();
    }
}
