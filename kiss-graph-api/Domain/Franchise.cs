namespace kiss_graph_api.Domain
{
    public record Franchise
    {
        public required string Uuid { get; init; }
        public required string Name { get; init; }
    }
}
