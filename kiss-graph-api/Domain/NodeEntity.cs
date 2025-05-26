using kiss_graph_api.Models.Enums;
using Neo4j.Driver;

namespace kiss_graph_api.Models.Node
{
    public record Person
    {
        public required string Uuid { get; init; }
        public required string Name { get; init; }
    }

    public record Character
    {
        public required string Uuid { get; init; }
        public required string Name { get; init; }
    }

    public record CreativeWork
    {
        public required string Uuid { get; init; }
        public required string Title { get; init; }
        public required CreativeWorkType Type { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }

    public record Franchise
    {
        public required string Uuid { get; init; }
        public required string Name { get; init; }
    }

    public record Genre
    {
        public required string Uuid { get; init; }
        public required string Name { get; init; }
    }

}