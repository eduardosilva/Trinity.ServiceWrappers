using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceWrappers
{
    public class ServiceWrapper<T> : IServiceWrapper<T>
        where T : class
    {
        internal UserNamePasswordClientCredential ServiceCredential { get; set; }
        internal IEnumerable<MessageHeader> MessageHeaders { get; set; }

        private IClientChannel proxy;
        private bool successfullyExecutedService = false;

        public ServiceWrapper()
        {
            MessageHeaders = new List<MessageHeader>();
        }

        public void Use(string endpointName, UseServiceDelegate<T> codeBlock)
        {
            CreateProxy(endpointName);

            using (var scope = new OperationContextScope(proxy))
            {
                ConfigureCurrentOperationScope();

                try
                {
                    successfullyExecutedService = false;
                    codeBlock((T)proxy);
                    CloseProxy();
                }
                finally { FinallyProxy(); }
            }
        }

        public void Use(UseServiceDelegate<T> codeBlock)
        {
            Use(endpointName: typeof(T).FullName, codeBlock: codeBlock);
        }

        public R Use<R>(string endpointName, UseServiceDelegate<T, R> codeBlock)
        {
            CreateProxy(endpointName);

            using (var scope = new OperationContextScope(proxy))
            {
                ConfigureCurrentOperationScope();

                try
                {
                    successfullyExecutedService = false;

                    var result = codeBlock((T)proxy);
                    CloseProxy();
                    return result;
                }
                finally{ FinallyProxy(); }
            }
        }

        public R Use<R>(UseServiceDelegate<T, R> codeBlock)
        {
            return Use<R>(endpointName: typeof(T).FullName, codeBlock: codeBlock);
        }

        private void CreateProxy(string endpointName)
        {
            try
            {
                var channelFactory = GetChannelFactory(endpointName);
                proxy = (IClientChannel)channelFactory.CreateChannel();

            }
            catch (TypeInitializationException ex)
            {
                var error = String.Format("It was not found an endpoint name {0} in the config file to the {1} interface.", endpointName, typeof(T));
                throw new TypeInitializationException(error, ex);
            }
        }

        private ChannelFactory<T> GetChannelFactory(string endpointName)
        {
            if (String.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentNullException("endpointConfigurationName");

            var channelFactory = new ChannelFactory<T>(endpointName);

            if (ServiceCredential != null)
            {
                channelFactory.Credentials.UserName.UserName = ServiceCredential.UserName;
                channelFactory.Credentials.UserName.Password = ServiceCredential.Password;
            }

            return channelFactory;
        }

        private void ConfigureCurrentOperationScope()
        {
            foreach (var messageHeader in MessageHeaders)
                OperationContext.Current.OutgoingMessageHeaders.Add(messageHeader);
        }

        private void CloseProxy()
        {
            proxy.Close();
            successfullyExecutedService = true;
        }

        private void FinallyProxy()
        {
            if (!successfullyExecutedService)
                proxy.Abort();
        }
    }
}
