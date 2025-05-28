using kiss_graph_api.Domain.Enums;

namespace kiss_graph_api.DTOs
{
    public record MovieDto
    {
        public string? Uuid { get; init; }
        public required string Title { get; init; }
        public required CreativeWorkType Type { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }

    public record CreateMovieDto
    {
        public required string Title { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }

    public record UpdateMovieDto
    {
        public string? Title { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }
}
