using BasisUrenregistratie.ViewModels;
using BasisUrenregistratie.Views;

namespace BasisUrenregistratie;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}