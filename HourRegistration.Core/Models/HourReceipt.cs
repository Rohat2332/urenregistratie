namespace UrenRegistratie.Core.Models;

public class HourReceipt
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public string? Status { get; set; }
    public int HoursWorked { get; set; }
    public int MinutesWorked { get; set; }
    public string? Remark { get; set; }
    public DateTime Date {get; set;}

    public HourReceipt(int id, int userId, int projectId, string status, int hoursWorked, int minutesWorked,string remark, DateTime date)
    {
        Id = id;
        UserId = userId;
        ProjectId = projectId;
        Status = status;
        HoursWorked = hoursWorked;
        MinutesWorked = minutesWorked;
        Remark = remark;
        Date = date;
    }

    
}