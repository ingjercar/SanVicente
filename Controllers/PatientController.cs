using HospitalSanVicente.Data;
using HospitalSanVicente.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalSanVicente.Controllers;

public class PatientController : Controller
{
    
    public readonly AppDbContext _context;
    
    public PatientController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        
        var passengers = await _context.Patients.ToListAsync();
        if (passengers == null) return NotFound();
        
        return View(passengers);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Patient patient)
    {
        bool docExists = await _context.Patients.AnyAsync(p => p.Document == patient.Document);
        if (docExists)
        {
            ModelState.AddModelError("", "Ya existe un paciente con este documento.");
            var patients = await _context.Patients.ToListAsync();
            return View("Index", patients); // vuelve al Index con el error
        }

        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int Id)
    {
        var patient = await _context.Patients.FindAsync(Id);
        if (patient == null)
        {
            return NotFound();
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var patient = await _context.Patients
            .FirstOrDefaultAsync(m => m.Id == id);

        if (patient == null)
            return NotFound();

        return View(patient);
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
            return NotFound();

        return View(patient);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Age,Document,Phone,Email")] Patient patient)
    {
        if (id != patient.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(patient);

        try
        {
            _context.Update(patient);
            await _context.SaveChangesAsync();
            ViewData["Message"] = "Información actualizada correctamente";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PatientExists(patient.Id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Details), new { id = patient.Id });
    }

    private bool PatientExists(int id)
    {
        return _context.Patients.Any(e => e.Id == id);
    }
    
}
