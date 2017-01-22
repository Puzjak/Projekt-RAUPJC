using System;
using System.Collections.Generic;

namespace RAUPJC_Projekt.Core.TermDateLogic
{
    public interface ITermDateRepository
    {
        /// <summary>
        /// Returns TermDate for a given Id. Throw TermDateAccessDeniedException whth appropriate message
        /// if user is not owner of TermDate. 
        /// </summary>
        /// <param name="termDateId">Term Date Id</param>
        /// <param name="userId">Id of the user who is trying to acces data</param>
        /// <returns>TermDate if found, null otherwise</returns>
        TermDate GetTermDate(Guid termDateId, Guid userId);

        /// <summary>
        /// Adds new term date to storage
        /// </summary>
        /// <param name="termDate">Term date to be stored</param>
        void Add(TermDate termDate);

        /// <summary>
        /// Removes term date from storage
        /// </summary>
        /// <param name="termDateId">Term date to be removed</param>
        /// <returns>True if succeded, false otherwise</returns>
        bool Remove(Guid termDateId);

        /// <summary>
        /// Checks if user who is trying to remove term date is owner of that term date
        /// and it is the user, term date is removed from storage
        /// </summary>
        /// <param name="termDateId">Term date to be removed</param>
        /// <param name="userId">User who is trying to remove term date</param>
        /// <returns>True if succeded, false otherwise</returns>
        bool Remove(Guid termDateId, Guid userId);

        /// <summary>
        /// Removes all completed term dates from storage.
        /// </summary>
        void RemoveCompleted();

        /// <summary>
        /// Removes all completed term dates for desired user.
        /// </summary>
        /// <param name="userId">Id of user whose completed term dates will
        /// be removed</param>
        void RemoveCompleted(Guid userId);

        /// <summary>
        /// Removes all term dates from storage
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Removes all term dates from storage for desired user.
        /// </summary>
        /// <param name="userId">Id of user whose term dates
        /// will be removed</param>
        void RemoveAll(Guid userId);

        /// <summary>
        /// Returns list of active term dates
        /// </summary>
        /// <returns></returns>
        List<TermDate> GetActive();

        /// <summary>
        /// Returns list of active term dates for desired user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<TermDate> GetActive(Guid userId);

        /// <summary>
        /// Returns list of completed term dates
        /// </summary>
        /// <returns></returns>
        List<TermDate> GetCompleted();
        
        /// <summary>
        /// Returns list of completed term dates for desired user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<TermDate> GetCompleted(Guid userId);

        /// <summary>
        /// Returns list of all term dates
        /// </summary>
        /// <returns></returns>
        List<TermDate> GetAll();

        /// <summary>
        /// Returns list of completed term dates for desired user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<TermDate> GetAll(Guid userId);

        /// <summary>
        /// Returns list of term dates according to filter function
        /// </summary>
        /// <param name="filterFunction"></param>
        /// <returns></returns>
        List<TermDate> GetFiltered(Func<TermDate, bool> filterFunction);

    }
}
