using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using EasyPluggableDI.Core.Configs;

namespace EasyPluggableServices.Extension
{
    public static class EasyPluggableDIExtension
    {
        private static List<AbstractPluginModule> _modules;
        private static bool _initialize;

        public static void LoadServicesInPluginModule(this IServiceCollection services, ILoggerFactory loggerFactory, List<string> locationModules)
        {
            InternalLoadServicesInPlugin(services, loggerFactory, locationModules);
        }

        public static void LoadServicesInPluginModule(this IServiceCollection services, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            EasyPluggableDIConfig config = configuration.Get<EasyPluggableDIConfig>();
            InternalLoadServicesInPlugin(services, loggerFactory, config.Modules.Select(p => p.Location).ToList());
        }

        internal static void InternalLoadServicesInPlugin(IServiceCollection services, ILoggerFactory loggerFactory, List<string> locationModules)
        {
            ILogger logger = loggerFactory.CreateLogger(typeof(EasyPluggableDIExtension));

            if (_initialize)
                throw new InvalidOperationException("Plugin already loaded");

            _modules = new List<AbstractPluginModule>();

            logger.LogDebug($"start loading plugin modules");

            foreach (string location in locationModules)
            {
                try
                {
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(location);

                    Type type = assembly.GetTypes().Where(p => typeof(AbstractPluginModule).IsAssignableFrom(p)).SingleOrDefault();
                    if (type == null)
                        throw new Exception("AbstractPluginModule not found or found multiple");

                    AbstractPluginModule pluginModule = (AbstractPluginModule)Activator.CreateInstance(type);

                    pluginModule.RegisterAndConfigureServices(services, loggerFactory.CreateLogger(type));

                    _modules.Add(pluginModule);
                }
                catch (Exception ex)
                {
                    string msg = $"Unable to load module with location path: {location}";
                    logger.LogError(msg, ex);
                    throw new LoadException(msg, ex);
                }
            }

            _initialize = true;
        }
    }
}