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
    }

    public static class NeoProp
    {
        private const string _Uuid = "uuid";
        private const string _Title = "title";
        private const string _Type = "type";
        private const string _ReleaseDate = "releaseDate";

        public static class CreativeWork
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;
        }

        public static class Movie
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;

            public const string ProductionCompany = "productionCompany";
        }


        public static class Show
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;

            public const string ProductionCompany = "productionCompany";
            public const string NumberOfEpisodes = "numberOfEpisodes";
        }

        public static class Book
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;

            public const string Isbn = "isbn";
            public const string Publisher = "publisher";
        }

        public static class Game
        {
            public const string Uuid = _Uuid;
            public const string Title = _Title;
            public const string Type = _Type;
            public const string ReleaseDate = _ReleaseDate;

            public const string Developer = "developer";
            public const string Publisher = "publisher";
        }

        public static class Person
        {
            public const string Uuid = _Uuid;
            public const string Name = "name";
            public const string BornDate = "bornDate";
            public const string Gender = "gender";
        }

        public static class Character
        {
            public const string Uuid = _Uuid;
            public const string Name = "name";
            public const string Gender = "gender";
        }

        public static class Genre
        {
            public const string Uuid = _Uuid;
            public const string Name = "name";
        }

        public static class ActedIn
        {
            public const string RoleType = "roleType";
        }

        public static class AppearsIn
        {
            public const string CharacterType = "CharacterType";
        }
    }

}