using System.Text.Json.Serialization;

namespace MovieDatabase.Model
{
    public class MovieDatabaseData
    {
        public List<Movie> Movies { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
    }

    [JsonSerializable(typeof(MovieDatabaseData))]
    internal sealed partial class MovieDatabaseDataContext : JsonSerializerContext
    {

    }
}
