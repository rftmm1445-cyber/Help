using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class AccountController : Controller
    {
        // =========================
        // صفحة تسجيل الدخول GET
        // =========================
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        // =========================
        // تسجيل الدخول POST
        // =========================
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message = "الرجاء إدخال اسم المستخدم وكلمة المرور.";
                return View();
            }

            // البحث عن اللاعب بواسطة username وكلمة المرور (مشفر)
            string hashedPassword = PlayerRepository.HashPassword(password);
            var player = PlayerRepository.GetPlayer(username, hashedPassword);

            if (player != null)
            {
                // 4️⃣ التحقق من أن الجلسة مفعلة
                if (!HttpContext.Session.IsAvailable)
                {
                    throw new InvalidOperationException("Session غير مفعلة، تأكدي من إضافة UseSession وAddSession في Program.cs");
                }

                // 5️⃣ تخزين بيانات الجلسة
                HttpContext.Session.SetString("Username", player.Username);
                HttpContext.Session.SetInt32("PlayerID", player.Id);

                //TempData["Message"] = $"مرحباً {player.Username}!";
                //return RedirectToAction("Index", "Home"); // الانتقال للصفحة الرئيسية
                return RedirectToAction("PlayerHub", "Dashboard");
            }
            else
            {
                ViewBag.Message = "اسم المستخدم أو كلمة المرور غير صحيحة.";
                return View();
            }
        }

        // =========================
        // صفحة تسجيل حساب جديد GET
        // =========================
        [HttpGet]
        public IActionResult SignUp()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        // =========================
        // تسجيل حساب جديد POST
        // =========================
        [HttpPost]
        public IActionResult SignUp(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message = "الرجاء تعبئة جميع الحقول.";
                return View();
            }

            // التحقق إذا كان هناك لاعب بنفس اسم المستخدم
            if (PlayerRepository.GetPlayerByUsername(username) != null)
            {
                ViewBag.Message = "اسم المستخدم موجود بالفعل. الرجاء اختيار اسم آخر.";
                return View();
            }

            // التحقق من قوة كلمة المرور
            if (!PlayerRepository.IsStrongPassword(password))
            {
                ViewBag.Message = "كلمة المرور ضعيفة، يجب أن تحتوي على 8 أحرف على الأقل، حرف كبير، حرف صغير، ورقم.";
                return View();
            }

            // إنشاء لاعب جديد
            Player newPlayer = new Player
            {
                Username = username,
                Email = email,
                Password = PlayerRepository.HashPassword(password), // تشفير كلمة المرور
                Score = 0,
                pLevel = 1
            };

            bool added = PlayerRepository.AddPlayer(newPlayer);

            if (added)
            {
                TempData["Message"] = "تم إنشاء الحساب بنجاح! يمكنك تسجيل الدخول الآن.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Message = "حدث خطأ أثناء إنشاء الحساب، حاول مرة أخرى.";
                return View();
            }
        }

        // =========================
        // تسجيل الخروج
        // =========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Message"] = "تم تسجيل الخروج بنجاح.";
            return RedirectToAction("Login");
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Data.OleDb;
//using WebApplication4.Models;

//namespace WebApplication4.Controllers
//{
//    public class AccountController : Controller
//    {
//        // نص الاتصال الموحد بقاعدة البيانات
//        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\Desktop\WebApplication4\App_Data\GameDB.accdb;Persist Security Info=False;";
//        //private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\GameDB.accdb;Persist Security Info=False;";

//        // 1. عرض صفحة تسجيل الدخول
//        public IActionResult Login()
//        {
//            return View();
//        }

//        // 2. معالجة عملية تسجيل الدخول
//        [HttpPost]
//        public IActionResult Login(string username, string password)
//        {
//            using (OleDbConnection conn = new OleDbConnection(connectionString))
//            {
//                string query = "SELECT * FROM Players WHERE Username = @User AND [Password] = @Pass";
//                OleDbCommand cmd = new OleDbCommand(query, conn);
//                cmd.Parameters.AddWithValue("@User", username);
//                cmd.Parameters.AddWithValue("@Pass", password);

//                conn.Open();
//                using (OleDbDataReader reader = cmd.ExecuteReader())
//                {
//                    if (reader.Read())
//                    {
//                        // إنشاء الجلسة (Session) لحفظ اسم اللاعب
//                        HttpContext.Session.SetString("Username", username);
//                        return RedirectToAction("Index", "Gameround"); // التوجيه الفوري لصفحة اللعب والخريطة
//                    }
//                }
//            }

//            ViewBag.Error = "اسم المستخدم أو كلمة المرور غير صحيحة!";
//            return View();
//        }

//        // 3. عرض صفحة إنشاء حساب جديد
//        public IActionResult Register()
//        {
//            return View();
//        }

//        // 4. معالجة إنشاء حساب جديد (يبدأ بـ 0 نقاط ومستوى 1 تلقائياً)
//        [HttpPost]
//        public IActionResult Register(string username, string email, string password)
//        {
//            using (OleDbConnection conn = new OleDbConnection(connectionString))
//            {
//                // الاستعلام لإدخال اللاعب الجديد مع تصفير النقاط وبدء المستوى الأول
//                string query = "INSERT INTO Players (Username, Email, [Password], Score, pLevel) VALUES (@User, @Email, @Pass, 0, 1)";
//                OleDbCommand cmd = new OleDbCommand(query, conn);
//                cmd.Parameters.AddWithValue("@User", username);
//                cmd.Parameters.AddWithValue("@Email", email);
//                cmd.Parameters.AddWithValue("@Pass", password);

//                conn.Open();
//                try
//                {
//                    cmd.ExecuteNonQuery();
//                    HttpContext.Session.SetString("Username", username); // تسجيل الدخول تلقائياً بعد التسجيل
//                    return RedirectToAction("Index", "Gameround");
//                }
//                catch (Exception)
//                {
//                    ViewBag.Error = "اسم المستخدم أو البريد الإلكتروني مسجل مسبقاً!";
//                    return View();
//                }
//            }
//        }

//        // 5. تسجيل الخروج وتدمير الجلسة
//        public IActionResult Logout()
//        {
//            HttpContext.Session.Clear();
//            return RedirectToAction("Login");
//        }
//    }
//}
