using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using StringHelpers;

namespace ID.Domain.Utility.Messages;

public partial class IDMsgs
{

    public static class Error
    {
        public const string UNKNOWN_ERROR = "An unknown error occurred. Please try again later.";
        public static readonly string SOMETHING_WENT_WRONG_ERROR = "Something Went Wrong :(. Contact your administrator if the problem persists";
        public static string Duplicate<T>() => $"There is already a {typeof(T).Name} with the same values in the database.";
        public static string MISSING_DATA<T>(T t) => $"Required data, {t}, is missing";
        public static string IsRequired(string propertyName) => $"{propertyName} is required.";


       
        public const string MISSING_APP_DATA = "Missing App Data";
        public const string MISSING_CUSTOMER_DATA = "Missing Customer Data";
        public const string MISSING_DEVICE_DATA = "Missing Device Data";
        public const string MISSING_SUB_DEF = "Missing Subscription Type Data";
        public const string MISSING_TEAM_DATA = "Missing Team Data";
        public static readonly string MISSING_HOST_USER_DATA = $"Missing Host Data.{NL}You must supply a host identifier when registering a guest.";
        public const string MISSING_USER_DATA = "Missing user data";

        
        
        public const string NO_APPS_FOUND = "User has no apps connected to the account.";
        public const string NO_DATA_SUPPLIED = "No data supplied";
        public const string NO_ID_SUPPLIED = "No id supplied";
        public static string NOT_FOUND_DEVICE(int id) => $"Device with id, {id} not found";
        public static string NOT_FOUND_DEVICE(string unqId) => $"Device with Unique Id, {unqId} not found";
        public static string NOT_FOUND_FILTER<T>() => $"No {typeof(T).Name} match the filter criteria.";
        public const string NOT_FOUND_APP = "Application not found";
        public const string NOT_FOUND_SUPER_USER = "SUPER User not found";

        public const string NOT_FOUND_TEAM = "Team not found";
        public const string NOT_FOUND_MNTC_TEAM = "MAINTENANCE User not found";
        public const string NOT_FOUND_SUPER_TEAM = "SUPER User not found";
        public const string NOT_FOUND_USER = "User not found";
        public static readonly string NOT_FOUND_HOST_USER = $"Referring user not found.{NL}You must supply a host identifier when registering a guest.";
        public static string NotFound<T>(object? id) => $"{typeof(T).Name}, '{id ?? "_blank_"}' was not found.";
        public static string NotFound<T>() => $"{typeof(T).Name} was not found.";



        public static readonly string REFRESH_TOKEN_DISABLED = $"Token refresh is not available at this time.";



        //-----------------------//

        public static class RequestData
        {
            public const string MISSING_USER_IDENTIFIER_DATA = "You must supply at least one of [Username, UserId or Email]";

        }

        //-----------------------//

        //public static class NotFound
        //{

        //}

        //-----------------------//

        public static class Email
        {
            public static readonly string USER_WITHOUT_EMAIL = "User has no email. Contact admin";
            public static string EMAIL_NOT_CONFIRMED(string email) => $"Email has not been confirmed yet. A new confirmation email has been resent to {email}. It should arrive in the next 5 minutes";
            public static string EMAIL_NOT_CONFIRMED_YET(string email) => $"Email has not been confirmed yet. A new confirmation email has been resent to {email}.";
            public static string EMAIL_NOT_FOUND(string email) => $"No user with email, {email} was found.";
            public static string INVALID_EMAIL_ADDRESS(AppUser member) => $"{member.FirstName} {member.LastName} does not have a valid email address : ({member.Email})";
            public static string NOT_FOUND_TEMPLATE_TYPE(string? letterType) => $"No email template named '{letterType}' was found.";
        }

        //-----------------------//

        public static class Jobs
        {
            public static string MISSING_OUTBOX_CONTENT(IdOutboxMessage msg) => $"Date:{DateTime.Now} Missing outbox content for: {msg}";
        }

        //-----------------------//

