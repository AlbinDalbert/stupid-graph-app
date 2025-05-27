using kiss_graph_api.Models.Enums;

namespace kiss_graph_api.DTOs
{
    public record CreativeWorkDto
    {
        public string? Uuid { get; init; }
        public required string Title { get; init; }
        public required CreativeWorkType Type { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }

    public record CreateCreativeWorkDto
    {
        public required string Title { get; init; }
        public required CreativeWorkType Type { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }
}
