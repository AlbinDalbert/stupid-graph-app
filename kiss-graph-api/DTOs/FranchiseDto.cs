using kiss_graph_api.Domain.Enums;

namespace kiss_graph_api.DTOs
{
    public record FranchiseDto
    {
        public string? Uuid { get; init; }
        public required string Name { get; init; }
    }

    public record CreateFranchiseDto
    {
        public string? Name { get; init; }
    }

    public record UpdateFranchiseDto
    {
        public string? Name { get; init; }
    }
}
