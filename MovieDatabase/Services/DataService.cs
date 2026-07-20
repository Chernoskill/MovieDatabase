namespace MovieDatabase.Services
{
    public class DataService
    {
        const string DataFileName = "moviedata.json";

        readonly FileService fileService;

        public List<Movie> Movies { get; private set; } = new();
        public List<Tag> Tags { get; private set; } = new();

        public DataService(FileService fileService)
        {
            this.fileService = fileService;
        }

        public async Task LoadAsync()
        {
            try
            {
                string? contents = await fileService.LoadFileAsync(DataFileName);

                if (contents == null)
                {
                    using Stream stream = await FileSystem.OpenAppPackageFileAsync(DataFileName);
                    using StreamReader reader = new StreamReader(stream);
                    contents = await reader.ReadToEndAsync();
                }

                MovieDatabaseData? data = JsonSerializer.Deserialize(contents, MovieDatabaseDataContext.Default.MovieDatabaseData);

                if (data != null)
                {
                    Movies = data.Movies;
                    Tags = data.Tags;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to load movie database data: {ex}");
            }
        }
    }
}
