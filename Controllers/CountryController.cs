//using Microsoft.AspNetCore.Mvc;
//using WebApplication4.Models;
//using Microsoft.AspNetCore.Http;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Collections.Generic;

//namespace WebApplication4.Controllers
//{
//    public class CountryController : Controller
//    {
//        // ================== عرض كل الدول ==================
//        public IActionResult ShowCountries()
//        {
//            ViewBag.Countries = Country.GetAllCountries() ?? new List<Country>();
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
//            ViewBag.Total = ViewBag.Countries.Count;
//            return View("ShowCountries");
//        }

//        // ================== عرض صفحة إضافة دولة ==================
//        public IActionResult AddNewCountryView()
//        {
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
//            return View("AddNewCountryView", new Country());
//        }

//        // ================== إضافة دولة جديدة ==================
//        [HttpPost]
//        public async Task<IActionResult> DoAddCountry(Country c, IFormFile Country_Flag_File)
//        {
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();

//            if (!ModelState.IsValid)
//                return View("AddNewCountryView", c);

//            if (Country_Flag_File != null && Country_Flag_File.Length > 0)
//            {
//                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Flags");
//                if (!Directory.Exists(uploadsFolder))
//                    Directory.CreateDirectory(uploadsFolder);

//                string fileName = $"{System.DateTime.Now.Ticks}_{Country_Flag_File.FileName}";
//                string filePath = Path.Combine(uploadsFolder, fileName);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await Country_Flag_File.CopyToAsync(stream);
//                }

//                c.Country_Flag = "/Flags/" + fileName;
//            }

//            int rows = Country.AddNewCountry(c);
//            TempData["Success"] = rows > 0 ? $"تم إضافة الدولة {c.Country_Name} بنجاح" : "حدث خطأ أثناء إضافة الدولة";

//            return RedirectToAction("ShowCountries");
//        }

//        // ================== تعديل دولة ==================
//        public IActionResult EditCountry(int Country_Id)
//        {
//            var c = Country.GetCountryById(Country_Id);
//            if (c == null) return NotFound();

//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
//            return View("EditCountryView", c);
//        }

//        [HttpPost]
//        public async Task<IActionResult> DoUpdateCountry(Country c, IFormFile Country_Flag_File)
//        {
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();

//            if (!ModelState.IsValid)
//                return View("EditCountryView", c);

//            if (Country_Flag_File != null && Country_Flag_File.Length > 0)
//            {
//                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Flags");
//                if (!Directory.Exists(uploadsFolder))
//                    Directory.CreateDirectory(uploadsFolder);

//                string fileName = $"{System.DateTime.Now.Ticks}_{Country_Flag_File.FileName}";
//                string filePath = Path.Combine(uploadsFolder, fileName);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await Country_Flag_File.CopyToAsync(stream);
//                }

//                c.Country_Flag = "/Flags/" + fileName;
//            }

//            int rows = Country.UpdateCountry(c);
//            TempData["Success"] = rows > 0 ? $"تم تعديل الدولة {c.Country_Name} بنجاح" : "حدث خطأ أثناء تعديل الدولة";

//            return RedirectToAction("ShowCountries");
//        }

//        // ================== حذف دولة ==================
//        public IActionResult DeleteCountry(int Country_Id)
//        {
//            int rows = Country.DeleteCountryById(Country_Id);
//            TempData["Success"] = rows > 0 ? "تم حذف الدولة بنجاح" : "حدث خطأ أثناء الحذف";
//            return RedirectToAction("ShowCountries");
//        }

//        // ================== عرض تفاصيل الدولة ==================
//        public IActionResult CountryDetails(int Country_Id)
//        {
//            var c = Country.GetCountryById(Country_Id);
//            if (c == null) return NotFound();
//            return View("CountryDetailsView", c);
//        }

//        // ================== البحث ==================
//        public IActionResult SmartSearch(string keyword)
//        {
//            var countries = string.IsNullOrEmpty(keyword)
//                ? Country.GetAllCountries() ?? new List<Country>()
//                : Country.SearchByNameOrCapital(keyword) ?? new List<Country>();

//            ViewBag.Countries = countries;
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
//            ViewBag.Keyword = keyword;

//            return View("ShowCountries");
//        }
//    }
//}






//using Microsoft.AspNetCore.Mvc;
//using WebApplication4.Models;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using System.Security.Claims;

//namespace WebApplication4.Controllers
//{
//    public class CountryController : Controller
//    {
//        // ================== الصفحة الرئيسية ==================
//        public IActionResult Index()
//        {
//            var countries = Country.GetAllCountries() ?? new List<Country>();
//            var continents = Continent.GetAllContinents() ?? new List<Continent>();

