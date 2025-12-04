namespace UrenRegistratie.Core.Models;

public class HourReceipt
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public string Status { get; set; }
    public int HoursWorked { get; set; }
    public int MinutesWorked { get; set; }
    public string Remark { get; set; }
    public DateTime Date {get; set;}
    

    public HourReceipt(int id,int userId, DateTime date, string status)
    {
        Id = id;
        UserId = userId;
        Date = date;
        Status = status;
    }
}