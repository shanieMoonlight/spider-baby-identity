using ID.Domain.Entities.Common;

namespace ID.Application.Features.Common.Dtos;
public class AuditableEntityDto
{
    public string? AdministratorUsername { get; init; }
    public string? AdministratorId { get; init; }

    public DateTime DateCreated { get; init; }
    public DateTime? LastModifiedDate { get; init; }

    //------------------------------------// 

    public AuditableEntityDto(IIdAuditableDomainEntity entity)
    {
        AdministratorUsername = entity.AdministratorUsername;
        AdministratorId = entity.AdministratorId;
        DateCreated = entity.DateCreated;
        LastModifiedDate = entity.LastModifiedDate;
    }

    public AuditableEntityDto()
    {
    }

    //------------------------------------// 

}
