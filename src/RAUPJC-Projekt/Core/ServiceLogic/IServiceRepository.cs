using System;
using System.Collections.Generic;

namespace RAUPJC_Projekt.Core.ServiceLogic
{
    public interface IServiceRepository
    {
        Service Get(Guid serviceId);

        bool Add(Service service);

        bool Remove(Service service);

        IList<Service> GetAll();

        void RemoveAll();
    }
}
