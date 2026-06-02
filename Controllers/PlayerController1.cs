using Microsoft.AspNetCore.Mvc;
using WebApplication4.Data; // استدعاء الـ Namespace الخاص بالـ Repository

namespace WebApplication4.Controllers
{
    public class PlayerController : Controller
    {
        [HttpPost]
        public IActionResult SaveProgress(int id, int points)
        {
            // استدعاء الدالة المجهزة مسبقاً في الـ Repository الذي أرسلته
            bool success = PlayerRepository.AddPointsToPlayer(id, points);

            if (success)
            {
                return Json(new { success = true, message = "تم تحديث نقاط ومستوى اللاعب بنجاح!" });
            }
            else
            {
                return Json(new { success = false, message = "حدث خطأ أثناء تحديث بيانات اللاعب في قاعدة البيانات." });
            }
        }
    }
}