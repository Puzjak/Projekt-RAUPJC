using Microsoft.AspNetCore.Mvc;
using RAUPJC_Projekt.Models;

namespace RAUPJC_Projekt.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index(MessageViewModel model)
        {
            return View(model);
        }
    }
}