using ClArch.ValueObjects;
using ID.Domain.Entities.Devices;
using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ID.Infrastructure.Persistance.EF.Config;
internal class DeviceConfig : IEntityTypeConfiguration<TeamDevice>
{
    public void Configure(EntityTypeBuilder<TeamDevice> builder)
    {
        builder.HasKey(x => x.Id);

        //builder.HasIndex(x => x.Name);
        //builder.HasIndex(p => new { p.Name, p.Official })
        //    .IsUnique();


        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(Name.MaxLength);


        builder.Property(b => b.Description)
            //.IsRequired(false)
            .HasMaxLength(Description.MaxLength);

     

        builder.Property(b => b.UniqueId)
            //.IsRequired(false)
            .HasMaxLength(UniqueId.MaxLength);    



    }//Configure

}//Cls
