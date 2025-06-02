using kiss_graph_api.Domain.Enums;

namespace kiss_graph_api.DTOs
{
    public class ActedInDto
    {
        public ActingRoleType? Role { get; init; }
    }

    public class ActedInSummaryDto : ActedInDto
    {
        public string? PersonName { get; init; }
        public string? PersonUuid { get; init; }

        public string? CreativeWorkTitle { get; init; }
        public string? CreativeWorkUuid { get; init; }
        public string? CreativeWorkType { get; init; }
    }    
}
