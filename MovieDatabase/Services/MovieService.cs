namespace MovieDatabase.Services
{
    public class MovieService
    {
        readonly DataService dataService;

        public MovieService(DataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<List<Movie>> GetMovies()
        {
            await dataService.EnsureLoadedAsync();
            return dataService.Movies;
        }

        public Task ToggleFavoriteAsync(Movie movie)
        {
            movie.Favorite = !movie.Favorite;
            return dataService.SaveAsync();
        }

        public Task DeleteMoviesAsync(IEnumerable<Movie> movies)
        {
            foreach (Movie movie in movies.ToList())
            {
                dataService.Movies.Remove(movie);
            }

            return dataService.SaveAsync();
        }
    }
}
