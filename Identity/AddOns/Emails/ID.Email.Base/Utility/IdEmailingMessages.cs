namespace ID.Email.Base.Utility;

public class IdEmailingMessages
{

    public static class Info
    {
        public const string SENT_SUCCESSFULLY = "Emails have been sent successfully.";
    }

    //#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#//

    public static class Error
    {
        public const string WRONG_TYPE = "Wrong email type.";
        public const string SEND_FAILURE_GENERAL = "Emails have been not sent.";
        public const string NO_VALID_EMAILS = "No valid emails found.";
    }


}//Cls