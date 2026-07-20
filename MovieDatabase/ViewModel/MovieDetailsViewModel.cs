using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    [QueryProperty(nameof(Movie), "Movie")]
    public partial class MovieDetailsViewModel : BaseViewModel
    {
        readonly MovieService movieService;

        [ObservableProperty]
        public required partial Movie Movie { get; set; }

        public MovieDetailsViewModel(MovieService movieService)
        {
            this.movieService = movieService;
        }

        [RelayCommand]
        Task ToggleFavoriteAsync()
        {
            return movieService.ToggleFavoriteAsync(Movie);
        }
    }
}
