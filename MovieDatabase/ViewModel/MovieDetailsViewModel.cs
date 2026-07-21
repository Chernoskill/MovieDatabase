using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    [QueryProperty(nameof(Movie), "Movie")]
    public partial class MovieDetailsViewModel : BaseViewModel
    {
        readonly MovieService movieService;
        readonly TagService tagService;

        [ObservableProperty]
        public required partial Movie Movie { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotEditing))]
        public partial bool IsEditing { get; set; }

        [ObservableProperty]
        public partial string? ActorsText { get; set; }

        [ObservableProperty]
        public partial string? YearText { get; set; }

        public ObservableCollection<TagOption> TagOptions { get; } = new();

        public bool IsNotEditing => !IsEditing;

        public string AssignedTagsDisplay => string.Join(", ", TagOptions.Where(option => option.IsSelected).Select(option => option.Name));

        public MovieDetailsViewModel(MovieService movieService, TagService tagService)
        {
            this.movieService = movieService;
            this.tagService = tagService;
        }

        partial void OnMovieChanged(Movie value)
        {
            _ = LoadTagOptionsAsync();
        }

        async Task LoadTagOptionsAsync()
        {
            List<Tag> tags = await tagService.GetTags();

            TagOptions.Clear();

            foreach (Tag tag in tags)
            {
                TagOptions.Add(new TagOption(tag, Movie.Tags.Contains(tag.Id)));
            }

            OnPropertyChanged(nameof(AssignedTagsDisplay));
        }

        [RelayCommand]
        Task ToggleFavoriteAsync()
        {
            return movieService.ToggleFavoriteAsync(Movie);
        }

        [RelayCommand]
        async Task ToggleEditAsync()
        {
            if (IsEditing)
            {
                Movie.Actors = string.IsNullOrWhiteSpace(ActorsText)
                    ? null
                    : ActorsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                Movie.Year = int.TryParse(YearText, out int year) ? new DateOnly(year, 1, 1) : null;

                Movie.Tags = TagOptions.Where(option => option.IsSelected).Select(option => option.Id).ToList();

                await movieService.SaveMovieAsync(Movie);

                OnPropertyChanged(nameof(AssignedTagsDisplay));

                IsEditing = false;
            }
            else
            {
                ActorsText = Movie.ActorsDisplay;
                YearText = Movie.Year?.Year.ToString();
                await LoadTagOptionsAsync();

                IsEditing = true;
            }
        }
    }
}
