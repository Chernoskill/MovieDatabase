using System.Windows.Input;
using Microsoft.Maui.Graphics;

namespace MovieDatabase.Controls
{
    public partial class ColorPickerView : ContentView
    {
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(ColorPickerView),
            0,
            BindingMode.TwoWay,
            propertyChanged: OnSelectedIndexChanged);

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public List<PaletteSwatch> Swatches { get; }

        public ICommand SelectCommand { get; }

        public ColorPickerView()
        {
            Swatches = TagPalette.Colors
                .Select((color, index) => new PaletteSwatch(index, color))
                .ToList();

            SelectCommand = new Command<int>(index => SelectedIndex = index);

            InitializeComponent();
            BindableLayout.SetItemsSource(PaletteGrid, Swatches);

            UpdateSelection();
        }

        static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ColorPickerView)bindable).UpdateSelection();
        }

        void UpdateSelection()
        {
            foreach (PaletteSwatch swatch in Swatches)
            {
                swatch.IsSelected = swatch.Index == SelectedIndex;
            }
        }
    }

    public partial class PaletteSwatch : ObservableObject
    {
        public int Index { get; }
        public int Row => Index / 4;
        public int Column => Index % 4;
        public Color Color { get; }

        [ObservableProperty]
        public partial bool IsSelected { get; set; }

        public PaletteSwatch(int index, Color color)
        {
            Index = index;
            Color = color;
        }
    }
}
