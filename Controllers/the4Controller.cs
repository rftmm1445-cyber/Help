using Microsoft.AspNetCore.Mvc;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class the4Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Randomm()
        {

            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrWhiteSpace(username))
            {
                TempData["Message"] = "يرجى تسجيل الدخول أولاً.";
                return RedirectToAction("Login", "Account");
            }

            /* =====================================
             * 2) جلب اللاعب الحالي
             * ===================================== */
            Player currentPlayer = PlayerRepository.GetPlayerByUsername(username);
            ViewBag.CurrentPlayer = currentPlayer;

            return View();
        }
    }
}
