using System.ComponentModel.DataAnnotations;

namespace RAUPJC_Projekt.Models
{
    public class WeekPickerViewModel
    {
        [Required]
        public string Week { get; set; }
    }
}
