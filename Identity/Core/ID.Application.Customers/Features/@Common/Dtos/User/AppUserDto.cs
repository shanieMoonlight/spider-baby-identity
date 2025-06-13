using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Customers.Features.Common.Dtos.User;

//Using AppUser_Customer_Dto instead of AppUserDto incase we want to use some Customer specific properties.
public class AppUser_Customer_Dto : AppUserDto
{
    #region ModelBindingCtor
    public AppUser_Customer_Dto() { }
    #endregion

    public AppUser_Customer_Dto(AppUser mdl) : base(mdl)
    { }

}

