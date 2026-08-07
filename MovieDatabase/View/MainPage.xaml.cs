namespace MovieDatabase.View
{
    public partial class MainPage : ContentPage
    {
        readonly MoviesViewModel viewModel;

        public MainPage(MoviesViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.GetMoviesCommand.ExecuteAsync(null);

            // Restore the scroll offset the user had before leaving MainPage (e.g. into DetailsPage
            // and back). Dispatched a cycle later so the CollectionView has finished laying out the
            // freshly (re)bound items before we ask it to scroll.
            if (viewModel.LastVisibleIndex > 0)
            {
                Dispatcher.Dispatch(() =>
                {
                    if (viewModel.LastVisibleIndex < viewModel.Movies.Count)
                    {
                        MoviesCollectionView.ScrollTo(viewModel.LastVisibleIndex, position: ScrollToPosition.Start, animate: false);
                    }
                });
            }
        }

        void OnCollectionViewScrolled(object? sender, ItemsViewScrolledEventArgs e)
        {
            viewModel.LastVisibleIndex = e.FirstVisibleItemIndex;
        }
    }
}
