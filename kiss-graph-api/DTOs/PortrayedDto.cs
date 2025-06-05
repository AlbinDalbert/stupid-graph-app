namespace kiss_graph_api.DTOs
{
    public class PortrayedDto
    {
    }
    public class PortrayedSummaryDto : ActedInDto
    {
        public string? PersonName { get; init; }
        public string? PersonUuid { get; init; }

        public string? CharacterName { get; init; }
        public string? CharacterUuid { get; init; }
    }
}
