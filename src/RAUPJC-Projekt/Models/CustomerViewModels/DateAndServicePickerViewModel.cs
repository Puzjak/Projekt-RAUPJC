using System;
using System.ComponentModel.DataAnnotations;

namespace RAUPJC_Projekt.Models.CustomerViewModels
{
    /// <summary>
    /// View model which is used to store Date and Service which will be 
    /// used to create TermDate
    /// </summary>
    public class DateAndServicePickerViewModel
    {
        /// <summary>
        /// Represents Date of TermDate
        /// </summary>
        [Required]
        [MinLength(10)]
        public string Date { get; set; }
        /// <summary>
        /// Represents Service of TermDate
        /// </summary>
        [Required]
        public Guid Id { get; set; }
    }
}

