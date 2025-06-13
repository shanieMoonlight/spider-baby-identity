using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Entities.Teams;
using ID.GlobalSettings.Constants;
using ID.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ID.Infrastructure.Persistance.EF;

public class IdDbContext(DbContextOptions<IdDbContext> options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    //public DbSet<IdentityAddress> Addresses => Set<IdentityAddress>();
    public DbSet<Team> Teams => Set<Team>();

    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<IdRefreshToken> RefreshTokens => Set<IdRefreshToken>();
    public DbSet<SubscriptionPlanFeature> SubscriptionPlanFeatures => Set<SubscriptionPlanFeature>();

    //public DbSet<Subscription> Subscriptions => Set<Subscription>();
    //public DbSet<Device> Devices => Set<Device>();

    public DbSet<IdOutboxMessage> OutboxMessages => Set<IdOutboxMessage>();


    //--------------------------------//

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdDbContext).Assembly);
        modelBuilder.HasDefaultSchema(IdGlobalConstants.Db.SCHEMA);
    }

    //--------------------------------//

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.LogTo(message => Debug.WriteLine(message));

    //--------------------------------//

    public virtual async Task<int> SaveChangesAsync(string? username = "SYSTEM", string? userId = "-1", CancellationToken cancellationToken = default)
    {
        var changedEntries = ChangeTracker.Entries<IIdAuditableDomainEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in changedEntries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.SetCreated(username, userId);
            else if (entry.State == EntityState.Modified)
                entry.Entity.SetModified(username, userId);

        }//foreach

        return await base.SaveChangesAsync(cancellationToken);
    }

    //--------------------------------//


}//Cls