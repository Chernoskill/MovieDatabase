using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    public partial class MoviesViewModel : BaseViewModel
    {
        public ObservableCollection<Movie> Movies { get; } = new();
        MovieService movieService;

        public MoviesViewModel(MovieService movieService)
        {
            Title = "Movie Database";
            this.movieService = movieService;
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
