namespace MovieDatabase.Services
{
    public class TagService
    {
        readonly DataService dataService;

        public TagService(DataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<List<Tag>> GetTags()
        {
            await dataService.EnsureLoadedAsync();
            return dataService.Tags;
        }
    }
}
