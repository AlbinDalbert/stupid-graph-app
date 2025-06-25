using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace kiss_graph_api.Clients.Models
{
    /// <summary>
    /// This is the main DTO that represents the entire JSON response for a single movie's details from TMDB.
    /// It corresponds to your 'Root' class.
    /// </summary>
    public class TmdbMovieDetailDto
    {
        [JsonPropertyName("adult")]
        public required bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public required string BackdropPath { get; set; }

        [JsonPropertyName("belongs_to_collection")]
        public required TmdbBelongsToCollection BelongsToCollection { get; set; }

        [JsonPropertyName("budget")]
        public required int Budget { get; set; }

        [JsonPropertyName("genres")]
        public required List<TmdbGenre> Genres { get; set; } // Corrected to be a typed list

        [JsonPropertyName("homepage")]
        public required string Homepage { get; set; }

        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("imdb_id")]
        public required string ImdbId { get; set; }

        [JsonPropertyName("origin_country")]
        public required List<string> OriginCountry { get; set; } // Corrected to be a typed list of strings

        [JsonPropertyName("original_language")]
        public required string OriginalLanguage { get; set; }

        [JsonPropertyName("original_title")]
        public required string OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public required string Overview { get; set; }

        [JsonPropertyName("popularity")]
        public required double Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public required string PosterPath { get; set; }

        [JsonPropertyName("production_companies")]
        public required List<TmdbProductionCompany> ProductionCompanies { get; set; } // Corrected

        [JsonPropertyName("production_countries")]
        public required List<TmdbProductionCountry> ProductionCountries { get; set; } // Corrected

        [JsonPropertyName("release_date")]
        public required string ReleaseDate { get; set; }

        [JsonPropertyName("revenue")]
        public required long Revenue { get; set; } // Changed to long to accommodate large numbers

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; } // Made nullable as it can sometimes be null

        [JsonPropertyName("spoken_languages")]
        public required List<TmdbSpokenLanguage> SpokenLanguages { get; set; } // Corrected

        [JsonPropertyName("status")]
        public required string Status { get; set; }

        [JsonPropertyName("tagline")]
        public required string Tagline { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("video")]
        public required bool Video { get; set; }

        [JsonPropertyName("vote_average")]
        public required double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public required int VoteCount { get; set; }
    }

    // --- Supporting Classes for Nested Objects and Lists ---

    public class TmdbBelongsToCollection
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("poster_path")]
        public required string PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public required string BackdropPath { get; set; }
    }

    public class TmdbGenre
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }

    public class TmdbProductionCompany
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("logo_path")]
        public required string LogoPath { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("origin_country")]
        public required string OriginCountry { get; set; }
    }

    public class TmdbProductionCountry
    {
        [JsonPropertyName("iso_3166_1")]
        public required string Iso3166_1 { get; set; } // Keeping underscore here is common for ISO codes

        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }

    public class TmdbSpokenLanguage
    {
        [JsonPropertyName("english_name")]
        public required string EnglishName { get; set; }

        [JsonPropertyName("iso_639_1")]
        public required string Iso639_1 { get; set; } // Keeping underscore for ISO codes

        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}