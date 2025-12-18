using BasisUrenregistratie.Views;

namespace BasisUrenregistratie;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(TestView), typeof(TestView));
        Routing.RegisterRoute(nameof(EmployeeOverview), typeof(EmployeeOverview));
        // Routing.RegisterRoute(nameof(FormPageView), typeof(FormPageView)); uncomment to enable the FormPage
    }
}