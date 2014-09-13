using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;

namespace Trinity.ServiceWrappers
{
    public static class ServiceWrapperManager
    {
        public static IServiceWrapper<T> GetServiceWrapper<T>()
            where T : class
        {
            return new ServiceWrapper<T>();
        }

        public static IServiceWrapper<T> GetServiceWrapper<T>(IEnumerable<MessageHeader> messageHeaders)
            where T: class
        {
            return new ServiceWrapper<T>() { MessageHeaders = messageHeaders };
        }

        public static IServiceWrapper<T> GetServiceWrapper<T>(UserNamePasswordClientCredential userName)
            where T :  class
        {
            return GetServiceWrapper<T>(userName, new MessageHeader[] { });
        }

        public static IServiceWrapper<T> GetServiceWrapper<T>(UserNamePasswordClientCredential userName, IEnumerable<MessageHeader> messageHeaders)
            where T : class
        {
            if (userName == null)
                throw new ArgumentNullException("userName");

            if (messageHeaders == null)
                throw new ArgumentNullException("messageHeaders");

            return new ServiceWrapper<T>() { UserName = userName, MessageHeaders = messageHeaders };
        }
    }
}
