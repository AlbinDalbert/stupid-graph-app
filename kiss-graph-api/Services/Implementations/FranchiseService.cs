using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Exceptions;

namespace kiss_graph_api.Services.Implementations
{
    public class FranchiseService : IFranchiseService
    {
        private readonly IFranchiseRepository _repository;
        private readonly ILogger<FranchiseService> _logger;

        public FranchiseService(IFranchiseRepository repository, ILogger<FranchiseService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<IEnumerable<FranchiseDto>> GetAllFranchisesAsync()
        {
            _logger.LogInformation("Service: Getting all Franchises");
            return await _repository.GetAllAsync();
        }
        public async Task<FranchiseDto> GetFranchiseByUuidAsync(string uuid)
        {
            _logger.LogInformation("Service: Getting Franchise By Uuid");
            var work = await _repository.GetByUuidAsync(uuid);

            if (work == null)
            {
                throw new NotFoundException($"Franchise with UUID '{uuid}' not found.");
            }

            return work;
        }

        public async Task<FranchiseDto> CreateFranchiseAsync(CreateFranchiseDto franchise)
        {
            _logger.LogInformation("Service: Add Franchise");
            return await _repository.CreateAsync(franchise);
        }

        public async Task<FranchiseDto> UpdateFranchiseAsync(string uuid, UpdateFranchiseDto franchise)
        {
            _logger.LogInformation("Service: Update Franchise");
            var work = await _repository.UpdateAsync(uuid, franchise);

            if (work == null)
            {
                throw new NotFoundException($"Franchise with UUID '{uuid}' not found.");
            }

            return work;
        }

        public async Task DeleteFranchiseAsync(string uuid)
        {
            _logger.LogInformation("Service: Delete Franchise");
            await _repository.DeleteAsync(uuid);
        }
    }
}