namespace MovieDatabase;

public partial class FilterPage : ContentPage
{
    readonly FilterViewModel viewModel;

    public FilterPage(FilterViewModel viewModel)
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
