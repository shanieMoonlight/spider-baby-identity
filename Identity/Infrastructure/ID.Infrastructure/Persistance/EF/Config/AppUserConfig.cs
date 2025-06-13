using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ID.Infrastructure.Persistance.EF.Config;
internal class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(x => x.Id);

        //builder.HasIndex(x => x.Name);
        //builder.HasIndex(p => new { p.Name, p.Official })
        //    .IsUnique();

        builder.Property(b => b.FirstName)
            //.IsRequired()
            .HasMaxLength(FirstName.MaxLength);

        builder.Property(b => b.LastName)
            //.IsRequired(false)
            .HasMaxLength(LastName.MaxLength);

        builder.Property(b => b.Email)
            .HasMaxLength(EmailAddress.MaxLength);

        builder.Property(b => b.PhoneNumber)
            .HasMaxLength(Phone.MaxLength);

        builder.Property(b => b.Tkn)
            .HasMaxLength(512);

        builder.Property(b => b.TwoFactorKey)
            .HasMaxLength(128);

        builder.HasOne(b => b.Team)
            .WithMany(t => t.Members)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(b => b.Address, address =>
        {
            address.Property(a => a.Line1).HasMaxLength(AddressLine.MaxLength);
            address.Property(a => a.Line2).HasMaxLength(AddressLine.MaxLength);
            address.Property(a => a.Line3).HasMaxLength(AddressLine.MaxLength);
            address.Property(a => a.Line4).HasMaxLength(AddressLine.MaxLength);
            address.Property(a => a.Line5).HasMaxLength(AddressLine.MaxLength);
            address.Property(a => a.AreaCode).HasMaxLength(AreaCode.MaxLength);
            address.Property(a => a.Notes).HasMaxLength(ShortNotesNullable.MaxLength);
        });

        builder.HasOne(b => b.OAuthInfo)
            .WithOne(t => t.AppUser)
            .OnDelete(DeleteBehavior.Cascade);


        //builder.HasMany(b => b.IdRefreshTokens)
        //    .WithOne(e => e.User)
        //    .OnDelete(DeleteBehavior.Cascade);

        var tfConverter = new EnumToStringConverter<TwoFactorProvider>();
        builder.Property(e => e.TwoFactorProvider)
                .HasMaxLength(50)
                .HasConversion(tfConverter);

        //builder.ComplexProperty(b => b.Address);
    }
}
