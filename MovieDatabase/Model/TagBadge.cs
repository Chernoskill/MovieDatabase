namespace MovieDatabase.Model
{
    public class TagBadge
    {
        public string Name { get; }
        public int ColorIndex { get; }

        public TagBadge(string name, int colorIndex)
        {
            Name = name;
            ColorIndex = colorIndex;
        }
    }
}
