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


    /// Tests the `AggregateHours` method of the `WeeklySummaryService` class to verify its ability to correctly sum the hours
    /// worked and filter them based on a specific date range.
    /// This test constructs a set of sample `HourReceipt` data for a given week, including entries that fall both inside and
    /// outside the specified range. It then validates whether `AggregateHours` produces the expected results for hours summed
    /// per date and ensures that data outside the range is ignored. Additionally, the test generates a detailed output table
    /// to display and compare the actual results versus expectations.
    /// Asserts include checks on the number of entries in the result, the correctness of hours for valid dates, and the absence
    /// of invalid dates.
    /// Exceptions, formatting, and result mismatches are explicitly captured via table logging in the output for better diagnostics.
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
            // ... (hourReceipts data blijft hetzelfde) ...
            // Binnen bereik (maandag) → 7.5 uur
            new() { Date = startOfWeek.AddDays(0), HoursWorked = 4, MinutesWorked = 30 },
            new() { Date = startOfWeek.AddDays(0), HoursWorked = 3, MinutesWorked = 0 },

            // Binnen bereik (woensdag) → 8.0 uur
            new() { Date = startOfWeek.AddDays(2), HoursWorked = 7, MinutesWorked = 45 },
            new() { Date = startOfWeek.AddDays(2), HoursWorked = 0, MinutesWorked = 15 },

            // Buiten bereik (genegeerd)
            new() { Date = startOfWeek.AddDays(-1), HoursWorked = 2, MinutesWorked = 0 },
            new() { Date = endOfWeek.AddDays(1), HoursWorked = 1, MinutesWorked = 0 },
        };

        // Act
        var result = _service.AggregateHours(hourReceipts, startOfWeek, endOfWeek);

        // Output Tabel
        var expectedHours = new Dictionary<DateTime, double>
        {
            { startOfWeek, 7.5 }, // Maandag
            { startOfWeek.AddDays(2), 8.0 } // Woensdag
        };

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]Aggregatie Resultaten: {startOfWeek:d} t/m {endOfWeek:d} (Tolerantie: {tolerance})[/]")
            .AddColumn(new TableColumn("[green]Datum[/]"))
            .AddColumn(new TableColumn("[green]Gevonden Uren[/]").Centered())
            .AddColumn(new TableColumn("[green]Verwachting[/]").Centered())
            .AddColumn(new TableColumn("[green]Status[/]").Centered());

        // Loop over de Verwachtingen om alle dagen te controleren (ook ontbrekende)
        foreach (var expected in expectedHours)
        {
            result.TryGetValue(expected.Key, out double actualHours);

            // 2. FIX: Gebruik Math.Abs om te controleren op gelijkheid binnen de tolerantie
            var isCorrect = Math.Abs(actualHours - expected.Value) < tolerance;

            var status =
                isCorrect ? "[lime]OK[/]" : "[red]FOUT[/]"; // Logische correctie: als het correct is, is het OK

            table.AddRow(
                expected.Key.ToString("ddd, d MMMM"),
                $"{actualHours:N2}",
                $"{expected.Value:N2}",
                status
            );
        }

        // Controleer of er onverwachte data is
        // Hier is de telling van Keys (integers) en niet floating point, dus directe vergelijking is prima.
        if (result.Keys.Count > expectedHours.Keys.Count)
        {
            table.AddRow("[red]Onverwachte data[/]", $"[{result.Keys.Count}]", $"[{expectedHours.Keys.Count}]",
                "[red]FOUT[/]");
        }

        WriteSpectreOutput(table);

        // Assert
        result.Keys.Should().HaveCount(2);

        // 3. TOLERANTIE IS CORRECT GEBRUIKT IN ASSERTS
        result[startOfWeek].Should().BeApproximately(7.5, tolerance);
        result[startOfWeek.AddDays(2)].Should().BeApproximately(8.0, tolerance);
        result.Should().NotContainKey(startOfWeek.AddDays(-1));

        _output.WriteLine("--- Test Succesvol ---");
    }

    /// Validates that the GenerateWeekOverview method returns a list of 7 days starting from a given start date.
    /// For any days within the week that are not explicitly specified in the input-aggregated hours dictionary,
    /// the method should assign a value of zero hours.
    /// This test ensures:
    /// - Correct generation of a 7-day week starting from the provided start date.
    /// - Accurate handling of missing data by setting TotalHours to zero for those days.
    /// - Proper inclusion of days with specified aggregated hours in the result.
    /// The method uses a predefined tolerance for numerical comparisons to verify floating-point accuracy
    /// and includes detailed validation for all 7 days using FluentAssertions to check the expected data structure
    /// and values.
    [Fact]
    public void GenerateWeekOverview_ShouldReturn7DaysStartingFromStartDate_WithZeroHoursForMissingDays()
    {
        // Arrange
        var startOfWeek = new DateTime(2023, 12, 04); // Maandag

        // Definieer de tolerantie HIER, zodat deze globaal is binnen deze methode.
        const double tolerance = 0.001;

        // Zorg ervoor dat System. Math is geimporteerd (via using System;)
        // Als je de Fluent Assertions. BeApproximately methode gebruikt, is Math. Abs niet strikt nodig,
        // maar het is goed om de tolerantie consistent te gebruiken voor zowel de tabel status als de asserts.

        // Geaggregeerde uren: alleen maandag (7.5) en woensdag (8.0) hebben data
        var aggregatedHours = new Dictionary<DateTime, double>
        {
            { startOfWeek.AddDays(0), 7.5 }, // Maandag
            { startOfWeek.AddDays(2), 8.0 } // Woensdag
        };

        _output.WriteLine(
            $"\n--- Test gestart: {nameof(GenerateWeekOverview_ShouldReturn7DaysStartingFromStartDate_WithZeroHoursForMissingDays)} ---");

        // Act
        var result = _service.GenerateWeekOverview(aggregatedHours, startOfWeek);

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

            // Verwachting: 7.5 op maandag, 8.0 op woensdag, anders 0.0
            var expectedHours = aggregatedHours.GetValueOrDefault(currentDay.Date);

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

        result.Should().HaveCount(7);
        result[0].Date.Should().Be(startOfWeek);

        // Assert
        result.Should().HaveCount(7);
        result.Should().ContainSingle(s =>
            s.Date == startOfWeek.AddDays(0) &&
            Math.Abs(s.TotalHours - 7.5) < tolerance);

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