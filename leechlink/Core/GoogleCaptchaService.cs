using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace leechlink.Core
{
  public class GoogleCaptchaService
  {
    private readonly IOptionsMonitor<GoogleCaptchaConfig> _config;
    public GoogleCaptchaService(IOptionsMonitor <GoogleCaptchaConfig> config) {
      _config = config;
    }
    public async Task<bool> VerifyToken(string token) { 

    
      try {


        var url = $"https://www.google.com/recaptcha/api/siteverify={_config.CurrentValue.SecretKey}&response={token}";
        using (var client = new HttpClient()) { 
          var httpResult= await client.GetAsync(url);
          if (!httpResult.IsSuccessStatusCode) { 
            return false;
          }
          var responseString = await httpResult.Content.ReadAsStringAsync();
          var googleResult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);
          return googleResult.Success && googleResult.score >= 0;
        }
      
      
      }
      
      
      catch
      { 
                return false;
      }
    }
  }
}
