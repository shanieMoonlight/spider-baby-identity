using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace ID.Domain.Entities.AppUsers;

//[Table("MyIdentityAddresses")]
public class IdentityAddress //: IdDomainEntity
{

    [Required]
    public string Line1 { get; private set; } = string.Empty;
    [Required]
    public string Line2 { get; private set; } = string.Empty;
    [MaxLength(100)]
    public string? Line3 { get; private set; } = string.Empty;
    [MaxLength(100)]
    public string? Line4 { get; private set; } = string.Empty;
    [MaxLength(100)]
    public string? Line5 { get; private set; } = string.Empty;

    [MaxLength(20)]
    public string? AreaCode { get; private set; } = string.Empty;


    [MaxLength(20)]
    public string? Notes { get; private set; } = string.Empty;

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }


    //------------------------//

    #region EfCore

    /// <summary>
    /// Used by EfCore
    /// </summary>
    private IdentityAddress() { }

    #endregion

    //------------------------//

    public static IdentityAddress Create(
    AddressLine line1,
    AddressLine line2,
    AddressLineNullable line3,
    AddressLineNullable line4,
    AddressLineNullable line5,
    AreaCodeNullable areaCode,
    ShortNotesNullable notes)
    {
        return new IdentityAddress()
        {
            Line1 = line1.Value,
            Line2 = line2.Value,
            Line3 = line3.Value,
            Line4 = line4.Value,
            Line5 = line5.Value,
            AreaCode = areaCode.Value,
            Notes = notes.Value
        };
    }

    //------------------------//

    public override string ToString() => $"{Line1}, {Line2}, {Line3}, {Line4}, {Line5}";

    //------------------------//

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (obj is not IdentityAddress)
            return false;

        var that = (IdentityAddress)obj;
        return ToString().Equals(that.ToString(), StringComparison.CurrentCultureIgnoreCase);
    }

    //------------------------//

    public override int GetHashCode() => HashCode.Combine(Line1, Line2, Line3, Line4, Line5);

    //------------------------//

}//Cls