using ClArch.ValueObjects;
using ID.Domain.Entities.SubscriptionPlans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ID.Infrastructure.Persistance.EF.Config;
internal class SubscriptionPlanConfig : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(x => x.Id);

        //builder.HasIndex(x => x.Name);
        //builder.HasIndex(p => new { p.Name, p.Official })
        //    .IsUnique();


        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(Name.MaxLength);


        builder.Property(b => b.Description)
            .HasMaxLength(Description.MaxLength);

        var renewalConverter = new EnumToStringConverter<SubscriptionRenewalTypes>();
        builder.Property(e => e.RenewalType)
                .HasMaxLength(50)
                .HasConversion(renewalConverter);


        builder.HasMany(b => b.Subscriptions)
            .WithOne(e => e.SubscriptionPlan)
            .OnDelete(DeleteBehavior.NoAction);

        //- - - - - - - - - - - -//
        //      Many-To-Many     //
        //- - - - - - - - - - - -//

        builder.HasMany(b => b.FeatureFlags)
          .WithMany(t => t.SubscriptionPlans)
          .UsingEntity<SubscriptionPlanFeature>()
          .HasKey(x => new { x.SubscriptionPlanId, x.FeatureFlagId });

        //- - - - - - - - - - - -//

    }//Configure

}//Cls
