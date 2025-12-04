using CommunityToolkit.Maui;
using HourRegistartion.Core.Data.Repositories;
using Microsoft.Extensions.Logging;
using UrenRegistratie.Core.Interfaces.Repositories;
using UrenRegistratie.Core.Interfaces.Services;
using UrenRegistratie.Core.Services;

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
        // --- Dependency Injection Configuration ---
        
        //Repositories (Data Access)
        builder.Services.AddSingleton<IHourReceiptRepository, HourReceiptsRepository>();
        builder.Services.AddSingleton<IUserRepository, UserRepository>();
        
        //Services (Business Logic)
        builder.Services.AddSingleton<IHourReceiptService, HourReceiptService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        
        // ViewModels (Data Presentation Logic)
        builder.Services.AddTransient<ViewModels.EmployeeOverviewViewModel>();
        
        //Views (Pages)
        builder.Services.AddTransient<Views.EmployeeOverview>();

        return builder.Build();
    }
}