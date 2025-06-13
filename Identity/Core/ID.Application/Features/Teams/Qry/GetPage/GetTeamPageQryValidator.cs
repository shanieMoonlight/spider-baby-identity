using ID.Application.Mediatr.Validation;
using Microsoft.AspNetCore.Hosting;

namespace ID.Application.Features.Teams.Qry.GetPage;
public class GetTeamPageQryValidator(IWebHostEnvironment env) 
    : AMntcMinimumOrDevValidator<GetTeamsPageQry>(env)
{}


