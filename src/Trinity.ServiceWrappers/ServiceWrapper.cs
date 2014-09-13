using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceWrappers
{
    public class ServiceWrapper<T> : IServiceWrapper<T>
        where T : class
    {
        private static Dictionary<string, ChannelFactory<T>> _channelFactories;

        static ServiceWrapper()
        {
            _channelFactories = new Dictionary<string, ChannelFactory<T>>();
        }

        public void Use(string endpointConfigurationName, IEnumerable<MessageHeader> messageHeaders, UseServiceDelegate<T> codeBlock)
        {
            var proxy = GetProxy(endpointConfigurationName);

            using (var scope = new OperationContextScope(proxy))
            {
                foreach (var messageHeander in messageHeaders)
                    OperationContext.Current.OutgoingMessageHeaders.Add(messageHeander);

                bool success = false;
                try
                {
                    codeBlock((T)proxy);
                    proxy.Close();
                    success = true;
                }
                finally
                {
                    if (!success)
                        proxy.Abort();
                }
            }
        }

        public void Use(IEnumerable<MessageHeader> messageHeaders, UseServiceDelegate<T> codeBlock)
        {
            Use(endpointConfigurationName: typeof(T).FullName, messageHeaders: messageHeaders, codeBlock: codeBlock);
        }

        public void Use(UseServiceDelegate<T> codeBlock)
        {
            Use(endpointConfigurationName: typeof(T).FullName, messageHeaders: new MessageHeader[] { }, codeBlock: codeBlock);
        }

        public R Use<R>(string endpointConfigurationName, IEnumerable<MessageHeader> messageHeaders, UseServiceDelegate<T, R> codeBlock)
        {
            var proxy = GetProxy(endpointConfigurationName);

            using (var scope = new OperationContextScope(proxy))
            {
                foreach (var messageHeander in messageHeaders)
                    OperationContext.Current.OutgoingMessageHeaders.Add(messageHeander);

                bool success = false;
                try
                {
                    var result = codeBlock((T)proxy);
                    proxy.Close();
                    success = true;
                    return result;
                }
                finally
                {
                    if (!success)
                        proxy.Abort();
                }
            }
        }

        public R Use<R>(IEnumerable<MessageHeader> messageHeaders, UseServiceDelegate<T, R> codeBlock)
        {
            return Use<R>(endpointConfigurationName: typeof(T).FullName, messageHeaders: messageHeaders, codeBlock: codeBlock);
        }

        public R Use<R>(UseServiceDelegate<T, R> codeBlock)
        {
            return Use<R>(endpointConfigurationName: typeof(T).FullName, messageHeaders: new MessageHeader[] { }, codeBlock: codeBlock);
        }

        private IClientChannel GetProxy(string endpointConfigurationName)
        {
            IClientChannel proxy = null;
            try
            {
                var channelFactory = GetChannelFactory(endpointConfigurationName);
                proxy = (IClientChannel)channelFactory.CreateChannel();
            }
            catch (TypeInitializationException ex)
            {
                var error = String.Format("Was not found a endpoint in the config file with a {0} name for interface {i}", endpointConfigurationName, typeof(T).ToString());
                throw new InvalidOperationException(error, ex);
            }
            return proxy;
        }

        private ChannelFactory<T> GetChannelFactory(string endpointConfigurationName)
        {
            if (String.IsNullOrWhiteSpace(endpointConfigurationName))
                throw new ArgumentNullException("endpointConfigurationName");

            if (!_channelFactories.Keys.Contains(endpointConfigurationName))
                _channelFactories.Add(endpointConfigurationName, new ChannelFactory<T>(endpointConfigurationName));

            return _channelFactories[endpointConfigurationName];
        }
    }
}
