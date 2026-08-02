namespace MovieDatabase.View
{
    public partial class TagsPage : ContentPage
    {
        readonly TagsViewModel viewModel;

        public TagsPage(TagsViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = viewModel.LoadAsync();
        }
    }
}
