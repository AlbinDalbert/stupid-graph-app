namespace kiss_graph_api.Domain
{
    public record Genre
    {
        public required string Uuid { get; init; }
        public required string Name { get; init; }
    }
}
