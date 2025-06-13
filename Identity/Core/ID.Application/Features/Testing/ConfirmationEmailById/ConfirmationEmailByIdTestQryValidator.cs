using ID.Application.Mediatr.Validation;
using Microsoft.AspNetCore.Hosting;

namespace ID.Application.Features.Testing.ConfirmationEmailById;
public class ConfirmationEmailByIdTestQryValidator(IWebHostEnvironment env)
    : ASimpleDevModeValidator<ConfirmationEmailByIdTestQry>(env)
{ }


