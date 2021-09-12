using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Sayuru.Mobile.API.Data;
using Sayuru.Mobile.API.Data.Interfaces;
using Sayuru.Mobile.API.Data.Settings;
using Sayuru.Mobile.API.Filters;
using Sayuru.Mobile.API.Helpers;

namespace Sayuru.Mobile.API
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Sayuru API", Version = "v1" });
                options.AddSecurityDefinition("SayuruAPIKey", new OpenApiSecurityScheme
                {
                    Description = "SayuruAPIKey authentication",
                    In = ParameterLocation.Header,
                    Name = "SayuruAPIKey",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "SayuruAPIKey",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "SayuruAPIKey"
                            },
                         },
                         new string[] {}
                     }
                });
            });


            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));
            services.Configure<ApplicationSettings>(Configuration.GetSection(nameof(ApplicationSettings)));
            services.AddSingleton<IDatabaseSettings>(x => x.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton<IApplicationSettings>(x => x.GetRequiredService<IOptions<ApplicationSettings>>().Value);
            services.AddSingleton<IDBContext, DBContext>();
            services.AddSingleton<Logger>();
            services.AddSingleton<IVRApi>();
            services.AddSingleton<PushNotificationHelper>();
            services.AddSingleton<SMSHelper>();

            services.AddControllers();

            services.AddCors(options =>
                options.AddPolicy("AllowAll", p => p
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    ));

            services.AddTransient<APIKeyAuthenticate>();
            services.AddMvc(options =>
            {
                options.Filters.AddService<APIKeyAuthenticate>();
            }
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(config =>
                    config.SwaggerEndpoint("v1/swagger.json", "Sayuru API V1")
                );
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
