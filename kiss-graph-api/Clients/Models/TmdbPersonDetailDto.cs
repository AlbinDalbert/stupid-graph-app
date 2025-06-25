using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace kiss_graph_api.Clients.Models
{
    /// <summary>
    /// Represents the detailed information for a single person from the TMDB API.
    /// Corresponds to your 'Root' class.
    /// </summary>
    public class TmdbPersonDetailDto
    {
        [JsonPropertyName("adult")]
        public required bool Adult { get; set; }

        [JsonPropertyName("also_known_as")]
        public required List<string> AlsoKnownAs { get; set; }

        [JsonPropertyName("biography")]
        public required string Biography { get; set; }

        [JsonPropertyName("birthday")]
        public required string? Birthday { get; set; } // Can be null, so string? is safer

        [JsonPropertyName("deathday")]
        public required string? Deathday { get; set; } // Corrected from object to nullable string

        [JsonPropertyName("gender")]
        public required int Gender { get; set; } // 0 = Not set, 1 = Female, 2 = Male, 3 = Non-binary

        [JsonPropertyName("homepage")]
        public required string? Homepage { get; set; } // Corrected from object to nullable string

        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("imdb_id")]
        public required string ImdbId { get; set; }

        [JsonPropertyName("known_for_department")]
        public required string KnownForDepartment { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("place_of_birth")]
        public required string? PlaceOfBirth { get; set; } // Can be null

        [JsonPropertyName("popularity")]
        public required double Popularity { get; set; }

        [JsonPropertyName("profile_path")]
        public required string? ProfilePath { get; set; } // Can be null, this is your image path
    }
}