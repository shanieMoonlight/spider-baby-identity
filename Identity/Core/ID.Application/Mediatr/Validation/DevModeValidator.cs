using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ID.Application.Mediatr.Validation;

/// <summary>
/// Base class for validation before handlers that should be accessed by Mntc Team Members only
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class ADevModeValidator<TRequest> : AbstractValidator<TRequest> where TRequest : class
{
    protected ADevModeValidator(IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            return;

        ConfigureRules();
    }

    //------------------------//

    protected abstract void ConfigureRules();

}//Cls

//==============================================================//


/// <summary>
/// Base class for validation before handlers that should be accessed by Mntc Team Members only
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class ASimpleDevModeValidator<TRequest>(IWebHostEnvironment env) : ADevModeValidator<TRequest>(env) where TRequest : class
{
    protected override void ConfigureRules() { }

}//Cls