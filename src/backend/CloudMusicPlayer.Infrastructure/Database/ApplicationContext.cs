using System.Linq.Expressions;
using System.Reflection;
using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CloudMusicPlayer.Infrastructure.Database;

internal sealed class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<PlaylistItem> PlaylistItems { get; set; } = null!;
    public DbSet<DataProvider> DataProviders { get; set; } = null!;
    public DbSet<Playlist> Playlists { get; set; } = null!;
    public DbSet<SongFile> SongFiles { get; set; } = null!;
    public DbSet<History> Histories { get; set; } = null!;
    public DbSet<HistoryItem> HistoryItems { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {

        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<DataProvider>()
            .Property(dp => dp.ProviderType)
            .HasConversion<string>();

        modelBuilder.Entity<DataProvider>()
            .HasIndex(dp => new { ProviderId = dp.ProviderType, dp.Name, dp.UserId }).IsUnique();

        modelBuilder.Entity<PlaylistItem>()
            .HasIndex(pi => new { ProviderId = pi.SongFileId, pi.PlaylistId }).IsUnique();
    }

    public async Task<Result> SaveChangesResult()
    {
        try
        {
            var result = await SaveChangesAsync();

            if (result > 0)
            {
                return Result.Success();
            }

            return Result.Failure(DataLayerErrors.Database.NoChangesMade());
        }
        catch (Exception ex) when (ex is DbUpdateException or DbUpdateConcurrencyException or OperationCanceledException)
        {
            return Result.Failure(DataLayerErrors.Database.TransactionError());
        }
    }

    public async Task<Result> ExecuteDeleteResult<T>(IQueryable<T> toBeDeleted)
    {
        try
        {
            var result = await toBeDeleted.ExecuteDeleteAsync();

            if (result > 0)
            {
                return Result.Success();
            }

            return Result.Failure(DataLayerErrors.Database.NoChangesMade());
        }
        // TODO Find out what exceptions are thrown by ExecuteDelete/ExecuteUpdate
        catch (Exception ex)
        {
            return Result.Failure(DataLayerErrors.Database.DeleteError());
        }
    }

    public async Task<Result> ExecuteUpdateResult<T>(IQueryable<T> toBeUpdated,
        Expression<Func<SetPropertyCalls<T>,SetPropertyCalls<T>>> setPropertyCalls)
    {
        try
        {
            var result = await toBeUpdated.ExecuteUpdateAsync(setPropertyCalls);

            if (result > 0)
            {
                return Result.Success();
            }

            return Result.Failure(DataLayerErrors.Database.NoChangesMade());
        }
        catch (Exception ex)
        {
            return Result.Failure(DataLayerErrors.Database.UpdateError());
        }
    }
}
