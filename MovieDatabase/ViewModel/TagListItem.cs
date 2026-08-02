namespace MovieDatabase.ViewModel
{
    public partial class TagListItem : ObservableObject
    {
        readonly Func<TagListItem, string, int, Task> save;
        readonly Func<TagListItem, Task> delete;

        public Tag Tag { get; }

        [ObservableProperty]
        public partial string Name { get; set; }

        [ObservableProperty]
        public partial int ColorIndex { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotEditing))]
        public partial bool IsEditing { get; set; }

        [ObservableProperty]
        public partial string? EditName { get; set; }

        [ObservableProperty]
        public partial int EditColorIndex { get; set; }

        public bool IsNotEditing => !IsEditing;

        public TagListItem(Tag tag, Func<TagListItem, string, int, Task> save, Func<TagListItem, Task> delete)
        {
            Tag = tag;
            Name = tag.Name;
            ColorIndex = tag.ColorIndex;
            this.save = save;
            this.delete = delete;
        }

        [RelayCommand]
        void Edit()
        {
            EditName = Name;
            EditColorIndex = ColorIndex;
            IsEditing = true;
        }

        [RelayCommand]
        async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(EditName))
            {
                return;
            }

            string name = EditName.Trim();
            await save(this, name, EditColorIndex);

            Name = name;
            ColorIndex = EditColorIndex;
            IsEditing = false;
        }

        [RelayCommand]
        void Cancel()
        {
            IsEditing = false;
        }

        [RelayCommand]
        Task DeleteAsync()
        {
            return delete(this);
        }
    }
}
