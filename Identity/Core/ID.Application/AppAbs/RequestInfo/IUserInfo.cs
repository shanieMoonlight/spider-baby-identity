namespace ID.Application.AppAbs.RequestInfo;

public interface IUserInfo
{
    string GetLoggedInNameAndEmail(string? fallback = null);
    string GetLoggedInUserEmail(string? fallback = null);
    string GetLoggedInUserId(string? fallback = null);
    string GetLoggedInUserName(string? fallback = null);
}