        public static class Jwt
        {
            public static string SYMETRIC_CRYPTO_HAS_NO_PUBLIC_KEY => $"This App User Symmetric Crypto and doesn't use Public/Private keys";
            public static string TOKEN_SIGNING_KEY_TOO_SHORT(int minLen) => $"Token signing key is too short (Min Length: {minLen})";
        }

        //-----------------------//

        public static class Phone
        {
            public static string INVALID_PHONE_NUMBER(AppUser member) => $"{member.FirstName} {member.LastName} does not have a valid phone address : ({member.PhoneNumber})";
            public static string PHONE_CONFIRMATION_FAILURE(string phone) => $"Phone number confirmation has failed.{NL}Make sure the phone number is correct.{NL}Number in our system:  {phone}.";
            public static string PHONE_NOT_CONFIRMED_YET(string phone) => $"Phone has not been confirmed yet. A new confirmation phone has been resent to {phone}.";
            public static string PHONE_NOT_FOUND(string phone) => $"No user with phone, {phone} was found.";
            public static string USER_WITHOUT_PHONE(AppUser user) => $"User {user.UserName ?? user.Email} has no phone number. Contact admin";
        }

        //-----------------------//

        public static class Setup
        {
            //public const string MISSING_APP_NAME = "You must supply a name for the application that is using MyIdentity";
            public const string MISSING_ASSYMETRIC_PRIVATE_KEY = "You must supply a private key when using asymmetrically signed JWTs";
            public const string MISSING_ASSYMETRIC_PUBLIC_KEY = "You must supply a public key when using asymmetrically signed JWTs";
            public const string MISSING_ASSYMETRIC_KEY_PAIR = "You must supply a public and private key when using asymmetrically signed JWTs";
            public const string MISSING_BRAND_COLORS = "You must supply a brand color for your email templates. (Hex format)";
            public const string MISSING_CONFIGURATION = "You must supply a configuration";
            public const string MISSING_CONNECTION_STRING = "You must supply a Connection String";
            public const string MISSING_EMAIL_FROM_ADDRESS = "You must supply a from address for use in account emailing";
            public const string MISSING_CUSTOMER_ACCOUNTS_URL = "You must supply an url for the Customer section of your website. This will be used for registration and forgot password links";
            public const string MISSING_MNTC_ACCOUNTS_URL = "You must supply an url for the Maintenance section of your website. This will be used for registration and forgot password links";


            public static string MISSING_SETUP_DATA(string missingProperty, string library) => $"You must supply a {missingProperty} when using {library}";
        }

        //-----------------------//

        public static class Subscriptions
        {
            public static readonly string CantDelete = $"You can't delete subscriptions because they are needed for auditing. Set it's Status to 'Cancelled'";
            public static string DeviceAlreadyRegistered(string deviceId) => $"Device {deviceId} is already registerd on this Subscription";
            public static readonly string DEVICE_LIMIT_REACHED = $"You can't add anymore devices to this App Subscription. Delete a device or upgrade your Subscription first.";
        }

        //-----------------------//

        public static class Teams
        {
            public static string ALREADY_A_TEAM_MEMBER(AppUser user, Team team) => $"User with email, {user.Email} is ALREADY a member of the Team, {team.Name}";
            public static string ALREADY_THE_TEAM_LEADER(AppUser user, Team team) => $"User with email, {user.Email} is ALREADY the leader of the Team, {team.Name}";

