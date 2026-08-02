using Microsoft.Maui.Graphics;

namespace MovieDatabase.Model
{
    public static class TagPalette
    {
        public static readonly IReadOnlyList<Color> Colors = new List<Color>
        {
            Color.FromArgb("#000000"),
            Color.FromArgb("#1D2B53"),
            Color.FromArgb("#7E2553"),
            Color.FromArgb("#008751"),
            Color.FromArgb("#AB5236"),
            Color.FromArgb("#5F574F"),
            Color.FromArgb("#C2C3C7"),
            Color.FromArgb("#FFF1E8"),
            Color.FromArgb("#FF004D"),
            Color.FromArgb("#FFA300"),
            Color.FromArgb("#FFEC27"),
            Color.FromArgb("#00E436"),
            Color.FromArgb("#29ADFF"),
            Color.FromArgb("#83769C"),
            Color.FromArgb("#FF77A8"),
            Color.FromArgb("#FFCCAA"),
        };

        public static Color GetColor(int index)
        {
            return index >= 0 && index < Colors.Count ? Colors[index] : Colors[0];
        }

        public static Color GetContrastTextColor(int index)
        {
            Color color = GetColor(index);
            double luminance = (0.299 * color.Red) + (0.587 * color.Green) + (0.114 * color.Blue);
            return luminance > 0.6 ? Microsoft.Maui.Graphics.Colors.Black : Microsoft.Maui.Graphics.Colors.White;
        }
    }
}
