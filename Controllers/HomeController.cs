using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Random rand = new Random();
        private const string DefaultPhoto = "Photo/default.jpg";

        // ===== الصفحة الرئيسية =====
        public IActionResult Index()
        {
            var continents = Continent.GetAllContinents();
            ViewBag.continents = continents;
            ViewBag.t = continents.Count;

            // عرض رسائل نجاح/خطأ بعد حذف أو تعديل
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];

            return View();
        }

        public IActionResult AddNewContinent()
        {
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoAddNewContinent(Continent c, IFormFile Continent_Photo_File)
        {
            if (Continent_Photo_File != null)
            {
                string fileName = await UploadFile(Continent_Photo_File, "Photo");
                c.PhotoUrl = "Photo/" + fileName;
            }

            int result = Continent.AddContinentToDB(c);
            if (result > 0)
                TempData["Success"] = $"تمت إضافة القارة {c.Name} بنجاح";
            else
                TempData["Error"] = "حدث خطأ أثناء إضافة القارة";

            return RedirectToAction("AddNewContinent");
        }

        // ===== حذف قارة مع عرض رسالة على نفس الصفحة =====
        [HttpPost]
        public IActionResult DeleteContinent(int id)
        {
            bool success = Continent.DeleteContinent(id);
            if (success)
                TempData["Success"] = "تم حذف القارة بنجاح";
            else
                TempData["Error"] = "فشل حذف القارة";

            return RedirectToAction("Index");
        }

        // ===== عرض نموذج تعديل قارة =====
        public IActionResult EditContinent(int Continent_Id)
        {
            Continent c = Continent.GetContinentById(Continent_Id);
            if (c == null)
            {
                TempData["Error"] = "القارة غير موجودة";
                return RedirectToAction("Index");
            }
            return View(c); // ستستخدم EditContinent.cshtml
        }

        // ===== حفظ التعديلات على القارة =====
        [HttpPost]
        public async Task<IActionResult> DoUpdateContinent(Continent c, IFormFile Continent_Photo_File)
        {
            if (Continent_Photo_File != null)
            {
                string fileName = await UploadFile(Continent_Photo_File, "Photo");
                c.PhotoUrl = "Photo/" + fileName;
            }

            int result = Continent.UpdateContinentInDB(c);
            if (result > 0)
                TempData["Success"] = $"تم تعديل القارة {c.Name} بنجاح";
            else
                TempData["Error"] = "حدث خطأ أثناء تعديل القارة";

            return RedirectToAction("Index");
        }

        // ===== رفع الملفات =====
        private async Task<string> UploadFile(IFormFile file, string folder)
        {
            var baseFolder = "wwwroot";
            var newFileName = DateTime.Now.Ticks.ToString() + "_" + file.FileName;
            var filePath = Path.Combine(baseFolder, folder, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return newFileName;
        }

        // ===== ✅ عرض صفحة Login من مجلد Country =====
        public IActionResult Login()
        {
            // يمكنك إضافة أي منطق قبل العرض، مثل ViewBag أو TempData
            return View("~/Views/Home/Login.cshtml");
        }
    }
}
