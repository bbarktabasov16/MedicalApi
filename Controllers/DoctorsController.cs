using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    public static List<Doctor> Doctors = new List<Doctor>
    {
        new Doctor { Id = 1, Name = "Анна Соколова", Specialty = "Терапевт", RoomNumber = 101, IsAvailable = true },
        new Doctor { Id = 2, Name = "Азамат Алиев", Specialty = "Педиатр", RoomNumber = 102, IsAvailable = true }
    };

    // Счётчик не уменьшается после удаления: Id не повторяются до перезапуска.
    private static int _nextId = 2;

    [HttpGet]
    [ProducesResponseType(typeof(List<Doctor>), StatusCodes.Status200OK)]
    public ActionResult<List<Doctor>> GetAll()
    {
        return Ok(Doctors);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Doctor), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Doctor> GetById(int id)
    {
        Doctor? item = Doctors.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Doctor), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<Doctor> Create(Doctor item)
    {
        string? error = Validate(item);
        if (error != null) return BadRequest(error);
        item.Id = Interlocked.Increment(ref _nextId);
        Doctors.Add(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Doctor), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<Doctor> Update(int id, Doctor item)
    {
        Doctor? existing = Doctors.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        string? error = Validate(item);
        if (error != null) return BadRequest(error);
        existing.Name = item.Name;
        existing.Specialty = item.Specialty;
        existing.RoomNumber = item.RoomNumber;
        existing.IsAvailable = item.IsAvailable;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult Delete(int id)
    {
        Doctor? item = Doctors.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        
        Doctors.Remove(item);
        return NoContent();
    }

    private static string? Validate(Doctor item)
    {
        if (string.IsNullOrWhiteSpace(item.Name)) return "Укажите имя врача";
        if (string.IsNullOrWhiteSpace(item.Specialty)) return "Укажите специальность врача";
        if (item.RoomNumber <= 0) return "Номер кабинета должен быть больше 0";
        return null;
    }
}
