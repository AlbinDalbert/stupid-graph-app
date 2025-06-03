using kiss_graph_api.Domain.Enums;

namespace kiss_graph_api.DTOs
{
    public record CharacterDto
    {
        public string? Uuid { get; init; }
        public required string Name { get; init; }
        public Gender? Gender { get; init; }
    }

    public record CreateCharacterDto
    {
        public required string Name { get; init; }
        public Gender? Gender { get; init; }
    }

    public record UpdateCharacterDto
    {
        public string? Name { get; init; }
        public Gender? Gender { get; init; }
    }
}
