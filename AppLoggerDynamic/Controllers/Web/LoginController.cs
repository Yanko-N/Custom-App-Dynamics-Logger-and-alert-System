using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppLoggerDynamic.Controllers.Web
{
    [Route("")]
    public class LoginController : WebBaseController
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public LoginController(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            if (GetSessionAccountId() != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] string apiKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ViewBag.Error = "Please enter an API key.";
                return View("Index");
            }

            var key = await _apiKeyRepository.ValidateApiKeyAsync(apiKey.Trim(), cancellationToken);
            if (key == null)
            {
                ViewBag.Error = "Invalid or expired API key.";
                return View("Index");
            }

            SetSessionAccountId(key.AccountId);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            ClearSession();
            return RedirectToAction("Index", "Login");
        }
    }
}
