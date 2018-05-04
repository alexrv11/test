using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Security.Cryptography.X509Certificates;
using System;
using BGBA.Services.N.Afip;
using BGBA.Services.N.Client;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using Microsoft.ApplicationInsights.Extensibility;

namespace BGBA.MS.N.Consulta.Cliente
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Consulta de cliente", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
            });

            services.AddSingleton(GetCertificate());
            services.AddScoped<IAfipServices, AfipServices>();
            services.AddAutoMapper(typeof(ClientProfiler).Assembly, typeof(BGBA.Models.N.Afip.AfipProfiler).Assembly);
            services.AddScoped<IObjectFactory, Models.N.Core.Utils.ObjectFactory.ObjectFactory>();
            services.AddScoped<IClientServices, ClientServices>();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
        }

        private X509Certificate2 GetCertificate()
        {
            return new X509Certificate2(Convert.FromBase64String(_configuration["Certificate:B64"]), _configuration["Certificate:Password"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole();

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                );

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Consulta de cliente API.");
            });
            app.UseMvc();
        }
    }
}
