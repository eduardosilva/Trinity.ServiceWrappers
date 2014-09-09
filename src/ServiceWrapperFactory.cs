using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceWrappers
{
    public interface IServiceWrapperFactory { }
    public interface IServiceWrapperFactory<S, T> : IServiceWrapperFactory
    {
        S CreateServiceWrapper<S, T>();    
    }

    public class ServiceWrapperFactory
    {
        public virtual S CreateServiceWrapper<S, T>()
            where S : IServiceWrapper
        {
            var assembly = Assembly.GetAssembly(typeof(S));
            var proxy = (S)assembly.CreateInstance(typeof(S).FullName);

            return proxy;
        }
    }
}
