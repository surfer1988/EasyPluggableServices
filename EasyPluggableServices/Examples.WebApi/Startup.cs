using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyPluggableServices.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Examples.WebApi
{
    public class Startup
    {
        private ILogger _logger;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        private IConfiguration Configuration { get; }

        private ILoggerFactory LoggerFactory { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.LoadServicesInPluginModule(LoggerFactory, new List<string>() { @"C:\Users\m.perdomini\source\repos\EasyPluggableServices\Examples.Plugin\bin\Debug\netcoreapp2.2\Examples.Plugin.dll" });

                var provider = services.BuildServiceProvider();

                Examples.Interfaces.ITest test = provider.GetService<Examples.Interfaces.ITest>();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(app.ApplicationServices.GetService<Examples.Interfaces.ITest>().Get());
            });
        }
    }
}
