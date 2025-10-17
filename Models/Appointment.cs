using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalSanVicente.Models;

public class Appointment
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    [ForeignKey("PatientId")]
    public Patient patient { get; set; }
    
    public int DoctorId { get; set; }
    [ForeignKey("DoctorId")]
    public Doctor doctor { get; set; }
    
    public string PatientName { get; set; }
    
    public string DoctorName { get; set; }
    
    public string Specialty { get; set; }
    
    public DateTime Date { get; set; }
    
    public string State { get; set; }
    
}