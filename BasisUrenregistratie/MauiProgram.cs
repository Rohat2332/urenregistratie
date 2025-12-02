using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace BasisUrenregistratie;

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
        builder.Services.AddSingleton<BasisUrenregistratie.ViewModels.EmployeeOverviewViewModel>();
        builder.Services.AddSingleton<BasisUrenregistratie.Views.EmployeeOverview>();

        return builder.Build();
    }
}