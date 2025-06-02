using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        Task<IEnumerable<PersonDto>> GetAllAsync();
        Task<IEnumerable<ActedInSummaryDto>> GetAllActedInAsync(string uuid);
        Task<PersonDto?> GetByUuidAsync(string uuid);
        Task<PersonDto> CreateAsync(CreatePersonDto personDto);
        Task<PersonDto?> UpdateAsync(string uuid, UpdatePersonDto personDto);
        Task DeleteAsync(string uuid);
    }
}