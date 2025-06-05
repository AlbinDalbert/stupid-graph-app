using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Exceptions;
using System;
using kiss_graph_api.Domain.Relationship;

namespace kiss_graph_api.Services.Implementations
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly IActedInRepository _actedInRepository;
        private readonly IPortrayedRepository _portrayedRepository;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonRepository repository, IActedInRepository actedInRepository, IPortrayedRepository portrayedRepository, ILogger<PersonService> logger)
        {
            _repository = repository;
            _actedInRepository = actedInRepository;
            _portrayedRepository = portrayedRepository;
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

        public async Task DeleteAssignedActingAsync(string uuid, string movieUuid)
        {
            _logger.LogInformation($"Service: Delete assigned acting in acting {uuid} in {movieUuid}");
            await _actedInRepository.DeleteLink(uuid, movieUuid);
        }

        public async Task<PortrayedSummaryDto> PortrayedAsync(string uuid, string characterUuid, PortrayedDto portrayed)
        {
            _logger.LogInformation($"Service: Delete assigned acting in acting {uuid} in {characterUuid}");
            return await _portrayedRepository.CreateLink(uuid, characterUuid, portrayed);
        }

        public async Task DeletePortrayedAsync(string uuid, string characterUuid)
        {
            _logger.LogInformation($"Service: Delete assigned acting in acting {uuid} in {characterUuid}");
            await _portrayedRepository.DeleteLink(uuid, characterUuid);
        }
    }
}