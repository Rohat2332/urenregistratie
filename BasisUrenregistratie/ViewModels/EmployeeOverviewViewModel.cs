using System.Collections.ObjectModel;
using System.Globalization;
using BasisUrenregistratie.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HourRegistration.Core.Interfaces.Services;
using HourRegistration.Core.Models;
using UrenRegistratie.Core.Interfaces.Services;

namespace BasisUrenregistratie.ViewModels;

/// <summary>
/// ViewModel managing the employee overview and its presentation logic.
/// Handles user data retrieval, weekly calculations, and binds to the UI.
/// </summary>
public partial class EmployeeOverviewViewModel : ObservableObject
{
    private readonly IHourReceiptService _hourReceiptService;
    private readonly IUserService _userService;
    private readonly IWeeklySummaryService _weeklySummaryService;
    private User? _user;
    private readonly DateTime _selectedDate = DateTime.Today;
    private readonly CultureInfo _cultureInfo = new ("nl-NL");
    private ObservableCollection<HourReceipt> HourReceipts { get; set; } = [];
    public ObservableCollection<WeeklyDaySummary> WeekOverview { get; set; } = [];
    private DateTime StartOfTheWeek { get; set; }
    private DateTime EndOfTheWeek { get; set; }
    

    public EmployeeOverviewViewModel(IHourReceiptService hourReceiptService, IUserService userService, IWeeklySummaryService weeklySummaryService)
    {
        _hourReceiptService = hourReceiptService;
        _userService = userService;
        _weeklySummaryService = weeklySummaryService;
        SetUser();
        
        if (_user != null)
        {
            InitializeData(_user.Id);
        }
    }

    /// <summary>
    /// Initializes the data for the employee overview by loading the user hour receipts
    /// and calculating the weekly summary for the current week based on the provided user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom data is to be initialized.</param>
    private async void InitializeData(int userId)
    {
        try
        {
            await LoadUserHourReceipts(userId);
            
            CalculateCurrentWeek(_selectedDate, _cultureInfo);
        }
        catch (InvalidOperationException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Operation error in InitializeData: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Invalid argument in InitializeData: {ex.Message}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Unexpected error in InitializeData: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously refreshes data in the employee overview, including loading user hour receipts
    /// and calculating the weekly summary for the selected date.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation of refreshing data.</returns>
    public async Task RefreshDataAsync()
    {
        if (_user == null) return;
    
        // Roep je bestaande laad-methoden aan
        await LoadUserHourReceipts(_user.Id);
        CalculateCurrentWeek(_selectedDate, _cultureInfo);
    }
    

    /// <summary>
    /// Asynchronously loads all hour receipts for a specific user and updates the current collection.
    /// Filters received hour receipts to include only those that match the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose hour receipts are to be loaded.</param>
    /// <returns>A task that represents the asynchronous operation of loading hour receipts.</returns>
    private async Task LoadUserHourReceipts(int userId)
    {
        try
        {
            HourReceipts.Clear();
            var allHourReceipts = await _hourReceiptService.GetAll();
            
            
            foreach (var hourReceipt in (allHourReceipts).Where(hourReceipt => hourReceipt.UserId == userId))
            {
                HourReceipts.Add(hourReceipt);
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading hour receipts: {e.Message}");
        }
    }

    /// <summary>
    /// Retrieves and sets the current user by invoking the user service.
    /// Defaults to a user with ID 0 if no specific user context is provided.
    /// </summary>
    private void SetUser()
    {
        _user = new User(0, "user1", "password123");
    }

    /// <summary>
    /// Calculates the start and end dates of the current week based on Dutch culture settings (Monday start)
    /// and populates the WeekOverview collection.
    /// </summary>
    private void CalculateCurrentWeek(DateTime selectedDate, CultureInfo culture)
    {
        WeekOverview.Clear();
        
        var daysSinceMonday = _weeklySummaryService.CalculateStartOfTheWeek(selectedDate, culture);

        // Set the start (Monday) and end (Sunday) of the week
        StartOfTheWeek = selectedDate.AddDays(-daysSinceMonday);
        EndOfTheWeek = StartOfTheWeek.AddDays(6);
        
        // Groepeer en aggregeer de uren in de geladen bonnen (HourReceipts)
        var aggregatedData = _weeklySummaryService.AggregateHours(HourReceipts.ToList(), StartOfTheWeek, EndOfTheWeek);

        // 3. Populate the WeekOverview collection for every day of the week (7 days)
        var summaries = _weeklySummaryService.GenerateWeekOverview(aggregatedData, StartOfTheWeek);

        foreach (var summary in summaries)
        {
            WeekOverview.Add(summary);
        }
    }

    /// <summary>
    /// Navigates to the form page for the given weekly day summary. If the summary does not exist,
    /// creates a new hour receipt entry and navigates to the form page associated with it.
    /// </summary>
    /// <param name="summary">The weekly day summary to be navigated to. If null, navigation is not performed.</param>
    /// <returns>A task that represents the asynchronous navigation operation.</returns>
    [RelayCommand]
    private async Task NavigateToFormPage(WeeklyDaySummary? summary)
    {
        if (summary == null) return;

        var targetId = 0;
        
        // Check if the summary is a new entry or an existing one
        if (summary.Id.HasValue && summary.Id.Value > 0)
        {
            targetId = summary.Id.Value;
        }
        else
        {
            // Create a new entry
            if (_user != null)
            {
                var hourReceipt = new HourReceipt()
                {
                    UserId = _user.Id,
                    Date = summary.Date,
                    Status = "Concept"
                };
            
                await _hourReceiptService.Add(hourReceipt);
                
                var allUserHourReceipts = await _hourReceiptService.GetAll();
                targetId = allUserHourReceipts.Where(x => x.UserId == _user.Id && x.Date.Date == summary.Date.Date)
                    .OrderByDescending(x => x.Id)
                    .First().Id;
            }
        }
        //Todo: Change to form page
        await Shell.Current.GoToAsync($"{nameof(TestView)}?ReceiptId={targetId}");
    }
}