//using ID.Application.Utility.ExtensionMethods;
//using Microsoft.AspNetCore.Http;

//namespace ID.Application.Filters.Abstractions;


////########################################################//

///// <summary>
///// Interface for handling authorization with position level.
///// </summary>
//internal interface IAuthPositionMinimumHandler
//{
//    /// <summary>
//    /// Determines whether the specified HTTP context is authorized based on the position level.
//    /// </summary>
//    /// <param name="context">The HTTP context.</param>
//    /// <param name="level">The position level. Default is -1.</param>
//    /// <returns><c>true</c> if the specified context is authorized; otherwise, <c>false</c>.</returns>
//    bool IsAuthorized(HttpContext context, int? level = null);
//}


////########################################################//

//internal abstract class AAuthPositionMinimumHandler : IAuthPositionMinimumHandler
//{
//    /// <summary>
//    /// Determines whether the specified HTTP context is authorized based on the position level.
//    /// User must have a position of <paramref name="level"/> or higher.
//    /// </summary>
//    /// <param name="context">The HTTP context.</param>
//    /// <param name="level">The position level. Default is -1.</param>
//    /// <returns><c>true</c> if the specified context is authorized; otherwise, <c>false</c>.</returns>
//    public bool IsAuthorized(HttpContext context, int? level = null)
//    {
//        var extraAuth = ExtraAuthorization(context, level);
//        var teamPosition = context.User.TeamPosition();
//        return level == null
//            ? extraAuth
//            : extraAuth && teamPosition >= level;
//    }

//    public abstract bool ExtraAuthorization(HttpContext context, int? level = -1);

//}



////########################################################//


