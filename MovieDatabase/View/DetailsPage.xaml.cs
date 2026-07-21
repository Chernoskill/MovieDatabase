namespace MovieDatabase;

public partial class DetailsPage : ContentPage
{
	bool? isAddMode;

	public DetailsPage(MovieDetailsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (BindingContext is not MovieDetailsViewModel viewModel)
		{
			return;
		}

		isAddMode ??= viewModel.Movie is null;

		if (isAddMode == true)
		{
			viewModel.StartNewMovie();
		}
	}
}