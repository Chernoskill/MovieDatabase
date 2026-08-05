using System.Globalization;
using System.Resources;

namespace MovieDatabase.Localization
{
    /// <summary>
    /// Wraps the Localization/Strings/AppResources resx table and exposes its strings as a
    /// bindable indexer. <see cref="CurrentCulture"/> fires an ordinary PropertyChanged whenever
    /// <see cref="SetLanguage"/> switches the active culture, which is what
    /// <see cref="TranslateExtension"/> binds to in order to re-read translations live - this is
    /// plain single-value binding, not the legacy WPF "indexer changed" convention, so it refreshes
    /// reliably even for pages Shell already realized (e.g. cached TabBar pages) before the switch.
    /// </summary>
    public class LocalizationResourceManager : INotifyPropertyChanged
    {
        const string PreferenceKey = "AppLanguage";

        static readonly Lazy<LocalizationResourceManager> instance = new(() => new LocalizationResourceManager());

        readonly ResourceManager resourceManager;
        CultureInfo currentCulture = CultureInfo.CurrentUICulture;

        public static LocalizationResourceManager Instance => instance.Value;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Raised after the active language changes, once translated strings are ready to re-read.</summary>
        public event Action? CultureChanged;

        public AppLanguage CurrentLanguage { get; private set; }

        public CultureInfo CurrentCulture
        {
            get => currentCulture;
            private set
            {
                currentCulture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
            }
        }

        LocalizationResourceManager()
        {
            resourceManager = new ResourceManager("MovieDatabase.Localization.Strings.AppResources", typeof(LocalizationResourceManager).Assembly);
            ApplyCulture(ResolveInitialLanguage(), persist: false);
        }

        public string this[string key] => resourceManager.GetString(key, CurrentCulture) ?? key;

        public void SetLanguage(AppLanguage language)
        {
            if (language == CurrentLanguage)
            {
                return;
            }

            ApplyCulture(language, persist: true);
        }

        static AppLanguage ResolveInitialLanguage()
        {
            string stored = Preferences.Default.Get(PreferenceKey, string.Empty);

            if (!string.IsNullOrEmpty(stored) && Enum.TryParse(stored, out AppLanguage parsed))
            {
                return parsed;
            }

            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase)
                ? AppLanguage.German
                : AppLanguage.English;
        }

        void ApplyCulture(AppLanguage language, bool persist)
        {
            CurrentLanguage = language;

            if (persist)
            {
                Preferences.Default.Set(PreferenceKey, language.ToString());
            }

            CultureInfo culture = new(language == AppLanguage.German ? "de" : "en");
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;

            // The CurrentCulture property setter raises PropertyChanged, which is what makes every
            // {loc:Translate} binding across the app re-read its string - see the class remarks.
            CurrentCulture = culture;

            CultureChanged?.Invoke();
        }
    }
}
