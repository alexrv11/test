using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace BGBA.Models.N.Core.Microservices
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton(GetCertificate(configuration));

            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(configuration["Swagger:Version"], new Info { Title = configuration["Swagger:Title"], Version = configuration["Swagger:Version"] });
                c.DescribeAllEnumsAsStrings();
            });
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
        }

        private static X509Certificate2 GetCertificate(IConfiguration configuration)
        {
            return new X509Certificate2(Convert.FromBase64String(configuration["Certificate:B64"]), configuration["Certificate:Password"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug();
            }

            loggerFactory.AddConsole();

            app.UseMvc();
        }
    }
}
