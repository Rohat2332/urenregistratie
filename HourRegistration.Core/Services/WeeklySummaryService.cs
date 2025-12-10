using System.Globalization;
using HourRegistration.Core.Models;
using UrenRegistratie.Core.Interfaces.Services;

namespace HourRegistration.Core.Services;

/// <summary>
/// Provides services for generating and aggregating weekly summaries based on provided hour receipts.
/// </summary>
public class WeeklySummaryService : IWeeklySummaryService
{
    /// <summary>
    /// Calculates the number of days since the start of the week for a given date,
    /// based on the specified culture's week start configuration.
    /// </summary>
    /// <param name="day">The date for which the calculation is performed.</param>
    /// <param name="culture">The culture that defines the starting day of the week.</param>
    /// <returns>The number of days has elapsed since the start of the week as an integer.</returns>
    public int CalculateStartOfTheWeek(DateTime day, CultureInfo culture)
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

    /// <summary>
    /// Aggregates the worked hours from the provided list of hour receipts within the specified date range.
    /// </summary>
    /// <param name="hourReceipts">The list of hour receipts containing the hours worked data.</param>
    /// <param name="startOfTheWeek">The start date of the week for which to aggregate hours.</param>
    /// <param name="endOfTheWeek">The end date of the week for which to aggregate hours.</param>
    /// <returns>A dictionary mapping each date within the specified range to the total number of hours worked on that date.</returns>
    public Dictionary<DateTime, double> AggregateHours(List<HourReceipt> hourReceipts, DateTime startOfTheWeek,
        DateTime endOfTheWeek)
    {
        // Groepeer en aggregeer de uren in de geladen bonnen (HourReceipts)
        var aggregatedHours = hourReceipts
            .Where(h => h.Date.Date >= startOfTheWeek.Date && h.Date.Date <= endOfTheWeek.Date)
            .GroupBy(h => h.Date.Date) // Groepeer op datum
            .ToDictionary(g => g.Key, 
                // Sum de HoursWorked en MinutesWorked
                g => g.Sum(h => h.HoursWorked + (h.MinutesWorked / 60.0)));
        
        return aggregatedHours;
    }

    /// <summary>
    /// Generates a weekly overview summarizing the hours for each day within the week,
    /// starting from the specified start date.
    /// </summary>
    /// <param name="aggregatedHours">A dictionary containing the total hours worked for each date.</param>
    /// <param name="startOfTheWeek">The start date of the week for which the overview is being generated.</param>
    /// <returns>A list of <see cref="WeeklyDaySummary"/> objects, each representing a summary of hours for a specific day.</returns>
    public List<WeeklyDaySummary> GenerateWeekOverview(Dictionary<DateTime, double> aggregatedHours, DateTime startOfTheWeek)
    {
        var summaries = new List<WeeklyDaySummary>();
        
        for (var i = 0; i < 7; i++)
        {
            var currenDay = startOfTheWeek.AddDays(i);
            
            aggregatedHours.TryGetValue(currenDay.Date, out var totalHours);
            
            summaries.Add(new WeeklyDaySummary
            {
                Date = currenDay,
                TotalHours = totalHours
            });
        }
        return summaries;
    }
}