using kiss_graph_api.Domain.Enums;

namespace kiss_graph_api.DTOs
{
    public class CWInFranchiseDto
    {
    }
    public class CWInFranchiseSummaryDto : CWInFranchiseDto
    {
        public string? CreativeWorkTitle { get; init; }
        public string? CreativeWorkUuid { get; init; }

        public string? FranchiseName { get; init; }
        public string? FranchiseUuid { get; init; }
    }
}
