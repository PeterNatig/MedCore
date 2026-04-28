using MedCoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MedCoreWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirect authenticated users to their appropriate portal
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin")) return RedirectToAction("Index", "AdminPortal");
                if (User.IsInRole("Doctor")) return RedirectToAction("Index", "DoctorPortal");
                if (User.IsInRole("Patient")) return RedirectToAction("Index", "PatientPortal");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
