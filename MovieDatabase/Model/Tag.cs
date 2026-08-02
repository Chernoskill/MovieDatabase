using System.Text.Json.Serialization;

namespace MovieDatabase.Model
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int ColorIndex { get; set; }

        public Tag(string name, int colorIndex = 0)
        {
            Id = Guid.NewGuid();
            Name = name;
            ColorIndex = colorIndex;
        }
    }

    [JsonSerializable(typeof(List<Tag>))]
    internal sealed partial class TagContext : JsonSerializerContext
    {

    }
}
