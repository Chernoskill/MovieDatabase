using System.Text.Json.Serialization;

namespace MovieDatabase.Model
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Director { get; set; }
        public string[]? Actors { get; set; }
        public DateOnly? Year { get; set; }
        public bool Favorite { get; set; }
        public List<Guid> Tags { get; set; } = new List<Guid>();

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
