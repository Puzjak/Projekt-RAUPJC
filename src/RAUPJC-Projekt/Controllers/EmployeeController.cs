using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RAUPJC_Projekt.Core;
using RAUPJC_Projekt.Core.TermDateLogic;
using RAUPJC_Projekt.Models;

namespace RAUPJC_Projekt.Controllers
{
    [Authorize(Policy = "ElevatedRights")]
    public class EmployeeController : Controller
    {
        private readonly ITermDateRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public EmployeeController(ITermDateRepository termDateRepository, UserManager<ApplicationUser> userManager, 
            ILogger<EmployeeController> logger)
        {
            _repository = termDateRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Remove(Guid id)
        {
            if (_repository.Remove(id))
            {
                if (HttpContext.User.IsInRole(Constants.AdministratorRoleName))
                {
                    _logger.LogInformation("User(ID = {UserId}) removed TermDate(ID = {TermDateId})",
                        _userManager.GetUserId(HttpContext.User), id.ToString());
                    return RedirectToAction("Index", "Message", MessageViewModel.Create(
                        Url, "Uspješno ste uklonili rezervirani termin", "Index", "Administrator"));
                }
                _logger.LogInformation("User(ID = {UserId}) removed TermDate(ID = {TermDateId})",
                    _userManager.GetUserId(HttpContext.User), id.ToString());
                return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Uspješno ste uklonili rezervirani termin", "Index", "Employee"));
            }
            if (HttpContext.User.IsInRole(Constants.AdministratorRoleName))
            {
                _logger.LogError("User(ID = {UserId}) tried to remove TermDate(ID = {TermDateId}) but it failed.",
                    _userManager.GetUserId(HttpContext.User), id.ToString());
                return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Uklanjanje nije uspjelo", "Index", "Administrator"));
            }
            _logger.LogError("User(ID = {UserId}) tried to remove TermDate(ID = {TermDateId}) but it failed.",
               _userManager.GetUserId(HttpContext.User), id.ToString());
            return RedirectToAction("Index", "Message", MessageViewModel.Create(
                Url, "Uklanjanje nije uspjelo", "Index", "Employee"));
        }

        public IActionResult RemoveCompleted()
        {
            _repository.RemoveCompleted();

            if (HttpContext.User.IsInRole(Constants.AdministratorRoleName))
            {
                _logger.LogInformation("User(ID = {UserId}) removed completed TermDates",
                       _userManager.GetUserId(HttpContext.User));
                return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Uspješno ste uklonili sve završene termine", "Index", "Administrator"));
            }
            _logger.LogInformation("User(ID = {UserId}) removed completed TermDates",
                       _userManager.GetUserId(HttpContext.User));
            return RedirectToAction("Index", "Message", MessageViewModel.Create(
                Url, "Uspješno ste uklonili sve završene termine", "Index", "Employee"));
        }

        public async Task<IActionResult> UserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            HttpContext.Session.SetObjectAsJson(Constants.SessionKeyUser, user);
            var termDates = _repository.GetAll(new Guid(user.Id));
            return View(termDates);
        }

        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _userManager.GetUsersInRoleAsync(Constants.CustomerRoleName);
            return View(customers);
        }

        public IActionResult GetAllActiveTermDates()
        {
            var termDates = _repository.GetActive().OrderBy(f => f.UserId);
            return View(termDates);
        }

        public IActionResult GetAllCompletedTermDates()
        {
            var termDates = _repository.GetCompleted().OrderBy(f => f.UserId);
            return View(termDates);
        }

        public IActionResult GetAllTermDates()
        {
            var termDates = _repository.GetAll().OrderBy(f => f.UserId);
            return View(termDates);
        }

        public IActionResult Index()
        {
            return View();
        }

        //Implement if necessary
        //There is similar logic in UserDetails

        //public IActionResult GetActiveTermDatesForUser(string userId)
        //{
        //    var termDates = _repository.GetActive(new Guid(userId));
        //    return View(termDates);
        //}

        //public IActionResult GetAllCompletedTermDatesForUser(string userId)
        //{
        //    var termDates = _repository.GetCompleted(new Guid(userId));
        //    return View(termDates);
        //}

        //public IActionResult GetAllTermDatesForUser(string userId)
        //{
        //    var termDates = _repository.GetAll(new Guid(userId));
        //    return View(termDates);
        //}

    }
}