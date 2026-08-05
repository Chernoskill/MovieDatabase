using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MovieDatabase.Localization;
using MovieDatabase.Services;
using MovieDatabase.View;

namespace MovieDatabase
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIconsOutlined-Regular.otf", "MaterialIconsOutlined");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // LocalizationResourceManager.Instance also initializes the active language (from the
            // saved preference, falling back to the device locale) as a side effect of first access.
            builder.Services.AddSingleton(LocalizationResourceManager.Instance);

            builder.Services.AddSingleton<FileService>();
            builder.Services.AddSingleton<DataService>();
            builder.Services.AddSingleton<MovieService>();
            builder.Services.AddSingleton<TagService>();
            builder.Services.AddSingleton<FilterService>();

            builder.Services.AddSingleton<MoviesViewModel>();
            builder.Services.AddTransient<MovieDetailsViewModel>();
            builder.Services.AddTransient<FilterViewModel>();
            builder.Services.AddTransient<OptionsViewModel>();

            builder.Services.AddTransient<DetailsPage>();
            builder.Services.AddTransient<FilterPage>();
            builder.Services.AddTransient<OptionsPage>();

            var app = builder.Build();

            _ = app.Services.GetRequiredService<DataService>().EnsureLoadedAsync();

            return app;
        }
    }
}
