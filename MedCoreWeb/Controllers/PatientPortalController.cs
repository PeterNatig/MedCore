using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedCore.Model;
using Service.Workflow;
using ViewModel.Patient;

namespace MedCoreWeb.Controllers;

[Authorize(Roles = "Patient,Admin")]
public class PatientPortalController : Controller
{
    private readonly IPatientWorkflowService _service;
    private readonly UserManager<User> _userManager;

    public PatientPortalController(IPatientWorkflowService service, UserManager<User> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    private async Task<string?> ResolvePatientEmailAsync(string? patientEmail)
    {
        if (User.IsInRole("Admin"))
        {
            return string.IsNullOrWhiteSpace(patientEmail) ? null : patientEmail;
        }
        var user = await _userManager.GetUserAsync(User);
        return user?.Email;
    }

    public async Task<IActionResult> Index(string? patientEmail)
    {
        var effectivePatientEmail = await ResolvePatientEmailAsync(patientEmail);
        if (string.IsNullOrWhiteSpace(effectivePatientEmail))
        {
            TempData["FlowMessage"] = "Please choose a patient account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.PatientEmail = effectivePatientEmail;
        return View(await _service.GetSpecialtiesAsync());
    }

    public async Task<IActionResult> Doctors(string specialtyId, string? patientEmail)
    {
        ViewBag.PatientEmail = await ResolvePatientEmailAsync(patientEmail);
        return View(await _service.GetDoctorsBySpecialtyAsync(specialtyId));
    }

    public async Task<IActionResult> Book(string doctorId, string? patientEmail)
    {
        var effectivePatientEmail = await ResolvePatientEmailAsync(patientEmail);
        if (string.IsNullOrWhiteSpace(effectivePatientEmail))
        {
            TempData["FlowMessage"] = "Please choose a patient account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.PatientEmail = effectivePatientEmail;
        var model = await _service.GetBookingModelAsync(doctorId, effectivePatientEmail);
        if (model is null)
        {
            TempData["FlowMessage"] = "Doctor availability could not be loaded.";
            return RedirectToAction(nameof(Index), new { patientEmail = effectivePatientEmail });
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookAppointmentVM model, string? patientEmail)
    {
        var effectivePatientEmail = await ResolvePatientEmailAsync(patientEmail);
        if (string.IsNullOrWhiteSpace(effectivePatientEmail))
        {
            TempData["FlowMessage"] = "Please choose a patient account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        if (!ModelState.IsValid)
        {
            var fullModel = await _service.GetBookingModelAsync(model.DoctorId, effectivePatientEmail);
            if (fullModel is null)
            {
                TempData["FlowMessage"] = "Doctor availability could not be loaded.";
                return RedirectToAction(nameof(Index), new { patientEmail = effectivePatientEmail });
            }
            fullModel.SelectedScheduleId = model.SelectedScheduleId;
            ViewBag.PatientEmail = effectivePatientEmail;
            return View(fullModel);
        }

        var result = await _service.BookAppointmentAsync(effectivePatientEmail, model.SelectedScheduleId);
        TempData["FlowMessage"] = result.Message;
        return RedirectToAction(nameof(MyRecords), new { patientEmail = effectivePatientEmail });
    }

    public async Task<IActionResult> MyRecords(string? patientEmail)
    {
        var effectivePatientEmail = await ResolvePatientEmailAsync(patientEmail);
        if (string.IsNullOrWhiteSpace(effectivePatientEmail))
        {
            TempData["FlowMessage"] = "Please choose a patient account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.PatientEmail = effectivePatientEmail;
        var model = await _service.GetRecordsAsync(effectivePatientEmail);
        return View(model);
    }

    public async Task<IActionResult> Prescriptions(string? patientEmail)
    {
        var effectivePatientEmail = await ResolvePatientEmailAsync(patientEmail);
        if (string.IsNullOrWhiteSpace(effectivePatientEmail))
        {
            TempData["FlowMessage"] = "Please choose a patient account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.PatientEmail = effectivePatientEmail;
        var model = await _service.GetPrescriptionsAsync(effectivePatientEmail);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelAppointment(string appointmentId, string? patientEmail)
    {
        var effectivePatientEmail = await ResolvePatientEmailAsync(patientEmail);
        if (string.IsNullOrWhiteSpace(effectivePatientEmail))
        {
            TempData["FlowMessage"] = "Please choose a patient account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        var success = await _service.CancelAppointmentAsync(effectivePatientEmail, appointmentId);
        TempData["FlowMessage"] = success ? "Appointment cancelled successfully." : "This appointment cannot be cancelled.";
        return RedirectToAction(nameof(MyRecords), new { patientEmail = effectivePatientEmail });
    }

    public async Task<IActionResult> Chat(string? patientEmail)
    {
        var effectivePatientEmail = await ResolvePatientEmailAsync(patientEmail);
        if (string.IsNullOrWhiteSpace(effectivePatientEmail))
        {
            TempData["FlowMessage"] = "Please choose a patient account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.PatientEmail = effectivePatientEmail;
        return View();
    }
}
