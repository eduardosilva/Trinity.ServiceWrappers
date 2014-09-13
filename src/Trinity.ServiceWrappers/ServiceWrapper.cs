using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceWrappers
{
    public class ServiceWrapper<T> : IServiceWrapper<T>
    {
        private static Dictionary<string, ChannelFactory<T>> _channelFactories;

        static ServiceWrapper()
        {
            _channelFactories = new Dictionary<string, ChannelFactory<T>>();
        }

        public void Use(string endpointConfigurationName, UseServiceDelegate<T> codeBlock)
        {
            IClientChannel proxy = null;
            try
            {
                var channelFactory = GetChannelFactory(endpointConfigurationName);
                proxy = (IClientChannel)channelFactory.CreateChannel();
            }
            catch (TypeInitializationException ex)
            {
                ThrowCustomTypeInializtionException(endpointConfigurationName, ex);
            }

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

        public void Use(UseServiceDelegate<T> codeBlock)
        {
            Use(endpointConfigurationName: typeof(T).FullName, codeBlock: codeBlock);
        }

        public R Use<R>(UseServiceDelegate<T, R> codeBlock)
        {
            return Use<R>(endpointConfigurationName: typeof(T).FullName, codeBlock: codeBlock);
        }

        public R Use<R>(string endpointConfigurationName, UseServiceDelegate<T, R> codeBlock)
        {
            IClientChannel proxy = null;
            try
            {
                var channelFactory = GetChannelFactory(endpointConfigurationName);
                proxy = (IClientChannel)channelFactory.CreateChannel();
            }
            catch (TypeInitializationException ex)
            {
                ThrowCustomTypeInializtionException(endpointConfigurationName, ex);
            }

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

        private ChannelFactory<T> GetChannelFactory(string endpointConfigurationName)
        {
            if (String.IsNullOrWhiteSpace(endpointConfigurationName))
                throw new ArgumentNullException("endpointConfigurationName");

            if (!_channelFactories.Keys.Contains(endpointConfigurationName))
                _channelFactories.Add(endpointConfigurationName, new ChannelFactory<T>(endpointConfigurationName));

            return _channelFactories[endpointConfigurationName];
        }

        private static void ThrowCustomTypeInializtionException(string endpointConfigurationName, TypeInitializationException ex)
        {
            var error = String.Format("Was not found a endpoint in the config file with a {0} name for interface {i}", endpointConfigurationName, typeof(T).ToString());
            throw new InvalidOperationException(error, ex);
        }
    }
}
