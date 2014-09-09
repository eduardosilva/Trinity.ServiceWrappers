using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trinity.ServiceWrappers.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            new ServiceWrapper<IServiceTest>().Use(s => s.Test());
        }
    }
}
