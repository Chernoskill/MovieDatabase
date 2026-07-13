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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<MovieService>();
            builder.Services.AddSingleton<MoviesViewModel>();

            builder.Services.AddTransient<MovieDetailsViewModel>();
            builder.Services.AddSingleton<DetailsPage>();

            return builder.Build();
        }
    }
}
