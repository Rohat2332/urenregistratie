using BasisUrenregistratie.ViewModels;

namespace BasisUrenregistratie.Views;

public partial class EmployeeOverview : ContentPage
{
    private readonly EmployeeOverviewViewModel _viewModel;
    public EmployeeOverview(EmployeeOverviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    /// <summary>
    /// Handles the logic executed when the EmployeeOverview page appears.
    /// Overrides the base OnAppearing method and engages the ViewModel to refresh the displayed data.
    /// Displays an alert in case of exceptions during data loading.
    /// </summary>
    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();
        
            await _viewModel.RefreshDataAsync();
        }
        catch (Exception e)
        {
            await DisplayAlert("Error", "Failed to load employee data. Please try again.", "OK");
            System.Diagnostics.Debug.WriteLine($"Error in EmployeeOverview.OnAppearing: {e.Message}");
        }
    }
}