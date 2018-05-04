using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using BGBA.Services.N.Afip;
using BGBA.Services.N.Client;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using Microsoft.ApplicationInsights.Extensibility;

namespace BGBA.MS.N.Client
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            BGBA.Models.N.Core.Microservices.Startup.ConfigureServices(services, _configuration);

            services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();
            services.AddScoped<IAfipServices, AfipServices>();
            services.AddAutoMapper(typeof(ClientProfiler).Assembly, typeof(BGBA.Models.N.Afip.AfipProfiler).Assembly);
            services.AddScoped<IObjectFactory, Models.N.Core.Utils.ObjectFactory.ObjectFactory>();
            services.AddScoped<IClientServices, ClientServices>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            BGBA.Models.N.Core.Microservices.Startup.Configure(app, env, loggerFactory, _configuration);
        }
    }
}
