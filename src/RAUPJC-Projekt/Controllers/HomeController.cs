using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RAUPJC_Projekt.Core;
using RAUPJC_Projekt.Core.TermDateLogic;
using RAUPJC_Projekt.Models;

namespace RAUPJC_Projekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITermDateRepository _repository;

        public HomeController(ITermDateRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                var date =
                    Helper.FirstDateOfWeekISO8601(
                        DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(DateTime.Now,
                            DateTimeFormatInfo.CurrentInfo.CalendarWeekRule,
                            DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek), DateTime.Now.Year);
                HttpContext.Session.SetObjectAsJson(Constants.SessionKeyDate, date);

            }

            return View();
        }

        [HttpPost]
        public IActionResult Index(WeekPickerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (DateTimeFormatInfo.CurrentInfo != null)
                {
                    
                    try
                    {
                        string[] strings = model.Week.Split(new[] { ',' }, 2);
                        strings[0] = strings[0].Remove(2);
                        strings[1] = strings[1].TrimStart().Remove(4);

                        var weekOfYear = int.Parse(strings[0]);
                        var year = int.Parse(strings[1]);

                        var date = Helper.FirstDateOfWeekISO8601(weekOfYear, year);

                        var termDates =
                            _repository.GetFiltered(
                                f => f.StartOfTerm.Date >= date.Date && f.StartOfTerm <= date.AddDays(6).Date);


                        HttpContext.Session.SetObjectAsJson(Constants.SessionKeyFreeTerms, termDates);
                        return View();
                    }
                    catch (FormatException)
                    {
                        ModelState.AddModelError(string.Empty, "KRIOV");
                        return View(model);
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "KRIOV");
            return View(model);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
