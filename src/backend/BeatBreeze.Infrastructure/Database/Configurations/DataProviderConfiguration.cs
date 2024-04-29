using BeatBreeze.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeatBreeze.Infrastructure.Database.Configurations;

internal sealed class DataProviderConfiguration : IEntityTypeConfiguration<DataProvider>
{
    public void Configure(EntityTypeBuilder<DataProvider> builder)
    {
        builder
            .Property(dp => dp.ProviderType)
            .HasConversion<string>();

        builder
            .HasIndex(dp => new { dp.ProviderType, dp.UserId, dp.Name })
            .IsUnique();
    }
}
