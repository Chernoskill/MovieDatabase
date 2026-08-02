using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    public partial class TagsViewModel : BaseViewModel
    {
        readonly TagService tagService;

        public ObservableCollection<TagListItem> Tags { get; } = new();

        [ObservableProperty]
        public partial string? NewTagName { get; set; }

        [ObservableProperty]
        public partial int NewTagColorIndex { get; set; }

        public TagsViewModel(TagService tagService)
        {
            Title = "Tags";
            this.tagService = tagService;
        }

        public async Task LoadAsync()
        {
            List<Tag> tags = await tagService.GetTags();

            Tags.Clear();

            foreach (Tag tag in tags)
            {
                Tags.Add(new TagListItem(tag, SaveTagAsync, DeleteTagAsync));
            }
        }

        [RelayCommand]
        async Task AddTagAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTagName))
            {
                return;
            }

            Tag tag = await tagService.AddTagAsync(NewTagName.Trim(), NewTagColorIndex);
            Tags.Add(new TagListItem(tag, SaveTagAsync, DeleteTagAsync));

            NewTagName = null;
            NewTagColorIndex = 0;
        }

        Task SaveTagAsync(TagListItem item, string name, int colorIndex)
        {
            return tagService.UpdateTagAsync(item.Tag, name, colorIndex);
        }

        async Task DeleteTagAsync(TagListItem item)
        {
            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete tag",
                $"Delete tag '{item.Name}'? It will be removed from all movies.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            await tagService.DeleteTagAsync(item.Tag);
            Tags.Remove(item);
        }
    }
}
