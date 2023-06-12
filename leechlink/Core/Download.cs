using leechlink.Helper;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace leechlink.Core
{
  public class Download
  {
    string UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";

    public string Curl(string url, string cookies, string post, int header = 1, int json = 0, int @ref = 0, int xml = 0)
    {
      HttpClientHandler handler = new HttpClientHandler();
      handler.AllowAutoRedirect = false; // Disable auto-redirect
      handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true; // Disable SSL certificate validation
      HttpClient client = new HttpClient(handler);

      client.DefaultRequestHeaders.Add("Connection", "keep-alive");
      client.DefaultRequestHeaders.Add("Keep-Alive", "300");
      client.DefaultRequestHeaders.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
      client.DefaultRequestHeaders.Add("Accept-Language", "en-us,en;q=0.5");
      if (header == 1)
      {
        client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
      }
      if (json == 1)
      {
        client.DefaultRequestHeaders.Add("Content-type", "application/json");
        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
      }

      if (xml == 1)
      {
        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
      }

      if (!string.IsNullOrEmpty(cookies))
      {
        client.DefaultRequestHeaders.Add("Cookie", cookies);
      }

      if (@ref == 0)
      {
        client.DefaultRequestHeaders.Referrer = new Uri(url);
      }
      else
      {
        client.DefaultRequestHeaders.Referrer = new Uri("");
      }

      if (!string.IsNullOrEmpty(post))
      {
        HttpContent content = new StringContent(post);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        HttpResponseMessage response = client.PostAsync(url, content).Result;
        string page = response.Content.ReadAsStringAsync().Result;
        return page;
      }
      else
      {
        HttpResponseMessage response = client.GetAsync(url).Result;
        string page = response.Content.ReadAsStringAsync().Result;
        return page;
      }
    }

    public async Task<string> DowLoadFile(string html) {

      HttpClientHandler handler = new HttpClientHandler();
      string filename = "";
      using (HttpClient httpClient = new HttpClient(handler))
      {
        using (HttpResponseMessage response2 = await httpClient.GetAsync(html, HttpCompletionOption.ResponseHeadersRead))
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


    public string GetCookies(string content)
    {
      Regex regex = new Regex(@"Set-Cookie:\s*(.*?)\s*;");
      MatchCollection matches = regex.Matches(content);

      Dictionary<string, string> cookies = new Dictionary<string, string>();
      foreach (Match match in matches)
      {
        string cookie = match.Groups[1].Value;
        int separatorIndex = cookie.IndexOf('=');
        if (separatorIndex != -1)
        {
          string key = cookie.Substring(0, separatorIndex);
          string value = cookie.Substring(separatorIndex + 1);
          cookies[key] = value;
        }
      }

      StringBuilder cookieString = new StringBuilder();
      foreach (var pair in cookies)
      {
        cookieString.Append($"{pair.Key}={pair.Value}; ");
      }

      return cookieString.ToString();
    }

    public string GetAllCookies(string page)
    {
      string[] lines = page.Split('\n');
      string retCookie = "";
      foreach (string line in lines)
      {
        Match match = Regex.Match(line, @"Set-Cookie: (.*)");
        if (match.Success)
        {
          string cookie = match.Groups[1].Value;
          int separatorIndex = cookie.IndexOf(';');
          if (separatorIndex != -1)
          {
            cookie = cookie.Substring(0, separatorIndex);
          }
          retCookie += cookie + ";";
        }
      }

      return retCookie;
    }
  }
}
