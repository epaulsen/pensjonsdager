using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pensjonsdager
{
    public static class FerieKalkulator
    {
        public static int VacationDays(int age)
        {
            if (age < 60)
            {
                return 25;
            }

            return 30;
        }
    }
}
