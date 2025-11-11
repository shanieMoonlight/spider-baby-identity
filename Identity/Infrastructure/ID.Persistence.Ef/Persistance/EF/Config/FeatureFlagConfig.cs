using ClArch.ValueObjects;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ID.Infrastructure.Persistance.EF.Config;
internal class FeatureFlagConfig : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Name)
            .IsUnique(); // Ensure SQL creates an index on the Name column

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(Name.MaxLength);

        builder.Property(b => b.Description)
            //.IsRequired(false)
            .HasMaxLength(Description.MaxLength);
    }

}//Cls
