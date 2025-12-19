using HourRegistration.Core.Interfaces.Repositories;
using UrenRegistratie.Core.Interfaces.Services;

namespace BasisUrenregistratie.ViewModels;

[QueryProperty(nameof(ReceiptId), "receiptId")]
public partial class TestViewModel
{
    private readonly IHourReceiptService _hourReceiptService;
    public int ReceiptId { get; set; }

    public TestViewModel(IHourReceiptService hourReceiptService)
    {
        _hourReceiptService = hourReceiptService;
    }

    public async Task LoadHourReceipt(int id)
    {
        var hourReceipt = await _hourReceiptService.GetById(id);

        // if (hourReceipt != null)
        // {
        //     SelectedDate = hourReceipt.Date;
        //     
        // }
    }
    
}