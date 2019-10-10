using EasyPluggableServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Examples.Plugin
{
    public class PluginModule : AbstractPluginModule
    {
        public PluginModule() : base()
        {
        }

        public override string Name => "Test";

        public override string Version => "1.0.0.0";

        public override void RegisterAndConfigureServices(IServiceCollection services, ILogger logger)
        {
            services.AddTransient<Examples.Interfaces.ITest, TestImpl>();
        }
    }
}
