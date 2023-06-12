using System.Net;
using HtmlAgilityPack;
using leechlink.Core;
using leechlink.Helper;
namespace leechlink.Host
{
  public class RapidgatorHost2: Download
  {
    public RapidgatorHost2() { }
    public async Task<string> GetFileRapidgator(string url)
    {
      HttpClientHandler handler = new HttpClientHandler();
      handler.CookieContainer = new CookieContainer();
      string cookie = "user__=2.0%7CP3CRYz3PM0M30dq1%2FzQymHH5SenzxUyXzwb1MsQkFcvNZMyJeZGxSCxcaarCSDZhmsbKv4QaBj9eVZTknLbrqagJL7RciwUDqGwImPHGgzVPHp02TGgXW93vpJdgdXc1M7i%2FsxJ9bat2yffyNSpwsQ57klYF4gBvE%2Bh5Rs5KxrA%3D%7C1686231990%7Cd89ffc5ec8928916357cb16dd74ecee81d049aee; expires=Sat, 08 Jul 2023 13:46:30 GMT; Max-Age=2592000; path=/; domain=.rapidgator.net; secure; HttpOnly; SameSite=Lax";
      string linkFile = "";
      string filename = "";
      //get html contnent
      using (HttpClient httpClient = new HttpClient(handler))
      {
        Common.addCookie(handler, cookie, url);
        using (HttpResponseMessage response = await httpClient.GetAsync(url))
        {
          response.EnsureSuccessStatusCode();
          HttpContent http = response.Content;
          string html = http.ReadAsStringAsync().Result;
          linkFile = getFileLink(html);
        }

      }
      // get file url
      using (HttpClient httpClient2 = new HttpClient())
      {
        using (HttpResponseMessage response2 = await httpClient2.GetAsync(linkFile, HttpCompletionOption.ResponseHeadersRead))
        {
          response2.EnsureSuccessStatusCode();

          using (Stream contentStream = await response2.Content.ReadAsStreamAsync())
          {
            filename = Common.DecodeFileName(response2.Content.Headers.ContentDisposition.FileName.ToString());
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string filePath = Path.Combine(wwwrootPath, filename);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
              await contentStream.CopyToAsync(fileStream);
            }
          }
        }
      }
      return filename;
    }

    public async Task<string> GetCookieByLogin()
    {
      string cookie = "";
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
            new KeyValuePair<string, string>("g-recaptcha-response", "1"),
            new KeyValuePair<string, string>("LoginForm[fp]", "a45dbcd6b11ef1d151b36e25ae02d1cf"),
        });
        dynamic cookies;
        var response = await httpClient.PostAsync("https://rapidgator.net/auth/login", requestContent);

        response.EnsureSuccessStatusCode();
        if (response.IsSuccessStatusCode)
        {
          if (response.Headers.TryGetValues("Set-Cookie", out var cookieValues))
          {
             cookies = cookieValues.ToList();
            // Xử lý cookie theo nhu cầu của bạn
          }
          else if (response.Headers.TryGetValues("Location", out var redirectUrls))
          {
            string redirectUrl = redirectUrls.FirstOrDefault();
            var cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(new Uri("https://rapidgator.net"), response.Headers.GetValues("Set-Cookie").FirstOrDefault());
            using (var httpClientWithCookie = new HttpClient(new HttpClientHandler { CookieContainer = cookieContainer }))
            {
              response = await httpClientWithCookie.GetAsync(redirectUrl);
              if (response.Headers.TryGetValues("Set-Cookie", out cookieValues))
              {
                cookie = string.Join("; ", cookieValues);
              }
            }
          }
        }
        else {
          cookie = "No cookie received";
        }
        
        
        HttpContent http = response.Content;
        string html = http.ReadAsStringAsync().Result;
      
      }
       

      return cookie;
    }

    public string getFileLink(string htmlContent)
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
  }
}
