using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    [QueryProperty(nameof(Movie), "Movie")]
    public partial class MovieDetailsViewModel : BaseViewModel
    {
        readonly MovieService movieService;
        readonly TagService tagService;

        bool isNewMovie;

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
        public ObservableCollection<TagBadge> AssignedTagBadges { get; } = new();

        public bool IsNotEditing => !IsEditing;

        public MovieDetailsViewModel(MovieService movieService, TagService tagService)
        {
            this.movieService = movieService;
            this.tagService = tagService;
        }

        partial void OnMovieChanged(Movie value)
        {
            isNewMovie = false;
            _ = LoadTagOptionsAsync();
        }

        public void StartNewMovie()
        {
            Movie = new Movie();
            isNewMovie = true;
            EnterEditMode();
        }

        async Task LoadTagOptionsAsync()
        {
            List<Tag> tags = await tagService.GetTags();

            TagOptions.Clear();
            AssignedTagBadges.Clear();

            foreach (Tag tag in tags)
            {
                bool isSelected = Movie.Tags.Contains(tag.Id);
                TagOptions.Add(new TagOption(tag, isSelected));

                if (isSelected)
                {
                    AssignedTagBadges.Add(new TagBadge(tag.Name, tag.ColorIndex));
                }
            }
        }

        void EnterEditMode()
        {
            ActorsText = Movie.ActorsDisplay;
            YearText = Movie.Year?.Year.ToString();
            _ = LoadTagOptionsAsync();

            IsEditing = true;
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

                if (isNewMovie)
                {
                    await movieService.AddMovieAsync(Movie);
                    isNewMovie = false;
                }
                else
                {
                    await movieService.SaveMovieAsync(Movie);
                }

                AssignedTagBadges.Clear();
                foreach (TagOption option in TagOptions.Where(option => option.IsSelected))
                {
                    AssignedTagBadges.Add(new TagBadge(option.Name, option.ColorIndex));
                }

                IsEditing = false;
            }
            else
            {
                EnterEditMode();
            }
        }
    }
}
