using ID.Application.Mediatr.Validation;
using Microsoft.AspNetCore.Hosting;

namespace ID.Application.Features.System.Qry.Settings;
public class GetSettingsCmdValidator(IWebHostEnvironment env) 
    : ASuperMinimumOrDevValidator<GetSettingsCmd>(env)
{}

