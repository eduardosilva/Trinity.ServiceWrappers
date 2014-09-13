using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.ServiceWrappers.ExamplesServices.Contracts;

namespace Trinity.ServiceWrappers.ExamplesClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = ServiceWrapperManager.GetServiceWrapper<IService>()
                                            .Use(service => service.GetData(3));

            var dataWithTrueValue = ServiceWrapperManager.GetServiceWrapper<IService>()
                                                         .Use(service => service.GetDataUsingDataContract(new CompositeType { BoolValue = true }));

            var dataWithFalseValue = ServiceWrapperManager.GetServiceWrapper<IService>()
                                                          .Use("myEndPointName", service => service.GetDataUsingDataContract(new CompositeType { BoolValue = false }));


            Console.WriteLine(data);
            Console.WriteLine(dataWithTrueValue.StringValue);
            Console.WriteLine(dataWithFalseValue.StringValue);
            Console.Read();
        }
    }
}
