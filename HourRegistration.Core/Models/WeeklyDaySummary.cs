namespace UrenRegistratie.Core.Models;

/// <summary>
/// Represents a summary of a specific day within a weekly schedule.
/// </summary>
public class WeeklyDaySummary
{
    public DateTime Date { get; set; }
    public double TotalHours { get; set; }
}