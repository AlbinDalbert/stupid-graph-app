using kiss_graph_api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kiss_graph_api.Services.Interfaces
{
    public interface ICreativeWorkService
    {
        Task<IEnumerable<CreativeWorkDto>> GetAllCreativeWorksAsync();
        // Add other methods
    }
}