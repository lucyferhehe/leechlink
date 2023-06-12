using System.Net;
using HtmlAgilityPack;
using leechlink.Core;
using leechlink.Helper;
namespace leechlink.Host
{
  public class RapidgatorHost : Download
  {
    public RapidgatorHost() { }
    public async Task<string> GetFileRapidgator(string url)
    {
      string cookie = "user__=2.0%7CP3CRYz3PM0M30dq1%2FzQymHH5SenzxUyXzwb1MsQkFcvNZMyJeZGxSCxcaarCSDZhmsbKv4QaBj9eVZTknLbrqagJL7RciwUDqGwImPHGgzVPHp02TGgXW93vpJdgdXc1M7i%2FsxJ9bat2yffyNSpwsQ57klYF4gBvE%2Bh5Rs5KxrA%3D%7C1686231990%7Cd89ffc5ec8928916357cb16dd74ecee81d049aee; expires=Sat, 08 Jul 2023 13:46:30 GMT; Max-Age=2592000; path=/; domain=.rapidgator.net; secure; HttpOnly; SameSite=Lax";
      string page = Curl(url, cookie, "");
      string linhFile = GetFileLink(page);
      string file = await DowLoadFile(linhFile);
      // return file url
      return file;
    }
    public string GetFileLink(string htmlContent)
    {
      string href = "";
      HtmlDocument document = new HtmlDocument();
      document.LoadHtml(htmlContent);
      // Tìm tất cả các thẻ div có class là "file-descr"
      HtmlNodeCollection divNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'file-descr')]");
      if (divNodes != null)
      {
        foreach (HtmlNode divNode in divNodes)
        {
          // Tìm các thẻ a trong div
          HtmlNodeCollection aNodes = divNode.SelectNodes(".//a");
          if (aNodes != null)
          {
            foreach (HtmlNode aNode in aNodes)
            {
              // Lấy giá trị href của thẻ a
              href = aNode.GetAttributeValue("href", string.Empty);
              // Xử lý giá trị href tùy ý ở đây
              return href;
            }
          }
        }
      }
      return href;
    }
    // login return cookie
    public async Task<string> Login()
    {
      string cookieString = "";
      using (HttpClient httpClient = new HttpClient())
      {
        string username = "vicente.telles@gmail.com";
        string password = "dejavu2510";
        var requestContent = new FormUrlEncodedContent(new[]
          {
            new KeyValuePair<string, string>("LoginForm[email]", username),
            new KeyValuePair<string, string>("LoginForm[password]", password),
            new KeyValuePair<string, string>("LoginForm[activation_code]", ""),
            new KeyValuePair<string, string>("LoginForm[twoStepAuthCode]", ""),
            new KeyValuePair<string, string>("LoginForm[rememberMe]", "1"),
        });
        var response = await httpClient.PostAsync("https://rapidgator.net/auth/login", requestContent);
        response.EnsureSuccessStatusCode();
        if (response.IsSuccessStatusCode)
        {
          if (response.Headers.TryGetValues("Set-Cookie", out var cookieValues))
          {
            cookieString = string.Join("; ", cookieValues);
          }
          else
          {
            cookieString = string.Empty;
          }
        }
      }
      return cookieString;
    }

  }
}
