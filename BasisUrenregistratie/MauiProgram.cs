using System.Collections.Immutable;
using CommunityToolkit.Maui;
using HourRegistration.Core.Data;
using HourRegistration.Core.Data.Repositories;
using HourRegistration.Core.Interfaces.Repositories;
using HourRegistration.Core.Interfaces.Services;
using HourRegistration.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrenRegistratie.Core.Interfaces.Services;

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
        // builder.Services.AddTransient<ViewModels.FormPageViewModel>(); Uncomment this line to enable the FormPage
        builder.Services.AddTransient<ViewModels.TestViewModel>();
        
        
        
        //Views (Pages)
        builder.Services.AddTransient<Views.EmployeeOverview>();
        builder.Services.AddTransient<Views.TestView>();
        //builder.Services.AddTransient<Views.FormPageView>();  Change FormPage to FormPageView to enable the FormPage
        
        //App settings
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        
        return builder.Build();
    }
}