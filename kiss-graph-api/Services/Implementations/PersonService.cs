using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces; // Use your Interface namespace
using kiss_graph_api.Repositories.Interfaces; // Use your Repo Interface namespace
using kiss_graph_api.Exceptions;
using System;

namespace kiss_graph_api.Services.Implementations
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly IActedInRepository _actedInRepository;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonRepository repository, IActedInRepository actedInRepository, ILogger<PersonService> logger)
        {
            _repository = repository;
            _actedInRepository = actedInRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<PersonDto>> GetAllPersonAsync()
        {
            _logger.LogInformation("Service: Getting all Person");
            return await _repository.GetAllAsync();
        }
        public async Task<PersonDto> GetPersonByUuidAsync(string uuid)
        {
            _logger.LogInformation("Service: Person By Uuid");
            var work = await _repository.GetByUuidAsync(uuid);

            if (work == null)
            {
                throw new NotFoundException($"CreativeWork with UUID '{uuid}' not found.");
            }

            return work;
        }

        public async Task<PersonDto> CreatePersonAsync(CreatePersonDto person)
        {
            _logger.LogInformation("Service: Add Person");
            return await _repository.CreateAsync(person);
        }

        public async Task<PersonDto> UpdatePersonAsync(string uuid, UpdatePersonDto person)
        {
            _logger.LogInformation("Service: Update Person");
            var work = await _repository.UpdateAsync(uuid, person);

            if (work == null)
            {
                throw new NotFoundException($"CreativeWork with UUID '{uuid}' not found.");
            }

            return work;
        }

        public async Task DeletePersonAsync(string uuid)
        {
            _logger.LogInformation("Service: Delete Person");
            await _repository.DeleteAsync(uuid);
        }

        public async Task<ActedInSummaryDto> AssignActingAsync(string uuid, string movieUuid, ActedInDto actedIn)
        {
            _logger.LogInformation($"Service: Assign acting {uuid}");
            return await _actedInRepository.CreateLink(uuid, movieUuid, actedIn);
        }

        public async Task<IEnumerable<ActedInSummaryDto>> GetAllActedInAsync(string uuid)
        {
            _logger.LogInformation($"Service: get all acting in acting {uuid}");
            return await _repository.GetAllActedInAsync(uuid);
        }
    }
}