//            ViewBag.Countries = countries;
//            ViewBag.Continents = continents;
//            ViewBag.Total = countries.Count;

//            return View("ShowCountries");
//        }

//        // ================== إدارة الدول ==================
//        public IActionResult ManageCountries()
//        {
//            var countries = Country.GetAllCountries() ?? new List<Country>();
//            var continents = Continent.GetAllContinents() ?? new List<Continent>();

//            ViewBag.Countries = countries;
//            ViewBag.Continents = continents;

//            return View("ManageCountries");
//        }

//        // صفحة إضافة دولة جديدة
//        public IActionResult AddNewCountryView()
//        {
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
//            return View("AddNewCountryView", new Country());
//        }

//        // تنفيذ إضافة دولة جديدة
//        [HttpPost]
//        public async Task<IActionResult> DoAddCountry(Country c, IFormFile Country_Flag_File)
//        {
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();

//            if (!ModelState.IsValid)
//            {
//                return View("AddNewCountryView", c);
//            }

//            if (Country_Flag_File != null && Country_Flag_File.Length > 0)
//            {
//                var fileName = await UploadFile(Country_Flag_File, "Flags");
//                c.Country_Flag = "/Flags/" + fileName;
//            }

//            int newId = Country.AddNewCountry(c);
//            TempData["Success"] = newId > 0 ? $"تم إضافة الدولة {c.Country_Name} بنجاح" : "حدث خطأ أثناء إضافة الدولة";

//            return RedirectToAction("ManageCountries");
//        }

//        // صفحة تعديل الدولة
//        public IActionResult EditCountry(int Country_Id)
//        {
//            var c = Country.GetCountryById(Country_Id);
//            if (c == null) return NotFound();

//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
//            return View("EditCountryView", c);
//        }

//        // حفظ تعديل الدولة
//        [HttpPost]
//        public async Task<IActionResult> DoUpdateCountry(Country c, IFormFile Country_Flag_File)
//        {
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();

//            if (!ModelState.IsValid)
//            {
//                return View("EditCountryView", c);
//            }

//            if (Country_Flag_File != null && Country_Flag_File.Length > 0)
//            {
//                var fileName = await UploadFile(Country_Flag_File, "Flags");
//                c.Country_Flag = "/Flags/" + fileName;
//            }

//            int updated = Country.UpdateCountry(c);
//            TempData["Success"] = updated > 0 ? $"تم تعديل الدولة {c.Country_Name} بنجاح" : "حدث خطأ أثناء تعديل الدولة";

//            return RedirectToAction("ManageCountries");
//        }

//        // حذف دولة
//        public IActionResult DeleteCountry(int Country_Id)
//        {
//            int deleted = Country.DeleteCountryById(Country_Id);
//            TempData["Success"] = deleted > 0 ? "تم حذف الدولة بنجاح" : "حدث خطأ أثناء الحذف";

//            return RedirectToAction("ManageCountries");
//        }

//        // عرض تفاصيل الدولة
//        public IActionResult CountryDetails(int Country_Id)
//        {
//            var c = Country.GetCountryById(Country_Id);
//            if (c == null) return NotFound();

//            return View("CountryDetailsView", c);
//        }

//        // ================== البحث ==================
//        public IActionResult SmartSearch(string keyword)
//        {
//            var countries = string.IsNullOrEmpty(keyword)
//                ? Country.GetAllCountries() ?? new List<Country>()
//                : Country.SearchByNameOrCapital(keyword) ?? new List<Country>();

//            ViewBag.Countries = countries;
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
//            ViewBag.Keyword = keyword;

//            return View("ManageCountries");
//        }

//        // ================== الإحصائيات ==================
//        public IActionResult CountryStatistics()
//        {
//            var countries = Country.GetAllCountries() ?? new List<Country>();
//            var continents = Continent.GetAllContinents() ?? new List<Continent>();

//            ViewBag.TotalCountries = countries.Count;
//            ViewBag.TotalPopulation = countries.Sum(c => c.Country_Population);
//            ViewBag.AverageArea = countries.Count > 0 ? countries.Average(c => c.Country_Area) : 0;

//            ViewBag.CountriesPerContinent = continents.Select(cont => new
//            {
//                ContinentName = cont.Name,
//                CountryCount = countries.Count(c => c.Continent_Id == cont.Id)
//            }).ToList();

//            return View();
//        }

