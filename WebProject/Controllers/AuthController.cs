using Microsoft.AspNetCore.Mvc;

namespace WebProject.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
