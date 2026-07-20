using System.Text.Json.Serialization;

namespace MovieDatabase.Model
{
    public partial class Movie : ObservableObject
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Director { get; set; }
        public string[]? Actors { get; set; }
        public DateOnly? Year { get; set; }

        [ObservableProperty]
        public partial bool Favorite { get; set; }

        [JsonIgnore]
        [ObservableProperty]
        public partial bool IsMarked { get; set; }

        public List<Guid> Tags { get; set; } = new List<Guid>();

        [JsonIgnore]
        public string ActorsDisplay => Actors is null ? string.Empty : string.Join(", ", Actors);

        public Movie()
        {
            Id = Guid.NewGuid();
        }

        public Movie(string title, DateOnly? year)
        {
            Id = Guid.NewGuid();
            Title = title;
            Year = year;
        }
    }

    [JsonSerializable(typeof(List<Movie>))]
    internal sealed partial class MovieContext : JsonSerializerContext
    {

    }
}
