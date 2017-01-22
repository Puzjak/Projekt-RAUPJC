using System;
using System.Collections.Generic;
using System.Linq;


namespace RAUPJC_Projekt.Core.TermDateLogic
{
    public class TermDateSqlRepository : ITermDateRepository
    {
        private readonly MyDbContext _context;

        public TermDateSqlRepository(MyDbContext context)
        {
            _context = context;
        }

        public TermDate GetTermDate(Guid termDateId, Guid userId)
        {
            var termDate = _context.TermDates.Find(termDateId);
            if (termDate == null)
                return null;
            if (termDate.UserId != userId)
                throw new TermDateAccessDeniedException();
            return termDate;
        }

        public void Add(TermDate termDate)
        {
            var tmp = _context.TermDates.Find(termDate.TermDateId);
            if (tmp != null)
                throw new DuplicateTermDateException();
            _context.TermDates.Add(termDate);
            _context.SaveChanges();
        }

        public bool Remove(Guid termDateId)
        {
            var tmp = _context.TermDates.Find(termDateId);
            if (tmp == null)
                return false;
            _context.TermDates.Remove(tmp);
            _context.SaveChanges();
            return true;
        }

        public bool Remove(Guid termDateId, Guid userId)
        {
            var tmp = _context.TermDates.Find(termDateId);
            if (tmp == null)
                return false;
            if (tmp.UserId != userId)
                throw new TermDateAccessDeniedException();
            _context.TermDates.Remove(tmp);
            _context.SaveChanges();
            return true;
        }

        public void RemoveCompleted()
        {
            foreach (var termDate in _context.TermDates)
            {
                if (termDate.IsCompleted())
                {
                    _context.TermDates.Remove(termDate);
                }
            }
        }

        public void RemoveCompleted(Guid userId)
        {
            foreach (var termDate in _context.TermDates)
            {
                if (!termDate.IsCompleted() && termDate.UserId == userId)
                {
                    _context.TermDates.Remove(termDate);
                }
            }
        }

        public void RemoveAll()
        {
            var termDates = _context.TermDates.ToList();
            foreach (var termDate in termDates)
            {
                _context.TermDates.Remove(termDate);
            }
            _context.SaveChanges();
        }

        public void RemoveAll(Guid userId)
        {
            var termDates = _context.TermDates.Where(t => t.UserId == userId).ToList();
            foreach (TermDate termDate in termDates)
            {
                _context.TermDates.Remove(termDate);
            }
            _context.SaveChanges();
        }

        public List<TermDate> GetActive()
        {
            var termDates = new List<TermDate>();
            foreach (var termDate in _context.TermDates)
            {
                if (!termDate.IsCompleted())
                {
                    termDates.Add(termDate);
                }
            }
            return termDates.OrderBy(t => t.StartOfTerm).ToList();
        }

        public List<TermDate> GetActive(Guid userId)
        {
            var termDates = new List<TermDate>();
            foreach (var termDate in _context.TermDates)
            {
                if (!termDate.IsCompleted() && termDate.UserId == userId)
                {
                    termDates.Add(termDate);
                }
            }
            return termDates.OrderBy(t => t.StartOfTerm).ToList();
        }

        public List<TermDate> GetCompleted()
        {
            var termDates = new List<TermDate>();
            foreach (var termDate in _context.TermDates)
            {
                if (termDate.IsCompleted())
                {
                    termDates.Add(termDate);
                }
            }
            return termDates.OrderBy(t => t.StartOfTerm).ToList();
        }

        public List<TermDate> GetCompleted(Guid userId)
        {
            List<TermDate> termDates = new List<TermDate>();
            foreach (var termDate in _context.TermDates)
            {
                if (termDate.IsCompleted() && termDate.UserId == userId)
                {
                    termDates.Add(termDate);
                }
            }
            return termDates.OrderBy(t => t.StartOfTerm).ToList();
        }

        public List<TermDate> GetAll()
        {
            return _context.TermDates.OrderBy(t => t.StartOfTerm).ToList();
        }

        public List<TermDate> GetAll(Guid userId)
        {
            return _context.TermDates.Where(t => t.UserId == userId)
                .OrderBy(t => t.StartOfTerm)
                .ToList();
        }

        public List<TermDate> GetFiltered(Func<TermDate, bool> filterFunction)
        {
            return _context.TermDates.Where(filterFunction).OrderBy(t => t.StartOfTerm).ToList();
        }

    }
}
