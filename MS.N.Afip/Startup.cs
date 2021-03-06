﻿using System;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Microsoft.ApplicationInsights.Extensibility;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace BGBA.MS.N.Afip
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
            services.AddScoped<Services.N.Afip.IAfipServices, Services.N.Afip.AfipServices>();
            services.AddSingleton<IObjectFactory, Models.N.Core.Utils.ObjectFactory.ObjectFactory>();
            services.AddScoped<BGBA.Services.N.Afip.IAfipServices, BGBA.Services.N.Afip.AfipServices>();
            services.AddSingleton<IObjectFactory, Models.N.Core.Utils.ObjectFactory.ObjectFactory>();
            services.AddAutoMapper(typeof(Models.N.Afip.AfipProfiler).Assembly);
            
            services.AddSingleton<IMapper, Mapper>();
            services.AddSingleton(GetCertificate());

            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Consulta AFIP", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
            });
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
        }

        private X509Certificate2 GetCertificate()
        {
            return new X509Certificate2(Convert.FromBase64String(Configuration["Certificate:B64"]), Configuration["Certificate:Password"]);
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Afip API.");
            });
            app.UseMvc();
        }
    }
}
