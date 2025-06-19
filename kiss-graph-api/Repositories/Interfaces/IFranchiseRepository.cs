using kiss_graph_api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface IFranchiseRepository
    {
        Task<IEnumerable<FranchiseDto>> GetAllAsync();
        Task<FranchiseDto?> GetByUuidAsync(string uuid);
        Task<FranchiseDto> CreateAsync(CreateFranchiseDto franchise);
        Task<FranchiseDto?> UpdateAsync(string uuid, UpdateFranchiseDto movie);
        Task DeleteAsync(string uuid);
    }
}