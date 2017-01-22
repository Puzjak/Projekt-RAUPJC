using Microsoft.AspNetCore.Mvc;

namespace RAUPJC_Projekt.Models
{
    public class MessageViewModel
    {
        public string Message { get; set; }
        public string ReturnUrl { get; set; }

        public static MessageViewModel Create(IUrlHelper urlService, string message, string returnAction,
            string returnController, object routeValues = null)
        {
            var returnUrl = urlService.Action(returnAction, returnController, routeValues);
            return new MessageViewModel()
            {
                Message = message,
                ReturnUrl = returnUrl
            };
        }
    }
}
