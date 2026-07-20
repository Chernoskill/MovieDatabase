namespace MovieDatabase.Services
{
    public class MovieService
    {
        readonly DataService dataService;

        public MovieService(DataService dataService)
        {
            this.dataService = dataService;
        }

        public Task<List<Movie>> GetMovies()
        {
            return Task.FromResult(dataService.Movies);
        }
    }
}
