using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RAUPJC_Projekt.Core;
using RAUPJC_Projekt.Models;
using RAUPJC_Projekt.Core.ServiceLogic;
using RAUPJC_Projekt.Core.TermDateLogic;
using RAUPJC_Projekt.Models.CustomerViewModels;

namespace RAUPJC_Projekt.Controllers
{

    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ITermDateRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger _logger;

        public CustomerController(ITermDateRepository repository, UserManager<ApplicationUser> userManager, 
            IServiceRepository serviceRepository, ILogger<CustomerController> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(DateAndServicePickerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var strings = model.Date.Split(new[] { '-' }, 3);
                    var date = new DateTime(int.Parse(strings[0]), int.Parse(strings[1]),
                        int.Parse(strings[2]), 0, 0, 0);
                    HttpContext.Session.SetObjectAsJson(Constants.SessionKeyDate, date);
                    HttpContext.Session.SetObjectAsJson(Constants.SessionKeyService, model.Id);
                    return RedirectToAction("Add");
                }
                catch (FormatException)
                {
                    ModelState.AddModelError(string.Empty, "Neispravan datum");
                    return RedirectToAction("Index");
                }

            }
            ModelState.AddModelError(string.Empty, "KRIVO");
            return View(model);
        }
        
        public IActionResult Add()
        {
            var date = HttpContext.Session.GetObjectFromJson<DateTime>(Constants.SessionKeyDate);
            var serviceId = HttpContext.Session.GetObjectFromJson<Guid>(Constants.SessionKeyService);
            var termDates = _repository.GetFiltered(f => f.StartOfTerm.Date == date.Date);
            var freeTerms = termDates.Count == 0
                ? Helper.GetAvailableDateTimes(_serviceRepository.Get(serviceId))
                : Helper.GetAvailableDateTimes(termDates, _serviceRepository.Get(serviceId));
            if (freeTerms.Count == 0)
            {
                return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Na žalost nema slobodnih termina za ovaj datum :(", "Index", "Customer"));
            }

            HttpContext.Session.SetObjectAsJson(Constants.SessionKeyFreeTerms, freeTerms);
            return View();

            
        }

        [HttpPost]
        public async Task<IActionResult> Add(TimePickerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = await Helper.GetCurrentUserId(_userManager, HttpContext);
                var date = HttpContext.Session.GetObjectFromJson<DateTime>(Constants.SessionKeyDate);
                var serviceId = HttpContext.Session.GetObjectFromJson<Guid>(Constants.SessionKeyService);
                date = date.AddHours(model.Time.Hour);
                date = date.AddMinutes(model.Time.Minute);
                var termDate = new TermDate(userId, date, _serviceRepository.Get(serviceId), model.Description);
                _repository.Add(termDate);
                _logger.LogInformation("User(ID = {UserId}) has created a new TermDate(ID = {TermDateId})", 
                    _userManager.GetUserId(HttpContext.User), termDate.TermDateId.ToString());
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "KRIVO");
            return View(model);
        }

        public async Task<IActionResult> GetAll()
        {
            var termDates = _repository.GetAll(await Helper.GetCurrentUserId(_userManager, HttpContext));
            return View(termDates);
        }

        public async Task<IActionResult> GetActive()
        {
            var termDates = _repository.GetActive(await Helper.GetCurrentUserId(_userManager, HttpContext));
            return View(termDates);
        }

        public async Task<IActionResult> GetCompleted()
        {
            var termDates = _repository.GetCompleted(await Helper.GetCurrentUserId(_userManager, HttpContext));
            return View(termDates);
        }

        public async Task<IActionResult> Remove(Guid id)
        {
            var userId = await Helper.GetCurrentUserId(_userManager, HttpContext);
            if (_repository.Remove(id, userId))
            {
                _logger.LogInformation("User(ID = {UserId}) has removed a TermDate(ID = {TermDateId})",
                _userManager.GetUserId(HttpContext.User), id.ToString());
                return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Uspješno ste uklonili termin", "GetAll", "Customer"));
            }
            _logger.LogError("User(ID = {UserId}) tried to remove TermDate(ID = {TermDateId}) but it failed.", 
                _userManager.GetUserId(HttpContext.User), id.ToString());
            return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Došlo je do pogreške i željena radnja nije ostvarena. Ukoliko se ovo opet ponovi molimo Vas da kontaktirate nekoga od osoblja, hvala.", "GetAll", "Customer"));

        }
        [AllowAnonymous]
        public IActionResult DrawCalendar()
        {
            var termDates = _repository.GetAll();
            var termDatesString = new StringBuilder();
            termDatesString.Append("events: [");
            foreach (var termDate in termDates)
            {
                termDatesString.Append('{');
                termDatesString.AppendFormat("title: '" + termDate.Service.Name + "', start: '" +
                    termDate.StartOfTerm.ToString("yyyy-MM-ddTHH:mm:ss") + "', end: '" + termDate.EndOfTerm.ToString("yyyy-MM-ddTHH:mm:ss") + "'");
                termDatesString.Append("}, ");
            }

            termDatesString.Remove(termDatesString.Length - 2, 1);
            termDatesString.Append("]");

            return View("DrawCalendar", termDatesString.ToString());
        }
    }
}