using ID.Domain.Entities.Common;
using MassTransit;
using System.ComponentModel.DataAnnotations;

namespace ID.Domain.Entities.Avatars;

/// <summary>
/// User's picture/portrait
/// </summary>
public class Avatar : IdDomainEntity
{
    /// <summary>
    /// Original file type
    /// </summary>
    [MaxLength(100)]
    public ImageSrcTypes SrcType { get; set; } = ImageSrcTypes.URL;

    /// <summary>
    /// Base 64 encoding of the image
    /// </summary>
    [MaxLength(5000)]
    public string? B64 { get; set; }

    /// <summary>
    /// Location of image (If cloud stored)
    /// </summary>
    [MaxLength(500)]
    public string? Url { get; set; }

    //------------------------//

    public Avatar(Guid id) : base(id) { }

    //- - - - - - - - - - - - //

    private Avatar() : base() { }

    //------------------------//

}//Cls