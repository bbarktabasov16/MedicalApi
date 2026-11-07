using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    public static List<SpravkaType> Types = new List<SpravkaType>
    {
        new SpravkaType { Id = 1, Name = "Справка 083", Price = 100, ValidDays = 365, RequiresExamination = true, ForChildren = false },
        new SpravkaType { Id = 2, Name = "Санитарная книжка", Price = 750, ValidDays = 365, RequiresExamination = true, ForChildren = false },
        new SpravkaType { Id = 3, Name = "Справка для занятий танцами", Price = 400, ValidDays = 180, RequiresExamination = true, ForChildren = true },
        new SpravkaType { Id = 4, Name = "Справка для участия в школьной экскурсии", Price = 250, ValidDays = 30, RequiresExamination = true, ForChildren = true }
    };

    // Счётчик не уменьшается после удаления: Id не повторяются до перезапуска.
    private static int _nextId = 4;

    [HttpGet]
    [ProducesResponseType(typeof(List<SpravkaType>), StatusCodes.Status200OK)]
    public ActionResult<List<SpravkaType>> GetAll()
    {
        return Ok(Types);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpravkaType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<SpravkaType> GetById(int id)
    {
        SpravkaType? item = Types.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SpravkaType), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<SpravkaType> Create(SpravkaType item)
    {
        string? error = Validate(item);
        if (error != null) return BadRequest(error);
        item.Id = Interlocked.Increment(ref _nextId);
        Types.Add(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SpravkaType), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<SpravkaType> Update(int id, SpravkaType item)
    {
        SpravkaType? existing = Types.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        string? error = Validate(item);
        if (error != null) return BadRequest(error);
        existing.Name = item.Name;
        existing.Price = item.Price;
        existing.ValidDays = item.ValidDays;
        existing.RequiresExamination = item.RequiresExamination;
        existing.ForChildren = item.ForChildren;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult Delete(int id)
    {
        SpravkaType? item = Types.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        if (SpravkiController.Spravki.Any(s => s.SpravkaTypeId == id))
            return BadRequest("Нельзя удалить вид, по которому выданы справки. Сначала удалите эти справки");
        Types.Remove(item);
        return NoContent();
    }

    private static string? Validate(SpravkaType item)
    {
        if (string.IsNullOrWhiteSpace(item.Name)) return "Укажите название вида справки";
        if (item.Price <= 0) return "Цена должна быть больше 0";
        if (item.ValidDays <= 0) return "Срок действия справки должен быть больше 0 дней";
        return null;
    }
}
