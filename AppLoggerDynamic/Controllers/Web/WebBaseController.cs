using Microsoft.AspNetCore.Mvc;

namespace AppLoggerDynamic.Controllers.Web
{
    public abstract class WebBaseController : Controller
    {
        private const string AccountIdKey = "AccountId";

        protected int? GetSessionAccountId() =>
            HttpContext.Session.GetInt32(AccountIdKey);

        protected void SetSessionAccountId(int accountId) =>
            HttpContext.Session.SetInt32(AccountIdKey, accountId);

        protected void ClearSession() =>
            HttpContext.Session.Clear();

        protected IActionResult? RequireAuth()
        {
            if (GetSessionAccountId() == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return null;
        }
    }
}
