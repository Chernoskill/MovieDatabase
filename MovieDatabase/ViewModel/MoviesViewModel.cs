using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    public partial class MoviesViewModel : BaseViewModel
    {
        public ObservableCollection<Movie> Movies { get; } = new();
        public ObservableCollection<Movie> MarkedMovies { get; } = new();
        readonly MovieService movieService;

        public MoviesViewModel(MovieService movieService)
        {
            Title = "Movie Database";
            this.movieService = movieService;
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

                if (Movies.Count != 0)
                {
                    Movies.Clear();
                }

                if (movies != null)
                {
                    foreach (Movie movie in movies)
                    {
                        Movies.Add(movie);
                    }
                }
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
    }
}
