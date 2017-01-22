using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RAUPJC_Projekt.Core;
using RAUPJC_Projekt.Core.ServiceLogic;
using RAUPJC_Projekt.Core.TermDateLogic;
using RAUPJC_Projekt.Data;
using RAUPJC_Projekt.Models;
using RAUPJC_Projekt.Models.AdministratorViewModels;

namespace RAUPJC_Projekt.Controllers
{
    [Authorize(Roles = Constants.AdministratorRoleName)]
    public class AdministratorController : Controller
    {

        private readonly ITermDateRepository _repository;
        private readonly IServiceRepository _serviceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger _logger;

        public AdministratorController(ITermDateRepository termDateRepository, IServiceRepository serviceRepository, 
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext applicationDbContext, ILogger<AdministratorController> logger)
        {
            _repository = termDateRepository;
            _serviceRepository = serviceRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                if (await _roleManager.RoleExistsAsync(model.RoleName))
                {
                    ModelState.AddModelError(string.Empty, "Uloga veæ postoji");
                    return View(model);
                    
                }
                else
                {
                    _logger.LogInformation("User(ID = {UserId}) created a new role(Name = {RoleName})",
                       _userManager.GetUserId(HttpContext.User), model.RoleName);
                    await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
                    return RedirectToAction("Index");
                }
                
            }
            ModelState.AddModelError(string.Empty, "Ovo polje je obavezno");
            return View(model);
        }

        public async Task<IActionResult> RemoveRole(string role)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.DeleteAsync(await _roleManager.FindByNameAsync(role));
                _logger.LogCritical("User(ID = {UserId} removed role(Name = {RoleName}))", 
                    _userManager.GetUserId(HttpContext.User), role);
                return RedirectToAction("Index", "Message", 
                    MessageViewModel.Create(Url, "Uloga je uspješno uklonjena", 
                    "Index", "Administrator"));
            }
            return RedirectToAction("Index", "Message",
                    MessageViewModel.Create(Url, "Uloga ne postoji",
                    "Index", "Administrator"));
        }

        public IActionResult GetCustomers()
        {
            return RedirectToAction("GetCustomers", "Employee");
        }

        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            return View(employees);
        }

        public async Task<IActionResult> RemoveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.DeleteAsync(user);
            _logger.LogCritical("User(ID = {UserId} has deleted another user(ID = {UserId}; UserName = {UserName}; E-mail = {Email}))", 
                _userManager.GetUserId(HttpContext.User), user.Id, user.UserName, user.Email);
            return RedirectToAction("Index", "Message", MessageViewModel.Create(
                Url, "Uspješno ste uklonili korisnika", "Index", "Administrator"));
        }

        public IActionResult AddToRole(string userId)
        {
            HttpContext.Session.SetString(Constants.SessionKeyUserId, userId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString(Constants.SessionKeyUserId);
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.AddToRoleAsync(user, model.RoleName);
                _logger.LogInformation("User(ID = {UserId}) added another user (ID = {SUserId}) to a role(Name = {RoleName})", 
                    _userManager.GetUserId(HttpContext.User), user.Id, model.RoleName);
                return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Uspješno ste dodali ulogu korisniku", "Index", "Administrator"));
            }
            ModelState.AddModelError(string.Empty, "Greška");
            return View(model);

        }

        public async Task<IActionResult> RemoveFromRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            HttpContext.Session.SetObjectAsJson(Constants.SessionKeyUser, user);
            HttpContext.Session.SetObjectAsJson(Constants.SessionKeyRoles, roles);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>(Constants.SessionKeyUser);
                await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                _logger.LogInformation("User(ID = {UserId}) has removed another user (ID = {SUserId}) from a role(Name = {RoleName})",
                    _userManager.GetUserId(HttpContext.User), user.Id, model.RoleName);
                return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Uspješno ste maknuli ulogu korisniku", "Index", "Administrator"));
            }
            ModelState.AddModelError(string.Empty, "Ovo polje je obavezno");
            return View(model);
        }

        public async Task<IActionResult> GetRoles()
        {
            var roles = await _applicationDbContext.Roles.ToListAsync();
            return View(roles);
        }

        public IActionResult DeleteAllTermDates()
        {
            _repository.RemoveAll();
            _logger.LogCritical("User(ID = {UserId}) has deleted all TermDates.", _userManager.GetUserId(HttpContext.User));
            return RedirectToAction("Index", "Message", MessageViewModel.Create(
                Url, "Uspješno ste uklonili sve rezervirane termine.", "Index", "Administrator"));
        }

        public IActionResult AddService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddService(AddServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new Service(model.Name, model.Duration);
                _serviceRepository.Add(service);
                _logger.LogInformation("User(ID = {UserId}) added new service (ID = {ServiceId})",
                    _userManager.GetUserId(HttpContext.User), service.ServiceId.ToString());
                return RedirectToAction("Index", "Message", MessageViewModel.Create(
                    Url, "Uspješno ste dodali novu uslugu", "Index", "Administrator"));
            }
            ModelState.AddModelError(string.Empty, "Greška");
            return View();
        }

        public IActionResult RemoveService(Guid id)
        {
            var service = _serviceRepository.Get(id);
            if (service != null)
            {
                _serviceRepository.Remove(service);
                _logger.LogInformation("User(ID = {UserId}) removed service (ID = {ServiceId}, Name = {ServiceName})",
                    _userManager.GetUserId(HttpContext.User), service.ServiceId.ToString(), service.Name);
                return RedirectToAction("Index", "Message", MessageViewModel.Create(Url, "Usluga je uklonjena",
                    "GetAllServices", "Administrator"));
            }
            return RedirectToAction("Index", "Message", MessageViewModel.Create(Url, "Usluga koju želite ukloniti ne postoji.",
                    "GetAllServices", "Administrator"));
        }

        public IActionResult GetAllServices()
        {
            var services = _serviceRepository.GetAll();
            return View(services);
        }
    }
}