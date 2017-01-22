using System;
using System.Collections.Generic;
using RAUPJC_Projekt.Core.TermDateLogic;

namespace RAUPJC_Projekt.Core.ServiceLogic
{
    public class Service
    {
        public Guid ServiceId { get; set; }

        public string Name { get; set; }

        public int Duration { get; set; }

        public virtual List<TermDate> TermDates { get; set; }


        public Service()
        {
            TermDates = new List<TermDate>();
        }

        public Service(string name, int duration)
        {
            ServiceId = Guid.NewGuid();
            Name = name;
            Duration = duration;

        }

    }
}
