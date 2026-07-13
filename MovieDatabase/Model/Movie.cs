using System.Text.Json.Serialization;

namespace MovieDatabase.Model
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? Year { get; set; }
        public bool Favorite { get; set; }

        public Movie()
        {
            Id = Guid.NewGuid();
        }
    }

    [JsonSerializable(typeof(List<Movie>))]
    internal sealed partial class MovieContext : JsonSerializerContext
    {

    }
}
