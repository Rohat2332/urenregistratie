using System;
using Microsoft.Maui.Controls;
using MySqlConnector;

namespace BasisUrenregistratie.Views;

public partial class FormPage : ContentView
{
    private const string ConnStr =
        "Server=127.0.0.1;Port=3306;Database=urenregistratie;User ID=root;Password=;";

    public FormPage()
    {
        InitializeComponent();
        WorkDatePicker.Date = DateTime.Today;
        MinutesPicker.SelectedIndex = 0;
    }

    
    
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(HoursEntry.Text))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Voer uren in", "OK");
            return;
        }
        if (!int.TryParse(HoursEntry.Text, out var hours) || hours < 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Ongeldige uren", "OK");
            return;
        }
        if (MinutesPicker.SelectedIndex < 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Selecteer minuten (15/30/45)", "OK");
            return;
        }

        var minutesText = MinutesPicker.SelectedItem?.ToString();
        if (!int.TryParse(minutesText, out var extraMinutes))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Ongeldige minuten", "OK");
            return;
        }

        var totalMinutes = hours * 60 + extraMinutes;
        var workDate = WorkDatePicker.Date.Date;
        var remarks = string.IsNullOrWhiteSpace(RemarksEntry.Text) ? null : RemarksEntry.Text.Trim();

        try
        {
            await using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            const string sql = @"
INSERT INTO urenbon
(werknemer_id, datum, aantal_uren, opdrachtgever_id, project_id, status, opmerkingen)
VALUES
(1, @datum, @aantal_uren, 1, 1, 1, @opmerkingen);";

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@datum", workDate);
            cmd.Parameters.AddWithValue("@aantal_uren", totalMinutes);
            cmd.Parameters.AddWithValue("@opmerkingen", (object?)remarks ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();

            await Application.Current.MainPage.DisplayAlert(
                "Success",
                $"Opgeslagen: {hours}u + {extraMinutes}m = {totalMinutes} minuten",
                "OK");

            HoursEntry.Text = string.Empty;
            MinutesPicker.SelectedIndex = -1;
            RemarksEntry.Text = string.Empty;
            WorkDatePicker.Date = DateTime.Today;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
