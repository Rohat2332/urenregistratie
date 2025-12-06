using System.Globalization;
using UrenRegistratie.Core.Models;

namespace UrenRegistratie.Core.Interfaces.Services;

public interface IWeeklySummaryService
{
    int CalculateStartOfTheWeek(DateTime day, CultureInfo culture);

    Dictionary<DateTime, double> AggregateHours(List<HourReceipt> hourReceipts, DateTime startOfTheWeek,
        DateTime endOfTheWeek);

    List<WeeklyDaySummary> GenerateWeekOverview(Dictionary<DateTime, double> aggregatedHours, DateTime startOfTheWeek);
}