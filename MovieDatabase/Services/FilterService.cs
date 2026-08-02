namespace MovieDatabase.Services
{
    public class FilterService
    {
        public FilterCriteria Criteria { get; } = new();

        public event Action? FilterChanged;

        public void NotifyFilterChanged()
        {
            FilterChanged?.Invoke();
        }
    }
}
