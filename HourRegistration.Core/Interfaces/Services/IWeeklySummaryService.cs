using System.Globalization;
using HourRegistration.Core.Models;

namespace HourRegistration.Core.Interfaces.Services;

public interface IWeeklySummaryService
{
    int CalculateStartOfTheWeek(DateTime day, CultureInfo culture);

    Dictionary<DateTime, (double TotalHours, int HourReceiptId)> AggregateHours(List<HourReceipt> hourReceipts,
        DateTime startOfTheWeek,
        DateTime endOfTheWeek);

    List<WeeklyDaySummary> GenerateWeekOverview(
        Dictionary<DateTime, (double TotalHours, int HourReceiptId)> aggregatedData, DateTime startOfTheWeek);
}