using System.Collections.Specialized;

namespace MovieDatabase.Model
{
    // ObservableCollection<T> has no built-in way to swap its whole contents in one notification -
    // Add/Remove/Clear each raise their own CollectionChanged event, which is expensive when a
    // CollectionView is bound to a list that gets rebuilt wholesale (e.g. after filtering/sorting).
    // ReplaceRange rebuilds the contents and raises a single Reset notification instead.
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        public void ReplaceRange(IEnumerable<T> items)
        {
            Items.Clear();

            foreach (T item in items)
            {
                Items.Add(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
