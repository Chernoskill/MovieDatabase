namespace MovieDatabase.ViewModel
{
    [QueryProperty(nameof(Movie), "Movie")]
    public partial class MovieDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public required partial Movie Movie { get; set; }

        public MovieDetailsViewModel()
        {

        }
    }
}
