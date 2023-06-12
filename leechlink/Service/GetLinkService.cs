using leechlink.Helper;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using leechlink.Host;

namespace leechlink.Service
{
    public class GetLinkService
    {
        public GetLinkService()
        {
        }
        public async Task<string> SwitchHost(string url)
        {
            string link = "";
            string host = GetMainDomain(url);      
            switch (host)
            {
                case "rapidgator.net":
                    var fileServer = new RapidgatorHost();
                    link = await fileServer.GetCookieByLogin();
                    break;
                default:
                    link = "";
                    break;
            }
            return link;
        }

        static string GetMainDomain(string url)
        {
            Uri uri = new Uri(url);
            string host = uri.Host;

            // Kiểm tra xem host có chứa "www" hay không
            if (host.StartsWith("www."))
            {
                // Loại bỏ "www"
                host = host.Substring(4);
            }
            // Tách tên miền chính
            string[] parts = host.Split('.');
            if (parts.Length >= 2)
            {
                return parts[parts.Length - 2] + "." + parts[parts.Length - 1];
            }
            // Nếu không tìm thấy tên miền chính, trả về host ban đầu
            return host;
        }

    }
}
