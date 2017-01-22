using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RAUPJC_Projekt.Core.TermDateLogic;

namespace RAUPJC_Projekt.Models.CustomerViewModels
{
    /// <summary>
    /// View model which can be viewed as final AddTermDateViewModel. 
    /// It every parameter needed for creating TermDate.
    /// </summary>
    public class TimePickerViewModel
    {
        public TimePickerViewModel()
        {
            
        }

        public TimePickerViewModel(IList<TermDate> termDates)
        {
            TermDates = termDates;
        }

        /// <summary>
        /// Time which will be added to DateAndServicePicker.Date and will represent full
        /// DateTime of the TermDate
        /// </summary>
        [Required, DataType(DataType.Time)]
        public DateTime Time { get; set; }

        /// <summary>
        /// Optional description which will be set as TermDate description
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// Helper list which will be used for checking which DateTimes are taken and which are free.
        /// </summary>
        public IList<TermDate> TermDates { get; set; }
    }
}
