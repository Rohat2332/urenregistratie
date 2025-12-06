using System.Collections.ObjectModel;
using System.Globalization;
using UrenRegistratie.Core.Interfaces.Services;
using UrenRegistratie.Core.Models;

namespace BasisUrenregistratie.ViewModels;

/// <summary>
/// Represents a summary of a specific day within a weekly schedule.
/// </summary>
public class WeeklyDaySummary
{
    /// <summary>
    /// Gets or sets the date for this specific day entry.
    /// </summary>
    public DateTime Date { get; set; }
    public HourReceipt? HourReceipt { get; set; }
    
}

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
    private ObservableCollection<WeeklyDaySummary> WeekOverview { get; set; } = [];
    private DateTime StartOfTheWeek { get; set; }
    public DateTime EndOfTheWeek { get; private set; }
    

    public EmployeeOverviewViewModel(IHourReceiptService hourReceiptService, IUserService userService)
    {
        _hourReceiptService = hourReceiptService;
        _userService = userService;
        SetUser();
        if (_user != null) LoadUserHourReceipts(_user.Id);
        CalculateCurrentWeek();
    }

    private void LoadUserHourReceipts(int userId)
    {
        HourReceipts.Clear();
        foreach (var hourReceipt in _hourReceiptService.GetAll())
        {
            if (hourReceipt.UserId == userId)
            {
                HourReceipts.Add(hourReceipt);
            }
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

        // 3. Populate the WeekOverview collection for every day of the week (7 days)
        for (var i = 0; i < 7; i++)
        {
            var currentDay = StartOfTheWeek.AddDays(i);

            foreach (var hourReceipt in HourReceipts)
            {
                if (hourReceipt.Date == currentDay.Date)
                {
                    WeekOverview.Add(new WeeklyDaySummary
                    {
                        Date = currentDay,
                        HourReceipt = hourReceipt
                    });
                }
                else
                {
                    WeekOverview.Add(new WeeklyDaySummary
                    {
                        Date = currentDay
                    });
                }
            }
        }
    }

    /// <summary>
    /// Calculates the start of the week with given day.
    /// </summary>
    /// <param name="day"> DateTime</param>
    /// <param name="culture">CultureInfo</param>
    /// <returns>int - Returns an int with the days since monday </returns>
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