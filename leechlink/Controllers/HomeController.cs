using leechlink.Core;
using leechlink.Models;
using leechlink.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
namespace leechlink.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly GoogleCaptchaService _gService;
    private readonly GetLinkService _getLinkService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public HomeController(ILogger<HomeController> logger, GetLinkService getLinkService, GoogleCaptchaService gService, IHttpContextAccessor httpContextAccessor)
    {
      _logger = logger;
      _gService = gService;
      _getLinkService = getLinkService;
      _httpContextAccessor = httpContextAccessor;
    }
    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetLink(string url)
    {
      var Result =  await _getLinkService.SwitchHost(url);
      string baseUrl = $"{Request.Scheme}://{Request.Host}";

      // Tạo đường dẫn đầy đủ đến tệp
      string fileUrl = $"{baseUrl}/{Result}";
      ViewBag.FileUrl = fileUrl;
      return Json(new { fileUrl });
    }

    [HttpGet("download/{id}")]
    public IActionResult DownloadFile(string id)
    {
      // Kiểm tra xem id có hợp lệ hay không
      // Lấy thông tin về file dựa trên id
      string filePath = GetFilePathById(id);
      if (filePath == null)
      {
        return NotFound();
      }
      var fileBytes = System.IO.File.ReadAllBytes(filePath);
      var fileName = Path.GetFileName(filePath);
      return File(fileBytes, "application/octet-stream", fileName);
    }
    private string GetFilePathById(string id)
    {
      return Path.Combine("C:\\Files", id );
    }   
  }
 
}
