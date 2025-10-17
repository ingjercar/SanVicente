namespace HospitalSanVicente.Models;
//nombre, documento, especialidad, teléfono,
// correo)
public class Doctor
{
    public int Id {get; set;}
    public string Name { get; set; }
    public string Document { get; set; }
    public string Specialty { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }


    public new List<Appointment> Appoinments { get; set; } = new List<Appointment>();



}