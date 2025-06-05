using kiss_graph_api.DTOs;

namespace kiss_graph_api.Services.Interfaces
{
    public interface IPersonService
    {
        // Basic creative node operations
        Task<IEnumerable<PersonDto>> GetAllPersonAsync();
        Task<IEnumerable<ActedInSummaryDto>> GetAllActedInAsync(string uuid);
        Task<PersonDto> GetPersonByUuidAsync(string uuid);
        Task<PersonDto> CreatePersonAsync(CreatePersonDto personDto);
        Task<PersonDto> UpdatePersonAsync(string uuid, UpdatePersonDto personDto);
        Task DeletePersonAsync(string uuid);
        // acting operations
        Task<ActedInSummaryDto> AssignActingAsync(string uuid, string movieUuid, ActedInDto actedIn);
        Task DeleteAssignedActingAsync(string uuid, string movieUuid);
        // portrtayed operations
        Task<PortrayedSummaryDto> PortrayedAsync(string uuid, string characterUuid, PortrayedDto portrayed);
        Task DeletePortrayedAsync(string uuid, string characterUuid);
    }
}