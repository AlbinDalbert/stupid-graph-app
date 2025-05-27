using kiss_graph_api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface ICreativeWorkRepository
    {
        Task<IEnumerable<CreativeWorkDto>> GetAllAsync();
        Task<CreativeWorkDto?> GetByUuidAsync(string uuid);
        Task<CreativeWorkDto> CreateAsync(CreateCreativeWorkDto creativeWork);
        Task DeleteAsync(string uuid);
    }
}