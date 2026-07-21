using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MovieDatabase.Services;

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
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<FileService>();
            builder.Services.AddSingleton<DataService>();
            builder.Services.AddSingleton<MovieService>();
            builder.Services.AddSingleton<TagService>();

            builder.Services.AddSingleton<MoviesViewModel>();
            builder.Services.AddTransient<MovieDetailsViewModel>();

            builder.Services.AddTransient<DetailsPage>();

            var app = builder.Build();

            _ = app.Services.GetRequiredService<DataService>().EnsureLoadedAsync();

            return app;
        }
    }
}
