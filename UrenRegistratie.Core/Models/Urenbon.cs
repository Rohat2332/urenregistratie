namespace UrenRegistratie.Core.Models;

public class Urenbon
{
    private int id;
    private DateOnly date;

    public Urenbon(int id, DateOnly date)
    {
        this.id = id;
        this.date = date;
    }
}