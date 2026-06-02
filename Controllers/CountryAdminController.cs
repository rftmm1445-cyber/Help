using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.Models;
using System.Threading.Tasks;

namespace WebApplication4.Controllers
{
    public class CountryAdminController : Controller
    {
        [Authorize(Roles = "CountryAdmin")]
        public IActionResult Index()
        {
            // جلب Country_Id من User.Identity.Name
            int Country_Id = int.Parse(User.Identity.Name);
            Country c = Country.GetCountryById(Country_Id);
            ViewBag.Country = c;
            return View();
        }

        public async Task<IActionResult> LogoutView()
        {
            await HttpContext.SignOutAsync();
            return View();
        }
    }
}
