using System.Text.Json.Serialization;

namespace MovieDatabase.Model
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Tag(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }

    [JsonSerializable(typeof(List<Tag>))]
    internal sealed partial class TagContext : JsonSerializerContext
    {

    }
}
