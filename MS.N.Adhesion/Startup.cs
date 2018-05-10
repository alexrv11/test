using Microsoft.ApplicationInsights.Extensibility;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace BGBA.MS.N.Adhesion
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
            services.AddScoped<IObjectFactory, Models.N.Core.Utils.ObjectFactory.ObjectFactory>();

            Models.N.Core.Microservices.Startup.ConfigureServices(services, Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Models.N.Core.Microservices.Startup.Configure(app, env, loggerFactory, Configuration);
        }
    }
}
