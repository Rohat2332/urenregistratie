using BasisUrenregistratie.ViewModels;
using BasisUrenregistratie.Views;

namespace BasisUrenregistratie;

public partial class App : Application
{
    public App(EmployeeOverviewViewModel viewModel)
    {
        InitializeComponent();

        MainPage = new EmployeeOverview(viewModel);
    }
}