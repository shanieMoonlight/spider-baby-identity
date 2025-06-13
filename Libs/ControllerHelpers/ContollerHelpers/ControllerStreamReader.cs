using System.Security.Claims;
using System.Text;

namespace ControllerHelpers;

public static class ControllerStreamReader
{

    public static async Task<string> ReadMessageAsync(this Stream body, ClaimsPrincipal user, string controller, string action)
    {
        try
        {
            StreamReader reader = new(body);
            string? line;
            var sbMsg = new StringBuilder($"Controller: {controller}, Action: {action}");
            sbMsg.AppendLine();

            while ((line = await reader.ReadLineAsync()) != null)
            {
                sbMsg.AppendLine(line);
            }

            var msgString = sbMsg.ToString()
               .Trim()
               .Replace(@"\r\n", @$"{Environment.NewLine}")
               .Replace(@"\n", @$"{Environment.NewLine}")
               .Replace(@"\t", "    ");

            if (msgString.StartsWith('{'))
                msgString = msgString[1..];

            if (msgString.EndsWith('}'))
                msgString = msgString[..^1];

            sbMsg = new StringBuilder(msgString);

            sbMsg.AppendLine()
               .AppendLine()
               .AppendLine()
               .AppendLine("User Info:")
               .AppendLine();


            foreach (var claim in user.Claims)
            {
                sbMsg.AppendLine($"{claim.Type}: {claim.Value}");
            }//foreach


            return sbMsg.ToString().Replace(@""",""", @$""",{Environment.NewLine} """);
        }
        catch (Exception e)
        {
            return $"Error in : {nameof(ReadMessageAsync)}: {e.Message}";
        }

    }//Read

    //----------------------------//

    public static async Task<string> ReadMessageAsync(this Stream body)
    {
        try
        {

            StreamReader reader = new(body);
            string? line;
            var sbMsg = new StringBuilder();
            sbMsg.AppendLine();

            while ((line = await reader.ReadLineAsync()) != null)
            {
                sbMsg.AppendLine(line);
            }

            return sbMsg.ToString();
        }
        catch (Exception e)
        {
            return $"Error in : {nameof(ReadMessageAsync)}: {e.Message}";
        }

    }

}//Cls

