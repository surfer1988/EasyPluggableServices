using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace EasyPluggableServices
{
    public abstract class AbstractPluginModule
    {
        public AbstractPluginModule()
        {
        }
        public abstract string Name { get; }

        public abstract string Version { get; }

        public abstract void RegisterAndConfigureServices(IServiceCollection services, ILogger logger);
    }
}
