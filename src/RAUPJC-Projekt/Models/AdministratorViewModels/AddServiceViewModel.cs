using System.ComponentModel.DataAnnotations;
using RAUPJC_Projekt.Core;

namespace RAUPJC_Projekt.Models.AdministratorViewModels
{
    public class AddServiceViewModel
    {
        [Required, MinLength(4), MaxLength(255)]
        public string Name { get; set; }

        [Required, Range(1, Constants.WorkingMinutes)]
        public int Duration { get; set; }
    }
}
