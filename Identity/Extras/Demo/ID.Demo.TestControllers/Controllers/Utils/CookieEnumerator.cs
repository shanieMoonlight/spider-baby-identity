using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace ID.Demo.TestControllers.Controllers.Utils;

public class CookieEnumerator
{
    public static void EnumerateFirstCookieValues(IRequestCookieCollection cookies)
    {
        if (cookies == null || cookies.Count == 0)
        {
            Debug.WriteLine("No cookies found in the request.");
            return;
        }
        // Get the first cookie
        var firstCookie = cookies.FirstOrDefault();

        Debug.WriteLine($"Name of the first cookie: {firstCookie.Key}");

        // The 'Value' property of a cookie is typically the main value.
        // If a cookie has sub-values (e.g., "cookieName=key1=value1&key2=value2"),
        // then you might need to parse the 'Value' string further.
        // However, the IRequestCookieCollection itself contains distinct cookies.

        // If you mean iterating through the 'values' within a single cookie string
        // that might be structured with multiple key-value pairs separated by '&' or similar,
        // you'd typically parse the 'Value' string.
        //
        // Example of parsing the Value string for multiple key-value pairs:
        string cookieValue = firstCookie.Value;
        Debug.WriteLine($"Main value of the first cookie: {cookieValue}");

        // If the cookie's value itself is structured with multiple key-value pairs (e.g., using '&' as a separator)
        // and you want to iterate over those inner key-value pairs, you'd parse the string:
        var innerValues = System.Web.HttpUtility.ParseQueryString(cookieValue); // Requires System.Web assembly for HttpUtility

        if (innerValues.Count > 0)
        {
            Debug.WriteLine("Inner values within the first cookie:");
            foreach (string key in innerValues.Keys)
            {
                Debug.WriteLine($"  Key: {key}, Value: {innerValues[key]}");
            }
        }
        else
        {
            Debug.WriteLine("No discernible inner key-value pairs found in the first cookie's value.");
        }

    }
}
