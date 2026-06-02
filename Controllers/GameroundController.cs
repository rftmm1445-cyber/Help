using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // ضروري للتعامل مع الـ Session
using WebApplication4.Models;
using WebApplication4.Data;

namespace WebApplication4.Controllers
{
    public class GameroundController : Controller
    {
        // =========================
        // عرض View Gameround (مع جلب بيانات اللاعب الحقيقية للبروفايل)
        // =========================
        public IActionResult Gameround()
        {
            // 1. التحقق من وجود معرّف اللاعب في الجلسة
            int? playerId = HttpContext.Session.GetInt32("PlayerID");
            if (!playerId.HasValue)
            {
                return RedirectToAction("Login", "Home");
            }

            // 2. جلب كائن اللاعب الحقيقي من قاعدة البيانات بواسطة الـ ID
            var player = PlayerRepository.GetPlayerByID(playerId.Value);
            if (player == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // 3. تمرير كائن اللاعب إلى الـ View لكي تعرض الشاشة نقاطه ومستواه الحقيقيين
            return View("Gameround", player);
        }

        // =========================
        // التحقق من التخمين وحساب النقاط (الكود الأصلي الخاص بك)
        // =========================
        [HttpPost]
        public IActionResult CheckGuess(double points)
        {
            int? playerId = HttpContext.Session.GetInt32("PlayerID");
            if (!playerId.HasValue) return RedirectToAction("Login", "Home");

            // تحديث النقاط مباشرة في Access
            PlayerRepository.AddPointsToPlayer(playerId.Value, (int)Math.Round(points));

            // الحصول على اللاعب بعد التحديث
            var player = PlayerRepository.GetPlayerByID(playerId.Value);

            // تمرير النتائج للفيو
            ViewBag.PointsEarned = points;
            ViewBag.TotalScore = player.Score;

            return View("Gameround", player); // مررنا الـ player هنا أيضاً للحفاظ على بيانات البروفايل علوياً
        }

        // ========================================================
        // العملية الجديدة: استقبال واستحقاق جائزة عجلة الحظ اليومية
        // ========================================================
        [HttpPost]
        public JsonResult ClaimWheelPrize(int points)
        {
            // 1. جلب الـ ID من الـ Session للتأكد من هوية اللاعب المسجل
            int? playerId = HttpContext.Session.GetInt32("PlayerID");

            if (playerId.HasValue)
            {
                // 2. جلب بيانات اللاعب قبل التحديث
                var player = PlayerRepository.GetPlayerByID(playerId.Value);

                if (player != null)
                {
                    // 3. حساب النقاط الجديدة
                    int updatedScore = player.Score + points;
                    int currentLevel = player.pLevel;

                    // 4. استخدام الدالة الموحدة الموجودة في كلاس Player لتحديث قاعدة البيانات حقيقياً
                    bool isSaved = Player.SavePlayerProgress(player.Id, updatedScore, currentLevel);

                    if (isSaved)
                    {
                        // إرسال رد نجاح إلى الـ JavaScript على الشاشة بالنقاط الجديدة لتحديث العداد فوراً
                        return Json(new { success = true, newScore = updatedScore });
                    }
                }
            }

            return Json(new { success = false });
        }
    }
}



//using Microsoft.AspNetCore.Mvc;
//using WebApplication4.Models;
//using WebApplication4.Data;

//namespace WebApplication4.Controllers
//{
//    public class GameroundController : Controller
//    {
//        // =========================
//        // عرض View Gameround
//        // =========================
//        public IActionResult Gameround()
//        {
//            return View("Gameround"); // يبقى في فولدر Home
//        }

//        // =========================
//        // التحقق من التخمين وحساب النقاط
//        // =========================
//        [HttpPost]
//        public IActionResult CheckGuess(double points)
//        {
//            int? playerId = HttpContext.Session.GetInt32("PlayerID");
//            if (!playerId.HasValue) return RedirectToAction("Login", "Home");

//            // تحديث النقاط مباشرة في Access
//            PlayerRepository.AddPointsToPlayer(playerId.Value, (int)Math.Round(points));

//            // الحصول على اللاعب بعد التحديث
//            var player = PlayerRepository.GetPlayerByID(playerId.Value);

//            // تمرير النتائج للفيو
//            ViewBag.PointsEarned = points;
//            ViewBag.TotalScore = player.Score;

//            return View("Gameround");
//        }
//    }
//}
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using WebApplication4.Models;

//namespace WebApplication4.Controllers
//{
//    public class GameroundController : Controller
//    {
//        private static readonly HttpClient _httpClient = new HttpClient();

//        // 1. أكشن عرض صفحة الخريطة واللعب
//        public IActionResult Index()
//        {
//            string username = HttpContext.Session.GetString("Username");
//            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

//            Player player = Player.GetPlayerByUsername(username);
//            return View(player); // تمرير اللاعب لرؤية نقاطه الحالية على الواجهة
//        }

//        // 2. معالجة الإجابة الصحيحة على الخريطة (الجوائز والمكافآت)
//        [HttpPost]
//        public IActionResult ClaimReward()
//        {
//            string username = HttpContext.Session.GetString("Username");
//            if (string.IsNullOrEmpty(username)) return Json(new { success = false });

//            Player player = Player.GetPlayerByUsername(username);

//            // إضافة جائزة الفوز (مثلاً 50 نقطة)
//            player.Score += 50;

//            // معادلة ترقية الليفل المتلائمة تلقائياً (كل 100 نقطة ترفع ليفل)
//            player.pLevel = 1 + (player.Score / 100);

//            // حفظ التحديث فوراً في قاعدة البيانات MS Access
//            Player.SavePlayerProgress(player.Id, player.Score, player.pLevel);

//            return Json(new { success = true, newScore = player.Score, newLevel = player.pLevel });
//        }

//        // 3. طلب تلميح ذكي AI مخصص بناءً على اختيار اللاعب (تاريخ، رياضة، موسيقى)
//        [HttpPost]
//        public async Task<IActionResult> GetAiHint(string category, string targetLocation)
//        {
//            string username = HttpContext.Session.GetString("Username");
//            if (string.IsNullOrEmpty(username)) return Json(new { success = false, message = "الرجاء تسجيل الدخول." });

//            Player player = Player.GetPlayerByUsername(username);
//            int hintCost = 10; // تكلفة التلميح

//            if (player.Score < hintCost)
//            {
//                return Json(new { success = false, message = "نقاطك غير كافية لطلب تلميح! تحتاج 10 نقاط." });
//            }

//            // إعداد هندسة الأوامر وجلب التلميح الغامض بصيغة JSON
//            string apiKey = "YOUR_GEMINI_API_KEY";
//            var promptData = new
//            {
//                contents = new[] {
//                    new { parts = new[] { new { text = $"أنت مساعد في لعبة جغرافية. أعطني تلميحاً غامضاً وذكياً واحداً عن الموقع: ({targetLocation}) بشرط أن يكون التلميح متعلقاً بموضوع ({category}). شروط صارمة: لا تذكر اسم المعلم أو المدينة أو الدولة أبداً، واجعل الإخراج باللغة العربية داخل نص JSON نقي يحتوي على الحقل 'hint' فقط دون أي مقدمات." } } }
//                }
//            };

//            var content = new StringContent(JsonSerializer.Serialize(promptData), Encoding.UTF8, "application/json");
//            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}", content);

//            if (response.IsSuccessStatusCode)
//            {
//                string jsonResult = await response.Content.ReadAsStringAsync();

//                // فك تشفير استجابة الذكاء الاصطناعي النظيفة
//                using JsonDocument doc = JsonDocument.Parse(jsonResult);
//                string rawText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

//                // تنظيف النص في حال وجود علامات اقتباس برمجية مضافة
//                rawText = rawText.Replace("```json", "").Replace("```", "").Trim();

//                // خصم نقاط التلميح وتحديث المستوى تلقائياً في حال هبوط النقاط
//                player.Score -= hintCost;
//                player.pLevel = 1 + (player.Score / 100);

//                // حفظ التغييرات في قاعدة البيانات
//                Player.SavePlayerProgress(player.Id, player.Score, player.pLevel);

//                return Json(new { success = true, aiResponse = rawText, newScore = player.Score, newLevel = player.pLevel });
//            }

//            return Json(new { success = false, message = "فشل الاتصال بمولد التلميحات الذكي." });
//        }
//    }
//}