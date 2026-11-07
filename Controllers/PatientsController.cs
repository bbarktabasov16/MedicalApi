using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    public static List<Patient> Patients = new List<Patient>
    {
        new Patient { Id = 1, Name = "Алиса Иванова", BirthDate = new DateTime(2000, 5, 15), Phone = "+996700111222", Address = "Бишкек", Apartment = 12, HasInsurance = true },
        new Patient { Id = 2, Name = "Тимур Асанов", BirthDate = new DateTime(1995, 3, 10), Phone = "+996700333444", Address = "Бишкек", Apartment = 8 },
        new Patient { Id = 3, Name = "Мария Петрова", BirthDate = new DateTime(2012, 8, 20), Phone = "+996700555666", Address = "Бишкек", Apartment = 5, HasInsurance = true }
    };

    // Счётчик не уменьшается после удаления: Id не повторяются до перезапуска.
    private static int _nextId = 3;

    [HttpGet]
    [ProducesResponseType(typeof(List<Patient>), StatusCodes.Status200OK)]
    public ActionResult<List<Patient>> GetAll()
    {
        return Ok(Patients);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Patient> GetById(int id)
    {
        Patient? item = Patients.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<Patient> Create(Patient item)
    {
        string? error = Validate(item);
        if (error != null) return BadRequest(error);
        item.Id = Interlocked.Increment(ref _nextId);
        Patients.Add(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<Patient> Update(int id, Patient item)
    {
        Patient? existing = Patients.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        string? error = Validate(item);
        if (error != null) return BadRequest(error);
        existing.Name = item.Name;
        existing.BirthDate = item.BirthDate;
        existing.Phone = item.Phone;
        existing.Address = item.Address;
        existing.Apartment = item.Apartment;
        existing.HasInsurance = item.HasInsurance;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult Delete(int id)
    {
        Patient? item = Patients.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        if (SpravkiController.Spravki.Any(s => s.PatientId == id))
            return BadRequest("Сначала удалите справки пациента, затем самого пациента");
        Patients.Remove(item);
        return NoContent();
    }

    [HttpGet("{id}/spravki")]
    [ProducesResponseType(typeof(List<Spravka>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<List<Spravka>> GetSpravki(int id)
    {
        if (!Patients.Any(p => p.Id == id)) return NotFound();
        return Ok(SpravkiController.Spravki.Where(s => s.PatientId == id).ToList());
    }

    private static string? Validate(Patient item)
    {
        if (string.IsNullOrWhiteSpace(item.Name)) return "Укажите имя пациента";
        if (item.Name.Trim().Length < 2) return "Имя пациента должно содержать минимум 2 символа";
        if (item.BirthDate == default || item.BirthDate.Date > DateTime.Today)
            return "Укажите дату рождения, которая не находится в будущем";
        if (item.Apartment < 0) return "Номер квартиры не может быть отрицательным";
        return null;
    }
}
