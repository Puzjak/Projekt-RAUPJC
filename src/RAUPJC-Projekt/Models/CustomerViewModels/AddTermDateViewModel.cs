using System;
using System.ComponentModel.DataAnnotations;
using RAUPJC_Projekt.Core.ServiceLogic;

namespace RAUPJC_Projekt.Models.CustomerViewModels
{
    public class AddTermDateViewModel
    {
        [Required]
        public DateTime StartOfTerm;

        [Required]
        public Service Service;

        [MaxLength(255)]
        public string Description;
    }
}
