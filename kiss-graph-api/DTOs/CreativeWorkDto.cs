﻿using kiss_graph_api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace kiss_graph_api.DTOs
{
    // --- output dtos --- //
    public record CreativeWorkDto
    {
        public string? Uuid { get; init; }
        public required string Title { get; init; }
        public required CreativeWorkType Type { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }

    // --- input dtos --- //
    public record CreateCreativeWorkDto
    {
        [Required(ErrorMessage = "of course it has a name you silly billy ;)")]
        public required string Title { get; init; }
        [Required(ErrorMessage = "at least say 'other' if you don't know")]
        public required CreativeWorkType Type { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }

    public record UpdateCreativeWorkDto
    {
        public string? Title { get; init; }
        public CreativeWorkType? Type { get; init; }
        public DateOnly? ReleaseDate { get; init; }
    }
}
