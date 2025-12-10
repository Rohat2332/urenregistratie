using BasisUrenregistratie.ViewModels;

namespace BasisUrenregistratie.Views;

public partial class EmployeeOverview : ContentPage
{
    public EmployeeOverview(EmployeeOverviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}