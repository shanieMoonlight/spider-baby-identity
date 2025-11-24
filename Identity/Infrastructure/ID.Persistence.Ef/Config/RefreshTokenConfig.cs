using ID.Domain.Claims.AuthMethods;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Refreshing.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace ID.Persistence.Ef.Config;
internal class RefreshTokenConfig : IEntityTypeConfiguration<IdRefreshToken>
{
    public void Configure(EntityTypeBuilder<IdRefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        //builder.HasIndex(b => b.UserId)
        //    .IsUnique();

        //- - - - - - - - - - - - - - - - -//   

        builder.Property(b => b.Payload)
            .IsRequired()
            .HasMaxLength(TokenPayload.MaxLength);

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        // Serialize List<AuthMethodRef> to JSON (stored as string) and provide a ValueComparer
        var authMethodConverter = new ValueConverter<List<AuthMethodRef>, string>(
            v => JsonSerializer.Serialize(v.Select(e => (int)e).ToList(), (JsonSerializerOptions?)null),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<AuthMethodRef>()
                : JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null)!.Select(i => (AuthMethodRef)i).ToList());

        var authMethodComparer = new ValueComparer<List<AuthMethodRef>>(
            (a, b) => ReferenceEquals(a, b) || (a != null && b != null && a.SequenceEqual(b)),
            a => a == null ? 0 : a.Aggregate(0, (h, v) => HashCode.Combine(h, (int)v)),
            a => a == null ? new List<AuthMethodRef>() : a.ToList());

        builder.Property(b => b.AuthMethodRefs)
            .HasConversion(authMethodConverter)
            .Metadata.SetValueComparer(authMethodComparer);

    }

}//Cls
