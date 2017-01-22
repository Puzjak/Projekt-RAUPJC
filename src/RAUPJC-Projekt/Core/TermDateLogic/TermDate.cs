using System;
using RAUPJC_Projekt.Core.ServiceLogic;

namespace RAUPJC_Projekt.Core.TermDateLogic
{
    public class TermDate
    {
        /// <summary>
        /// Id of term date
        /// </summary>
        public Guid TermDateId { get; set; }
        /// <summary>
        /// Id of user which owns this term date.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Starting time and date of the term.
        /// </summary>
        public DateTime StartOfTerm { get; set; }

        /// <summary>
        /// Ending time and date of the term.
        /// </summary>
        public DateTime EndOfTerm { get; set; }

        public virtual Service Service { get; set; }

        public string Description { get; set; }

        public TermDate(Guid userId, DateTime startOfTerm,  Service service, string description)
        {
            TermDateId = Guid.NewGuid();
            UserId = userId;
            StartOfTerm = startOfTerm;
            Service = service;
            EndOfTerm = StartOfTerm.AddMinutes(service.Duration);
            Description = description;
        }

        public TermDate()
        {
            
        }

        public bool IsCompleted()
        {
            return EndOfTerm < DateTime.Now;
        }

        public bool IsOwner(Guid userId)
        {
            return this.UserId == userId;
        }
    }
}
