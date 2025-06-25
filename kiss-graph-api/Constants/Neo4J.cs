namespace kiss_graph_api.Constants
{
    public static class NeoLabels
    {
        public const string CreativeWork = "CreativeWork";
        public const string Person = "Person";
        public const string Character = "Character";
        public const string Franchise = "Franchise";
        public const string Genre = "Genre";
        public const string ActedIn = "ACTED_IN";
        public const string Portrayed = "PORTRAYED";
        public const string AppearsIn = "APPEARS_IN";
        public const string InFranchise = "IN_FRANCHISE";
        public const string InGenre = "IN_GENRE";
    }

    public static class NeoProp
    {
        private const string _Uuid = "uuid";
        private const string _Title = "title";
        private const string _Name = "name";
        private const string _Type = "type";
        private const string _ReleaseDate = "releaseDate";
        private const string _ImageUrl = "imageUrl";
        private const string _Description = "description";
        private const string _TmdbId = "tmdbId";
        private const string _TmdbRating = "rating";
        private const string _TmdbVoteCount = "tmdbVoteCount";

        public static class CreativeWork
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Description = _Description;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;
            public const string ImageUrl = _ImageUrl;
            public const string TmdbId = _TmdbId;
            public const string TmdbRating = _TmdbRating;
            public const string TmdbVoteCount = _TmdbVoteCount;
            
        }

        public static class Movie
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Description = _Description;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;
            public const string ImageUrl = _ImageUrl;
            public const string TmdbId = _TmdbId;
            public const string TmdbRating = _TmdbRating;
            public const string TmdbVoteCount = _TmdbVoteCount;


            public const string ProductionCompany = "productionCompany";
        }

        public static class Franchise
        {
            public const string Uuid = _Uuid;
            public const string Name = _Name;
            public const string ImageUrl = _ImageUrl;
        }


        public static class Show
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Description = _Description;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;
            public const string ImageUrl = _ImageUrl;
            public const string TmdbId = _TmdbId;

            public const string ProductionCompany = "productionCompany";
            public const string NumberOfEpisodes = "numberOfEpisodes";
        }

        public static class Book
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Description = _Description;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;
            public const string ImageUrl = _ImageUrl;

            public const string Isbn = "isbn";
            public const string Publisher = "publisher";
        }

        public static class Game
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Description = _Description;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;
            public const string ImageUrl = _ImageUrl;

            public const string Developer = "developer";
            public const string Publisher = "publisher";
        }

        public static class Person
        {
            public const string Uuid = _Uuid;
            public const string Name = _Name;
            public const string Description = _Description;
            public const string ImageUrl = _ImageUrl;
            public const string TmdbId = _TmdbId;

            public const string BornDate = "bornDate";
            public const string Gender = "gender";
        }

        public static class Character
        {
            public const string Uuid = _Uuid;
            public const string Name = _Name;
            public const string Description = _Description;
            public const string ImageUrl = _ImageUrl;
            public const string Gender = "gender";
        }

        public static class Genre
        {
            public const string Uuid = _Uuid;
            public const string Name = _Name;
        }

        public static class ActedIn
        {
            public const string RoleType = "roleType";
        }

        public static class AppearsIn
        {
            public const string CharacterType = "CharacterType";
        }

        public static class InFranchise
        {
        }
    }

}