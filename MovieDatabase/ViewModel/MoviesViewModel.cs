using MovieDatabase.Localization;
using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    public partial class MoviesViewModel : BaseViewModel
    {
        public RangeObservableCollection<Movie> Movies { get; } = new();
        public ObservableCollection<Movie> MarkedMovies { get; } = new();
        public ObservableCollection<ActiveFilterChip> ActiveFilters { get; } = new();

        // Persisted here (not on the page) because MoviesViewModel is a singleton and outlives
        // MainPage - this is what lets us restore the scroll position after navigating back to it.
        public int LastVisibleIndex { get; set; }

        readonly MovieService movieService;
        readonly FilterService filterService;
        readonly TagService tagService;
        readonly LocalizationResourceManager localization;

        public bool HasActiveFilters => ActiveFilters.Count > 0;

        public MoviesViewModel(MovieService movieService, FilterService filterService, TagService tagService, LocalizationResourceManager localization)
        {
            this.movieService = movieService;
            this.filterService = filterService;
            this.tagService = tagService;
            this.localization = localization;
            filterService.FilterChanged += () => _ = GetMoviesCommand.ExecuteAsync(null);
            // This view model is a singleton, so it outlives every page - the active filter chip
            // labels below are built in code and need to be rebuilt whenever the language changes.
            localization.CultureChanged += () => _ = GetMoviesCommand.ExecuteAsync(null);
            _ = GetMoviesCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        async Task GoToDetailsAsync(Movie movie)
        {
            if (movie == null)
            {
                return;
            }

            await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
        {
            {"Movie", movie }
        });
        }

        [RelayCommand]
        async Task GoToFilterAsync()
        {
            await Shell.Current.GoToAsync(nameof(FilterPage));
        }

        [RelayCommand]
        void ClearAllFilters()
        {
            filterService.Criteria.Reset();
            filterService.NotifyFilterChanged();
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSortByDisplayId))]
        public partial MovieSortMode SortMode { get; set; }

        public bool IsSortByDisplayId => SortMode == MovieSortMode.DisplayId;

        [RelayCommand]
        void ToggleSortMode()
        {
            SortMode = SortMode == MovieSortMode.Title ? MovieSortMode.DisplayId : MovieSortMode.Title;
            _ = GetMoviesCommand.ExecuteAsync(null);
        }

        [ObservableProperty]
        public partial bool IsRefreshing { get; set; }

        [RelayCommand]
        Task ToggleFavoriteAsync(Movie movie)
        {
            return movieService.ToggleFavoriteAsync(movie);
        }

        [ObservableProperty]
        public partial bool HasMarkedMovies { get; set; }

        [RelayCommand]
        void ToggleMark(Movie movie)
        {
            if (movie == null)
            {
                return;
            }

            movie.IsMarked = !movie.IsMarked;

            if (movie.IsMarked)
            {
                MarkedMovies.Add(movie);
            }
            else
            {
                MarkedMovies.Remove(movie);
            }

            HasMarkedMovies = MarkedMovies.Count > 0;
        }

        [RelayCommand]
        void UnmarkAll()
        {
            foreach (Movie movie in MarkedMovies)
            {
                movie.IsMarked = false;
            }

            MarkedMovies.Clear();
            HasMarkedMovies = false;
        }

        [RelayCommand]
        async Task DeleteMarkedAsync()
        {
            if (MarkedMovies.Count == 0)
            {
                return;
            }

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                localization["MoviesPage_DeleteMoviesTitle"],
                string.Format(localization["MoviesPage_DeleteMoviesMessage"], MarkedMovies.Count),
                localization["Common_Delete"],
                localization["Common_Cancel"]);

            if (!confirmed)
            {
                return;
            }

            List<Movie> moviesToDelete = MarkedMovies.ToList();

            await movieService.DeleteMoviesAsync(moviesToDelete);

            foreach (Movie movie in moviesToDelete)
            {
                Movies.Remove(movie);
                MarkedMovies.Remove(movie);
            }

            HasMarkedMovies = MarkedMovies.Count > 0;
        }

        [RelayCommand]
        async Task GetMoviesAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                List<Movie>? movies = await movieService.GetMovies();
                List<Tag> tags = await tagService.GetTags();
                Dictionary<Guid, Tag> tagsById = tags.ToDictionary(tag => tag.Id);

                FilterCriteria criteria = filterService.Criteria;
                IEnumerable<Movie> matching = (movies ?? new List<Movie>()).Where(criteria.Matches);

                IEnumerable<Movie> sorted = SortMode == MovieSortMode.DisplayId
                    ? matching
                        .OrderBy(movie => string.IsNullOrEmpty(movie.DisplayId))
                        .ThenBy(movie => movie.DisplayId, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(movie => movie.Title, StringComparer.OrdinalIgnoreCase)
                    : matching.OrderBy(movie => movie.Title, StringComparer.OrdinalIgnoreCase);

                List<Movie> result = sorted.ToList();

                foreach (Movie movie in result)
                {
                    movie.TagBadges = movie.Tags
                        .Select(tagId => tagsById.GetValueOrDefault(tagId))
                        .Where(tag => tag != null)
                        .Select(tag => new TagBadge(tag!.Name, tag.ColorIndex))
                        .ToList();
                }

                Movies.ReplaceRange(result);

                RefreshActiveFilters(criteria, tags);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get vies: {ex.Message}");
                await Shell.Current.DisplayAlertAsync(localization["MoviesPage_ErrorTitle"], ex.Message, localization["Common_OK"]);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }

        }

        void RefreshActiveFilters(FilterCriteria criteria, List<Tag> tags)
        {
            ActiveFilters.Clear();

            if (!string.IsNullOrWhiteSpace(criteria.TitleFilter))
            {
                ActiveFilters.Add(new ActiveFilterChip($"{localization["FilterPage_TitleLabel"]}: {criteria.TitleFilter}", () =>
                {
                    criteria.TitleFilter = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (!string.IsNullOrWhiteSpace(criteria.DescriptionFilter))
            {
                ActiveFilters.Add(new ActiveFilterChip($"{localization["FilterPage_DescriptionLabel"]}: {criteria.DescriptionFilter}", () =>
                {
                    criteria.DescriptionFilter = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (!string.IsNullOrWhiteSpace(criteria.DirectorFilter))
            {
                ActiveFilters.Add(new ActiveFilterChip($"{localization["FilterPage_DirectorLabel"]}: {criteria.DirectorFilter}", () =>
                {
                    criteria.DirectorFilter = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (!string.IsNullOrWhiteSpace(criteria.ActorsFilter))
            {
                ActiveFilters.Add(new ActiveFilterChip($"{localization["FilterPage_ActorsLabel"]}: {criteria.ActorsFilter}", () =>
                {
                    criteria.ActorsFilter = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (criteria.YearFrom != null || criteria.YearTo != null)
            {
                string label = $"{localization["MoviesPage_ChipYear"]}: {criteria.YearFrom?.ToString() ?? "…"}-{criteria.YearTo?.ToString() ?? "…"}";
                ActiveFilters.Add(new ActiveFilterChip(label, () =>
                {
                    criteria.YearFrom = null;
                    criteria.YearTo = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (criteria.Favorite != null)
            {
                string label = criteria.Favorite == true ? localization["Filter_FavoriteOnly"] : localization["Filter_FavoriteNonOnly"];
                ActiveFilters.Add(new ActiveFilterChip(label, () =>
                {
                    criteria.Favorite = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            foreach (Guid tagId in criteria.TagIds.ToList())
            {
                string name = tags.FirstOrDefault(tag => tag.Id == tagId)?.Name ?? localization["MoviesPage_ChipUnknownTag"];
                ActiveFilters.Add(new ActiveFilterChip($"{localization["MoviesPage_ChipTag"]}: {name}", () =>
                {
                    criteria.TagIds.Remove(tagId);
                    filterService.NotifyFilterChanged();
                }));
            }

            OnPropertyChanged(nameof(HasActiveFilters));
        }
    }
}
