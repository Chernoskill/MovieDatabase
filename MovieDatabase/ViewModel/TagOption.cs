namespace MovieDatabase.ViewModel
{
    public partial class TagOption : ObservableObject
    {
        public Guid Id { get; }
        public string Name { get; }

        [ObservableProperty]
        public partial bool IsSelected { get; set; }

        public TagOption(Tag tag, bool isSelected)
        {
            Id = tag.Id;
            Name = tag.Name;
            IsSelected = isSelected;
        }
    }
}
