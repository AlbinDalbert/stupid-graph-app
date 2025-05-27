using kiss_graph_api.Models.Enums;

namespace kiss_graph_api.Domain
{
    public record CreativeWork
    {
        public required string Uuid { get; init; }
        public required string Title { get; init; }
        public required CreativeWorkType Type { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }
}
