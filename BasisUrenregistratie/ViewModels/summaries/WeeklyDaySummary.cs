using UrenRegistratie.Core.Models;

namespace BasisUrenregistratie.ViewModels.summaries;

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