using ID.Domain.Entities.TrustedDevices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ID.Domain.Entities.TrustedDevices.ValueObjects;

namespace ID.Persistence.Ef.Config;

internal class TrustedDeviceConfig : IEntityTypeConfiguration<TrustedDevice>
{
    public void Configure(EntityTypeBuilder<TrustedDevice> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new { x.UserId, x.DeviceFingerprint })
            .IsUnique();

        builder.Property(x => x.DeviceFingerprint)
            .IsRequired()
            .HasMaxLength(DeviceFingerprint.MaxLength);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(DeviceName.MaxLength);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(UserAgent.MaxLength);

        builder.HasOne(x => x.User)
            .WithMany(u => u.TrustedDevices)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        var nav = builder.Metadata.FindNavigation(nameof(TrustedDevice.User));
        nav?.SetPropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
    }
}