            public const string CAN_ONLY_REMOVE_CUSTOMER_TEAM = $"You can only delete a customer team.";
            public const string CANT_CLOSE_ACCOUNT_OF_OTHER_TEAM = $"You can only close your own account.";
            public const string CANT_CHANGE_POSITION_OF_LEADER = "The Team leader must also have the highest team position";
            public const string MISSING_TEAM_DATA = "Missing Team Data";
            public const string MNTC_TEAM_ALREADY_CREATED = "The Maintenance Team is already created.";
            public const string ONlY_LEADER_CAN_UPDATE = $"Only the Team Leader can update the Team.";
            public const string ONlY_LEADER_CAN_UPDATE_LEADER = $"Only the Team Leader or IT Administrator can set the new Leader.";
            public const string SUPER_TEAM_ALREADY_CREATED = "The Super Team is already created ";
            public static readonly string CANT_ADD_SUPER_LEADER = $"You can't add another leader to the Super Team.{NL}Choose a lower position.";
            public static readonly string CANT_CHANGE_LAST_SUPER_ROLE = $"You can't change the Role of the last member of the Super Team.{NL}There must be at least one Leader on the Super Team.";
            public static readonly string CANT_DELETE_LEADER = $"The Team Leader can't be deleted.{NL}Assign a new Leader before deleting this user";
            public static readonly string CANT_EDIT_SUPER_LEADER_ROLE = $"You can't change the Super Team Leaders Role.";
            public static readonly string CANT_REMOVE_LAST_SUPER_MEMBER = $"You can't remove the last member of the Super Team.{NL}Add another user before removing this one.";
            public static readonly string CANT_REMOVE_TEAM_LEADER = $"You can't remove the Team Leader from a team.{NL}You must set a new Team Leader first";
            public static readonly string CANT_REMOVE_LAST_CUSTOMER_MEMBER = $"Cannot remove the last member from a customer team. Did you mean to delete the team?";
            public static readonly string CANT_REMOVE_LAST_MAINTENANCE_OR_SUPER_MEMBER = $"Cannot remove the last member from the Maintenance or Super team.";
            public static readonly string NO_MSG_PROVIDER_SET = $"No message provider has been setup yet.";
            public const string CANNOT_DELETE_SUPER_TEAM = "You can't delete the Super team";
            public const string CANNOT_DELETE_MNTC_TEAM = "You can't delete the Maintenance team";

            public const string NOT_MAINTENANCE_TEAM_MEMBER = "User is not a member of the Maintenance Team";
            public const string NOT_SUPER_TEAM_MEMBER = "User is not a member of the Super Team";

            public static string CANT_ADMINISTER_HIGHER_POSITIONS(int position) => $"Can only administer positions of {position} or lower";
            public static string CANT_DEMOTE_SAME_POSITION => $"Can only demote users with TeamPositions already lower than yours";
            public static string EMPTY_TEAM(string teamName) => $"Team {teamName} has no members.";
            public static string NotACustomerTeam(Guid teamId) => $"Team {teamId} is not  a Customer Team";
            public static string NOT_TEAM_MEMBER(AppUser user, Guid teamIdentifier) => NOT_TEAM_MEMBER(user, teamIdentifier.ToString());
            public static string NOT_TEAM_MEMBER(AppUser user, string teamIdentifier) => $"User with email, {user.Email} is not a member of the Team, {teamIdentifier}";
            public static string NOT_TEAM_MEMBER(AppUser user, Team team) => NOT_TEAM_MEMBER(user, team.Name);

            public static string NOT_TEAM_MEMBER(Guid userIdentifier, string teamIdentifier) => NOT_TEAM_MEMBER(userIdentifier.ToString(), teamIdentifier);
            public static string NOT_TEAM_MEMBER(string userIdentifier, string teamIdentifier) => $"User, {userIdentifier} is not a member of the Team, {teamIdentifier}";
            public static string TEAM_HAS_NO_LEADER(string teamName) => $"Team {teamName} has no leader set.";
            public static string UNAUTHORIZED_FOR_SUPER_TEAM(int position) => $"You must have a SuperTeam Member ({position}) to view it's members.";

            public static string UNAUTHORIZED_FOR_TEAM_TYPE(TeamType teamType) => $"You do not have permission to access {teamType}.";
            public static string CAPACITY_EXCEEDED(Team team) => $"The Team capacity, {team.Capacity} has been reached for team, {team.Name} has already reached";



        }

        //-----------------------//

        public static class Tokens
        {
            public static string InvalidTkn(string serviceName) => $"{serviceName}-Token-Error: Invalid Token";
            public static string InvalidTkn(AppUser user) => $"All token for {user.FriendlyName} removed!";

        }

