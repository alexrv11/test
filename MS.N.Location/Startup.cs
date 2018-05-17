using System;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Microsoft.ApplicationInsights.Extensibility;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Services.N.ATReference;
using BGBA.Services.N.Location;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace BGBA.MS.N.Location
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();
            services.AddScoped<IMapServices, GoogleMapsServices>();
            services.AddScoped<ISucursalServices, SucursalServices>();
            services.AddSingleton<IObjectFactory, BGBA.Models.N.Core.Utils.ObjectFactory.ObjectFactory>();
            services.AddScoped<ITableServices, TableRestServices>();
            services.AddScoped<TableHelper>();
            services.AddAutoMapper();

            BGBA.Models.N.Core.Microservices.Startup.ConfigureServices(services, Configuration);
        }

        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            BGBA.Models.N.Core.Microservices.Startup.Configure(app, env, loggerFactory, Configuration);
        }
    }
}
