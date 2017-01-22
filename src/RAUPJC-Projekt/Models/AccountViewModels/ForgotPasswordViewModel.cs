using System.ComponentModel.DataAnnotations;

namespace RAUPJC_Projekt.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
