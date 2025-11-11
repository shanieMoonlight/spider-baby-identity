using ClArch.ValueObjects;
using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ID.Infrastructure.Persistance.EF.Config;
internal class TeamConfig : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(x => x.Id);

        //builder.HasIndex(x => x.Name);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(Name.MaxLength);


        builder.Property(b => b.Description)
            //.IsRequired(false)
            .HasMaxLength(Description.MaxLength);



        builder.HasMany(b => b.Members)
            .WithOne(e => e.Team)
            .OnDelete(DeleteBehavior.Restrict);


        var subNavigation = builder.Metadata.FindNavigation(nameof(Team.Subscriptions));
        subNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field); //Use Backing Field



        var mbrNavigation = builder.Metadata.FindNavigation(nameof(Team.Members));
        mbrNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field); //Use Backing Field

        var dvcNavigation = builder.Metadata.GetNavigations();
    }

}//Cls
