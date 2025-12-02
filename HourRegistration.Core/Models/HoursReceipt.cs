namespace UrenRegistratie.Core.Models;

public class HoursReceipt
{
    public DateTime Date {get; set;}
    

    public HoursReceipt(DateTime date)
    {
        Date = date;
    }
}