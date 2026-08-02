namespace MovieDatabase
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
            Routing.RegisterRoute(nameof(FilterPage), typeof(FilterPage));

            Navigated += OnShellNavigated;
        }

        void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
        {
            if (e.Source != ShellNavigationSource.ShellSectionChanged && e.Source != ShellNavigationSource.ShellItemChanged)
            {
                return;
            }

            ShellSection? section = CurrentItem?.CurrentItem;

            if (section?.CurrentItem?.Route != "MainPage" || section.Stack.Count <= 1)
            {
                return;
            }

            // Shell still holds its navigation lock while Navigated handlers run, so popping
            // synchronously here throws ("Only one operation can be active at a time"). Deferring
            // to the next dispatcher cycle lets the in-flight tab-switch navigation finish first.
            Dispatcher.Dispatch(async () =>
            {
                try
                {
                    await section.Navigation.PopToRootAsync(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to reset Movies tab to root: {ex}");
                }
            });
        }
    }
}
