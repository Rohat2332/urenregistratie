using System.Globalization;
using FluentAssertions;
using HourRegistration.Core.Models;
using HourRegistration.Core.Services;
using Xunit;
using Xunit.Abstractions;
using Spectre.Console;

namespace HourRegistration.Test.ServicesTests;

/// <summary>
/// Test class for the WeeklySummaryService. This class contains unit tests
/// to verify the correct functionality of various methods in the WeeklySummaryService
/// related to calculating weekly summaries, aggregating hours, or generating week overviews.
/// </summary>
public class WeeklySummaryServiceTest
{
    private readonly WeeklySummaryService _service;
    private readonly ITestOutputHelper _output;

    public WeeklySummaryServiceTest(ITestOutputHelper output)
    {
        _service = new WeeklySummaryService();
        _output = output;
    }


    /// Writes the specified Spectre.Console `Table` object to the console output stream.
    /// This method uses an `AnsiConsole` instance configured to direct its output to a `StringWriter`,
    /// allowing for the formatted table content to be captured as a string and written to `_output`.
    /// <param name="table">The Spectre.Console table instance to be formatted and written to the console output.</param>
    private void WriteSpectreOutput(Table table)
    {
        using var writer = new StringWriter();
        AnsiConsole.Create(new AnsiConsoleSettings
        {
            Out = new AnsiConsoleOutput(writer)
        }).Write(table);

        _output.WriteLine(writer.ToString());
    }

    /// Tests the `CalculateStartOfTheWeek` method of the `WeeklySummaryService` to ensure that it
    /// returns the correct offset in days from the specified date to the start of the week,
    /// based on the given culture's `FirstDayOfWeek`.
    /// <param name="dateString">The date in string format to be evaluated.</param>
    /// <param name="cultureName">The name of the culture to use for determining the first day of the week.</param>
    /// <param name="expectedOffset">The expected offset in days from the input date to the start of the week.</param>
    [Theory]
    [InlineData("2023-12-08", "nl-NL", 4)] // 8 dec 2023 is vrijdag (5), nl-NL start op maandag (1). 5-1=4.
    [InlineData("2023-12-10", "nl-NL", 6)] // 10 dec 2023 is zondag (0). Edge case, moet 6 retourneren.
    [InlineData("2023-12-06", "en-US", 3)] // 6 dec 2023 is woensdag (3), en-US start op zondag (0). 3-0=3.
    public void CalculateStartOfTheWeek_ShouldReturnCorrectOffset_BasedOnCulture(string dateString, string cultureName,
        int expectedOffset)
    {
        // Arrange
        var day = DateTime.Parse(dateString);
        var culture = new CultureInfo(cultureName);
        var testCase =
            $"Datum: {day:ddd, d MMMM}, Cultuur: {cultureName} (Weekstart: {culture.DateTimeFormat.FirstDayOfWeek})";

        _output.WriteLine(
            $"\n--- Test gestart: {nameof(CalculateStartOfTheWeek_ShouldReturnCorrectOffset_BasedOnCulture)} ---");

        // Act
        var result = _service.CalculateStartOfTheWeek(day, culture);

        // Output Tabel
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]{testCase}[/]")
            .AddColumn(new TableColumn("[green]Actie[/]"))
            .AddColumn(new TableColumn("[green]Verwacht[/]").Centered())
            .AddColumn(new TableColumn("[green]Gevonden[/]").Centered())
            .AddColumn(new TableColumn("[green]Resultaat[/]").Centered());

        var status = result == expectedOffset ? "[lime]SUCCESS[/]" : "[red]FAILED[/]";

        table.AddRow(
            "Dagen sinds start van de week",
            expectedOffset.ToString(),
            result.ToString(),
            status
        );
        WriteSpectreOutput(table);

