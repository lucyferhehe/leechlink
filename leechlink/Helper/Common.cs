using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
namespace leechlink.Helper
{
  public static class Common
  {
    public static void addCookie(HttpClientHandler handler, string cookie, string url)
    {
      var temp = cookie.Split(';');
      foreach (var item in temp)
      {
        var temp2 = item.Split('=');
        if (temp2.Count() > 1)
        {
          handler.CookieContainer.Add(new Uri(url), new Cookie(temp2[0].Trim(), temp2[1].Trim()));
        }
      }

    }

    public static string DecodeFileName(string encodedFileName)
    {
      string pattern = "\"(.*)\"";
      Match match = Regex.Match(encodedFileName, pattern);

      if (match.Success && match.Groups.Count > 1)
      {
        string encodedValue = match.Groups[1].Value;
        return Regex.Unescape(encodedValue);
      }
      return string.Empty;
    }
    
  }
}
