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

        public override bool Equals(object obj)
        {
            var other = (MonthYearViewModel)obj;
            return other.Month == Month && other.Year == Year;
        }
    }
}
