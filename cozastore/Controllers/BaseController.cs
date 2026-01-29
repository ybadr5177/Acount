using Microsoft.AspNetCore.Mvc;

namespace cozastore.Controllers
{
    public class BaseController : Controller
    {
        protected string CurrentLang
        {
            get
            {
                var lang = Request.Cookies["site_lang"];
                return string.IsNullOrEmpty(lang) ? "ar" : lang;
            }
        }
    }
}
