using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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