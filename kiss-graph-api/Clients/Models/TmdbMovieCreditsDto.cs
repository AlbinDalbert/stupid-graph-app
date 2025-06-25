using System.Collections.Generic;
using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace kiss_graph_api.Clients.Models
{
    /// <summary>
    /// Represents the top-level response from the TMDB /movie/{id}/credits endpoint.
    /// Corresponds to your 'Root' class.
    /// </summary>
    public class TmdbMovieCreditsDto
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; } // The TMDB ID of the movie the credits belong to

        [JsonPropertyName("cast")]
        public required List<TmdbCastMemberDto> Cast { get; set; } // Correctly typed list

        [JsonPropertyName("crew")]
        public required List<TmdbCrewMemberDto> Crew { get; set; } // Correctly typed list
    }

    /// <summary>
    /// Represents a single cast member (an actor's role).
    /// </summary>
    public class TmdbCastMemberDto
    {
        [JsonPropertyName("adult")]
        public required bool Adult { get; set; }

        [JsonPropertyName("gender")]
        public required int Gender { get; set; }

        [JsonPropertyName("id")]
        public required int Id { get; set; } // This is the Person's TMDB ID

        [JsonPropertyName("known_for_department")]
        public required string KnownForDepartment { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; } // This is the Person's name

        [JsonPropertyName("original_name")]
        public required string OriginalName { get; set; }

        [JsonPropertyName("popularity")]
        public required double Popularity { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; } // Made nullable for safety

        [JsonPropertyName("cast_id")]
        public required int CastId { get; set; }

        [JsonPropertyName("character")]
        public required string Character { get; set; } // This is the Character's name as a string

        [JsonPropertyName("credit_id")]
        public required string CreditId { get; set; }

        [JsonPropertyName("order")]
        public required int Order { get; set; }
    }

    /// <summary>
    /// Represents a single crew member (director, writer, producer, etc.).
    /// </summary>
    public class TmdbCrewMemberDto
    {
        [JsonPropertyName("adult")]
        public required bool Adult { get; set; }

        [JsonPropertyName("gender")]
        public required int Gender { get; set; }

        [JsonPropertyName("id")]
        public required int Id { get; set; } // This is the Person's TMDB ID

        [JsonPropertyName("known_for_department")]
        public required string KnownForDepartment { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; } // This is the Person's name

        [JsonPropertyName("original_name")]
        public required string OriginalName { get; set; }

        [JsonPropertyName("popularity")]
        public required double Popularity { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; } // Made nullable for safety

        [JsonPropertyName("credit_id")]
        public required string CreditId { get; set; }

        [JsonPropertyName("department")]
        public required string Department { get; set; }

        [JsonPropertyName("job")]
        public required string Job { get; set; }
    }
}