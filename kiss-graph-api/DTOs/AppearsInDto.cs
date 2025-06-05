using kiss_graph_api.Domain.Enums;

namespace kiss_graph_api.DTOs
{
    public class AppearsInDto
    {
        public CharacterType? CharacterType { get; set; }
    }
    public class AppearsInSummaryDto : AppearsInDto
    {
        public string? CharacterName { get; init; }
        public string? CharacterUuid { get; init; }

        public string? CreativeWorkTitle { get; init; }
        public string? CreativeWorkUuid { get; init; }
        public string? CreativeWorkType { get; init; }
    }
}
