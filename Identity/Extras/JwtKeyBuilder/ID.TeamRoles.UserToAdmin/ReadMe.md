# ID.TeamRoles.UserToAdmin

This project is part of the Identity system and provides functionality for managing user roles and claims within teams. It includes various authenticators and claims related to user roles in different teams.
It provides mre user friendly names for the roles and claims, rather than usigng THe integer position values.


## Project Structure

- **Target Framework**: .NET 8.0
- **Dependencies**:
  - `ID.Application`
  - `ID.Domain`

## Features

### Authenticators

The project includes several authenticators to handle different user roles and permissions within teams. Some of the key authenticators are:

- **SuperAdminMinimumAuthenticator**: Ensures that the user has at least SuperAdmin privileges.
- **CustomerAdmin**: Handles authentication for customer administrators ONLY.
- **CustomerMgr**: Handles authentication for customer managers ONLY.
- **CustomerMgrMin**: Handles authentication for customer managers or HIGHER.
- **MntcUserMin**: Handles authentication for maintenance users or HIGHER.
- **MntcMgrMin**: Handles authentication for maintenance managers or HIGHER.

### Claims

The project defines various claims related to user roles within teams. These claims are used to determine the permissions and access levels of users.

## Usage

To use the authenticators and claims provided by this project, you need to reference it in your application and configure the necessary services.

In order to use the Policies (***Authenticator.Policy) you need to call

    services.AddTeamRolesUserToAdmin()
This important because it Registers the policies from the authenticators.


IN order to add user friendly claims. You need use ID.TeamRoles.UserToAdmin.Jwt.TeamRole_User_to_Mgr_ClaimsGenerator as the TExtraClaimsGenerator generic paramter in the MyIdSetup.

    builder.Services.AddMyId<TeamRole_User_to_Mgr_ClaimsGenerator>(
           config =>
           {
               config.ApplicationName = startupData.APP_NAME;
               config.ConnectionString = startupData.GetConnectionStrings_SqlDb();
               config.TokenSigningKey = startupData.GetIdentity_SecretyKey();
               config.AsymmetricPrivateKey_Xml = startupData.GetAsymmetricPrivateKey_Xml_String();
               config.AsymmetricPublicKey_Xml = startupData.GetAsymmetricPublicKey_Xml_String();
               config.TokenExpirationMinutes = startupData.GetIdentity_JwtExpirationMinutes();
               config.HangfireUrl = startupData.HANG_FIRE_URL;
           })

### Example

Here is an example of how to use the `***Authenticator.ActionFilter` in your application:


    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IdTesterController : Controller
    {
        //-----------------------------------//

        [HttpGet]
        [CustomerAdminAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> CustomerAdmin() =>
            MessageResponseDto.Generate($"You must be {nameof(CustomerAdmin)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [CustomerManagerAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> CustomerManager() =>
            MessageResponseDto.Generate($"You must be {nameof(CustomerManager)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [CustomerUserAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> CustomerUser() =>
            MessageResponseDto.Generate($"You must be {nameof(CustomerUser)}");

        //-----------------------------------//

        [HttpGet]
        [CustomerAdminMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> CustomerAdminMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(CustomerAdminMinimum)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [CustomerManagerMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> CustomerManagerMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(CustomerManagerMinimum)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [CustomerUserMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> CustomerUserMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(CustomerUserMinimum)}");

        //-----------------------------------//

        [HttpGet]
        [MntcAdminAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> MntcAdmin() =>
            MessageResponseDto.Generate($"You must be {nameof(MntcAdmin)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [MntcManagerAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> MntcManager() =>
            MessageResponseDto.Generate($"You must be {nameof(MntcManager)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [MntcUserAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> MntcUser() =>
            MessageResponseDto.Generate($"You must be {nameof(MntcUser)}");

        //-----------------------------------//

        [HttpGet]
        [MntcAdminMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> MntcAdminMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(MntcAdminMinimum)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [MntcManagerMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> MntcManagerMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(MntcManagerMinimum)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [MntcUserMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> MntcUserMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(MntcUserMinimum)}");

        //-----------------------------------//

        [HttpGet]
        [SuperAdminAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> SuperAdmin() =>
            MessageResponseDto.Generate($"You must be {nameof(SuperAdmin)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [SuperManagerAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> SuperManager() =>
            MessageResponseDto.Generate($"You must be {nameof(SuperManager)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [SuperUserAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> SuperUser() =>
            MessageResponseDto.Generate($"You must be {nameof(SuperUser)}");

        //-----------------------------------//

        [HttpGet]
        [SuperAdminMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> SuperAdminMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(SuperAdminMinimum)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [SuperManagerMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> SuperManagerMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(SuperManagerMinimum)}");

        //- - - - - - - - - - - - - - - - - -//

        [HttpGet]
        [SuperUserMinimumAuthenticator.ActionFilter]
        public ActionResult<MessageResponseDto> SuperUserMinimum() =>
            MessageResponseDto.Generate($"You must be {nameof(SuperUserMinimum)}");

        //-----------------------------------//

    } //Cls
