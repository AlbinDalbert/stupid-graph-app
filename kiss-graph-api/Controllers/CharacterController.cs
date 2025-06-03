using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _characterService;
    private readonly ILogger<CharacterController> _logger;

    public CharacterController(ICharacterService characterService, ILogger<CharacterController> logger)
    {
        _characterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCharacter()
    {
        _logger.LogInformation("API: Getting all Characters");
        var works = await _characterService.GetAllPersonAsync();
        return Ok(works);
    }

    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetCharacterByUuid(string uuid)
    {
        _logger.LogInformation($"Getting Character by uuid: {uuid}");
        var work = await _characterService.GetCharacterByUuidAsync(uuid);
        return Ok(work);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacter([FromBody] CreateCharacterDto character)
    {
        _logger.LogInformation($"Creating new Character: {character.Name}");
        var work = await _characterService.CreateCharacterAsync(character);
        return CreatedAtAction(nameof(CreateCharacter), work);
    }

    //[HttpPatch("{uuid}")]
    //public async Task<IActionResult> UpdateCharacter(string uuid,[FromBody] UpdateCharacterDto character)
    //{
    //    _logger.LogInformation($"Creating new Character: {character.Name}");
    //    var work = await _characterService.UpdateCharacterAsync(uuid, character);
    //    return CreatedAtAction(nameof(CreateCharacter), work);
    //}

    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeleteCharacter(string uuid)
    {
        _logger.LogInformation($"Deleting Character: {uuid}");
        await _characterService.DeletePersonAsync(uuid);
        return NoContent();
    }
}

