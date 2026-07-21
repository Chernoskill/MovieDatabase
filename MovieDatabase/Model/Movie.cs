using System.Text.Json.Serialization;

namespace MovieDatabase.Model
{
    public partial class Movie : ObservableObject
    {
        public Guid Id { get; set; }

        [ObservableProperty]
        public partial string? Title { get; set; }

        [ObservableProperty]
        public partial string? Description { get; set; }

        [ObservableProperty]
        public partial string? Director { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ActorsDisplay))]
        public partial string[]? Actors { get; set; }

        [ObservableProperty]
        public partial DateOnly? Year { get; set; }

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
