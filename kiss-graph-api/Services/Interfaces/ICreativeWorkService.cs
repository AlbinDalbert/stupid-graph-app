using kiss_graph_api.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kiss_graph_api.Services.Interfaces
{
    public interface ICreativeWorkService
    {
        // Basic creative node operations
        Task<IEnumerable<CreativeWorkDto>> GetAllCreativeWorksAsync();
        Task<CreativeWorkDto> GetCreativeWorkByUuidAsync(string uuid);
        Task<CreativeWorkDto> CreateCreativeWorkAsync(CreateCreativeWorkDto creativeWork);
        Task DeleteCreativeWorkAsync(string uuid);
    }
}