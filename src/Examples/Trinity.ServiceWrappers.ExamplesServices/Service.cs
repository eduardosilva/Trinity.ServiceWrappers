using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Trinity.ServiceWrappers.ExamplesServices.Contracts;

namespace Trinity.ServiceWrappers.ExamplesServices
{
    public class Service : IService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }


        public void WithoutReturn()
        {
            // example without return;
        }
    }
}
