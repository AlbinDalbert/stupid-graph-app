
namespace kiss_graph_api.DTOs
{
    public record GenreDto
    {
        public string? Uuid { get; init; }
        public required string Name { get; init; }
    }

    public record CreateGenreDto
    {
        public required string Name { get; init; }
    }

    public record UpdateGenreDto
    {
        public string? Name { get; init; }
    }
}
