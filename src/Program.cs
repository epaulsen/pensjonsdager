// See https://aka.ms/new-console-template for more information

using System.Globalization;
using pensjonsdager;
using Spectre.Console;

DateOnly today = DateHelper.Today();

var pensjonsDato = AnsiConsole.Prompt(
    new TextPrompt<DateOnly>("Hvilken dag ønsker du å pensjonere deg på (DD.MM.ÅÅÅÅ)? ")
    {
        Culture = CultureInfo.GetCultureInfo("nb-NO")
    }
        .Validate((n) =>
        {
            if (n < today)
            {
                return ValidationResult.Error("Du kan ikke pensjonere deg på en dag som har vært");
            }

            if (n > DateHelper.Today().AddYears(75))
            {
                return ValidationResult.Error("Hallo i luken! Hvor lenge har du egentlig tenkt å jobbe??");
            }

            return ValidationResult.Success();
        }));

var birthYear = AnsiConsole.Prompt(new TextPrompt<int>("Hvilket år er du født (ÅÅÅÅ)? ").Validate(y =>
{
    if (y<1944)
    {
        return ValidationResult.Error("Du burde da ha gått av med pensjon for lenge siden! Skriv inn et gyldig årstall");
    }

    if (y > DateHelper.Today().Year)
    {
        return ValidationResult.Error("Umulig å beregne arbeidsdager på personer som ikke er født enda.");
    }

    return ValidationResult.Success();
}));

List<PotentialDaysOff> candidates = new List<PotentialDaysOff>
{
    new("Julaften", 12, 24),
    new("Nyttårsaften", 12, 31),
};

List<YearCalculator.DayOfYear> extraDaysOff = new List<YearCalculator.DayOfYear>();

foreach (var candidate in candidates)
{
    var hasTimeOff =  AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title($"Har du fri {candidate.Name}?")
            .PageSize(5)
            .AddChoices(new[] {
                "Ja", "Nei",
            }));
    if (hasTimeOff == "Ja")
    {
        extraDaysOff.Add(new YearCalculator.DayOfYear(candidate.Month,candidate.Day));
    }
}

var table = new Table().LeftAligned();

var yearCount = pensjonsDato.Year - DateHelper.Today().Year +1;

var list = Enumerable.Range(DateHelper.Today().Year, yearCount)
    .Select(i => new YearData(i, birthYear, DateHelper.Today(), pensjonsDato, extraDaysOff.ToArray()))
    .ToList();

table.AddColumn("År").Centered();
table.AddColumn("Alder").Centered();
table.AddColumn(new TableColumn("Arbeidsdager").Centered());
table.AddColumn(new TableColumn("Feriedager").Centered());
table.AddColumn(new TableColumn("Sum").Centered());

foreach (var year in list)
{
    table.AddRow(year.Year.ToString(), year.Age.ToString(), year.WorkingDays.ToString(), year.VacationDays.ToString(), year.WorkingDaysTotal.ToString());
}

table.AddRow("----", "--", "---", "--", "---");
table.AddRow("Sum", "", list.Sum(l => l.WorkingDays).ToString(), list.Sum(l => l.VacationDays).ToString(), list.Sum(l => l.WorkingDaysTotal).ToString());

AnsiConsole.Write(table.LeftAligned());

AnsiConsole.Markup("[Yellow]Bemerk: Feriedager i start/sluttår er ikke beregnet, så total sum kan avvike[/]");

Console.ReadKey();


internal record PotentialDaysOff(string Name, int Month, int Day);






