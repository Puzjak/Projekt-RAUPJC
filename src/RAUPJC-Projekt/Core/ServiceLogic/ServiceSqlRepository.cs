using System;
using System.Collections.Generic;
using System.Linq;

namespace RAUPJC_Projekt.Core.ServiceLogic
{
    public class ServiceSqlRepository : IServiceRepository
    {
        private readonly MyDbContext _context;

        public ServiceSqlRepository(MyDbContext context)
        {
            _context = context;
        }

        public Service Get(Guid serviceId)
        {
            return serviceId == null ? null : _context.Services.Find(serviceId);
        }

        public bool Add(Service service)
        {
            if (service == null) return false;
            if (_context.Services.Find(service.ServiceId) != null) return false;
            _context.Services.Add(service);
            _context.SaveChanges();
            return true;
        }

        public bool Remove(Service service)
        {
            if (service == null) return false;
            if (_context.Services.Find(service.ServiceId) == null) return false;
            _context.Services.Remove(service);
            _context.SaveChanges();
            return true;
        }

        public IList<Service> GetAll()
        {
            return _context.Services.OrderBy(f => f.ServiceId).ToList();
        }

        public void RemoveAll()
        {
            var services = _context.Services.ToList();
            foreach (var service in services)
            {
                Remove(service);
            }
            _context.SaveChanges();
        }
    }
}
