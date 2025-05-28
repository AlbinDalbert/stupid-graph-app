using kiss_graph_api.DTOs;

namespace kiss_graph_api.Services.Interfaces
{
    public interface IPersonService
    {
        // Basic creative node operations
        Task<IEnumerable<PersonDto>> GetAllPersonAsync();
        Task<PersonDto> GetPersonByUuidAsync(string uuid);
        Task<PersonDto> CreatePersonAsync(CreatePersonDto personDto);
        Task<PersonDto> UpdatePersonAsync(string uuid, UpdatePersonDto personDto);
        Task DeletePersonAsync(string uuid);
    }
}