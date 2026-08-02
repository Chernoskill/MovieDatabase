namespace MovieDatabase.View
{
    public partial class OptionsPage : ContentPage
    {
        readonly OptionsViewModel viewModel;

        public OptionsPage(OptionsViewModel viewModel)
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
