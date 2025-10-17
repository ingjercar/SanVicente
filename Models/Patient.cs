namespace HospitalSanVicente.Models;
//nombre, documento, edad, teléfono,
// correo)
public class Patient
{
    public int Id { get; set; }
    public string Name {get; set;}
    public int Age {get; set;}
    public string Document {get; set;}
    public string Phone {get; set;}
    public string Email {get; set;}
    
    public new List<Appointment> Appoinments { get; set; } = new List<Appointment>();

}