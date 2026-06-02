using Microsoft.AspNetCore.Mvc;
using WebApplication4.Data;

namespace WebApplication4.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Start()
        {
            int? playerId = HttpContext.Session.GetInt32("PlayerID");
            if (!playerId.HasValue)
            {
                TempData["Message"] = "يرجى تسجيل الدخول أولاً.";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddPoints([FromBody] PointsModel data)
        {
            int? playerId = HttpContext.Session.GetInt32("PlayerID");
            if (!playerId.HasValue) return Json(new { success = false });

            PlayerRepository.AddPointsToPlayer(playerId.Value, data.points);
            return Json(new { success = true });
        }
    }

    public class PointsModel { public int points { get; set; } }
}
