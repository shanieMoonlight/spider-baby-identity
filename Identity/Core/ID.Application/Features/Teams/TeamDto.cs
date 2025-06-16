using ID.Application.Dtos.User;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Features.Teams;
using ID.Domain.Entities.Teams;
using System.Text.Json.Serialization;

namespace ID.Application.Features.Teams;
public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public int MinPosition { get; set; }
    public int MaxPosition { get; set; }

    public Guid? LeaderId { get; set; }
    public AppUserDto? Leader { get; set; }


    public List<AppUserDto> Members { get; set; } = [];


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TeamType TeamType { get; set; }

    public List<SubscriptionDto> Subscriptions = [];

    //------------------------------------//

    #region ModelBinding
    public TeamDto() { }
    #endregion
    public TeamDto(Team team)
    {
        Id = team.Id;
        Name = team.Name;
        Description = team.Description;
        Members = [.. team.Members?.Select(m => m.ToDto()) ?? []];
        TeamType = team.TeamType;
        LeaderId = team.LeaderId;
        Leader = team.Leader?.ToDto();
        Subscriptions = team.Subscriptions?.Select(s => s.ToDto())?.ToList() ?? [];
        MaxPosition = team.MaxPosition;
        MinPosition = team.MinPosition;
    }

    //------------------------------------//

}//Cls
