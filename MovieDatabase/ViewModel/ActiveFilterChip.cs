namespace MovieDatabase.ViewModel
{
    public partial class ActiveFilterChip : ObservableObject
    {
        readonly Action clear;

        public string Label { get; }

        public ActiveFilterChip(string label, Action clear)
        {
            Label = label;
            this.clear = clear;
        }

        [RelayCommand]
        void Clear()
        {
            clear();
        }
    }
}
