using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces; // Use your Interface namespace
using kiss_graph_api.Repositories.Interfaces; // Use your Repo Interface namespace
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using kiss_graph_api.Exceptions;

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
            return await _repository.GetAllAsync();
        }
        public async Task<CreativeWorkDto> GetCreativeWorkByUuidAsync(string uuid)
        {
            _logger.LogInformation("Service: Creative Works By Uuid");
            var work = await _repository.GetByUuidAsync(uuid);

            if (work == null)
            {
                throw new NotFoundException($"CreativeWork with UUID '{uuid}' not found.");
            }

            return work;
        }

        public async Task<CreativeWorkDto> CreateCreativeWorkAsync(CreateCreativeWorkDto creativeWork)
        {
            _logger.LogInformation("Service: Add Creative Work");
            return await _repository.CreateAsync(creativeWork);
        }

        public async Task DeleteCreativeWorkAsync(string uuid)
        {
            _logger.LogInformation("Service: Delete Creative Work");
            await _repository.DeleteAsync(uuid);
        }
    }
}