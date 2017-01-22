using System.ComponentModel.DataAnnotations;

namespace RAUPJC_Projekt.Models.AdministratorViewModels
{
    public class RoleViewModel
    {
        [Required, MinLength(2)]
        public string RoleName { get; set; }
    }
}
