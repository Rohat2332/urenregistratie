namespace HourRegistration.Core.Models;

/// <summary>
/// Represents a summary of a specific day within a weekly schedule.
/// </summary>
public class WeeklyDaySummary
{
    public int Id;
    public DateTime Date { get; set; }
    public double TotalHours { get; set; }
    public bool IsDayEmpty => TotalHours < 0.001 && Id.Equals(null);
}