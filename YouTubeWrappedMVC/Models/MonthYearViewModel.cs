using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class MonthYearViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public MonthYearViewModel()
        {
        }

        public MonthYearViewModel(int month, int year)
        {
            Month = month;
            Year = year;
        }

        public static bool operator ==(MonthYearViewModel vm1, MonthYearViewModel vm2)
        {
            return vm1.Month == vm2.Month && vm1.Year == vm2.Year;
        }

        public static bool operator !=(MonthYearViewModel vm1, MonthYearViewModel vm2)
        {
            return vm1.Month != vm2.Month || vm1.Year != vm2.Year;
        }
    }
}
