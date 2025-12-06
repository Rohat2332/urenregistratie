using System.Collections.ObjectModel;
using System.Globalization;
using BasisUrenregistratie.ViewModels.summaries;
using UrenRegistratie.Core.Interfaces.Services;
using UrenRegistratie.Core.Models;

namespace BasisUrenregistratie.ViewModels;

/// <summary>
/// ViewModel responsible for the Employee Overview.
/// Manages the logic for calculating week ranges and displaying daily summaries.
/// </summary>
public class EmployeeOverviewViewModel
{
    private readonly IHourReceiptService _hourReceiptService;
    private readonly IUserService _userService;
    private User? _user;
    private ObservableCollection<HourReceipt> HourReceipts { get; set; } = [];
    public ObservableCollection<WeeklyDaySummary> WeekOverview { get; set; } = [];
    private DateTime StartOfTheWeek { get; set; }
    public DateTime EndOfTheWeek { get; private set; }
    

    public EmployeeOverviewViewModel(IHourReceiptService hourReceiptService, IUserService userService)
    {
        _hourReceiptService = hourReceiptService;
        _userService = userService;
        SetUser();
        if (_user != null)
        {
            _ = LoadUserHourReceipts(_user.Id);
        }
        CalculateCurrentWeek();
    }

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

    private void SetUser()
    {
        _user = _userService.GetById(0);
    }

    /// <summary>
    /// Calculates the start and end dates of the current week based on Dutch culture settings (Monday start),
    /// and populates the WeekOverview collection.
    /// </summary>
    private void CalculateCurrentWeek()
    {
        WeekOverview.Clear();

        // Determine the culture object to ensure the week starts on Monday (standard in NL)
        var culture = new CultureInfo("nl-NL");

        // 1. Calculate the start of the current week (Monday)
        var today = DateTime.Today;

        var daysSinceMonday = CalculateStartOfTheWeek(today, culture);

        // Set the start (Monday) and end (Sunday) of the week
        StartOfTheWeek = today.AddDays(-daysSinceMonday);
        EndOfTheWeek = StartOfTheWeek.AddDays(6);

        // --- DATA AGGREGATIE ---
        // Groepeer en aggregeer de uren in de geladen bonnen (HourReceipts)
        var aggregatedHours = AggregateHours();

        // 3. Populate the WeekOverview collection for every day of the week (7 days)
        PopulateWeekOverview(aggregatedHours);
        
    }

    /// <summary>
    /// Aggregates the worked hours from the loaded receipts for the current week.
    /// </summary>
    /// <returns>A dictionary mapping each date to the total number of hours worked on that date.</returns>

    private Dictionary<DateTime, double> AggregateHours()
    {
        // Groepeer en aggregeer de uren in de geladen bonnen (HourReceipts)
        var aggregatedHours = HourReceipts
            .Where(h => h.Date.Date >= StartOfTheWeek.Date && h.Date.Date <= EndOfTheWeek.Date)
            .GroupBy(h => h.Date.Date) // Groepeer op datum
            .ToDictionary(g => g.Key, 
                // Sum de HoursWorked en MinutesWorked
                g => g.Sum(h => h.HoursWorked + (h.MinutesWorked / 60.0)));
        
        return aggregatedHours;
    }

    /// <summary>
    /// Populates the WeekOverview collection with a summary for each day of the current week.
    /// </summary>
    /// <param name="aggregatedHours">A dictionary containing dates and their corresponding total hours worked.</param>
    private void PopulateWeekOverview(Dictionary<DateTime, double> aggregatedHours)
    {
        // 3. Populate the WeekOverview collection for every day of the week (7 days)
        for (var i = 0; i < 7; i++)
        {
            var currentDay = StartOfTheWeek.AddDays(i);
            double totalHours = 0;
            
            // Zoek geaggregeerde uren op, standaard 0 als er niets is gevonden.
            aggregatedHours.TryGetValue(currentDay.Date, out totalHours);

            // Voeg EEN ENKELE WeeklyDaySummary toe voor de dag
            WeekOverview.Add(new WeeklyDaySummary
            {
                Date = currentDay,
                // TotalHours = totalHours
            });
        }
    }

    /// <summary>
    /// Calculates the number of days since Monday for a given date,
    /// considering the specified culture's week start settings.
    /// </summary>
    /// <param name="day">The date for which the calculation is performed.</param>
    /// <param name="culture">The culture whose first day of the week setting will be applied.</param>
    /// <returns>The number of days since Monday as an integer.</returns>
    private static int CalculateStartOfTheWeek(DateTime day, CultureInfo culture)
    {
        var daysSinceMonday = (int)day.DayOfWeek - (int)culture.DateTimeFormat.FirstDayOfWeek;

        /*
         Handle the Sunday edge case:
        In the DayOfWeek enum, Sunday is 0 and Monday is 1.
        If today is Sunday (0) and the week starts on Monday (1), the math above yields -1.
        However, physically, Sunday is 6 days after the previous Monday.
        */
        if (day.DayOfWeek == DayOfWeek.Sunday && culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Monday)
        {
            daysSinceMonday = 6;
        }
        else if (daysSinceMonday < 0)
        {
            // Adjust for any other systems/cultures where the calculation might result in a negative offset
            daysSinceMonday += 7;
        }

        return daysSinceMonday;
    }
}