        // Assert
        result.Should().Be(expectedOffset);
    }


    /// Tests the method responsible for aggregating worked hours by summing them and filtering the data
    /// based on the provided date range. This test ensures that the aggregation correctly sums worked hours
    /// within the specified date range and filters out any entries outside that range.
    /// It also verifies that the resulting data links correctly to the corresponding HourReceipt ID and
    /// that all calculated total hours meet the expected values within a defined tolerance.
    /// The method further validates that such aggregation has the correct number of keys in the result set.
    /// <remarks>
    /// The test considers edge cases where the entries lie on the boundary of the date range
    /// and ensures that only valid entries are included in the results.
    /// </remarks>
    [Fact]
    public void AggregateHours_ShouldCorrectlySumHoursAndFilterByDateRange()
    {
        // Arrange
        var startOfWeek = new DateTime(2023, 12, 04); // Maandag
        var endOfWeek = new DateTime(2023, 12, 10); // Zondag

        // 1. DEFINIEER TOLERANTIE
        const double tolerance = 0.001;

        _output.WriteLine(
            $"\n--- Test gestart: {nameof(AggregateHours_ShouldCorrectlySumHoursAndFilterByDateRange)} ---");

        var hourReceipts = new List<HourReceipt>
        {
            // Binnen bereik (maandag) → 7.5 uur. ID 10 en 11.
            // De service pakt .First().Id, dus we verwachten ID 10.
            new() { Id = 10, Date = startOfWeek.AddDays(0), HoursWorked = 4, MinutesWorked = 30 },
            new() { Id = 11, Date = startOfWeek.AddDays(0), HoursWorked = 3, MinutesWorked = 0 },

            // Binnen bereik (woensdag) → 8.0 uur. ID 20 en 21. Verwacht ID 20.
            new() { Id = 20, Date = startOfWeek.AddDays(2), HoursWorked = 7, MinutesWorked = 45 },
            new() { Id = 21, Date = startOfWeek.AddDays(2), HoursWorked = 0, MinutesWorked = 15 },

            // Buiten bereik (genegeerd)
            new() { Id = 99, Date = startOfWeek.AddDays(-1), HoursWorked = 2, MinutesWorked = 0 },
            new() { Id = 100, Date = endOfWeek.AddDays(1), HoursWorked = 1, MinutesWorked = 0 },
        };

        // Act
        // Result is nu: Dictionary<DateTime, (double TotalHours, int ReceiptId)>
        var result = _service.AggregateHours(hourReceipts, startOfWeek, endOfWeek);

        // Output Tabel
        var expectedData = new Dictionary<DateTime, (double Hours, int Id)>
        {
            { startOfWeek, (7.5, 10) }, // Maandag
            { startOfWeek.AddDays(2), (8.0, 20) } // Woensdag
        };

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]Aggregatie Resultaten: {startOfWeek:d} t/m {endOfWeek:d}[/]")
            .AddColumn(new TableColumn("[green]Datum[/]"))
            .AddColumn(new TableColumn("[green]Gevonden (Uur | ID)[/]").Centered())
            .AddColumn(new TableColumn("[green]Verwacht (Uur | ID)[/]").Centered())
            .AddColumn(new TableColumn("[green]Status[/]").Centered());

        foreach (var expected in expectedData)
        {
            // 2. Tuple uitpakken
            result.TryGetValue(expected.Key, out var actualData);

            // Vergelijk uren (met tolerantie) EN ID (exact)
            var hoursCorrect = Math.Abs((actualData.TotalHours - expected.Value.Hours)!) < tolerance;

            // FIX: Gebruik 'ReceiptId' in plaats van 'HourReceiptId'
            var idCorrect = actualData.HourReceiptId == expected.Value.Id;

            var isCorrect = hoursCorrect && idCorrect;
            var status = isCorrect ? "[lime]OK[/]" : "[red]FOUT[/]";

            table.AddRow(
                expected.Key.ToString("ddd, d MMMM"),
                // FIX: Gebruik 'ReceiptId' ook hier
                $"{actualData.TotalHours:N2}u | ID: {actualData.HourReceiptId}",
                $"{expected.Value.Hours:N2}u | ID: {expected.Value.Id}",
                status
            );
        }

        if (result.Keys.Count > expectedData.Keys.Count)
        {
            table.AddRow("[red]Onverwachte data[/]", $"[{result.Keys.Count}]", $"[{expectedData.Keys.Count}]",
                "[red]FOUT[/]");
        }

        WriteSpectreOutput(table);

        // Assert
        result.Keys.Should().HaveCount(2);

        // 3. AANGEPASTE ASSERTS MET JUISTE NAAMGEVING

        // Check Maandag
        result[startOfWeek].TotalHours.Should().BeApproximately(7.5, tolerance);
        result[startOfWeek].HourReceiptId.Should().Be(10); // FIX: ReceiptId

        // Check Woensdag
        result[startOfWeek.AddDays(2)].TotalHours.Should().BeApproximately(8.0, tolerance);
        result[startOfWeek.AddDays(2)].HourReceiptId.Should().Be(20); // FIX: ReceiptId

        // Check Buiten bereik
        result.Should().NotContainKey(startOfWeek.AddDays(-1));

        _output.WriteLine("--- Test Succesvol ---");
    }

    /// Validates the functionality of generating a weekly overview that spans exactly seven consecutive days,
    /// beginning from the specified start date. This test ensures that the generated overview includes all days,
    /// assigning zero hours to any days not explicitly present in the input data.
    /// <remarks>
    /// The input consists of a starting date and a dictionary mapping specific dates to their corresponding
    /// total hours and receipt IDs. The test checks that the result covers the full week, aligning with the
    /// expected format and data for days with and without an input.
    /// </remarks>
    [Fact]
    public void GenerateWeekOverview_ShouldReturn7DaysStartingFromStartDate_WithZeroHoursForMissingDays()
    {
        // Arrange
        var startOfWeek = new DateTime(2023, 12, 04); // Maandag

        // Definieer de tolerantie HIER, zodat deze globaal is binnen deze methode.
        const double tolerance = 0.001;

        // FIX 1: De Dictionary moet nu Tuples bevatten (Uren, Id)
        // We verzinnen hier even dummy ID's (bijv. 10 en 20), omdat die nodig zijn voor de input.
        var aggregatedData = new Dictionary<DateTime, (double TotalHours, int ReceiptId)>
        {
            { startOfWeek.AddDays(0), (7.5, 10) }, // Maandag (7.5 uur, ID 10)
            { startOfWeek.AddDays(2), (8.0, 20) } // Woensdag (8.0 uur, ID 20)
        };

        _output.WriteLine(
            $"\n--- Test gestart: {nameof(GenerateWeekOverview_ShouldReturn7DaysStartingFromStartDate_WithZeroHoursForMissingDays)} ---");

        // Act
        var result = _service.GenerateWeekOverview(aggregatedData, startOfWeek);

        // Output Tabel
        var table = new Table()
            .Border(TableBorder.Heavy)
            .Title("[yellow]Week Overzicht (7 dagen)[/]")
            .AddColumn(new TableColumn("[green]Dag[/]"))
            .AddColumn(new TableColumn("[green]Gevonden Uren[/]").Centered())
            .AddColumn(new TableColumn("[green]Verwachting[/]").Centered())
            .AddColumn(new TableColumn("[green]Status[/]").Centered());

        for (var i = 0; i < 7; i++)
        {
            var currentDay = startOfWeek.AddDays(i);
            var actualHours = result[i].TotalHours;

            // FIX 2: Data ophalen uit de Tuple Dictionary
            // GetValueOrDefault geeft (0, 0) terug als de datum niet bestaat.
            // Item1 (of TotalHours) is dus 0.0 als er geen data is, wat klopt voor deze test.
            var expectedData = aggregatedData.GetValueOrDefault(currentDay.Date);
            var expectedHours = expectedData.TotalHours;

            // Gebruik de Math.Abs functie in de lus om de status te bepalen.
            var isCorrect = Math.Abs(actualHours - expectedHours) < tolerance;

            var status = isCorrect ? "[lime]OK[/]" : "[red]FOUT[/]";

            table.AddRow(
                currentDay.ToString("ddd, d MMMM"),
                $"{actualHours:N2}",
                $"{expectedHours:N2}",
                status
            );
        }

        WriteSpectreOutput(table);

        // Assert
        result.Should().HaveCount(7);
        result[0].Date.Should().Be(startOfWeek);

        result.Should().ContainSingle(s =>
            s.Date == startOfWeek.AddDays(0) &&
            Math.Abs(s.TotalHours - 7.5) < tolerance); // ID check optioneel hier

        result.Should().ContainSingle(s =>
            s.Date == startOfWeek.AddDays(2) &&
            Math.Abs(s.TotalHours - 8.0) < tolerance);

        result.Should().Contain(s =>
            s.Date == startOfWeek.AddDays(1) &&
            Math.Abs(s.TotalHours - 0.0) < tolerance);

        result.Should().Contain(s =>
            s.Date == startOfWeek.AddDays(6) &&
            Math.Abs(s.TotalHours - 0.0) < tolerance);
    }
}