//        // ================== تصدير CSV ==================
//        public IActionResult ExportCountriesToCSV()
//        {
//            var countries = Country.GetAllCountries() ?? new List<Country>();
//            var csv = "Country_Name,Country_Capital,Country_Population,Country_Area,Continent\n";

//            foreach (var c in countries)
//            {
//                var continent = Continent.GetContinentById(c.Continent_Id)?.Name ?? "";
//                csv += $"{c.Country_Name},{c.Country_Capital},{c.Country_Population},{c.Country_Area},{continent}\n";
//            }

//            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
//            return File(bytes, "text/csv", "Countries.csv");
//        }

//        // ================== عرض الدول حسب القارة ==================
//        public IActionResult CountriesByContinent(int Continent_Id)
//        {
//            var countries = Country.GetCountriesByContinentId(Continent_Id) ?? new List<Country>();
//            ViewBag.Countries = countries;
//            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
//            ViewBag.ContinentName = Continent.GetContinentById(Continent_Id)?.Name ?? "غير معروف";

//            return View("ManageCountries");
//        }

//        // ================== تسجيل الدخول ==================
//        public IActionResult LoginCountryAdmin() => View();

//        [HttpPost]
//        public async Task<IActionResult> CheckLogin(string Country_Admin_Username, string Country_Admin_Password)
//        {
//            var c = Country.GetCountryByUsernameAndPassword(Country_Admin_Username, Country_Admin_Password);
//            if (c != null)
//            {
//                var claims = new List<Claim>
//                {
//                    new Claim(ClaimTypes.Name, c.Country_Id.ToString()),
//                    new Claim(ClaimTypes.Role,"CountryAdmin")
//                };

//                var identity = new ClaimsIdentity(claims, "SecureLogin");
//                var principal = new ClaimsPrincipal(identity);

//                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
//                    principal,
//                    new AuthenticationProperties
//                    {
//                        ExpiresUtc = System.DateTime.UtcNow.AddMinutes(30),
//                        IsPersistent = false
//                    });

//                return RedirectToAction("ManageCountries");
//            }

//            ViewBag.msg = "خطأ في اسم المستخدم أو كلمة المرور، الرجاء المحاولة مجدداً";
//            return View("LoginCountryAdmin");
//        }

//        // ================== رفع الملفات ==================
//        private async Task<string> UploadFile(IFormFile f1, string folder)
//        {
//            var baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);
//            if (!Directory.Exists(baseFolder)) Directory.CreateDirectory(baseFolder);

//            var newFileName = $"{System.DateTime.Now.Ticks}_{f1.FileName}";
//            var filePath = Path.Combine(baseFolder, newFileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await f1.CopyToAsync(stream);
//            }

