using MovieDatabase.Localization;
using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    public partial class FilterViewModel : BaseViewModel
    {
        readonly string[] favoriteOptionValues;

        readonly FilterService filterService;
        readonly TagService tagService;

        [ObservableProperty]
        public partial string? TitleFilter { get; set; }

        [ObservableProperty]
        public partial string? DescriptionFilter { get; set; }

        [ObservableProperty]
        public partial string? DirectorFilter { get; set; }

        [ObservableProperty]
        public partial string? ActorsFilter { get; set; }

        [ObservableProperty]
        public partial string? YearFromText { get; set; }

        [ObservableProperty]
        public partial string? YearToText { get; set; }

        [ObservableProperty]
        public partial string FavoriteOption { get; set; }

        public string[] FavoriteOptions => favoriteOptionValues;

        public ObservableCollection<TagOption> TagOptions { get; } = new();

        public FilterViewModel(FilterService filterService, TagService tagService, LocalizationResourceManager localization)
        {
            this.filterService = filterService;
            this.tagService = tagService;
            favoriteOptionValues = new[]
            {
                localization["Filter_FavoriteAny"],
                localization["Filter_FavoriteOnly"],
                localization["Filter_FavoriteNonOnly"]
            };
            FavoriteOption = favoriteOptionValues[0];
        }

        public async Task LoadAsync()
        {
            FilterCriteria criteria = filterService.Criteria;

            TitleFilter = criteria.TitleFilter;
            DescriptionFilter = criteria.DescriptionFilter;
            DirectorFilter = criteria.DirectorFilter;
            ActorsFilter = criteria.ActorsFilter;
            YearFromText = criteria.YearFrom?.ToString();
            YearToText = criteria.YearTo?.ToString();
            FavoriteOption = criteria.Favorite switch
            {
                true => favoriteOptionValues[1],
                false => favoriteOptionValues[2],
                null => favoriteOptionValues[0]
            };

            List<Tag> tags = await tagService.GetTags();

            TagOptions.Clear();

            foreach (Tag tag in tags)
            {
                TagOptions.Add(new TagOption(tag, criteria.TagIds.Contains(tag.Id)));
            }
        }

        [RelayCommand]
        async Task ApplyAsync()
        {
            FilterCriteria criteria = filterService.Criteria;

            criteria.TitleFilter = NullIfEmpty(TitleFilter);
            criteria.DescriptionFilter = NullIfEmpty(DescriptionFilter);
            criteria.DirectorFilter = NullIfEmpty(DirectorFilter);
            criteria.ActorsFilter = NullIfEmpty(ActorsFilter);
            criteria.YearFrom = int.TryParse(YearFromText, out int yearFrom) ? yearFrom : null;
            criteria.YearTo = int.TryParse(YearToText, out int yearTo) ? yearTo : null;
            criteria.Favorite = FavoriteOption == favoriteOptionValues[1]
                ? true
                : FavoriteOption == favoriteOptionValues[2] ? false : null;
            criteria.TagIds = TagOptions.Where(option => option.IsSelected).Select(option => option.Id).ToList();

            filterService.NotifyFilterChanged();

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task ClearAllAsync()
        {
            filterService.Criteria.Reset();
            filterService.NotifyFilterChanged();
            await LoadAsync();
        }

        [RelayCommand]
        async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        static string? NullIfEmpty(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
