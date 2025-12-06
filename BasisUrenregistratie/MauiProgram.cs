using CommunityToolkit.Maui;
using HourRegistartion.Core.Data.Repositories;
using HourRegistration.Core.Data;
using HourRegistration.Core.Data.Repositories;
using Microsoft.Extensions.Configuration;
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
        // Database
        builder.Services.AddSingleton<DatabaseConnection>();
        
        //Repositories (Data Access)
        builder.Services.AddSingleton<IHourReceiptRepository, HourReceiptsRepository>();
        builder.Services.AddSingleton<IUserRepository, UserRepository>();
        
        //Services (Business Logic)
        builder.Services.AddSingleton<IHourReceiptService, HourReceiptService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IConfigurationService, ConfigurationService > ();
        builder.Services.AddSingleton<IWeeklySummaryService, WeeklySummaryService>();
        
        // ViewModels (Data Presentation Logic)
        builder.Services.AddTransient<ViewModels.EmployeeOverviewViewModel>();
        
        //Views (Pages)
        builder.Services.AddTransient<Views.EmployeeOverview>();
        
        //App settings
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        
        return builder.Build();
    }
}