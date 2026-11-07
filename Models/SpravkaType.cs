namespace MedicalApi.Models;

public class SpravkaType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int ValidDays { get; set; } = 30;
    public bool RequiresExamination { get; set; }
    public bool ForChildren { get; set; }
}
