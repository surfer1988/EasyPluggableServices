using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using EasyPluggableServices.Extension;
using EasyPluggableServices;
using System.Reflection;

namespace Test.EasyPluggableServices
{

    #region [ test resources ]

    public interface ITest
    {
        string Get();
    }

    public interface IGo
    {
        string Start();
    }

    public class TestImpl : ITest
    {
        public string Get()
        {
            return "this is a test";
        }
    }

    public class PluginModule : AbstractPluginModule
    {
        public PluginModule() : base()
        {
        }

        public override string Name => "Test";

        public override string Version => "1.0.0.0";

        public override void RegisterAndConfigureServices(IServiceCollection services, ILogger logger)
        {
            services.AddTransient<ITest, TestImpl>();
        }
    }

    public class Data
    {
        public Mock<ILoggerFactory> LogFactoryMock;
        public IServiceCollection serviceCollection;
        public IConfiguration configuration;
    }

    #endregion

    [TestClass]
    public class MainTest
    {
        static bool initialize = false;
        static Data _data = null;

        public static Data InitializeTest(IEnumerable<KeyValuePair<string, string>> intitialData)
        {
            if (initialize) return _data;

            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(intitialData);

            IConfiguration config = configBuilder.Build();

            var logMock = new Mock<ILogger>();
            var logFactoryMock = new Mock<ILoggerFactory>();
            logFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
                          .Returns(logMock.Object);

            IServiceCollection services = new ServiceCollection();

            services.LoadServicesInPluginModule(logFactoryMock.Object, new List<string> { Assembly.GetExecutingAssembly().Location });

            _data = new Data
            {
                LogFactoryMock = logFactoryMock,
                serviceCollection = services,
                configuration = config
            };

            initialize = true;

            return _data;
        }

        [TestMethod]
        public void TestLoadExternalDependency_Success_WithLocation()
        {
            Data data = InitializeTest(new Dictionary<string, string>());

            IServiceProvider serviceProvider = data.serviceCollection.BuildServiceProvider();

            Assert.IsNotNull(serviceProvider.GetService<ITest>());
        }

        [TestMethod]
        public void TestLoadExternalDependency_ExpectedServiceNotFoun()
        {
            Data data = InitializeTest(new Dictionary<string, string>());

            IServiceProvider serviceProvider = data.serviceCollection.BuildServiceProvider();

            Assert.IsNull(serviceProvider.GetService<IGo>());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestLoadExternalDependency_ExpectedLoadMultipleTime()
        {
            Data data = InitializeTest(new Dictionary<string, string>());

            data.serviceCollection.LoadServicesInPluginModule(data.LogFactoryMock.Object, new List<string> { Assembly.GetExecutingAssembly().Location });

            IServiceProvider serviceProvider = data.serviceCollection.BuildServiceProvider();

            Assert.IsNotNull(serviceProvider.GetService<ITest>());
        }
    }
}
