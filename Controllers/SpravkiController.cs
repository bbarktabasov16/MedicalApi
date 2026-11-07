using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    public static List<Spravka> Spravki = new List<Spravka>
    {
        new Spravka { Id = 1, PatientId = 1, SpravkaTypeId = 1, Name = "Справка 083", Price = 100 },
        new Spravka { Id = 2, PatientId = 1, SpravkaTypeId = 3, Name = "Справка для занятий танцами", Price = 400 },
        new Spravka { Id = 3, PatientId = 2, SpravkaTypeId = 2, Name = "Санитарная книжка", Price = 750 }
    };

    // Счётчик не уменьшается после удаления: Id не повторяются до перезапуска.
    private static int _nextId = 3;

    [HttpGet]
    [ProducesResponseType(typeof(List<Spravka>), StatusCodes.Status200OK)]
    public ActionResult<List<Spravka>> GetAll()
    {
        return Ok(Spravki);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Spravka), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Spravka> GetById(int id)
    {
        Spravka? item = Spravki.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Spravka), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<Spravka> Create(Spravka item)
    {
        string? error = Validate(item);
        if (error != null) return BadRequest(error);
        item.Id = Interlocked.Increment(ref _nextId);
        Spravki.Add(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Spravka), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<Spravka> Update(int id, Spravka item)
    {
        Spravka? existing = Spravki.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        string? error = Validate(item);
        if (error != null) return BadRequest(error);
        existing.Name = item.Name;
        existing.Price = item.Price;
        existing.PatientId = item.PatientId;
        existing.SpravkaTypeId = item.SpravkaTypeId;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult Delete(int id)
    {
        Spravka? item = Spravki.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        
        Spravki.Remove(item);
        return NoContent();
    }

    [HttpGet("statistics")]
    [ProducesResponseType(typeof(SpravkaStatistics), StatusCodes.Status200OK)]
    public ActionResult<SpravkaStatistics> GetStatistics()
    {
        return Ok(new SpravkaStatistics
        {
            Count = Spravki.Count,
            TotalAmount = Spravki.Sum(s => s.Price)
        });
    }

    private static string? Validate(Spravka item)
    {
        if (PatientsController.Patients.FirstOrDefault(p => p.Id == item.PatientId) == null)
            return "Выберите существующего пациента: проверьте PatientId";
        SpravkaType? type = SpravkaTypesController.Types.FirstOrDefault(t => t.Id == item.SpravkaTypeId);
        if (type == null) return "Выберите существующий вид справки: проверьте SpravkaTypeId";
        item.Name = type.Name;
        item.Price = type.Price;
        return null;
    }
}
