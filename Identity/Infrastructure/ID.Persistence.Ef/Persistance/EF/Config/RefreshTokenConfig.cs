using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Refreshing.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ID.Infrastructure.Persistance.EF.Config;
internal class RefreshTokenConfig : IEntityTypeConfiguration<IdRefreshToken>
{
    public void Configure(EntityTypeBuilder<IdRefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        //builder.HasIndex(b => b.UserId)
        //    .IsUnique();

        //- - - - - - - - - - - - - - - - - - //   

        builder.Property(b => b.Payload)
            .IsRequired()
            .HasMaxLength(TokenPayload.MaxLength);

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);



    }

}//Cls
