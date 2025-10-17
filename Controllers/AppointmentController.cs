using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalSanVicente.Data;
using HospitalSanVicente.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace HospitalSanVicente.Controllers
{
    
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        private readonly EmailSettings _emailSettings;

        public AppointmentController(AppDbContext context, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _emailSettings = emailSettings.Value;
        }

        // GET: /Appointment
        public async Task<IActionResult> Index(string specialty)
        {
            
            // Construir consulta
            var query = _context.Appointments
                .Include(a => a.doctor)
                .Include(a => a.patient)
                .AsQueryable();
            

            var appointments = await query.ToListAsync();
            return View(appointments);
        }
        
        public async Task<IActionResult> Create()
        {
            ViewBag.Doctors = await _context.Doctors.ToListAsync();
            ViewBag.Patients = await _context.Patients.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);
                var patient = await _context.Patients.FindAsync(appointment.PatientId);

                if (doctor == null || patient == null)
                {
                    ModelState.AddModelError("", "Debe seleccionar un doctor y un paciente válidos.");
                    ViewBag.Doctors = await _context.Doctors.ToListAsync();
                    ViewBag.Patients = await _context.Patients.ToListAsync();
                    return View(appointment);
                }

                bool doctorBusy = await _context.Appointments
                    .AnyAsync(a => a.DoctorId == doctor.Id && a.Date == appointment.Date && a.State != "Cancelado");

                bool patientBusy = await _context.Appointments
                    .AnyAsync(a => a.PatientId == patient.Id && a.Date == appointment.Date && a.State != "Cancelado");

                if (doctorBusy || patientBusy)
                {
                    ModelState.AddModelError("", "El doctor o el paciente ya tienen una cita en esta hora.");
                    ViewBag.Doctors = await _context.Doctors.ToListAsync();
                    ViewBag.Patients = await _context.Patients.ToListAsync();
                    return View(appointment);
                }

                appointment.DoctorName = doctor.Name;
                appointment.PatientName = patient.Name;
                appointment.Specialty = doctor.Specialty;
                appointment.State = "Programada";
                // 
                Console.WriteLine("intentr enviar codigo ");
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

// 📨 Enviar correo al paciente
                var subject = "Confirmación de cita - Hospital San Vicente";
                var body = $@"
                <h3>Hola {patient.Name},</h3>
                <p>Tu cita ha sido programada con éxito.</p>
                <ul>
                    <li><b>Doctor:</b> {doctor.Name}</li>
                    <li><b>Especialidad:</b> {doctor.Specialty}</li>
                    <li><b>Fecha:</b> {appointment.Date:g}</li>
                    <li><b>Estado:</b> {appointment.State}</li>
                </ul>
                <p>Por favor llega 10 minutos antes de la hora programada.</p>
                <p>Saludos,<br><b>Hospital San Vicente</b></p>
                ";

                SendEmail(patient.Email, subject, body);
                Console.WriteLine($"el correo para enviar es {patient.Email}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            
            if (appointment == null) return NotFound();

            appointment.State = "Cancelado";
            
            _context.Update(appointment);
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using var smtp = new SmtpClient(_emailSettings.Host)
                {
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password),
                    EnableSsl = _emailSettings.EnableSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_emailSettings.UserName, _emailSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                smtp.Send(mail);

                Console.WriteLine($"Correo enviado exitosamente a {toEmail}");
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"⚠Error SMTP al enviar correo a {toEmail}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Detalle: {ex.InnerException.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al enviar correo: {ex.Message}");
                return false;
            }
        }
    }
}
