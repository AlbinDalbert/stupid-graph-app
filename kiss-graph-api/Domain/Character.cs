namespace kiss_graph_api.Domain
{
    public record Character
    {
        public required string Uuid { get; init; }
        public required string Name { get; init; }
    }
}
