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
        // Roep de async methode aan, de berekening gebeurt daar nu NA het laden
        if (_user != null)
        {
            InitializeData(_user.Id);
        }
    }
    private async void InitializeData(int userId)
    {
        try
        {
            // Wacht tot de data er is
            await LoadUserHourReceipts(userId);
    
            // Nu pas berekenen, want nu is HourReceipts gevuld
            CalculateCurrentWeek(_selectedDate, _cultureInfo);
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
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

    [RelayCommand]
    private async Task NavigateToFormPage(WeeklyDaySummary? summary)
    {
        if (summary == null) return;
        if (_user != null)
        {
            var hourReceipt = new HourReceipt()
            {
                UserId = _user.Id,
                Date = summary.Date,
                Status = "Concept"
            };

            // await _hourReceiptService.Add(hourReceipt);
        }


        await Shell.Current.GoToAsync(nameof(TestView));
    }
}