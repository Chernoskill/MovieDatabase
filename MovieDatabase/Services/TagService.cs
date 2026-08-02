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

        public async Task<Tag> AddTagAsync(string name, int colorIndex)
        {
            await dataService.EnsureLoadedAsync();

            Tag tag = new Tag(name, colorIndex);
            dataService.Tags.Add(tag);
            await dataService.SaveAsync();

            return tag;
        }

        public async Task UpdateTagAsync(Tag tag, string name, int colorIndex)
        {
            await dataService.EnsureLoadedAsync();

            tag.Name = name;
            tag.ColorIndex = colorIndex;
            await dataService.SaveAsync();
        }

        public async Task DeleteTagAsync(Tag tag)
        {
            await dataService.EnsureLoadedAsync();

            dataService.Tags.Remove(tag);

            foreach (Movie movie in dataService.Movies)
            {
                movie.Tags.Remove(tag.Id);
            }

            await dataService.SaveAsync();
        }
    }
}
