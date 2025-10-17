using HospitalSanVicente.Data;
using HospitalSanVicente.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalSanVicente.Controllers;

public class DoctorController : Controller
{

    private readonly AppDbContext _context;
    
    public DoctorController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var doctors = await _context.Doctors.ToListAsync();
        if (doctors == null) return NotFound();
        
        return View(doctors);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Doctor doctor)
    {
        await _context.Doctors.AddAsync(doctor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int Id)
    {
        var doctor = await _context.Doctors.FindAsync(Id);
        if (doctor == null)
        {
            return NotFound();
        }

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(m => m.Id == id);

        if (doctor == null)
            return NotFound();
        
        var doctors = await _context.Doctors.ToListAsync();
        
        if (doctors == null) return NotFound();
        
        return View(doctors);

        return View(doctor);
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor == null)
            return NotFound();

        return View(doctor);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Document, Specialty, Phone, Email")] Doctor doctor)
    {
        if (id != doctor.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(doctor);

        try
        {
            _context.Update(doctor);
            await _context.SaveChangesAsync();
            ViewData["Message"] = "Information updated correctly";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DoctorExists(doctor.Id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Details), new { id = doctor.Id });
    }

    private bool DoctorExists(int id)
    {
        return _context.Doctors.Any(e => e.Id == id);
    }
    
}