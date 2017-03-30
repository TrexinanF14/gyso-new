using Nancy;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYSOManager.Modules
{
    public class CalendarModule : NancyModule
    {
        public enum Division
        {
            Kindergarten = 0,
            Grade1Through3,
            Grade4Through8Girls,
            Grade4Through8Boys
        }

        public CalendarModule()
        {
            Get["/calendar"] = _ =>
            {
                var year = DateTime.Now.Year;

                var info = new
                {
                    KPractices = GetPracticeDays(year, Division.Kindergarten).Select(x => x.ToString("MMM d", null)),
                    KGames = GetGameDays(year, Division.Kindergarten).Select(x => x.ToString("MMM d", null)),
                    G1G3Practices = GetPracticeDays(year, Division.Grade1Through3).Select(x => x.ToString("MMM d", null)),
                    G1G3Games = GetGameDays(year, Division.Grade1Through3).Select(x => x.ToString("MMM d", null)),
                    G4G8GirlsPractices = GetPracticeDays(year, Division.Grade4Through8Girls).Select(x => x.ToString("MMM d", null)),
                    G4G8GirlsGames = GetGameDays(year, Division.Grade4Through8Girls).Select(x => x.ToString("MMM d", null)),
                    G4G8BoysPractices = GetPracticeDays(year, Division.Grade4Through8Boys).Select(x => x.ToString("MMM d", null)),
                    G4G8BoysGames = GetGameDays(year, Division.Grade4Through8Boys).Select(x => x.ToString("MMM d", null))
                };

                return View["calendar", info];
            };
        }

        public static List<LocalDate> GetGameDays(int year, Division division)
        {
            int weekDay;
            switch (division)
            {
                case Division.Kindergarten:
                    //Kindergarten plays on monday..
                    weekDay = 1;
                    break;
                case Division.Grade1Through3:
                    //1st and 2nd-3rd plays on tuesday..
                    weekDay = 2;
                    break;
                case Division.Grade4Through8Girls:
                    //4th-5th and Middle girls plays on monday..
                    weekDay = 1;
                    break;
                case Division.Grade4Through8Boys:
                    //4th-5th and Middle boys plays on thursday..
                    weekDay = 4;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return GetDateSequence(year, weekDay);
        }

        public static List<LocalDate> GetPracticeDays(int year, Division division)
        {
            int weekDay;
            switch (division)
            {
                case Division.Kindergarten:
                    //Kindergarten practices on thursday..
                    weekDay = 4;
                    break;
                case Division.Grade1Through3:
                    //1st and 2nd-3rd practices on thursday..
                    weekDay = 4;
                    break;
                case Division.Grade4Through8Girls:
                    //4th-5th and Middle girls practices on tuesday..
                    weekDay = 2;
                    break;
                case Division.Grade4Through8Boys:
                    //4th-5th and Middle boys practices on monday..
                    weekDay = 1;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return GetDateSequence(year, weekDay);
        }

        private static List<LocalDate> GetDateSequence(int year, int dayOfWeek)
        {
            var startDate = GetFirstPlayDate(year);
            while (startDate.DayOfWeek != dayOfWeek)
            {
                startDate = startDate.PlusDays(1);
            }
            var dates = new List<LocalDate>();
            dates.Add(startDate);
            while (startDate.PlusWeeks(1) <= GetLastPlayDate(year))
            {
                startDate = startDate.PlusWeeks(1);
                //Instead of memorial day, teams play on that week's wednesday.
                if (startDate == GetMemorialDay(year))
                {
                    dates.Add(startDate.PlusDays(2));
                }
                else
                {
                    dates.Add(startDate);
                }
            }
            return dates;
        }

        private static LocalDate GetFirstPlayDate(int year)
        {
            var aprilStart = new LocalDate(year, 4, 1);
            while (aprilStart.DayOfWeek != 1)
            {
                aprilStart = aprilStart.PlusDays(1);
            }
            //gyso starts 2 weeks after goshen spring break, which is the first
            //full week of april.
            aprilStart = aprilStart.PlusWeeks(2);
            return aprilStart;
        }

        private static LocalDate GetLastPlayDate(int year)
        {
            var firstPlayDate = GetFirstPlayDate(year);
            //GYSO runs for 8 weeks, that final thursday is the last day.
            return firstPlayDate.PlusWeeks(7).PlusDays(3);
        }

        private static LocalDate GetMemorialDay(int year)
        {
            var mayStart = new LocalDate(year, 5, 1);
            while (mayStart.DayOfWeek != 1)
            {
                mayStart = mayStart.PlusDays(1);
            }
            while (mayStart.PlusWeeks(1).Month == 5)
            {
                mayStart = mayStart.PlusWeeks(1);
            }
            return mayStart;
        }
    }
}