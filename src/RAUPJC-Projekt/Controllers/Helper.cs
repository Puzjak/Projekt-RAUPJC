using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RAUPJC_Projekt.Core;
using RAUPJC_Projekt.Core.ServiceLogic;
using RAUPJC_Projekt.Core.TermDateLogic;
using RAUPJC_Projekt.Models;

namespace RAUPJC_Projekt.Controllers
{
    public class Helper
    {
        /// <summary>
        /// Gets Guid of the currently logged user. 
        /// </summary>
        public static async Task<Guid> GetCurrentUserId(UserManager<ApplicationUser> userManager, HttpContext httpContext)
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            return new Guid(user.Id);
        }

        public static IList<DateTime> GetAvailableDateTimes(Service service)
        {
            IList<DateTime> result = new List<DateTime>();

            for (int i = 0; i <= Constants.WorkingMinutes - Constants.MinuteStep; i += Constants.MinuteStep)
            {
                var endOfTerm = Constants.StartOfWorkingHours.AddMinutes(i + service.Duration);
                if (endOfTerm > Constants.EndOfWorkingHours) break;
                result.Add(Constants.StartOfWorkingHours.AddMinutes(i));
            }
            return result;
        }

        /// <summary>
        /// Gets free term dates.
        /// </summary>
        /// <param name="termDates"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IList<DateTime> GetAvailableDateTimes(IList<TermDate> termDates, Service service)
        {
            IList<DateTime> result = new List<DateTime>();
            
            for (int i = 0; i <= Constants.WorkingMinutes - Constants.MinuteStep; i += Constants.MinuteStep)
            {
                var startOfTerm = Constants.StartOfWorkingHours.AddMinutes(i);
                var endOfTerm = Constants.StartOfWorkingHours.AddMinutes(i + service.Duration);

                //If duration of service for this term exceeds end of working hours every term after that 
                //isn't avaliable
                if (endOfTerm > Constants.EndOfWorkingHours) break;

                //Returns all term dates which make current term unavailable for reserving with desired service
                var tmp = termDates.Where(f => (((startOfTerm >= default(DateTime).Add(f.StartOfTerm.TimeOfDay)) &&
                                               (startOfTerm < default(DateTime).Add(f.EndOfTerm.TimeOfDay))) ||
                                               ((startOfTerm < default(DateTime).Add(f.StartOfTerm.TimeOfDay)) &&
                                               (endOfTerm > default(DateTime).Add(f.StartOfTerm.TimeOfDay)))))
                                               .ToList();

                //If number of workers is larger then number of term dates then this term is avaliable for 
                //reserving with desired service
                if (tmp.Count() < Constants.NumberOfWorkers)
                {
                    result.Add(Constants.StartOfWorkingHours.AddMinutes(i));
                }
            }
            return result;
        }

        public static DateTime FirstDateOfWeekISO8601(int weekOfYear, int year) 
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 2;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }
    }
}