        //-----------------------//

        public static class TwoFactor
        {
            public const string INVALID_2_FACTOR_PROVIDER = "Invalid 2-Factor Verification Provider.";
            public const string INVALID_2_FACTOR_TOKEN = "Invalid 2-Factor verification token. Try logging in again";
            public const string INVALID_2_FACTOR_CODE = "Invalid 2-Factor verification code.";
            public static readonly string MULTI_FACTOR_NOT_YET_VERIFIED = $"Unauthorized: Two-factor authentication required.";
            public static readonly string NO_MSG_PROVIDER_SET = $"No message provider has been setup yet.";
            public static readonly string MULTI_FACTOR_NOT_ENABLED = $"2-Factor auith has not been enabled on this account";
            public static string NO_EMAIL_FOR_TWO_FACTOR(AppUser user) => user.UserName.IsNullOrWhiteSpace() ? NO_EMAIL_FOR_TWO_FACTOR(user.Id.ToString()) : NO_EMAIL_FOR_TWO_FACTOR(user.UserName!);
            public static string NO_EMAIL_FOR_TWO_FACTOR(string userIdentifier) => $"There is no email address on {userIdentifier}'s account.{NL}A email number is required for 2-Factor authentication if you have set 'email' as the preferred 2-Factor method.";
            public static string NO_PHONE_FOR_TWO_FACTOR(AppUser user) => user.UserName.IsNullOrWhiteSpace() ? NO_PHONE_FOR_TWO_FACTOR(user.Email!) : NO_PHONE_FOR_TWO_FACTOR(user.UserName!);
            public static string NO_PHONE_FOR_TWO_FACTOR(string userIdentifier) => $"There is no phone number on {userIdentifier}'s account.{NL}A phone number is required for Sms or WhatsApp 2-Factor authentication.";
            public static string TWO_FACTOR_EMAIL_FALLBACK(string email) => $"Sending sms has failed.{NL}We've emailed your code to {email} instead";
        }

        //-----------------------//

        public static class Authorization
        {
            //public static readonly string TWO_FACTOR_REQUIRED = $"Two-Factor auth required to complete login.";
            public static string TWO_FACTOR_REQUIRED(TwoFactorProvider provider) => $"Two-Factor auth required to complete login.{NL}Provider: {provider}";

            public const string UNAUTHORIZED = "Incorrect username or password.";
            public static string UNAUTHORIZED_FOR_POSITION(int position) => $"You must have a position of {position} or higher to complete this ;[";
            public static string UNAUTHORIZED_FOR_POSITION(string position) => $"You must have a position of {position} or higher to complete this action.";
            public static string UNAUTHORIZED_FOR_ROLE(string roleName) => $"You must have a role of {roleName} or higher to complete this action.";
            public const string UNAUTHORIZED_DEVICE = "This device is not on the list of registered devices";
            public const string GENERAL_UNAUTHORIZED = "You do not have the correct authorization to access this resource";
            public static readonly string INVALID_AUTH = $"Invalid username or password.";
            public static readonly string INVALID_AUTH_EXPIRED_TOKEN = $"Expired Token.";
            public static readonly string INVALID_LOGIN = $"Login Failure.{NL}Invalid username or password.";
            public const string NON_MATCHING_PASSOWRDS = "Passwords don't match.";
        }

        //-----------------------//

        public static class Users
        {
            public static readonly string CantDeleteFromAnotherTeam = $"You can only delete members of your own team!";
            public static readonly string CantUpdateFromAnotherTeam = $"You can only update members of your own team!";
            public static string CanOnlyUpdateSelf => $"You can only update yourself!.";
            public static string CantDeleteSelf => $"You can't delete yourself!.";
            public static string MissingOauthSetupData<TUser>(TUser user) where TUser : AppUser =>
                $"Team {user.FriendlyName} is missing OAuth Setup data.";
        }

        //-----------------------//

        public static class GoogleAuth
        {
            public static readonly string InvalidPin = $"The PIN supplied was invalid.";
        }



    }


}//Cls
