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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = viewModel.GetMoviesCommand.ExecuteAsync(null);
        }
    }
}
