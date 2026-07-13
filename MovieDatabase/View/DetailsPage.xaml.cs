namespace MovieDatabase;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(MovieDetailsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}