using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceWrappers
{
    public delegate void UseServiceDelegate<T>(T proxy);
    public delegate R UseServiceDelegate<T, R>(T proxy);

    public interface IServiceWrapper 
    {
    }

    public interface IServiceWrapper<T> : IServiceWrapper
    {
        void Use(UseServiceDelegate<T> codeBlock);
        R Use<R>(UseServiceDelegate<T, R> codeBlock);
    }
}
