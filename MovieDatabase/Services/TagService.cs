namespace MovieDatabase.Services
{
    public class TagService
    {
        readonly DataService dataService;

        public TagService(DataService dataService)
        {
            this.dataService = dataService;
        }

        public Task<List<Tag>> GetTags()
        {
            return Task.FromResult(dataService.Tags);
        }
    }
}
