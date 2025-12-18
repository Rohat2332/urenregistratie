namespace HourRegistration.Core.Models;

/// <summary>
/// Represents a summary of a specific day within a weekly schedule.
/// </summary>
public class WeeklyDaySummary
{
    public int? Id;
    public DateTime Date { get; set; }
    public double TotalHours { get; set; }
    public bool IsDayEmpty => Id is null or 0;
}