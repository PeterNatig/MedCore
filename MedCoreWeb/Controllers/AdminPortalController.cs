using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Workflow;
using ViewModel.Admin;

namespace MedCoreWeb.Controllers;

[Authorize(Roles = "Admin")]
public class AdminPortalController : Controller
{
    private readonly IAdminWorkflowService _service;

    public AdminPortalController(IAdminWorkflowService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _service.GetDashboardAsync());
    }

    public async Task<IActionResult> Specialties()
    {
        return View(await _service.GetSpecialtiesAsync());
    }

    public async Task<IActionResult> Medications()
    {
        return View(await _service.GetMedicationsAsync());
    }

    public IActionResult Accounts()
    {
        return View();
    }

    public async Task<IActionResult> OperateDoctor()
    {
        return View(await _service.GetUserPickersAsync());
    }

    public async Task<IActionResult> OperatePatient()
    {
        return View(await _service.GetUserPickersAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSpecialty(SpecialtyManagementVM model)
    {
        if (!ModelState.IsValid)
        {
            TempData["FlowMessage"] = "Name and description are required.";
            return RedirectToAction(nameof(Specialties));
        }

        await _service.AddSpecialtyAsync(model.Name, model.Description);
        TempData["FlowMessage"] = "Specialty added successfully.";
        return RedirectToAction(nameof(Specialties));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSpecialty(string specialtyId)
    {
        var success = await _service.DeleteSpecialtyAsync(specialtyId);
        TempData["FlowMessage"] = success ? "Specialty deleted successfully." : "Specialty not found.";
        return RedirectToAction(nameof(Specialties));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMedication(MedicationManagementVM model)
    {
        if (!ModelState.IsValid)
        {
            TempData["FlowMessage"] = "Brand name and generic name are required.";
            return RedirectToAction(nameof(Medications));
        }

        await _service.AddMedicationAsync(model.Name, model.GenericName);
        TempData["FlowMessage"] = "Medication added successfully.";
        return RedirectToAction(nameof(Medications));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMedication(string medicationId)
    {
        var success = await _service.DeleteMedicationAsync(medicationId);
        TempData["FlowMessage"] = success ? "Medication deleted successfully." : "Medication not found.";
        return RedirectToAction(nameof(Medications));
    }
}
