namespace MedicalApi.Models;

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Specialty { get; set; } = "";
    public int RoomNumber { get; set; }
    public bool IsAvailable { get; set; } = true;
}
