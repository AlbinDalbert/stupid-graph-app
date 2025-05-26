using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces; // Use your Interface namespace
using kiss_graph_api.Repositories.Interfaces; // Use your Repo Interface namespace
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kiss_graph_api.Services.Implementations
{
    public class CreativeWorkService : ICreativeWorkService
    {
        private readonly ICreativeWorkRepository _repository;
        private readonly ILogger<CreativeWorkService> _logger;

        public CreativeWorkService(ICreativeWorkRepository repository, ILogger<CreativeWorkService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<CreativeWorkDto>> GetAllCreativeWorksAsync()
        {
            _logger.LogInformation("Service: Getting all Creative Works");

            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error getting all creative works.");
                return new List<CreativeWorkDto>(); // Or throw
            }
        }
        // --- Add other methods (GetById, Create, Delete) here ---
    }
}