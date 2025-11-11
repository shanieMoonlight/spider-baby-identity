using ID.Domain.Entities.SubscriptionPlans.Subscriptions;
using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ID.Infrastructure.Persistance.EF.Config;

public class TeamSubscriptionConfig : IEntityTypeConfiguration<TeamSubscription>
{
    public void Configure(EntityTypeBuilder<TeamSubscription> builder)
    {
        builder.HasKey(x => x.Id);


        builder.HasIndex(p => new { p.TeamId, p.SubscriptionPlanId })
            .IsUnique();

        var statusConverter = new EnumToStringConverter<SubscriptionStatus>();
        builder.Property(e => e.SubscriptionStatus)
            .HasMaxLength(50)
            .HasConversion(statusConverter);


        builder.HasMany(b => b.Devices)
             .WithOne(e => e.Subscription)
             .OnDelete(DeleteBehavior.Cascade);


        var navigation = builder.Metadata.FindNavigation(nameof(TeamSubscription.Devices));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field); //Use Backing Field

        var dvcNavigation = builder.Metadata.GetNavigations();
    }

}//Cls