//            return newFileName;
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using WebApplication4.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace WebApplication4.Controllers
{
    public class CountryController : Controller
    {
        // ================== الصفحة الرئيسية ==================
        public IActionResult Index()
        {
            var countries = Country.GetAllCountries() ?? new List<Country>();
            var continents = Continent.GetAllContinents() ?? new List<Continent>();

            ViewBag.Countries = countries;
            ViewBag.Continents = continents;
            ViewBag.Total = countries.Count;

            return View("ShowCountries");
        }

        // ================== إضافة دولة جديدة ==================
        public IActionResult AddNewCountryView()
        {
            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
            return View("AddNewCountryView", new Country());
        }

        [HttpPost]
        public async Task<IActionResult> DoAddCountry(Country c, IFormFile Country_Flag_File)
        {
            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();

            if (!ModelState.IsValid)
            {
                return View("AddNewCountryView", c);
            }

            // رفع صورة العلم إذا تم اختيارها
            if (Country_Flag_File != null && Country_Flag_File.Length > 0)
            {
                var fileName = await UploadFile(Country_Flag_File, "Flags");
                c.Country_Flag = "/Flags/" + fileName;
            }

            // إضافة الدولة إلى قاعدة البيانات
            int newId = Country.AddNewCountry(c);
            TempData["Success"] = newId > 0 ? $"تم إضافة الدولة {c.Country_Name} بنجاح" : "حدث خطأ أثناء إضافة الدولة";

            // الانتقال مباشرة إلى صفحة ShowCountries بعد الإضافة
            return RedirectToAction("Index");
        }

        // ================== تعديل الدولة ==================
        public IActionResult EditCountry(int Country_Id)
        {
            var c = Country.GetCountryById(Country_Id);
            if (c == null) return NotFound();

            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
            return View("EditCountryView", c);
        }

        [HttpPost]
        public async Task<IActionResult> DoUpdateCountry(Country c, IFormFile Country_Flag_File)
        {
            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();

            if (!ModelState.IsValid)
            {
                return View("EditCountryView", c);
            }

            if (Country_Flag_File != null && Country_Flag_File.Length > 0)
            {
                var fileName = await UploadFile(Country_Flag_File, "Flags");
                c.Country_Flag = "/Flags/" + fileName;
            }

            int updated = Country.UpdateCountry(c);
            TempData["Success"] = updated > 0 ? $"تم تعديل الدولة {c.Country_Name} بنجاح" : "حدث خطأ أثناء تعديل الدولة";

            return RedirectToAction("Index");
        }

        // ================== حذف الدولة ==================
        public IActionResult DeleteCountry(int Country_Id)
        {
            int deleted = Country.DeleteCountryById(Country_Id);
            TempData["Success"] = deleted > 0 ? "تم حذف الدولة بنجاح" : "حدث خطأ أثناء الحذف";

            return RedirectToAction("Index");
        }

        // ================== عرض تفاصيل الدولة ==================
        public IActionResult CountryDetails(int Country_Id)
        {
            var c = Country.GetCountryById(Country_Id);
            if (c == null) return NotFound();

            return View("CountryDetailsView", c);
        }

        // ================== البحث ==================
        public IActionResult SmartSearch(string keyword)
        {
            var countries = string.IsNullOrEmpty(keyword)
                ? Country.GetAllCountries() ?? new List<Country>()
                : Country.SearchByNameOrCapital(keyword) ?? new List<Country>();

            ViewBag.Countries = countries;
            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
            ViewBag.Keyword = keyword;

            return View("ShowCountries");
        }

        // ================== الإحصائيات ==================
        public IActionResult CountryStatistics()
        {
            var countries = Country.GetAllCountries() ?? new List<Country>();
            var continents = Continent.GetAllContinents() ?? new List<Continent>();

            ViewBag.TotalCountries = countries.Count;
            ViewBag.TotalPopulation = countries.Sum(c => c.Country_Population);
            ViewBag.AverageArea = countries.Count > 0 ? countries.Average(c => c.Country_Area) : 0;

            ViewBag.CountriesPerContinent = continents.Select(cont => new
            {
                ContinentName = cont.Name,
                CountryCount = countries.Count(c => c.Continent_Id == cont.Id)
            }).ToList();

            return View();
        }

        // ================== تصدير CSV ==================
        public IActionResult ExportCountriesToCSV()
        {
            var countries = Country.GetAllCountries() ?? new List<Country>();
            var csv = "Country_Name,Country_Capital,Country_Population,Country_Area,Continent\n";

            foreach (var c in countries)
            {
                var continent = Continent.GetContinentById(c.Continent_Id)?.Name ?? "";
                csv += $"{c.Country_Name},{c.Country_Capital},{c.Country_Population},{c.Country_Area},{continent}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "Countries.csv");
        }

        // ================== عرض الدول حسب القارة ==================
        public IActionResult CountriesByContinent(int Continent_Id)
        {
            var countries = Country.GetCountriesByContinentId(Continent_Id) ?? new List<Country>();
            ViewBag.Countries = countries;
            ViewBag.Continents = Continent.GetAllContinents() ?? new List<Continent>();
            ViewBag.ContinentName = Continent.GetContinentById(Continent_Id)?.Name ?? "غير معروف";

            return View("ShowCountries");
        }

        // ================== تسجيل الدخول ==================
        public IActionResult LoginCountryAdmin() => View();

        [HttpPost]
        public async Task<IActionResult> CheckLogin(string Country_Admin_Username, string Country_Admin_Password)
        {
            var c = Country.GetCountryByUsernameAndPassword(Country_Admin_Username, Country_Admin_Password);
            if (c != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, c.Country_Id.ToString()),
                    new Claim(ClaimTypes.Role,"CountryAdmin")
                };

                var identity = new ClaimsIdentity(claims, "SecureLogin");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = System.DateTime.UtcNow.AddMinutes(30),
                        IsPersistent = false
                    });

                return RedirectToAction("Index");
            }

            ViewBag.msg = "خطأ في اسم المستخدم أو كلمة المرور، الرجاء المحاولة مجدداً";
            return View("LoginCountryAdmin");
        }

        // ================== رفع الملفات ==================
        private async Task<string> UploadFile(IFormFile f1, string folder)
        {
            var baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);
            if (!Directory.Exists(baseFolder)) Directory.CreateDirectory(baseFolder);

            var newFileName = $"{System.DateTime.Now.Ticks}_{f1.FileName}";
            var filePath = Path.Combine(baseFolder, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await f1.CopyToAsync(stream);
            }

            return newFileName;
        }
    }
}


