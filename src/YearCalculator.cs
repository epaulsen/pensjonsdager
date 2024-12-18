using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using static pensjonsdager.YearCalculator;

namespace pensjonsdager;

public static class DateHelper
{
    public static DateOnly Today()
    {
        return new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    }

    internal static bool IsWorkingDay(this DateOnly date)
    {
        return (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);
    }

    internal static HashSet<DateOnly> AddFixedHolidays(this HashSet<DateOnly> list, int year)
    {
        if (new DateOnly(year, 1, 1).IsWorkingDay()) list.Add(new DateOnly(year, 1, 1));
        if (new DateOnly(year, 5, 1).IsWorkingDay()) list.Add(new DateOnly(year, 5, 1));
        if (new DateOnly(year, 5, 17).IsWorkingDay()) list.Add(new DateOnly(year, 5, 17));
        if (new DateOnly(year, 12, 25).IsWorkingDay()) list.Add(new DateOnly(year, 12, 25));
        if (new DateOnly(year, 12, 26).IsWorkingDay()) list.Add(new DateOnly(year, 12, 26));
        return list;
    }
}


public class YearCalculator
{
    public static int WorkingDaysForAGivenYear(int year, DateOnly? startDay = null, DateOnly? endDay = null, DayOfYear[]? extraDaysOff = null)
    {
        startDay ??= new DateOnly(year, 1, 1);
        endDay ??= new DateOnly(year, 12, 31);

        var holidaysOnWorkingDays = GetHolidays(year, extraDaysOff)
            .Select(h => h.Date)
            .Where(d => d.IsWorkingDay())
            .ToHashSet();

        return Days(year, startDay, endDay)
            .Count(d => d.IsWorkingDay() && !holidaysOnWorkingDays.Contains(d));
    }

    public static IEnumerable<Holiday> GetHolidays(int year, DayOfYear[]? extraDaysOff = null)
    {
        yield return new Holiday(new DateOnly(year, 1, 1), "1. nyttårsdag");
        yield return new Holiday(new DateOnly(year, 5, 1), "Arbeidernes dag");
        yield return new Holiday(new DateOnly(year, 5, 17), "Grunnlovsdag");
        yield return new Holiday(new DateOnly(year, 12, 25), "1. juledag");
        yield return new Holiday(new DateOnly(year, 12, 25), "1. juledag");
        yield return new Holiday(new DateOnly(year, 12, 26), "2. juledag");

        DateOnly easterSunday = FindEasterSunday(year);
        yield return new Holiday(easterSunday.AddDays(-3), "Skjærtorsdag");
        yield return new Holiday(easterSunday.AddDays(-2), "Langfredag");
        yield return new Holiday(easterSunday.AddDays(1), "2. påskedag");
        yield return new Holiday(GetAscensionDay(easterSunday), "Kristi himmelfart");
        yield return new Holiday(easterSunday.AddDays(50), "2. pinsedag");
        foreach (var holiday in extraDaysOff)
        {
            yield return new Holiday(new DateOnly(year, holiday.Month, holiday.Day), string.Empty);
        }
    }

    private static IEnumerable<DateOnly> Days(int year, DateOnly? start = null, DateOnly? end = null)
    {
        start ??= new DateOnly(year, 1, 1);
        end ??= new DateOnly(year, 12, 31);

        while (start <= end)
        {
            yield return start.Value;
            start = start.Value.AddDays(1);
        }
    }

    private static DateOnly GetAscensionDay(DateOnly easterSunday)
    {
        DateOnly start = easterSunday.AddDays(40); // 40 dager etter påskesøndag

        // Finn neste torsdag.
        while (start.DayOfWeek != DayOfWeek.Thursday)
        {
            start = start.AddDays(1);
        }

        return start;
    }

    public static DateOnly FindEasterSunday(int year)
    {
        int epoch = FindEpoch(year);
        int goldenNumber = year % 19 + 1;
        DateOnly day;
        if (epoch == 25)
        {
            if (goldenNumber > 11)
            {
                day = new DateOnly(year, 4, 17);
            }
            else
            {
                day = new DateOnly(year, 4, 18);
            }
        }
        else
        {
            var fullMoonData = FullMoons[epoch];
            day = new DateOnly(year, fullMoonData.Month, fullMoonData.Day);
        }

        while (day.DayOfWeek != DayOfWeek.Sunday)
        {
            day = day.AddDays(1);
        }

        return day;
    }

    private static int FindEpoch(int year)
    {
        int goldenNumber = year % 19 + 1;

        int julianEpact = 11 * (goldenNumber - 1) % 30;
        if (julianEpact == 0)
        {
            julianEpact = 30;
        }
        int century = year / 100 + 1;
        int sun = 3 * century / 4;
        int lunar = (8 * century + 5) / 25;

        int gregorianEpact = (julianEpact - sun + lunar + 8) % 30;
        if (gregorianEpact == 0)
        {
            gregorianEpact = 30;
        }
        return gregorianEpact;
    }

    public record Holiday(DateOnly Date, string Name);

    public record DayOfYear(int Month, int Day);

    internal static Dictionary<int, DayOfYear> FullMoons = new()
    {
        { 1, new DayOfYear(4, 12) },
        { 2, new DayOfYear(4, 11) },
        { 3, new DayOfYear(4, 10) },
        { 4, new DayOfYear(4, 9) },
        { 5, new DayOfYear(4, 8) },
        { 6, new DayOfYear(4, 7) },
        { 7, new DayOfYear(4, 6) },
        { 8, new DayOfYear(4, 5) },
        { 9, new DayOfYear(4, 4) },
        { 10, new DayOfYear(4, 3) },
        { 11, new DayOfYear(4, 2) },
        { 12, new DayOfYear(4, 1) },
        { 13, new DayOfYear(3, 31) },
        { 14, new DayOfYear(3, 30) },
        { 15, new DayOfYear(3, 29) },
        { 16, new DayOfYear(3, 28) },
        { 17, new DayOfYear(3, 27) },
        { 18, new DayOfYear(3, 26) },
        { 19, new DayOfYear(3, 25) },
        { 20, new DayOfYear(3, 24) },
        { 21, new DayOfYear(3, 23) },
        { 22, new DayOfYear(3, 22) },
        { 23, new DayOfYear(3, 21) },
        { 24, new DayOfYear(4, 18) },
        { 26, new DayOfYear(4, 17) },
        { 27, new DayOfYear(4, 16) },
        { 28, new DayOfYear(4, 15) },
        { 29, new DayOfYear(3, 14) },
        { 30, new DayOfYear(3, 13) },
    };
}