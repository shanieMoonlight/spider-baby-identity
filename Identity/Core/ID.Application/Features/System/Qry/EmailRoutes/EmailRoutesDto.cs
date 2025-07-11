namespace ID.Application.Features.System.Qry.EmailRoutes;
public class EmailRoutesDto()
{
    public string ConfirmEmail { get; set; } = string.Empty;
    public string ConfirmEmailWithPassword { get; set; } = string.Empty;
    //public string ConfirmEmailSpr { get; set; } = string.Empty;
    //public string ConfirmEmailGuest { get; set; } = string.Empty;
    public Params RouteParams { get; set; } = new Params();

    public class Params
    {
        public string UserId { get; set; } = string.Empty;
        public string ConfirmationToken { get; set; } = string.Empty;
    }
}
//public record EmailRoutesDto(string Routes);
