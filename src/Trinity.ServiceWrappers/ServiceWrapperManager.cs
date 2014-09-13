using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.ServiceWrappers
{
    public static class ServiceWrapperManager
    {
        static ServiceWrapperManager()
        {
        }

        public static IServiceWrapper<T> GetServiceWrapper<T>()
            where T : class
        {
            return new ServiceWrapper<T>();
        }
    }
}
