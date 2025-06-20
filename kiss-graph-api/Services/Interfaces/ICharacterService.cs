﻿using kiss_graph_api.DTOs;

namespace kiss_graph_api.Services.Interfaces
{
    public interface ICharacterService
    {
        // Basic creative node operations
        Task<IEnumerable<CharacterDto>> GetAllPersonAsync();
        Task<CharacterDto?> GetCharacterByUuidAsync(string uuid);
        Task<CharacterDto> CreateCharacterAsync(CreateCharacterDto characterDto);
        Task<CharacterDto> UpdateCharacterAsync(string uuid, UpdateCharacterDto characterDto);
        Task DeletePersonAsync(string uuid);
        Task<AppearsInSummaryDto> AppearsInAsync(string uuid, string creativeWorkUuid, AppearsInDto appearsInDto);
        Task DeleteAppearsInAsync (string uuid, string creativeWorkUuid);
        Task AssignToFranchise(string characterUuid, string franchiseUuid);
        Task DeleteFromFranchise(string characterUuid, string franchiseUuid);
    }
}