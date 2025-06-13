using ID.Domain.Entities.AppUsers.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ID.Infrastructure.Persistance.EF.Config;
internal class OAuthConfig : IEntityTypeConfiguration<OAuthInfo>
{
    public void Configure(EntityTypeBuilder<OAuthInfo> builder)
    {
        builder.HasKey(x => x.Id);

        var oaConverter = new EnumToStringConverter<OAuthProvider>();
        builder.Property(e => e.Provider)
                .HasMaxLength(50)
                .HasConversion(oaConverter);


    }//Configure

}//Cls
