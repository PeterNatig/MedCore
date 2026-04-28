using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedCore.Model;
using Service;
using ViewModel;

namespace MedCoreWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(IPatientService patient, IDoctorService doctor,
            SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _patientService = patient;
            _doctorService = doctor;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        private IActionResult RedirectAuthenticatedUser()
        {
            if (User.IsInRole("Admin")) return RedirectToAction("Index", "AdminPortal");
            if (User.IsInRole("Doctor")) return RedirectToAction("Index", "DoctorPortal");
            return RedirectToAction("Index", "PatientPortal");
        }

        public IActionResult PatientRegister()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectAuthenticatedUser();
            return View();
        }

        public async Task<IActionResult> DoctorRegister()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectAuthenticatedUser();
            ViewBag.Specialties = await _doctorService.GetSpecialtySelectListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientRegister(PatientRegisterVM pat)
        {
            if (ModelState.IsValid)
            {
                var result = await _patientService.Add(pat);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(pat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorRegister(DoctorRegisterVM doc)
        {
            if (ModelState.IsValid)
            {
                var result = await _doctorService.Add(doc);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login), new { role = "doctor" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.Specialties = await _doctorService.GetSpecialtySelectListAsync();
            return View(doc);
        }

        public IActionResult DoctorLogin()
        {
            return RedirectToAction(nameof(Login), new { role = "doctor" });
        }

        public IActionResult PatientLogin()
        {
            return RedirectToAction(nameof(Login), new { role = "patient" });
        }

        public IActionResult Login(string role = "patient")
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectAuthenticatedUser();
            ViewData["LoginRole"] = role.Equals("doctor", StringComparison.OrdinalIgnoreCase) ? "doctor" : "patient";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string role = "patient")
        {
            role = role.Equals("doctor", StringComparison.OrdinalIgnoreCase) ? "doctor" : "patient";
            ViewData["LoginRole"] = role;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                ModelState.AddModelError("", "Email or Password not correct");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = roles.Contains("Admin");
            var isDoctor = roles.Contains("Doctor");
            var isPatient = roles.Contains("Patient");

            // Validate role matches the login tab
            if (role == "doctor" && !isDoctor && !isAdmin)
            {
                ModelState.AddModelError("", "This account is not registered as a doctor.");
                return View(model);
            }
            if (role == "patient" && !isPatient && !isAdmin)
            {
                ModelState.AddModelError("", "This account is not registered as a patient.");
                return View(model);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password not correct");
                return View(model);
            }

            // Redirect based on role
            if (isAdmin) return RedirectToAction("Index", "AdminPortal");
            if (isDoctor) return RedirectToAction("Index", "DoctorPortal");
            return RedirectToAction("Index", "PatientPortal");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
