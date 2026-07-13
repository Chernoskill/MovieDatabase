namespace MovieDatabase.Services
{
    public class MovieService
    {
        List<Movie>? movieList;
        public async Task<List<Movie>?> GetMovies()
        {
            if (movieList?.Count > 0)
            {
                return movieList;
            }

            try
            {
                // Offline
                using Stream stream = await FileSystem.OpenAppPackageFileAsync("moviedata.json");
                using StreamReader reader = new StreamReader(stream);
                string contents = await reader.ReadToEndAsync();
                movieList = JsonSerializer.Deserialize(contents, MovieContext.Default.ListMovie);
            }
            catch
            {

            }

            return movieList;
        }
    }
}