using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Trinity.ServiceWrappers.ExamplesServices.Contracts;

namespace Trinity.ServiceWrappers.ExamplesServices.Contracts
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        void WithoutReturn();
    }
}
