using kiss_graph_api.Domain.Enums;

namespace kiss_graph_api.DTOs
{
    public record PersonDto
    {
        public string? Uuid { get; init; }
        public required string Name { get; init; }
        public string? ImageUrl { get; init; }
        public DateOnly? BornDate { get; init; }
        public Gender? Gender { get; init; }
    }

    public record CreatePersonDto
    {
        public required string Name { get; init; }
        public string? ImageUrl { get; init; }
        public DateOnly? BornDate { get; init; }
        public Gender? Gender { get; init; }
    }

    public record UpdatePersonDto
    {
        public string? Name { get; init; }
        public string? ImageUrl { get; init; }
        public DateOnly? BornDate { get; init; }
        public Gender? Gender { get; init; }
    }
}
