using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace kiss_graph_api.Clients.Models
{
    /// <summary>
    /// Represents the top-level response from the TMDB /person/{id}/tv_credits endpoint.
    /// </summary>
    public class TmdbPersonTvCreditsDto
    {
        [JsonPropertyName("cast")]
        public required List<TmdbPersonTvCastCreditDto> Cast { get; set; }

        [JsonPropertyName("crew")]
        public required List<TmdbPersonTvCrewCreditDto> Crew { get; set; }

        [JsonPropertyName("id")]
        public required int Id { get; set; } // The TMDB ID of the person these credits belong to
    }

    /// <summary>
    /// Represents a single TV show a person acted in, including their character name and episode count.
    /// </summary>
    public class TmdbPersonTvCastCreditDto
    {
        [JsonPropertyName("adult")]
        public required bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("genre_ids")]
        public required List<int> GenreIds { get; set; }

        [JsonPropertyName("id")]
        public required int Id { get; set; } // This is the TV Show's TMDB ID

        [JsonPropertyName("origin_country")]
        public required List<string> OriginCountry { get; set; }

        [JsonPropertyName("original_language")]
        public required string OriginalLanguage { get; set; }

        [JsonPropertyName("original_name")]
        public required string OriginalName { get; set; }

        [JsonPropertyName("overview")]
        public required string Overview { get; set; }

        [JsonPropertyName("popularity")]
        public required double Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("first_air_date")]
        public string? FirstAirDate { get; set; } // Note: Different from 'release_date' for movies

        [JsonPropertyName("name")]
        public required string Name { get; set; } // Note: Different from 'title' for movies

        [JsonPropertyName("vote_average")]
        public required double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public required int VoteCount { get; set; }

        [JsonPropertyName("character")]
        public required string Character { get; set; }

        [JsonPropertyName("credit_id")]
        public required string CreditId { get; set; }

        [JsonPropertyName("episode_count")]
        public required int EpisodeCount { get; set; }
    }

    /// <summary>
    /// Represents a single TV show a person worked on as crew, including their job.
    /// </summary>
    public class TmdbPersonTvCrewCreditDto
    {
        [JsonPropertyName("adult")]
        public required bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("genre_ids")]
        public required List<int> GenreIds { get; set; }

        [JsonPropertyName("id")]
        public required int Id { get; set; } // This is the TV Show's TMDB ID

        [JsonPropertyName("origin_country")]
        public required List<string> OriginCountry { get; set; }

        [JsonPropertyName("original_language")]
        public required string OriginalLanguage { get; set; }

        [JsonPropertyName("original_name")]
        public required string OriginalName { get; set; }

        [JsonPropertyName("overview")]
        public required string Overview { get; set; }

        [JsonPropertyName("popularity")]
        public required double Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("first_air_date")]
        public string? FirstAirDate { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("vote_average")]
        public required double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public required int VoteCount { get; set; }

        [JsonPropertyName("credit_id")]
        public required string CreditId { get; set; }

        [JsonPropertyName("department")]
        public required string Department { get; set; }

        [JsonPropertyName("episode_count")]
        public required int EpisodeCount { get; set; }

        [JsonPropertyName("job")]
        public required string Job { get; set; }
    }
}