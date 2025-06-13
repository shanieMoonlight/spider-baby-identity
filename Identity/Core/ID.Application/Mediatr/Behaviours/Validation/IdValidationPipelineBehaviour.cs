using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MyResults;

namespace ID.Application.Mediatr.Behaviours.Validation;
public sealed class IdValidationPipelineBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> _validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : BasicResult
{
    //------------------------------------//

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var ctx = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(ctx))
            .SelectMany(v => v.Errors)
            .Where(e => e is not null)
            .Distinct()
            .ToArray();

        if (failures.Length == 0)
            return await next(cancellationToken);

        return CreateValidationResult(failures);
    }

    //------------------------------------//

    private static TResponse CreateValidationResult(ValidationFailure[] errors)
    {
        ValidationError errorType = GetValidationErrorType(errors);

        var errorResponse = new ValidationFailureReponse();
        foreach (var error in errors)
            errorResponse.AddError(GetErrorInfo(error));


        return typeof(TResponse) == typeof(BasicResult)
            ? CreateBasicValidationResult(errorType, errorResponse)
            : CreateGenericValidationResult(errorType, errorResponse);
    }

    //- - - - - - - - - - - - - - - - - - //

    private static TResponse CreateGenericValidationResult(ValidationError errorType, ValidationFailureReponse errorResponse)
    {
        var genericType = typeof(TResponse).GenericTypeArguments[0];
        Type? resultType = typeof(GenResult<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(genericType);

        return errorType switch
        {
            ValidationError.BadRequest => (TResponse)resultType
                                        .GetMethod(nameof(GenResult<object>.BadRequestResult), [typeof(object)])!
                                        .Invoke(null, [errorResponse])!,

            ValidationError.Unauthorized => (TResponse)resultType
                                        .GetMethod(nameof(GenResult<object>.UnauthorizedResult), [typeof(string)])!
                                        .Invoke(null, [errorResponse.ToString()])!,

            ValidationError.Forbidden => (TResponse)resultType
                                        .GetMethod(nameof(GenResult<object>.ForbiddenResult), [typeof(string)])!
                                        .Invoke(null, [errorResponse.ToString()])!,

            _ => (TResponse)resultType.GetMethod(nameof(GenResult<object>.BadRequestResult), [typeof(object)])!
                                        .Invoke(null, [errorResponse])!,
        };
    }

    //- - - - - - - - - - - - - - - - - - //

    private static TResponse CreateBasicValidationResult(ValidationError errorType, ValidationFailureReponse errorResponse) =>
        errorType switch
        {
            ValidationError.BadRequest => (BasicResult.BadRequestResult(errorResponse) as TResponse)!,
            ValidationError.Unauthorized => (BasicResult.UnauthorizedResult(errorResponse.ToString()) as TResponse)!,
            ValidationError.Forbidden => (BasicResult.ForbiddenResult(errorResponse.ToString()) as TResponse)!,
            _ => (BasicResult.BadRequestResult(errorResponse) as TResponse)!,
        };

    //- - - - - - - - - - - - - - - - - - //

    private static ValidationErrorInfo GetErrorInfo(ValidationFailure error)
    {
        if (error.CustomState is ValidationError)
            return new ValidationErrorInfo(error.CustomState.ToString()!, true);

        var key = error.PropertyName.Contains('.')
            ? Path.GetExtension(error.PropertyName).Replace(".", "")
            : error.PropertyName;


        return new ValidationErrorInfo(key, error.ErrorMessage);
    }

    //- - - - - - - - - - - - - - - - - - //

    private static ValidationError GetValidationErrorType(ValidationFailure[] errors)
    {
        if (errors.Any(e => e.CustomState as ValidationError? == ValidationError.Unauthorized))
            return ValidationError.Unauthorized;

        if (errors.Any(e => e.CustomState as ValidationError? == ValidationError.Forbidden))
            return ValidationError.Forbidden;

        return ValidationError.BadRequest; //Anything else is a bad request
    }

    //------------------------------------//

}//Cls
