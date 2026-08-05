using System.Globalization;

namespace MovieDatabase.Localization
{
    /// <summary>
    /// XAML markup extension: {loc:Translate SomeKey} produces a Binding to
    /// LocalizationResourceManager.Instance.CurrentCulture - a plain property, not an indexer - so
    /// the ordinary PropertyChanged notification fired by SetLanguage reliably re-runs the converter
    /// below (which does the actual key lookup) and repaints the bound control. This works even for
    /// pages Shell realized before the language switch, since it doesn't rely on any page being
    /// currently visible, just on the binding still existing.
    /// </summary>
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension<BindingBase>, IValueConverter
    {
        public string Key { get; set; } = string.Empty;

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding(
                path: nameof(LocalizationResourceManager.CurrentCulture),
                source: LocalizationResourceManager.Instance,
                converter: this);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return LocalizationResourceManager.Instance[Key];
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
