using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedCore.Model;
using Service.Workflow;
using ViewModel.Doctor;

namespace MedCoreWeb.Controllers;

[Authorize(Roles = "Doctor,Admin")]
public class DoctorPortalController : Controller
{
    private readonly IDoctorWorkflowService _service;
    private readonly UserManager<User> _userManager;

    public DoctorPortalController(IDoctorWorkflowService service, UserManager<User> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    private async Task<string?> ResolveDoctorEmailAsync(string? doctorEmail)
    {
        if (User.IsInRole("Admin"))
        {
            return string.IsNullOrWhiteSpace(doctorEmail) ? null : doctorEmail;
        }
        var user = await _userManager.GetUserAsync(User);
        return user?.Email;
    }

    private IActionResult HandleMissingDoctorContext(string messageForDoctor)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "AdminPortal");
        }

        TempData["FlowMessage"] = messageForDoctor;
        return RedirectToAction("Login", "Account", new { role = "doctor" });
    }

    public async Task<IActionResult> Index(string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.DoctorEmail = effectiveDoctorEmail;
        var model = await _service.GetDashboardAsync(effectiveDoctorEmail);
        if (model is null)
        {
            return HandleMissingDoctorContext("We could not load your dashboard right now.");
        }
        return View(model);
    }

    public async Task<IActionResult> Schedules(string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.DoctorEmail = effectiveDoctorEmail;
        var model = await _service.GetSchedulesAsync(effectiveDoctorEmail);
        if (model is null)
        {
            return HandleMissingDoctorContext("We could not load schedules right now.");
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSchedule(DoctorScheduleVM model, string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        if (!ModelState.IsValid)
        {
            TempData["FlowMessage"] = "Please provide a valid date and time.";
            return RedirectToAction(nameof(Schedules), new { doctorEmail = effectiveDoctorEmail });
        }

        // Each slot is fixed at 30 minutes
        var endTime = model.StartTime.AddMinutes(30);

        var success = await _service.AddScheduleAsync(effectiveDoctorEmail, model.StartTime, endTime);
        TempData["FlowMessage"] = success
            ? "Schedule slot added (30 min)."
            : "Cannot add slot: must be in the future and must not overlap existing slots.";
        return RedirectToAction(nameof(Schedules), new { doctorEmail = effectiveDoctorEmail });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSchedule(string scheduleId, string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        var success = await _service.DeleteScheduleAsync(effectiveDoctorEmail, scheduleId);
        TempData["FlowMessage"] = success ? "Schedule deleted." : "Booked slots cannot be deleted.";
        return RedirectToAction(nameof(Schedules), new { doctorEmail = effectiveDoctorEmail });
    }

    public async Task<IActionResult> Patients(string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.DoctorEmail = effectiveDoctorEmail;
        var model = await _service.GetPatientsAsync(effectiveDoctorEmail);
        if (model is null)
        {
            return HandleMissingDoctorContext("We could not load patient records right now.");
        }
        return View(model);
    }

    public async Task<IActionResult> ConsultationRoom(string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.DoctorEmail = effectiveDoctorEmail;
        var model = await _service.GetConsultationAsync(effectiveDoctorEmail);
        if (model is null)
        {
            return HandleMissingDoctorContext("We could not load the consultation room right now.");
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPrescription(ConsultationVM model, string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        if (!ModelState.IsValid)
        {
            TempData["FlowMessage"] = "Please fill all prescription fields.";
            return RedirectToAction(nameof(ConsultationRoom), new { doctorEmail = effectiveDoctorEmail });
        }

        var success = await _service.AddPrescriptionAsync(effectiveDoctorEmail, model);
        TempData["FlowMessage"] = success ? "Prescription added and appointment completed." : "Cannot add prescription for this appointment.";
        return RedirectToAction(nameof(ConsultationRoom), new { doctorEmail = effectiveDoctorEmail });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmAppointment(string appointmentId, string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        var success = await _service.ConfirmAppointmentAsync(effectiveDoctorEmail, appointmentId);
        TempData["FlowMessage"] = success ? "Appointment confirmed." : "Could not confirm this appointment.";
        return RedirectToAction(nameof(Patients), new { doctorEmail = effectiveDoctorEmail });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectAppointment(string appointmentId, string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        var success = await _service.RejectAppointmentAsync(effectiveDoctorEmail, appointmentId);
        TempData["FlowMessage"] = success ? "Appointment rejected." : "Could not reject this appointment.";
        return RedirectToAction(nameof(Patients), new { doctorEmail = effectiveDoctorEmail });
    }

    public async Task<IActionResult> Chat(string? doctorEmail)
    {
        var effectiveDoctorEmail = await ResolveDoctorEmailAsync(doctorEmail);
        if (string.IsNullOrWhiteSpace(effectiveDoctorEmail))
        {
            TempData["FlowMessage"] = "Please choose a doctor account from admin dashboard.";
            return RedirectToAction("Index", "AdminPortal");
        }

        ViewBag.DoctorEmail = effectiveDoctorEmail;
        return View();
    }
}
