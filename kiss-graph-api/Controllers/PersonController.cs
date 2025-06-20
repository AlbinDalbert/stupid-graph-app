using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly ILogger<PersonController> _logger;

    public PersonController(IPersonService personService, ILogger<PersonController> logger)
    {
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPerson()
    {
        _logger.LogInformation("API: Getting all Creative Works");
        var works = await _personService.GetAllPersonAsync();
        return Ok(works);
    }

    [HttpGet("acting/{uuid}")]
    public async Task<IActionResult> GetAllActedIn(string uuid)
    {
        _logger.LogInformation("API: Getting all Creative Works");
        var works = await _personService.GetAllActedInAsync(uuid);
        return Ok(works);
    }

    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetPersonByUuid(string uuid)
    {
        _logger.LogInformation($"Getting CreativeWork by uuid: {uuid}");
        var work = await _personService.GetPersonByUuidAsync(uuid);
        return Ok(work);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonDto person)
    {
        _logger.LogInformation($"Creating new Creative Work: {person.Name}");
        var work = await _personService.CreatePersonAsync(person);
        return CreatedAtAction(nameof(CreatePerson), work);
    }

    [HttpPost("{personUuid}/ActedIn/{creativeWorkUuid}")]
    public async Task<IActionResult> AssignActing(string personUuid, string creativeWorkUuid, [FromBody] ActedInDto actedInDto)
    {
        _logger.LogInformation($"Assign acting for person");
        var work = await _personService.AssignActingAsync(personUuid, creativeWorkUuid, actedInDto);
        return Ok(work);
    }

    [HttpDelete("{personUuid}/ActedIn/{creativeWorkUuid}")]
    public async Task<IActionResult> DeleteAssignedActing(string personUuid, string creativeWorkUuid)
    {
        _logger.LogInformation($"Delete assigned acting for person");
        await _personService.DeleteAssignedActingAsync(personUuid, creativeWorkUuid);
        return NoContent();
    }

    [HttpPost("{personUuid}/Portrayed/{characterUuid}")]
    public async Task<IActionResult> Portrayed(string personUuid, string characterUuid, [FromBody] PortrayedDto portrayedDto)
    {
        _logger.LogInformation($"Assign character portrayed for person");
        var work = await _personService.PortrayedAsync(personUuid, characterUuid, portrayedDto);
        return Ok(work);
    }

    [HttpDelete("{personUuid}/Portrayed/{characterUuid}")]
    public async Task<IActionResult> DeletePortrayed(string personUuid, string characterUuid)
    {
        _logger.LogInformation($"Delete character portrayed for person");
        await _personService.DeletePortrayedAsync(personUuid, characterUuid);
        return NoContent();
    }

    [HttpPatch("{uuid}")]
    public async Task<IActionResult> UpdatePerson(string uuid,[FromBody] UpdatePersonDto person)
    {
        _logger.LogInformation($"Creating new Creative Work: {person.Name}");
        var work = await _personService.UpdatePersonAsync(uuid, person);
        return CreatedAtAction(nameof(CreatePerson), work);
    }

    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeletePerson(string uuid)
    {
        _logger.LogInformation($"Deleting Creative Work: {uuid}");
        await _personService.DeletePersonAsync(uuid);
        return NoContent();
    }
}

