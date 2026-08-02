using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    public partial class MoviesViewModel : BaseViewModel
    {
        public ObservableCollection<Movie> Movies { get; } = new();
        public ObservableCollection<Movie> MarkedMovies { get; } = new();
        public ObservableCollection<ActiveFilterChip> ActiveFilters { get; } = new();

        readonly MovieService movieService;
        readonly FilterService filterService;
        readonly TagService tagService;

        public bool HasActiveFilters => ActiveFilters.Count > 0;

        public MoviesViewModel(MovieService movieService, FilterService filterService, TagService tagService)
        {
            Title = "Movie Database";
            this.movieService = movieService;
            this.filterService = filterService;
            this.tagService = tagService;
            filterService.FilterChanged += () => _ = GetMoviesCommand.ExecuteAsync(null);
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
                "Delete movies",
                $"Delete {MarkedMovies.Count} selected movie(s)?",
                "Delete",
                "Cancel");

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

                if (Movies.Count != 0)
                {
                    Movies.Clear();
                }

                FilterCriteria criteria = filterService.Criteria;

                if (movies != null)
                {
                    foreach (Movie movie in movies.Where(criteria.Matches))
                    {
                        movie.TagBadges = movie.Tags
                            .Select(tagId => tags.FirstOrDefault(tag => tag.Id == tagId))
                            .Where(tag => tag != null)
                            .Select(tag => new TagBadge(tag!.Name, tag.ColorIndex))
                            .ToList();

                        Movies.Add(movie);
                    }
                }

                RefreshActiveFilters(criteria, tags);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get vies: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error!", ex.Message, "OK");
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
                ActiveFilters.Add(new ActiveFilterChip($"Title: {criteria.TitleFilter}", () =>
                {
                    criteria.TitleFilter = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (!string.IsNullOrWhiteSpace(criteria.DescriptionFilter))
            {
                ActiveFilters.Add(new ActiveFilterChip($"Description: {criteria.DescriptionFilter}", () =>
                {
                    criteria.DescriptionFilter = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (!string.IsNullOrWhiteSpace(criteria.DirectorFilter))
            {
                ActiveFilters.Add(new ActiveFilterChip($"Director: {criteria.DirectorFilter}", () =>
                {
                    criteria.DirectorFilter = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (!string.IsNullOrWhiteSpace(criteria.ActorsFilter))
            {
                ActiveFilters.Add(new ActiveFilterChip($"Actors: {criteria.ActorsFilter}", () =>
                {
                    criteria.ActorsFilter = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (criteria.YearFrom != null || criteria.YearTo != null)
            {
                string label = $"Year: {criteria.YearFrom?.ToString() ?? "…"}-{criteria.YearTo?.ToString() ?? "…"}";
                ActiveFilters.Add(new ActiveFilterChip(label, () =>
                {
                    criteria.YearFrom = null;
                    criteria.YearTo = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            if (criteria.Favorite != null)
            {
                string label = criteria.Favorite == true ? "Favorites only" : "Non-favorites";
                ActiveFilters.Add(new ActiveFilterChip(label, () =>
                {
                    criteria.Favorite = null;
                    filterService.NotifyFilterChanged();
                }));
            }

            foreach (Guid tagId in criteria.TagIds.ToList())
            {
                string name = tags.FirstOrDefault(tag => tag.Id == tagId)?.Name ?? "?";
                ActiveFilters.Add(new ActiveFilterChip($"Tag: {name}", () =>
                {
                    criteria.TagIds.Remove(tagId);
                    filterService.NotifyFilterChanged();
                }));
            }

            OnPropertyChanged(nameof(HasActiveFilters));
        }
    }
}
