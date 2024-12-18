using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pensjonsdager
{
    public class YearData
    {
        public int Year;

        public IList<YearCalculator.Holiday> Holidays { get; }

        public int WorkingDays { get; }

        public int Age { get; }

        public int VacationDays { get; }

        public int WorkingDaysTotal => WorkingDays - VacationDays;

        public YearData(int year, int birthYear, DateOnly? startDay, DateOnly? endDay, YearCalculator.DayOfYear[]? extraDaysOff = null)
        {
            Year = year;
            Holidays = YearCalculator.GetHolidays(year, extraDaysOff).OrderBy(d => d.Date).ToList();
            WorkingDays = YearCalculator.WorkingDaysForAGivenYear(year,
                startDay?.Year == year ? startDay.Value : null,
                endDay?.Year == year ? endDay.Value : null, extraDaysOff: extraDaysOff);
            Age=year - birthYear;
            if (startDay?.Year != year && endDay?.Year != year)
            {
                VacationDays=FerieKalkulator.VacationDays(Age);
            }
        }
    }
}
