using kiss_graph_api.Domain.Enums;

namespace kiss_graph_api.DTOs
{
    public class CInFranchiseDto
    {
    }
    public class CInFranchiseSummaryDto : CInFranchiseDto
    {
        public string? CharacterName { get; init; }
        public string? CharacterUuid { get; init; }

        public string? FranchiseName { get; init; }
        public string? FranchiseUuid { get; init; }
    }
}
