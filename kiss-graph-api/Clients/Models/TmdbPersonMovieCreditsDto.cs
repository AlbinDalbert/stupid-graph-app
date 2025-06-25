using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace kiss_graph_api.Clients.Models
{
    /// <summary>
    /// Represents the top-level response from the TMDB /person/{id}/movie_credits endpoint.
    /// It contains lists of movies the person acted in or worked on as crew.
    /// Corresponds to your 'Root' class.
    /// </summary>
    public class TmdbPersonMovieCreditsDto
    {
        [JsonPropertyName("cast")]
        public required List<TmdbPersonCastCreditDto> Cast { get; set; } // Correctly typed list

        [JsonPropertyName("crew")]
        public required List<TmdbPersonCrewCreditDto> Crew { get; set; } // Correctly typed list

        [JsonPropertyName("id")]
        public required int Id { get; set; } // The TMDB ID of the person these credits belong to
    }

    /// <summary>
    /// Represents a single movie a person acted in, including their character name.
    /// </summary>
    public class TmdbPersonCastCreditDto
    {
        [JsonPropertyName("adult")]
        public required bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; } // Made nullable

        [JsonPropertyName("genre_ids")]
        public required List<int> GenreIds { get; set; } // Correctly typed list

        [JsonPropertyName("id")]
        public required int Id { get; set; } // This is the Movie's TMDB ID

        [JsonPropertyName("original_language")]
        public required string OriginalLanguage { get; set; }

        [JsonPropertyName("original_title")]
        public required string OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public required string Overview { get; set; }

        [JsonPropertyName("popularity")]
        public required double Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; } // Made nullable

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; } // Made nullable

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("video")]
        public required bool Video { get; set; }

        [JsonPropertyName("vote_average")]
        public required double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public required int VoteCount { get; set; }

        [JsonPropertyName("character")]
        public required string Character { get; set; } // The character name as a string

        [JsonPropertyName("credit_id")]
        public required string CreditId { get; set; }

        [JsonPropertyName("order")]
        public required int Order { get; set; }
    }

    /// <summary>
    /// Represents a single movie a person worked on as crew, including their job.
    /// </summary>
    public class TmdbPersonCrewCreditDto
    {
        [JsonPropertyName("adult")]
        public required bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; } // Made nullable

        [JsonPropertyName("genre_ids")]
        public required List<int> GenreIds { get; set; } // Correctly typed list

        [JsonPropertyName("id")]
        public required int Id { get; set; } // This is the Movie's TMDB ID

        [JsonPropertyName("original_language")]
        public required string OriginalLanguage { get; set; }

        [JsonPropertyName("original_title")]
        public required string OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public required string Overview { get; set; }

        [JsonPropertyName("popularity")]
        public required double Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; } // Made nullable

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; } // Made nullable

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("video")]
        public required bool Video { get; set; }

        [JsonPropertyName("vote_average")]
        public required double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public required int VoteCount { get; set; }

        [JsonPropertyName("credit_id")]
        public required string CreditId { get; set; }

        [JsonPropertyName("department")]
        public required string Department { get; set; }

        [JsonPropertyName("job")]
        public required string Job { get; set; }
    }
}